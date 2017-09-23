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
        /// A private class used by the LexicographicalComparer method to compare sequences
        /// (IEnumerable) of T by there Lexicographical ordering.
        /// </summary>
        [Serializable]
        private class LexicographicalComparerClass<T> : IComparer<IEnumerable<T>>
        {
            readonly IComparer<T> itemComparer;

            /// <summary>
            /// Creates a new instance that comparer sequences of T by their lexicographical
            /// ordered.
            /// </summary>
            /// <param name="itemComparer">The IComparer used to compare individual items of type T.</param>
            public LexicographicalComparerClass(IComparer<T> itemComparer)
            {
                this.itemComparer = itemComparer;
            }

            public int Compare(IEnumerable<T> x, IEnumerable<T> y)
            {
                return LexicographicalCompare(x, y, itemComparer);
            }


            // For comparing this comparer to others.

            public override bool Equals(object obj)
            {
                if (obj is LexicographicalComparerClass<T> lexicographicalComparer)
                    return this.itemComparer.Equals(lexicographicalComparer.itemComparer);
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return itemComparer.GetHashCode();
            }
        }
    }
}
