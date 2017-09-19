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
        /// The class that implements a non-generic IList wrapper
        /// around a generic IList&lt;T&gt; interface.
        /// </summary>
        [Serializable]
        private class UntypedList<T> : IList
        {
            private readonly IList<T> wrappedList;

            /// <summary>
            /// Create a non-generic IList wrapper
            /// around a generic IList&lt;T&gt; interface.
            /// </summary>
            /// <param name="wrappedList">The IList&lt;T&gt; interface to wrap.</param>
            public UntypedList(IList<T> wrappedList)
            {
                this.wrappedList = wrappedList;
            }

            /// <summary>
            /// Convert the given parameter to T. Throw an ArgumentException
            /// if it isn't.
            /// </summary>
            /// <param name="name">parameter name</param>
            /// <param name="value">parameter value</param>
            private static T ConvertToItemType(string name, object value)
            {
                try {
                    return (T)value;
                }
                catch (InvalidCastException) {
                    throw new ArgumentException(string.Format(Strings.WrongType, value, typeof(T)), name);
                }
            }


            public int Add(object value)
            {  
                // We assume that Add always adds to the end. Is this true?
                wrappedList.Add(ConvertToItemType(nameof(value), value));
                return wrappedList.Count - 1;
            }

            public void Clear()
            {  wrappedList.Clear(); }

            public bool Contains(object value)
            {
                if (value is T)
                    return wrappedList.Contains((T)value);
                else
                    return false;
            }

            public int IndexOf(object value)
            {
                if (value is T)
                    return wrappedList.IndexOf((T)value);
                else
                    return -1;
            }

            public void Insert(int index, object value)
            { wrappedList.Insert(index, ConvertToItemType(nameof(value), value)); }

            public bool IsFixedSize
            {
                get { return false; }
            }

            public bool IsReadOnly
            {
                get { return wrappedList.IsReadOnly; }
            }

            public void Remove(object value)
            {  
                if (value is T)
                    wrappedList.Remove((T)value); 
            }   

            public void RemoveAt(int index)
            {  wrappedList.RemoveAt(index);}

            public object this[int index]
            {
                get { return wrappedList[index]; }
                set { wrappedList[index] = ConvertToItemType(nameof(value), value); }
            }

            public void CopyTo(Array array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));

                int i = 0;
                int count = wrappedList.Count;

                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), index, Strings.ArgMustNotBeNegative);
                if (index >= array.Length || count > array.Length - index)
                    throw new ArgumentException(Strings.ArrayTooSmall, nameof(index));

                foreach (T item in wrappedList) {
                    if (i >= count)
                        break;

                    array.SetValue(item, index);
                    ++index;
                    ++i;
                }
            }

            public int Count
            {
                get { return wrappedList.Count; }
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
            {  return ((IEnumerable)wrappedList).GetEnumerator(); }
        }
    }
}
