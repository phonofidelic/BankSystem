using BankRUs.Application.Services.PaginationService;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.Services.TransactionService;

public record TransactionsPageQuery(
    Guid? BankAccountId,
    DateTime? StartPeriodUtc,
    DateTime? EndPeriodUtc,
    TransactionType? Type,
    int Page = 1,
    int PageSize = 50,
    SortOrder SortOrder = SortOrder.Descending) : BasePageQuery(Page, PageSize, SortOrder);
