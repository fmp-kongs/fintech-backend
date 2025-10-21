using FintechBackend.Data;
using FintechBackend.DTOs;
using FintechBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FintechBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionsController(AppDbContext db, IMemoryCache memoryCache) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMemoryCache _cache = memoryCache;

    [HttpGet]
    public async Task<IActionResult> GetAllTransactions()
    {
        var transactions = await _db.Transactions.ToListAsync();
        return Ok(transactions);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction(CreateTransactionRequest transaction)
    {
        if (transaction == null) return BadRequest();

        var transactions = await _db.Transactions.FirstOrDefaultAsync(t => t.UserId == transaction.UserId && t.ProductId == transaction.ProductId && t.Status == transaction.Status);
        if (transactions != null) return Conflict("Transaction with the same UserId, ProductId, and Status already exists.");

        var newTransaction = new Transaction
        {
            UserId = transaction.UserId,
            Amount = transaction.Amount,
            ProductId = transaction.ProductId,
            Status = transaction.Status
        };
        _db.Transactions.Add(newTransaction);

        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAllTransactions), new { id = newTransaction.Id }, newTransaction);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(int id, [FromBody] UpdateTransactionRequest updatedTransaction)
    {
        if (updatedTransaction == null) return BadRequest();
        var transaction = await _db.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (transaction == null) return NotFound();
        transaction.UserId = updatedTransaction.UserId;
        transaction.Amount = updatedTransaction.Amount;
        transaction.ProductId = updatedTransaction.ProductId;
        transaction.Status = updatedTransaction.Status;
        await _db.SaveChangesAsync();
        return Ok(transaction);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var transaction = await _db.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (transaction == null) return NotFound();
        _db.Transactions.Remove(transaction);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetTransactionsByUser(long userId)
    {
        var transactions = await _db.Transactions.Where(t => t.UserId == userId).ToListAsync();
        return Ok(transactions);
    }


    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel()
    {
        var transactions = await _db.Transactions
            .Include(t => t.UserId)
            .Include(t => t.ProductId)
            .ToListAsync();

        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Transactions");
        worksheet.Cell(1, 1).Value = "Transaction ID";
        worksheet.Cell(1, 2).Value = "User ID";
        worksheet.Cell(1, 3).Value = "Product ID";
        worksheet.Cell(1, 4).Value = "Amount";
        worksheet.Cell(1, 5).Value = "Status";
        worksheet.Cell(1, 6).Value = "Created At";

        for (int i = 0; i < transactions.Count; i++)
        {
            var t = transactions[i];
            worksheet.Cell(i + 2, 1).Value = t.Id;
            worksheet.Cell(i + 2, 2).Value = t.UserId;
            worksheet.Cell(i + 2, 3).Value = t.ProductId;
            worksheet.Cell(i + 2, 4).Value = t.Amount;
            worksheet.Cell(i + 2, 5).Value = t.Status.ToString();
            worksheet.Cell(i + 2, 6).Value = t.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        using var stream = new System.IO.MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = "Transactions.xlsx";
        return File(stream, contentType, fileName);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetTransactionsByDate([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = _db.Transactions.AsQueryable();
        if (startDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= endDate.Value);
        }
        var transactions = await query.ToListAsync();
        return Ok(transactions);
    }

    [HttpGet("monthly-summary-cached")]
    public async Task<IActionResult> GetMonthlyTransactionSummaryCached()
    {
        var cacheKey = "MonthlyTransactionSummary";
        if (!_cache.TryGetValue(cacheKey, out List<object> cachedData))
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
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));
            _cache.Set(cacheKey, data, cacheEntryOptions);
            return Ok(data);
        }
        return Ok(cachedData);
    }
}