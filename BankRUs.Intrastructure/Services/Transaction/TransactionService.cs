using BankRUs.Application;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using BankRUs.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

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
    }
}
