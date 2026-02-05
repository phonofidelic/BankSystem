using System.ComponentModel.DataAnnotations;

namespace BankRUs.Api.Dtos.Auth;

public sealed record LoginRequestDto(
    [Required]
    string UserName,
    [Required]
    string Password);
