using BankSystem.Application.Services.PaginationService;
using BankSystem.Application.Services.TransactionService;

namespace BankSystem.Application.UseCases.ListTransactionsForBankAccount;

public record ListTransactionsForBankAccountQuery(
    Guid CustomerId,
    Guid BankAccountId,
    TransactionsPageQuery TransactionsPageQuery) : BasePageQuery(
        Page: TransactionsPageQuery.Page, 
        Size: TransactionsPageQuery.Size, 
        Order: TransactionsPageQuery.Order);
