
using BankRUs.Application.Pagination;
using BankRUs.Domain.Entities;

namespace BankRUs.Api.Dtos.BankAccounts;

public record ListTransactionsResponseDto(
    Guid AccountId,
    string Currency,
    decimal Balance,
    IReadOnlyList<CustomerTransactionDto> Items,
    PagedResultMetadata Paging);

public record CustomerTransactionDto(
    Guid TransationId,
    string Type,
    decimal Amount,
    DateTime CreatedAt,
    decimal BalanceAfter,
    string? Reference);