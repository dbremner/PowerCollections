﻿//******************************
// Written by Peter Golde
// Copyright (c) 2004-2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement 
// contained in the file "License.txt" accompanying this file.
//******************************

using System;
using System.Collections.Generic;

namespace Wintellect.PowerCollections {
    public static partial class Algorithms
    {
        /// <summary>
        /// A class, implementing IEqualityComparer&lt;T&gt;, that compares objects
        /// for object identity only. Only Equals and GetHashCode can be used;
        /// this implementation is not appropriate for ordering.
        /// </summary>
        [Serializable]
        private class IdentityComparer<T> : EqualityComparer<T>
            where T:class
        {
            public override bool Equals(T x, T y)
            {
                return ReferenceEquals(x, y);
            }

            public override int GetHashCode(T obj)
            {
                return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
            }

            // For comparing two IComparers to see if they compare the same thing.
            public override bool Equals(object obj)
            {
                return (obj is IdentityComparer<T>);
            }

            // For comparing two IComparers to see if they compare the same thing.
            public override int GetHashCode()
            {
                return this.GetType().Name.GetHashCode() ^ typeof(T).GetHashCode();
            }
        }
    }
}
