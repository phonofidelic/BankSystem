using BankRUs.Application.Services.CustomerAccountService;
using BankRUs.Application.Services.EmailService;


namespace BankRUs.Application.UseCases.OpenBankAccount;

public class OpenBankAccountHandler : IHandler<OpenBankAccountCommand, OpenBankAccountResult>
{
    private readonly ICustomerAccountService _customerService;
    private readonly IEmailSender _emailSender;

    public OpenBankAccountHandler(
        ICustomerAccountService customerService,
         IEmailSender emailSender)
    {
        _customerService = customerService;
        _emailSender = emailSender;
    }

    public async Task<OpenBankAccountResult> HandleAsync(OpenBankAccountCommand command)
    {
        // Create new bank account for existing Customer
        var createdNewBankAccountResult = await _customerService.CreateBankAccountAsync(new CreateBankAccountRequest(
            CustomerId: command.CustomerId,
            BankAccountName: command.BankAccountName));
       

        // Send confirmation email to customer
        var sendEmailRequest = new OpenCustomerAccountConfirmationEmail(
            to: command.CustomerEmail,
            from: "customerservice@bank.example.com",
            body: "Welcome to Bank Example! \nYour Customer account is ready."
        );

        await _emailSender.SendEmailAsync(sendEmailRequest);

        return new OpenBankAccountResult(
           BankAccountId: createdNewBankAccountResult.BankAccount.Id);
    }
}
