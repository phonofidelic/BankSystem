using BankSystem.Application.Services.PaginationService;
using BankSystem.Domain.Entities;

namespace BankSystem.Application.Services.TransactionService;

public record TransactionsPageQuery(
    string? Search,
    Guid? BankAccountId,
    DateTime? Start,
    DateTime? End,
    TransactionType? Type,
    int Page,
    int Size,
    SortOrder Order) : BasePageQuery(Page, Size, Order: Order);
