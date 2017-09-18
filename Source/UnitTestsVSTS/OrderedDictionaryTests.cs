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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Wintellect.PowerCollections.Tests.TestHelpers;
using static Wintellect.PowerCollections.Tests.UtilTests;

namespace Wintellect.PowerCollections.Tests
{
	/// <summary>
	/// Tests for testing the OrderedDictionary class.
	/// </summary>
	[TestClass]
	public class OrderedDictionaryTests
	{
		const int LENGTH = 1200;			// length of each random array of values.
		const int ITERATIONS = 80;		// number of iterations
		private OrderedDictionary<int,string> dict;

		/// <summary>
		/// Create a string from an integer
		/// </summary>
		/// <param name="i">The int.</param>
		/// <returns>The string made from the int.</returns>
		private string StringFromInt(int i)
		{
			return string.Format("e{0}", i);
		}

		/// <summary>
		/// Create a random array of values.
		/// </summary>
		/// <param name="seed">Seed for random number generators</param>
		/// <param name="length">Length of array</param>
		/// <param name="max">Maximum value of number. Should be much 
		/// greater than length.</param>
		/// <param name="allowDups">Whether to allow duplicate elements.</param>
		/// <returns></returns>
		private int[] CreateRandomArray(int seed, int length, int max, bool allowDups)
		{
			var rand = new Random(seed);
			int[] a = new int[length];

			for (int i = 0; i < a.Length; ++i)
				a[i] = -1;

			for (int el = 0; el < a.Length; ++el)
			{
				int value;

				do
				{
					value = rand.Next(max);
				} while (!allowDups && Array.IndexOf(a, value) >= 0);

				a[el] = value;
			}

			return a;
		}
		/// <summary>
		/// Insert all the elements of an integer array into the dictionary. The
		/// values in the dictionary are the indexes of the array.
		/// </summary>
		/// <param name="a">Array of values to insert.</param>
		private void InsertArray(int[] a)
		{
			for (int i = 0; i < a.Length; ++i)
			{
				string s = StringFromInt(i);

				dict.Add(a[i], s);
			}
		}

		/// <summary>
		/// Iterate the dictionary, making sure that everything is in order, the values
		/// match, and that all the keys in the array are found.
		/// </summary>
		/// <param name="a"></param>
		private void CheckArray(int[] a)
		{
			int count = 0;
			int lastKey = -1;

			foreach (KeyValuePair<int,string> pair in dict)
			{
				Assert.IsTrue(lastKey < pair.Key, "Keys are not in order");

				int index = Array.IndexOf(a, pair.Key);

				Assert.IsTrue(index >= 0, "key wasn't found in the array");
				Assert.AreEqual(StringFromInt(index), pair.Value);
				a[index] = -1;

				++count;
                lastKey = pair.Key;
			}

			Assert.AreEqual(count, dict.Count, "Number of keys found is not correct");

			// All the items should have been knocked out.
			foreach (int x in a)
			{
				Assert.AreEqual(-1, x);
			}
		}

		/// <summary>
		/// Insert a bunch of entries into the tree, make sure that the count is correct, 
		/// and that they iterate correct in order.
		/// </summary>
		[TestMethod]
		public void RandomAdd()
		{
			for (int iter = 0; iter < ITERATIONS; ++iter)
			{
				// Insert the array into the dictionary.
				dict = new OrderedDictionary<int,string>();

				int[] a = CreateRandomArray(iter + 1, LENGTH, LENGTH * 10, false);

				InsertArray(a);

				// Make sure the count is correct.
				Assert.AreEqual(LENGTH, dict.Count);
				CheckArray(a);
			}
		}

	    /// <summary>
        /// Test adding keys/values to the collection. Make sure the return value is right, and that
        /// the keys are present in the collection afterward.
        /// </summary>
        [TestMethod]
        public void Add()
        {
            var dict1 = new OrderedDictionary<double, int> {
                {4.67, 12},
                {double.NaN, -17},
                {double.PositiveInfinity, 0}
            };

            try {
                dict1.Add(4.67, 187); Assert.Fail("Exception should be thrown");
            }
            catch (Exception e) {
                Assert.IsTrue(e is ArgumentException);
            }
            dict1.Add(double.NegativeInfinity, 188921);
            try {
                dict1.Add(double.NegativeInfinity, 421); Assert.Fail("Exception should be thrown");
            }
            catch (Exception e) {
                Assert.IsTrue(e is ArgumentException);
            }
            try {
                dict1.Add(4.67, 222); Assert.Fail("Exception should be thrown");
            }
            catch (Exception e) {
                Assert.IsTrue(e is ArgumentException);
            }

            dict1.Add(double.MaxValue, 444);

            Assert.AreEqual(5, dict1.Count);

            Assert.AreEqual(12, dict1[4.67]);
            Assert.AreEqual(188921, dict1[double.NegativeInfinity]);
            Assert.AreEqual(0, dict1[double.PositiveInfinity]);
            Assert.AreEqual(-17, dict1[double.NaN]);
            Assert.AreEqual(444, dict1[double.MaxValue]);

            InterfaceTests.TestReadonlyCollectionGeneric(dict1.Keys, new double[] { double.NaN, double.NegativeInfinity, 4.67, double.MaxValue, double.PositiveInfinity }, true, "KeysCollection");
            InterfaceTests.TestReadonlyCollectionGeneric(dict1.Values, new int[] { -17, 188921, 12, 444, 0 }, true, "ValuesCollection");
        }

#if false
        /// <summary>
		/// Test adding keys/values to the collection. Make sure the return value is right, and that
		/// the keys are present in the collection afterward.
		/// </summary>
		[TestMethod]
		public void AddOrUpdate()
		{
			OrderedDictionary<double,int> dict1 = new OrderedDictionary<double,int>();
			bool b;

			b = dict1.AddOrUpdate(4.67, 12);
			Assert.IsFalse(b);
			b = dict1.AddOrUpdate(double.NaN, -17);
			Assert.IsFalse(b);
			b = dict1.AddOrUpdate(double.PositiveInfinity, 0);
			Assert.IsFalse(b);
			b = dict1.AddOrUpdate(4.67, 187);
			Assert.IsTrue(b);
			b = dict1.AddOrUpdate(double.NegativeInfinity, 188921);
			Assert.IsFalse(b);
			b = dict1.AddOrUpdate(double.NegativeInfinity, 421);
			Assert.IsTrue(b);
			b = dict1.AddOrUpdate(4.67, 222);
			Assert.IsTrue(b);
			b = dict1.AddOrUpdate(double.MaxValue, 444);
			Assert.IsFalse(b);

			Assert.AreEqual(5, dict1.Count);

			Assert.AreEqual(222, dict1[4.67]);
			Assert.AreEqual(421, dict1[double.NegativeInfinity]);
			Assert.AreEqual(0, dict1[double.PositiveInfinity]);
			Assert.AreEqual(-17, dict1[double.NaN]);
			Assert.AreEqual(444, dict1[double.MaxValue]);

			TestUtil.TestReadonlyCollectionGeneric(dict1.Keys, new double[] { double.NaN, double.NegativeInfinity, 4.67, double.MaxValue, double.PositiveInfinity }, true, "KeysCollection");
			TestUtil.TestReadonlyCollectionGeneric(dict1.Values, new int[] { -17, 421, 222, 444, 0 }, true, "ValuesCollection");
		}
#endif

