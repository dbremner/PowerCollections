using System.Collections.Generic;

namespace Wintellect.PowerCollections.Tests
{
    internal class Mod2EqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return ((x & 1) == (y & 1)) ;
        }

        public int GetHashCode(int obj)
        {
            return (obj & 1).GetHashCode();
        }
    }
}