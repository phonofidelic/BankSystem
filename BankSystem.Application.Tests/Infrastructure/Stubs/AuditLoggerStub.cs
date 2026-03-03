using System;
using BankSystem.Application.Services.AuditLog;

namespace BankSystem.Application.Tests.Infrastructure.Stubs;

public class AuditLoggerStub : IAuditLogger
{
    public IEnumerable<AuditLog> GetAuditLogs()
    {
        return [];
    }

    public void Log(string message)
    {
        
    }
}
