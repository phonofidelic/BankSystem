using BankRUs.Application;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using Bogus;
using Bogus.Extensions.Sweden;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;

namespace BankRUs.Infrastructure.Persistence;

public static class Seeder
{
    private const int MIN_BANK_ACCOUNTS = 1;
    private const int MAX_BANK_ACCOUNTS = 3;
    private const int MIN_TRANSACTIONS = 15;
    private const int MAX_TRANSACTIONS = 125;

    public static async Task RemoveGeneratedCustomersAsync(int count, int seed, IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        //Randomizer.Seed = new Random(seed);

        //var customer = new Faker<Customer>()
        //    .RuleFor(c => c.Id, (f, c) => f.Random.Guid());

        var customers = await GenerateCustomersAsync(count, seed, serviceProvider);
        foreach (var customer in customers)
        {
            var storedCustomer = await context.Customers.FirstOrDefaultAsync(c => c.Email == customer.Email);
            if (storedCustomer != null) {
                Console.WriteLine("Removing customer: {0} {1}, ID: {2}", storedCustomer.FirstName, storedCustomer.LastName, storedCustomer.Id);

                context.Customers.Remove(storedCustomer);
            }
        }
    }

    public static async Task<List<Customer>>GenerateCustomersAsync(int count, int seed, IServiceProvider serviceProvider)
    {
        Randomizer.Seed = new Random(seed);
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;

        var customer = new Faker<Customer>()
            .RuleFor(c => c.ApplicationUserId, (f, c) => f.Random.Guid())
            .RuleFor(c => c.FirstName, (f, c) => f.Person.FirstName)
            .RuleFor(c => c.LastName, (f, c) => f.Person.LastName + " GENERATED")
            .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
            .RuleFor(c => c.SocialSecurityNumber, (f, c) => f.Person.Personnummer())
            .RuleFor(c => c.CreatedAt, (f, c) => f.Date.Past(f.Random.Int(0, 5)))
            .RuleFor(c => c.UpdatedAt, (f, c) => f.Date.Recent());

        var customers = customer.Generate(count);

        foreach (var generatedCustomer in customers)
        {
            await GenerateBankAccountsForCustomer(generatedCustomer, serviceProvider);
            await context.Customers.AddAsync(generatedCustomer);
        }

        //var serializerSettings = new JsonSerializerSettings();
        //serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        
        //await context.Customers.AddRangeAsync(customers);
        
        return customers;
    }

    private static async Task GenerateBankAccountsForCustomer(Customer customer, IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
        var refDate = customer.CreatedAt;

        var currency = appSettings.DefaultCurrency;
        var bankAccount = new Faker<BankAccount>()
            .RuleFor(b => b.Id, (f, b) => f.Random.Guid())
            .RuleFor(b => b.Name, (f, b) => f.Finance.AccountName())
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
            });

        var bankAccounts = bankAccount.GenerateBetween(MIN_BANK_ACCOUNTS, MAX_BANK_ACCOUNTS);

        foreach (var generatedBankAccount in bankAccounts)
        {
            await GenerateTransactionsForBankAccount(generatedBankAccount, serviceProvider);
            customer.AddBankAccount(generatedBankAccount);
        }
    }

    private static async Task GenerateTransactionsForBankAccount(BankAccount bankAccount, IServiceProvider serviceProvider)
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
            .RuleFor(t => t.Reference, (f, t) => f.Lorem.Sentence() + " GENERATED")
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
}
