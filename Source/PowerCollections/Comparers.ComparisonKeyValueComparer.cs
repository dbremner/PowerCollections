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
        /// Class to change an Comparison&lt;TKey&gt; to an IComparer&lt;KeyValuePair&lt;TKey, TValue&gt;&gt;.
        /// GetHashCode cannot be used on this class.
        /// </summary>
        [Serializable]
        class ComparisonKeyValueComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
        {
            private readonly Comparison<TKey> comparison;

            public ComparisonKeyValueComparer(Comparison<TKey> comparison)
            { this.comparison = comparison; }

            public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            { return comparison(x.Key, y.Key); }

            public override bool Equals(object obj)
            {
                if (obj is ComparisonKeyValueComparer<TKey, TValue> comparer)
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
