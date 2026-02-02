using BankRUs.Api.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BankRUs.Api.Dtos.Accounts;

// Använd record när: DTO, Command, Query
// Använd class när: entitet
public record CreateAccountRequestDto(
    
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
    string Email
)
{
    public string FirstName { get; init; } = FirstName.Trim();
    public string LastName { get; init; } = LastName.Trim();
    public string SocialSecurityNumber { get; init; } = SocialSecurityNumber.Trim();
    public string Email { get; init; } = Email.Trim();

};