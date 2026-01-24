using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PCM.Api.Models.Members;

[Table("025_Members")]
public class Member
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string FullName { get; set; } = null!;

    public DateTime JoinDate { get; set; } = DateTime.UtcNow;

    public double RankLevel { get; set; } = 3.0;

    public bool IsActive { get; set; } = true;

    // Liên kết Identity
    [Required]
    public string UserId { get; set; } = null!;

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public int TotalMatches { get; set; }
    public int WinMatches { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
    public Wallet? Wallet { get; set; }

}

