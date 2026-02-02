using BankRUs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Infrastructure.Persistence.Configurations
{
    internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder
                .HasMany(c => c.BankAccounts)
                .WithOne(b => b.Customer);

            builder
                .HasMany(c => c.Transactions)
                .WithOne(t => t.Customer)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasIndex(c => c.SocialSecurityNumber)
                .IsUnique();
        }
    }
}
