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
        /// A private class used to implement GetCollectionEqualityComparer(). This
        /// class implements IEqualityComparer&lt;IEnumerable&lt;T&gt;gt; to compare
        /// two enumerables for equality, where order is significant.
        /// </summary>
        [Serializable]
        private class CollectionEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
        {
            private readonly IEqualityComparer<T> equalityComparer;

            public CollectionEqualityComparer(IEqualityComparer<T> equalityComparer)
            {
                this.equalityComparer = equalityComparer;
            }

            public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
            {
                return Algorithms.EqualCollections(x, y, equalityComparer);
            }

            public int GetHashCode(IEnumerable<T> obj)
            {
                int hash = 0x374F293E;
                foreach (T t in obj) {
                    int itemHash = Util.GetHashCode(t, equalityComparer);
                    hash += itemHash;
                    hash = (hash << 9) | (hash >> 23);
                }

                return hash & 0x7FFFFFFF;
            }
        }
    }
}
