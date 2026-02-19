using System;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.Tests.Infrastructure.Stubs;

public class TransactionServiceStub : ITransactionService
{
    public Task<CreateTransactionResult> CreateTransactionAsync(CreateTransactionRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<IQueryable<Transaction>> GetTransactionsAsync(TransactionsPageQuery query)
    {
        throw new NotImplementedException();
    }
}
