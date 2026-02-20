using BankRUs.Application.Services.PaginationService;

namespace BankRUs.Api.Dtos.Transactions;

public record GetTransactionsResponseDto(
    PagedResultMetadata Paging,
    IReadOnlyList<TransactionsListItemDto> Items);

public record TransactionsListItemDto(
    Guid TransactionId,
    string Type,
    decimal Amount,
    DateTime CreatedAt,
    decimal BalanceAfter,
    string Currency,
    string? Reference);
