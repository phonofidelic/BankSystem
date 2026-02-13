using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.UseCases.GetCustomerAccountDetails;

public record GetCustomerAccountDetailsResult (
    Guid CustomerAccountId,
    CustomerAccountDetails CustomerAccountDetails,
    IReadOnlyList<BankAccount> BankAccounts
);
