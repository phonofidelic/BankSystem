using BankRUs.Application.Services.CustomerService.GetBankAccount;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.Services.CustomerService;

public record OpenCustomerAccountRequest(
    Customer CustomerAccount,
    CompleteCustomerAccountDetails CustomerAccountDetails,
    Guid ApplicationUserId);
