using BankRUs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Infrastructure.Persistance.Configurations
{
    internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
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
