using BankSystem.Application.UseCases.ListCustomerAccounts;
using BankSystem.Domain.Entities;
using BankSystem.Domain.ValueObjects;

namespace BankSystem.Application.Services.CustomerAccountService;

public interface ICustomerAccountService
{
    public Task<IQueryable<CustomerAccount>> SearchCustomerAccountsAsync(ListCustomerAccountsPageQuery query);
        
    public Task<Guid> GetCustomerAccountIdAsync(Guid applicationUserId);

    public Task<CustomerAccount?> GetClosedCustomerAccountBySocialSecurityNumber(string socialSecurityNumber);

    CompleteCustomerAccountDetails ValidateCustomerAccountDetails(CustomerAccountDetails customerAccountDetails);

    public bool EmailExists(string email);
    
    public bool SsnExists(string ssn);
}
