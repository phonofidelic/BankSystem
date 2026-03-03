using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystem.Application.Exceptions;

public class UnauthorizedException(string message) : Exception(message)
{
}
