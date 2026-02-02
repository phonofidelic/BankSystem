using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Domain.Entities
{
    internal class Deposit : Transaction
    {
        public Deposit() : base(TransactionType.Deposit) { }
    }
}
