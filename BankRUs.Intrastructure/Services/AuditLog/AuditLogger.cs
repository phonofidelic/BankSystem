using BankRUs.Application.Services.AuditLog;
using System.Diagnostics;

namespace BankRUs.Infrastructure.Services.AuditLogService
{
    public class AuditLogger : IAuditLogger
    {
        private List<AuditLog> _logs = [];
        public void Log(string message)
        {
            var methodInfo = new StackTrace()?.GetFrame(1)?.GetMethod();
            _logs.Add(new(message, methodInfo?.ReflectedType?.Name));
        }

        public IEnumerable<AuditLog> GetAuditLogs() => _logs.ToList(); 
    }
}
