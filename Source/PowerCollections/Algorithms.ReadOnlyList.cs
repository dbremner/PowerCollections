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
        /// The read-only IList&lt;T&gt; implementation that is used by the ReadOnly method.
        /// Methods that modify the list throw a NotSupportedException, methods that don't
        /// modify are fowarded through to the wrapped list.
        /// </summary>
        [Serializable]
        private sealed class ReadOnlyList<T> : IList<T>
        {
            private readonly IList<T> wrappedList;  // The list we are wrapping (never null).

            /// <summary>
            /// Create a ReadOnlyList wrapped around the given list.
            /// </summary>
            /// <param name="wrappedList">List to wrap.</param>
            public ReadOnlyList(IList<T> wrappedList)
            {
                this.wrappedList = wrappedList;
            }

            /// <summary>
            /// Throws an NotSupportedException stating that this collection cannot be modified.
            /// </summary>
            private static void MethodModifiesCollection()
            {
                throw new NotSupportedException(string.Format(Strings.CannotModifyCollection, "read-only list"));
            }


            public IEnumerator<T> GetEnumerator()
            { return wrappedList.GetEnumerator(); }

            IEnumerator IEnumerable.GetEnumerator()
            { return ((IEnumerable)wrappedList).GetEnumerator(); }

            public int IndexOf(T item)
            { return wrappedList.IndexOf(item); }

            public bool Contains(T item)
            {  return wrappedList.Contains(item); }

            public void CopyTo(T[] array, int arrayIndex)
            {  wrappedList.CopyTo(array, arrayIndex); }

            public int Count
            {
                get { return wrappedList.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public T this[int index]
            {
                get { return wrappedList[index]; }
                set { MethodModifiesCollection(); }
            }

            public void Add(T item)
            { MethodModifiesCollection(); }

            public void Clear()
            { MethodModifiesCollection(); }

            public void Insert(int index, T item)
            { MethodModifiesCollection(); }

            public void RemoveAt(int index)
            { MethodModifiesCollection(); }

            public bool Remove(T item)
            { MethodModifiesCollection(); return false; }
        }
    }
}
