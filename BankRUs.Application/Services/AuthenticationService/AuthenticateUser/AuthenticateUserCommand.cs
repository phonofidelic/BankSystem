using System;
namespace BankRUs.Application.Services.Authentication.AuthenticateUser;

public sealed record AuthenticateUserCommand(
    string UserName,
    string Password);
