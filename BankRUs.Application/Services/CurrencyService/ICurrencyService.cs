using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.Services.CurrencyService;

public interface ICurrencyService
{
    Currency GetDefaultCurrency();
    Currency ParseIsoSymbol(string isoSymbol);
}
