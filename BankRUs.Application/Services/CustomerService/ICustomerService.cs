using BankRUs.Application.Services.CustomerService.GetCustomer;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.Services.CustomerService;

public interface ICustomerService
{
    public Task<IQueryable<Customer>> GetCustomersQueryAsync();
    public Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request);
    public Task<CreateBankAccountResult> CreateBankAccountAsync(CreateBankAccountRequest request);
    public Task<GetCustomerIdResult> GetCustomerIdAsync(GetCustomerIdRequest request);

    public bool EmailExists(string email);
    public bool SsnExists(string ssn);
}
