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
        ///  The class that provides a typed IEnumerator&lt;T&gt;
        /// view onto an untyped IEnumerator interface.
        /// </summary>
        [Serializable]
        private class TypedEnumerator<T> : IEnumerator<T>
        {
            private readonly IEnumerator wrappedEnumerator;

            /// <summary>
            /// Create a typed IEnumerator&lt;T&gt;
            /// view onto an untyped IEnumerator interface 
            /// </summary>
            /// <param name="wrappedEnumerator">IEnumerator to wrap.</param>
            public TypedEnumerator(IEnumerator wrappedEnumerator)
            {
                this.wrappedEnumerator = wrappedEnumerator;
            }

            T IEnumerator<T>.Current
            {
                get { return (T) wrappedEnumerator.Current; }
            }

            void IDisposable.Dispose()
            {
                if (wrappedEnumerator is IDisposable)
                    ((IDisposable)wrappedEnumerator).Dispose();
            }

            object IEnumerator.Current
            {
                get { return wrappedEnumerator.Current; }
            }

            bool IEnumerator.MoveNext()
            {
                return wrappedEnumerator.MoveNext();
            }

            void IEnumerator.Reset()
            {
                wrappedEnumerator.Reset();
            }
        }
    }
}
