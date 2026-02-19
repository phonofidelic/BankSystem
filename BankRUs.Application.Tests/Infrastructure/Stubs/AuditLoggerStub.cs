using System;
using BankRUs.Application.Services.AuditLog;

namespace BankRUs.Application.Tests.Infrastructure.Stubs;

public class AuditLoggerStub : IAuditLogger
{
    public IEnumerable<AuditLog> GetAuditLogs()
    {
        throw new NotImplementedException();
    }

    public void Log(string message)
    {
        throw new NotImplementedException();
    }
}
