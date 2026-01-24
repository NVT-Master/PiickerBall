using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Enums;
using PCM.Api.Models;
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
    public IActionResult GetAll()
    {
        return Ok(_db.Challenges.OrderByDescending(c => c.CreatedDate).ToList());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Challenge challenge)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var member = _db.Members.First(m => m.UserId == userId);

        challenge.CreatedBy = member.Id;
        challenge.SetStatus(ChallengeStatus.Open);

        _db.Challenges.Add(challenge);
        await _db.SaveChangesAsync();

        return Ok(challenge);
    }

    [Authorize(Roles = "Member")]
    [HttpPost("{id}/join")]
    public async Task<IActionResult> Join(int id)
    {
        var challenge = await _db.Challenges.FindAsync(id);
        if (challenge == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var member = _db.Members.First(m => m.UserId == userId);

        var existed = _db.Participants
            .Any(p => p.ChallengeId == id && p.MemberId == member.Id);

        if (existed)
            return BadRequest("Đã tham gia kèo này");

        var participant = new Participant
        {
            ChallengeId = id,
            MemberId = member.Id,
            EntryFeePaid = true,
            EntryFeeAmount = challenge.EntryFee,
            Status = "Confirmed"
        };

        _db.Participants.Add(participant);
        await _db.SaveChangesAsync();

        return Ok(participant);
    }

    [HttpPost("{id}/finish")]
    public async Task<IActionResult> FinishChallenge(int id, int winnerMemberId)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var challenge = await _db.Challenges
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (challenge == null)
                return NotFound("Challenge not found");

            if (challenge.Status == "Finished")
                return BadRequest("Challenge already finished");

            // Fix: Cast Participants to IEnumerable<Participant> before using LINQ
            var participants = challenge.Participants as IEnumerable<Participant>;
            if (participants == null)
                return BadRequest("Participants not loaded");

            var winner = participants
                .FirstOrDefault(p => p.MemberId == winnerMemberId && p.Status == "Joined");

            if (winner == null)
                return BadRequest("Winner not valid");

            var wallet = await _db.Wallets
                .FirstOrDefaultAsync(w => w.MemberId == winnerMemberId);

            if (wallet == null)
                return BadRequest("Winner wallet not found");

            wallet.Balance += challenge.PrizeAmount;
            wallet.UpdatedAt = DateTime.UtcNow;

            _db.WalletTransactions.Add(new WalletTransaction
            {
                MemberId = winnerMemberId,
                Amount = challenge.PrizeAmount,
                Type = "PRIZE",
                Description = $"Prize for challenge {challenge.Title}",
                CreatedAt = DateTime.UtcNow
            });

            challenge.Status = "Finished";
            challenge.WinnerId = winnerMemberId;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok("Challenge finished and prize distributed");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpGet("{id}/ranking")]
    public async Task<IActionResult> GetRanking(int id)
    {
        var ranking = await _db.Participants
            .Where(p => p.ChallengeId == id && p.Status == "Joined")
            .Include(p => p.Member)
            .Select(p => new
            {
                p.MemberId,
                p.Member.FullName
            })
            .ToListAsync();

        return Ok(ranking);
    }
}