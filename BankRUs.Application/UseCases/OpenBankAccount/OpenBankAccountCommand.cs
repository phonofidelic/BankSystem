using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.OpenBankAccount
{
    internal record OpenBankAccountCommand(
        Guid Id,
        Guid CustomerId);
}
