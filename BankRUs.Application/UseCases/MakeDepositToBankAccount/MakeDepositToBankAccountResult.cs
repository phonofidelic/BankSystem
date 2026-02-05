using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BankRUs.Application.UseCases.MakeDepositToBankAccount
{
    public record MakeDepositToBankAccountResult(
        Guid TransactionId,
        Guid CustomerId,
        TransactionType Type,
        decimal Amount,
        decimal BalanceAfter,
        string Currency,
        string? Reference,
        DateTime CreatedAt);
}
