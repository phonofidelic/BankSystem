using BankSystem.Application.Services.PaginationService;
using BankSystem.Domain.Entities;

namespace BankSystem.Application.Services.TransactionService
{
    public interface ITransactionService
    {
        public Task<CreateTransactionResult> CreateTransactionAsync(CreateTransactionRequest request);

        public Task<IQueryable<Transaction>> GetTransactionsAsync(TransactionsPageQuery query);
    }
}
