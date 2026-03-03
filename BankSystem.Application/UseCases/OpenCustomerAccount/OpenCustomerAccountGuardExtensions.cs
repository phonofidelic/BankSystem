using BankSystem.Application.Exceptions;
using BankSystem.Application.GuardClause;

namespace BankSystem.Application.UseCases.OpenCustomerAccount;

public static class OpenCustomerAccountGuardExtensions
{
    public static string DuplicateCustomer(this IGuardClause _, string input, Func<string, bool> customerExists)
    {
        if (customerExists(input))
        {
            throw new DuplicateCustomerException();
        }
        return input;
    }
}
