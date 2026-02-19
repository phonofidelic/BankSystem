using BankRUs.Application.BankAccounts;
using BankRUs.Application.Exceptions;
using BankRUs.Application.GuardClause;
using BankRUs.Application.Guards;
using BankRUs.Application.Services.AuditLog;
using BankRUs.Application.Services.CurrencyService;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.UseCases.MakeDepositToBankAccount;

public class MakeDepositToBankAccountHandler(
    IUnitOfWork unitOfWork,
    IBankAccountsRepository bankAccountsRepository,
    ICurrencyService currencyService,
    ITransactionService transactionService,
    IAuditLogger auditLogger) : IHandler<MakeDepositToBankAccountCommand, MakeDepositToBankAccountResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBankAccountsRepository _bankAccountRepository = bankAccountsRepository;
    private readonly ICurrencyService _currencyService = currencyService;
    private readonly ITransactionService _transactionService = transactionService;

    public async Task<MakeDepositToBankAccountResult> HandleAsync(MakeDepositToBankAccountCommand command)
    {
        // A Bank Deposit can be made if...

        // 1) The Bank Account exists
        bool bankAccountexists = _bankAccountRepository.BankAccountExists(command.BankAccountId);
        if (!bankAccountexists) throw new BankAccountNotFoundException();

        // 2) The Customer owns the Bank Account
        var bankAccountOwnerId = await _bankAccountRepository.GetCustomerIdForBankAccountAsync(command.BankAccountId);
        var sanitizedCustomerId = Guard.Against.BankAccountNotOwned(command.CustomerId, bankAccountOwnerId);

        // 3) The Deposit Amount is a positive decimal
        decimal sanitizedAmount = command.Amount;
        sanitizedAmount = Guard.Ensure.RoundToNearestHundredth(sanitizedAmount, auditLogger);
        sanitizedAmount = Guard.Against.NegativeAmount(sanitizedAmount);
        sanitizedAmount = Guard.Against.ZeroAmount(sanitizedAmount);

        // 4) The Deposit Reference message has no more than 140 characters
        string? sanitizedReference = command.Reference;
        sanitizedReference = Guard.Against.MaxReferenceLength(sanitizedReference);

        // 5) The Bank Account supports the provided Currency
        var parsedCurrency = _currencyService.ParseIsoSymbol(command.Currency);
        var bankAccountCurrency = await _bankAccountRepository.GetBankAccountCurrency(command.BankAccountId);
        var sanitizedTransactionCurrency = Guard.Against.BankAccountUnsupportedCurrency(parsedCurrency, bankAccountCurrency);

        // Get the result from the Transaction service
        var createTransactionResult = await _transactionService.CreateTransactionAsync(new CreateTransactionRequest(
            CustomerId: sanitizedCustomerId,
            BankAccountId: command.BankAccountId,
            Type: TransactionType.Deposit,
            Amount: sanitizedAmount,
            Currency: sanitizedTransactionCurrency,
            Reference: sanitizedReference));

        // Get the Transaction instance
        var createdTransaction = createTransactionResult.Transaction;

        var bankAccount = await _bankAccountRepository.GetBankAccountAsync(command.BankAccountId);

        // Transaction functionality is implemented in BankAccount and Transaction entities
        bankAccount.AddTransaction(createdTransaction);

        // Complete unit of work
        await _unitOfWork.SaveAsync();

        return createTransactionResult == null
            ? throw new Exception("Deposit transaction could not be made")
            : new MakeDepositToBankAccountResult(
                TransactionId: createdTransaction.Id,
                CustomerId: createdTransaction.CustomerId,
                Type: createdTransaction.Type,
                Amount: createdTransaction.Amount,
                BalanceAfter: createdTransaction.BalanceAfter,
                Currency: createdTransaction.Currency.ToString(),
                Reference: createdTransaction.Reference,
                CreatedAt: createdTransaction.CreatedAt);
    }
}
