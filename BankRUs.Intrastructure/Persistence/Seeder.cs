using BankRUs.Application.Configuration;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using Bogus;
using Bogus.Extensions.Sweden;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BankRUs.Infrastructure.Persistence;

public static class Seeder
{
    private const int MIN_BANK_ACCOUNTS = 1;
    private const int MAX_BANK_ACCOUNTS = 3;
    private const int MIN_TRANSACTIONS = 15;
    private const int MAX_TRANSACTIONS = 125;

    public static string GenerateSocialSecurityNumber(int seed)
    {
        Randomizer.Seed = new Random(seed);
        var faker = new Faker();
        return faker.Person.Personnummer();
    }

    public static async Task RemoveSeededDataAsync(int seed, IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        var toRemove = await context.Customers.Where(c => c.LastName.Contains(SeedStamp(seed))).ToListAsync();
        context.Customers.RemoveRange(toRemove);
    }

    public static async Task GenerateSeededDataAsync(int count, int seed, IServiceProvider serviceProvider)
    {
        Randomizer.Seed = new Random(seed);
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;

        var customer = new Faker<Customer>()
            .CustomInstantiator((f) => new Customer(f.Random.Guid(), f.Person.Personnummer()))
            .RuleFor(c => c.ApplicationUserId, (f, c) => f.Random.Guid())
            .RuleFor(c => c.FirstName, (f, c) => f.Person.FirstName)
            .RuleFor(c => c.LastName, (f, c) => f.Person.LastName + SeedStamp(seed))
            .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
            // .RuleFor(c => c.SocialSecurityNumber, (f, c) => f.Person.Personnummer())
            .RuleFor(c => c.CreatedAt, (f, c) => f.Date.Past(f.Random.Int(0, 5)))
            .RuleFor(c => c.UpdatedAt, (f, c) => f.Date.Recent());

        var customers = customer.Generate(count);

        foreach (var generatedCustomer in customers)
        {
            await GenerateBankAccountsForCustomer(generatedCustomer, seed, serviceProvider);
            await context.Customers.AddAsync(generatedCustomer);
        }
    }

    private static async Task GenerateBankAccountsForCustomer(Customer customer, int seed, IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
        var refDate = customer.CreatedAt;

        var currency = appSettings.DefaultCurrency;
        var bankAccount = new Faker<BankAccount>()
            .RuleFor(b => b.Id, (f, b) => f.Random.Guid())
            .RuleFor(b => b.Name, (f, b) => f.Finance.AccountName() + SeedStamp(seed))
            .RuleFor(b => b.CustomerId, () => customer.Id)
            .RuleFor(b => b.Customer, () => customer)
            .RuleFor(b => b.Balance, () => 1000)
            .RuleFor(b => b.Currency, (f, b) => new Currency
            {
                EnglishName = currency.EnglishName,
                NativeName = currency.NativeName,
                ISOSymbol = currency.ISOSymbol,
                Symbol = currency.Symbol,
            })
            .RuleFor(b => b.CreatedAt, (f, b) =>
            {
                var openDate = f.Date.Between(refDate, f.Date.Soon(f.Random.Int(1, 5), refDate));
                refDate = openDate;
                return openDate;
            })
            .RuleFor(b => b.UpdatedAt, (f, b) => f.Date.Between(b.CreatedAt, f.Date.Recent()));

        var bankAccounts = bankAccount.GenerateBetween(MIN_BANK_ACCOUNTS, MAX_BANK_ACCOUNTS);

        foreach (var generatedBankAccount in bankAccounts)
        {
            await GenerateTransactionsForBankAccount(generatedBankAccount, seed, serviceProvider);
            customer.AddBankAccount(generatedBankAccount);
        }
    }

    private static async Task GenerateTransactionsForBankAccount(BankAccount bankAccount, int seed, IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
        var refDate = bankAccount.CreatedAt;
        var currency = appSettings.DefaultCurrency;

        var transaction = new Faker<Transaction>()
            .CustomInstantiator(f => new Transaction(bankAccount.Balance > 0 ? f.Random.Enum<TransactionType>() : TransactionType.Deposit) 
            { 
                BankAccountId = bankAccount.Id,
                CustomerId = bankAccount.CustomerId,
                Currency = new Currency
                {
                    EnglishName = currency.EnglishName,
                    NativeName = currency.NativeName,
                    ISOSymbol = currency.ISOSymbol,
                    Symbol = currency.Symbol,
                }
            })
            .RuleFor(t => t.Id, (f) => f.Random.Guid())
            .RuleFor(t => t.CustomerId, () => bankAccount.CustomerId)
            .RuleFor(t => t.Customer, () => bankAccount.Customer)
            .RuleFor(t => t.BankAccountId, () => bankAccount.Id)
            .RuleFor(t => t.BankAccount, () => bankAccount)
            .RuleFor(t => t.Amount, (f, t) => f.Finance.Amount())
            .RuleFor(t => t.Reference, (f, t) => f.Lorem.Sentence() + SeedStamp(seed))
            .RuleFor(t => t.CreatedAt, (f, t) => {
                var transactionDate = f.Date.Between(refDate, f.Date.Soon(f.Random.Int(1,5), refDate));
                refDate = transactionDate;
                return transactionDate;
             });

        var transactions = transaction.GenerateBetween(MIN_TRANSACTIONS, MAX_TRANSACTIONS);

        foreach (var t in transactions)
        {
            try
            {
                bankAccount.AddTransaction(t);
            }
            catch (Exception ex)
            {
                if (ex is InsufficientFundsException)
                {
                    continue;
                }
                throw;
            }
        }
    }

    private static string SeedStamp(int seed) => $" [SEED:{seed}]";
}
