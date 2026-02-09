
using BankRUs.Application.Pagination;
using BankRUs.Domain.Entities;

namespace BankRUs.Api.Dtos.BankAccounts;

public record ListTransactionsResponseDto(
    Guid AccountId,
    string Currency,
    decimal Balance,
    IReadOnlyList<Transaction> Items,
    PagedResultMetadata Paging);
