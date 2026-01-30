using BankRUs.Application.BankAccounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.GetBankAccountsForCustomer
{
    public class GetBankAccountsForCustomerHandler(
        IBankAccountsRepository bankAccountsRepository) : IHandler<GetBankAccountsForCustomerQuery, GetBankAccountsForCustomerResult>
    {
        private readonly IBankAccountsRepository _bankAccountsRepository = bankAccountsRepository;
        public async Task<GetBankAccountsForCustomerResult> HandleAsync(GetBankAccountsForCustomerQuery query)
        {
            try
            {
                var bankAccountsQuery = await _bankAccountsRepository.GetBankAccountsForCustomerAsync(query.ApplicationUserId);

                return new GetBankAccountsForCustomerResult(bankAccountsQuery);
            }
            catch
            {
                throw new Exception("Could not retrieve bank accounts");
            }
        }
    }
}
