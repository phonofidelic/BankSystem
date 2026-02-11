using BankRUs.Application.Services.PaginationService;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.Services.TransactionService;

public record TransactionsPageQuery(
    Guid? BankAccountId,
    DateTime? StartPeriodUtc,
    DateTime? EndPeriodUtc,
    TransactionType? Type,
    int Page,
    int PageSize,
    SortOrder SortOrder) : BasePageQuery(Page, PageSize, SortOrder);
