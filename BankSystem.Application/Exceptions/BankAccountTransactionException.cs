namespace BankSystem.Application.Exceptions;

public class BankAccountTransactionException(string message) : BadRequestException(message)
{
}
