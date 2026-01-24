using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Api.Models.Courts;

[ApiController]
[Route("api/courts")]
public class CourtsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public CourtsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_db.Courts.Where(c => c.IsActive).ToList());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(Court court)
    {
        _db.Courts.Add(court);
        await _db.SaveChangesAsync();
        return Ok(court);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Court input)
    {
        var court = await _db.Courts.FindAsync(id);
        if (court == null) return NotFound();

        court.Name = input.Name;
        court.Description = input.Description;
        court.IsActive = input.IsActive;

        await _db.SaveChangesAsync();
        return Ok(court);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var court = await _db.Courts.FindAsync(id);
        if (court == null) return NotFound();

        court.IsActive = false;
        await _db.SaveChangesAsync();

        return Ok();
    }
}
