namespace BankRUs.Domain.ValueObjects;

public class BankAccountDetails : ValueObject
{
    public string? Name { get; init; }

    public Currency? Currency { get; init; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        if (Name != null) yield return Name;
        if (Currency != null) yield return Currency;
    }
}