        /// <summary>
        /// Test updating. 
        /// </summary>
        [TestMethod]
        public void Update()
        {
            var dict1 = new OrderedDictionary<double, int> {
                { 4.67, 12 },
                { double.NaN, -17 },
                { double.PositiveInfinity, 0 },
                { double.MaxValue, 441 }
            };
            dict1.Replace(4.67, -89);
            try {
                dict1.Replace(double.NegativeInfinity, 187); Assert.Fail("Exception should be thrown");
            }
            catch (Exception e) {
                Assert.IsTrue(e is KeyNotFoundException);
            }
            dict1.Add(double.NegativeInfinity, 188921);
            dict1.Replace(double.MaxValue, -1);
            dict1.Replace(double.NaN, 188);
            try {
                dict1.Replace(12, 3); Assert.Fail("Exception should be thrown");
            }
            catch (Exception e) {
                Assert.IsTrue(e is KeyNotFoundException);
            }

            dict1.Replace(double.MaxValue, 33);

            Assert.AreEqual(5, dict1.Count);

            Assert.AreEqual(-89, dict1[4.67]);
            Assert.AreEqual(188921, dict1[double.NegativeInfinity]);
            Assert.AreEqual(0, dict1[double.PositiveInfinity]);
            Assert.AreEqual(188, dict1[double.NaN]);
            Assert.AreEqual(33, dict1[double.MaxValue]);
            Assert.IsFalse(dict1.ContainsKey(12));

            InterfaceTests.TestReadonlyCollectionGeneric(dict1.Keys, new double[] { double.NaN, double.NegativeInfinity, 4.67, double.MaxValue, double.PositiveInfinity }, true, "KeysCollection");
            InterfaceTests.TestReadonlyCollectionGeneric(dict1.Values, new int[] { 188, 188921, -89, 33, 0 }, true, "ValuesCollection");
        }

        /// <summary>
        /// Test adding keys/values to the collection. Like the Add() test, but uses the indexer dict operation.
		/// Make sure the return value is right, and that
		/// the keys are present in the collection afterward.
		/// </summary>
		[TestMethod]
		public void IndexerSet()
		{
            var dict1 = new OrderedDictionary<double, int> {
                [4.67] = 12,
                [double.NaN] = -17,
                [double.PositiveInfinity] = 0,
                [4.67] = 187,
                [double.NegativeInfinity] = 188921,
                [double.NegativeInfinity] = 421,
                [4.67] = 222,
                [double.MaxValue] = 444
            };

            Assert.AreEqual(5, dict1.Count);
			Assert.AreEqual(222, dict1[4.67]);
			Assert.AreEqual(421, dict1[double.NegativeInfinity]);
			Assert.AreEqual(0, dict1[double.PositiveInfinity]);
			Assert.AreEqual(-17, dict1[double.NaN]);
			Assert.AreEqual(444, dict1[double.MaxValue]);
			InterfaceTests.TestReadonlyCollectionGeneric(dict1.Keys, new double[] { double.NaN, double.NegativeInfinity, 4.67, double.MaxValue, double.PositiveInfinity }, true, "KeysCollection");
			InterfaceTests.TestReadonlyCollectionGeneric(dict1.Values, new int[] { -17, 421, 222, 444, 0 }, true, "ValuesCollection");
		}

        [TestMethod]
        public void IndexerGet()
        {
            var dict1 = new OrderedDictionary<string, int>(StringComparer.InvariantCultureIgnoreCase) {
                ["foo"] = 12,
                [null] = 18,
                ["BAR"] = 11
            };

            Assert.AreEqual(12, dict1["Foo"]);
            Assert.AreEqual(18, dict1[null]);
            Assert.AreEqual(11, dict1["bar"]);

            try {
                int unused = dict1["foobar"];
                Assert.Fail("should throw");
            }
            catch (Exception e) {
                Assert.IsTrue(e is KeyNotFoundException);
            }
        }

		/// <summary>
		/// Test removing items from the tree.
		///</summary>
		[TestMethod]
		public void Remove()
		{
			var dict1 = new OrderedDictionary<double,int>();
			bool b;

			dict1[4.67] =  12;
			dict1[double.NaN] =  -17;
			dict1[double.PositiveInfinity] =  0;
			dict1[4.67] =  187;
			dict1[double.NegativeInfinity] =  188921;
			dict1[double.NegativeInfinity] =  421;
			dict1[4.67] =  222;
			dict1[double.MaxValue] =  444;

			b = dict1.Remove(double.NaN);
			Assert.IsTrue(b);
			b = dict1.Remove(double.NaN);
			Assert.IsFalse(b);
			b = dict1.Remove(double.MinValue);
			Assert.IsFalse(b);
			b = dict1.Remove(4.67);
			Assert.IsTrue(b);
			b = dict1.Remove(4.67);
			Assert.IsFalse(b);
			
			InterfaceTests.TestReadonlyCollectionGeneric(dict1.Keys, new double[] { double.NegativeInfinity, double.MaxValue, double.PositiveInfinity }, true, "KeysCollection");
			InterfaceTests.TestReadonlyCollectionGeneric(dict1.Values, new int[] { 421, 444, 0 }, true, "ValuesCollection");
		}

