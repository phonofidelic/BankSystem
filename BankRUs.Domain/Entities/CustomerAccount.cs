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

    public void Open(BankAccount defaultBankAccount)
    {
        AddBankAccount(defaultBankAccount);

        EnforceInvariants();

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
        // A Customer account can be closed if...

        // 1) all bank accounts have a positive balance
        var canClose = BankAccounts.All(bankAccount => bankAccount.Balance >= 0);

        if (!canClose)
        {
            var bankAccountsWithNegativeBalance = BankAccounts.Where(bankAccount => bankAccount.Balance < 0).ToList();
            throw new CloseCustomerAccountException(string.Format("Could not close Customer account. {0} bank accounts have a negative balance", bankAccountsWithNegativeBalance.Count));
        }

        List<Transaction> closingTransactions = [];

        foreach (var bankAccount in BankAccounts)
        {
            bankAccount.Close();
            var closingTransaction = bankAccount.GetClosingTransaction();
            if (closingTransaction != null)
                closingTransactions.Add(closingTransaction);
        }

        FirstName = "";
        LastName = "";
        Email = "";
        SocialSecurityNumber = "";
        Status = CustomerAccountStatus.Closed;
        ClosedOn = DateTime.UtcNow;
    }

    public IReadOnlyList<Transaction> GetClosingTransactions()
    {
        List<Transaction> closingTransactions = [];
        
        foreach(var bankAccount in BankAccounts)
        {
            var closingTransaction = bankAccount.GetClosingTransaction();
            if (closingTransaction != null)
                closingTransactions.Add(closingTransaction);
        }

        return closingTransactions;
    }

    private void EnforceInvariants()
    {
        if (string.IsNullOrEmpty(FirstName)) throw new OpenCustomerAccountException(FirstName);
        if (string.IsNullOrEmpty(LastName)) throw new OpenCustomerAccountException(LastName);
        if (string.IsNullOrEmpty(Email)) throw new OpenCustomerAccountException(FirstName);
        if (string.IsNullOrEmpty(SocialSecurityNumber)) throw new OpenCustomerAccountException(SocialSecurityNumber);
        if (BankAccounts.Count < 1) throw new OpenCustomerAccountException(BankAccounts);
    }
}

public enum CustomerAccountStatus
{
    Default,
    Opened,
    Closed
}

class OpenCustomerAccountException(object prop) : Exception($"Could not open Customer account. Value of property '{nameof(prop)}' is invalid");
class CloseCustomerAccountException(string message = "Could not close Customer account") : Exception(message);