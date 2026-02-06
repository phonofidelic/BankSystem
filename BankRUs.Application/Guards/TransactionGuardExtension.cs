using BankRUs.Application.Exceptions;
using BankRUs.Application.GuardClause;
using BankRUs.Application.Services.AuditLog;

namespace BankRUs.Application.Guards;

// ToDo: Move core business rules to Domain?
// Implementation should live in the entities and value objects of the Domain?
public static class TransactionGuardExtension
{
    public static void BankAccountOverdraft(this IGuardClause _, decimal currentBalance, decimal withdrawalAmount)
    {
        if (currentBalance < withdrawalAmount)
            throw new BankAccountTransactionException("Insufficient funds");
    }
    public static void BankAccountNotOwned(this IGuardClause _, Guid bankAccountOwnerId, Guid customerId)
    {
        if (bankAccountOwnerId != customerId)
            throw new BankAccountTransactionException(string.Format("The provided Bank account is not owned by Customer with Id {0}", customerId));
    }
    public static string? MaxReferenceLength(this IGuardClause guardClause, string? input)
    {
        return MaxLength(guardClause, input, 140);
    }
    public static decimal NegativeAmount(this IGuardClause guardClause, decimal input)
    {
        return Negative(guardClause, input, 0);
    }

    public static decimal ZeroAmount(this IGuardClause guardClause, decimal input)
    {
        return Zero(guardClause, input, 0);
    }

    public static decimal RoundToNearestHundredth(
        this IGuardClause guardClause, 
        decimal input, 
        IAuditLogger auditLogger)
    {
        decimal roundedInput = decimal.Round(input, 2, MidpointRounding.AwayFromZero);
        if (input != roundedInput)
        {
            auditLogger.Log(string.Format("Rounded {0} to {1}", input, roundedInput));
            return roundedInput;
        }

        return input;
    }

    private static T Negative<T>(this IGuardClause _, T input, T compareTo)
    {
        Comparer<T> comparer = Comparer<T>.Default;

        if (comparer.Compare(input, compareTo) < 0)
        {
            throw new BankAccountTransactionException("Transaction amount cannot be negative");
        }

        return input;
    }

    private static T Zero<T>(this IGuardClause _, T input, T compareTo)
    {
        Comparer<T> comparer = Comparer<T>.Default;

        if (comparer.Compare(input, compareTo) == 0)
        {
            throw new BankAccountTransactionException("Transaction amount must be greater than zero");
        }

        return input;
    }

    private static string? MaxLength(this IGuardClause _, string? input, int maxLength)
    {
        //if (input == null) return string.Empty;

        if (input?.Length > maxLength)
        {
            throw new BankAccountTransactionException(string.Format("Reference message exceeds maximum length of {0} characters", maxLength));
        }

        return input;
    }
}
