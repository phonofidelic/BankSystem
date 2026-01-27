using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public required Guid ApplicationUserId { get; init; }
        public ICollection<BankAccount> BankAccounts { get; set; } = [];
    }
}
