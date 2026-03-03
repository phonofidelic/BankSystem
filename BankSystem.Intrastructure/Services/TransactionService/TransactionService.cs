using BankSystem.Application.Services.PaginationService;
using BankSystem.Application.Services.TransactionService;
using BankSystem.Domain.Entities;
using BankSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Infrastructure.Services.TransactionService
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

        public async Task<IQueryable<Transaction>> GetTransactionsAsync(TransactionsPageQuery query)
        {
            var transactions = _context.Transactions.AsNoTracking();

            if (query.Search != null)
                transactions = transactions.Where(t =>
                    t.Reference == null || t.Reference.Contains(query.Search))
                    .AsQueryable();

            if (query.BankAccountId != null)
                transactions = transactions.Where(t => t.BankAccountId == query.BankAccountId);

            transactions = query.Order == SortOrder.Ascending
                ? transactions.OrderBy(t => t.CreatedAt)
                : transactions.OrderByDescending(t => t.CreatedAt);

            if (query.Start != null)
                transactions = transactions.Where(t => t.CreatedAt >= query.Start);

            if (query.End != null)
                transactions = transactions.Where(t => t.CreatedAt <= query.End);

            if (query.Type != null)
                transactions = transactions.Where(t => t.Type == query.Type);

            return transactions;
        }
    }
}
