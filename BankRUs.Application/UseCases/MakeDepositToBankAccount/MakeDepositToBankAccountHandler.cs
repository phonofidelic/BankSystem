using BankRUs.Application.BankAccounts;
using BankRUs.Application.Exceptions;
using BankRUs.Application.GuardClause;
using BankRUs.Application.Guards;
using BankRUs.Application.Services.AuditLog;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.UseCases.MakeDepositToBankAccount;

public class MakeDepositToBankAccountHandler(
    IUnitOfWork unitOfWork,
    IBankAccountsRepository bankAccountsRepository,
    ITransactionService transactionService,
    IAuditLogger auditLogger) : IHandler<MakeDepositToBankAccountCommand, MakeDepositToBankAccountResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBankAccountsRepository _bankAccountRepository = bankAccountsRepository;
    private readonly ITransactionService _transactionService = transactionService;

    public async Task<MakeDepositToBankAccountResult> HandleAsync(MakeDepositToBankAccountCommand command)
    {
        // A Bank Deposit can be made if...

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

        // Get the result from the Transaction service
        var createTransactionResult = await _transactionService.CreateTransactionAsync(new CreateTransactionRequest(
            CustomerId: command.CustomerId,
            BankAccountId: command.BankAccountId,
            Type: TransactionType.Deposit,
            Amount: sanitizedAmount,
            Currency: command.Currency,
            Reference: sanitizedReference));

        // Get the Transaction instance
        var createdTransaction = createTransactionResult.Transaction;

        // Post the transaction to update the bank account balance
        await _bankAccountRepository.UpdateBankAccountBalanceWithTransactionAsync(createdTransaction);

        // Retrieve the new balance
        var balanceAfter = await _bankAccountRepository.GetBankAccountBalance(createTransactionResult.Transaction.BankAccountId);

        // Update the Transaction with updated balanceAfter
        createdTransaction.UpdateBalanceAfter(balanceAfter);

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
