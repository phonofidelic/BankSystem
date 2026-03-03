using BankSystem.Domain.ValueObjects;

namespace BankSystem.Application.Services.EmailService;
public record SendEmailRequest(string To, string From, string Subject, string Body);

