using System;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using Bogus;

namespace BankRUs.Application.Tests.Infrastructure.Stubs;

public class TransactionServiceStub : ITransactionService
{
    public async Task<CreateTransactionResult> CreateTransactionAsync(CreateTransactionRequest request)
    {
        return new CreateTransactionResult(new Transaction {
            Id = Guid.NewGuid(),
            Type = request.Type,
            BankAccountId = request.BankAccountId,
            CustomerId = request.CustomerId,
            Currency = request.Currency,
            Amount = request.Amount,
            Reference = request.Reference
        });
    }

    public Task<IQueryable<Transaction>> GetTransactionsAsync(TransactionsPageQuery query)
    {
        throw new NotImplementedException();
    }
}
