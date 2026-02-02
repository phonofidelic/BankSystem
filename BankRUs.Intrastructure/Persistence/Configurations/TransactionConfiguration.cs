using BankRUs.Application;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Infrastructure.Persistence.Configurations
{
    internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        private readonly CurrencyConfig _currencyConfiguration;
        public TransactionConfiguration(CurrencyConfig currencyConfiguration) {
            _currencyConfiguration = currencyConfiguration;
        }
        public TransactionConfiguration() { }

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

            builder
                .Property(t => t.Currency)
                .HasConversion(
                    v => v.ToString(),
                    v => Currency.Parse(v, _currencyConfiguration.SupportedCurrencies));

            //builder
            //    .OwnsOne(t => t.Currency, t =>
            //    {
            //        t.Property(c => c.ISOSymbol).HasColumnName(nameof(Currency));
            //    });

            //builder.Ignore(t => t.Currency);
        }
    }
}
