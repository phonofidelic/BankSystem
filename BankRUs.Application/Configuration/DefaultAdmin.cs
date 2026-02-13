using System;
using System.ComponentModel.DataAnnotations;

namespace BankRUs.Application.Configuration;

public class DefaultAdmin
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = "Default";
    public string LastName { get; set; } = "Admin";
}
