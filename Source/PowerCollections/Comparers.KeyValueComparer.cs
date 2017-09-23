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
        /// Class to change an IComparer&lt;TKey&gt; to an IComparer&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; 
        /// Only the keys are compared.
        /// </summary>
        [Serializable]
        class KeyValueComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
        {
            private readonly IComparer<TKey> keyComparer;

            public KeyValueComparer(IComparer<TKey> keyComparer)
            { this.keyComparer = keyComparer; }

            public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            { return keyComparer.Compare(x.Key, y.Key); }

            public override bool Equals(object obj)
            {
                if (obj is KeyValueComparer<TKey, TValue> comparer)
                    return object.Equals(keyComparer, comparer.keyComparer);
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return keyComparer.GetHashCode();
            }
        }
    }
}
