
namespace BankRUs.Application.UseCases.OpenAccount;

// Command, Query, DTO = använd record (+ se till att den är immutable)
public record OpenCustomerAccountCommand(
    string FirstName,
    string LastName,
    string SocialSecurityNumber,
    string Email
);
