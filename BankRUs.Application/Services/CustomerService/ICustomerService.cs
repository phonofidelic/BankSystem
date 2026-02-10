using BankRUs.Application.Paginatioin;
using BankRUs.Application.Services.CustomerService.GetCustomer;
using BankRUs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.CustomerService;

public interface ICustomerService
{
    public Task<BasePagedResult<Customer>> GetCustomersAsPagedResult(CustomersPageQuery query);
    public Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request);
    public Task<CreateBankAccountResult> CreateBankAccountAsync(CreateBankAccountRequest request);
    public Task<GetCustomerIdResult> GetCustomerIdAsync(GetCustomerIdRequest request);

    public bool EmailExists(string email);
    public bool SsnExists(string ssn);
}
