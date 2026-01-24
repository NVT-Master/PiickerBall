using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Enums;
using PCM.Api.Models.Matches;
using System.Security.Claims;

namespace PCM.Api.Controllers;

[ApiController]
[Route("api/matches")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public MatchesController(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Lấy tất cả trận đấu
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isRanked = null)
    {
        var query = _db.Matches.AsQueryable();

        if (isRanked.HasValue)
            query = query.Where(m => m.IsRanked == isRanked);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(m => m.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Lấy thông tin tên người chơi
        var playerIds = items
            .SelectMany(m => new[] { m.Team1_Player1Id, m.Team1_Player2Id, m.Team2_Player1Id, m.Team2_Player2Id })
            .Where(id => id.HasValue || id > 0)
            .Select(id => id ?? 0)
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        var players = await _db.Members
            .Where(m => playerIds.Contains(m.Id))
            .Select(m => new { m.Id, m.FullName })
            .ToDictionaryAsync(m => m.Id, m => m.FullName);

        var result = items.Select(m => new
        {
            m.Id,
            m.Date,
            m.IsRanked,
            m.MatchFormat,
            m.WinningSide,
            m.ChallengeId,
            Team1 = new
            {
                Player1 = players.GetValueOrDefault(m.Team1_Player1Id),
                Player2 = m.Team1_Player2Id.HasValue ? players.GetValueOrDefault(m.Team1_Player2Id.Value) : null
            },
            Team2 = new
            {
                Player1 = players.GetValueOrDefault(m.Team2_Player1Id),
                Player2 = m.Team2_Player2Id.HasValue ? players.GetValueOrDefault(m.Team2_Player2Id.Value) : null
            }
        });

        return Ok(new
        {
            items = result,
            total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    /// <summary>
    /// Lấy chi tiết trận đấu
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var match = await _db.Matches.FindAsync(id);
        if (match == null)
            return NotFound("Không tìm thấy trận đấu");

        return Ok(match);
    }

    /// <summary>
    /// Ghi nhận kết quả trận đấu (Referee/Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Referee")]
    public async Task<IActionResult> Create([FromBody] CreateMatchDto dto)
    {
        // Validate players
        var playerIds = new List<int> { dto.Team1_Player1Id, dto.Team2_Player1Id };
        if (dto.Team1_Player2Id.HasValue) playerIds.Add(dto.Team1_Player2Id.Value);
        if (dto.Team2_Player2Id.HasValue) playerIds.Add(dto.Team2_Player2Id.Value);

        // Kiểm tra không có người chơi trùng
        if (playerIds.Distinct().Count() != playerIds.Count)
            return BadRequest("Không thể có người chơi xuất hiện ở cả 2 đội");

        // Kiểm tra tất cả players tồn tại
        var existingPlayers = await _db.Members
            .Where(m => playerIds.Contains(m.Id))
            .Select(m => m.Id)
            .ToListAsync();

        if (existingPlayers.Count != playerIds.Count)
            return BadRequest("Một số người chơi không tồn tại");

        var match = new Match
        {
            Date = dto.Date ?? DateTime.UtcNow,
            IsRanked = dto.IsRanked,
            MatchFormat = dto.MatchFormat,
            ChallengeId = dto.ChallengeId,
            Team1_Player1Id = dto.Team1_Player1Id,
            Team1_Player2Id = dto.Team1_Player2Id,
            Team2_Player1Id = dto.Team2_Player1Id,
            Team2_Player2Id = dto.Team2_Player2Id,
            WinningSide = dto.WinningSide
        };

        _db.Matches.Add(match);

        // Cập nhật thống kê cho players nếu là trận xếp hạng
        if (match.IsRanked && match.WinningSide != WinningSide.None)
        {
            var winnerIds = match.WinningSide == WinningSide.Team1
                ? new List<int> { match.Team1_Player1Id, match.Team1_Player2Id ?? 0 }.Where(id => id > 0)
                : new List<int> { match.Team2_Player1Id, match.Team2_Player2Id ?? 0 }.Where(id => id > 0);

            var loserIds = match.WinningSide == WinningSide.Team1
                ? new List<int> { match.Team2_Player1Id, match.Team2_Player2Id ?? 0 }.Where(id => id > 0)
                : new List<int> { match.Team1_Player1Id, match.Team1_Player2Id ?? 0 }.Where(id => id > 0);

            // Update winners
            await _db.Members
                .Where(m => winnerIds.Contains(m.Id))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(m => m.TotalMatches, m => m.TotalMatches + 1)
                    .SetProperty(m => m.WinMatches, m => m.WinMatches + 1));

            // Update losers
            await _db.Members
                .Where(m => loserIds.Contains(m.Id))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(m => m.TotalMatches, m => m.TotalMatches + 1));
        }

        await _db.SaveChangesAsync();

        return Ok(match);
    }

    /// <summary>
    /// Cập nhật kết quả trận đấu
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Referee")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMatchDto dto)
    {
        var match = await _db.Matches.FindAsync(id);
        if (match == null)
            return NotFound("Không tìm thấy trận đấu");

        if (dto.WinningSide.HasValue)
            match.WinningSide = dto.WinningSide.Value;

        await _db.SaveChangesAsync();
        return Ok(match);
    }

    /// <summary>
    /// Xóa trận đấu
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var match = await _db.Matches.FindAsync(id);
        if (match == null)
            return NotFound("Không tìm thấy trận đấu");

        _db.Matches.Remove(match);
        await _db.SaveChangesAsync();

        return Ok("Đã xóa trận đấu");
    }

    /// <summary>
    /// Lấy lịch sử trận đấu của member
    /// </summary>
    [HttpGet("member/{memberId}")]
    public async Task<IActionResult> GetByMember(int memberId)
    {
        var matches = await _db.Matches
            .Where(m => m.Team1_Player1Id == memberId
                || m.Team1_Player2Id == memberId
                || m.Team2_Player1Id == memberId
                || m.Team2_Player2Id == memberId)
            .OrderByDescending(m => m.Date)
            .Take(20)
            .ToListAsync();

        return Ok(matches);
    }
}

// DTOs
public class CreateMatchDto
{
    public DateTime? Date { get; set; }
    public bool IsRanked { get; set; }
    public MatchFormat MatchFormat { get; set; }
    public int? ChallengeId { get; set; }
    public int Team1_Player1Id { get; set; }
    public int? Team1_Player2Id { get; set; }
    public int Team2_Player1Id { get; set; }
    public int? Team2_Player2Id { get; set; }
    public WinningSide WinningSide { get; set; }
}

public class UpdateMatchDto
{
    public WinningSide? WinningSide { get; set; }
}
