using BankRUs.Api.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BankRUs.Api.Dtos.Accounts;

// Använd record när: DTO, Command, Query
// Använd class när: entitet
public record CreateCustomerAccountRequestDto(
    
    [Required]
    [MaxLength(25)]
    string FirstName,
    
    [Required]
    [MaxLength(25)]
    string LastName,

    [Required]
    [SwedishSocialSecurityNumber]
    string SocialSecurityNumber,

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
    public string SocialSecurityNumber { get; init; } = SocialSecurityNumber.Trim();
    public string Email { get; init; } = Email.Trim();
    public string Password { get; init; } = Password.Trim();
};