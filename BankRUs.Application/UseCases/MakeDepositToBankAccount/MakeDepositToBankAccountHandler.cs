using BankRUs.Application.Services.TransactionService;

namespace BankRUs.Application.UseCases.MakeDepositToBankAccount;

public class MakeDepositToBankAccountHandler(ITransactionService transactionService) : IHandler<MakeDepositeToBankAccountCommand, MakeDepositToBankAccountResult>
{
    private readonly ITransactionService _transactionService = transactionService;

    public async Task<MakeDepositToBankAccountResult> HandleAsync(MakeDepositeToBankAccountCommand command)
    {
        // ToDo: Validate business rules for making a Bank Deposit Transaction

        // A Bank Deposit can be made if...

        // 1) The Customer has a Customer Account

        // 2) The Customer has a Bank Account

        // 3) The Deposit Amount is a positive decimal

        // 4) The Deposit Reference message has no more than 140 characters

        var createTransactionResult = await _transactionService.CreateTransactionAsync(new CreateTransactionRequest(
            CustomerId: command.CustomerId,
            BankAccountId: command.BankAccountId,
            Amount: command.Amount,
            Currency: command.Currency,
            Reference: command.Reference));

        return createTransactionResult == null
            ? throw new Exception("Deposit transaction could not be made")
            : new MakeDepositToBankAccountResult(
                TransactionId: createTransactionResult.Transaction.Id,
                CustomerId: createTransactionResult.Transaction.CustomerId,
                Type: createTransactionResult.Transaction.Type,
                Amount: createTransactionResult.Transaction.Amount,
                Currency: createTransactionResult.Transaction.Currency.ToString());
    }
}
