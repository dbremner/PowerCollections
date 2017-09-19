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
        /// The read-only ICollection&lt;T&gt; implementation that is used by the ReadOnly method.
        /// Methods that modify the collection throw a NotSupportedException, methods that don't
        /// modify are fowarded through to the wrapped collection.
        /// </summary>
        [Serializable]
        private sealed class ReadOnlyCollection<T> : ICollection<T>
        {
            private readonly ICollection<T> wrappedCollection;  // The collection we are wrapping (never null).

            /// <summary>
            /// Create a ReadOnlyCollection wrapped around the given collection.
            /// </summary>
            /// <param name="wrappedCollection">Collection to wrap.</param>
            public ReadOnlyCollection(ICollection<T> wrappedCollection)
            {
                this.wrappedCollection = wrappedCollection;
            }

            /// <summary>
            /// Throws an NotSupportedException stating that this collection cannot be modified.
            /// </summary>
            private static void MethodModifiesCollection()
            {
                throw new NotSupportedException(string.Format(Strings.CannotModifyCollection, "read-only collection"));
            }


            public IEnumerator<T> GetEnumerator()
            { return wrappedCollection.GetEnumerator(); }

            IEnumerator IEnumerable.GetEnumerator()
            { return ((IEnumerable)wrappedCollection).GetEnumerator(); }

            public bool Contains(T item)
            { return wrappedCollection.Contains(item); }

            public void CopyTo(T[] array, int arrayIndex)
            { wrappedCollection.CopyTo(array, arrayIndex); }

            public int Count
            {
                get { return wrappedCollection.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public void Add(T item)
            { MethodModifiesCollection(); }

            public void Clear()
            { MethodModifiesCollection(); }

            public bool Remove(T item)
            { MethodModifiesCollection(); return false; }
        }
    }
}
