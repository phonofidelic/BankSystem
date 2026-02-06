using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}
