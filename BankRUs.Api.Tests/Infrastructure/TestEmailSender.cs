using BankRUs.Application.Services.EmailService;
using BankRUs.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Api.Tests.Infrastructure;

public class TestEmailSender : IEmailSender
{
    public async Task SendEmailAsync(Email request)
    {
        await Task.Delay(100);
    }
}
