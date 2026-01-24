using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.Api.Models;

[Table("025_News")]
public class News
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = null!;

    [Required]
    public string Content { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public bool IsPinned { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public string? AuthorId { get; set; }  // UserId của người viết
}
