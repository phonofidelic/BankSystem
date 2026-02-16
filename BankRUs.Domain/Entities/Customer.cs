using BankRUs.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace BankRUs.Domain.Entities;

public class Customer : BaseUpdatableEntity<Guid>
{
    public override Guid Id { get; set; } = Guid.NewGuid();

    public CustomerAccountStatus Status { get; private set; }

    public Guid ApplicationUserId { get; private set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string SocialSecurityNumber { get; set; } = string.Empty;

    public ICollection<BankAccount> BankAccounts { get; set; } = [];

    public ICollection<Transaction> Transactions { get; set; } = [];

    public void SetApplicationUserId(Guid applicationUserId) => ApplicationUserId = applicationUserId; 

    public void Open(CustomerAccountDetails details)
    {
        FirstName = details.FirstName ?? throw new OpenCustomerAccountException(FirstName);
        LastName = details.LastName ?? throw new OpenCustomerAccountException(LastName);
        Email = details.Email ?? throw new OpenCustomerAccountException(FirstName);
        SocialSecurityNumber = details.SocialSecurityNumber ?? throw new OpenCustomerAccountException(SocialSecurityNumber);

        Status = CustomerAccountStatus.Opened;
    }
    public void AddBankAccount(BankAccount bankAccount)
    {
        BankAccounts.Add(bankAccount);
    }

    public CustomerAccountDetails GetDetails()
    {
        return new CustomerAccountDetails(
            FirstName,
            LastName,
            Email,
            SocialSecurityNumber
        );
    }

    public IReadOnlyList<BankAccount> GetBankAccounts()
    {
        return BankAccounts.ToList();
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

    public void Remove(Action<Customer> removeAction)
    {
        // Validate that Customer account removal is allowed
        var canRemove = BankAccounts.All(bankAccount => bankAccount.Balance == 0);

        if (!canRemove)
        {
            var bankAccountsWithRemainingBalance = BankAccounts.Where(bankAccount => bankAccount.Balance != 0);
            throw new CloseCustomerAccountException(string.Format("Could not close Customer account. {0} bank accounts have a remaining balance", bankAccountsWithRemainingBalance.Count()));
        }

        removeAction(this);
    }
}

public enum CustomerAccountStatus
{
    Default,
    Opened,
    Closed
}

class OpenCustomerAccountException(object param) : ArgumentException("Could not open Customer account. Missing required parameter.", paramName: nameof(param));
class CloseCustomerAccountException(string message = "Could not close Customer account") : Exception(message);