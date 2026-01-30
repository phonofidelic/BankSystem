namespace BankRUs.Application.Services.Identity;

public sealed record CreateUserRequest(
    string FirstName,
    string LastName,
    string SocialSecurityNumber,
    string Email
);
