namespace BankRUs.Application.Services.Authentication.AuthenticateUser;

public sealed record AuthenticateUserResult
{
    public static AuthenticateUserResult Succeeded(string accessToken, DateTime expiresAtUtc)
    {
        return new AuthenticateUserResult
        {
            Succeed = true,
            AccessToken = accessToken,
            ExpiresAtUtc = expiresAtUtc
        };
    }

    public static AuthenticateUserResult Failed()
    {
        return new AuthenticateUserResult
        {
            Succeed = false
        };
    }

    public bool Succeed { get; init; }
    public string? AccessToken { get; init; }
    public DateTime? ExpiresAtUtc { get; init; }
}
