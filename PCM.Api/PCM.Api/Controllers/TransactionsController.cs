using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Enums;
using PCM.Api.Models;
using System.Security.Claims;

namespace PCM.Api.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize(Roles = "Admin,Treasurer")]
public class TransactionsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public TransactionsController(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Lấy tất cả giao dịch quỹ
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] TransactionType? type = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var query = _db.WalletTransactions.AsQueryable();

        if (type.HasValue)
            query = query.Where(t => t.Type == type);

        if (fromDate.HasValue)
            query = query.Where(t => t.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(t => t.CreatedAt <= toDate.Value.AddDays(1));

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
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
    /// Lấy tổng quan quỹ
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var transactions = await _db.WalletTransactions.ToListAsync();

        var totalIncome = transactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var totalExpense = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        return Ok(new
        {
            totalIncome,
            totalExpense,
            balance = totalIncome - totalExpense,
            transactionCount = transactions.Count
        });
    }

    /// <summary>
    /// Tạo giao dịch thu
    /// </summary>
    [HttpPost("income")]
    public async Task<IActionResult> CreateIncome([FromBody] CreateTransactionDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var transaction = new WalletTransaction
        {
            Amount = dto.Amount,
            Type = TransactionType.Income,
            Category = dto.Category,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        _db.WalletTransactions.Add(transaction);
        await _db.SaveChangesAsync();

        return Ok(transaction);
    }

    /// <summary>
    /// Tạo giao dịch chi
    /// </summary>
    [HttpPost("expense")]
    public async Task<IActionResult> CreateExpense([FromBody] CreateTransactionDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var transaction = new WalletTransaction
        {
            Amount = dto.Amount,
            Type = TransactionType.Expense,
            Category = dto.Category,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        _db.WalletTransactions.Add(transaction);
        await _db.SaveChangesAsync();

        return Ok(transaction);
    }

    /// <summary>
    /// Xóa giao dịch
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var transaction = await _db.WalletTransactions.FindAsync(id);
        if (transaction == null)
            return NotFound("Không tìm thấy giao dịch");

        _db.WalletTransactions.Remove(transaction);
        await _db.SaveChangesAsync();

        return Ok("Đã xóa giao dịch");
    }
}

public class CreateTransactionDto
{
    public decimal Amount { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}
