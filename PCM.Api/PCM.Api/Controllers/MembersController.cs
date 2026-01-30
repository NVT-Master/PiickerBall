using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Models;
using PCM.Api.Models.Members;
using System.Security.Claims;

namespace PCM.Api.Controllers
{
    [ApiController]
    [Route("api/members")]
    [Authorize]
    public class MembersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public MembersController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Lấy danh sách tất cả members
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var query = _db.Members
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.RankLevel)
                .ThenBy(m => m.FullName);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                items,
                total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        }

        /// <summary>
        /// Lấy thông tin member theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var member = await _db.Members
                .Include(m => m.Wallet)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
                return NotFound("Không tìm thấy thành viên");

            return Ok(member);
        }

        /// <summary>
        /// Lấy thông tin member hiện tại (đang đăng nhập)
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var member = await _db.Members
                .Include(m => m.Wallet)
                .FirstOrDefaultAsync(m => m.UserId == userId);

            if (member == null)
                return NotFound("Bạn chưa có hồ sơ hội viên");

            return Ok(member);
        }

        /// <summary>
        /// Tạo member mới (Admin)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateMemberDto dto)
        {
            var member = new Member
            {
                FullName = dto.FullName,
                UserId = dto.UserId,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                DateOfBirth = dto.DateOfBirth,
                RankLevel = dto.RankLevel
            };

            _db.Members.Add(member);
            await _db.SaveChangesAsync();

            return Ok(member);
        }

        /// <summary>
        /// Cập nhật thông tin member
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMemberDto dto)
        {
            var member = await _db.Members.FindAsync(id);
            if (member == null)
                return NotFound("Không tìm thấy thành viên");

            // Kiểm tra quyền: chỉ admin hoặc chính chủ mới được sửa
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && member.UserId != userId)
                return Forbid();

            member.FullName = dto.FullName ?? member.FullName;
            member.PhoneNumber = dto.PhoneNumber ?? member.PhoneNumber;
            member.DateOfBirth = dto.DateOfBirth ?? member.DateOfBirth;
            member.ModifiedDate = DateTime.UtcNow;

            // Chỉ admin mới được cập nhật rank
            if (isAdmin && dto.RankLevel.HasValue)
            {
                member.RankLevel = dto.RankLevel.Value;
            }

            await _db.SaveChangesAsync();
            return Ok(member);
        }

        /// <summary>
        /// Xóa member (soft delete - Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _db.Members.FindAsync(id);
            if (member == null)
                return NotFound("Không tìm thấy thành viên");

            member.IsActive = false;
            member.ModifiedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok("Đã xóa thành viên");
        }

        /// <summary>
        /// Lấy bảng xếp hạng top members
        /// </summary>
        [HttpGet("ranking")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRanking([FromQuery] int top = 10)
        {
            var ranking = await _db.Members
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.RankLevel)
                .ThenByDescending(m => m.WinMatches)
                .Take(top)
                .Select(m => new
                {
                    m.Id,
                    m.FullName,
                    m.RankLevel,
                    m.TotalMatches,
                    m.WinMatches,
                    WinRate = m.TotalMatches > 0 ? (double)m.WinMatches / m.TotalMatches * 100 : 0
                })
                .ToListAsync();

            return Ok(ranking);
        }

        /// <summary>
        /// Lấy bảng xếp hạng top members (alias cho frontend)
        /// </summary>
        [HttpGet("top-ranking")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopRanking([FromQuery] int limit = 5)
        {
            var ranking = await _db.Members
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.RankLevel)
                .ThenByDescending(m => m.WinMatches)
                .Take(limit)
                .Select(m => new
                {
                    m.Id,
                    m.FullName,
                    m.RankLevel,
                    m.TotalMatches,
                    m.WinMatches,
                    WinRate = m.TotalMatches > 0 ? (double)m.WinMatches / m.TotalMatches * 100 : 0
                })
                .ToListAsync();

            return Ok(ranking);
        }

        /// <summary>
        /// Tạo ví cho member
        /// </summary>
        [HttpPost("{id}/wallet")]
        [Authorize(Roles = "Admin,Treasurer")]
        public async Task<IActionResult> CreateWallet(int id, [FromBody] CreateWalletDto dto)
        {
            var member = await _db.Members
                .Include(m => m.Wallet)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
                return NotFound("Không tìm thấy thành viên");

            if (member.Wallet != null)
                return BadRequest("Thành viên đã có ví");

            member.Wallet = new Wallet
            {
                Balance = dto.Balance,
                UpdatedAt = DateTime.UtcNow
            };

            await _db.SaveChangesAsync();
            return Ok(member.Wallet);
        }
    }

    // DTOs
    public class CreateMemberDto
    {
        public string FullName { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public double RankLevel { get; set; } = 3.0;
    }

    public class UpdateMemberDto
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public double? RankLevel { get; set; }
    }

    public class CreateWalletDto
    {
        public decimal Balance { get; set; } = 0;
    }
}
