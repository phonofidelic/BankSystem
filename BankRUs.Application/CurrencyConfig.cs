using BankRUs.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BankRUs.Application
{
    public sealed record CurrencyConfig
    {
        [Required]
        public Dictionary<string, Currency> SupportedCurrencies = default!;
    }
}
