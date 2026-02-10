using BankRUs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankRUs.Infrastructure.Persistence.Configurations
{
    internal class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.ToTable("BankAccounts").HasKey(b => b.Id);

            builder.OwnsOne(b => b.Currency, currencyBuilder =>
            {
                currencyBuilder.ConfigureProps();
            });

            builder
                .Property(b => b.Balance)
                .HasPrecision(19, 2);

            builder
                .HasMany(ba => ba.Transactions)
                .WithOne(t => t.BankAccount);

            builder
                .HasOne(ba => ba.Customer)
                .WithMany(c => c.BankAccounts);
        }
    }
}
