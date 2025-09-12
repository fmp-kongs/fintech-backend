using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FintechBackend.Models;

[Table("users")]
public class User
{
	[Key]
	public long Id { get; set; }

	[Required]
	public string Name { get; set; } = string.Empty;

	[EmailAddress]
	public string? Email {  get; set; }
}