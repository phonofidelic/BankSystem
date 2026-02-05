using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.OpenAccount.Exceptions
{
    public class DuplicateCustomerException(string message, string paramName) : ArgumentException(message, paramName)
    {
    }
}

