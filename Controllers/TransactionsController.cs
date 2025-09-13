using FintechBackend.Data;
using FintechBackend.Models;
using FintechBackend.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FintechBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionsController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

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
}


public class CreateTransactionRequest
{
    public long UserId { get; set; }
    public decimal Amount { get; set; }
    public int ProductId { get; set; }
    public TransactionStatus Status { get; set; }
}

public class UpdateTransactionRequest
{
    public long UserId { get; set; }
    public decimal Amount { get; set; }
    public int ProductId { get; set; }
    public TransactionStatus Status { get; set; }
}