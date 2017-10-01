//******************************
// Written by Peter Golde
// Copyright (c) 2004-2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement
// contained in the file "License.txt" accompanying this file.
//******************************


namespace Wintellect.PowerCollections.Tests
{
    public partial class HashTests
    {
        internal class DataComparer : System.Collections.Generic.IEqualityComparer<TestItem>
        {
            public bool Equals(TestItem x, TestItem y)
            {
                return string.Equals(x.key, y.key);
            }

            public int GetHashCode(TestItem obj)
            {
                return obj.key.GetHashCode();
            }
        }
    }
}

