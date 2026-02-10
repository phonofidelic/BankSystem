using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.BankAccounts
{
    public interface IBankAccountsRepository
    {
        public Task Add(BankAccount bankAccount);

        // ToDo: Task<IReadonlyList<BankAccount>> ?
        public Task<IQueryable<BankAccount>> GetBankAccountsForCustomerAsync(Guid customerId);

        public Task<BankAccount> GetBankAccountAsync(Guid bankAccountId);

        public Task<Guid> GetCustomerIdForBankAccountAsync(Guid bankAccountId);

        public Task UpdateBankAccountBalanceWithTransactionAsync(Transaction transaction);

        public Task<decimal> GetBankAccountBalance(Guid bankAccountId);

        public Task<Currency> GetBankAccountCurrency(Guid bankAccountId);

        public bool BankAccountExists(Guid bankAccountId);
    }
}
