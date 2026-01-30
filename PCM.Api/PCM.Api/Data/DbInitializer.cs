using Microsoft.AspNetCore.Identity;
using PCM.Api.Models.Members;
using PCM.Api.Models.Courts;
using PCM.Api.Models.Bookings;
using PCM.Api.Models.Challenges;
using PCM.Api.Models.Matches;
using PCM.Api.Models;
using PCM.Api.Enums;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var db = services.GetRequiredService<ApplicationDbContext>();

        string[] roles = { "Admin", "Member", "Treasurer", "Referee" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Admin
        var adminEmail = "admin@pcm.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };

            await userManager.CreateAsync(admin, "Admin@123");
            await userManager.AddToRoleAsync(admin, "Admin");

            db.Members.Add(new Member
            {
                FullName = "PCM Admin",
                UserId = admin.Id,
                Email = adminEmail,
                RankLevel = 5.0
            });

            await db.SaveChangesAsync();
        }

        // Seed sample data if database is empty
        await SeedSampleDataAsync(db, userManager);
    }

    private static async Task SeedSampleDataAsync(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        // Check if data already exists
        if (db.Members.Count() > 1)
            return;

        // Create sample users and members
        var members = new List<Member>();
        for (int i = 1; i <= 25; i++)
        {
            var email = $"member{i}@pcm.com";
            var user = await userManager.FindByEmailAsync(email);
            
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "Member@123");
                await userManager.AddToRoleAsync(user, "Member");
            }

            var member = new Member
            {
                FullName = $"Thành viên {i}",
                UserId = user.Id,
                Email = email,
                PhoneNumber = $"09{i:00}123456",
                DateOfBirth = DateTime.Now.AddYears(-20 - i),
                JoinDate = DateTime.Now.AddMonths(-i),
                RankLevel = 1.0 + (i % 10) * 0.5,
                TotalMatches = i * 2,
                WinMatches = i,
                IsActive = true
            };
            
            db.Members.Add(member);
            members.Add(member);
        }
        await db.SaveChangesAsync();

        // Create courts
        var courts = new List<Court>
        {
            new Court { Name = "Sân 1", Description = "Sân chính, ánh sáng tốt", IsActive = true },
            new Court { Name = "Sân 2", Description = "Sân phụ", IsActive = true },
            new Court { Name = "Sân 3", Description = "Sân VIP, có điều hòa", IsActive = true }
        };
        db.Courts.AddRange(courts);
        await db.SaveChangesAsync();

        // Create bookings
        var random = new Random();
        for (int i = 0; i < 8; i++)
        {
            var member = members[random.Next(members.Count)];
            var court = courts[random.Next(courts.Count)];
            var startTime = DateTime.Now.AddDays(i - 3).AddHours(random.Next(8, 20));

            db.Bookings.Add(new Booking
            {
                MemberId = member.Id,
                CourtId = court.Id,
                StartTime = startTime,
                EndTime = startTime.AddHours(2),
                Status = i < 3 ? BookingStatus.Pending : (i < 6 ? BookingStatus.Confirmed : BookingStatus.Cancelled),
                Notes = $"Đặt sân cho trận đấu {i + 1}"
            });
        }
        await db.SaveChangesAsync();

        // Create challenges
        for (int i = 0; i < 5; i++)
        {
            var creator = members[random.Next(members.Count)];
            db.Challenges.Add(new Challenge
            {
                Title = $"Giải đấu {i + 1}",
                Description = $"Mô tả giải đấu {i + 1}",
                ChallengeType = (ChallengeType)(i % 3),
                MatchFormat = (MatchFormat)(i % 2),
                MaxParticipants = 8 + (i * 2),
                EntryFee = 50000 + (i * 10000),
                PrizeAmount = 200000 + (i * 50000),
                ScheduledDate = DateTime.Now.AddDays(7 + i),
                Status = i < 3 ? ChallengeStatus.Open : ChallengeStatus.Ongoing,
                CreatedBy = creator.Id,
                CreatedDate = DateTime.Now.AddDays(-i)
            });
        }
        await db.SaveChangesAsync();

        // Create matches
        for (int i = 0; i < 10; i++)
        {
            var player1 = members[random.Next(members.Count)];
            var player2 = members[random.Next(members.Count)];
            var player3 = members[random.Next(members.Count)];
            var player4 = members[random.Next(members.Count)];

            db.Matches.Add(new Match
            {
                Date = DateTime.Now.AddDays(-i),
                IsRanked = i % 2 == 0,
                MatchFormat = (MatchFormat)(i % 2),
                Team1_Player1Id = player1.Id,
                Team1_Player2Id = i % 2 == 0 ? player2.Id : null,
                Team2_Player1Id = player3.Id,
                Team2_Player2Id = i % 2 == 0 ? player4.Id : null,
                WinningSide = (WinningSide)(random.Next(1, 3))
            });
        }
        await db.SaveChangesAsync();

        // Create wallet transactions
        db.WalletTransactions.AddRange(
            new WalletTransaction
            {
                Amount = 500000,
                Type = TransactionType.Income,
                Category = "Phí thành viên",
                Description = "Thu phí thành viên tháng 1",
                CreatedAt = DateTime.Now.AddDays(-20)
            },
            new WalletTransaction
            {
                Amount = 300000,
                Type = TransactionType.Income,
                Category = "Phí đặt sân",
                Description = "Thu tiền đặt sân",
                CreatedAt = DateTime.Now.AddDays(-15)
            },
            new WalletTransaction
            {
                Amount = 200000,
                Type = TransactionType.Expense,
                Category = "Mua thiết bị",
                Description = "Mua vợt và bóng",
                CreatedAt = DateTime.Now.AddDays(-10)
            },
            new WalletTransaction
            {
                Amount = 150000,
                Type = TransactionType.Expense,
                Category = "Chi phí sân",
                Description = "Tiền thuê sân tháng 1",
                CreatedAt = DateTime.Now.AddDays(-5)
            }
        );
        await db.SaveChangesAsync();

        // Create news
        db.News.AddRange(
            new News
            {
                Title = "Thông báo giải đấu mùa xuân 2026",
                Content = "CLB thông báo sẽ tổ chức giải đấu mùa xuân vào tháng 3/2026. Đăng ký tham gia trước ngày 15/2.",
                IsPinned = true,
                CreatedAt = DateTime.Now.AddDays(-5)
            },
            new News
            {
                Title = "Lịch tập tuần này",
                Content = "Lịch tập: Thứ 2, 4, 6 từ 18h-20h tại sân 1",
                IsPinned = false,
                CreatedAt = DateTime.Now.AddDays(-3)
            }
        );
        await db.SaveChangesAsync();
    }
}