        [TestMethod]
        public void TryGetValue()
        {
            var dict1 = new OrderedDictionary<double, int>();
            bool b;
            int val;

            b = dict1.TryGetValue(4.67, out val);
            Assert.IsFalse(b);
            dict1[4.67] =  12;
            b = dict1.TryGetValue(4.67, out val);
            Assert.IsTrue(b); Assert.AreEqual(val, 12);
            dict1[double.NaN] =  -17;
            dict1[double.PositiveInfinity] =  0;
            b = dict1.TryGetValue(12.3, out val);
            Assert.IsFalse(b);
            dict1[4.67] =  187;
            dict1[double.NegativeInfinity] =  188921;
            b = dict1.TryGetValue(double.NegativeInfinity, out val);
            Assert.IsTrue(b); Assert.AreEqual(val, 188921);
            dict1[double.NegativeInfinity] =  421;
            dict1[4.67] =  222;
            b = dict1.TryGetValue(double.MaxValue, out val);
            Assert.IsFalse(b);
            dict1[double.MaxValue] =  444;
            b = dict1.TryGetValue(double.NaN, out val);
            Assert.IsTrue(b); Assert.AreEqual(val, -17);
            b = dict1.TryGetValue(4.67, out val);
            Assert.IsTrue(b); Assert.AreEqual(val, 222);
            b = dict1.TryGetValue(double.NegativeInfinity, out val);
            Assert.IsTrue(b); Assert.AreEqual(val, 421);
        }

        [TestMethod]
        public void GetValueElseAdd()
        {
            var dict1 = new OrderedDictionary<double, int>();
            bool b; 
            int val;

            val = 12;
            b = dict1.GetValueElseAdd(4.67, ref val);
            Assert.IsFalse(b); Assert.AreEqual(12, val);

            val = -17;
            b = dict1.GetValueElseAdd(double.NaN, ref val);
            Assert.IsFalse(b); Assert.AreEqual(-17, val);

            val = 0;
            b = dict1.GetValueElseAdd(double.PositiveInfinity, ref val);
            Assert.IsFalse(b); Assert.AreEqual(0, val);

            val = 187;
            b = dict1.GetValueElseAdd(4.67, ref val);
            Assert.IsTrue(b); Assert.AreEqual(12, val);

            val = 188921;
            b = dict1.GetValueElseAdd(double.NegativeInfinity, ref val);
            Assert.IsFalse(b); Assert.AreEqual(188921, val);

            Assert.AreEqual(12, dict1[4.67]);
            dict1.Replace(4.67, 999);

            val = 121;
            b = dict1.GetValueElseAdd(4.67, ref val);
            Assert.IsTrue(b); Assert.AreEqual(999, val);

            val = 421;
            b = dict1.GetValueElseAdd(double.NegativeInfinity, ref val);
            Assert.IsTrue(b); Assert.AreEqual(188921, val);

            Assert.AreEqual(188921, dict1[double.NegativeInfinity]);
            Assert.AreEqual(999, dict1[4.67]);
            Assert.AreEqual(0, dict1[double.PositiveInfinity]);
            Assert.AreEqual(-17, dict1[double.NaN]);
        }

        /// <summary>
		/// Test clearing the dictionary.
		/// </summary>
		[TestMethod]
		public void Clear()
		{
			var dict1 = new OrderedDictionary<string,double>();

			Assert.AreEqual(0, dict1.Count);

			dict1["hello"] = 4.5;
			dict1["hi"] = 1.22;
			dict1[""] = 7.6;

			Assert.AreEqual(3, dict1.Count);

			dict1.Clear();

			Assert.AreEqual(0, dict1.Count);
			InterfaceTests.TestReadonlyCollectionGeneric(dict1.Keys, new string[0], true, "KeysCollection");
			InterfaceTests.TestReadonlyCollectionGeneric(dict1.Values, new double[0], true, "ValuesCollection");

			Assert.IsFalse(dict1.ContainsKey("hello"));
			Assert.IsFalse(dict1.ContainsKey("hi"));
			Assert.IsFalse(dict1.ContainsKey("banana"));
			Assert.IsFalse(dict1.ContainsKey(""));
		}

		/// <summary>
		/// Test ContainsKey. Make sure it returns the right valuesl. 
		/// </summary>
		[TestMethod]
		public void ContainsKey()
		{
            var dict1 = new OrderedDictionary<string, string> {
                ["b"] = "foo",
                ["r"] = "bar",
                [""] = "golde",
                ["n"] = "baz",
                ["B"] = "hello",
                [""] = "peter",
                ["n"] = "horton",
                ["x"] = "hears"
            };
            dict1.Remove("n");
			dict1["c"] =  "a who";
			dict1["q"] =  null;

			Assert.IsTrue(dict1.ContainsKey("b"));
			Assert.IsTrue(dict1.ContainsKey("r"));
			Assert.IsTrue(dict1.ContainsKey(""));
			Assert.IsFalse(dict1.ContainsKey("n"));
			Assert.IsTrue(dict1.ContainsKey("B"));
			Assert.IsTrue(dict1.ContainsKey("x"));
			Assert.IsTrue(dict1.ContainsKey("q"));
			Assert.IsFalse(dict1.ContainsKey("a"));

			dict1.Remove("");

			Assert.IsFalse(dict1.ContainsKey(""));
		}

		class ComparableClass1: IComparable<ComparableClass1>
		{
			public int Value = 0;
			int IComparable<ComparableClass1>.CompareTo(ComparableClass1 other)
			{
				if (Value > other.Value)
					return 1;
				else if (Value < other.Value)
					return -1;
				else
					return 0;
			}
        }

		class ComparableClass2: IComparable
		{
			public int Value = 0;
			int IComparable.CompareTo(object other)
			{
				if (other is ComparableClass2)
				{
					var o = (ComparableClass2)other;

					if (Value > o.Value)
						return 1;
					else if (Value < o.Value)
						return -1;
					else
						return 0;
				}
				else 
					throw new ArgumentException(Strings.ArgOfWrongType, nameof(other));
			}
		}

