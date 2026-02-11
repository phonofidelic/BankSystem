using BankRUs.Domain.ValueObjects;

namespace BankRUs.Domain.Entities;

public class BankAccount : BaseUpdatableEntity<Guid>
{
    public override Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "Checking account";
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
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
}

public class InsufficientFundsException() : Exception("Insufficient Funds");