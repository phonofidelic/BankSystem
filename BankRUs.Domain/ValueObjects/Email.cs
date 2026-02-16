
namespace BankRUs.Domain.ValueObjects;

public abstract class Email() : ValueObject
{
    public abstract string To { get; protected init; }
    public abstract string From { get; protected init; }
    public abstract string Subject { get; protected init; }
    public abstract string Body { get; protected init; } 
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return To;
        yield return From;
        yield return Subject;
        yield return Body;
    }
}
