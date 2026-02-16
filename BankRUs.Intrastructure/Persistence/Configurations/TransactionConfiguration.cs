using BankRUs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankRUs.Infrastructure.Persistence.Configurations
{
    internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        private const string TABLE_NAME = "Transactions";
        private const string TABLE_HISTORY_NAME = "TransactionHistory";
        private const string TABLE_HISTORY_PERIOD_START_NAME = "PeriodStart";
        private const string TABLE_HISTORY_PERIOD_END_NAME = "PeriodEnd";

        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable(TABLE_NAME, t => t.IsTemporal(t => {
                t.HasPeriodStart(TABLE_HISTORY_PERIOD_START_NAME).HasColumnName(TABLE_HISTORY_PERIOD_START_NAME);
                t.HasPeriodEnd(TABLE_HISTORY_PERIOD_END_NAME).HasColumnName(TABLE_HISTORY_PERIOD_END_NAME);
                t.UseHistoryTable(TABLE_HISTORY_NAME);
            })).HasKey(t => t.Id);

            
            // Temporal table with owned entity (value object)
            // https://stackoverflow.com/a/79591498
            builder.OwnsOne(b => b.Currency, currencyBuilder =>
            {
                currencyBuilder.ToTable(
                    TABLE_NAME, 
                    tableBuilder =>
                    {
                        tableBuilder.IsTemporal(t =>
                        {
                            t.HasPeriodStart(TABLE_HISTORY_PERIOD_START_NAME).HasColumnName(TABLE_HISTORY_PERIOD_START_NAME);
                            t.HasPeriodEnd(TABLE_HISTORY_PERIOD_END_NAME).HasColumnName(TABLE_HISTORY_PERIOD_END_NAME);
                            t.UseHistoryTable(TABLE_HISTORY_NAME);
                        });
                    });

                currencyBuilder.ConfigureProps();
            });

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
                .Property(t => t.Type)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<TransactionType>(v, ignoreCase: true));
        }
    }
}
