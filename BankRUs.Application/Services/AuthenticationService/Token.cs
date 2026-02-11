namespace BankRUs.Application.Services.Authentication;

public sealed record Token(
    string AccessToken,
    DateTime ExpiresAtUtc);
