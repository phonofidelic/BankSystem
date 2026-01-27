using BankRUs.Domain.Entities;

namespace BankRUs.Application.UseCases.GetBankAccountsForCustomer
{
    public record GetBankAccountsForCustomerResult(IQueryable<BankAccount> bankAccounts);
}