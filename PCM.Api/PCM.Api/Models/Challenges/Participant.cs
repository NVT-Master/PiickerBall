using PCM.Api.Models.Challenges;
using PCM.Api.Models.Members;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PCM.Api.Models.Challenges;

[Table("025_Participants")]
public class Participant
{
    [Key]
    public int Id { get; set; }

    public int ChallengeId { get; set; }

    [ForeignKey(nameof(ChallengeId))]
    [JsonIgnore]
    public Challenge? Challenge { get; set; }

    public int MemberId { get; set; }

    [ForeignKey(nameof(MemberId))]
    public Member? Member { get; set; }

    public string Team { get; set; } = "None"; // TeamA / TeamB / None

    public bool EntryFeePaid { get; set; }
    public decimal EntryFeeAmount { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending"; // Pending / Confirmed / Withdrawn
}
