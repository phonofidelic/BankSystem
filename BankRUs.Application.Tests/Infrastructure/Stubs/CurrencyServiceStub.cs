using System;
using BankRUs.Application.Services.CurrencyService;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.Tests.Infrastructure.Stubs;

public class CurrencyServiceStub : ICurrencyService
{
    public Currency ParseIsoSymbol(string isoSymbol)
    {
        return new Currency
        {
            EnglishName = "Swedish Krona",
            NativeName = "Svensk krona",
            ISOSymbol = "SEK",
            Symbol = "kr"
        };
    }
}
