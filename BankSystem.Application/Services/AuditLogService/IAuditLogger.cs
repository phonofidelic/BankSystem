using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystem.Application.Services.AuditLog
{
    public interface IAuditLogger
    {
        void Log(string message);

        IEnumerable<AuditLog> GetAuditLogs();
    }
}
