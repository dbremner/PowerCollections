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
        /// The class used to create a typed IList&lt;T&gt; view onto
        /// an untype IList interface.
        /// </summary>
        [Serializable]
        private class TypedList<T> : IList<T>
        {
            private readonly IList wrappedList;

            /// <summary>
            /// Create a typed IList&lt;T&gt; view onto
            /// an untype IList interface.
            /// </summary>
            /// <param name="wrappedList">The IList to wrap.</param>
            public TypedList(IList wrappedList)
            {
                this.wrappedList = wrappedList;
            }


            public IEnumerator<T> GetEnumerator()
            { return new TypedEnumerator<T>(wrappedList.GetEnumerator()); }

            IEnumerator IEnumerable.GetEnumerator()
            { return wrappedList.GetEnumerator(); }

            public int IndexOf(T item)
            {  return wrappedList.IndexOf(item); }

            public void Insert(int index, T item)
            {  wrappedList.Insert(index, item); }

            public void RemoveAt(int index)
            {  wrappedList.RemoveAt(index); }

            public void Add(T item)
            {  wrappedList.Add(item); }

            public void Clear()
            {  wrappedList.Clear(); }

            public bool Contains(T item)
            {  return wrappedList.Contains(item); }

            public void CopyTo(T[] array, int arrayIndex)
            {  wrappedList.CopyTo(array, arrayIndex); }

            public T this[int index]
            {
                get { return (T)wrappedList[index]; }
                set { wrappedList[index] = value; }
            }

            public int Count
            {
                get { return wrappedList.Count ; }
            }

            public bool IsReadOnly
            {
                get { return wrappedList.IsReadOnly; }
            }

            public bool Remove(T item)
            {
                if (wrappedList.Contains(item)) {
                    wrappedList.Remove(item);
                    return true;
                }
                else {
                    return false;
                }
            }
        }
    }
}
