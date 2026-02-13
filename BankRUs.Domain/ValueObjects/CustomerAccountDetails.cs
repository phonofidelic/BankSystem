using System.Collections.Frozen;


namespace BankRUs.Domain.ValueObjects;

public class CustomerAccountDetails : ValueObject
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? SocialSecurityNumber { get; init; }
    private HashSet<string> _attributes { get; set; } = [];
    public IReadOnlySet<string> Fields { get => _attributes.ToFrozenSet(); }

    public CustomerAccountDetails(
    string? firstName,
    string? lastName,
    string? email,
    string? socialSecurityNumber)
    {
        // ToDo: Add validation here?
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        SocialSecurityNumber = socialSecurityNumber;

        if (firstName != null) _attributes.Add(nameof(FirstName));
        if (lastName != null) _attributes.Add(nameof(LastName));
        if (email  != null) _attributes.Add(nameof(Email));
        if (socialSecurityNumber != null) _attributes.Add(nameof(SocialSecurityNumber));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        if (FirstName != null) yield return FirstName;
        if (LastName != null) yield return LastName;
        if (Email != null) yield return Email;
        if (SocialSecurityNumber != null) yield return SocialSecurityNumber;
    }
}
