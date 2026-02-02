using BankRUs.Application.Services.Email;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace BankRUs.Infrastructure.Services.Email
{
    public class FakeEmailSender : IEmailSender
    {
        public async Task SendEmailAsync(SendEmailRequest request)
        {
            var smtpClient = new SmtpClient("localhost", 25);

            await smtpClient.SendMailAsync(
                new MailMessage(
                    from: request.From, 
                    to: request.To, 
                    subject: request.Subject, 
                    body: request.Body));
        }
    }
}
