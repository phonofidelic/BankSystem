using System;
using BankRUs.Application.Exceptions;
using BankRUs.Application.Repositories;
using BankRUs.Application.Services.CustomerAccountService;

namespace BankRUs.Application.UseCases.GetCustomerAccountDetails;

public class GetCustomerAccountDetailsHandler(
    ICustomerAccountService customerService,
    ICustomerAccountsRepository customerAccountRepository
) : IHandler<GetCustomerAccountDetailsQuery, GetCustomerAccountDetailsResult>
{
    private readonly ICustomerAccountService _customerService = customerService;
    private readonly ICustomerAccountsRepository _customerAccountRepository = customerAccountRepository;

    public async Task<GetCustomerAccountDetailsResult> HandleAsync(GetCustomerAccountDetailsQuery query)
    {
        var applicationUserId = query.ApplicationUserId;
        var customerAccountId = await _customerService.GetCustomerAccountIdAsync(applicationUserId);
        var customerAccount = await _customerAccountRepository.GetCustomerAccountAsync(customerAccountId);

        return new GetCustomerAccountDetailsResult(
            CustomerAccountId: customerAccount.Id,
            CustomerAccountDetails: customerAccount.GetDetails(),
            BankAccounts: customerAccount.GetBankAccounts());
    }
}
