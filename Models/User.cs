using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FintechBackend.Models;

[Table("users")]
public class User
{
	[Key]
	public long Id { get; set; }

	[Required]
	public string Name { get; set; } = string.Empty;

	[EmailAddress]
	[Required]
    public string Email {  get; set; } = string.Empty;

	[NotMapped] // won't be saved directly to the database
    public string Password { get; set; } = string.Empty;

    [Required]
	[JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;
}