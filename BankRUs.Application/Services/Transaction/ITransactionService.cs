using BankRUs.Application.Pagination;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.Services.TransactionService
{
    public interface ITransactionService
    {
        public Task<CreateTransactionResult> CreateTransactionAsync(CreateTransactionRequest request);

        public Task<PagedResult<Transaction>> GetTransactionsAsPagedResultAsync(TransactionsPageQuery query);

        public Task<decimal> GetBalanceAfterAsync(Guid bankAccountId, Guid transactionId);
    }
}
