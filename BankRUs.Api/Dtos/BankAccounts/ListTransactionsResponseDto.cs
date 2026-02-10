
using BankRUs.Application.Paginatioin;
using BankRUs.Domain.Entities;

namespace BankRUs.Api.Dtos.BankAccounts;

public record ListTransactionsResponseDto(
    Guid AccountId,
    string Currency,
    decimal Balance,
    PagedResultMetadata Paging,
    IReadOnlyList<CustomerTransactionDto> Items);

public record CustomerTransactionDto(
    Guid TransationId,
    string Type,
    decimal Amount,
    DateTime CreatedAt,
    decimal BalanceAfter,
    string? Reference);