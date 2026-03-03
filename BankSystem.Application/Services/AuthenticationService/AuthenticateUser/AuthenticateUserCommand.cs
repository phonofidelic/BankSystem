using System;
namespace BankSystem.Application.Services.Authentication.AuthenticateUser;

public sealed record AuthenticateUserCommand(
    string UserName,
    string Password);
