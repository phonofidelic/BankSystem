using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Domain.ValueObjects
{
    /// <summary>
    /// See: https://enterprisecraftsmanship.com/posts/value-object-better-implementation/
    /// </summary>
    public abstract class ValueObject : IComparable, IComparable<ValueObject>
    {
        private int? _cachedHashCode;
        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            var valueObject = (ValueObject) obj;

            return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            if (!_cachedHashCode.HasValue) {
                _cachedHashCode = GetEqualityComponents()
                    .Aggregate(1, (current, obj) =>
                    {
                        unchecked
                        {
                            return current * 23 + (obj?.GetHashCode() ?? 0);
                        }
                    });
            }

            return _cachedHashCode.Value;
        }

        public int CompareTo(object? obj)
        {
            Type thisType = GetType();
            Type otherType = obj.GetType();

            if (thisType != otherType)
            {
                return string.Compare(thisType.ToString(), otherType.ToString(), StringComparison.Ordinal);
            }

            var other = (ValueObject)obj;

            object[] components = GetEqualityComponents().ToArray();
            object[] otherComponents = other.GetEqualityComponents().ToArray();

            for (var i = 0; i < components.Length; i++) {
                int comparison = CompareComponents(components[i], otherComponents[i]);
                if (comparison != 0) { 
                    return comparison;
                }
            }

            return 0;
        }

        public int CompareTo(ValueObject? other)
        {
            return CompareTo(other as object);
        }

        public static bool operator ==(ValueObject left, ValueObject right)
        {
            return EqualOperator(left, right);
        }

        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return NotEqualOperator(left, right);
        }

        private static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (left is null && right is null)
            {
                return true;
            }
            if (left is null || right is null)
            {
                return false;
            }
            return ReferenceEquals(left, right) || left.Equals(right);
        }

        private static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !(left == right);
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
