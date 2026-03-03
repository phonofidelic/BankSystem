using BankSystem.Domain.Entities;
using BankSystem.Domain.ValueObjects;

namespace BankSystem.Application.UseCases.GetCustomerAccountDetails;

public record GetCustomerAccountDetailsResult (
    Guid CustomerAccountId,
    CustomerAccountDetails CustomerAccountDetails,
    IReadOnlyList<BankAccount> BankAccounts
);
