//******************************
// Written by Peter Golde
// Copyright (c) 2004-2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement
// contained in the file "License.txt" accompanying this file.
//******************************

using System.Collections.Generic;

namespace Wintellect.PowerCollections.Tests
{
    using MyInt = OrderedDictionaryTests.MyInt;

    public partial class MultiDictionaryTests
    {
        private class MyIntComparer : IEqualityComparer<MyInt>
        {
            public bool Equals(MyInt x, MyInt y)
            {
                if (x == null)
                    return y == null;
                else if (y == null)
                    return x == null;
                else
                    return x.value == y.value;
            }

            public int GetHashCode(MyInt obj)
            {
                if (obj == null)
                    return 0x42834E;
                else
                    return obj.value.GetHashCode();
            }
        }
    }
}

