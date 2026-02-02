using BankRUs.Application.Services.Identity;

namespace BankRUs.Application.UseCases.OpenAccount;

public class OpenCustomerAccountHandler
{
    private readonly IIdentityService _identityService;

    public OpenCustomerAccountHandler(IIdentityService identityService)
        => _identityService = identityService;

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

        // TODO: Skapa bankkonto
        
        // TODO: Skicka välkomstmail till kund
        // Delegera till infrastructure
        // _emailSender.Send("Ditt bankkonto är nu redo!");

        return new OpenCustomerAccountResult(UserId: createApplicationUserResult.UserId);
    }
}