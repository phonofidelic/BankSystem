using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.OpenBankAccount
{
    public record OpenBankAccountCommand(
        string BankAccountName,
        Guid CustomerId,
        string CustomerEmail);
}
