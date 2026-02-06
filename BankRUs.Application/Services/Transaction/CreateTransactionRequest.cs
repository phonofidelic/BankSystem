using BankRUs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.TransactionService
{
    public record CreateTransactionRequest(
        Guid CustomerId,
        Guid BankAccountId,
        decimal Amount,
        TransactionType Type,
        string Currency,
        string? Reference);
}
