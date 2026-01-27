using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Domain.Entities
{
    public class BankAccount
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Balance { get; set; }
        // ToDo:
        // Transactions
    }
}
