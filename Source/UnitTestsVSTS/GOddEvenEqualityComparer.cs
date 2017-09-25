using System.Collections.Generic;

namespace Wintellect.PowerCollections.Tests
{
    /// <summary>
    /// Comparer that compares ints, sorting odds before evens.
    /// </summary>
    class GOddEvenEqualityComparer : IEqualityComparer<int> {
        public bool Equals(int e1, int e2)
        {
            return ((e1 & 1) == (e2 & 1));
        }

        public int GetHashCode(int i)
        {
            return i;
        }

        public override bool Equals(object obj)
        {
            return (obj is GOddEvenComparer);
        }

        public override int GetHashCode()
        {
            return 123569;
        }
    }
}