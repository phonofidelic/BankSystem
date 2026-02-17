namespace BankRUs.Application.UseCases.OpenCustomerAccount;

public record OpenCustomerAccountCommand(
    string FirstName,
    string LastName,
    string SocialSecurityNumber,
    string Email,
    string Password
);
