using BankRUs.Application.BankAccounts;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using Bogus;

namespace BankRUs.Application.Tests.Infrastructure.Stubs;

public class BankAccountRepositoryStub : IBankAccountsRepository
{
    private readonly Guid _customerId;
    private readonly Currency _currency;

    public BankAccountRepositoryStub(Guid customerId)
    {
        _customerId = customerId;
        _currency = new Currency
        {
            EnglishName = "Swedish Krona",
            NativeName = "Svensk krona",
            ISOSymbol = "SEK",
            Symbol = "kr"
        };
    }
    
    public async Task AddAsync(BankAccount bankAccount)
    {
        await Task.Delay(100);
    }

    public bool BankAccountExists(Guid bankAccountId)
    {
        return true;
    }

    public async Task<BankAccount> GetBankAccountAsync(Guid bankAccountId)
    {
        var bankAccount = new Faker<BankAccount>()
            .CustomInstantiator((f) => new BankAccount(_customerId))
            .RuleFor(b => b.Currency, () => _currency)
            .RuleFor(b => b.Id, () => bankAccountId)
            .RuleFor(b => b.Status, () => BankAccountStatus.Opened)
            .RuleFor(b => b.Name, (f) => f.Finance.AccountName())
            .RuleFor(b => b.Balance, (f) => f.Finance.Amount());

        return bankAccount.Generate();
    }

    public async Task<decimal> GetBankAccountBalanceAsync(Guid bankAccountId)
    {
        return new Faker().Finance.Amount();
    }

    public async Task<Currency> GetBankAccountCurrencyAsync(Guid bankAccountId)
    {
        return _currency;
    }

    public async Task<IQueryable<BankAccount>> GetBankAccountsForCustomerAsync(Guid customerId)
    {
        var bankAccount = new Faker<BankAccount>()
            .CustomInstantiator((f) => new BankAccount(_customerId))
            .RuleFor(b => b.Currency, () => _currency)
            .RuleFor(b => b.Id, () => Guid.NewGuid())
            .RuleFor(b => b.Status, () => BankAccountStatus.Opened)
            .RuleFor(b => b.Name, (f) => f.Finance.AccountName())
            .RuleFor(b => b.Balance, (f) => f.Finance.Amount());

        return bankAccount.Generate(2).AsQueryable();
    }

    public async Task<BankAccount?> GetClosedBankAccountBySocialSecurityNumberAsync(string socialSecurityNumber)
    {
        var bankAccount = new Faker<BankAccount>()
            .CustomInstantiator((f) => new BankAccount(_customerId))
            .RuleFor(b => b.Currency, () => _currency)
            .RuleFor(b => b.Id, Guid.NewGuid)
            .RuleFor(b => b.Status, () => BankAccountStatus.Closed)
            .RuleFor(b => b.Name, () => "")
            .RuleFor(b => b.Balance, () => 0);

        return bankAccount.Generate();
    }

    public async Task<Guid> GetCustomerAccountIdForBankAccountAsync(Guid bankAccountId)
    {
        return _customerId;
    }
}
