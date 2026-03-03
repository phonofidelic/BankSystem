using BankSystem.Domain.Entities;

namespace BankSystem.Application.Repositories;

public interface ICustomerAccountsRepository
{
    public Task<CustomerAccount?> GetCustomerAccountAsync(Guid customerAccountId);

    public Task AddCustomerAccountAsync(CustomerAccount customerAccount);
}
