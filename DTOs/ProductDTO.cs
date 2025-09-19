using System.ComponentModel.DataAnnotations;

namespace FintechBackend.DTOs;

public class CreateProductRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "Name cannot be empty.")]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }
}

public class UpdateProductRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "Name cannot be empty.")]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }
}
