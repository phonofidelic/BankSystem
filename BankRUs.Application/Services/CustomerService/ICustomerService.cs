using BankRUs.Application.UseCases.ListCustomerAccounts;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.Services.CustomerService;

public interface ICustomerService
{
    public Task<IQueryable<Customer>> SearchCustomersAsync(ListCustomerAccountsQuery query);
    
    public Task<Customer> GetCustomerAsync(Guid customerId);
    
    public Task<Guid> GetCustomerIdAsync(Guid applicationUserId);
    
    public Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request);
    
    // ToDo: Move to BankAccountsRepository?
    public Task<CreateBankAccountResult> CreateBankAccountAsync(CreateBankAccountRequest request);
    
    public bool EmailExists(string email);
    
    public bool SsnExists(string ssn);
}
