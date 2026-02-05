using BankRUs.Application.Services.CustomerService.GetCustomer;
using BankRUs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.CustomerService;

public interface ICustomerService
{
    Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request);
    public Task<CreateBankAccountResult> CreateBankAccountAsync(CreateBankAccountRequest request);
    public Task<GetCustomerIdResult> GetCustomerIdAsync(GetCustomerIdRequest request);
}
