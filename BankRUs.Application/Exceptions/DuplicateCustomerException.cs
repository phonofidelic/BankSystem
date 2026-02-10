using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Exceptions
{
    public class DuplicateCustomerException : BadRequestException
    {
        public DuplicateCustomerException(string message, string errorCode) : base(string.Format("{0}: {1}", message, errorCode))
        { 
        }

        public DuplicateCustomerException() : base(message: "Account already exists") 
        {
        }
    }
}

