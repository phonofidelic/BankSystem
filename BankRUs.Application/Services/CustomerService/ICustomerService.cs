using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.Services.CustomerAccountService;

public interface ICustomerAccountService
{
    public Task<IQueryable<CustomerAccount>> SearchCustomersAsync(CustomerAccountsPageQuery query);
    
    public Task<CustomerAccount> GetCustomerAsync(Guid customerId);
    
    public Task<Guid> GetCustomerIdAsync(Guid applicationUserId);

    public Task<CustomerAccount?> GetClosedCustomerAccountBySocialSecurityNumber(string socialSecurityNumber);

    CompleteCustomerAccountDetails ValidateCustomerAccountDetails(CustomerAccountDetails customerAccountDetails);
    public Task<CreateCustomerAccountResult> CreateCustomerAsync(CreateCustomerAccountRequest request);

    public Task OpenCustomerAccountAsync(OpenCustomerAccountRequest request);

    // ToDo: Move to BankAccountsRepository?
    public Task<CreateBankAccountResult> CreateBankAccountAsync(CreateBankAccountRequest request);
    
    public bool EmailExists(string email);
    
    public bool SsnExists(string ssn);
}
