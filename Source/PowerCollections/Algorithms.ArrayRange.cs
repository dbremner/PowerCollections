//******************************
// Written by Peter Golde
// Copyright (c) 2004-2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement 
// contained in the file "License.txt" accompanying this file.
//******************************

using System;

namespace Wintellect.PowerCollections {
    public static partial class Algorithms
    {
        /// <summary>
        /// The class that is used to implement IList&lt;T&gt; to view a sub-range
        /// of an array. The object stores a wrapped array, and a start/count indicating
        /// a sub-range of the array. Insertion/deletions through the sub-range view
        /// cause the count to change up to the size of the underlying array. Elements
        /// fall off the end of the underlying array.
        /// </summary>
        [Serializable]
        private class ArrayRange<T> : ListBase<T>
        {
            private readonly T[] wrappedArray;
            private readonly int start;
            private int count;

            /// <summary>
            /// Create a sub-range view object on the indicate part 
            /// of the array.
            /// </summary>
            /// <param name="wrappedArray">Array to wrap.</param>
            /// <param name="start">The start index of the view in the wrapped list.</param>
            /// <param name="count">The number of items in the view.</param>
            public ArrayRange(T[] wrappedArray, int start, int count)
            {
                this.wrappedArray = wrappedArray;
                this.start = start;
                this.count = count;
            }

            public override int Count
            {
                get
                {
                    return count;
                }
            }

            public override void Clear()
            {
                Array.Copy(wrappedArray, start + count, wrappedArray, start, wrappedArray.Length - (start + count));
                Algorithms.FillRange(wrappedArray, wrappedArray.Length - count, count, default(T));
                count = 0;
            }

            public override void Insert(int index, T item)
            {
                if (index < 0 || index > count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                int i = start + index;

                if (i + 1 < wrappedArray.Length)
                    Array.Copy(wrappedArray, i, wrappedArray, i + 1, wrappedArray.Length - i - 1);
                if (i < wrappedArray.Length)
                    wrappedArray[i] = item;

                if (start + count < wrappedArray.Length)
                    ++count;
            }

            public override void RemoveAt(int index)
            {
                if (index < 0 || index >= count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                int i = start + index;

                if (i < wrappedArray.Length - 1)
                    Array.Copy(wrappedArray, i + 1, wrappedArray, i, wrappedArray.Length - i - 1);
                wrappedArray[wrappedArray.Length - 1] = default(T);

                --count;
            }

            public override T this[int index]
            {
                get
                {
                    if (index < 0 || index >= count)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    return wrappedArray[start + index];
                }
                set
                {
                    if (index < 0 || index >= count)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    wrappedArray[start + index] = value;
                }
            }
        }
    }
}
