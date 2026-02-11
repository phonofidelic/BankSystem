using BankRUs.Application.Services.PaginationService;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.UseCases.ListTransactionsForBankAccount;

public record ListTransactionsForBankAccountResult(
    Guid BankAccountId,
    Currency Currency,
    decimal CurrentBalance,
    BasePagedResult<Transaction> QueryResult);
