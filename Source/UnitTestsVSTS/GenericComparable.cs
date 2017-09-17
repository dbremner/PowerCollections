using System;

namespace Wintellect.PowerCollections.Tests
{
    class GenericComparable : IComparable<GenericComparable>
    {
        private readonly int _value;
        public GenericComparable(int value) => _value = value;
        public int CompareTo(GenericComparable other) => _value.CompareTo(other._value);
    }
}
