using BankRUs.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankRUs.Infrastructure.Persistence.Configurations;

public static class CurrencyConfigurationExtensions
{

    public static void ConfigureProps<T>(this OwnedNavigationBuilder<T, Currency> currencyBuilder) where T : class
    {
        currencyBuilder.Property(pp => pp.ISOSymbol)
            .HasDefaultValue(Currency.Placeholder.ISOSymbol)
            .HasColumnName(string.Format("{0}_{1}", nameof(Currency), nameof(Currency.ISOSymbol)));

        currencyBuilder.Property(pp => pp.Symbol)
            .HasDefaultValue(Currency.Placeholder.Symbol)
            .HasColumnName(string.Format("{0}_{1}", nameof(Currency), nameof(Currency.Symbol)));

        currencyBuilder.Property(pp => pp.EnglishName)
            .HasDefaultValue(Currency.Placeholder.EnglishName)
            .HasColumnName(string.Format("{0}_{1}", nameof(Currency), nameof(Currency.EnglishName)));

        currencyBuilder.Property(pp => pp.NativeName)
            .HasDefaultValue(Currency.Placeholder.NativeName)
            .HasColumnName(string.Format("{0}_{1}", nameof(Currency), nameof(Currency.NativeName)));
    }
}

