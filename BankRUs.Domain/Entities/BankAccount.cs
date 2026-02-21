using BankRUs.Domain.ValueObjects;

namespace BankRUs.Domain.Entities;

public class BankAccount : BaseUpdatableEntity<Guid>
{
    public override Guid Id { get; set; } = Guid.NewGuid();

    public BankAccountStatus Status { get; private set; }

    public DateTime? ClosedOn { get; set; } = null;

    public string Name { get; set; } = "Checking account";

    public Guid CustomerId { get; set; }
    
    public CustomerAccount Customer { get; set; } = default!;
    
    public required Currency Currency { get; set; }
    
    public decimal Balance { get; private set; }
    
    public ICollection<Transaction> Transactions { get; set; } = [];

    public void AddTransaction(Transaction transaction)
    {
        var pendingBalance = Balance + transaction.Value;
        if (pendingBalance < 0)
        {
            throw new InsufficientFundsException();
        }

        transaction.UpdateBalanceAfter(pendingBalance);
        Balance = pendingBalance;
        Transactions.Add(transaction);
    }

    public void Close()
    {
        if (Balance < 0)
        {
            throw new NegativeBalanceException();
        }

        Name = "";
        Status = BankAccountStatus.Closed;
        ClosedOn = DateTime.UtcNow;
    }
}

public enum BankAccountStatus
{
    // ToDo: add Default status and only set to Opened if conditions are met
    // Default,
    Opened,
    Closed
}
public class InsufficientFundsException() : Exception("Insufficient Funds");

public class NegativeBalanceException() : Exception("Bank account has negative balance");