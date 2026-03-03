using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystem.Domain.Entities
{
    public abstract class BaseCreatableEntity<TId> : BaseEntity<TId>
    {
        public virtual DateTime CreatedAt { get; set; }
    }
}
