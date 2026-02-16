using BankRUs.Application.Services.EmailService;
using BankRUs.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace BankRUs.Infrastructure.Services.EmailService
{
    public class FakeEmailSender : IEmailSender
    {
        public async Task SendEmailAsync(Email request)
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
