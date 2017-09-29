namespace Wintellect.PowerCollections.Tests
{
    /// <summary>
    /// Comparer that compares ints, sorting odds before evens.
    /// </summary>
    internal class GOddEvenComparer : System.Collections.Generic.IComparer<int>
    {
        public int Compare(int e1, int e2)
        {
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
            return (obj is GOddEvenComparer);
        }

        public override int GetHashCode()
        {
            return 123569;
        }
    }
}