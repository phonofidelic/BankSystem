using BankSystem.Application.Configuration;
using BankSystem.Application.Exceptions;
using BankSystem.Application.Services.CurrencyService;
using BankSystem.Domain.ValueObjects;
using Microsoft.Extensions.Options;

namespace BankSystem.Infrastructure.Services.CurrencyService;

public class CurrencyService(IOptions<AppSettings> appSettings) : ICurrencyService
{
    private readonly AppSettings _appSettings = appSettings.Value;

    public Currency GetDefaultCurrency()
    {
        return _appSettings.DefaultCurrency;
    }

    public Currency ParseIsoSymbol(string isoSymbol)
    {
        var currency = _appSettings.SupportedCurrencies.FirstOrDefault(currency => currency.ISOSymbol == isoSymbol);

        return currency ?? throw new UnsupportedCurrencyException();
    }
}
