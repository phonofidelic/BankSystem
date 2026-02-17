using BankRUs.Api.Dtos.Auth;
using BankRUs.Application.Services.Authentication.AuthenticateUser;
using Microsoft.AspNetCore.Mvc;

namespace BankRUs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    AuthenticateUserHandler authenticateUserHandler) : ControllerBase
{
    private readonly AuthenticateUserHandler _authenticateUserHandler = authenticateUserHandler;

    // POST /api/auth/login
    [HttpPost("login")]
    [Produces("application/json")]
    [ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        var authenticateUserResult = await _authenticateUserHandler.HandleAsync(new AuthenticateUserCommand(
            UserName: request.UserName,
            Password: request.Password));

        if (!authenticateUserResult.Succeed)
        {
            // 401 Unauthorized
            return Unauthorized();
        }

        return Ok(new LoginResponseDto(
            Token: authenticateUserResult.AccessToken,
            ExpiresAtUtc: authenticateUserResult.ExpiresAtUtc));
    }
}
