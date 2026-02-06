using BankRUs.Application.Services.AuditLog;

namespace BankRUs.Api.Dtos.BankAccounts;

public record PostWithdrawalResponseDto(
    Guid TransactionId,
    Guid CustomerId,
    string Type,
    decimal Amount,
    string Currency,
    string? Reference,
    DateTime CreatedAt,
    decimal BalanceAfter,
    IEnumerable<AuditLog> AuditLog);