		// Not comparable, because the type parameter on ComparableClass is incorrect.
		public class UncomparableClass1: IComparable<ComparableClass1>
		{
			public int Value = 0;
			int IComparable<ComparableClass1>.CompareTo(ComparableClass1 other)
			{
				if (Value > other.Value)
					return 1;
				else if (Value < other.Value)
					return -1;
				else
					return 0;
			}
        }

	    /// <summary>
		/// Make sure that the parameterless constructor on SimpleDictionary can be called with
		/// a comparable struct and class.
		/// </summary>
		[TestMethod]
		public void SimpleConstruction()
		{
			var unused = new OrderedDictionary<int,string>();
			var unused1 = new OrderedDictionary<string,string>();
			var unused2 = new OrderedDictionary<ComparableClass2,string>();
			var unused3 = new OrderedDictionary<ComparableClass1,string>();
		}

	    /// <summary>
	    /// Check that a OrderedDictionary can't be instantiated on an incomparable type.
	    /// </summary>
	    [TestMethod]
	    public void NotComparable1()
        {
            ThrowsInvalidResult( () => new OrderedDictionary<UncomparableClass1, string>() );
	    }

	    /// <summary>
	    /// Check that a OrderedDictionary can't be instantiated on an incomparable type.
	    /// </summary>
	    [TestMethod]
	    public void NotComparable2()
        {
            ThrowsInvalidResult( () => new OrderedDictionary<UncomparableClass2, string>() );
	    }

        /// <summary>
        /// Check that IsReadOnly always returns false.
        /// </summary>
        [TestMethod]
		public void IsReadOnly()
		{
			var dict1 = new OrderedDictionary<int,string>();

			Assert.IsFalse(((IDictionary)dict1).IsReadOnly, "IsReadOnly should be false");
			Assert.IsFalse(((IDictionary<int,string>)dict1).IsReadOnly, "IsReadOnly should be false");
		}

		/// <summary>
		/// Check that IsFixedSize always returns false.
		/// </summary>
		[TestMethod]
		public void IsFixedSize()
		{
			var dict1 = new OrderedDictionary<int,string>();

			Assert.IsFalse(((IDictionary)dict1).IsFixedSize, "IsFixedSize should be false");
		}

		/// <summary>
		/// Check the Keys and Values collections.
		/// </summary>
		[TestMethod]
		public void KeysValuesCollections()
		{
            var dict1 = new OrderedDictionary<string, int> {
                { "q", 17 },
                { "a", 143 },
                { "r", -5 },
                { "z", 0 },
                { "x", 12 },
                { "m", 17 }
            };

            ICollection keysCollection = ((IDictionary)dict1).Keys;
			ICollection<string> keysGenCollection = dict1.Keys;
			ICollection valuesCollection = ((IDictionary)dict1).Values;
			ICollection<int> valuesGenCollection = dict1.Values;

			InterfaceTests.TestCollection(keysCollection, new string[] { "a", "m", "q", "r", "x", "z" }, true);
            InterfaceTests.TestReadonlyCollectionGeneric(keysGenCollection, new string[] { "a", "m", "q", "r", "x", "z" }, true, "KeysCollection");
            InterfaceTests.TestCollection(valuesCollection, new int[] { 143, 17, 17, -5, 12, 0 }, true);
            InterfaceTests.TestReadonlyCollectionGeneric(valuesGenCollection, new int[] { 143, 17, 17, -5, 12, 0 }, true, "ValuesCollection");
        }

		/// <summary>
		/// Check that null keys and values work correctly.
		/// </summary>
		[TestMethod]
		public void NullKeysValues()
		{
			var dict1 = new OrderedDictionary<string,string>();

			Assert.IsFalse(dict1.ContainsKey(null));

			dict1.Add("q",null);
			dict1.Add("a", "hello");
			dict1.Add(null, "goodbye");

			ICollection keysCollection = ((IDictionary)dict1).Keys;
			ICollection<string> keysGenCollection = dict1.Keys;
			ICollection valuesCollection = ((IDictionary)dict1).Values;
			ICollection<string> valuesGenCollection = dict1.Values;

			InterfaceTests.TestCollection(keysCollection, new string[] { null, "a", "q" }, true);
            InterfaceTests.TestReadonlyCollectionGeneric(keysGenCollection, new string[] { null, "a", "q" }, true, "KeysCollection");
            InterfaceTests.TestCollection(valuesCollection, new string[] { "goodbye", "hello", null }, true);
            InterfaceTests.TestReadonlyCollectionGeneric(valuesGenCollection, new string[] { "goodbye", "hello", null }, true, "ValuesCollection");

            Assert.IsNull(dict1["q"]);
			Assert.AreEqual("goodbye", dict1[null]);
			Assert.IsTrue(dict1.ContainsKey(null));

			// Enumerate key/values directly. The pair with a null key will be enumerated.
			int i = 0;
			foreach (KeyValuePair<string,string> pair in dict1)
			{
                if (i == 0) {
                    Assert.AreEqual(null, pair.Key);
                    Assert.AreEqual("goodbye", pair.Value);
                }
                else if (i == 1) {
					Assert.AreEqual("a", pair.Key);
					Assert.AreEqual("hello", pair.Value);
				}
				else if (i == 2)
				{
					Assert.AreEqual("q", pair.Key);
					Assert.AreEqual(null, pair.Value);
				}
				else
				{
					Assert.Fail("should only enumerate two items");
				}

				++i;
			}

			dict1.Remove(null);
			Assert.IsFalse(dict1.ContainsKey(null));
		}	

		/// <summary>
		/// Check that enumerators are enumerating the correct keys and values.
		/// </summary>
		/// <param name="inorder">An IEnumerable enumerating in order</param>
		/// <param name="reversed">An IEnumerable enumerating reversed</param>
		/// <param name="keys">Expected keys in order</param>
		/// <param name="values">Expected values in order</param>
		void CheckEnumeration<TKey,TValue> (IEnumerable<KeyValuePair<TKey,TValue>> inorder, IEnumerable<KeyValuePair<TKey,TValue>> reversed,
																				TKey[] keys, TValue[] values)
		{
			int i = 0;

			foreach (KeyValuePair<TKey,TValue> pair in inorder)
			{
				Assert.AreEqual(keys[i], pair.Key);
				Assert.AreEqual(values[i], pair.Value);
				++i;
			}
            Assert.AreEqual(i, keys.Length);

            i = 0;
			foreach (KeyValuePair<TKey,TValue> pair in reversed)
			{
				Assert.AreEqual(keys[keys.Length - i - 1], pair.Key);
				Assert.AreEqual(values[values.Length - i - 1], pair.Value);
				++i;
			}
            Assert.AreEqual(i, keys.Length);
        }

