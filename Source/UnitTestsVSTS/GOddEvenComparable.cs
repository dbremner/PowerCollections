using System;

namespace Wintellect.PowerCollections.Tests
{
    /// <summary>
    /// Comparable that compares ints, sorting odds before evens.
    /// </summary>
    class GOddEvenComparable : IComparable<GOddEvenComparable>
    {
        public int val;

        public GOddEvenComparable(int v)
        {
            val = v;
        }

        public int CompareTo(GOddEvenComparable other)
        {
            int e1 = val;
            int e2 = other.val;
            if ((e1 & 1) == 1 && (e2 & 1) == 0)
                return -1;
            else if ((e1 & 1) == 0 && (e2 & 1) == 1)
                return 1;
            else if (e1 < e2)
                return -1;
            else if (e1 > e2)
                return 1;
            else
                return 0;
        }
    }
}