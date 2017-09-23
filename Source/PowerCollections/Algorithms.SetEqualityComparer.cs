//******************************
// Written by Peter Golde
// Copyright (c) 2004-2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement 
// contained in the file "License.txt" accompanying this file.
//******************************

using System;
using System.Collections.Generic;

namespace Wintellect.PowerCollections {
    public static partial class Algorithms
    {
        /// <summary>
        /// A private class used to implement GetSetEqualityComparer(). This
        /// class implements IEqualityComparer&lt;IEnumerable&lt;T&gt;gt; to compare
        /// two enumerables for equality, where order is not significant.
        /// </summary>
        [Serializable]
        private class SetEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
        {
            private readonly IEqualityComparer<T> equalityComparer;

            public SetEqualityComparer(IEqualityComparer<T> equalityComparer)
            {
                this.equalityComparer = equalityComparer;
            }

            public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
            {
                return Algorithms.EqualSets(x, y, equalityComparer);
            }

            public int GetHashCode(IEnumerable<T> obj)
            {
                int hash = 0x624F273C;
                foreach (T t in obj) {
                    int itemHash = Util.GetHashCode(t, equalityComparer);
                    hash += itemHash;
                }

                return hash & 0x7FFFFFFF;
            }
        }
    }
}
