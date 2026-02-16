using BankRUs.Application.BankAccounts;
using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.EmailService;
using BankRUs.Application.Services.Identity;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.UseCases.CloseCustomerAccount;

public class CloseCustomerAccountHandler(
    IUnitOfWork unitOfWork,
    IIdentityService identityService,
    ICustomerService customerService,
    ITransactionService transactionService,
    IBankAccountsRepository bankAccountsRepository,
    IEmailSender emailSender
) : IHandler<CloseCustomerAccountCommand, CloseCustomerAccountResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IIdentityService _identityService = identityService;
    private readonly ICustomerService _customerService = customerService;
    private readonly ITransactionService _transactionService = transactionService;
    private readonly IBankAccountsRepository _bankAccountRepository = bankAccountsRepository;
    private readonly IEmailSender _emailSender = emailSender;
    public async Task<CloseCustomerAccountResult> HandleAsync(CloseCustomerAccountCommand command)
    {
        // A Customer account can be closed if...

        // 1) all bank accounts have a zero balance
        var customerAccount = await _customerService.GetCustomerAsync(command.CustomerAccountId);
        
        var bankAccounts = customerAccount.GetBankAccounts();
        
        List<Transaction> closingTransactions = [];
        
        // ToDo: Move to service?
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

            bankAccount.AddTransaction(closingTransaction);
            bankAccount.Close();

            closingTransactions.Add(closingTransaction);
        }

        // Remove the ApplicationUser
        await _identityService.DeleteApplicationUserAsync(customerAccount.ApplicationUserId);

        // Create confirmation email with relevant data
        var confirmationEmail = new CloseCustomerAccountConfirmationEmail(
            to: customerAccount.Email,
            from: "customerservice@bank.example.com",
            closingDate: DateTime.UtcNow,
            closingTransactions: closingTransactions
        );

        // Close the Customer account
        customerAccount.Close();

        // Send confirmation email
        await _emailSender.SendEmailAsync(confirmationEmail);

        await _unitOfWork.SaveAsync();
        return new CloseCustomerAccountResult();
    }
}
