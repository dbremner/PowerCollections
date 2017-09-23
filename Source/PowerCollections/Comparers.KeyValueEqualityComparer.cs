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
        /// Class to change an IEqualityComparer&lt;TKey&gt; to an IEqualityComparer&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; 
        /// Only the keys are compared.
        /// </summary>
        [Serializable]
        class KeyValueEqualityComparer<TKey, TValue> : IEqualityComparer<KeyValuePair<TKey, TValue>>
        {
            private readonly IEqualityComparer<TKey> keyEqualityComparer;

            public KeyValueEqualityComparer(IEqualityComparer<TKey> keyEqualityComparer)
            { this.keyEqualityComparer = keyEqualityComparer; }

            public bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            { return keyEqualityComparer.Equals(x.Key, y.Key); }

            public int GetHashCode(KeyValuePair<TKey, TValue> obj)
            {
                return Util.GetHashCode(obj.Key, keyEqualityComparer);
            }

            public override bool Equals(object obj)
            {
                if (obj is KeyValueEqualityComparer<TKey, TValue> comparer)
                    return object.Equals(keyEqualityComparer, comparer.keyEqualityComparer);
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return keyEqualityComparer.GetHashCode();
            }
        }
    }
}
