using BankRUs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.BankAccounts
{
    public interface IBankAccountsRepository
    {
        public Task Add(BankAccount bankAccount);
        public Task<IQueryable<BankAccount>> GetBankAccountsForCustomerAsync(Guid customerId);

        public Task<Guid> GetCustomerIdForBankAccountAsync(Guid bankAccountId);

        public Task UpdateBankAccountBalanceWithTransactionAsync(Transaction transaction);

        public Task<decimal> GetBankAccountBalance(Guid bankAccountId);

        public bool BankAccountExists(Guid bankAccountId);
    }
}
