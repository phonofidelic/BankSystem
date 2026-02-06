namespace BankRUs.Application.Exceptions;

public class CustomerNotFoundException(string message) : NotFoundException(message)
{
}
