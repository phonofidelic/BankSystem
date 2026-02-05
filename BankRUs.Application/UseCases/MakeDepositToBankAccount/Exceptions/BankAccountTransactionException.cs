using System.Diagnostics;

namespace BankRUs.Application.UseCases.MakeDepositToBankAccount.Exceptions;

public class BankAccountTransactionException : Exception
{
    public string? Caller { get; init; }
    public BankAccountTransactionException(string message) : base(message)
    {
        Caller = new StackTrace()?.GetFrame(1)?.GetMethod()?.ReflectedType?.Name;
    }
}
