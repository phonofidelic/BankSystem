using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Exceptions;

public class CustomerAccountDetailsValidationException(string message = "Customer account details contains invalid data.") : BadRequestException(message)
{
}
