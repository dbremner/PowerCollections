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
        /// The class that is used to provide an untyped ICollection
        /// view onto a typed ICollection&lt;T&gt; interface.
        /// </summary>
        [Serializable]
        private sealed class UntypedCollection<T> : ICollection
        {
            private readonly ICollection<T> wrappedCollection;

            /// <summary>
            /// Create an untyped ICollection
            /// view onto a typed ICollection&lt;T&gt; interface.
            /// </summary>
            /// <param name="wrappedCollection">The ICollection&lt;T&gt; to wrap.</param>
            public UntypedCollection(ICollection<T> wrappedCollection)
            {
                this.wrappedCollection = wrappedCollection;
            }


            public void CopyTo(Array array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));

                int i = 0;
                int count = wrappedCollection.Count;

                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), index, Strings.ArgMustNotBeNegative);
                if (index >= array.Length || count > array.Length - index)
                    throw new ArgumentException(Strings.ArrayTooSmall, nameof(index));

                foreach (T item in wrappedCollection) {
                    if (i >= count)
                        break;

                    array.SetValue(item, index);
                    ++index;
                    ++i;
                }
            }

            public int Count
            {
                get { return wrappedCollection.Count; }
            }

            public bool IsSynchronized
            {
                get { return false; }
            }

            public object SyncRoot
            {
                get { return this; }
            }

            public IEnumerator GetEnumerator()
            {
                return ((IEnumerable)wrappedCollection).GetEnumerator();
            }
        }
    }
}
