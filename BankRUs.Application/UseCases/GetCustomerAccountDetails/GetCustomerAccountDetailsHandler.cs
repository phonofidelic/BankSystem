using System;
using BankRUs.Application.Exceptions;
using BankRUs.Application.Services.CustomerAccountService;

namespace BankRUs.Application.UseCases.GetCustomerAccountDetails;

public class GetCustomerAccountDetailsHandler(
    ICustomerAccountService customerService
) : IHandler<GetCustomerAccountDetailsQuery, GetCustomerAccountDetailsResult>
{
    private readonly ICustomerAccountService _customerService = customerService;
    public async Task<GetCustomerAccountDetailsResult> HandleAsync(GetCustomerAccountDetailsQuery query)
    {
        var applicationUserId = query.ApplicationUserId;
        var customerAccountId = await _customerService.GetCustomerIdAsync(applicationUserId);
        var customerAccount = await _customerService.GetCustomerAsync(customerAccountId);

        return new GetCustomerAccountDetailsResult(
            CustomerAccountId: customerAccount.Id,
            CustomerAccountDetails: customerAccount.GetDetails(),
            BankAccounts: customerAccount.GetBankAccounts());
    }
}
