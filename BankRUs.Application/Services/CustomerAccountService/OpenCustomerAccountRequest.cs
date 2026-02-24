using BankRUs.Domain.Entities;

namespace BankRUs.Application.Services.CustomerAccountService;

public record OpenCustomerAccountRequest(
    CustomerAccount CustomerAccount,
    CompleteCustomerAccountDetails CustomerAccountDetails,
    BankAccount DefaultBankAccount,
    Guid ApplicationUserId);
