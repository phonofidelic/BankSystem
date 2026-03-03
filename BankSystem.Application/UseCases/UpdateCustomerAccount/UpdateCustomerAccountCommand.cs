using BankSystem.Domain.ValueObjects;

namespace BankSystem.Application.UseCases.UpdateCustomerAccount;

public record UpdateCustomerAccountCommand(
    Guid CustomerAccountId,
    CustomerAccountDetails Details);
