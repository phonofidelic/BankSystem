using BankRUs.Application.BankAccounts;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using Bogus;

namespace BankRUs.Application.Tests.Infrastructure.Stubs;

public class BankAccountRepositoryStub : IBankAccountsRepository
{
    public async Task Add(BankAccount bankAccount)
    {
        await Task.Delay(100);
    }

    public bool BankAccountExists(Guid bankAccountId)
    {
        return false;
    }

    public async Task<BankAccount> GetBankAccountAsync(Guid bankAccountId)
    {
        var bankAccount = new Faker<BankAccount>()
            .RuleFor(b => b.Id, () => bankAccountId)
            .RuleFor(b => b.Status, () => BankAccountStatus.Opened)
            .RuleFor(b => b.Name, (f) => f.Finance.AccountName())
            .RuleFor(b => b.CustomerId, Guid.NewGuid)
            .RuleFor(b => b.Currency, () => new Currency
            {
                EnglishName = "Swedish Krona",
                NativeName = "Svensk krona",
                ISOSymbol = "SEK",
                Symbol = "kr"
            })
            .RuleFor(b => b.Balance, (f) => f.Finance.Amount());

            return bankAccount.Generate();
    }

    public async Task<decimal> GetBankAccountBalance(Guid bankAccountId)
    {
        return new Faker().Finance.Amount();
    }

    public async Task<Currency> GetBankAccountCurrency(Guid bankAccountId)
    {
        return new Currency
            {
                EnglishName = "Swedish Krona",
                NativeName = "Svensk krona",
                ISOSymbol = "SEK",
                Symbol = "kr"
            };
    }

    public Task<IQueryable<BankAccount>> GetBankAccountsForCustomerAsync(Guid customerId)
    {
        throw new NotImplementedException();
    }

    public Task<BankAccount?> GetClosedBankAccountBySocialSecurityNumber(string socialSecurityNumber)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> GetCustomerIdForBankAccountAsync(Guid bankAccountId)
    {
        throw new NotImplementedException();
    }
}
