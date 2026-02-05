using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Domain.Entities
{
    public class BankAccount : BaseUpdatableEntity<Guid>
    {
        public override Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Checking account";
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;
        public decimal Balance { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = [];
    }
}
