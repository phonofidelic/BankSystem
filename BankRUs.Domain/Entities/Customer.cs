using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BankRUs.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public required Guid ApplicationUserId { get; init; }


        [EmailAddress]
        public required string Email { get; set; }
        
        public required string SocialSecurityNumber { get; set; }

        public ICollection<BankAccount> BankAccounts { get; set; } = [];
    }
}
