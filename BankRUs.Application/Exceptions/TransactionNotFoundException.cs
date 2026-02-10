namespace BankRUs.Application.Exceptions;

public class TransactionNotFoundException() : NotFoundException("Transaction not found")
{
}
