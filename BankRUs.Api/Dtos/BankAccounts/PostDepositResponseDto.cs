using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Api.Dtos.BankAccounts;

public record PostDepositResponseDto(
    Guid TransactionId,
    Guid CustomerId,
    string Type,
    decimal Amount,
    string Currency,
    string Reference,
    DateTime CreatedAt,
    decimal BalanceAfter);
