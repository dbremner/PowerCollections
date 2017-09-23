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
        /// Class to change an IComparer&lt;TKey&gt; and IComparer&lt;TValue&gt; to an IComparer&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; 
        /// Keys are compared, followed by values.
        /// </summary>
        [Serializable]
        class PairComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
        {
            private readonly IComparer<TKey> keyComparer;
            private readonly IComparer<TValue> valueComparer;

            public PairComparer(IComparer<TKey> keyComparer, IComparer<TValue> valueComparer)
            { 
                this.keyComparer = keyComparer; 
                this.valueComparer = valueComparer; 
            }

            public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            {
                int keyCompare = keyComparer.Compare(x.Key, y.Key);

                if (keyCompare == 0)
                    return valueComparer.Compare(x.Value, y.Value);
                else
                    return keyCompare;
            }

            public override bool Equals(object obj)
            {
                if (obj is PairComparer<TKey, TValue> comparer)
                    return object.Equals(keyComparer, comparer.keyComparer) &&
                        object.Equals(valueComparer, comparer.valueComparer);
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return keyComparer.GetHashCode() ^ valueComparer.GetHashCode();
            }
        }
    }
}
