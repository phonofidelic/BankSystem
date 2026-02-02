using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.CustomerService
{
    public record CreateBankAccountRequest(
        Guid CustomerId,
        string BankAccountName);
}
