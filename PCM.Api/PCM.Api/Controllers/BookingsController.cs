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

        if (dto.StartTime < DateTime.UtcNow)
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

}
