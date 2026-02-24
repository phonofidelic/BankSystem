using System;
using BankRUs.Application.Services.CurrencyService;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.Tests.Infrastructure.Stubs;

public class CurrencyServiceStub : ICurrencyService
{
    public Currency GetDefaultCurrency()
    {
        return new Currency
        {
            EnglishName = "Swedish Krona",
            NativeName = "Svensk krona",
            ISOSymbol = "SEK",
            Symbol = "kr"
        };
    }

    public Currency ParseIsoSymbol(string isoSymbol)
    {
        if (isoSymbol == "SEK")
            return new Currency
            {
                EnglishName = "Swedish Krona",
                NativeName = "Svensk krona",
                ISOSymbol = "SEK",
                Symbol = "kr"
            };

        return new Currency
        {
            EnglishName = "Fake Currency",
            NativeName = "Fake Currency",
            ISOSymbol = "FAK",
            Symbol = "#"
        };
    }
}
