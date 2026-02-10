using BankRUs.Application.Pagination;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.UseCases.ListTransactionsForBankAccount;

public record ListTransactionsForBankAccountResult(
    Guid BankAccountId,
    Currency Currency,
    decimal CurrentBalance,
    PagedResult<Transaction> QueryResult);
