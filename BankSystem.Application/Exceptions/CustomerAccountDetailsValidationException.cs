using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystem.Application.Exceptions;

public class CustomerAccountDetailsValidationException(string message = "Customer account details contains invalid data.") : BadRequestException(message)
{
}
