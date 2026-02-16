namespace BankRUs.Application.Services.Authentication;

public interface IAuthenticationService
{
    Task <AuthenticatedUser?> AuthenticateUserAsync(string username, string password);
}
