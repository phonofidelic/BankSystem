namespace BankRUs.Application.Services.Customer.Exceptions;

public class CustomerNotFoundException(string message) : Exception(message)
{
}
