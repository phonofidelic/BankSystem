using BankRUs.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace BankRUs.Domain.Entities;

public class Customer : BaseUpdatableEntity<Guid>
{
    public override Guid Id { get; set; } = Guid.NewGuid();

    public Guid ApplicationUserId { get; private set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    [EmailAddress]
    public required string Email { get; set; }
    
    public required string SocialSecurityNumber { get; set; }

    public ICollection<BankAccount> BankAccounts { get; set; } = [];

    public ICollection<Transaction> Transactions { get; set; } = [];

    public void SetApplicationUserId(Guid applicationUserId) => ApplicationUserId = applicationUserId; 

    public void AddBankAccount(BankAccount bankAccount)
    {
        BankAccounts.Add(bankAccount);
    }

    public void Update(CustomerAccountDetails details)
    {
        if (details.FirstName != null && details.FirstName != FirstName) { 
            FirstName = details.FirstName;
        }

        if (details.LastName != null && details.LastName != LastName)
        {
            LastName = details.LastName;
        }

        if (details.Email != null && details.Email != Email) { 
            Email = details.Email;
        }

        if (details.SocialSecurityNumber != null && details.SocialSecurityNumber != SocialSecurityNumber) { 
            SocialSecurityNumber = details.SocialSecurityNumber;
        }
    }
}
