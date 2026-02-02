using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.Email
{
    public record SendEmailRequest(string To, string From, string Subject, string Body);
}
