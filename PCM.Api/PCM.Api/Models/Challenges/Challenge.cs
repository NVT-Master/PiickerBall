using PCM.Api.Enums;
using PCM.Api.Models.Members;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("025_Challenges")]
public class Challenge
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    public ChallengeType Type { get; set; }
    public GameMode GameMode { get; set; }
    private ChallengeStatus status = ChallengeStatus.Open;

    public ChallengeStatus GetStatus()
    {
        return status;
    }

    public void SetStatus(ChallengeStatus value)
    {
        status = value;
    }

    public int? Config_TargetWins { get; set; }

    public int CurrentScore_TeamA { get; set; }
    public int CurrentScore_TeamB { get; set; }

    public decimal EntryFee { get; set; }
    public decimal PrizePool { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    [ForeignKey(nameof(CreatedBy))]
    public Member Creator { get; set; } = null!;

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }

    public decimal PrizeAmount { get; set; }
    public string Status { get; set; } = "Open"; // Open | Finished
    public int? WinnerId { get; set; }

    // Navigation property
    public ICollection<Participant> Participants { get; set; } = new List<Participant>();
}
