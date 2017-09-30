namespace Wintellect.PowerCollections.Tests
{
    /// <summary>
    /// Comparer that compares ints, sorting odds before evens.
    /// </summary>
    internal class GOddEvenComparer : System.Collections.Generic.IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if ((x & 1) == 1 && (y & 1) == 0)
                return -1;
            else if ((x & 1) == 0 && (y & 1) == 1)
                return 1;
            else if (x < y)
                return -1;
            else if (x > y)
                return 1;
            else
                return 0;
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