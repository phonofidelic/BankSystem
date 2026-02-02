using BankRUs.Application.Services.Email;
using BankRUs.Application.Services.Identity;

namespace BankRUs.Application.UseCases.OpenAccount;

public class OpenCustomerAccountHandler
{
    private readonly IIdentityService _identityService;
    private readonly IEmailSender _emailSender;

    public OpenCustomerAccountHandler(
        IIdentityService identityService,
        IEmailSender emailSender)
    { 
        _identityService = identityService; 
        _emailSender = emailSender;
    }

    public async Task<OpenCustomerAccountResult> HandleAsync(OpenCustomerAccountCommand command)
    {
        // ToDo: Validate business rules for Customer Account Creation

        // Create new ApplicationUser
        var createApplicationUserResult = await _identityService.CreateApplicationUserAsync(new CreateApplicationUserRequest(
            FirstName: command.FirstName,
            LastName: command.LastName,
            Email: command.Email
         ));

        // Create Customer
        var createCustomerResult = await _identityService.CreateCustomerAsync(new CreateCustomerRequest(
            ApplicationUserId: createApplicationUserResult.UserId,
            SocialSecurityNumber: command.SocialSecurityNumber,
            Email: command.Email));

        // TODO: Move bank account creation here from IdentityService?

        // Send confirmation email to customer
        var sendEmailRequest = new SendEmailRequest(
            To: command.Email,
            From: "your.bank@example.com",
            Subject: "Ditt bankkonto är nu redo!",
            Body: "Ditt bankkonto är nu redo!");

         await _emailSender.SendEmailAsync(sendEmailRequest);

        return new OpenCustomerAccountResult(UserId: createApplicationUserResult.UserId);
    }
}