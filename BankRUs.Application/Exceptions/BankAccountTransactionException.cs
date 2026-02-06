namespace BankRUs.Application.Exceptions;

public class BankAccountTransactionException(string message) : BadRequestException(message)
{
}
