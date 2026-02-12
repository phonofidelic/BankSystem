namespace BankRUs.Application.UseCases.UpdateCustomerAccount;

public record UpdateCustomerAccountCommand(
    Guid CustomerAccountId,
    string? FirstName,
    string? LastName,
    string? Email,
    string? SocialSecurityNumber);
