using BankSystem.Application;
using BankSystem.Infrastructure.Persistence;

namespace BankSystem.Infrastructure.Repositories;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
