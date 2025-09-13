using FintechBackend.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace FintechBackend.Models;

public class Transaction
{
    [Key]
    public long Id { get; set; }
    [Required]
    public long UserId { get; set; }
    [Required]
    public decimal Amount { get; set; }
    [Required]
    public int ProductId { get; set; }
    [Required]
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}