        [TestMethod]
		public void Enumerate()
		{
            var dict1 = new OrderedDictionary<string, int> {
                ["foo"] = 23,
                ["a"] = 11,
                ["b"] = 119,
                [""] = 981,
                ["r4"] = 9,
                ["b"] = 7,
                ["hello"] = 198,
                ["q"] = 199,
                ["ww"] = -8,
                ["ww"] = -9,
                ["p"] = 1234
            };

            CheckEnumeration(dict1, dict1.Reversed(),
					new string[] {"", "a", "b", "foo", "hello", "p", "q", "r4", "ww" },
			        new int[] {981, 11, 7, 23, 198, 1234, 199, 9, -9});
		}

		/// <summary>
		/// Test that cloning works.
		/// </summary>
		[TestMethod]
		public void Clone()
		{
			var dict1 = new OrderedDictionary<string,int>();
			OrderedDictionary<string,int> dict2;

			dict1["foo"] = 23;
			dict1["a"] = 11;
			dict1["b"] = 119;
			dict1[""] = 981;
			dict1["r4"] = 9;
			dict1["b"] = 7;
			dict1["hello"] = 198;
			dict1["q"] = 199;
			dict1["ww"] = -8;
			dict1["ww"] = -9;
			dict1["p"] = 1234;

			dict2 = dict1.Clone();

            Assert.IsFalse(dict2 == dict1);

			// Modify dict1, make sure dict2 doesn't change.
			dict1.Remove("a");
			dict1.Remove("b");
			dict1.Remove("");
			dict1["qqq"] = 1;

			CheckEnumeration(dict2, dict2.Reversed(), new string[] { "", "a", "b", "foo", "hello", "p", "q", "r4", "ww" }, new int[] { 981, 11, 7, 23, 198, 1234, 199, 9, -9 });
			Assert.AreEqual(981, dict2[""]);

            var dict4 = new OrderedDictionary<string, int>();
            OrderedDictionary<string, int> dict5;
            dict5 = dict4.Clone();
            Assert.IsFalse(dict4 == dict5);
            Assert.IsTrue(dict4.Count == 0 && dict5.Count == 0);
            dict4.Add("hello", 1);
            Assert.IsTrue(dict4.Count == 1 && dict5.Count == 0);
            dict5.Add("hi", 7);
            dict4.Clear();
            Assert.IsTrue(dict4.Count == 0 && dict5.Count == 1);
        }

        // Simple class for testing cloning.
        public class MyInt : ICloneable
        {
            public int value;
            public MyInt(int value)
            {
                this.value = value;
            }

            public object Clone()
            {
                return new MyInt(value);
            }

            public override bool Equals(object obj)
            {
                return (obj is MyInt && ((MyInt)obj).value == value);
            }

            public override int GetHashCode()
            {
                return value.GetHashCode();
            }

            public override string ToString()
            {
                return value.ToString();
            }



        }

        void CompareClones<K, V>(OrderedDictionary<K, V> d1, OrderedDictionary<K, V> d2)
        {
            IEnumerator<KeyValuePair<K, V>> e1 = d1.GetEnumerator();
            IEnumerator<KeyValuePair<K, V>> e2 = d2.GetEnumerator();

            // Check that the dictionaries are equal, but not reference equals (e.g., have been cloned).
            while (e1.MoveNext()) {
                e2.MoveNext();
                if (e1.Current.Key == null)
                    Assert.IsNull(e2.Current.Key);
                else {
                    Assert.IsTrue(e1.Current.Key.Equals(e2.Current.Key));
                    Assert.IsFalse(object.ReferenceEquals(e1.Current.Key, e2.Current.Key));
                }

                if (e1.Current.Value == null)
                    Assert.IsNull(e2.Current.Value);
                else {
                    Assert.IsTrue(e1.Current.Value.Equals(e2.Current.Value));
                    Assert.IsFalse(object.ReferenceEquals(e1.Current.Value, e2.Current.Value));
                }
            }
        }

        [TestMethod]
        public void CloneContents()
        {
            var dict1 = new OrderedDictionary<int, MyInt> {
                [4] = new MyInt(143),
                [7] = new MyInt(2),
                [11] = new MyInt(9),
                [18] = null,
                [3] = new MyInt(14),
                [1] = new MyInt(111)
            };
            OrderedDictionary<int, MyInt> dict2 = dict1.CloneContents();
            CompareClones(dict1, dict2);

            var dict3 = new OrderedDictionary<MyInt, int>(
                (v1, v2) => v2.value.CompareTo(v1.value)) {
                [new MyInt(7)] = 144,
                [new MyInt(16)] = 13,
                [new MyInt(-6)] = -14,
                [new MyInt(0)] = 31415,
                [new MyInt(1111)] = 0
            };

            OrderedDictionary<MyInt, int> dict4 = dict3.CloneContents();
            CompareClones(dict3, dict4);

            int Comparison(CloneableStruct s1, CloneableStruct s2) {
                return s1.value.CompareTo(s2.value);
            }

            var dict5 = new OrderedDictionary<CloneableStruct, CloneableStruct>(Comparison) {
                [new CloneableStruct(7)] = new CloneableStruct(144),
                [new CloneableStruct(16)] = new CloneableStruct(13),
                [new CloneableStruct(-6)] = new CloneableStruct(-14),
                [new CloneableStruct(0)] = new CloneableStruct(31415),
                [new CloneableStruct(1111)] = new CloneableStruct(0)
            };
            OrderedDictionary<CloneableStruct,CloneableStruct> dict6 = dict5.CloneContents();

            IEnumerator<KeyValuePair<CloneableStruct, CloneableStruct>> e1 = dict5.GetEnumerator();
            IEnumerator<KeyValuePair<CloneableStruct, CloneableStruct>> e2 = dict6.GetEnumerator();

            Assert.IsTrue(dict5.Count == dict6.Count);

            // Check that the dictionaries are equal, but not identical (e.g., have been cloned).
            while (e1.MoveNext()) {
                e2.MoveNext();
                Assert.IsTrue(e1.Current.Key.Equals(e2.Current.Key));
                Assert.IsFalse(e1.Current.Key.Identical(e2.Current.Key));

                Assert.IsTrue(e1.Current.Value.Equals(e2.Current.Value));
                Assert.IsFalse(e1.Current.Value.Identical(e2.Current.Value));
            }
        }

