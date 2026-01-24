using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Models;
using PCM.Api.Models.Bookings;
using PCM.Api.Models.Challenges;
using PCM.Api.Models.Courts;
using PCM.Api.Models.Matches;
using PCM.Api.Models.Members;

public class ApplicationDbContext
    : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Member> Members { get; set; }
    public DbSet<Court> Courts { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Challenge> Challenges { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    public DbSet<News> News { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Member>()
            .HasOne(m => m.Wallet)
            .WithOne(w => w.Member)
            .HasForeignKey<Wallet>(w => w.MemberId);

        // WalletTransaction - Member relationship (optional)
        modelBuilder.Entity<WalletTransaction>()
            .HasOne(t => t.Member)
            .WithMany()
            .HasForeignKey(t => t.MemberId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
