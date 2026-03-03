using BankSystem.Domain.Entities;

namespace BankSystem.Application.Services.CustomerAccountService;

public record OpenCustomerAccountRequest(
    CustomerAccount CustomerAccount,
    CompleteCustomerAccountDetails CustomerAccountDetails,
    BankAccount DefaultBankAccount,
    Guid ApplicationUserId);
