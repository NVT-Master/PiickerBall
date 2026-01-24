using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Models;
using System.Security.Claims;

namespace PCM.Api.Controllers;

[ApiController]
[Route("api/news")]
public class NewsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public NewsController(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Lấy tất cả tin tức (public)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = _db.News.OrderByDescending(n => n.IsPinned).ThenByDescending(n => n.CreatedAt);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            items,
            total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    /// <summary>
    /// Lấy chi tiết tin tức
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var news = await _db.News.FindAsync(id);
        if (news == null)
            return NotFound("Không tìm thấy tin tức");

        return Ok(news);
    }

    /// <summary>
    /// Tạo tin tức mới (Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateNewsDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var news = new News
        {
            Title = dto.Title,
            Content = dto.Content,
            ImageUrl = dto.ImageUrl,
            IsPinned = dto.IsPinned,
            AuthorId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _db.News.Add(news);
        await _db.SaveChangesAsync();

        return Ok(news);
    }

    /// <summary>
    /// Cập nhật tin tức
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateNewsDto dto)
    {
        var news = await _db.News.FindAsync(id);
        if (news == null)
            return NotFound("Không tìm thấy tin tức");

        news.Title = dto.Title ?? news.Title;
        news.Content = dto.Content ?? news.Content;
        news.ImageUrl = dto.ImageUrl ?? news.ImageUrl;
        news.IsPinned = dto.IsPinned ?? news.IsPinned;
        news.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(news);
    }

    /// <summary>
    /// Toggle ghim tin tức
    /// </summary>
    [HttpPatch("{id}/toggle-pin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> TogglePin(int id)
    {
        var news = await _db.News.FindAsync(id);
        if (news == null)
            return NotFound("Không tìm thấy tin tức");

        news.IsPinned = !news.IsPinned;
        news.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(news);
    }

    /// <summary>
    /// Xóa tin tức
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var news = await _db.News.FindAsync(id);
        if (news == null)
            return NotFound("Không tìm thấy tin tức");

        _db.News.Remove(news);
        await _db.SaveChangesAsync();

        return Ok("Đã xóa tin tức");
    }
}

// DTOs
public class CreateNewsDto
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public bool IsPinned { get; set; } = false;
}

public class UpdateNewsDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsPinned { get; set; }
}
