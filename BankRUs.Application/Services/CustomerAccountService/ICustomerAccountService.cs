using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.Services.CustomerAccountService;

public interface ICustomerAccountService
{
    public Task<IQueryable<CustomerAccount>> SearchCustomerAccountsAsync(CustomerAccountsPageQuery query);
    
    public Task<CustomerAccount> GetCustomerAccountAsync(Guid customerId);
    
    public Task<Guid> GetCustomerAccountIdAsync(Guid applicationUserId);

    public Task<CustomerAccount?> GetClosedCustomerAccountBySocialSecurityNumber(string socialSecurityNumber);

    CompleteCustomerAccountDetails ValidateCustomerAccountDetails(CustomerAccountDetails customerAccountDetails);

    public Task OpenCustomerAccountAsync(OpenCustomerAccountRequest request);

    public bool EmailExists(string email);
    
    public bool SsnExists(string ssn);
}
