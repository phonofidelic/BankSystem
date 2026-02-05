using BankRUs.Application.BankAccounts;
using BankRUs.Application.GuardClause;
using BankRUs.Application.Services.AuditLog;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Application.UseCases.MakeDepositToBankAccount.Guards;

namespace BankRUs.Application.UseCases.MakeDepositToBankAccount;

public class MakeDepositToBankAccountHandler(
    IBankAccountsRepository bankAccountsRepository,
    ITransactionService transactionService,
    IAuditLogger auditLogger) : IHandler<MakeDepositToBankAccountCommand, MakeDepositToBankAccountResult>
{
    private readonly IBankAccountsRepository _bankAccountRepository = bankAccountsRepository;
    private readonly ITransactionService _transactionService = transactionService;

    public async Task<MakeDepositToBankAccountResult> HandleAsync(MakeDepositToBankAccountCommand command)
    {
        // A Bank Deposit can be made if...

        // 1) The Customer owns the Bank Account
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

        var createTransactionResult = await _transactionService.CreateTransactionAsync(new CreateTransactionRequest(
            CustomerId: command.CustomerId,
            BankAccountId: command.BankAccountId,
            Amount: sanitizedAmount,
            Currency: command.Currency,
            Reference: sanitizedReference));

        return createTransactionResult == null
            ? throw new Exception("Deposit transaction could not be made")
            : new MakeDepositToBankAccountResult(
                TransactionId: createTransactionResult.Transaction.Id,
                CustomerId: createTransactionResult.Transaction.CustomerId,
                Type: createTransactionResult.Transaction.Type,
                Amount: createTransactionResult.Transaction.Amount,
                Currency: createTransactionResult.Transaction.Currency.ToString(),
                Reference: createTransactionResult.Transaction.Reference,
                CreatedAt: createTransactionResult.Transaction.CreatedAt);
    }
}
