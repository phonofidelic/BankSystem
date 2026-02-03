using BankRUs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.TransactionService
{
    public record CreateTransactionResult(Transaction Transaction);
}
