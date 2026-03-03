using System;

namespace BankSystem.Api.Tests.Exceptions;

public class CreateTestCustomerAccountException() : Exception(message: "Could not create Customer account in test")
{

}
