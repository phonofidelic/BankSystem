using System;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.Services.EmailService;

public class OpenCustomerAccountConfirmationEmail(
    string to,
    string from,
    string body) : Email
{
    public override string To { get; protected init; } = to;
    public override string From { get; protected init; } = from;
    public override string Subject { get; protected init; } = "Your Customer account is ready!";
    public override string Body { get; protected init; } = body;
}
