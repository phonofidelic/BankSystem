using BankRUs.Application.Services.PaginationService;
using BankRUs.Domain.Entities;

namespace BankRUs.Api.Dtos.BankAccounts;

public record ListTransactionsResponseDto(
    Guid AccountId,
    string Currency,
    decimal Balance,
    PagedResultMetadata Paging,
    IReadOnlyList<CustomerTransactionsListItemDto> Items);

public record CustomerTransactionsListItemDto(
    Guid TransationId,
    string Type,
    decimal Amount,
    DateTime CreatedAt,
    decimal BalanceAfter,
    string? Reference);