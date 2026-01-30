using BankRUs.Domain.Entities;

namespace BankRUs.Api.Dtos.Accounts;

public record GetBankAccountsForCustomerResponseDto(IEnumerable<CustomerBankAccountDto> Accounts);

public record CustomerBankAccountDto(
    Guid Id,
    Guid CustomerId,
    decimal Balance,
    DateTime OpenedAt,
    DateTime UpdatedAt);