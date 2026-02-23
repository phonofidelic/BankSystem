using BankRUs.Application.Services.CustomerAccountService;
using BankRUs.Application.Services.EmailService;
using BankRUs.Application.Services.Identity;

namespace BankRUs.Application.UseCases.CloseCustomerAccount;

public class CloseCustomerAccountHandler(
    IUnitOfWork unitOfWork,
    IIdentityService identityService,
    ICustomerAccountService customerService,
    IEmailSender emailSender
) : IHandler<CloseCustomerAccountCommand, CloseCustomerAccountResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private readonly IIdentityService _identityService = identityService;

    private readonly ICustomerAccountService _customerService = customerService;

    private readonly IEmailSender _emailSender = emailSender;
    
    public async Task<CloseCustomerAccountResult> HandleAsync(CloseCustomerAccountCommand command)
    {
        var customerAccount = await _customerService.GetCustomerAccountAsync(command.CustomerAccountId);
        
        // Close the Customer account
        customerAccount.Close();

        // Get closing bank account Transactions
        var closingTransactions = customerAccount.GetClosingTransactions();

        // Remove the ApplicationUser
        await _identityService.DeleteApplicationUserAsync(customerAccount.ApplicationUserId);

        // Create confirmation email with relevant data
        var confirmationEmail = new CloseCustomerAccountConfirmationEmail(
            to: customerAccount.Email,
            from: "customerservice@bank.example.com",
            closingDate: DateTime.UtcNow,
            closingTransactions: closingTransactions
        );

        await _unitOfWork.SaveAsync();

        // Send confirmation email
        await _emailSender.SendEmailAsync(confirmationEmail);

        return new CloseCustomerAccountResult();
    }
}
