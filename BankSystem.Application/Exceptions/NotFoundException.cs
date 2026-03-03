using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystem.Application.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}
