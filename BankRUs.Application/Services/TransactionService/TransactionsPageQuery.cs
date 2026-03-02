using BankRUs.Application.Services.PaginationService;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.Services.TransactionService;

public record TransactionsPageQuery(
    string? Search,
    Guid? BankAccountId,
    DateTime? Start,
    DateTime? End,
    TransactionType? Type,
    int Page,
    int Size,
    SortOrder Order) : BasePageQuery(Page, Size, Order: Order);
