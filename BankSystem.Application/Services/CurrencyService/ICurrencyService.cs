using BankSystem.Domain.ValueObjects;

namespace BankSystem.Application.Services.CurrencyService;

public interface ICurrencyService
{
    Currency GetDefaultCurrency();
    Currency ParseIsoSymbol(string isoSymbol);
}
