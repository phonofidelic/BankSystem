namespace BankRUs.Application.Exceptions;

public class BankAccountNotOwnedException(string message = "Bank account does not belong to the Customer") : UnauthorizedException(message)
{
}
