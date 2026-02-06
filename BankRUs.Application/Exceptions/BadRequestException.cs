using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Exceptions;

public class BadRequestException(string message) : Exception(message)
{
}
