namespace BankRUs.Application.Exceptions;

public class CustomerNotFoundException(string message) : Exception(message)
{
}
