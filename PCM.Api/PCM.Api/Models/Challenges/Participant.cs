using PCM.Api.Models.Members;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("025_Participants")]
public class Participant
{
    [Key]
    public int Id { get; set; }

    public int ChallengeId { get; set; }

    [ForeignKey(nameof(ChallengeId))]
    public Challenge Challenge { get; set; } = null!;

    public int MemberId { get; set; }

    [ForeignKey(nameof(MemberId))]
    public Member Member { get; set; } = null!;

    public string Team { get; set; } = "None"; // TeamA / TeamB / None

    public bool EntryFeePaid { get; set; }
    public decimal EntryFeeAmount { get; set; }

    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending"; // Pending / Confirmed / Withdrawn
}
