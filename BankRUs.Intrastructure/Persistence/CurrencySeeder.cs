using BankRUs.Application;
using BankRUs.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BankRUs.Infrastructure.Persistence;

public static class CurrencySeeder
{
    //private readonly ApplicationDbContext _context = context;
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var appSettings = services.GetRequiredService<IOptions<AppSettings>>().Value;

        var supportedCurrencies = appSettings.SupportedCurrencies.Select(c => new Currency()
        {
            EnglishName = c.EnglishName,
            NativeName = c.NativeName,
            Symbol = c.Symbol,
            ISOSymbol = c.ISOSymbol,
        }).ToList();

        var defaultCurrency = new Currency
        {
            EnglishName = appSettings.DefaultCurrency.EnglishName,
            NativeName = appSettings.DefaultCurrency.NativeName,
            Symbol = appSettings.DefaultCurrency.Symbol,
            ISOSymbol = appSettings.DefaultCurrency.ISOSymbol,
        };

        //int transactionsUpdated = 0;
        var bankAccounts = await context.BankAccounts.Where(b => b.Currency.ISOSymbol == Currency.Placeholder.ISOSymbol).ToListAsync();
        foreach (var bankAccount in bankAccounts)
        {
            bankAccount.Currency = defaultCurrency;
        }

        var transactions = await context.Transactions.TemporalAll().Where(t => t.Currency.ISOSymbol == Currency.Placeholder.ISOSymbol).ToListAsync();
        foreach (var transaction in transactions)
        {
            transaction.Currency = defaultCurrency;
        }

        //context.Transactions.UpdateRange(transactions);

        //.ExecuteUpdateAsync(u => u.SetProperty(b => b.Currency, new Currency
        // {
        //     EnglishName = defaultCurrency.EnglishName,
        //     NativeName = defaultCurrency.NativeName,
        //     Symbol = defaultCurrency.Symbol,
        //     ISOSymbol = defaultCurrency.ISOSymbol,
        //}));

        //foreach (Currency supportedCurrency in supportedCurrencies) {
        //    //var transactions = await context.Transactions.ToListAsync();
        //        //.Where(t => t.Currency.ISOSymbol.ToString() == supportedCurrency.ISOSymbol.ToString())
        //        //.ToListAsync();

        //    //foreach (var transaction in transactions) {
        //    //    if (transaction.Currency.ToString() == supportedCurrency.ISOSymbol)
        //    //    {
        //    //        transaction.Currency = supportedCurrency;
        //    //    }
        //    //}

        //    bankAccountsUpdated = await context.BankAccounts
        //        .Where(b => b.Currency.ToString() == supportedCurrency.ISOSymbol)
        //        .ExecuteUpdateAsync(u => u.SetProperty(b => b.Currency, supportedCurrency));
        //}
        await context.SaveChangesAsync();
    }
}
