using BankRUs.Domain.Entities;

namespace BankRUs.Application.Repositories;

public interface ICustomerAccountsRepository
{
    public Task<CustomerAccount?> GetCustomerAccountAsync(Guid customerAccountId);

    public Task AddCustomerAccountAsync(CustomerAccount customerAccount);
}
