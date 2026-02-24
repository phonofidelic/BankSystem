using BankRUs.Domain.ValueObjects;

namespace BankRUs.Domain.Entities;

public class BankAccount(Guid customerAccountId) : BaseUpdatableEntity<Guid>
{
    public override Guid Id { get; set; } = Guid.NewGuid();

    public BankAccountStatus Status { get; private set; }

    public DateTime? ClosedOn { get; set; } = null;

    public string Name { get; set; } = "Checking account";

    public Guid CustomerAccountId { get; set; } = customerAccountId;
    
    public CustomerAccount CustomerAccount { get; set; } = default!;

    public Currency Currency { get; set; } = default!;
    
    public decimal Balance { get; private set; }
    
    public ICollection<Transaction> Transactions { get; set; } = [];

    // ToDo: add UpdateAccountDetails method for BankAccount
    public void UpdateAccountDetails(BankAccountDetails details)
    {
        if (details.Name != null)
            Name = details.Name;

        if (Currency == null && details.Currency != null)
            Currency = details.Currency;
    }

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

        EnforceInvariants();
    }

    public void Close()
    {
        if (Balance < 0)
        {
            throw new NegativeBalanceException();
        }
        
        if (Balance > 0)
        {
            var closingTransaction = new Transaction
            {
                CustomerId = CustomerAccountId,
                BankAccountId = Id,
                Amount = Balance,
                Currency = Currency,
                Reference = $"Closing transaction for Bank account {Name}",
                Type = TransactionType.Withdrawal,
            };

            AddTransaction(closingTransaction);
        }

        Name = "";
        Status = BankAccountStatus.Closed;
        ClosedOn = DateTime.UtcNow;
    }
    public Transaction? GetClosingTransaction()
    {
        if (Status != BankAccountStatus.Closed)
            throw new UnexpectedBankAccountStatus(expectedStatus: BankAccountStatus.Closed, actualStatus: Status);

        return Transactions.OrderBy(t => t.CreatedAt).LastOrDefault();
    }

    private void EnforceInvariants()
    {
        var unsupportedCurrencyTransaction = Transactions.Any(t => t.Currency != Currency);
        if (unsupportedCurrencyTransaction) throw new InvalidTransactionCurrencyException();
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

public class UnexpectedBankAccountStatus(BankAccountStatus expectedStatus, BankAccountStatus actualStatus) : Exception($"Bank account status is '{actualStatus}' but was expected to be '{expectedStatus}'");

public class InvalidTransactionCurrencyException() : Exception("Bank account cannot contain a transaction of a different currency");