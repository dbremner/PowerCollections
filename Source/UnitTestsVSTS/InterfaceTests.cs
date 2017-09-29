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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Wintellect.PowerCollections.Tests.TestPredicates;

namespace Wintellect.PowerCollections.Tests {
	/// <summary>
	/// A collection of generic tests for various interfaces.
	/// </summary>
	internal static class InterfaceTests
	{
        public static BinaryPredicate<KeyValuePair<TKey,TValue>> KeyValueEquals<TKey, TValue>(BinaryPredicate<TKey> keyEquals, BinaryPredicate<TValue> valueEquals)
        {
            if (keyEquals == null)
                keyEquals = ObjectEquals;
            if (valueEquals == null)
                valueEquals = ObjectEquals;

            bool KvpEquals(KeyValuePair<TKey, TValue> pair1, KeyValuePair<TKey, TValue> pair2) {
                return keyEquals(pair1.Key, pair2.Key) && valueEquals(pair1.Value, pair2.Value);
            }

            return KvpEquals;
        }

        public static BinaryPredicate<KeyValuePair<TKey, TValue>> KeyValueEquals<TKey, TValue>()
        {
            return KeyValueEquals<TKey, TValue>(null, null);
        }

        public static BinaryPredicate<ICollection<T>> CollectionEquals<T>(BinaryPredicate<T> equals, bool inOrder)
        {
            if (inOrder) {
                return (ICollection<T> enum1, ICollection<T> enum2) => {
                    if (enum1 == null || enum2 == null)
                        return (enum1 == enum2);
                    else
                        return Algorithms.EqualCollections(enum1, enum2, equals);
                };
            }
            else {
                return (ICollection<T> enum1, ICollection<T> enum2) => {
                    if (enum1 == null || enum2 == null)
                        return (enum1 == enum2);

                    T[] expected = Enumerable.ToArray(enum2);
                    bool[] found = new bool[expected.Length];
                    int i = 0;
                    foreach (T item in enum1) {
                        int index;
                        for (index = 0; index < expected.Length; ++index) {
                            if (!found[index] && equals(expected[index], item))
                                break;
                        }
                        if (index >= expected.Length)
                            return false;
                        if (!equals(expected[index], item))
                            return false;
                        found[index] = true;
                        ++i;
                    }
                    if (expected.Length != i)
                        return false;
                    else
                        return true;
                };
            }
        }

       /// <summary>
       /// Test an IEnumerable should contain the given values in order
       /// </summary>
       public static void TestEnumerableElements<T>(IEnumerable<T> e, T[] expected)
       {
            TestEnumerableElements(e, expected, null);
       }

       public static void TestEnumerableElements<T>(IEnumerable<T> e, T[] expected, BinaryPredicate<T> equals)
       {
           if (equals == null)
               equals = ObjectEquals;

           int i = 0;
           foreach (T item in e) {
               Assert.IsTrue(equals(expected[i], item));
               ++i;
           }
           Assert.AreEqual(expected.Length, i);
       }

       /// <summary>
       /// Test an IEnumerable should contain the given values in any order
       /// </summary>
       public static void TestEnumerableElementsAnyOrder<T>(IEnumerable<T> e, T[] expected)
       {
            TestEnumerableElementsAnyOrder(e, expected, null);
       }

       public static void TestEnumerableElementsAnyOrder<T>(IEnumerable<T> e, T[] expected, BinaryPredicate<T> equals)
       {
           if (equals == null)
               equals = ObjectEquals;

           bool[] found = new bool[expected.Length];
           int i = 0;
           foreach (T item in e) {
               int index;
               for (index = 0; index < expected.Length; ++index) {
                   if (!found[index] && equals(expected[index], item))
                       break;
               }
               Assert.IsTrue(index < expected.Length);
               Assert.IsTrue(equals(expected[index], item));
               found[index] = true;
               ++i;
           }
           Assert.AreEqual(expected.Length, i);
       }

       /// <summary>
       ///  Test an ICollection that should contain the given values, possibly in order.
		/// </summary>
		/// <param name="coll">ICollection to test. </param>
		/// <param name="valueArray">The values that should be in the collection.</param>
		/// <param name="mustBeInOrder">Must the values be in order?</param>
		public static void TestCollection<T>(ICollection coll, T[] valueArray, bool mustBeInOrder)
		{
			var values = (T[])valueArray.Clone();		// clone the array so we can destroy it.

			// Check ICollection.Count.
			Assert.AreEqual(values.Length, coll.Count);

			// Check ICollection.GetEnumerator().
			int i = 0, j;

			foreach (T s in coll)
			{
				if (mustBeInOrder)
				{
					Assert.AreEqual(values[i], s);
				}
				else
				{
					bool found = false;

					for (j = 0; j < values.Length; ++j)
					{
                        if (object.Equals(values[j],s))
						{
							found = true;
							values[j] = default(T);
							break;
						}
					}

					Assert.IsTrue(found);
				}

				++i;
			}

			// Check IsSyncronized, SyncRoot.
			Assert.IsFalse(coll.IsSynchronized);
			Assert.IsNotNull(coll.SyncRoot);

			// Check CopyTo.
			values = (T[])valueArray.Clone();		// clone the array so we can destroy it.

			T[] newKeys = new T[coll.Count + 2];

			coll.CopyTo(newKeys, 1);
			for (i = 0, j = 1; i < coll.Count; ++i, ++j)
			{
				if (mustBeInOrder)
				{
					Assert.AreEqual(values[i], newKeys[j]);
				}
				else
				{
					bool found = false;

					for (int k = 0; k < values.Length; ++k)
					{
						if (object.Equals(values[k], newKeys[j]))
						{
							found = true;
                            values[k] = default(T);
                            break;
						}
					}

					Assert.IsTrue(found);
				}
			}

			// Shouldn't have disturbed the values around what was filled in.
			Assert.AreEqual(default(T), newKeys[0]);
			Assert.AreEqual(default(T), newKeys[coll.Count + 1]);

			// Check CopyTo exceptions.
            if (coll.Count > 0) {
			    Assert.ThrowsException<ArgumentNullException>(() => coll.CopyTo(null, 0));
			    Assert.ThrowsException<ArgumentException>(() => coll.CopyTo(newKeys, 3));
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.CopyTo(newKeys, -1));
            }
    }

