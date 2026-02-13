using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.UseCases.UpdateCustomerAccount;

public record UpdateCustomerAccountCommand(
    Guid CustomerAccountId,
    CustomerAccountDetails Details);
