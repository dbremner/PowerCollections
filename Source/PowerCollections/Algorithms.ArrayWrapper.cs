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
        /// The class that is used to implement IList&lt;T&gt; to view an array
        /// in a read-write way. Insertions cause the last item in the array
        /// to fall off, deletions replace the last item with the default value.
        /// </summary>
        [Serializable]
        private class ArrayWrapper<T> : ListBase<T>, IList
        {
            private readonly T[] wrappedArray;

            /// <summary>
            /// Create a list wrapper object on an array.
            /// </summary>
            /// <param name="wrappedArray">Array to wrap.</param>
            public ArrayWrapper(T[] wrappedArray)
            {
                this.wrappedArray = wrappedArray;
            }

            public override int Count
            {
                get
                {
                    return wrappedArray.Length;
                }
            }

            public override void Clear()
            {
                int count = wrappedArray.Length;
                for (int i = 0; i < count; ++i)
                    wrappedArray[i] = default(T);
            }

            public override void Insert(int index, T item)
            {
                if (index < 0 || index > wrappedArray.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (index + 1 < wrappedArray.Length)
                    Array.Copy(wrappedArray, index, wrappedArray, index + 1, wrappedArray.Length - index - 1);
                if (index < wrappedArray.Length)
                    wrappedArray[index] = item;
            }

            public override void RemoveAt(int index)
            {
                if (index < 0 || index >= wrappedArray.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (index < wrappedArray.Length - 1)
                    Array.Copy(wrappedArray, index + 1, wrappedArray, index, wrappedArray.Length - index - 1);
                wrappedArray[wrappedArray.Length - 1] = default(T);
            }

            public override T this[int index]
            {
                get
                {
                    if (index < 0 || index >= wrappedArray.Length)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    return wrappedArray[index];
                }
                set
                {
                    if (index < 0 || index >= wrappedArray.Length)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    wrappedArray[index] = value;
                }
            }

            public override void CopyTo(T[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (array.Length < wrappedArray.Length)
                    throw new ArgumentException(Strings.ArrayTooSmall, nameof(array));
                if (arrayIndex < 0 || arrayIndex >= array.Length)
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                if (array.Length + arrayIndex < wrappedArray.Length)
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex));

                Array.Copy(wrappedArray, 0, array, arrayIndex, wrappedArray.Length);
            }

            public override IEnumerator<T> GetEnumerator()
            {
                return ((IList<T>)wrappedArray).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IList)wrappedArray).GetEnumerator();
            }

            /// <summary>
            /// Return true, to indicate that the list is fixed size.
            /// </summary>
            bool IList.IsFixedSize
            {
                get
                {
                    return true;
                }
            }
        }
    }
}
