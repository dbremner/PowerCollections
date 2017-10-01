//******************************
// Written by Peter Golde
// Copyright (c) 2004-2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement
// contained in the file "License.txt" accompanying this file.
//******************************

#region Using directives

using System.Collections.Generic;
#endregion

namespace Wintellect.PowerCollections.Tests
{
    public partial class BagTests
    {
        // Strange comparer that uses modulo arithmetic.
        private class ModularComparer : IEqualityComparer<int>
        {
            private readonly int mod;

            public ModularComparer(int mod)
            {
                this.mod = mod;
            }

            public bool Equals(int x, int y)
            {
                return (x % mod) == (y % mod);
            }

            public int GetHashCode(int obj)
            {
                return (obj % mod).GetHashCode();
            }
        }
    }
}

