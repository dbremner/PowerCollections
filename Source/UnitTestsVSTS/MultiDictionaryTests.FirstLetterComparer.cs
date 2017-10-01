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
    public partial class MultiDictionaryTests
    {
        private class FirstLetterComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                if (x == null)
                    return y == null;
                else if (x.Length == 0)
                    return (y != null && y.Length == 0);
                else {
                    if (y == null || y.Length == 0)
                        return false;
                    else
                        return x[0] == y[0];
                }
            }

            public int GetHashCode(string obj)
            {
                if (obj == null)
                    return 0x12383;
                else if (obj.Length == 0)
                    return 17;
                else
                    return obj[0].GetHashCode();
            }
        }
    }
}

