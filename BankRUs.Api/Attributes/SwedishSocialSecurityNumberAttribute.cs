using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BankRUs.Api.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class SwedishSocialSecurityNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string ssn || string.IsNullOrWhiteSpace(ssn))
            return ValidationResult.Success; // använd [Required] separat

        var digits = ssn.Replace("-", "").Replace("+", "");

        // 12 siffror => ta bort sekel
        if (digits.Length == 12)
            digits = digits.Substring(2);

        // Grundkontroll
        if (digits.Length != 10 || !digits.All(char.IsDigit))
            return Error();

        // Datumkontroll (YYMMDD)
        var datePart = digits.Substring(0, 6);
        if (!DateTime.TryParseExact(
                datePart,
                "yyMMdd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
        {
            return Error();
        }

        // Luhn
        if (!IsValidLuhn(digits))
            return Error();

        return ValidationResult.Success;
    }

    private static ValidationResult Error() =>
        new ValidationResult("Invalid social security number");

    private static bool IsValidLuhn(string tenDigits)
    {
        int sum = 0;

        for (int i = 0; i < 9; i++)
        {
            int d = tenDigits[i] - '0';
            int factor = (i % 2 == 0) ? 2 : 1;
            int product = d * factor;

            sum += (product / 10) + (product % 10);
        }

        int controlDigit = tenDigits[9] - '0';
        int expected = (10 - (sum % 10)) % 10;

        return controlDigit == expected;
    }
}
