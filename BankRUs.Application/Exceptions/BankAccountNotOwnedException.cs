namespace BankRUs.Application.Exceptions;

internal class BankAccountNotOwnedException(string message = "Bank account does not belong to the Customer") : UnauthorizedException(message)
{
}
