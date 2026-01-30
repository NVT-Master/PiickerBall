using PCM.Api.Enums;
using PCM.Api.Models.Members;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.Api.Models.Challenges;

[Table("025_Challenges")]
public class Challenge
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public ChallengeType ChallengeType { get; set; }
    public MatchFormat MatchFormat { get; set; }

    public ChallengeStatus Status { get; set; } = ChallengeStatus.Open;

    public int? MaxParticipants { get; set; }

    public decimal EntryFee { get; set; }
    public decimal PrizeAmount { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    [ForeignKey(nameof(CreatedBy))]
    public Member? Creator { get; set; }

    public int? WinnerId { get; set; }

    [ForeignKey(nameof(WinnerId))]
    public Member? Winner { get; set; }

    public DateTime? ScheduledDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }

    // Navigation property
    public ICollection<Participant>? Participants { get; set; }
}
