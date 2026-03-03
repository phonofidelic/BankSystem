using System.ComponentModel.DataAnnotations;

namespace BankSystem.Api.Dtos.Auth;

public sealed record LoginRequestDto(
    [Required]
    string UserName,
    [Required]
    string Password);
