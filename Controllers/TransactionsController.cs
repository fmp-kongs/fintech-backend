using FintechBackend.Data;
using FintechBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FintechBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TransactionsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAllTransactions()
    {
        var transactions = await _db.Transactions.ToListAsync();
        return Ok(transactions);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction(Transaction transaction)
    {
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAllTransactions), new { id = transaction.Id }, transaction);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(int id, [FromBody] Transaction updatedTransaction)
    {
        var transaction = await _db.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (transaction == null) return NotFound();
        transaction.Amount = updatedTransaction.Amount;
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
}