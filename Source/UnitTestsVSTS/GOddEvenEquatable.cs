using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wintellect.PowerCollections.Tests
{
    /// <summary>
    /// Comparable that compares ints, equating odds with odds and evens with evrents..
    /// </summary>
    class GOddEvenEquatable : System.IEquatable<GOddEvenEquatable>
    {
        public int val;

        public GOddEvenEquatable(int v)
        {
            val = v;
        }

        public bool Equals(GOddEvenEquatable other)
        {
            return ((this.val & 1) == (other.val & 1));
        }

        public override int GetHashCode()
        {
            return val.GetHashCode();
        }
    }
}