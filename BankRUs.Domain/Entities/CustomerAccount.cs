using BankRUs.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace BankRUs.Domain.Entities;

public class CustomerAccount(Guid applicationUserId, string socialSecurityNumber) : BaseUpdatableEntity<Guid>
{
    public override Guid Id { get; set; } = Guid.NewGuid();

    public CustomerAccountStatus Status { get; private set; }

    public DateTime? ClosedOn { get; private set; } = null;

    public Guid ApplicationUserId { get; private set; } = applicationUserId;

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    [EmailAddress]
    public string Email { get; private set; } = string.Empty;
    
    public string SocialSecurityNumber { get; private set; } = socialSecurityNumber;

    public ICollection<BankAccount> BankAccounts { get; set; } = [];

    public ICollection<Transaction> Transactions { get; set; } = [];

    public void SetApplicationUserId(Guid applicationUserId) => ApplicationUserId = applicationUserId; 

    public void Open()
    {
        if (string.IsNullOrEmpty(FirstName)) throw new OpenCustomerAccountException(FirstName);
        if (string.IsNullOrEmpty(LastName)) throw new OpenCustomerAccountException(LastName);
        if (string.IsNullOrEmpty(Email)) throw new OpenCustomerAccountException(FirstName);
        if (string.IsNullOrEmpty(SocialSecurityNumber)) throw new OpenCustomerAccountException(SocialSecurityNumber);

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

    public void UpdateAccountDetails(CustomerAccountDetails details)
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

    public void Close()
    {
        // Validate that Customer account closure is allowed
        var canClose = BankAccounts.All(bankAccount => bankAccount.Balance == 0);

        if (!canClose)
        {
            var bankAccountsWithRemainingBalance = BankAccounts.Where(bankAccount => bankAccount.Balance != 0);
            throw new CloseCustomerAccountException(string.Format("Could not close Customer account. {0} bank accounts have a remaining balance", bankAccountsWithRemainingBalance.Count()));
        }

        FirstName = "";
        LastName = "";
        Email = "";
        SocialSecurityNumber = "";
        Status = CustomerAccountStatus.Closed;
        ClosedOn = DateTime.UtcNow;
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