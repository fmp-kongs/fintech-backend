using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FintechBackend.Models;
using FintechBackend.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace FintechBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController(AppDbContext db) : ControllerBase
{
	private readonly AppDbContext _db = db;

    [HttpGet]
	public async Task<IActionResult> GetAll()
	{
		var users = await _db.Users.ToListAsync();
		return Ok(users);
	}

	[HttpPost]
	public async Task<IActionResult> Create(User user)
	{
		_db.Users.Add(user);
		await _db.SaveChangesAsync();
		return CreatedAtAction(nameof(GetAll), new { id = user.Id }, user);
	}

	[HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto updatedUser)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();
        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        await _db.SaveChangesAsync();
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public class UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}