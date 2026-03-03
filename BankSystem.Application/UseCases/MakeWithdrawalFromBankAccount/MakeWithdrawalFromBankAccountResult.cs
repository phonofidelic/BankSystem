using BankSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystem.Application.UseCases.MakeWithdrawalFromBankAccount;

public record MakeWithdrawalFromBankAccountResult(
    Guid TransactionId,
        Guid CustomerId,
        TransactionType Type,
        decimal Amount,
        decimal BalanceAfter,
        string Currency,
        string? Reference,
        DateTime CreatedAt);
