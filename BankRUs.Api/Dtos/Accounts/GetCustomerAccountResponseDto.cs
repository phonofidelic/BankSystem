namespace BankRUs.Api.Dtos.Accounts;

public record GetCustomerAccountResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Ssn,
    string Email,
    IReadOnlyList<CustomerBankAccountListItemDto> BankAccounts);

public record CustomerBankAccountListItemDto(
    Guid Id,
    string Name,
    decimal CurrentBalance,
    string Currency,
    DateTime OpenedAt);