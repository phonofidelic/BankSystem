using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.Services.EmailService;
public record SendEmailRequest(string To, string From, string Subject, string Body);

