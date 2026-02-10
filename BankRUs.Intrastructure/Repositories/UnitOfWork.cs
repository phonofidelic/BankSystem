using BankRUs.Application;
using BankRUs.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Infrastructure.Repositories;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
