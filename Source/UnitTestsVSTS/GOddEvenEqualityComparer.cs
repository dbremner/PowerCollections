using System.Collections.Generic;

namespace Wintellect.PowerCollections.Tests
{
    /// <summary>
    /// Comparer that compares ints, sorting odds before evens.
    /// </summary>
    internal class GOddEvenEqualityComparer : IEqualityComparer<int> {
        public bool Equals(int x, int y)
        {
            return ((x & 1) == (y & 1));
        }

        public int GetHashCode(int obj)
        {
            return obj;
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