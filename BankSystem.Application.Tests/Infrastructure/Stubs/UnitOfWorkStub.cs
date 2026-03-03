using System;

namespace BankSystem.Application.Tests.Infrastructure.Stubs;

public class UnitOfWorkStub : IUnitOfWork
{
    public async Task SaveAsync()
    {
        await Task.Delay(100);
    }
}
