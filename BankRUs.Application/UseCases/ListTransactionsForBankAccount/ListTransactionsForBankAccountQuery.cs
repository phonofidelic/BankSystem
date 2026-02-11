using BankRUs.Domain.Entities;

namespace BankRUs.Application.UseCases.ListTransactionsForBankAccount;

public record ListTransactionsForBankAccountQuery(
    Guid CustomerId,
    // TransactionsPageQuery
    Guid BankAccountId,
    DateTime? StartPeriodUtc = null,
    DateTime? EndPeriodUdc = null,
    TransactionType? Type = null,
    // BasePageQuery
    int Page = 1,
    int PageSize = 20,
    SortOrder? SortOrder = SortOrder.Descending
    );
