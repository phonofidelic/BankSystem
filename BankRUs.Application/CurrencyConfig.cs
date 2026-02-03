using BankRUs.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BankRUs.Application;

public class CurrencyConfig
{
    [Required]
    public Dictionary<string, Currency> SupportedCurrencies { get; set; } = default!;
}
public partial class ApiConfig : CurrencyConfig
{
}
