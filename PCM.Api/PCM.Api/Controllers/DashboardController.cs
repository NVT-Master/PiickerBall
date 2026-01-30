using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Enums;

namespace PCM.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Test endpoint - không cần database
        /// </summary>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Dashboard API working!", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Lấy thống kê tổng quan cho Dashboard (không cần đăng nhập)
        /// </summary>
        [HttpGet("statistics")]
        public IActionResult GetStatistics()
        {
            var today = DateTime.Today;
            
            // Thống kê thành viên
            var totalMembers = _db.Members.Count(m => m.IsActive);
            
            // Thống kê booking hôm nay
            var todayBookings = _db.Bookings
                .Count(b => b.StartTime.Date == today && b.Status != BookingStatus.Cancelled);

            // Thống kê kèo đang mở
            var openChallenges = _db.Challenges
                .Count(c => c.Status == ChallengeStatus.Open || c.Status == ChallengeStatus.Full);

            // Tổng thu/chi quỹ
            var totalIncome = _db.WalletTransactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            var totalExpense = _db.WalletTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            return Ok(new
            {
                totalMembers,
                todayBookings,
                openChallenges,
                treasury = new
                {
                    totalIncome,
                    totalExpense,
                    balance = totalIncome - totalExpense
                }
            });
        }
    }
}
