using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Domain.ValueObjects
{
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
    }
}
