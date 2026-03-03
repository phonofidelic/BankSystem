using BankSystem.Application.Services.PaginationService;
using BankSystem.Domain.Entities;
using BankSystem.Domain.ValueObjects;

namespace BankSystem.Application.UseCases.ListTransactionsForBankAccount;

public record ListTransactionsForBankAccountResult(
    Guid BankAccountId,
    Currency Currency,
    decimal CurrentBalance,
    BasePagedResult<Transaction> QueryResult);
