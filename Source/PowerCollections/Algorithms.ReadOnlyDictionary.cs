//******************************
// Written by Peter Golde
// Copyright (c) 2004-2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement 
// contained in the file "License.txt" accompanying this file.
//******************************

using System;
using System.Collections;
using System.Collections.Generic;

namespace Wintellect.PowerCollections {
    public static partial class Algorithms
    {
        /// <summary>
        /// The private class that implements a read-only wrapped for 
        /// IDictionary &lt;TKey,TValue&gt;.
        /// </summary>
        [Serializable]
        private sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        {
            // The dictionary that is wrapped
            private readonly IDictionary<TKey, TValue> wrappedDictionary;

            /// <summary>
            /// Create a read-only dictionary wrapped around the given dictionary.
            /// </summary>
            /// <param name="wrappedDictionary">The IDictionary&lt;TKey,TValue&gt; to wrap.</param>
            public ReadOnlyDictionary(IDictionary<TKey, TValue> wrappedDictionary)
            {
                this.wrappedDictionary = wrappedDictionary;
            }

            /// <summary>
            /// Throws an NotSupportedException stating that this collection cannot be modified.
            /// </summary>
            private static void MethodModifiesCollection()
            {
                throw new NotSupportedException(string.Format(Strings.CannotModifyCollection, "read-only dictionary"));
            }

            public void Add(TKey key, TValue value)
            { MethodModifiesCollection(); }

            public bool ContainsKey(TKey key)
            { return wrappedDictionary.ContainsKey(key); }

            public ICollection<TKey> Keys
            { 
                get { return ReadOnly(wrappedDictionary.Keys); } 
            }

            public ICollection<TValue> Values
            { 
                get { return ReadOnly(wrappedDictionary.Values); } 
            }

            public bool Remove(TKey key)
            { 
                MethodModifiesCollection();
                return false;  // never reached
            }

            public bool TryGetValue(TKey key, out TValue value)
            { return wrappedDictionary.TryGetValue(key, out value); }

            public TValue this[TKey key]
            {
                get { return wrappedDictionary[key];}
                set { MethodModifiesCollection(); }
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            { MethodModifiesCollection(); }

            public void Clear()
            { MethodModifiesCollection(); }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            { return wrappedDictionary.Contains(item); }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            { wrappedDictionary.CopyTo(array, arrayIndex); }

            public int Count
            {
                get { return wrappedDictionary.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            { 
                MethodModifiesCollection();
                return false;     // never reached
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            { return wrappedDictionary.GetEnumerator(); }

            IEnumerator IEnumerable.GetEnumerator()
            { return ((IEnumerable)wrappedDictionary).GetEnumerator(); }
        }
    }
}
