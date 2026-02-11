namespace BankRUs.Application.Services.Authentication;

public interface ITokenService
{
    Token CreateToken(
        string userId,
        string email,
        IEnumerable<string>? roles = null);
}
