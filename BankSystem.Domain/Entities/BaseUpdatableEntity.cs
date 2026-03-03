using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystem.Domain.Entities
{
    public abstract class BaseUpdatableEntity<TId> : BaseCreatableEntity<TId>
    {
        public virtual DateTime UpdatedAt { get; set; }
    }
}
