using BankRUs.Api.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BankRUs.Api.Dtos.Accounts;

public record CreateEmployeeAccountRequestDto(
    [Required]
    [MaxLength(25)]
    string FirstName,

    [Required]
    [MaxLength(25)]
    string LastName,

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    string Email,

    [Required]
    [PasswordPropertyText]
    string Password
    )
{
    public string FirstName { get; init; } = FirstName.Trim();
    public string LastName { get; init; } = LastName.Trim();
    public string Email { get; init; } = Email.Trim();
    public string Password { get; init; } = Password.Trim();
};
