using System;

namespace BankRUs.Application.Tests.Infrastructure.Stubs;

public class UnitOfWorkStub : IUnitOfWork
{
    public async Task SaveAsync()
    {
        await Task.Delay(100);
    }
}
