using BankRUs.Domain.ValueObjects;

namespace BankRUs.Domain.Entities;

public class Transaction : BaseCreatableEntity<Guid>
{
    public Transaction()
    {}

    public Transaction(TransactionType type)
    {
        Type = type;
        _multiplier = type == TransactionType.Deposit ? 1 : -1;
    }
    private int _multiplier { get; set; }
    public override Guid Id { get; set; } = Guid.NewGuid();

    public Guid  CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public Guid BankAccountId { get; set; }
    public BankAccount BankAccount { get; set; } = default!;

    public decimal Amount { get; set; }

    public decimal BalanceAfter { get; private set; }

    public required Currency Currency { get; set; }

    public string? Reference { get; set; } = string.Empty;

    public TransactionType Type { get; init; }

    public decimal Value { get => Amount * _multiplier; }

    public void UpdateBalanceAfter(decimal balance)
    {
        BalanceAfter = balance;
    }
}

public enum TransactionType
{
    Deposit,
    Withdrawal
}