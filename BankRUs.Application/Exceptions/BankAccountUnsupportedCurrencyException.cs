namespace BankRUs.Application.Exceptions;

/// <summary>
/// Represents an exception that is thrown when the specified currency is not supported by the Bank Account.
/// </summary>
public class BankAccountUnsupportedCurrencyException() : BadRequestException("The bank account does not support the specified currency")
{
}
