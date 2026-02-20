using System;
using BankRUs.Application.Services.PaginationService;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.UseCases.ListAllTransactions;

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
