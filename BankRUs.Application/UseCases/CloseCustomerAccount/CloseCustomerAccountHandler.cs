using BankRUs.Application.BankAccounts;
using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.EmailService;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.UseCases.CloseCustomerAccount;

public class CloseCustomerAccountHandler(
    IUnitOfWork unitOfWork,
    ICustomerService customerService,
    ITransactionService transactionService,
    IBankAccountsRepository bankAccountsRepository,
    IEmailSender emailSender
) : IHandler<CloseCustomerAccountCommand, CloseCustomerAccountResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICustomerService _customerService = customerService;
    private readonly ITransactionService _transactionService = transactionService;
    private readonly IBankAccountsRepository _bankAccountRepository = bankAccountsRepository;
    private readonly IEmailSender _emailSender = emailSender;
    public async Task<CloseCustomerAccountResult> HandleAsync(CloseCustomerAccountCommand command)
    {
        // A Customer account cana be closed if...

        // 1) all bank accounts have a zero balance
        var customerAccount = await _customerService.GetCustomerAsync(command.CustomerAccountId);
        
        var bankAccounts = customerAccount.GetBankAccounts();
        
       List<Transaction> closingTransactions = [];
        
        foreach(var bankAccount in bankAccounts)
        {
            var createTransactionRequest = new CreateTransactionRequest
            (
                CustomerId: customerAccount.Id,
                BankAccountId: bankAccount. Id,
                Amount: bankAccount.Balance,
                Currency: bankAccount.Currency,
                Reference: string.Format("Closing transaction for Bank account {0}", bankAccount.Name),
                Type: TransactionType.Withdrawal
            );

            var createTransactionResult = await _transactionService.CreateTransactionAsync(createTransactionRequest);
            
            var closingTransaction = createTransactionResult.Transaction;

            bankAccount.Close(closingTransaction);

            closingTransactions.Add(closingTransaction);

            await _bankAccountRepository.RemoveBankAccount(bankAccount);

        }
        
        customerAccount.Remove(_customerService.RemoveCustomerAccount);

        await _unitOfWork.SaveAsync();

        // Send confirmation email
        var confirmationEmail = new CloseCustomerAccountConfirmationEmail(
            to: customerAccount.Email,
            from: "customerservice@bank.example.com",
            closingDate: DateTime.UtcNow,
            closingTransactions: closingTransactions
        );

        await _emailSender.SendEmailAsync(confirmationEmail);

        return new CloseCustomerAccountResult();
    }
}
