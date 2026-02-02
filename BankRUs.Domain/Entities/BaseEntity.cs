using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Domain.Entities
{
    public abstract class BaseEntity<TId>
    {
        public virtual TId Id { get; set; } = default!;
    }
}
