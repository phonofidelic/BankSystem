namespace BankRUs.Api.Dtos.Auth;

public sealed record LoginResponseDto(
    string? Token,
    DateTime? ExpiresAtUtc);
