using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.GetBankAccountsForCustomer
{
    public record GetBankAccountsForCustomerQuery(Guid ApplicationUserId);
}
