namespace BankRUs.Application.UseCases.OpenAccount;

public record OpenCustomerAccountCommand(
    string FirstName,
    string LastName,
    string SocialSecurityNumber,
    string Email,
    string Password
);
