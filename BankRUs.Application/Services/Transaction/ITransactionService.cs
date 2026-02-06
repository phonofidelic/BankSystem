using BankRUs.Domain.Entities;

namespace BankRUs.Application.Services.TransactionService
{
    public interface ITransactionService
    {
        public Task<CreateTransactionResult> CreateTransactionAsync(CreateTransactionRequest request);

        public Task<PagedResult<Transaction>> GetTransactionsAsPagedResult(TransactionsPageQuery query);
    }
}
