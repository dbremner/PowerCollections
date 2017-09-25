using System;

namespace Wintellect.PowerCollections.Tests
{
    /// <summary>
    /// Comparable that compares ints, sorting odds before evens.
    /// </summary>
    class OddEvenComparable : IComparable
    {
        public int val;

        public OddEvenComparable(int v)
        {
            val = v;
        }

        public int CompareTo(object other)
        {
            int e1 = val;
            int e2 = ((OddEvenComparable)other).val;
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

        public override bool Equals(object obj)
        {
            if (obj is OddEvenComparable) 
                return CompareTo((OddEvenComparable) obj) == 0;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return val.GetHashCode();
        }

    }
}