	    [TestMethod]
	    public void CantCloneContents() {
	        var dict1 = new OrderedDictionary<int, NotCloneable> {
	            [4] = new NotCloneable(),
	            [5] = new NotCloneable()
	        };

	        ThrowsInvalid(() => dict1.CloneContents());
	    }

        // Check that a View has the correct keys and values in it.
        private void CheckView<TKey, TValue>(OrderedDictionary<TKey,TValue>.View view, TKey[] keys, TValue[] values, TKey nonKey)
        {
            CheckEnumeration(view, view.Reversed(), keys, values);

            // Check Count.
            Assert.AreEqual(keys.Length, view.Count);
            Assert.AreEqual(values.Length, view.Count);

            // Check CopyTo
            KeyValuePair<TKey, TValue>[] pairArray = new KeyValuePair<TKey, TValue>[view.Count];
            view.CopyTo(pairArray, 0);

            for (int i = 0; i < view.Count; ++i) {
                Assert.AreEqual(keys[i], pairArray[i].Key);
                Assert.AreEqual(values[i], pairArray[i].Value);
            }

            InterfaceTests.TestDictionary((IDictionary)view, keys, values, nonKey, true);
            InterfaceTests.TestDictionaryGeneric((IDictionary<TKey, TValue>)view, keys, values, nonKey, true, null, null);
        }

        [TestMethod]
        public void Range()
        {
            var dict1 = new OrderedDictionary<string, int> {
                ["foo"] = 23,
                ["a"] = 11,
                ["b"] = 119,
                [""] = 981,
                ["r4"] = 9,
                ["b"] = 7,
                ["hello"] = 198,
                ["q"] = 199,
                ["ww"] = -8,
                ["ww"] = -9,
                ["p"] = 1234
            };

            CheckView(dict1.Range("a", true, "z", false), 
                                        new string[] { "a", "b", "foo", "hello", "p", "q", "r4", "ww" },
                                        new int[] { 11, 7, 23, 198, 1234, 199, 9, -9 },
                                        "");
            CheckView(dict1.Range("b", true, "q", false),
                                        new string[] { "b", "foo", "hello", "p" },
                                        new int[] { 7, 23, 198, 1234},
                                        "q");
            CheckView(dict1.Range("b", false, "q", false),
                                        new string[] { "foo", "hello", "p" },
                                        new int[] { 23, 198, 1234 },
                                        "q");
            CheckView(dict1.Range("b", true, "q", true),
                                        new string[] { "b", "foo", "hello", "p", "q" },
                                        new int[] { 7, 23, 198, 1234, 199 },
                                        "ww");
            CheckView(dict1.Range("b", false, "q", true),
                                        new string[] { "foo", "hello", "p", "q" },
                                        new int[] { 23, 198, 1234, 199 },
                                        "b");
            CheckView(dict1.Range("", true, "z", false),
                                        new string[] { "", "a", "b", "foo", "hello", "p", "q", "r4", "ww" },
                                        new int[] { 981, 11, 7, 23, 198, 1234, 199, 9, -9 },
                                        "FOO");
            CheckView(dict1.Range("", false, "z", true),
                                        new string[] { "a", "b", "foo", "hello", "p", "q", "r4", "ww" },
                                        new int[] { 11, 7, 23, 198, 1234, 199, 9, -9 },
                                        "FOO");
            CheckView(dict1.Range("f", true, "i", false),
                                        new string[] { "foo", "hello" },
                                        new int[] { 23, 198 },
                                        "b");
            CheckView(dict1.Range("b", true, "b", false),
                                        new string[] { }, new int[] {  },
                                        "b");
            CheckView(dict1.Range("p", true, "q1", false),
                                        new string[] { "p", "q" }, new int[] { 1234, 199 },
                                        "q1");
            CheckView(dict1.Range("p", true, "q", false),
                                        new string[] { "p" }, new int[] { 1234},
                                        "q");
            CheckView(dict1.Range("p", false, "q", true),
                                        new string[] { "q" }, new int[] { 199 },
                                        "p");
            CheckView(dict1.Range("p", false, "q", false),
                                        new string[] {  }, new int[] {  },
                                        "q");
            CheckView(dict1.Range("p1", true, "q1", false),
                                        new string[] {  "q" }, new int[] { 199 },
                                        "p");
            CheckView(dict1.Range("p1", true, "q", false),
                                        new string[] {  }, new int[] {  },
                                        "q");
            CheckView(dict1.Range("p1", false, "q", true),
                                        new string[] { "q" }, new int[] { 199 },
                                        "p");
            CheckView(dict1.Range("z", true, "f", false),
                                        new string[0], new int[0],
                                        "p");
            CheckView(dict1.Range("g", true, "h", false),
                                        new string[0], new int[0],
                                        "h");

            CheckView(dict1.Range("a", true, "z", false).Reversed(),
                                        new string[] { "ww", "r4", "q", "p", "hello", "foo", "b", "a" },
                                        new int[] { -9, 9, 199, 1234, 198, 23, 7, 11 },
                                        "");
            CheckView(dict1.Range("f", true, "s", false).Reversed(),
                                        new string[] { "r4", "q", "p", "hello", "foo"},
                                        new int[] { 9, 199, 1234, 198, 23},
                                        "a");
            CheckView(dict1.Range("f", false, "s", true).Reversed(),
                                        new string[] { "r4", "q", "p", "hello", "foo" },
                                        new int[] { 9, 199, 1234, 198, 23 },
                                        "a");

            CheckView(dict1.RangeFrom("hello", true),
                                        new string[] { "hello", "p", "q", "r4", "ww" },
                                        new int[] { 198, 1234, 199, 9, -9 },
                                        "b");
            CheckView(dict1.RangeFrom("hello", false),
                                        new string[] { "p", "q", "r4", "ww" },
                                        new int[] { 1234, 199, 9, -9 },
                                        "b");
            CheckView(dict1.RangeFrom("z", true),
                                        new string[] { },
                                        new int[] {  },
                                        "ww");
            CheckView(dict1.RangeFrom("z", false),
                                        new string[] { },
                                        new int[] { },
                                        "ww");
            CheckView(dict1.RangeTo("hello", false),
                                        new string[] { "", "a", "b", "foo" },
                                        new int[] { 981, 11, 7, 23 },
                                        "hello");
            CheckView(dict1.RangeTo("hello", true),
                                        new string[] { "", "a", "b", "foo", "hello" },
                                        new int[] { 981, 11, 7, 23, 198 },
                                        "q");
            CheckView(dict1.RangeTo("", false),
                                        new string[] { },
                                        new int[] {  },
                                        "");
            CheckView(dict1.RangeTo("", true),
                                        new string[] { ""},
                                        new int[] {981  },
                                        "1");
        }