        /// /// <summary>
		///  Test an ICollection&lt;string&gt; that should contain the given values, possibly in order. Checks only the following items:
		///     GetEnumerator, CopyTo, Count, Contains
		/// </summary>
		/// <param name="coll">ICollection to test. </param>
		/// <param name="values">The elements that should be in the collection.</param>
		/// <param name="mustBeInOrder">Must the elements be in order?</param>
        /// <param name="equals">Predicate to test for equality; null for default.</param>
		private static void TestCollectionGeneric<T>(ICollection<T> coll, T[] values, bool mustBeInOrder, BinaryPredicate<T> equals)
		{
            if (equals == null)
                equals = ObjectEquals;

            bool[] used = new bool[values.Length];

			// Check ICollection.Count.
			Assert.AreEqual(values.Length, coll.Count);

			// Check ICollection.GetEnumerator().
			int i = 0, j;

			foreach (T s in coll)
			{
				if (mustBeInOrder)
				{
					Assert.IsTrue(equals(values[i], s));
				}
				else
				{
					bool found = false;

					for (j = 0; j < values.Length; ++j)
					{
						if (!used[j] && equals(values[j],s))
						{
							found = true;
                            used[j] = true;
							break;
						}
					}

					Assert.IsTrue(found);
				}

				++i;
			}

            // Check Contains
            foreach (T s in values) {
                Assert.IsTrue(coll.Contains(s));
            }

            // Check CopyTo.
            used = new bool[values.Length];

			T[] newKeys = new T[coll.Count + 2];

			coll.CopyTo(newKeys, 1);
			for (i = 0, j = 1; i < coll.Count; ++i, ++j)
			{
				if (mustBeInOrder)
				{
					Assert.IsTrue(equals(values[i], newKeys[j]));
				}
				else
				{
					bool found = false;

					for (int k = 0; k < values.Length; ++k)
					{
						if (!used[k] && equals(values[k], newKeys[j]))
						{
							found = true;
                            used[k] = true;
							break;
						}
					}

					Assert.IsTrue(found);
				}
			}

			// Shouldn't have distubed the values around what was filled in.
			Assert.IsTrue(equals(default(T), newKeys[0]));
            Assert.IsTrue(equals(default(T), newKeys[coll.Count + 1]));

            if (coll.Count != 0)
			{
				// Check CopyTo exceptions.
				Assert.ThrowsException<ArgumentNullException>(() => coll.CopyTo(null, 0));
				Assert.ThrowsException<ArgumentException>(() => coll.CopyTo(newKeys, 3));
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.CopyTo(newKeys, -1));
            }
        }

	    /// <summary>
		///  Test a readonly ICollection&lt;string&gt; that should contain the given values, possibly in order. Checks only the following items:
		///     GetEnumerator, CopyTo, Count, Contains, IsReadOnly
		/// </summary>
		/// <param name="coll">ICollection&lt;T&gt; to test. </param>
		/// <param name="valueArray">The values that should be in the collection.</param>
		/// <param name="mustBeInOrder">Must the value be in order?</param>
		/// <param name="name">Expected name of the collection, or null for don't check.</param>
        public static void TestReadonlyCollectionGeneric<T>(ICollection<T> coll, T[] valueArray, bool mustBeInOrder, string name)
        {
            TestReadonlyCollectionGeneric(coll, valueArray, mustBeInOrder, null, null);
        }

        public static void TestReadonlyCollectionGeneric<T>(ICollection<T> coll, T[] valueArray, bool mustBeInOrder, string name, BinaryPredicate<T> equals)
        {
            TestCollectionGeneric(coll, valueArray, mustBeInOrder, equals);

            // Test read-only flag.
            Assert.IsTrue(coll.IsReadOnly);

            // Check that Clear throws correct exception
            if (coll.Count > 0) {
                Assert.ThrowsException<NotSupportedException>(() => coll.Clear());
            }

            // Check that Add throws correct exception
            Assert.ThrowsException<NotSupportedException>(() => coll.Add(default(T)));

            // Check throws correct exception
            Assert.ThrowsException<NotSupportedException>(() => coll.Remove(default(T)));
        }

        /// <summary>
        ///  Test a read-write ICollection&lt;string&gt; that should contain the given values, possibly in order. Destroys the collection in the process.
        /// </summary>
        /// <param name="coll">ICollection to test. </param>
        /// <param name="valueArray">The values that should be in the collection.</param>
        /// <param name="mustBeInOrder">Must the values be in order?</param>
        public static void TestReadWriteCollectionGeneric<T>(ICollection<T> coll, T[] valueArray, bool mustBeInOrder)
        {
            TestReadWriteCollectionGeneric(coll, valueArray, mustBeInOrder, null);
        }

        public static void TestReadWriteCollectionGeneric<T>(ICollection<T> coll, T[] valueArray, bool mustBeInOrder, BinaryPredicate<T> equals)
        {
            TestCollectionGeneric(coll, valueArray, mustBeInOrder, equals);

            // Test read-only flag.
            Assert.IsFalse(coll.IsReadOnly);

            // Clear and Count.
            coll.Clear();
            Assert.AreEqual(0, coll.Count);

            // Add all the items back.
            foreach (T item in valueArray)
                coll.Add(item);
            Assert.AreEqual(valueArray.Length, coll.Count);
            TestCollectionGeneric(coll, valueArray, mustBeInOrder, equals);

            // Remove all the items again.
            foreach (T item in valueArray)
                coll.Remove(item);
            Assert.AreEqual(0, coll.Count);
        }

        /// <summary>
        /// Test an IDictionary that should contains the given keys and values, possibly in order.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys</typeparam>
        /// <typeparam name="TValue">Type of the values</typeparam>
        /// <param name="dict">IDictionary to test</param>
        /// <param name="keys">key values for the dictionary</param>
        /// <param name="values">values for the dictionary</param>
        /// <param name="nonKey">A TKey that isn't in the dictionary</param>
        /// <param name="mustBeInOrder">True if the entries must be in order.</param>
        public static void TestDictionary<TKey, TValue>(IDictionary dict, TKey[] keys, TValue[] values, TKey nonKey, bool mustBeInOrder)
        {
            // Check Count.
            Assert.AreEqual(keys.Length, dict.Count);

            // Check containment.
            for (int i = 0; i < keys.Length; ++i) {
                Assert.IsTrue(dict.Contains(keys[i]));
                Assert.AreEqual(dict[keys[i]], values[i]);
            }

            Assert.IsFalse(dict.Contains(nonKey));
            Assert.IsNull(dict[nonKey]);

            Assert.IsFalse(dict.Contains(new object()));
            Assert.IsNull(dict[new object()]);

            // Check synchronization
            Assert.IsFalse(dict.IsSynchronized);
            Assert.IsNotNull(dict.SyncRoot);

            // Check Keys, Values collections
            TestCollection(dict.Keys, keys, mustBeInOrder);
            TestCollection(dict.Values, values, mustBeInOrder);

            // Check DictionaryEnumerator.
            int count = 0;
            bool[] found = new bool[keys.Length];

            IDictionaryEnumerator enumerator = dict.GetEnumerator();
            while (enumerator.MoveNext()) {
                DictionaryEntry entry = enumerator.Entry;

                Assert.AreEqual(enumerator.Entry.Key, enumerator.Key);
                Assert.AreEqual(enumerator.Entry.Value, enumerator.Value);
                Assert.AreEqual(((DictionaryEntry)(enumerator.Current)).Key, enumerator.Key);
                Assert.AreEqual(((DictionaryEntry)(enumerator.Current)).Value, enumerator.Value);

                // find the entry.
                if (mustBeInOrder) {
                    Assert.AreEqual(keys[count], enumerator.Key);
                    Assert.AreEqual(values[count], enumerator.Value);
                }
                else {
                    for (int i = 0; i < keys.Length; ++i) {
                        if ((!found[i]) && object.Equals(keys[i], enumerator.Key) && object.Equals(values[i], enumerator.Value)) {
                            found[i] = true;
                        }
                    }
                }
                ++count;
            }
            Assert.AreEqual(count, keys.Length);
            if (!mustBeInOrder)
                for (int i = 0; i < keys.Length; ++i)
                    Assert.IsTrue(found[i]);
        }

        /// <summary>
        /// Test an read-only IDictionary that should contains the given keys and values, possibly in order.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys</typeparam>
        /// <typeparam name="TValue">Type of the values</typeparam>
        /// <param name="dict">IDictionary to test</param>
        /// <param name="keys">key values for the dictionary</param>
        /// <param name="values">values for the dictionary</param>
        /// <param name="mustBeInOrder">True if the entries must be in order.</param>
        /// <param name="nonKey">A TKey that isn't in the dictionary</param>
        /// <param name="name">Name of the dictionary, used in exceptions.</param>
        public static void TestReadOnlyDictionary<TKey, TValue>(IDictionary dict, TKey[] keys, TValue[] values, TKey nonKey, bool mustBeInOrder, string name)
        {
            DictionaryEntry[] entries = new DictionaryEntry[keys.Length];
            for (int i = 0; i < keys.Length; ++i)
                entries[i] = new DictionaryEntry(keys[i], values[i]);

            TestCollection((ICollection)dict, entries, mustBeInOrder);

            TestDictionary(dict, keys, values, nonKey, mustBeInOrder);

            Assert.IsTrue(dict.IsReadOnly);
            Assert.IsTrue(dict.IsFixedSize);

            // Check exceptions.
            Assert.ThrowsException<NotSupportedException>(() => dict.Clear());

            Assert.ThrowsException<NotSupportedException>(() => dict.Add(keys[0], values[0]));

            Assert.ThrowsException<NotSupportedException>(() => dict.Remove(keys[0]));

            Assert.ThrowsException<NotSupportedException>(() => dict[keys[0]] = values[0]);
        }

        /// <summary>
        /// Test an read-write IDictionary that should contains the given keys and values, possibly in order.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys</typeparam>
        /// <typeparam name="TValue">Type of the values</typeparam>
        /// <param name="dict">IDictionary to test</param>
        /// <param name="keys">key values for the dictionary</param>
        /// <param name="values">values for the dictionary</param>
        /// <param name="mustBeInOrder">True if the entries must be in order.</param>
        /// <param name="nonKey">A TKey that isn't in the dictionary</param>
        /// <param name="name">Name of the dictionary, used in exceptions.</param>
        public static void TestReadWriteDictionary<TKey, TValue>(IDictionary dict, TKey[] keys, TValue[] values, TKey nonKey, bool mustBeInOrder, string name)
        {
            DictionaryEntry[] entries = new DictionaryEntry[keys.Length];
            for (int i = 0; i < keys.Length; ++i)
                entries[i] = new DictionaryEntry(keys[i], values[i]);

            TestCollection((ICollection)dict, entries, mustBeInOrder);
            TestDictionary(dict, keys, values, nonKey, mustBeInOrder);

            Assert.IsFalse(dict.IsReadOnly);
            Assert.IsFalse(dict.IsFixedSize);

            // Check exceptions for adding existing elements.
            for (int i = 0; i < keys.Length; ++i) {
                Assert.ThrowsException<ArgumentException>(() => dict.Add(keys[i], values[i]));
            }

            // Check Clear.
            dict.Clear();
            Assert.AreEqual(0, dict.Count);

            // Check Add with incorrect types.
            Assert.ThrowsException<ArgumentException>(() => dict.Add(new object(), values[0]));

            Assert.ThrowsException<ArgumentException>(() => dict.Add(keys[0], new object()));

            // Check Add().
            for (int i = 0; i < keys.Length; ++i)
                dict.Add(keys[i], values[i]);

            TestCollection((ICollection)dict, entries, mustBeInOrder);
            TestDictionary(dict, keys, values, nonKey, mustBeInOrder);

            // Check Remove. 2nd remove should do nothing.
            for (int i = 0; i < keys.Length; ++i) {
                dict.Remove(keys[i]);
                dict.Remove(keys[i]);
            }

            // Remove with incorrect type.
            dict.Remove(new object());

            Assert.AreEqual(0, dict.Count);

            // Check indexer with incorrect types.
            Assert.ThrowsException<ArgumentException>(() => dict[new object()] = values[0]);

            Assert.ThrowsException<ArgumentException>(() => dict[keys[0]] = new object());

            // Check adding via the indexer
            for (int i = 0; i < keys.Length; ++i)
                dict[keys[i]] = values[i];

            TestCollection((ICollection)dict, entries, mustBeInOrder);
            TestDictionary(dict, keys, values, nonKey, mustBeInOrder);
        }

        /// <summary>
        /// Test an generic IDictionary&lt;K,V&gt; that should contains the given keys and values, possibly in order.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys</typeparam>
        /// <typeparam name="TValue">Type of the values</typeparam>
        /// <param name="dict">IDictionary&lt;K,V&gt; to test</param>
        /// <param name="keys">key values for the dictionary</param>
        /// <param name="values">values for the dictionary</param>
        /// <param name="nonKey">A TKey that isn't in the dictionary</param>
        /// <param name="mustBeInOrder">True if the entries must be in order.</param>
        public static void TestDictionaryGeneric<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey[] keys, TValue[] values, TKey nonKey, bool mustBeInOrder, BinaryPredicate<TKey> keyEquals, BinaryPredicate<TValue> valueEquals)
        {
            bool result;
            TValue val;

            if (keyEquals == null)
                keyEquals = ObjectEquals;
            if (valueEquals == null)
                valueEquals = ObjectEquals;

            // Check Count.
            Assert.AreEqual(keys.Length, dict.Count);

            // Check containment.
            for (int i = 0; i < keys.Length; ++i) {
                Assert.IsTrue(dict.ContainsKey(keys[i]));
                Assert.IsTrue(valueEquals(values[i], dict[keys[i]]));
                result = dict.TryGetValue(keys[i], out val);
                Assert.IsTrue(result);
                Assert.IsTrue(valueEquals(values[i], val));
            }

            Assert.IsFalse(dict.ContainsKey(nonKey));
            result = dict.TryGetValue(nonKey, out val);
            Assert.IsFalse(result);
            Assert.AreEqual(default(TValue), val);

            Assert.ThrowsException<KeyNotFoundException>(() => {var unused = dict[nonKey];});

            // Check Keys, Values collections
            TestReadonlyCollectionGeneric(dict.Keys, keys, mustBeInOrder, null, keyEquals);
            TestReadonlyCollectionGeneric(dict.Values, values, mustBeInOrder, null, valueEquals);
        }

        /// <summary>
        /// Test an read-only IDictionary&lt;K,V&gt; that should contains the given keys and values, possibly in order.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys</typeparam>
        /// <typeparam name="TValue">Type of the values</typeparam>
        /// <param name="dict">IDictionary&lt;K,V&gt; to test</param>
        /// <param name="keys">key values for the dictionary</param>
        /// <param name="values">values for the dictionary</param>
        /// <param name="mustBeInOrder">True if the entries must be in order.</param>
        /// <param name="nonKey">A TKey that isn't in the dictionary</param>
        /// <param name="name">Name of the dictionary, used in exceptions.</param>
        public static void TestReadOnlyDictionaryGeneric<TKey, TValue>(IDictionary<TKey,TValue> dict, TKey[] keys, TValue[] values, TKey nonKey, bool mustBeInOrder, string name,
            BinaryPredicate<TKey> keyEquals, BinaryPredicate<TValue> valueEquals)
        {
            if (keyEquals == null)
                keyEquals = ObjectEquals;
            if (valueEquals == null)
                valueEquals = ObjectEquals;

            KeyValuePair<TKey, TValue>[] entries = new KeyValuePair<TKey, TValue>[keys.Length];
            for (int i = 0; i < keys.Length; ++i)
                entries[i] = Kvp.Of(keys[i], values[i]);

            TestDictionaryGeneric(dict, keys, values, nonKey, mustBeInOrder, keyEquals, valueEquals);
            TestReadonlyCollectionGeneric((ICollection<KeyValuePair<TKey,TValue>>)dict, entries, mustBeInOrder, name, KeyValueEquals(keyEquals, valueEquals));

            // Check exceptions.
            Assert.ThrowsException<NotSupportedException>(() => dict.Clear());

            if (keys.Length > 0) {
                Assert.ThrowsException<NotSupportedException>(() => dict.Add(keys[0], values[0]));

                Assert.ThrowsException<NotSupportedException>(() => dict.Remove(keys[0]));

                Assert.ThrowsException<NotSupportedException>(() => dict[keys[0]] = values[0]);
            }
        }

        /// <summary>
        /// Test an read-write IDictionary&lt;K,V&gt; that should contains the given keys and values, possibly in order.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys</typeparam>
        /// <typeparam name="TValue">Type of the values</typeparam>
        /// <param name="dict">IDictionary&lt;K,V&gt; to test</param>
        /// <param name="keys">key values for the dictionary</param>
        /// <param name="values">values for the dictionary</param>
        /// <param name="mustBeInOrder">True if the entries must be in order.</param>
        /// <param name="nonKey">A TKey that isn't in the dictionary</param>
        /// <param name="name">Name of the dictionary, used in exceptions.</param>
        public static void TestReadWriteDictionaryGeneric<TKey, TValue>(IDictionary<TKey,TValue> dict, TKey[] keys, TValue[] values, TKey nonKey, bool mustBeInOrder, string name,
            BinaryPredicate<TKey> keyEquals, BinaryPredicate<TValue> valueEquals)
        {
            if (keyEquals == null)
                keyEquals = ObjectEquals;
            if (valueEquals == null)
                valueEquals = ObjectEquals;

            KeyValuePair<TKey, TValue>[] entries = new KeyValuePair<TKey, TValue>[keys.Length];
            for (int i = 0; i < keys.Length; ++i)
                entries[i] = Kvp.Of(keys[i], values[i]);

            TestDictionaryGeneric(dict, keys, values, nonKey, mustBeInOrder, keyEquals, valueEquals);
            TestCollectionGeneric((ICollection<KeyValuePair<TKey, TValue>>)dict, entries, mustBeInOrder, KeyValueEquals(keyEquals, valueEquals));

            Assert.IsFalse(dict.IsReadOnly);

            // Check exceptions for adding existing elements.
            for (int i = 0; i < keys.Length; ++i) {
                Assert.ThrowsException<ArgumentException>(() => dict.Add(keys[i], values[i]));
            }

            // Check Clear.
            dict.Clear();
            Assert.AreEqual(0, dict.Count);

            // Check Add().
            for (int i = 0; i < keys.Length; ++i)
                dict.Add(keys[i], values[i]);

            TestDictionaryGeneric(dict, keys, values, nonKey, mustBeInOrder, keyEquals, valueEquals);
            TestCollectionGeneric((ICollection<KeyValuePair<TKey, TValue>>)dict, entries, mustBeInOrder, KeyValueEquals(keyEquals, valueEquals));

            // Check Remove. 2nd remove should return false.
            for (int i = 0; i < keys.Length; ++i) {
                Assert.IsTrue(dict.Remove(keys[i]));
                Assert.IsFalse(dict.Remove(keys[i]));
            }

            Assert.AreEqual(0, dict.Count);

            // Check adding via the indexer
            for (int i = 0; i < keys.Length; ++i)
                dict[keys[i]] = values[i];

            TestDictionaryGeneric(dict, keys, values, nonKey, mustBeInOrder, keyEquals, valueEquals);
            TestCollectionGeneric((ICollection<KeyValuePair<TKey, TValue>>)dict, entries, mustBeInOrder, KeyValueEquals(keyEquals, valueEquals));
        }

        /// <summary>
        /// Test read-only IList&lt;T&gt; that should contain the given values, possibly in order. Does not change
        /// the list. Does not force the list to be read-only.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll">IList&lt;T&gt; to test. </param>
        /// <param name="valueArray">The values that should be in the list.</param>
        public static void TestListGeneric<T>(IList<T> coll, T[] valueArray)
        {
            TestListGeneric(coll, valueArray, null);
        }

        public static void TestListGeneric<T>(IList<T> coll, T[] valueArray, BinaryPredicate<T> equals)
        {
            if (equals == null)
                equals = ObjectEquals;

            // Check basic read-only collection stuff.
            TestCollectionGeneric(coll, valueArray, true, equals);

            // Check the indexer getter and IndexOf, backwards
            for (int i = coll.Count - 1; i >= 0; --i) {
                Assert.IsTrue(equals(valueArray[i], coll[i]));
                int index = coll.IndexOf(valueArray[i]);
                Assert.IsTrue(index >= 0);
                Assert.IsTrue(index == i || (index < i && equals(coll[index], valueArray[i])));
            }

            // Check the indexer getter and IndexOf, forwards
            for (int i = 0; i < valueArray.Length ; ++i) {
                Assert.IsTrue(equals(valueArray[i], coll[i]));
                int index = coll.IndexOf(valueArray[i]);
                Assert.IsTrue(index >= 0);
                Assert.IsTrue(index == i || (index < i && equals(coll[index], valueArray[i])));
            }

            // Check the indexer getter and IndexOf, jumping by 3s
            for (int i = 0; i < valueArray.Length; i += 3) {
                Assert.IsTrue(equals(valueArray[i], coll[i]));
                int index = coll.IndexOf(valueArray[i]);
                Assert.IsTrue(index >= 0);
                Assert.IsTrue(index == i || (index < i && equals(coll[index], valueArray[i])));
            }

            // Check exceptions from index out of range.
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[-1];});
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[int.MinValue];});
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[-2];});
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[coll.Count];});
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[int.MaxValue];});
        }

        /// <summary>
        /// Test read-only non-generic IList that should contain the given values, possibly in order. Does not change
        /// the list. Does not force the list to be read-only.
        /// </summary>
        /// <param name="coll">IList to test. </param>
        /// <param name="valueArray">The values that should be in the list.</param>
        public static void TestList<T>(IList coll, T[] valueArray)
        {
            // Check basic read-only collection stuff.
            TestCollection(coll, valueArray, true);

            // Check the indexer getter and IndexOf, backwards
            for (int i = coll.Count - 1; i >= 0; --i) {
                Assert.AreEqual(valueArray[i], coll[i]);
                int index = coll.IndexOf(valueArray[i]);
                Assert.IsTrue(coll.Contains(valueArray[i]));
                Assert.IsTrue(index >= 0);
                Assert.IsTrue(index == i || (index < i && object.Equals(coll[index], valueArray[i])));
            }

            // Check the indexer getter and IndexOf, forwards
            for (int i = 0; i < valueArray.Length; ++i) {
                Assert.AreEqual(valueArray[i], coll[i]);
                int index = coll.IndexOf(valueArray[i]);
                Assert.IsTrue(coll.Contains(valueArray[i]));
                Assert.IsTrue(index >= 0);
                Assert.IsTrue(index == i || (index < i && object.Equals(coll[index], valueArray[i])));
            }

            // Check the indexer getter and IndexOf, jumping by 3s
            for (int i = 0; i < valueArray.Length; i += 3) {
                Assert.AreEqual(valueArray[i], coll[i]);
                int index = coll.IndexOf(valueArray[i]);
                Assert.IsTrue(coll.Contains(valueArray[i]));
                Assert.IsTrue(index >= 0);
                Assert.IsTrue(index == i || (index < i && object.Equals(coll[index], valueArray[i])));
            }

            // Check exceptions from index out of range.
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[-1];});
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[int.MinValue];});
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[-2];});
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[coll.Count];});
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[int.MaxValue];});

            // Check bad type.
            if (typeof(T) != typeof(object)) {
                int index = coll.IndexOf(new object());
                Assert.AreEqual(-1, index);

                bool b = coll.Contains(new object());
                Assert.IsFalse(b);
            }
        }

        /// <summary>
        ///  Test a read-write IList&lt;T&gt; that should contain the given values, possibly in order. Destroys the collection in the process.
        /// </summary>
        /// <param name="coll">IList&lt;T&gt; to test. </param>
        /// <param name="valueArray">The values that should be in the list.</param>
        public static void TestReadWriteListGeneric<T>(IList<T> coll, T[] valueArray)
        {
            TestReadWriteListGeneric(coll, valueArray, null);
        }

        public static void TestReadWriteListGeneric<T>(IList<T> coll, T[] valueArray, BinaryPredicate<T> equals)
        {
            if (equals == null)
                equals = ObjectEquals;

            TestListGeneric(coll, valueArray, equals);     // Basic read-only list stuff.

            // Check the indexer getter.
            T[] save = new T[coll.Count];
            for (int i = coll.Count - 1; i >= 0; --i) {
                Assert.AreEqual(valueArray[i], coll[i]);
                int index = coll.IndexOf(valueArray[i]);
                Assert.IsTrue(index >= 0);
                Assert.IsTrue(index == i || (index < i && object.Equals(coll[index], valueArray[i])));
                save[i] = coll[i];
            }

            // Check the setter by reversing the list.
            for (int i = 0; i < coll.Count / 2; ++i) {
                T temp = coll[i];
                coll[i] = coll[coll.Count - 1 - i];
                coll[coll.Count - 1 - i] = temp;
            }

            for (int i = 0; i < coll.Count; ++i ) {
                Assert.AreEqual(valueArray[coll.Count - 1 - i], coll[i]);
                int index = coll.IndexOf(valueArray[coll.Count - 1 - i]);
                Assert.IsTrue(index >= 0);
                Assert.IsTrue(index == i || (index < i && object.Equals(coll[index], valueArray[coll.Count - 1 - i])));
            }

            // Reverse back
            for (int i = 0; i < coll.Count / 2; ++i) {
                T temp = coll[i];
                coll[i] = coll[coll.Count - 1 - i];
                coll[coll.Count - 1 - i] = temp;
            }

            T item = valueArray.Length > 0 ? valueArray[valueArray.Length / 2] : default(T);
            // Check exceptions from index out of range.
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll[-1] = item);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll[int.MinValue] = item);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[-2];});
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll[coll.Count] = item);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[coll.Count];});
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[int.MaxValue];});
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll[int.MaxValue] = item);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.Insert(-1, item));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.Insert(coll.Count + 1, item));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.Insert(int.MaxValue, item));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.RemoveAt(coll.Count));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.RemoveAt(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.RemoveAt(int.MaxValue));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.RemoveAt(coll.Count));

            // Insert at the beginning.
            coll.Insert(0, item);
            Assert.AreEqual(coll[0], item);
            Assert.AreEqual(valueArray.Length + 1, coll.Count);
            for (int i = 0; i < valueArray.Length; ++i)
                Assert.AreEqual(valueArray[i], coll[i + 1]);

            // Insert at the end
            coll.Insert(valueArray.Length + 1, item);
            Assert.AreEqual(coll[valueArray.Length + 1], item);
            Assert.AreEqual(valueArray.Length + 2, coll.Count);
            for (int i = 0; i < valueArray.Length; ++i)
                Assert.AreEqual(valueArray[i], coll[i + 1]);

            // Delete at the beginning.
            coll.RemoveAt(0);
            Assert.AreEqual(coll[valueArray.Length], item);
            Assert.AreEqual(valueArray.Length + 1, coll.Count);
            for (int i = 0; i < valueArray.Length; ++i)
                Assert.AreEqual(valueArray[i], coll[i]);

            // Delete at the end.
            coll.RemoveAt(valueArray.Length);
            Assert.AreEqual(valueArray.Length, coll.Count);
            for (int i = 0; i < valueArray.Length; ++i)
                Assert.AreEqual(valueArray[i], coll[i]);

            // Insert at the middle.
            coll.Insert(valueArray.Length / 2, item);
            Assert.AreEqual(valueArray.Length + 1, coll.Count);
            Assert.AreEqual(item, coll[valueArray.Length / 2]);
            for (int i = 0; i < valueArray.Length; ++i) {
                if (i < valueArray.Length / 2)
                    Assert.AreEqual(valueArray[i], coll[i]);
                else
                    Assert.AreEqual(valueArray[i], coll[i+1]);
            }

            // Delete at the middle.
            coll.RemoveAt(valueArray.Length / 2);
            Assert.AreEqual(valueArray.Length, coll.Count);
            for (int i = 0; i < valueArray.Length; ++i)
                Assert.AreEqual(valueArray[i], coll[i]);

            // Delete all from the middle.
            for (int i = 0; i < valueArray.Length; ++i)
                coll.RemoveAt(coll.Count / 2);
            Assert.AreEqual(0, coll.Count);

            // Build up in order.
            for (int i = 0; i < save.Length; ++i) {
                coll.Insert(i, save[i]);
            }

            TestListGeneric(coll, valueArray, equals);     // Basic read-only list stuff.

            coll.Clear();
            Assert.AreEqual(0, coll.Count);

            // Build up in reverse order.
            for (int i = 0; i < save.Length; ++i) {
                coll.Insert(0, save[save.Length - 1 - i]);
            }
            TestListGeneric(coll, valueArray, equals);     // Basic read-only list stuff.

            // Check read-write collection stuff.
            TestReadWriteCollectionGeneric(coll, valueArray, true);
        }

        /// <summary>
        ///  Test a read-write non-generic IList that should contain the given values, possibly in order. Destroys the collection in the process.
        /// </summary>
        /// <param name="coll">IList to test. </param>
        /// <param name="valueArray">The values that should be in the list.</param>
        public static void TestReadWriteList<T>(IList coll, T[] valueArray)
        {
            TestList(coll, valueArray);     // Basic read-only list stuff.

            // Check read only
            Assert.IsFalse(coll.IsReadOnly);
            Assert.IsFalse(coll.IsReadOnly);

            // Check the indexer getter.
            T[] save = new T[coll.Count];
            for (int i = coll.Count - 1; i >= 0; --i) {
                Assert.AreEqual(valueArray[i], coll[i]);
                int index = coll.IndexOf(valueArray[i]);
                Assert.IsTrue(index >= 0);
                Assert.IsTrue(index == i || (index < i && object.Equals(coll[index], valueArray[i])));
                save[i] = (T) coll[i];
            }

            // Check the setter by reversing the list.
            for (int i = 0; i < coll.Count / 2; ++i) {
                var temp = (T) coll[i];
                coll[i] = coll[coll.Count - 1 - i];
                coll[coll.Count - 1 - i] = temp;
            }

            for (int i = 0; i < coll.Count; ++i ) {
                Assert.AreEqual(valueArray[coll.Count - 1 - i], coll[i]);
                int index = coll.IndexOf(valueArray[coll.Count - 1 - i]);
                Assert.IsTrue(index >= 0);
                Assert.IsTrue(index == i || (index < i && object.Equals(coll[index], valueArray[coll.Count - 1 - i])));
            }

            // Reverse back
            for (int i = 0; i < coll.Count / 2; ++i) {
                var temp = (T) coll[i];
                coll[i] = coll[coll.Count - 1 - i];
                coll[coll.Count - 1 - i] = temp;
            }

            T item = valueArray.Length > 0 ? valueArray[valueArray.Length / 2] : default(T);
            // Check exceptions from index out of range.
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll[-1] = item);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll[int.MinValue] = item);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[-2];});
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll[coll.Count] = item);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[coll.Count];});
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var dummy = coll[int.MaxValue];});
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll[int.MaxValue] = item);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.Insert(-1, item));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.Insert(coll.Count + 1, item));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.Insert(int.MaxValue, item));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.RemoveAt(coll.Count));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.RemoveAt(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.RemoveAt(int.MaxValue));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll.RemoveAt(coll.Count));

            // Check operations with bad type.
            if (typeof(T) != typeof(object)) {
                Assert.ThrowsException<ArgumentException>(() => coll.Add(new object()));

                Assert.ThrowsException<ArgumentException>(() => coll.Insert(0, new object()));

                int index = coll.IndexOf(new object());
                Assert.AreEqual(-1, index);

                coll.Remove(new object());
            }

            // Insert at the beginning.
            coll.Insert(0, item);
            Assert.AreEqual(coll[0], item);
            Assert.AreEqual(valueArray.Length + 1, coll.Count);
            for (int i = 0; i < valueArray.Length; ++i)
                Assert.AreEqual(valueArray[i], coll[i + 1]);

            // Insert at the end
            coll.Insert(valueArray.Length + 1, item);
            Assert.AreEqual(coll[valueArray.Length + 1], item);
            Assert.AreEqual(valueArray.Length + 2, coll.Count);
            for (int i = 0; i < valueArray.Length; ++i)
                Assert.AreEqual(valueArray[i], coll[i + 1]);

            // Delete at the beginning.
            coll.RemoveAt(0);
            Assert.AreEqual(coll[valueArray.Length], item);
            Assert.AreEqual(valueArray.Length + 1, coll.Count);
            for (int i = 0; i < valueArray.Length; ++i)
                Assert.AreEqual(valueArray[i], coll[i]);

            // Delete at the end.
            coll.RemoveAt(valueArray.Length);
            Assert.AreEqual(valueArray.Length, coll.Count);
            for (int i = 0; i < valueArray.Length; ++i)
                Assert.AreEqual(valueArray[i], coll[i]);

            // Insert at the middle.
            coll.Insert(valueArray.Length / 2, item);
            Assert.AreEqual(valueArray.Length + 1, coll.Count);
            Assert.AreEqual(item, coll[valueArray.Length / 2]);
            for (int i = 0; i < valueArray.Length; ++i) {
                if (i < valueArray.Length / 2)
                    Assert.AreEqual(valueArray[i], coll[i]);
                else
                    Assert.AreEqual(valueArray[i], coll[i+1]);
            }

            // Delete at the middle.
            coll.RemoveAt(valueArray.Length / 2);
            Assert.AreEqual(valueArray.Length, coll.Count);
            for (int i = 0; i < valueArray.Length; ++i)
                Assert.AreEqual(valueArray[i], coll[i]);

            // Delete all from the middle.
            for (int i = 0; i < valueArray.Length; ++i)
                coll.RemoveAt(coll.Count / 2);
            Assert.AreEqual(0, coll.Count);

            // Build up in order.
            for (int i = 0; i < save.Length; ++i) {
                coll.Insert(i, save[i]);
            }

            TestList(coll, valueArray);     // Basic read-only list stuff.

            coll.Clear();
            Assert.AreEqual(0, coll.Count);

            // Build up in order with Add
            for (int i = 0; i < save.Length; ++i) {
                coll.Add(save[i]);
            }

            TestList(coll, valueArray);     // Basic read-only list stuff.

            // Remove in order with Remove.
            for (int i = 0; i < valueArray.Length; ++i) {
                coll.Remove(valueArray[i]);
            }

            Assert.AreEqual(0, coll.Count);

            // Build up in reverse order with Insert
            for (int i = 0; i < save.Length; ++i) {
                coll.Insert(0, save[save.Length - 1 - i]);
            }
            TestList(coll, valueArray);     // Basic read-only list stuff.

            // Check read-write collection stuff.
            TestCollection(coll, valueArray, true);
        }

        /// <summary>
        /// Test read-only IList&lt;T&gt; that should contain the given values, possibly in order. Does not change
        /// the list. Forces the list to be read-only.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll">IList&lt;T&gt; to test. </param>
        /// <param name="valueArray">The values that should be in the list.</param>
        /// <param name="name">Name of the collection, for exceptions. Null to not check.</param>
        public static void TestReadOnlyListGeneric<T>(IList<T> coll, T[] valueArray, string name)
        {
            TestReadOnlyListGeneric(coll, valueArray, name, null);
        }

        public static void TestReadOnlyListGeneric<T>(IList<T> coll, T[] valueArray, string name, BinaryPredicate<T> equals)
        {
            if (equals == null)
                equals = ObjectEquals;

            // Basic list stuff.
            TestListGeneric(coll, valueArray, equals);
            TestReadonlyCollectionGeneric(coll, valueArray, true, name, equals);

            // Check read only and fixed size bits.
            Assert.IsTrue(coll.IsReadOnly);

            // Check exceptions.
            if (coll.Count > 0) {
                Assert.ThrowsException<NotSupportedException>(() => coll.Clear());
            }

            Assert.ThrowsException<NotSupportedException>(() => coll.Insert(0, default(T)));

            if (coll.Count > 0) {
                Assert.ThrowsException<NotSupportedException>(() => coll.RemoveAt(0));

                Assert.ThrowsException<NotSupportedException>(() => coll[0] = default(T));
            }
        }

        /// <summary>
        /// Test read-only non-generic IList; that should contain the given values, possibly in order. Does not change
        /// the list. Forces the list to be read-only.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll">IList to test. </param>
        /// <param name="valueArray">The values that should be in the list.</param>
        /// <param name="name">Name of the collection, for exceptions. Null to not check.</param>
        public static void TestReadOnlyList<T>(IList coll, T[] valueArray, string name)
        {
            // Basic list stuff.
            TestList(coll, valueArray);
            TestCollection(coll, valueArray, true);

            // Check read only and fixed size bits.
            Assert.IsTrue(coll.IsReadOnly);
            Assert.IsTrue(coll.IsFixedSize);

            // Check exceptions.
            Assert.ThrowsException<NotSupportedException>(() => coll.Clear());

            // Check exceptions.
            Assert.ThrowsException<NotSupportedException>(() => coll.Clear());

            Assert.ThrowsException<NotSupportedException>(() => coll.Insert(0, default(T)));

            Assert.ThrowsException<NotSupportedException>(() => coll.Add(default(T)));

            if (coll.Count > 0) {
                Assert.ThrowsException<NotSupportedException>(() => coll.RemoveAt(0));

                Assert.ThrowsException<NotSupportedException>(() => coll.Remove(coll[0]));

                Assert.ThrowsException<NotSupportedException>(() => coll[0] = default(T));
            }
        }

        public static void TestReadWriteMultiDictionaryGeneric<TKey, TValue>(IDictionary<TKey, ICollection<TValue>> dict, TKey[] keys, TValue[][] values, TKey nonKey, TValue nonValue, bool mustBeInOrder, string name,
            BinaryPredicate<TKey> keyEquals, BinaryPredicate<TValue> valueEquals)
        {
            if (keyEquals == null)
                keyEquals = ObjectEquals;
            if (valueEquals == null)
                valueEquals = ObjectEquals;
            BinaryPredicate<ICollection<TValue>> valueCollectionEquals = CollectionEquals(valueEquals, mustBeInOrder);

            TestReadWriteDictionaryGeneric(dict, keys, values, nonKey, mustBeInOrder, name, keyEquals, valueCollectionEquals);
        }

        public static void TestReadOnlyMultiDictionaryGeneric<TKey, TValue>(IDictionary<TKey, ICollection<TValue>> dict, TKey[] keys, TValue[][] values, TKey nonKey, TValue nonValue, bool mustBeInOrder, string name,
            BinaryPredicate<TKey> keyEquals, BinaryPredicate<TValue> valueEquals)
        {
            if (keyEquals == null)
                keyEquals = ObjectEquals;
            if (valueEquals == null)
                valueEquals = ObjectEquals;
            BinaryPredicate<ICollection<TValue>> valueCollectionEquals = CollectionEquals(valueEquals, mustBeInOrder);

            TestReadOnlyDictionaryGeneric(dict, keys, values, nonKey, mustBeInOrder, name, keyEquals, valueCollectionEquals);
        }

        public static void TestMultiDictionaryGeneric<TKey, TValue>(IDictionary<TKey, ICollection<TValue>> dict, TKey[] keys, TValue[][] values, TKey nonKey, TValue nonValue, bool mustBeInOrder,
            BinaryPredicate<TKey> keyEquals, BinaryPredicate<TValue> valueEquals)
        {
            if (keyEquals == null)
                keyEquals = ObjectEquals;
            if (valueEquals == null)
                valueEquals = ObjectEquals;
            BinaryPredicate<ICollection<TValue>> valueCollectionEquals = CollectionEquals(valueEquals, mustBeInOrder);

            TestDictionaryGeneric(dict, keys, values, nonKey, mustBeInOrder, keyEquals, valueCollectionEquals);
        }

        /// <summary>
        /// This class has Equal and GetHashCode semantics for identity semantics.
        /// </summary>
        [Serializable]
        public class Unique
        {
            public string val;

            public Unique(string v)
            {
                val = v;
            }

            public override string ToString()
            {
                return val;
            }

            static public bool EqualValues(Unique x, Unique y)
            {
                if (x == null || y == null)
                    return x == y;
                else
                    return string.Equals(x.val, y.val);
            }
        }

        /// <summary>
        /// Round-trip serialize and deserialize an object.
        /// </summary>
        /// <param name="objToSerialize">Object to serialize</param>
        /// <returns>Result of the serialization.</returns>
        public static object SerializeRoundTrip(object objToSerialize)
        {
            object result;

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream("TestSerialization.bin", FileMode.Create, FileAccess.Write, FileShare.None)) {
                formatter.Serialize(stream, objToSerialize);
            }

            formatter = new BinaryFormatter();
            using (Stream stream = new FileStream("TestSerialization.bin", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                result = formatter.Deserialize(stream);
            }

            return result;
        }
    }
}

