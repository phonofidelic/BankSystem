namespace BankSystem.Application;

public interface IUnitOfWork
{
    public Task SaveAsync();
}
