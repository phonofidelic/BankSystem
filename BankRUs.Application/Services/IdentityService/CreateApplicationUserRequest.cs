namespace BankRUs.Application.Services.Identity;

public sealed record CreateApplicationUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password
);
