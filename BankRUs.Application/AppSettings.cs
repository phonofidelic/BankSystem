using BankRUs.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BankRUs.Application
{
    public class AppSettings
    {
        [Required]
        public string SystemCulture { get; set; } = string.Empty;

        [Required]
        public IEnumerable<Currency> SupportedCurrencies { get; set; } = [];
        //public Dictionary<string, Currency> SupportedCurrencies { get; set; } = default!;

        [Required]
        public Currency DefaultCurrency { get; set; } = default!;
    }
}
