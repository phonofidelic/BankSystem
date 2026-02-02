using BankRUs.Domain.Entities;

namespace BankRUs.Api.Dtos.BankAccounts;

public record GetBankAccountsResponseDto(IEnumerable<CustomerBankAccountDto> Accounts);

public record CustomerBankAccountDto(
    Guid Id,
    Guid CustomerId,
    string AccountName,
    decimal Balance,
    DateTime OpenedAt,
    DateTime UpdatedAt);