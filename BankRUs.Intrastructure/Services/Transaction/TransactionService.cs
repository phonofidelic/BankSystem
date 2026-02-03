using BankRUs.Application;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using BankRUs.Infrastructure.Persistence;
using Microsoft.Extensions.Options;

namespace BankRUs.Infrastructure.Services.TransactionService
{
    public class TransactionService(ApplicationDbContext context, IOptions<CurrencyConfig> currencyConfig) : ITransactionService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly CurrencyConfig _currencyConfig = currencyConfig.Value;
        public async Task<CreateTransactionResult> CreateTransactionAsync(CreateTransactionRequest request)
        {
            Transaction transaction = new(TransactionType.Deposit) { 
                CustomerId = request.CustomerId,
                BankAccountId = request.BankAccountId,
                Amount = request.Amount,
                Currency = Currency.Parse(request.Currency, _currencyConfig.SupportedCurrencies),
                Reference = request.Reference
            };

            await _context.SaveChangesAsync();

            return new CreateTransactionResult(transaction);
        }
    }
}
