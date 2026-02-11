using BankRUs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.AuditLog
{
    public class AuditLog(string message, string? caller) : BaseCreatableEntity<Guid>
    {
        public override Guid Id { get; set; } = Guid.NewGuid();
        public string? Caller { get; set; } = caller;
        public string Message { get; init; } = message;
        public override DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
