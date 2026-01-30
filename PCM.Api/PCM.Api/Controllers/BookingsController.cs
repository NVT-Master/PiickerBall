using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Api.DTOs.Bookings;
using PCM.Api.Enums;
using PCM.Api.Models.Bookings;
using System.Security.Claims;
namespace PCM.Api.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public BookingsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingDto dto)
    {
        if (dto.EndTime <= dto.StartTime)
            return BadRequest("EndTime phải lớn hơn StartTime");

        if (dto.StartTime < DateTime.Now)
            return BadRequest("Không được đặt sân trong quá khứ");

        var isConflict = _db.Bookings.Any(b =>
            b.CourtId == dto.CourtId &&
            b.Status != BookingStatus.Cancelled &&
            dto.StartTime < b.EndTime &&
            dto.EndTime > b.StartTime
        );

        if (isConflict)
            return BadRequest("Khung giờ này đã có người đặt");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var member = _db.Members.FirstOrDefault(m => m.UserId == userId);

        if (member == null)
            return Unauthorized("Không tìm thấy hội viên");

        var booking = new Booking
        {
            CourtId = dto.CourtId,
            MemberId = member.Id,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Notes = dto.Notes,
            Status = BookingStatus.Confirmed
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        return Ok(booking);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var bookings = _db.Bookings
            .OrderByDescending(b => b.StartTime)
            .ToList();

        return Ok(bookings);
    }

    [HttpGet("my-bookings")]
    public IActionResult GetMyBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var memberId = _db.Members
            .Where(m => m.UserId == userId)
            .Select(m => m.Id)
            .FirstOrDefault();

        var bookings = _db.Bookings
            .Where(b => b.MemberId == memberId)
            .OrderByDescending(b => b.StartTime)
            .ToList();

        return Ok(bookings);
    }

    [HttpGet("available-slots")]
    public IActionResult GetAvailableSlots(int courtId, DateTime date)
    {
        var openTime = date.Date.AddHours(6);
        var closeTime = date.Date.AddHours(22);
        var slotDuration = TimeSpan.FromHours(1);

        var bookings = _db.Bookings
            .Where(b => b.CourtId == courtId &&
                        b.StartTime.Date == date.Date &&
                        b.Status != BookingStatus.Cancelled)
            .ToList();

        var slots = new List<AvailableSlotDto>();
        var current = openTime;

        while (current + slotDuration <= closeTime)
        {
            var slotEnd = current + slotDuration;

            var conflict = bookings.Any(b =>
                current < b.EndTime && slotEnd > b.StartTime);

            if (!conflict)
            {
                slots.Add(new AvailableSlotDto
                {
                    StartTime = current,
                    EndTime = slotEnd
                });
            }

            current = slotEnd;
        }

        return Ok(slots);
    }

    /// <summary>
    /// Lấy lịch đặt sân theo tuần (calendar view)
    /// </summary>
    [HttpGet("calendar")]
    public IActionResult GetCalendar(
        [FromQuery] int? courtId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var query = _db.Bookings.AsQueryable();

        // Filter by court
        if (courtId.HasValue)
            query = query.Where(b => b.CourtId == courtId);

        // Filter by date range
        if (startDate.HasValue)
            query = query.Where(b => b.StartTime >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(b => b.StartTime <= endDate.Value.AddDays(1));

        var bookings = query
            .Where(b => b.Status != BookingStatus.Cancelled)
            .OrderBy(b => b.StartTime)
            .Select(b => new
            {
                b.Id,
                b.CourtId,
                b.MemberId,
                b.StartTime,
                b.EndTime,
                b.Status,
                b.Notes,
                Member = _db.Members
                    .Where(m => m.Id == b.MemberId)
                    .Select(m => new { m.Id, m.FullName })
                    .FirstOrDefault(),
                Court = _db.Courts
                    .Where(c => c.Id == b.CourtId)
                    .Select(c => new { c.Id, c.Name })
                    .FirstOrDefault()
            })
            .ToList();

        return Ok(bookings);
    }

    /// <summary>
    /// Xóa booking (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var booking = await _db.Bookings.FindAsync(id);
        
        if (booking == null)
            return NotFound("Không tìm thấy booking");

        _db.Bookings.Remove(booking);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Đã xóa booking thành công" });
    }

    /// <summary>
    /// Lấy thống kê booking
    /// </summary>
    [HttpGet("statistics")]
    [AllowAnonymous]
    public IActionResult GetStatistics()
    {
        var today = DateTime.Today;
        var todayBookings = _db.Bookings
            .Count(b => b.StartTime.Date == today && b.Status != BookingStatus.Cancelled);

        var totalBookings = _db.Bookings
            .Count(b => b.Status != BookingStatus.Cancelled);

        return Ok(new
        {
            todayBookings,
            totalBookings
        });
    }

}
