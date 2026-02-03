using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using BankRUs.Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BankRUs.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{

    public DbSet<Customer> Customers { get; set; } = default!;
    public DbSet<BankAccount> BankAccounts { get; set; } = default!;
    public DbSet<Transaction> Transactions { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //builder.Entity<Currency>().HasNoKey();

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        AddTimestamps();
        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private void AddTimestamps()
    {
        var now = DateTime.UtcNow; // current datetime

        //var entities = ChangeTracker.Entries()
        //    .Where(x => x.Entity is BaseCreatableEntity<Guid> && (x.State == EntityState.Added || x.State == EntityState.Modified));

        var creatableEntries = ChangeTracker.Entries()
            .Where(e => (e.Entity is BaseCreatableEntity<int> || e.Entity is BaseCreatableEntity<Guid>)
                && e.State == EntityState.Added);

        foreach (var entry in creatableEntries) {
           

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
            if (entry.Entity is BaseCreatableEntity<int>)
                ((BaseUpdatableEntity<int>)entry.Entity).UpdatedAt = now;

            if (entry.Entity is BaseCreatableEntity<Guid>)
                ((BaseUpdatableEntity<Guid>)entry.Entity).UpdatedAt = now;
        }
    }
}

