namespace BankRUs.Api.Dtos.CustomerAccounts;

public record GetCustomerAccountResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Ssn,
    string Email,
    string AccountStatus,
    IReadOnlyList<CustomerBankAccountListItemDto> BankAccounts);

public record CustomerBankAccountListItemDto(
    Guid Id,
    string Name,
    decimal CurrentBalance,
    string Currency,
    DateTime OpenedAt,
    string AccountStatus);