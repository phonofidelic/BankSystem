using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;

namespace BankRUs.Domain.ValueObjects
{
    [NotMapped]
    public class Currency : ValueObject
    {
        public required string EnglishName { get; set; }
        public required string NativeName { get; set; }
        public required string Symbol { get; set; }
        public required string ISOSymbol { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return EnglishName;
            yield return NativeName;
            yield return Symbol;
            yield return ISOSymbol;
        }

        public override string ToString() {
            return ISOSymbol;
        }

        public static Currency Parse(string value, Dictionary<string, Currency> supportedCurrencyDictionary)
        {
            var currency = supportedCurrencyDictionary[value] ?? throw new Exception("Unsupported currency");
            return currency;
        }
    }
}
