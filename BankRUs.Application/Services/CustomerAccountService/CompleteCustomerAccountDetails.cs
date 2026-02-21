using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.Services.CustomerAccountService;

public class CompleteCustomerAccountDetails(
    string FirstName,
    string LastName,
    string Email,
    string SocialSecurityNumber) : CustomerAccountDetails(FirstName, LastName, Email, SocialSecurityNumber);
