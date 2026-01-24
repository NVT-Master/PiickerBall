using Microsoft.AspNetCore.Identity;
using PCM.Api.Models.Members;

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
    }
}
