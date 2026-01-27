using BankRUs.Application.Identity;

namespace BankRUs.Application.UseCases.OpenAccount;

public class OpenAccountHandler
{
    private readonly IIdentityService _identityService;

    public OpenAccountHandler(IIdentityService identityService)
        => _identityService = identityService;

    public async Task<OpenAccountResult> HandleAsync(OpenAccountCommand command)
    {
        // TODO: Skapa användarkonto (ASP.NET Core Identity)
        // Delegera till infrastructure
        var createUserResult = await _identityService.CreateUserAsync(new CreateUserRequest(
            FirstName: command.FirstName,
            LastName: command.LastName,
            SocialSecurityNumber: command.SocialSecurityNumber,
            Email: command.Email
         ));

        // TODO: SocialSecurityNumber + Email ska vara UNIQUE

        // TODO: Skapa bankkonto
        // Delegera till infrastructure
        var createCustomerResult = await _identityService.CreateCustomerAsync(new CreateCustomerRequest(createUserResult.UserId));
        
        // TODO: Skicka välkomstmail till kund
        // Delegera till infrastructure
        // _emailSender.Send("Ditt bankkonto är nu redo!");

        return new OpenAccountResult(UserId: createUserResult.UserId);
    }
}