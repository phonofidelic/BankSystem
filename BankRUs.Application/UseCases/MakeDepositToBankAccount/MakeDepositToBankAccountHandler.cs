using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.MakeDepositToBankAccount;

public class MakeDepositToBankAccountHandler : IHandler<MakeDepositeToBankAccountCommand, MakeDepositToBankAccountResult>
{
    private readonly CurrencyConfig _currencyConfig;

    public MakeDepositToBankAccountHandler(CurrencyConfig currencyConfig)
    {
        _currencyConfig = currencyConfig;
    }
    public async Task<MakeDepositToBankAccountResult> HandleAsync(MakeDepositeToBankAccountCommand request)
    {
        return new MakeDepositToBankAccountResult(
            TransactionId: Guid.NewGuid(),
            CustomerId: request.CustomerId,
            TransactionType.Deposit,
            Ammount: (decimal)100.00,
            Currency: Currency.Parse("SEK", _currencyConfig.SupportedCurrencies));
    }
}
