using BankRUs.Application.BankAccounts;
using BankRUs.Application.Exceptions;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using BankRUs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankRUs.Infrastructure.Repositories;

public class BankAccountsRepository(ApplicationDbContext context) : IBankAccountsRepository
{
    private readonly ApplicationDbContext _context = context;

    public bool BankAccountExists(Guid bankAccountId)
    {
        return _context.BankAccounts.Find(bankAccountId) != null;
    }
    
    public async Task<IQueryable<BankAccount>> GetBankAccountsForCustomerAsync(Guid userId)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
        
        return customer == null
            ? throw new Exception("Customer not found")
            : _context.BankAccounts
            .AsNoTracking()
            .Where(b => b.CustomerId == customer.Id);
    }

    public async Task<Guid> GetCustomerAccountIdForBankAccountAsync(Guid bankAccountId)
    {
        var bankAccount = await _context.BankAccounts.FindAsync(bankAccountId);
        return bankAccount?.CustomerId ?? throw new Exception("Bank account not found");
    }

    public async Task<decimal> GetBankAccountBalanceAsync(Guid bankAccountId)
    {
        var bankAccount = await _context.BankAccounts.FindAsync(bankAccountId)
            ?? throw new BankAccountNotFoundException();

        return bankAccount.Balance;
    }

    public async Task<BankAccount> GetBankAccountAsync(Guid bankAccountId)
    {
        return await _context.BankAccounts.FindAsync(bankAccountId) ?? throw new BankAccountNotFoundException();
    }

    public async Task<Currency> GetBankAccountCurrencyAsync(Guid bankAccountId)
    {
        var bankAccount = await GetBankAccountAsync(bankAccountId);
        return bankAccount.Currency;
    }

    public async Task<BankAccount?> GetClosedBankAccountBySocialSecurityNumberAsync(string socialSecurityNumber)
    {
        return await _context.BankAccounts.FirstOrDefaultAsync(b => b.Customer.SocialSecurityNumber == socialSecurityNumber);
    }
}