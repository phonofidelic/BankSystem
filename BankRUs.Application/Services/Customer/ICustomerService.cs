using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.CustomerService;

public interface ICustomerService
{
    Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request);
    public Task<CreateBankAccountResult> CreateBankAccountAsync(CreateBankAccountRequest request);
}
