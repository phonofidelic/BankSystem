using BankRUs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankRUs.Infrastructure.Persistence.Configurations
{
    internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder
                .HasOne(t => t.Customer)
                .WithMany(c => c.Transactions)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(t => t.BankAccount)
                .WithMany(b => b.Transactions)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .Property(t => t.Amount)
                .HasPrecision(19, 2);

            builder
                .Property(t => t.BalanceAfter)
                .HasPrecision(19, 2);

            builder
                .OwnsOne(t => t.Currency, currencyBuilder =>
                {
                    currencyBuilder.ConfigureProps();
                });

            builder
                .Property(t => t.Type)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<TransactionType>(v, ignoreCase: true));
        }
    }
}
