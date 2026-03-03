using BankSystem.Application.Services.AuditLog;


namespace BankSystem.Api.Dtos.BankAccounts;

public record PostDepositResponseDto(
    Guid TransactionId,
    Guid CustomerId,
    string Type,
    decimal Amount,
    string Currency,
    string? Reference,
    DateTime CreatedAt,
    decimal BalanceAfter,
    IEnumerable<AuditLog> AuditLog);
