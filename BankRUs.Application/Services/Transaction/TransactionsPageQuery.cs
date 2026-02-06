using BankRUs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.TransactionService;

public record TransactionsPageQuery : BasePageQuery
{
    public Guid BankAccountId { get; init; }
    public DateTime? StartPeriod { get; init; }
    public DateTime? EndPeriod { get; init; }
    public TransactionType? Type { get; set; } = null;
};
