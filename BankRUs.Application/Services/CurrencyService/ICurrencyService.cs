using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.Services.CurrencyService;

public interface ICurrencyService
{
    Currency ParseIsoSymbol(string isoSymbol);
}
