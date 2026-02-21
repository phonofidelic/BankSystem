namespace BankRUs.Application.Services.CustomerAccountService;

public sealed record CreateCustomerAccountRequest
(
    Guid ApplicationUserId,
    string SocialSecurityNumber
);
