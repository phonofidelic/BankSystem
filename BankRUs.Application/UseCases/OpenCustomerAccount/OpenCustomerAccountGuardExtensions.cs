using BankRUs.Application.Exceptions;
using BankRUs.Application.GuardClause;

namespace BankRUs.Application.UseCases.OpenCustomerAccount;

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