        [TestMethod]
        public void CustomIComparer()
        {
            IComparer<int> myComparer = new GOddEvenComparer();

            var dict1 = new OrderedDictionary<int, string>(myComparer) {
                [3] = "foo",
                [8] = "bar",
                [9] = "baz",
                [12] = "biff"
            };

            InterfaceTests.TestReadonlyCollectionGeneric(dict1.Keys, new int[] { 3, 9, 8, 12 }, true, "KeysCollection");
            InterfaceTests.TestReadonlyCollectionGeneric(dict1.Values, new string[] { "foo", "baz", "bar", "biff" }, true, "ValuesCollection");
        }

        [TestMethod]
        public void CustomOrdering()
        {
            Comparison<int> myOrdering = ComparersTests.CompareOddEven;

            var dict1 = new OrderedDictionary<int, string>(myOrdering) {
                [3] = "foo",
                [8] = "bar",
                [9] = "baz",
                [12] = "biff"
            };

            InterfaceTests.TestReadonlyCollectionGeneric(dict1.Keys, new int[] { 3, 9, 8, 12 }, true, "KeysCollection");
            InterfaceTests.TestReadonlyCollectionGeneric(dict1.Values, new string[] { "foo", "baz", "bar", "biff" }, true, "ValuesCollection");
        }

        // Check that cloned dictionaries maintain a custom ordering.
        [TestMethod]
        public void CloneCustomOrdering()
        {
            Comparison<int> myOrdering = ComparersTests.CompareOddEven;

            var dict1 = new OrderedDictionary<int, string>(myOrdering) {
                [3] = "foo",
                [8] = "bar",
                [9] = "baz",
                [12] = "biff"
            };

            OrderedDictionary<int, string> dict2 = dict1.Clone();
            OrderedDictionary<int, string> dict3 = dict1.CloneContents();
            dict1[7] = "goofy";

            InterfaceTests.TestReadWriteDictionaryGeneric(dict2, new int[] { 3, 9, 8, 12 }, new string[] { "foo", "baz", "bar", "biff" }, 7, true, null, null, null);
            InterfaceTests.TestReadWriteDictionaryGeneric(dict3, new int[] { 3, 9, 8, 12 }, new string[] { "foo", "baz", "bar", "biff" }, 7, true, null, null, null);
        }

        // Test that it looks like a non-generic ICollection of DictionaryEntrys.
        [TestMethod]
        public void ICollectionMembers()
        {
            var dict1 = new OrderedDictionary<int, string> {
                [9] = "baz",
                [8] = "bar",
                [12] = "biff",
                [3] = "foo"
            };

            InterfaceTests.TestCollection((ICollection)dict1,
                new DictionaryEntry[] {
                    new DictionaryEntry(3, "foo"), 
                    new DictionaryEntry(8, "bar"), 
                    new DictionaryEntry(9, "baz"), 
                    new DictionaryEntry(12, "biff") },
                true);
        }

        // Test that it looks like a non-generic ICollection of DictionaryEntrys.
        [TestMethod]
        public void GenericICollectionMembers()
        {
            var dict1 = new OrderedDictionary<int, string> {
                [9] = "baz",
                [8] = "bar",
                [12] = "biff",
                [3] = "foo"
            };

            InterfaceTests.TestReadWriteCollectionGeneric((ICollection<KeyValuePair<int, string>>)dict1, 
                new KeyValuePair<int,string>[] {
                    new KeyValuePair<int,string>(3, "foo"), 
                    new KeyValuePair<int,string>(8, "bar"), 
                    new KeyValuePair<int,string>(9, "baz"), 
                    new KeyValuePair<int,string>(12, "biff") },
                true);
        }

        [TestMethod]
        public void AddMany()
        {
            var dict1 = new OrderedDictionary<int, string>();
            var dict2 = new Dictionary<int, string>();

            dict1[9] = "baz";
            dict1[8] = "bar";
            dict1[12] = "biff";
            dict1[3] = "foo";

            dict2[0] = "fribble";
            dict2[8] = "banana";
            dict2[123] = "biff";
            dict2[3] = "hello";

            dict1.AddMany(dict2);

            InterfaceTests.TestReadWriteCollectionGeneric((ICollection<KeyValuePair<int, string>>)dict1,
               new KeyValuePair<int, string>[] {
                    new KeyValuePair<int,string>(0, "fribble"), 
                    new KeyValuePair<int,string>(3, "hello"), 
                    new KeyValuePair<int,string>(8, "banana"), 
                    new KeyValuePair<int,string>(9, "baz"), 
                    new KeyValuePair<int,string>(12, "biff"),
                    new KeyValuePair<int,string>(123, "biff")},
               true);
        }

        [TestMethod]
        public void RemoveCollection()
        {
            var dict1 = new OrderedDictionary<int, string> {
                [9] = "baz",
                [8] = "bar",
                [12] = "biff",
                [3] = "foo",
                [127] = "hello"
            };

            int[] array = new int[] { 8, 127, 4, 3 };

            int count = dict1.RemoveMany(array);
            Assert.AreEqual(3, count);

            InterfaceTests.TestReadWriteCollectionGeneric((ICollection<KeyValuePair<int, string>>)dict1,
               new KeyValuePair<int, string>[] {
                    new KeyValuePair<int,string>(9, "baz"), 
                    new KeyValuePair<int,string>(12, "biff") },
               true);
        }

