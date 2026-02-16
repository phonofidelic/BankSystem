using BankRUs.Application.Services.CustomerService.GetBankAccount;
using BankRUs.Application.UseCases.ListCustomerAccounts;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.Services.CustomerService;

public interface ICustomerService
{
    public Task<IQueryable<Customer>> SearchCustomersAsync(ListCustomerAccountsQuery query);
    
    public Task<Customer> GetCustomerAsync(Guid customerId);
    
    public Task<Guid> GetCustomerIdAsync(Guid applicationUserId);

    CompleteCustomerAccountDetails ValidateCustomerAccountDetails(CustomerAccountDetails customerAccountDetails);
    public Task<CreateCustomerResult> CreateCustomerAsync();

    public Task OpenCustomerAccountAsync(OpenCustomerAccountRequest request);

    public void RemoveCustomerAccount(Customer customer);
    
    // ToDo: Move to BankAccountsRepository?
    public Task<CreateBankAccountResult> CreateBankAccountAsync(CreateBankAccountRequest request);
    
    public bool EmailExists(string email);
    
    public bool SsnExists(string ssn);
}
