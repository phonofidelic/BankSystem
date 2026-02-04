namespace BankRUs.Application.Services.Authentication;

public record AuthenticatedUser(
    string UserId,
    string UserName,
    string Email,
    IEnumerable<string> Roles = null);
