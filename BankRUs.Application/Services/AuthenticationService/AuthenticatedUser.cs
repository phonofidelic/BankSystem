namespace BankRUs.Application.Services.Authentication;

public record AuthenticatedUser(
    string UserId,
    string UserName,
    string Email,
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    IEnumerable<string> Roles = null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
