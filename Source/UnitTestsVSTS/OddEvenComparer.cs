using System.Collections;

namespace Wintellect.PowerCollections.Tests {
    /// <summary>
    /// Comparer that compares ints, sorting odds before evens.
    /// </summary>
    class OddEvenComparer : IComparer {
        public int Compare(object x, object y)
        {
            int e1 = (int)x;
            int e2 = (int)y;
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