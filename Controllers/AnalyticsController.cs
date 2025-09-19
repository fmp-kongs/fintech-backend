using FintechBackend.Data;
using FintechBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FintechBackend.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AnalyticsController(AppDbContext appDbContext) : ControllerBase
{
    private readonly AppDbContext _db = appDbContext;

    [HttpGet("transactions/daily")]
    public async Task<IActionResult> GetDailyTransactions()
    {
        var data = await _db.Transactions
            .GroupBy(t => t.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                TotalAmount = g.Sum(t => t.Amount),
                TransactionCount = g.Count()
            }).ToListAsync();

        return Ok(data);
    }

    [HttpGet("transactions/monthly")]
    public async Task<IActionResult> GetMonthlyTransactions()
    {
        var data = await _db.Transactions
            .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalAmount = g.Sum(t => t.Amount),
                TransactionCount = g.Count()
            }).ToListAsync();
        return Ok(data);
    }

    [HttpGet("users/count")]
    public async Task<IActionResult> GetUserCount()
    {
        var userCount = await _db.Users.CountAsync();
        return Ok(new { TotalUsers = userCount });
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue()
    {
        var totalRevenue = await _db.Transactions
            .Where(t => t.Status == Models.Enums.TransactionStatus.Success)
            .SumAsync(t => t.Amount);
        return Ok(new { TotalRevenue = totalRevenue });
    }

    [HttpGet("user-spending/{userId}")]
    public async Task<IActionResult> GetUserSpending([FromBody] int userId)
    {
        var spending = await _db.Transactions
            .Where(t => t.UserId == userId)
            .SumAsync(t => t.Amount);
        return Ok(spending);
    }
}
