using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.MakeDepositToBankAccount
{
    internal record MakeDepositeToBankAccountCommand(
        Guid CustomerId,
        decimal Ammount,
        string? Reference = "");
}