	    private void CheckNullKeyException(Exception e) {
            Assert.IsTrue(e is ArgumentNullException);
        }
        [TestMethod]
        public void IDictionaryInterface()
        {
            string[] s_array = { "Eric", "Clapton", "Rules", "The", "World" };
            int[] i_array = { 1, 5, 6, 5, 19 };
            string[] s_array_sorted = { "Clapton", "Eric", "Rules", "The", "World" };
            int[] i_array_sorted = { 5, 1, 6, 5, 19 };

            var dict1 = new OrderedDictionary<string, int>();
            for (int i = 0; i < s_array.Length; ++i) {
                dict1.Add(s_array[i], i_array[i]);
            }

            InterfaceTests.TestReadWriteDictionary(dict1, s_array_sorted, i_array_sorted, "foo", true, "ReadOnlyTestDictionary");
            InterfaceTests.TestReadWriteDictionaryGeneric(dict1, s_array_sorted, i_array_sorted, "foo", true, "ReadOnlyTestDictionary", null, null);
        }

	    [TestMethod]
	    public void FailFastEnumerator1()
        {
	        var dict1 = new OrderedDictionary<double, int>();

	        double d = 1.218034;
	        for (int i = 0; i < 50; ++i) {
	            dict1[d] = i;
	            d = d * 1.3451 - .31;
	        }

	        // enumeration of the Keys collection should throw once the dictionary is modified.
	        void InvalidOperation()
            {
	            foreach (double k in dict1.Keys) {
	                if (k > 3.0) {
                        dict1[4.5] = 9;
                    }
                }
	        }

	        ThrowsInvalid(InvalidOperation);
	    }

	    [TestMethod]
	    public void FailFastEnumerator2()
        {
	        var dict1 = new OrderedDictionary<double, int>();

	        double d = 1.218034;
	        for (int i = 0; i < 100; ++i) {
	            dict1[d] = i;
	            d = d * 1.3451 - .31;
	        }

	        // enumeration of a view should throw once the dictionary is modified.
	        void InvalidOperation()
            {
	            foreach (KeyValuePair<double, int> p in dict1.Range(1.7, true, 11.4, false)) {
	                if (p.Key > 7.0) {
                        dict1.Clear();
                    }
                }
	        }
	        ThrowsInvalid(InvalidOperation);
	    }

        [TestMethod]
        public void ComparerProperty()
        {
            IComparer<int> comparer1 = new GOddEvenComparer();
            var dict1 = new OrderedDictionary<int, string>(comparer1);
            Assert.AreSame(comparer1, dict1.Comparer);
            var dict2 = new OrderedDictionary<decimal, string>();
            Assert.AreSame(Comparer<decimal>.Default, dict2.Comparer);
            var dict3 = new OrderedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Assert.AreSame(StringComparer.OrdinalIgnoreCase, dict3.Comparer);

            Comparison<int> comparison1 = ComparersTests.CompareOddEven;
            var dict4 = new OrderedDictionary<int, string>(comparison1);
            var dict5 = new OrderedDictionary<int, string>(comparison1);
            Assert.AreEqual(dict4.Comparer, dict5.Comparer);
            Assert.IsFalse(dict4.Comparer == dict5.Comparer);
            Assert.IsFalse(object.Equals(dict4.Comparer, dict1.Comparer));
            Assert.IsFalse(object.Equals(dict4.Comparer, Comparer<int>.Default));
            Assert.IsTrue(dict4.Comparer.Compare(7, 6) < 0);

            Assert.AreSame(dict1.Comparer, dict1.Clone().Comparer);
            Assert.AreSame(dict2.Comparer, dict2.Clone().Comparer);
            Assert.AreSame(dict3.Comparer, dict3.Clone().Comparer);
            Assert.AreSame(dict4.Comparer, dict4.Clone().Comparer);
            Assert.AreSame(dict5.Comparer, dict5.Clone().Comparer);
        }


        [TestMethod]
        public void SerializeStrings()
        {
            var d = new OrderedDictionary<string, double>(StringComparer.InvariantCultureIgnoreCase) {
                { "hEllo", 13 },
                { "foo", 7 },
                { "world", -9.5 },
                { "elvis", 0.9 },
                { null, 1.4 }
            };

            var result = (OrderedDictionary<string, double>)InterfaceTests.SerializeRoundTrip(d);

            InterfaceTests.TestDictionaryGeneric(result,
                new string[] { null, "eLVis", "FOO", "Hello", "WORLD" },
                new double[] {  1.4, 0.9, 7, 13, -9.5 },
                "zippy", true, StringComparer.InvariantCultureIgnoreCase.Equals, null);
        }

        [TestMethod]
        public void ConstructionWithInitialization()
        {
            var init = new Dictionary<string, int> {
                ["foo"] = 16,
                ["FOO"] = 19,
                ["fiddle"] = 107,
                ["goofy"] = 11,
                ["trackstar"] = 19,
                ["GOOfy"] = 110,
                ["bar"] = 99
            };

            var dict1 = new OrderedDictionary<string, int>(init);

            InterfaceTests.TestDictionaryGeneric(dict1,
                new string[] { "bar", "fiddle", "foo", "FOO", "goofy", "GOOfy", "trackstar" },
                new int[] { 99, 107, 16, 19, 11, 110, 19 },
                "zippy", true, null, null);

            var dict2 = new OrderedDictionary<string, int>(dict1, Algorithms.GetReverseComparer(StringComparer.InvariantCultureIgnoreCase));

            InterfaceTests.TestDictionaryGeneric(dict2,
                new string[] { "trackstar", "GOOfy", "FOO", "fiddle", "bar" },
                new int[] { 19, 110, 19, 107, 99 },
                "zippy", true, null, null);

            int EqualFirstChar(string x, string y) {
                return x[0].CompareTo(y[0]);
            }

            var dict3 = new OrderedDictionary<string, int>(dict1, Algorithms.GetReverseComparison((Comparison<string>) EqualFirstChar));

            InterfaceTests.TestDictionaryGeneric(dict3,
                new string[] { "trackstar", "goofy", "foo", "bar", "GOOfy", "FOO"},
                new int[] { 19, 11, 16, 99, 110, 19},
                "zippy", true, null, null);

        }

    }
}

