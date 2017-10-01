using System.Collections;
using System.Collections.Generic;

namespace Wintellect.PowerCollections.Tests
{
    /// <summary>
    /// Wrapped for ICollection&lt;T&gt; so that no casting to another interface can be done.
    /// </summary>
    internal class ICollectionWrapper<T> : ICollection<T>
    {
        private readonly ICollection<T> wrapped;

        public ICollectionWrapper(ICollection<T> wrapped)
        {
            this.wrapped = wrapped;
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