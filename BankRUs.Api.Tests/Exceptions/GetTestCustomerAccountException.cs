using System;

namespace BankRUs.Api.Tests.Exceptions;

public class GetTestCustomerAccountException() : Exception(message: "Could not get Customer account in test")
{

}
