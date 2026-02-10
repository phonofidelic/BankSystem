using BankRUs.Domain.ValueObjects;

namespace BankRUs.Domain.Entities
{
    public class BankAccount : BaseUpdatableEntity<Guid>
    {
        public override Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Checking account";
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;
        public required Currency Currency { get; set; }
        public decimal Balance { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = [];
    }
}
