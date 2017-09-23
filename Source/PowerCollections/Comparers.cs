//******************************
// Written by Peter Golde
// Copyright (c) 2004-2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement 
// contained in the file "License.txt" accompanying this file.
//******************************

using System;
using System.Collections.Generic;

namespace Wintellect.PowerCollections
{
    /// <summary>
    /// A collection of methods to create IComparer and IEqualityComparer instances in various ways.
    /// </summary>
    internal static partial class Comparers
    {
        /// <summary>
        /// Given an Comparison on a type, returns an IComparer on that type. 
        /// </summary>
        /// <typeparam name="T">T to compare.</typeparam>
        /// <param name="comparison">Comparison delegate on T</param>
        /// <returns>IComparer that uses the comparison.</returns>
        public static IComparer<T> ComparerFromComparison<T>(Comparison<T> comparison)
        {
            if (comparison == null)
                throw new ArgumentNullException(nameof(comparison));

            return new ComparisonComparer<T>(comparison);
        }

        /// <summary>
        /// Given an IComparer on TKey, returns an IComparer on
        /// key-value Pairs. 
        /// </summary>
        /// <typeparam name="TKey">TKey of the pairs</typeparam>
        /// <typeparam name="TValue">TValue of the apris</typeparam>
        /// <param name="keyComparer">IComparer on TKey</param>
        /// <returns>IComparer for comparing key-value pairs.</returns>
        public static IComparer<KeyValuePair<TKey, TValue>> ComparerKeyValueFromComparerKey<TKey, TValue>(IComparer<TKey> keyComparer)
        {
            if (keyComparer == null)
                throw new ArgumentNullException(nameof(keyComparer));

            return new KeyValueComparer<TKey, TValue>(keyComparer);
        }

        /// <summary>
        /// Given an IEqualityComparer on TKey, returns an IEqualityComparer on
        /// key-value Pairs. 
        /// </summary>
        /// <typeparam name="TKey">TKey of the pairs</typeparam>
        /// <typeparam name="TValue">TValue of the apris</typeparam>
        /// <param name="keyEqualityComparer">IComparer on TKey</param>
        /// <returns>IEqualityComparer for comparing key-value pairs.</returns>
        public static IEqualityComparer<KeyValuePair<TKey, TValue>> EqualityComparerKeyValueFromComparerKey<TKey, TValue>(IEqualityComparer<TKey> keyEqualityComparer)
        {
            if (keyEqualityComparer == null)
                throw new ArgumentNullException(nameof(keyEqualityComparer));

            return new KeyValueEqualityComparer<TKey, TValue>(keyEqualityComparer);
        }

        /// <summary>
        /// Given an IComparer on TKey and TValue, returns an IComparer on
        /// key-value Pairs of TKey and TValue, comparing first keys, then values. 
        /// </summary>
        /// <typeparam name="TKey">TKey of the pairs</typeparam>
        /// <typeparam name="TValue">TValue of the apris</typeparam>
        /// <param name="keyComparer">IComparer on TKey</param>
        /// <param name="valueComparer">IComparer on TValue</param>
        /// <returns>IComparer for comparing key-value pairs.</returns>
        public static IComparer<KeyValuePair<TKey, TValue>> ComparerPairFromKeyValueComparers<TKey, TValue>(IComparer<TKey> keyComparer, IComparer<TValue> valueComparer)
        {
            if (keyComparer == null)
                throw new ArgumentNullException(nameof(keyComparer));
            if (valueComparer == null)
                throw new ArgumentNullException(nameof(valueComparer));

            return new PairComparer<TKey, TValue>(keyComparer, valueComparer);
        }

        /// <summary>
        /// Given an Comparison on TKey, returns an IComparer on
        /// key-value Pairs. 
        /// </summary>
        /// <typeparam name="TKey">TKey of the pairs</typeparam>
        /// <typeparam name="TValue">TValue of the apris</typeparam>
        /// <param name="keyComparison">Comparison delegate on TKey</param>
        /// <returns>IComparer for comparing key-value pairs.</returns>
        public static IComparer<KeyValuePair<TKey, TValue>> ComparerKeyValueFromComparisonKey<TKey, TValue>(Comparison<TKey> keyComparison)
        {
            if (keyComparison == null)
                throw new ArgumentNullException(nameof(keyComparison));

            return new ComparisonKeyValueComparer<TKey, TValue>(keyComparison);
        }

        /// <summary>
        /// Given an element type, check that it implements IComparable&lt;T&gt; or IComparable, then returns
        /// a IComparer that can be used to compare elements of that type.
        /// </summary>
        /// <returns>The IComparer&lt;T&gt; instance.</returns>
        /// <exception cref="InvalidOperationException">T does not implement IComparable&lt;T&gt;.</exception>
        public static IComparer<T> DefaultComparer<T>()
        {
            if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)) ||
                typeof(System.IComparable).IsAssignableFrom(typeof(T))) 
            {
                return Comparer<T>.Default;
            }
            else {
                throw new InvalidOperationException(string.Format(Strings.UncomparableType, typeof(T).FullName));
            }
        }

        /// <summary>
        /// Given an key and value type, check that TKey implements IComparable&lt;T&gt; or IComparable, then returns
        /// a IComparer that can be used to compare KeyValuePairs of those types.
        /// </summary>
        /// <returns>The IComparer&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; instance.</returns>
        /// <exception cref="InvalidOperationException">TKey does not implement IComparable&lt;T&gt;.</exception>
        public static IComparer<KeyValuePair<TKey, TValue>> DefaultKeyValueComparer<TKey, TValue>()
        {
            IComparer<TKey> keyComparer = DefaultComparer<TKey>();
            return ComparerKeyValueFromComparerKey<TKey,TValue>(keyComparer);
        }
    }
}
