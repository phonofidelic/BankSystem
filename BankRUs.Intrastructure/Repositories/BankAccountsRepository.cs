using BankRUs.Application.BankAccounts;
using BankRUs.Domain.Entities;
using BankRUs.Intrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BankRUs.Intrastructure.Repositories;

public class BankAccountsRepository(ApplicationDbContext context) : IBankAccountsRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task<IQueryable<BankAccount>> GetBankAccountsForCustomerAsync(Guid userId)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
        if (customer == null)
            throw new Exception("Customer not found");

        return _context.BankAccounts.Where(b => b.CustomerId == customer.Id);
    }
}