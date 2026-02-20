using BankRUs.Application.Services.PaginationService;

namespace BankRUs.Api.Dtos.BankAccounts;

public record GetTransactionsForBankAccountResponseDto(
    Guid AccountId,
    string Currency,
    decimal Balance,
    PagedResultMetadata Paging,
    IReadOnlyList<CustomerTransactionsListItemDto> Items);

public record CustomerTransactionsListItemDto(
    Guid TransactionId,
    string Type,
    decimal Amount,
    DateTime CreatedAt,
    decimal BalanceAfter,
    string? Reference);