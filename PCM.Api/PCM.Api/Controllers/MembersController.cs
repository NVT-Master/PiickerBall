using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Models;
using PCM.Api.Models.Members;

namespace PCM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly DbContext _context;

        public MembersController(DbContext context)
        {
            _context = context;
        }

        [HttpPost("{id}/wallet")]
        public async Task<IActionResult> CreateWallet(int id, decimal balance)
        {
            var member = await _context.Set<Member>()
                .Include(m => m.Wallet)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
                return NotFound("Member not found");

            if (member.Wallet != null)
                return BadRequest("Wallet already exists");

            member.Wallet = new Wallet
            {
                Balance = balance,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.SaveChangesAsync();
            return Ok("Wallet created");
        }
    }
}
