using BankRUs.Application.Services.Email;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace BankRUs.Intrastructure.Services.Email
{
    public class FakeEmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string to, string from, string subject, string body)
        {
            var smtpClient = new SmtpClient("localhost", 25);

            await smtpClient.SendMailAsync(
                new MailMessage(from, to, subject, body));
        }
    }
}
