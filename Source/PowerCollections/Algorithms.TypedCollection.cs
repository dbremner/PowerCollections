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
        /// The class that provides a typed ICollection&lt;T&gt; view
        /// onto an untyped ICollection interface. The ICollection&lt;T&gt;
        /// is read-only.
        /// </summary>
        [Serializable]
        private sealed class TypedCollection<T> : ICollection<T>
        {
            private readonly ICollection wrappedCollection;

            /// <summary>
            /// Create a typed ICollection&lt;T&gt; view
            /// onto an untyped ICollection interface.
            /// </summary>
            /// <param name="wrappedCollection">ICollection interface to wrap.</param>
            public TypedCollection(ICollection wrappedCollection)
            {
                this.wrappedCollection = wrappedCollection;
            }

            /// <summary>
            /// Throws an NotSupportedException stating that this collection cannot be modified.
            /// </summary>
            private static void MethodModifiesCollection()
            {
                throw new NotSupportedException(string.Format(Strings.CannotModifyCollection, "strongly-typed Collection"));
            }

            public void Add(T item)
            {  MethodModifiesCollection(); }

            public void Clear()
            {  MethodModifiesCollection(); }

            public bool Remove(T item)
            { MethodModifiesCollection(); return false; }

            public bool Contains(T item)
            {
                IEqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
                foreach (object obj in wrappedCollection) {
                    if (obj is T && equalityComparer.Equals(item, (T)obj))
                        return true;
                }
                return false;
            }

            public void CopyTo(T[] array, int arrayIndex)
            {  wrappedCollection.CopyTo(array, arrayIndex); }

            public int Count
            {
                get { return wrappedCollection.Count;  }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public IEnumerator<T> GetEnumerator()
            {  return new TypedEnumerator<T>(wrappedCollection.GetEnumerator()); }

            IEnumerator IEnumerable.GetEnumerator()
            {  return wrappedCollection.GetEnumerator(); }
        }
    }
}
