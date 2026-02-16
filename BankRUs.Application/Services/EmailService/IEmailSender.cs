using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.Services.EmailService;

public interface IEmailSender
{
    Task SendEmailAsync(Email request);
}
