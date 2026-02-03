using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Api.Dtos.BankAccounts;

public record PostDepositResponseDto(
    Guid TransactionId,
    Guid CustomerId,
    TransactionType Type,
    decimal Amount,
    string Currency,
    string Reference,
    DateTime CreatedAt,
    decimal BalanceAfter);
