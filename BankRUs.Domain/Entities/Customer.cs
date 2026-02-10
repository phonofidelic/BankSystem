using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BankRUs.Domain.Entities
{
    public class Customer : BaseUpdatableEntity<Guid>
    {
        public override Guid Id { get; set; } = Guid.NewGuid();

        public Guid ApplicationUserId { get; private set; }


        [EmailAddress]
        public required string Email { get; set; }
        
        public required string SocialSecurityNumber { get; set; }

        public ICollection<BankAccount> BankAccounts { get; set; } = [];

        public ICollection<Transaction> Transactions { get; set; } = [];

        public void SetApplicationUserId(Guid applicationUserId) => ApplicationUserId = applicationUserId; 
    }
}
