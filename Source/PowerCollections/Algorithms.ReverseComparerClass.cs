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
        /// An IComparer instance that can be used to reverse the sense of 
        /// a wrapped IComparer instance.
        /// </summary>
        [Serializable]
        private class ReverseComparerClass<T> : IComparer<T>
        {
            readonly IComparer<T> comparer;

            /// <summary>
            /// </summary>
            /// <param name="comparer">The comparer to reverse.</param>
            public ReverseComparerClass(IComparer<T> comparer)
            {
                this.comparer = comparer;
            }

            public int Compare(T x, T y)
            {
                return - comparer.Compare(x, y);
            }

            // For comparing this comparer to others.

            public override bool Equals(object obj)
            {
                if (obj is ReverseComparerClass<T> reverseComparer)
                    return this.comparer.Equals(reverseComparer.comparer);
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return comparer.GetHashCode();
            }
        }
    }
}
