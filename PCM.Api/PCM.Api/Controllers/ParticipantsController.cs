using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Models;
using PCM.Api.Models.Members;
using System;

namespace PCM.Api.Controllers
{
    [ApiController]
    [Route("api/participants")]
    public class ParticipantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParticipantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("join")]
        public async Task<IActionResult> Join(int memberId, int challengeId)
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var member = await _context.Members
                    .Include(m => m.Wallet)
                    .FirstOrDefaultAsync(m => m.Id == memberId);

                if (member == null)
                    return NotFound("Member not found");

                var challenge = await _context.Challenges.FindAsync(challengeId);
                if (challenge == null)
                    return NotFound("Challenge not found");

                var exists = await _context.Participants
                    .AnyAsync(p => p.MemberId == memberId && p.ChallengeId == challengeId);

                if (exists)
                    return BadRequest("Member already joined");

                if (challenge.EntryFee > 0)
                {
                    if (member.Wallet == null)
                        return BadRequest("Wallet not found");

                    if (member.Wallet.Balance < challenge.EntryFee)
                        return BadRequest("Insufficient balance");

                    member.Wallet.Balance -= challenge.EntryFee;
                    member.Wallet.UpdatedAt = DateTime.UtcNow;

                    _context.WalletTransactions.Add(new WalletTransaction
                    {
                        MemberId = memberId,
                        Amount = -challenge.EntryFee,
                        Type = "ENTRY_FEE",
                        Description = $"Join challenge {challenge.Title}",
                        CreatedAt = DateTime.UtcNow
                    });
                }

                _context.Participants.Add(new Participant
                {
                    MemberId = memberId,
                    ChallengeId = challengeId,
                    EntryFeePaid = challenge.EntryFee > 0,
                    EntryFeeAmount = challenge.EntryFee,
                    JoinedDate = DateTime.UtcNow,
                    Status = "Joined"
                });

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return Ok("Joined challenge successfully");
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }
    }
}