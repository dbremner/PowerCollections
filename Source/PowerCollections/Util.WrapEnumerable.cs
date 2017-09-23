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

namespace Wintellect.PowerCollections
{
    internal static partial class Util
    {
        /// <summary>
        /// Wrap an enumerable so that clients can't get to the underlying 
        /// implementation via a down-cast.
        /// </summary>
        [Serializable]
        class WrapEnumerable<T> : IEnumerable<T>
        {
            IEnumerable<T> wrapped;

            /// <summary>
            /// Create the wrapper around an enumerable.
            /// </summary>
            /// <param name="wrapped">IEnumerable to wrap.</param>
            public WrapEnumerable(IEnumerable<T> wrapped)
            {
                this.wrapped = wrapped;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return wrapped.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)wrapped).GetEnumerator();
            }
        }
    }
}
