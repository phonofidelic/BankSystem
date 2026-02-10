using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.Email;
using BankRUs.Application.Services.Identity;

namespace BankRUs.Application.UseCases.OpenAccount;

public class OpenCustomerAccountHandler : IHandler<OpenCustomerAccountCommand, OpenCustomerAccountResult>
{
    // ToDo: Add ILoggerService?
    //private readonly ILoggerService<OpenCustomerAccountHandler> _logger;
    private readonly IIdentityService _identityService;
    private readonly ICustomerService _customerService;
    private readonly IEmailSender _emailSender;

    public OpenCustomerAccountHandler(
        IIdentityService identityService,
        ICustomerService customerService,
        IEmailSender emailSender)
    { 
        _identityService = identityService;
        _customerService = customerService;
        _emailSender = emailSender;
    }

    public async Task<OpenCustomerAccountResult> HandleAsync(OpenCustomerAccountCommand command)
    {
        // ToDo: Validate business rules for Customer Account Creation

        // Create new ApplicationUser
        var createApplicationUserResult = await _identityService.CreateApplicationUserAsync(new CreateApplicationUserRequest(
            FirstName: command.FirstName,
            LastName: command.LastName,
            Email: command.Email,
            Password: command.Password
         ));

        // Create new Customer
        var createCustomerResult = await _customerService.CreateCustomerAsync(new CreateCustomerRequest(
            ApplicationUserId: createApplicationUserResult.UserId,
            SocialSecurityNumber: command.SocialSecurityNumber,
            Email: command.Email));

        // Create default bank account for new Customer
        var createdDefaultBankAccountResult = await _customerService.CreateBankAccountAsync(new CreateBankAccountRequest(
            CustomerId: createCustomerResult.CustomerId,
            BankAccountName: "Default Checking Account"));

        try
        {
            // Send confirmation email to customer
            var sendEmailRequest = new SendEmailRequest(
                To: command.Email,
                From: "your.bank@example.com",
                Subject: "Välkommen till BankAB!",
                Body: "Ditt bankkonto är nu redo!");

             await _emailSender.SendEmailAsync(sendEmailRequest);
        } catch
        {
            // ToDo: Log email sender error
        }

        return new OpenCustomerAccountResult(UserId: createApplicationUserResult.UserId);
    }
}