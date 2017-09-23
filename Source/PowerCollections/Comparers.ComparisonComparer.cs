//******************************
// Written by Peter Golde
// Copyright (c) 2004-2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement 
// contained in the file "License.txt" accompanying this file.
//******************************

using System;
using System.Collections.Generic;

namespace Wintellect.PowerCollections
{
    internal static partial class Comparers
    {
        /// <summary>
        /// Class to change an Comparison&lt;T&gt; to an IComparer&lt;T&gt;.
        /// </summary>
        [Serializable]
        class ComparisonComparer<T> : IComparer<T>
        {
            private readonly Comparison<T> comparison;

            public ComparisonComparer(Comparison<T> comparison)
            { this.comparison = comparison; }

            public int Compare(T x, T y)
            { return comparison(x, y); }

            public override bool Equals(object obj)
            {
                if (obj is ComparisonComparer<T> comparer)
                    return comparison.Equals(comparer.comparison);
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return comparison.GetHashCode();
            }
        }
    }
}
