using BankRUs.Application;
using BankRUs.Application.Exceptions;
using BankRUs.Application.Pagination;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using BankRUs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BankRUs.Infrastructure.Services.TransactionService
{
    public class TransactionService(ApplicationDbContext context, IOptions<AppSettings> appSettings) : ITransactionService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly AppSettings _appSettings = appSettings.Value;
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

        public async Task<decimal> GetBalanceAfter(Guid bankAccountId, Guid transactionId)
        {
            var targetTransaction = await _context.Transactions.FindAsync(transactionId) ?? throw new TransactionNotFoundException();
            
            return await _context.Transactions
                .Where(t => t.BankAccountId == bankAccountId)
                .OrderBy(t => t.CreatedAt)
                .Where(t => t.BalanceAfter == null)
                .Where(t => t.CreatedAt <= targetTransaction.CreatedAt)
                .Where(t => t.Type == TransactionType.Deposit)
                .SumAsync(t => t.Value);
        }

        public Task<decimal> GetBalanceAfterAsync(Guid bankAccountId, Guid transactionId)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<Transaction>> GetTransactionsAsPagedResultAsync(TransactionsPageQuery query)
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

            var totalItems = transactions.Count();
            var totalPages = (totalItems / query.PageSize) + 1;

            var items = await transactions
                .Skip(query.Skip).Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<Transaction>
            (
                Items: items,
                Meta: new PagedResultMetadata(
                    Page: query.Page,
                    PageSize: query.PageSize,
                    TotalCount: totalItems,
                    TotalPages: totalPages,
                    Sort: query.SortOrder.ToString().ToLower())
            );
        }
    }
}
