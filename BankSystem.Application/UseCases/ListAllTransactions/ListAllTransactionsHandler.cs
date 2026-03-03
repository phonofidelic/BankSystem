using System;
using BankSystem.Application.Services.PaginationService;
using BankSystem.Application.Services.TransactionService;
using BankSystem.Domain.Entities;

namespace BankSystem.Application.UseCases.ListAllTransactions;

public class ListAllTransactionsHandler(
    ITransactionService transactionsService,
    IPaginationService paginationService) : IHandler<TransactionsPageQuery, BasePagedResult<Transaction>>
{
    private readonly ITransactionService _transactionsService = transactionsService;
    private readonly IPaginationService _paginationService = paginationService;
    public async Task<BasePagedResult<Transaction>> HandleAsync(TransactionsPageQuery query)
    {
        var queryResult = await _transactionsService.GetTransactionsAsync(query);
        var paginationResult = await _paginationService.GetPagedResultAsync(query, queryResult);

        return paginationResult;
    }
}
