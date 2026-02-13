using BankRUs.Application.Configuration;
using BankRUs.Application.Exceptions;
using BankRUs.Application.Services.CurrencyService;
using BankRUs.Domain.ValueObjects;
using Microsoft.Extensions.Options;

namespace BankRUs.Infrastructure.Services.CurrencyService;

public class CurrencyService(IOptions<AppSettings> appSettings) : ICurrencyService
{
    private readonly AppSettings _appSettings = appSettings.Value;
    public Currency ParseIsoSymbol(string isoSymbol)
    {
        var currency = _appSettings.SupportedCurrencies.FirstOrDefault(currency => currency.ISOSymbol == isoSymbol);

        return currency ?? throw new UnsupportedCurrencyException();
    }
}
