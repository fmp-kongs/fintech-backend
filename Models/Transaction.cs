using FintechBackend.Models.Enums;
using System;

namespace FintechBackend.Models;

public class Transaction
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public decimal Amount { get; set; }
    public int ProductId { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}