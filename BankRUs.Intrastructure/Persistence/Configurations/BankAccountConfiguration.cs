using BankRUs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Infrastructure.Persistence.Configurations
{
    internal class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
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
