using BankRUs.Application.Repositories;
using BankRUs.Domain.Entities;
using BankRUs.Infrastructure.Persistence;

namespace BankRUs.Infrastructure.Repositories;

public class CustomerAccountsRepository(ApplicationDbContext context) : ICustomerAccountsRepository
{
    private readonly ApplicationDbContext _context = context;
    
    public async Task<CustomerAccount?> GetCustomerAccountAsync(Guid customerAccountId)
    {
        return await _context.Customers.FindAsync(customerAccountId);
    }

    public async Task AddCustomerAccountAsync(CustomerAccount customerAccount)
    {
        await _context.Customers.AddAsync(customerAccount);
    }
}
