namespace BankRUs.Application.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task <AuthenticatedUser?> AuthenticateUser(string username, string password);
    }
}
