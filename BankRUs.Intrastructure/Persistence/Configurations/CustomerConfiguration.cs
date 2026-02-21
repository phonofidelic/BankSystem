using BankRUs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankRUs.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<CustomerAccount>
    {
        public void Configure(EntityTypeBuilder<CustomerAccount> builder)
        {
            builder
                .HasMany(c => c.BankAccounts)
                .WithOne(b => b.Customer);

            builder
                .HasMany(c => c.Transactions)
                .WithOne(t => t.Customer)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasIndex(c => new { c.ApplicationUserId, c.SocialSecurityNumber })
                .IsUnique();
        }
    }
}
