using BankRUs.Application;
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
                Currency = Currency.Parse(request.Currency, _appSettings.SupportedCurrencies),
                Reference = request.Reference
            };

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return new CreateTransactionResult(transaction);
        }

        public async Task<PagedResult<Transaction>> GetTransactionsAsPagedResult(TransactionsPageQuery query)
        {
            var transactions = _context.Transactions.Where(t => t.BankAccountId == query.BankAccountId);

            transactions = query.SortOrder == SortOrder.Ascending 
                ? transactions.OrderBy(t => t.CreatedAt)
                : transactions.OrderByDescending(t => t.CreatedAt);

            if (query.StartPeriod != null)
                transactions = transactions.Where(t => t.CreatedAt >= query.StartPeriod);

            if (query.EndPeriod != null)
                transactions = transactions.Where(t => t.CreatedAt <= query.EndPeriod);

            if (query.Type != null)
                transactions = transactions.Where(t => t.Type == query.Type);

            var totalItems = transactions.Count();
            var totalPages = totalItems / query.PageSize;
            var items = await transactions
                .Skip(query.Skip).Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<Transaction>
            (
                Items: items,
                Meta: new PageMetadata(
                    Page: query.Offset,
                    PageSize: query.PageSize,
                    TotalCount: totalItems,
                    TotalPages: totalPages
                    )
            );
        }
    }
}
