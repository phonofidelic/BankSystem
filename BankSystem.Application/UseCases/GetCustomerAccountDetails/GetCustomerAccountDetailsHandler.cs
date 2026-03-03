using System;
using BankSystem.Application.Exceptions;
using BankSystem.Application.Repositories;
using BankSystem.Application.Services.CustomerAccountService;

namespace BankSystem.Application.UseCases.GetCustomerAccountDetails;

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
        var customerAccount = await _customerAccountRepository.GetCustomerAccountAsync(customerAccountId) ?? throw new CustomerNotFoundException();

        return new GetCustomerAccountDetailsResult(
            CustomerAccountId: customerAccount.Id,
            CustomerAccountDetails: customerAccount.GetDetails(),
            BankAccounts: customerAccount.GetBankAccounts());
    }
}
