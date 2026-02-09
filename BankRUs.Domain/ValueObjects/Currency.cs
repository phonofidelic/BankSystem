using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Reflection.Emit;
using System.Text;

namespace BankRUs.Domain.ValueObjects;

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

    public override string ToString()
    {
        return ISOSymbol;

        //return string.Format("{0}_{1}_{2}_{3}", ISOSymbol, Symbol, NativeName, EnglishName);
    }

    public static Currency Placeholder  => new()
    {
        ISOSymbol = "DEF",
        Symbol = "def",
        EnglishName = "Placeholder Currency",
        NativeName = "Placeholder Currency"
    };
    

    public string Format()
    {
        return string.Format("{0}_{1}_{2}_{3}", ISOSymbol, Symbol, NativeName, EnglishName);
    }


    //public static Currency Parse(string value, Func<IEnumerable<Currency>> getSupportedCurrencies)
    //{
    //    var currency = getSupportedCurrencies().FirstOrDefault(c => c.ISOSymbol == value) ?? throw new Exception("Unsupported currency");
    //    return currency;
    //}
    public static Currency Parse(string value)
    {
        var props = value.Split('_');

        if (props.Length < 2) {
            return new Currency { 
                EnglishName = value,
                NativeName = value,
                Symbol = value,
                ISOSymbol = value,
            };
        }

        return new Currency
        {
            ISOSymbol = props[0],
            Symbol = props[1],
            NativeName = props[2],
            EnglishName = props[3]
        };
    }
}
