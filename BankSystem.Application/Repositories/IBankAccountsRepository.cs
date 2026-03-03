using BankSystem.Domain.Entities;
using BankSystem.Domain.ValueObjects;

namespace BankSystem.Application.BankAccounts
{
    public interface IBankAccountsRepository
    {
        // ToDo: Task<IReadonlyList<BankAccount>> ?
        public Task<IQueryable<BankAccount>> GetBankAccountsForCustomerAsync(Guid customerAccountId);

        public Task<BankAccount> GetBankAccountAsync(Guid bankAccountId);

        public Task<Guid> GetCustomerAccountIdForBankAccountAsync(Guid bankAccountId);

        public Task<decimal> GetBankAccountBalanceAsync(Guid bankAccountId);

        public Task<Currency> GetBankAccountCurrencyAsync(Guid bankAccountId);

        public Task<BankAccount?> GetClosedBankAccountBySocialSecurityNumberAsync(string socialSecurityNumber);

        public Task AddAsync(BankAccount bankAccount);

        public bool BankAccountExists(Guid bankAccountId);
    }
}
