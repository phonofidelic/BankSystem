using BankRUs.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.CustomerService.GetBankAccount;

public class CompleteCustomerAccountDetails(
    string FirstName,
    string LastName,
    string Email,
    string SocialSecurityNumber) : CustomerAccountDetails(FirstName, LastName, Email, SocialSecurityNumber);
