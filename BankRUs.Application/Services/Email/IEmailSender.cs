using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.Email;

public interface IEmailSender
{
    Task SendEmailAsync(SendEmailRequest request);
}
