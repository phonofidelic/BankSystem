using BankRUs.Application.BankAccounts;
using BankRUs.Application.Exceptions;
using BankRUs.Application.GuardClause;
using BankRUs.Application.Guards;
using BankRUs.Application.Services.AuditLog;
using BankRUs.Application.Services.CurrencyService;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.UseCases.MakeWithdrawalFromBankAccount;

public class MakeWithdrawalFromBankAccountHandler(
    IUnitOfWork unitOfWork,
    IBankAccountsRepository bankAccountsRepository,
    ICurrencyService currencyService,
    ITransactionService transactionService,
    IAuditLogger auditLogger) : IHandler<MakeWithdrawalFromBankAccountCommand, MakeWithdrawalFromBankAccountResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBankAccountsRepository _bankAccountRepository = bankAccountsRepository;
    private readonly ICurrencyService _currencyService = currencyService;
    private readonly ITransactionService _transactionService = transactionService;

    public async Task<MakeWithdrawalFromBankAccountResult> HandleAsync(MakeWithdrawalFromBankAccountCommand command)
    {
        // A withdrawal can be made from a Bank Account if...

        // 1) The Bank Account exists
        bool bankAccountexists = _bankAccountRepository.BankAccountExists(command.BankAccountId);
        if (!bankAccountexists) throw new BankAccountNotFoundException();

        // 2) The Customer owns the Bank Account
        var bankAccountOwnerId = await _bankAccountRepository.GetCustomerIdForBankAccountAsync(command.BankAccountId);
        Guard.Against.BankAccountNotOwned(bankAccountOwnerId, command.CustomerId);

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
        var sanitizedCurrency = Guard.Against.BankAccountUnsupportedCurrency(parsedCurrency, bankAccountCurrency);

        // 6) The current balance covers the withdrawal amount
        var currentBankAccountBalance = await _bankAccountRepository.GetBankAccountBalance(command.BankAccountId);
        Guard.Against.BankAccountOverdraft(currentBankAccountBalance, command.Amount);

        // Get the result from the Transaction service
        var createTransactionResult = await _transactionService.CreateTransactionAsync(new CreateTransactionRequest(
            CustomerId: command.CustomerId,
            BankAccountId: command.BankAccountId,
            Type: TransactionType.Withdrawal,
            Amount: sanitizedAmount,
            Currency: sanitizedCurrency,
            Reference: sanitizedReference));

        // Get the Transaction instance
        var createdTransaction = createTransactionResult.Transaction;

        // Post the transaction to update the bank account balance
        await _bankAccountRepository.UpdateBankAccountBalanceWithTransactionAsync(createdTransaction);

        // Retrieve the new balance
        var balanceAfter = await _bankAccountRepository.GetBankAccountBalance(createdTransaction.BankAccountId);

        // Update the Transaction record with the new balance
        createdTransaction.UpdateBalanceAfter(balanceAfter);

        // Complete unit of work
        await _unitOfWork.SaveAsync();

        return createTransactionResult == null
            ? throw new Exception("Deposit transaction could not be made")
            : new MakeWithdrawalFromBankAccountResult(
                TransactionId: createTransactionResult.Transaction.Id,
                CustomerId: createTransactionResult.Transaction.CustomerId,
                Type: createTransactionResult.Transaction.Type,
                Amount: createTransactionResult.Transaction.Amount,
                BalanceAfter: balanceAfter,
                Currency: createTransactionResult.Transaction.Currency.ToString(),
                Reference: createTransactionResult.Transaction.Reference,
                CreatedAt: createTransactionResult.Transaction.CreatedAt);
    }
}
