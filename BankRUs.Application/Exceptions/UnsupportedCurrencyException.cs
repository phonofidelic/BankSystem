namespace BankRUs.Application.Exceptions;

/// <summary>
/// Represents an exception that is thrown when the specified currency is not supported by the system.
/// </summary>
public class UnsupportedCurrencyException() : BadRequestException("Unsupported currency")
{
}
