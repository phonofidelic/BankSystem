using System.ComponentModel.DataAnnotations;

namespace BankRUs.Application.Services.Authentication;

public sealed record JwtOptions
{
    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Required]
    public string Secret { get; init; } = string.Empty;

    [Required]
    public int ExpiresMinutes { get; init; } = 15;
}
