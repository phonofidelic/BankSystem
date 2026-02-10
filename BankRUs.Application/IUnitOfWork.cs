namespace BankRUs.Application;

public interface IUnitOfWork
{
    public Task SaveAsync();
}
