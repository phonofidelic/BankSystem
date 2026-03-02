using BankRUs.Application.Services.PaginationService;
using BankRUs.Application.Services.TransactionService;

namespace BankRUs.Application.UseCases.ListTransactionsForBankAccount;

public record ListTransactionsForBankAccountQuery(
    Guid CustomerId,
    Guid BankAccountId,
    TransactionsPageQuery TransactionsPageQuery) : BasePageQuery(
        Page: TransactionsPageQuery.Page, 
        Size: TransactionsPageQuery.Size, 
        Order: TransactionsPageQuery.Order);
