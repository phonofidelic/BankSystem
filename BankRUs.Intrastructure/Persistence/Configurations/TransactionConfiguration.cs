using BankRUs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Infrastructure.Persistence.Configurations
{
    internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions", t => t.IsTemporal(t => {
                t.HasPeriodStart("PeriodStart");
                t.HasPeriodEnd("PeriodEnd");
                t.UseHistoryTable("TransactionHistory");
            }));

            builder
                .HasOne(t => t.Customer)
                .WithMany(c => c.Transactions);

            builder
                .Property(t => t.Amount)
                .HasPrecision(19, 2);

            builder
                .Property(t => t.Type)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<TransactionType>(v, ignoreCase: true));
        }
    }
}
