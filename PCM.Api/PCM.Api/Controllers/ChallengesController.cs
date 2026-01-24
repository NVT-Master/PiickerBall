using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Enums;
using PCM.Api.Models;
using PCM.Api.Models.Challenges;
using System.Security.Claims;

[ApiController]
[Route("api/challenges")]
public class ChallengesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ChallengesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] ChallengeStatus? status = null)
    {
        var query = _db.Challenges
            .Include(c => c.Participants)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(c => c.Status == status);

        var challenges = await query
            .OrderByDescending(c => c.CreatedDate)
            .ToListAsync();

        return Ok(challenges);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var challenge = await _db.Challenges
            .Include(c => c.Participants)
                .ThenInclude(p => p.Member)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (challenge == null)
            return NotFound("Không tìm thấy kèo đấu");

        return Ok(challenge);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateChallengeDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var member = await _db.Members.FirstOrDefaultAsync(m => m.UserId == userId);

        if (member == null)
            return Unauthorized("Bạn chưa có hồ sơ hội viên");

        var challenge = new Challenge
        {
            Title = dto.Title,
            Description = dto.Description,
            EntryFee = dto.EntryFee,
            PrizeAmount = dto.PrizeAmount,
            MaxParticipants = dto.MaxParticipants,
            MatchFormat = dto.MatchFormat,
            ChallengeType = dto.ChallengeType,
            ScheduledDate = dto.ScheduledDate,
            CreatedBy = member.Id,
            Status = ChallengeStatus.Open,
            CreatedDate = DateTime.UtcNow
        };

        _db.Challenges.Add(challenge);
        await _db.SaveChangesAsync();

        return Ok(challenge);
    }

    [HttpPost("{id}/join")]
    [Authorize]
    public async Task<IActionResult> Join(int id)
    {
        var challenge = await _db.Challenges
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (challenge == null)
            return NotFound("Không tìm thấy kèo đấu");

        if (challenge.Status != ChallengeStatus.Open)
            return BadRequest("Kèo đã đóng hoặc kết thúc");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var member = await _db.Members.FirstOrDefaultAsync(m => m.UserId == userId);

        if (member == null)
            return Unauthorized("Bạn chưa có hồ sơ hội viên");

        var existed = challenge.Participants?.Any(p => p.MemberId == member.Id) ?? false;
        if (existed)
            return BadRequest("Bạn đã tham gia kèo này rồi");

        // Kiểm tra số lượng
        var currentCount = challenge.Participants?.Count ?? 0;
        if (challenge.MaxParticipants.HasValue && currentCount >= challenge.MaxParticipants)
            return BadRequest("Kèo đã đủ người");

        var participant = new Participant
        {
            ChallengeId = id,
            MemberId = member.Id,
            EntryFeePaid = true,
            EntryFeeAmount = challenge.EntryFee,
            Status = "Confirmed",
            JoinedAt = DateTime.UtcNow
        };

        _db.Participants.Add(participant);

        // Nếu đủ người thì chuyển status
        if (challenge.MaxParticipants.HasValue && currentCount + 1 >= challenge.MaxParticipants)
        {
            challenge.Status = ChallengeStatus.Full;
        }

        await _db.SaveChangesAsync();

        return Ok(participant);
    }

    [HttpPost("{id}/leave")]
    [Authorize]
    public async Task<IActionResult> Leave(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var member = await _db.Members.FirstOrDefaultAsync(m => m.UserId == userId);

        if (member == null)
            return Unauthorized();

        var participant = await _db.Participants
            .FirstOrDefaultAsync(p => p.ChallengeId == id && p.MemberId == member.Id);

        if (participant == null)
            return BadRequest("Bạn chưa tham gia kèo này");

        _db.Participants.Remove(participant);
        await _db.SaveChangesAsync();

        return Ok("Đã rời kèo");
    }

    [HttpPost("{id}/finish")]
    [Authorize(Roles = "Admin,Referee")]
    public async Task<IActionResult> FinishChallenge(int id, [FromBody] FinishChallengeDto dto)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var challenge = await _db.Challenges
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (challenge == null)
                return NotFound("Không tìm thấy kèo đấu");

            if (challenge.Status == ChallengeStatus.Completed)
                return BadRequest("Kèo đã kết thúc");

            var participants = challenge.Participants?.ToList() ?? new List<Participant>();
            var winner = participants.FirstOrDefault(p => p.MemberId == dto.WinnerMemberId);

            if (winner == null)
                return BadRequest("Người thắng không hợp lệ");

            // Cập nhật ví người thắng
            var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.MemberId == dto.WinnerMemberId);
            if (wallet != null)
            {
                wallet.Balance += challenge.PrizeAmount;
                wallet.UpdatedAt = DateTime.UtcNow;
            }

            // Ghi nhận giao dịch
            _db.WalletTransactions.Add(new WalletTransaction
            {
                MemberId = dto.WinnerMemberId,
                Amount = challenge.PrizeAmount,
                Type = TransactionType.Income,
                Category = "Giải thưởng",
                Description = $"Giải thưởng kèo: {challenge.Title}",
                CreatedAt = DateTime.UtcNow
            });

            challenge.Status = ChallengeStatus.Completed;
            challenge.WinnerId = dto.WinnerMemberId;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok("Đã kết thúc kèo và phát thưởng");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var challenge = await _db.Challenges.FindAsync(id);
        if (challenge == null)
            return NotFound();

        challenge.Status = ChallengeStatus.Cancelled;
        await _db.SaveChangesAsync();

        return Ok("Đã hủy kèo");
    }
}

// DTOs
public class CreateChallengeDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal EntryFee { get; set; }
    public decimal PrizeAmount { get; set; }
    public int? MaxParticipants { get; set; }
    public MatchFormat MatchFormat { get; set; }
    public ChallengeType ChallengeType { get; set; }
    public DateTime? ScheduledDate { get; set; }
}

public class FinishChallengeDto
{
    public int WinnerMemberId { get; set; }
}