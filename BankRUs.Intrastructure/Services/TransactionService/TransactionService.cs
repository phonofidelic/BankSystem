using BankRUs.Application.Services.PaginationService;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Domain.Entities;
using BankRUs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankRUs.Infrastructure.Services.TransactionService
{
    public class TransactionService(ApplicationDbContext context) : ITransactionService
    {
        private readonly ApplicationDbContext _context = context;
        public async Task<CreateTransactionResult> CreateTransactionAsync(CreateTransactionRequest request)
        {
            Transaction transaction = new(request.Type) { 
                CustomerId = request.CustomerId,
                BankAccountId = request.BankAccountId,
                Amount = request.Amount,
                Currency = request.Currency,
                Reference = request.Reference
            };

            await _context.Transactions.AddAsync(transaction);

            return new CreateTransactionResult(transaction);
        }

        public Task<decimal> GetBalanceAfterAsync(Guid bankAccountId, Guid transactionId)
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryable<Transaction>> GetTransactionsAsync(TransactionsPageQuery query)
        {
            var transactions = _context.Transactions.AsQueryable();

            if (query.BankAccountId != null)
                transactions = transactions.Where(t => t.BankAccountId == query.BankAccountId);

            transactions = query.SortOrder == SortOrder.Ascending
                ? transactions.OrderBy(t => t.CreatedAt)
                : transactions.OrderByDescending(t => t.CreatedAt);

            if (query.StartPeriodUtc != null)
                transactions = transactions.Where(t => t.CreatedAt >= query.StartPeriodUtc);

            if (query.EndPeriodUtc != null)
                transactions = transactions.Where(t => t.CreatedAt <= query.EndPeriodUtc);

            if (query.Type != null)
                transactions = transactions.Where(t => t.Type == query.Type);

            return transactions;
        }
    }
}
