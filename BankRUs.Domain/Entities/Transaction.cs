using BankRUs.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BankRUs.Domain.Entities;

public class Transaction(TransactionType type) : BaseCreatableEntity<Guid>
{
    public override Guid Id { get; set; } = Guid.NewGuid();

    public required Guid  CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public required Guid BankAccountId { get; set; }
    public BankAccount BankAccount { get; set; } = default!;

    public int Amount { get; set; }

    // Stored as Currency ISOSymbol
    [NotMapped]
    public required Currency Currency { get; set; }

    public string Reference { get; set; } = string.Empty;

    public required TransactionType Type { get; init; } = type;
}

public enum TransactionType
{
    Deposit,
    Withdrawal
}