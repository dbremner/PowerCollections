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
        /// The class that provides a typed IEnumerable&lt;T&gt; view
        /// onto an untyped IEnumerable interface.
        /// </summary>
        [Serializable]
        private sealed class TypedEnumerable<T> : IEnumerable<T>
        {
            private readonly IEnumerable wrappedEnumerable;

            /// <summary>
            /// Create a typed IEnumerable&lt;T&gt; view
            /// onto an untyped IEnumerable interface.
            /// </summary>
            /// <param name="wrappedEnumerable">IEnumerable interface to wrap.</param>
            public TypedEnumerable(IEnumerable wrappedEnumerable)
            {
                this.wrappedEnumerable = wrappedEnumerable;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new TypedEnumerator<T>(wrappedEnumerable.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return wrappedEnumerable.GetEnumerator();
            }
        }
    }
}
