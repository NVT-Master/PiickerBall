using PCM.Api.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PCM.Api.Models.Matches;

[Table("025_Matches")]
public class Match
{
    [Key]
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public bool IsRanked { get; set; }

    public int? ChallengeId { get; set; }

    public MatchFormat MatchFormat { get; set; }

    public int Team1_Player1Id { get; set; }
    public int? Team1_Player2Id { get; set; }

    public int Team2_Player1Id { get; set; }
    public int? Team2_Player2Id { get; set; }

    public WinningSide WinningSide { get; set; } = WinningSide.None;
}

