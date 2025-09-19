﻿using System.ComponentModel.DataAnnotations;

namespace FintechBackend.DTOs;

public class RegisterDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; } = string.Empty;
}

public class UpdateUserDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}