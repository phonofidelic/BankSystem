using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Domain.ValueObjects
{
    public abstract class ValueObject : IComparable, IComparable<ValueObject>
    {

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(ValueObject other)
        {
            throw new NotImplementedException();
        }

        private static int CompareComponents(object object1, object object2)
        {
            if (object1 is null && object2 is null)
            {
                return 0;
            }

            if (object1 is null)
            {
                return -1;
            }

            if (object2 is null)
            {
                return 1;
            }

            if (object1 is IComparable comparable1 && object2 is IComparable comparable2)
            {
                return comparable1.CompareTo(comparable2);
            }

            return object1.Equals(object2) ? 0 : -1;
        }
    }
}
