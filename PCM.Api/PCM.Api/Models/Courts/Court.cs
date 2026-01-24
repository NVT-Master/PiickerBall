using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PCM.Api.Models.Courts;

[Table("025_Courts")]
public class Court
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public string? Description { get; set; }
}