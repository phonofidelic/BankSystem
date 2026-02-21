using BankRUs.Domain.Entities;
using BankRUs.Infrastructure.Persistence.Configurations;
using BankRUs.Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BankRUs.Infrastructure.Persistence;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options
    ) : IdentityDbContext<ApplicationUser>(options)
{
    private bool _setTimestamps { get; set; } = true;
    public DbSet<CustomerAccount> Customers { get; set; } = default!;
    public DbSet<BankAccount> BankAccounts { get; set; } = default!;
    public DbSet<Transaction> Transactions { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        if (Database.IsSqlServer())
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        else
        {
            // Exclude temporal configuration when running integration tests using SQLite
            builder.ApplyConfigurationsFromAssembly(
                Assembly.GetExecutingAssembly(),
                type => type != typeof(TransactionTemporalConfiguration));
        }
    }

    public void SetTimestamps(bool setTimestamps)
    {
        _setTimestamps = setTimestamps; 
    }
    public override int SaveChanges()
    {
        if (_setTimestamps)
        {
            AddTimestamps();
        }
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        if (_setTimestamps)
        {
            AddTimestamps();
        }
        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private void AddTimestamps()
    {
        var now = DateTime.UtcNow;

        var creatableEntries = ChangeTracker.Entries()
            .Where(e => (e.Entity is BaseCreatableEntity<int> || e.Entity is BaseCreatableEntity<Guid>)
                && e.State == EntityState.Added);

        foreach (var entry in creatableEntries) 
        {
            if (entry.Entity is BaseCreatableEntity<int>)
                ((BaseCreatableEntity<int>)entry.Entity).CreatedAt = now;

            if (entry.Entity is BaseCreatableEntity<Guid>)
                ((BaseCreatableEntity<Guid>)entry.Entity).CreatedAt = now;
        }

        var updatableEntries = ChangeTracker.Entries()
            .Where(e => (e.Entity is BaseUpdatableEntity<int> || e.Entity is BaseUpdatableEntity<Guid>)
                && e.State == EntityState.Modified);

        foreach (var entry in updatableEntries)
        {
            if (entry.Entity is BaseUpdatableEntity<int>)
                ((BaseUpdatableEntity<int>)entry.Entity).UpdatedAt = now;

            if (entry.Entity is BaseUpdatableEntity<Guid>)
                ((BaseUpdatableEntity<Guid>)entry.Entity).UpdatedAt = now;
        }
    }
}

