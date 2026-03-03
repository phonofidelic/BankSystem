using BankSystem.Application.Services.EmailService;
using BankSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystem.Api.Tests.Infrastructure;

public class TestEmailSender : IEmailSender
{
    public async Task SendEmailAsync(Email request)
    {
        await Task.Delay(100);
    }
}
