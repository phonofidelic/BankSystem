using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Domain.Entities
{
    public class Deposit : Transaction
    {
        public Deposit() : base(TransactionType.Deposit) { }
    }
}
