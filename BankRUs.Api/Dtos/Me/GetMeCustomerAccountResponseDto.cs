namespace BankRUs.Api.Dtos.Me;

public record GetMeCustomerAccountResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Ssn,
    string Email,
    IReadOnlyList<MeCustomerBankAccountListItemDto> BankAccounts
);

public record MeCustomerBankAccountListItemDto(
    Guid Id,
    string Name,
    decimal CurrentBalance,
    string Currency,
    DateTime OpenedAt);