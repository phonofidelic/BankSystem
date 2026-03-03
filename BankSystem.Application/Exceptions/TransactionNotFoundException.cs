namespace BankSystem.Application.Exceptions;

public class TransactionNotFoundException() : NotFoundException("Transaction not found")
{
}
