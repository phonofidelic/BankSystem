using BankSystem.Domain.ValueObjects;

namespace BankSystem.Application.Services.EmailService;

public interface IEmailSender
{
    Task SendEmailAsync(Email request);
}
