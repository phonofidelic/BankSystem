using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Exceptions
{
    public class DuplicateCustomerException(string message, string paramName) : ArgumentException(message, paramName)
    {
    }
}

