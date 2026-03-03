using BankSystem.Application.Services.EmailService;
using BankSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace BankSystem.Infrastructure.Services.EmailService
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
