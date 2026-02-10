using BankRUs.Application.Paginatioin;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.Services.TransactionService
{
    public interface ITransactionService
    {
        public Task<CreateTransactionResult> CreateTransactionAsync(CreateTransactionRequest request);

        public Task<BasePagedResult<Transaction>> GetTransactionsAsPagedResultAsync(TransactionsPageQuery query);

        public Task<decimal> GetBalanceAfterAsync(Guid bankAccountId, Guid transactionId);
    }
}
