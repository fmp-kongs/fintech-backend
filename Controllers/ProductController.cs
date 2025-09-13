using FintechBackend.Data;
using FintechBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FintechBackend.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProductController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _db.Products.ToListAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest product)
    {
        if(product == null) return BadRequest();
        var existingProduct = await _db.Products.FirstOrDefaultAsync(p => p.Name == product.Name && p.Price == product.Price);
        if (existingProduct != null) return Conflict("Product with the same Name and Price already exists.");
        var newProduct = new Product
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price
        };
        _db.Products.Add(newProduct);
        return await _db.SaveChangesAsync() > 0
            ? CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct)
            : StatusCode(500, "A problem happened while handling your request.");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest updatedProduct)
    {
        if (updatedProduct == null) return BadRequest();
        var existingProduct = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (existingProduct == null) return NotFound();
        existingProduct.Name = updatedProduct.Name;
        existingProduct.Description = updatedProduct.Description;
        existingProduct.Price = updatedProduct.Price;
        await _db.SaveChangesAsync();
        return Ok(existingProduct);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
