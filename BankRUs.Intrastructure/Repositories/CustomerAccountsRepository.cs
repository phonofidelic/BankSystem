using BankRUs.Application.Repositories;
using BankRUs.Domain.Entities;
using BankRUs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankRUs.Infrastructure.Repositories;

public class CustomerAccountsRepository(ApplicationDbContext context) : ICustomerAccountsRepository
{
    private readonly ApplicationDbContext _context = context;
    
    public async Task<CustomerAccount?> GetCustomerAccountAsync(Guid customerAccountId)
    {
        return await _context.Customers
            .Include(c => c.BankAccounts)
            .FirstOrDefaultAsync(c => c.Id == customerAccountId);
    }

    public async Task AddCustomerAccountAsync(CustomerAccount customerAccount)
    {
        await _context.Customers.AddAsync(customerAccount);
    }
}
