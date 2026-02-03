using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.MakeDepositToBankAccount
{
    public record MakeDepositToBankAccountCommand(
        Guid CustomerId,
        Guid BankAccountId,
        decimal Amount,
        string Currency,
        string? Reference);
}
