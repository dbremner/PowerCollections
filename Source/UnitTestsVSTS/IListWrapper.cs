//******************************
// Written by Peter Golde
// Copyright (c) 2004-2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement
// contained in the file "License.txt" accompanying this file.
//******************************

using System.Collections.Generic;
using System.Collections;

namespace Wintellect.PowerCollections.Tests
{
    /// <summary>
    /// Wrapper for IList&lt;T&gt; so that no casting to another interface can be done.
    /// </summary>
    internal class IListWrapper<T> : IList<T>
    {
        private readonly IList<T> wrapped;

        public IListWrapper(IList<T> wrapped)
        {
            this.wrapped = wrapped;
        }

        int IList<T>.IndexOf(T item)
        {
            return wrapped.IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            wrapped.Insert(index, item);
        }

        void IList<T>.RemoveAt(int index)
        {
            wrapped.RemoveAt(index);
        }

        T IList<T>.this[int index]
        {
            get
            {
                return wrapped[index];
            }
            set
            {
                wrapped[index] = value;
            }
        }

        void ICollection<T>.Add(T item)
        {
            wrapped.Add(item);
        }

        void ICollection<T>.Clear()
        {
            wrapped.Clear();
        }

        bool ICollection<T>.Contains(T item)
        {
            return wrapped.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            wrapped.CopyTo(array, arrayIndex);
        }

        int ICollection<T>.Count
        {
            get { return wrapped.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return wrapped.IsReadOnly; }
        }

        bool ICollection<T>.Remove(T item)
        {
            return wrapped.Remove(item);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)wrapped).GetEnumerator();
        }
    }
}
