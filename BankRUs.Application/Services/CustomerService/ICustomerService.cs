using BankRUs.Application.Services.CustomerService.GetCustomer;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.Services.CustomerService;

public interface ICustomerService
{
    public Task<IQueryable<Customer>> GetCustomersAsync();
    
    public Task<Customer> GetCustomerAsync(Guid customerId);
    
    public Task<GetCustomerIdResult> GetCustomerIdAsync(GetCustomerIdRequest request);
    
    public Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request);
    
    // ToDo: Move to BankAccountsRepository?
    public Task<CreateBankAccountResult> CreateBankAccountAsync(CreateBankAccountRequest request);
    
    public bool EmailExists(string email);
    
    public bool SsnExists(string ssn);
}
