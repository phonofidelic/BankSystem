using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.Email;


namespace BankRUs.Application.UseCases.OpenBankAccount;

public class OpenBankAccountHandler : IHandler<OpenBankAccountCommand, OpenBankAccountResult>
{
    private readonly ICustomerService _customerService;
    private readonly IEmailSender _emailSender;

    public OpenBankAccountHandler(
        ICustomerService customerService,
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
            BankAccountId: Guid.NewGuid()));
       

        // Send confirmation email to customer
        var sendEmailRequest = new SendEmailRequest(
            To: command.CustomerEmail,
            From: "your.bank@example.com",
            Subject: "Ditt bankkonto är nu redo!",
            Body: "Ditt bankkonto är nu redo!");

        return new OpenBankAccountResult(
           BankAccountId: createdNewBankAccountResult.BankAccount.Id);
    }
}
