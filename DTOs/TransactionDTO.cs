using FintechBackend.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FintechBackend.DTOs;

public class CreateTransactionRequest
{
    [Required]
    public long UserId { get; set; }
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
    [Required]
    public int ProductId { get; set; }
    [Required]
    public TransactionStatus Status { get; set; }
}

public class UpdateTransactionRequest
{
    [Required]
    public long UserId { get; set; }
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
    [Required]
    public int ProductId { get; set; }
    [Required]
    public TransactionStatus Status { get; set; }
}
