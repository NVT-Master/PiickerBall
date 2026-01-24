using PCM.Api.Enums;
using PCM.Api.Models.Courts;
using PCM.Api.Models.Members;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PCM.Api.Models.Bookings;

[Table("025_Bookings")]
public class Booking
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CourtId { get; set; }

    [ForeignKey(nameof(CourtId))]
    public Court Court { get; set; } = null!;

    [Required]
    public int MemberId { get; set; }

    [ForeignKey(nameof(MemberId))]
    public Member Member { get; set; } = null!;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
