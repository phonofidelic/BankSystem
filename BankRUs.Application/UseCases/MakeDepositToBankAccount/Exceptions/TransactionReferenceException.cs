namespace BankRUs.Application.UseCases.MakeDepositToBankAccount.Exceptions;

public class TransactionReferenceException(string message) : ArgumentException(message)
{
}
