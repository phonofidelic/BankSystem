namespace BankRUs.Application.Services.CustomerService;

public sealed record CreateCustomerAccountRequest
(
    Guid ApplicationUserId,
    string SocialSecurityNumber
);
