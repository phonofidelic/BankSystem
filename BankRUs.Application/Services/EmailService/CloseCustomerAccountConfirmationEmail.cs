using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.Services.EmailService;

public class CloseCustomerAccountConfirmationEmail(
    string to,
    string from,
    DateTime closingDate,
    IReadOnlyList<Transaction> closingTransactions) : Email
{
    public override string To { get; protected init; } = to;
    public override string Subject { get; protected init; } = "Your Customer Account has been closed";
    public override string From { get; protected init; } = from;
    public override string Body { get; protected init; } = 
        $"Your Customer Account has been closed on {closingDate:d}"
        + $"\n"
        + $"\nClosed Bank Accounts:"
        + "\nAccount name:\t" + "\nClosing balance:"
        + RenderClosingTransactionLines(closingTransactions)
        + $"\n"
        + "\nThe remaining funds have been transferred to {ToDo: add transfer bank routing + account number}";

    private static string RenderClosingTransactionLines(IReadOnlyList<Transaction> closingTransactions)
    {
        string lines = "";
        foreach (var closingTransaction in closingTransactions)
        {
            lines += $"\n{closingTransaction.BankAccount.Name}\t" + $"\n{closingTransaction.Amount}\t";
        }
        return lines;
    }
}

