using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.Authentication.AuthenticateUser
{
    public sealed class AuthenticateUserHandler(
        IAuthenticationService authenticationService,
        ITokenService tokenService) : IHandler<AuthenticateUserCommand, AuthenticateUserResult>
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<AuthenticateUserResult> HandleAsync(AuthenticateUserCommand command)
        {
            // 1. Check for a user with given credentials
            var authenticatedUser = await _authenticationService.AuthenticateUserAsync(
                username: command.UserName,
                password: command.Password);

            if (authenticatedUser == null)
            {
                return AuthenticateUserResult.Failed();
            }

            // 2. Create new token for authenticated user
            var token = _tokenService.CreateToken(
                userId: authenticatedUser.UserId,
                email: authenticatedUser.Email,
                roles: authenticatedUser.Roles);

            // 3. Return the token
            return AuthenticateUserResult.Succeeded(
                accessToken: token.AccessToken,
                expiresAtUtc: token.ExpiresAtUtc);
        }
    }
}
