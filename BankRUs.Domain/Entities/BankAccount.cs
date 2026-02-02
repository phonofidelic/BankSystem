using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Domain.Entities
{
    public class BankAccount : BaseEntity
    {
        public Guid Id { get; set; }

        // ToDo: Add bank account name
        //public string Name { get; set; } = "Checking account";
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public decimal Balance { get; set; }
        // ToDo:
        // Transactions
    }
}
