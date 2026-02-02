using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.MakeDepositToBankAccount
{
    internal record MakeDepositeToBankAccountCommand(
        decimal Ammount,
        string? Reference = "");
}
