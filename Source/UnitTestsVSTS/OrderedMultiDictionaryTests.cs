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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wintellect.PowerCollections.Tests;
using static Wintellect.PowerCollections.Tests.TestPredicates;
using static Wintellect.PowerCollections.Tests.UtilTests;

namespace Wintellect.PowerCollections.Tests
{
    using MyInt = OrderedDictionaryTests.MyInt;

    [TestClass]
    public class OrderedMultiDictionaryTests
    {
        // Check the contents of a Multi-Dictionary non-destructively. Keys and Values must be in order.
        internal static void CheckOrderedMultiDictionaryContents<TKey, TValue>(OrderedMultiDictionary<TKey, TValue> dict, TKey[] keys, TValue[][] values, TKey nonKey, TValue nonValue, BinaryPredicate<TKey> keyEquals, BinaryPredicate<TValue> valueEquals)
        {
            int iKey, iValue;
            ICollection<TValue> getValues;

            if (keyEquals == null)
                keyEquals = ObjectEquals;
            if (valueEquals == null)
                valueEquals = ObjectEquals;

            // Check Count.
            Assert.AreEqual(keys.Length, dict.Count);

            // Check indexer, ContainsKey, Contains, TryGetValue for each key.
            for (iKey = 0; iKey < keys.Length; ++iKey) {
                Assert.IsTrue(dict.ContainsKey(keys[iKey]));
                Assert.IsTrue(dict.Contains(new KeyValuePair<TKey, ICollection<TValue>>(keys[iKey], values[iKey])));

                bool b = ((IDictionary<TKey,ICollection<TValue>>)dict).TryGetValue(keys[iKey], out getValues);
                Assert.IsTrue(b);
                iValue = 0;
                foreach (TValue val in getValues) {
                    Assert.IsTrue(valueEquals(values[iKey][iValue], val));
                    ++iValue;
                }

                iValue = 0;
                foreach (TValue val in values[iKey]) {
                    Assert.IsTrue(dict.Contains(keys[iKey], val));
                    ++iValue;
                }

                iValue = 0;
                foreach (TValue val in dict[keys[iKey]]) {
                    Assert.IsTrue(valueEquals(values[iKey][iValue], val));
                    ++iValue;
                }
                Assert.IsTrue(iValue == values[iKey].Length);
            }

            // Check Keys collection.
            iKey = 0;
            foreach (TKey key in dict.Keys) {
                Assert.IsTrue(keyEquals(keys[iKey], key));
                ++iKey;
            }
            Assert.IsTrue(iKey == keys.Length);
            InterfaceTests.TestReadonlyCollectionGeneric(dict.Keys, keys, true, null, keyEquals);

            // Check Values collection
            iKey = 0; iValue = 0;
            int valueCount = 0;
            foreach (TValue val in dict.Values) {
                Assert.IsTrue(valueEquals(values[iKey][iValue], val));
                ++iValue;
                if (iValue == values[iKey].Length) {
                    iValue = 0;
                    ++iKey;
                }
                ++valueCount;
            }
            Assert.IsTrue(iKey == keys.Length);

            int a = 0;
            TValue[] vals = new TValue[valueCount];
            for (iKey = 0; iKey < keys.Length; ++iKey) {
                for (iValue = 0; iValue < values[iKey].Length; ++iValue) {
                    vals[a++] = values[iKey][iValue];
                }
            }
            InterfaceTests.TestReadonlyCollectionGeneric(dict.Values, vals, true, null, valueEquals);

            // Check KeyValuePairs collection.
            iKey = 0; iValue = 0;
            valueCount = 0;
            foreach (KeyValuePair<TKey, TValue> pair in dict.KeyValuePairs) {
                Assert.IsTrue(keyEquals(keys[iKey], pair.Key));
                Assert.IsTrue(valueEquals(values[iKey][iValue], pair.Value));
                ++iValue;
                if (iValue == values[iKey].Length) {
                    iValue = 0;
                    ++iKey;
                }
                ++valueCount;
            }
            Assert.IsTrue(iKey == keys.Length);

            a = 0;
            KeyValuePair<TKey, TValue>[] pairs = new KeyValuePair<TKey, TValue>[valueCount];
            for (iKey = 0; iKey < keys.Length; ++iKey) {
                for (iValue = 0; iValue < values[iKey].Length; ++iValue) {
                    pairs[a++] = Kvp.Of(keys[iKey], values[iKey][iValue]);
                }
            }
            InterfaceTests.TestReadonlyCollectionGeneric(dict.KeyValuePairs, pairs, true, null, InterfaceTests.KeyValueEquals(keyEquals, valueEquals));

            // Tests Contains, ContainsKey, TryGetValue for wrong values.
            Assert.IsFalse(dict.ContainsKey(nonKey));
            Assert.IsFalse(((IDictionary<TKey, ICollection<TValue>>)dict).TryGetValue(nonKey, out getValues));
            for (iKey = 0; iKey < keys.Length; ++iKey) {
                Assert.IsFalse(dict.Contains(keys[iKey], nonValue));
                Assert.IsFalse(dict.Contains(new KeyValuePair<TKey, ICollection<TValue>>(keys[iKey], new TValue[1] { nonValue })));
            }

            // Test IDictionary<TKey,IEnumerable<TValue>> implementation
            InterfaceTests.TestReadWriteMultiDictionaryGeneric(dict, keys, values, nonKey, nonValue, true, "OrderedMultiDictionary", keyEquals, valueEquals);
        }

        // Do random add,remove,replaces and create an array.
        private int[,] AddRemoveRandom(Random rand, OrderedMultiDictionary<int, string> dict, bool useDups, int iter)
        {
            const int MAXKEY = 100, MAXVAL = 50;
            int[,] counts = new int[MAXKEY, MAXVAL];

            for (int x = 0; x < iter; ++x) {
                int key = rand.Next(MAXKEY);
                int val = rand.Next(MAXVAL);
                string valString = string.Format("A{0:0000}", val);

                if (counts[key, val] == 0) {
                    if (rand.Next(30) == 0) {
                        // Do a replace
                        dict.Replace(key, valString);
                        for (int i = 0; i < MAXVAL; ++i)
                            counts[key, i] = 0;
                        counts[key, val] = 1;
                    }
                    else {
                        // Do an add
                        dict.Add(key, valString);
                        counts[key, val] = 1;
                    }
                }
                else {
                    if (rand.Next(30) == 0) {
                        // Do a replace
                        dict.Replace(key, valString);
                        for (int i = 0; i < MAXVAL; ++i)
                            counts[key, i] = 0;
                        counts[key, val] = 1;
                    }
                    else if (rand.Next(5) < 2) {
                        // Do an add
                        dict.Add(key, valString);
                        if (useDups)
                            counts[key, val] += 1;
                    }
                    else {
                        // Do a remove
                        dict.Remove(key, valString);
                        counts[key, val] -= 1;
                    }
                }
            }

            return counts;
        }

        // Check an ordered multi-dictionary against an array.
        private void CheckAgainstArray(OrderedMultiDictionary<int, string> dict, int[,] array)
        {
            var values = new List<string[]>();
            var keys = new List<int>();
            var vals = new List<string>();

            for (int i = 0; i < array.GetLength(0); ++i) {
                bool hasval = false;
                for (int j = 0; j < array.GetLength(1); ++j) {
                    if (array[i, j] > 0) {
                        hasval = true;
                        for (int x = 0; x < array[i, j]; ++x)
                            vals.Add(string.Format("A{0:0000}", j));
                    }
                }

                if (hasval) {
                    keys.Add(i);
                    values.Add(vals.ToArray());
                    vals.Clear();
                }
            }

            int[] keysArray = keys.ToArray();
            string[][] valsArray = values.ToArray();

            CheckOrderedMultiDictionaryContents(dict,
                keysArray,
                valsArray,
                -1, "Foo", null, null);
        }

        [TestMethod]
        public void RandomAdd()
        {
            var rand = new Random(14);
            var dict = new OrderedMultiDictionary<int,string>(true);

            int[,] array = AddRemoveRandom(rand, dict, true, 3000);
            CheckAgainstArray(dict, array);
        }


        [TestMethod]
        public void Add()
        {
            // Test without duplicate values.
            var dict1 = new OrderedMultiDictionary<string, double>(false) {
                { "foo", 3.5 },
                { "foo", -1.2 },
                { null, 11.1 },
                { "foo", 8.8 },
                { null, 11.1 },
                { "bar", 9.8 },
                { "foo", 8.8 },
                { "gib", 7.1 },
                { "S", -9 },
                { null, 5.5 }
            };

            CheckOrderedMultiDictionaryContents(dict1,
                new string[] { null, "bar", "foo", "gib", "S" },
                new double[][] { new double[] { 5.5, 11.1 }, new double[] { 9.8 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 7.1 }, new double[] { -9 } },
                "zip", -100, null, null);

            // Test with duplicate values.
            dict1 = new OrderedMultiDictionary<string, double>(true) {
                { "foo", 3.5 },
                { "foo", -1.2 },
                { null, 11.1 },
                { "foo", 8.8 },
                { null, 11.1 },
                { "bar", 9.8 },
                { "foo", 8.8 },
                { "gib", 7.1 },
                { "S", -9 },
                { null, 5.5 }
            };

            CheckOrderedMultiDictionaryContents(dict1,
                new string[] { null, "bar", "foo", "gib", "S" },
                new double[][] { new double[] { 5.5, 11.1, 11.1 }, new double[] { 9.8 }, new double[] { -1.2, 3.5, 8.8, 8.8 }, new double[] { 7.1 }, new double[] { -9 } },
                "zip", -100, null, null);

            // Test duplicate values with distinct equal values.
            var dict2 = new OrderedMultiDictionary<string, string>(false, StringComparer.InvariantCultureIgnoreCase, StringComparer.InvariantCultureIgnoreCase) {
                { "foo", "BAR" },
                { "Foo", "bar" }
            };
            InterfaceTests.TestEnumerableElements(dict2.Keys, new string[] { "Foo" });
            InterfaceTests.TestEnumerableElements(dict2["FOO"], new string[] { "bar" });
            dict2 = new OrderedMultiDictionary<string, string>(true, StringComparer.InvariantCultureIgnoreCase, StringComparer.InvariantCultureIgnoreCase) {
                { "foo", "BAR" },
                { "Foo", "bar" }
            };
            InterfaceTests.TestEnumerableElements(dict2.Keys, new string[] { "foo" });
            InterfaceTests.TestEnumerableElements(dict2["FOO"], new string[] { "BAR", "bar" });
            Console.WriteLine(dict2.ToString());
            InterfaceTests.TestEnumerableElements(dict2.KeyValuePairs, new KeyValuePair<string, string>[] { Kvp.Of("foo", "BAR"),
                Kvp.Of("Foo", "bar")}, InterfaceTests.KeyValueEquals<string,string>());
        }

        [TestMethod]
        public void AddMany1()
        {
            // Test without duplicate values.
            var dict1 = new OrderedMultiDictionary<string, double>(false, StringComparer.InvariantCultureIgnoreCase);

            dict1.AddMany("foo", AlgorithmsTests.EnumerableFromArray(new double[] { 9.8, 1.2, -9, 9.8, -9, 4 }));
            dict1.AddMany("hi", new double[0]);
            dict1.AddMany("FOO", AlgorithmsTests.EnumerableFromArray(new double[] { 8, -9 }));

            Assert.AreEqual(1, dict1.Count);
            Assert.IsTrue(dict1.ContainsKey("foo"));
            Assert.IsFalse(dict1.ContainsKey("hi"));
            InterfaceTests.TestEnumerableElements(dict1.Keys, new string[] { "FOO" });
            InterfaceTests.TestEnumerableElements(dict1["fOo"], new double[] { -9, 1.2, 4, 8, 9.8 });
            InterfaceTests.TestEnumerableElements
                (dict1.KeyValuePairs, new KeyValuePair<string, double>[] { 
                            new KeyValuePair<string,double>("FOO", -9),
                            new KeyValuePair<string,double>("foo", 1.2),
                            new KeyValuePair<string,double>("foo", 4),
                            new KeyValuePair<string,double>("FOO",8),
                            new KeyValuePair<string,double>("foo",9.8)
                            });

            // Test with duplicate values
            dict1 = new OrderedMultiDictionary<string, double>(true, StringComparer.InvariantCultureIgnoreCase);

            dict1.AddMany("foo", AlgorithmsTests.EnumerableFromArray(new double[] { 9.8, 1.2, -9, 9.8, -9, 4 }));
            dict1.AddMany("hi", new double[0]);
            dict1.AddMany("a", new double[] { 2, 1, 2 });
            dict1.AddMany("FOO", AlgorithmsTests.EnumerableFromArray(new double[] { 8, -9 }));

            Assert.AreEqual(2, dict1.Count);
            Assert.IsTrue(dict1.ContainsKey("foo"));
            Assert.IsFalse(dict1.ContainsKey("hi"));
            InterfaceTests.TestEnumerableElements(dict1.Keys, new string[] { "a", "foo"});
            InterfaceTests.TestEnumerableElements(dict1["fOo"], new double[] { -9, -9, -9, 1.2, 4, 8, 9.8, 9.8 });
            InterfaceTests.TestEnumerableElements
                (dict1.KeyValuePairs, new KeyValuePair<string, double>[] { 
                            new KeyValuePair<string,double>("a", 1),
                            new KeyValuePair<string,double>("a", 2),
                            new KeyValuePair<string,double>("a", 2),
                            new KeyValuePair<string,double>("foo", -9),
                            new KeyValuePair<string,double>("foo", -9),
                            new KeyValuePair<string,double>("FOO", -9),
                            new KeyValuePair<string,double>("foo", 1.2),
                            new KeyValuePair<string,double>("foo", 4),
                            new KeyValuePair<string,double>("FOO",8),
                            new KeyValuePair<string,double>("foo",9.8),
                            new KeyValuePair<string,double>("foo",9.8)
                            });
        }


        [TestMethod]
        public void Replace()
        {
            var dict1 = new OrderedMultiDictionary<string, int>(true) {
                { "foo", 4 },
                { "bar", 7 },
                { "foo", 6 },
                { "z", 3 },
                { "bar", 8 },
                { "z", 3 },
                { "foo", 1 }
            };

            dict1.Replace("foo", 13);
            dict1.Replace("z", 19);
            dict1.Replace("hello", 193);
            dict1.Replace("foo", 123);
            dict1.Add("foo", 123);

            CheckOrderedMultiDictionaryContents(dict1,
                new string[] { "bar", "foo", "hello", "z" },
                new int[][] { new int[] { 7, 8 }, new int[] { 123, 123 }, new int[] {193}, new int[] { 19 } },
                "sailor", 19921, null, null);
        }

        [TestMethod]
        public void ReplaceMany()
        {
            var dict1 = new OrderedMultiDictionary<string, int>(false) {
                { "foo", 4 },
                { "bar", 7 },
                { "foo", 6 },
                { "z", 3 },
                { "bar", 8 },
                { "z", 3 },
                { "foo", 1 },
                { "bill", 9 }
            };

            dict1.ReplaceMany("bill", new int[0]);
            dict1.ReplaceMany("foo", new int[] { 13, 4 });
            dict1.ReplaceMany("z", new int[] { 19 });
            dict1.ReplaceMany("hello", new int[] { 193, -11, 193 });
            dict1.ReplaceMany("goodbye", new int[0]);
            dict1.ReplaceMany("foo", new int[] { 123, 0, 4 });
            dict1.Add("foo", 29);

            CheckOrderedMultiDictionaryContents(dict1,
                new string[] { "bar", "foo", "hello", "z" },
                new int[][] { new int[] { 7, 8 }, new int[] { 0, 4, 29, 123 }, new int[] { -11, 193 }, new int[] { 19 } },
                "sailor", 19921, null, null);
        }

        [TestMethod]
        public void RemoveKey()
        {
            var dict1 = new OrderedMultiDictionary<string, int>(true) {
                { "foo", 4 },
                { "bar", 7 },
                { "foo", 6 },
                { "z", 3 },
                { "bar", 8 },
                { "z", 10 },
                { "z", 3 },
                { "foo", 4 },
                { "bill", 9 }
            };

            Assert.IsTrue(dict1.ContainsKey("bill"));
            Assert.IsTrue(dict1.ContainsKey("foo"));
            Assert.IsTrue(dict1.ContainsKey("z"));

            Assert.IsTrue(dict1.Remove("bill"));
            Assert.IsFalse(dict1.Remove("bill"));
            Assert.IsFalse(dict1.Remove("smell"));
            Assert.IsTrue(dict1.Remove("foo"));

            CheckOrderedMultiDictionaryContents(dict1,
                new string[] { "bar", "z" },
                new int[][] { new int[] { 7, 8 }, new int[] { 3, 3, 10 }},
                "sailor", 19921, null, null);
        }

        [TestMethod]
        public void RemoveManyKeys()
        {
            var dict1 = new OrderedMultiDictionary<string, int>(true) {
                { "foo", 4 },
                { "bar", 7 },
                { "foo", 6 },
                { "z", 3 },
                { "bar", 8 },
                { "z", 10 },
                { "z", 3 },
                { "foo", 4 },
                { "bill", 9 }
            };

            Assert.IsTrue(dict1.ContainsKey("bill"));
            Assert.IsTrue(dict1.ContainsKey("foo"));
            Assert.IsTrue(dict1.ContainsKey("z"));

            Assert.AreEqual(2, dict1.RemoveMany(new string[] { "bill", "smell", "foo", "bill" }));

            CheckOrderedMultiDictionaryContents(dict1,
                new string[] { "bar", "z" },
                new int[][] { new int[] { 7, 8 }, new int[] { 3, 3, 10 } },
                "sailor", 19921, null, null);
        }

        [TestMethod]
        public void Remove()
        {
            var dict1 = new OrderedMultiDictionary<string, int>(true) {
                { "foo", 4 },
                { "bar", 7 },
                { "foo", 6 },
                { "z", 3 },
                { "bar", 8 },
                { "z", 10 },
                { "z", 3 },
                { "foo", 4 },
                { "bill", 9 },
                { "foo", 4 }
            };

            Assert.IsTrue(dict1.Remove("foo", 4));
            Assert.IsTrue(dict1.Remove("foo", 4));
            Assert.IsTrue(dict1.Remove("z", 10));
            Assert.IsFalse(dict1.Remove("z", 10));
            Assert.IsFalse(dict1.Remove("foo", 11));
            Assert.IsFalse(dict1.Remove(null, 0));
            Assert.IsTrue(dict1.Remove("bill", 9));

            CheckOrderedMultiDictionaryContents(dict1,
                new string[] { "bar", "foo", "z" },
                new int[][] { new int[] { 7, 8 }, new int[] { 4, 6 }, new int[] { 3, 3 } },
                "sailor", 19921, null, null);
        }

        [TestMethod]
        public void RemoveMany1()
        {
            var dict1 = new OrderedMultiDictionary<string, int>(true) {
                { "bill", 7 },
                { "foo", 4 },
                { "bar", 7 },
                { "foo", 6 },
                { "z", 3 },
                { "bar", 8 },
                { "z", 10 },
                { "z", 3 },
                { "foo", 4 },
                { "bill", 9 },
                { "foo", 4 }
            };

            Assert.AreEqual(2, dict1.RemoveMany("foo", new int[] { 4, 11, 4 }));
            Assert.AreEqual(1, dict1.RemoveMany("z", new int[] { 9, 2, 10 }));
            Assert.AreEqual(0, dict1.RemoveMany("z", new int[] { 10, 16, 144, 10 }));
            Assert.AreEqual(0, dict1.RemoveMany("foo", new int[0]));
            Assert.AreEqual(0, dict1.RemoveMany(null, new int[2] { 1, 2 }));
            Assert.AreEqual(2, dict1.RemoveMany("bill", new int[] { 9, 7 }));

            CheckOrderedMultiDictionaryContents(dict1,
                new string[] { "bar", "foo", "z" },
                new int[][] { new int[] { 7, 8 }, new int[] { 4, 6 }, new int[] { 3, 3 } },
                "sailor", 19921, null, null);
        }

        [TestMethod]
        public void Clear()
        {
            var dict1 = new OrderedMultiDictionary<string, int>(true) {
                { "foo", 4 },
                { "bill", 7 },
                { "foo", 4 },
                { "bar", 7 },
                { "foo", 6 },
                { "z", 3 },
                { "bar", 8 },
                { "z", 10 },
                { null, 3 },
                { "foo", 4 },
                { "bill", 9 },
                { "foo", 4 }
            };

            dict1.Clear();

            Assert.AreEqual(0, dict1.Count);
            Assert.IsFalse(dict1.ContainsKey("foo"));
            Assert.IsFalse(dict1.ContainsKey("z"));
            Assert.IsFalse(dict1.ContainsKey(null));
            Assert.AreEqual(0, Enumerable.Count(dict1.Keys));
            Assert.AreEqual(0, Enumerable.Count(dict1.Values));
            Assert.AreEqual(0, Enumerable.Count(dict1.KeyValuePairs));

            CheckOrderedMultiDictionaryContents(dict1, new string[0], new int[0][], "foo", 4, null, null);
        }

        [TestMethod]
        public void Count()
        {
            var dict1 = new OrderedMultiDictionary<string, int>(true) {
                { "foo", 4 },
                { null, 7 },
                { "bar", 11 },
                { "foo", 7 },
                { null, 7 },
                { "hello", 11 },
                { "foo", 4 }
            };
            Assert.AreEqual(4, dict1.Count);

            var dict2 = new OrderedMultiDictionary<string, int>(false) {
                { "foo", 4 },
                { null, 7 },
                { "bar", 11 },
                { "foo", 7 },
                { null, 7 },
                { "hello", 11 },
                { "foo", 4 }
            };
            Assert.AreEqual(4, dict2.Count);

            dict2.Remove("foo");
            Assert.AreEqual(3, dict2.Count);

            dict2.Clear();
            Assert.AreEqual(0, dict2.Count);
        }

        [TestMethod]
        public void ContainsKey()
        {
            var dict1 = new OrderedMultiDictionary<string, int>(true) {
                { "foo", 4 },
                { null, 7 },
                { "bar", 11 },
                { "foo", 7 },
                { null, 7 },
                { "hello", 11 },
                { "foo", 4 }
            };

            Assert.IsTrue(dict1.ContainsKey(null));
            Assert.IsTrue(dict1.ContainsKey("foo"));
            Assert.IsTrue(dict1.ContainsKey("bar"));
            Assert.IsTrue(dict1.ContainsKey("hello"));
            dict1.Remove("hello", 11);
            Assert.IsFalse(dict1.ContainsKey("hello"));
            dict1.Remove(null, 7);
            Assert.IsTrue(dict1.ContainsKey(null));
            dict1.Remove(null, 7);
            Assert.IsFalse(dict1.ContainsKey(null));
        }

        [TestMethod]
        public void Contains()
        {
            var dict1 = new OrderedMultiDictionary<string, int>(true) {
                { "foo", 4 },
                { null, 7 },
                { "bar", 11 },
                { "foo", 7 },
                { null, 7 },
                { "hello", 11 },
                { "foo", 4 }
            };

            Assert.IsTrue(dict1.Contains(null, 7));
            Assert.IsTrue(dict1.Contains("foo", 4));
            Assert.IsTrue(dict1.Contains("bar", 11));
            Assert.IsTrue(dict1.Contains("hello", 11));
            Assert.IsFalse(dict1.Contains("HELLO", 11));
            Assert.IsFalse(dict1.Contains("bar", 12));
            Assert.IsFalse(dict1.Contains("foo", 0));
            dict1.Remove("hello", 11);
            Assert.IsFalse(dict1.Contains("hello", 11));
            dict1.Remove(null, 7);
            Assert.IsTrue(dict1.Contains(null, 7));
            dict1.Remove(null, 7);
            Assert.IsFalse(dict1.Contains(null, 7));
        }

        [TestMethod]
        public void KeysCollection()
        {
            var dict1 = new OrderedMultiDictionary<string, int>(false, StringComparer.InvariantCultureIgnoreCase) {
                { "foo", 4 },
                { null, 2 },
                { "bar", 3 },
                { "sailor", 0 },
                { "FOO", 9 },
                { "b", 7 },
                { "Foo", -1 },
                { "BAR", 3 }
            };
            dict1.Remove("b", 7);

            InterfaceTests.TestReadonlyCollectionGeneric(dict1.Keys, new string[] { null, "BAR", "Foo", "sailor" }, true, null);

            Assert.IsTrue(dict1.Keys.Contains("foo"));
            Assert.IsTrue(dict1.Keys.Contains("Foo"));
            Assert.IsTrue(dict1.Keys.Contains(null));
            Assert.IsTrue(dict1.Keys.Contains("Sailor"));
            Assert.IsFalse(dict1.Keys.Contains("banana"));

            var dict2 = new OrderedMultiDictionary<string, int>(false, StringComparer.InvariantCultureIgnoreCase);
            InterfaceTests.TestEnumerableElements(dict2.Keys, new string[] { });
        }

        [TestMethod]
        public void ValuesCollection1()
        {
            var dict = new OrderedMultiDictionary<double, string>(false, Comparer<double>.Default, StringComparer.InvariantCultureIgnoreCase) {
                { 7, "Gizzle" },
                { 4, "foo" },
                { 6, "Foo" },
                { 3, "FOO" },
                { 3, "baz" },
                { 3, "bar" },
                { 4, "FOo" },
                { 3, "BAZ" },
                { 5, "bAZ" },
                { 7, "hello" },
                { 7, "foo" }
            };

            ICollection<string> vals = dict.Values;

            string[] expected = {
                "bar", "BAZ", "FOO", "FOo", "bAZ", "Foo", "foo", "Gizzle", "hello"};

            InterfaceTests.TestReadonlyCollectionGeneric(vals, expected, true, null);

            Assert.IsTrue(vals.Contains("gizzle"));
            Assert.IsTrue(vals.Contains("FOO"));
            Assert.IsTrue(vals.Contains("fOO"));
            Assert.IsTrue(vals.Contains("hello"));
            Assert.IsTrue(vals.Contains("bar"));
            Assert.IsTrue(vals.Contains("BAR"));
            Assert.IsFalse(vals.Contains("qatar"));
        }

        [TestMethod]
        public void ValuesCollection2()
        {
            var dict = new OrderedMultiDictionary<double, string>(true, Comparer<double>.Default, StringComparer.InvariantCultureIgnoreCase) {
                { 7, "Gizzle" },
                { 4, "foo" },
                { 6, "Foo" },
                { 3, "FOO" },
                { 3, "baz" },
                { 3, "bar" },
                { 4, "FOo" },
                { 3, "BAZ" },
                { 5, "bAZ" },
                { 7, "hello" },
                { 7, "foo" }
            };

            ICollection<string> vals = dict.Values;

            string[] expected = {
                "bar", "baz", "BAZ", "FOO", "foo", "FOo", "bAZ", "Foo", "foo", "Gizzle", "hello"};

            InterfaceTests.TestReadonlyCollectionGeneric(vals, expected, true, null);

            Assert.IsTrue(vals.Contains("gizzle"));
            Assert.IsTrue(vals.Contains("FOO"));
            Assert.IsTrue(vals.Contains("fOO"));
            Assert.IsTrue(vals.Contains("hello"));
            Assert.IsTrue(vals.Contains("bar"));
            Assert.IsTrue(vals.Contains("BAR"));
            Assert.IsFalse(vals.Contains("qatar"));
        }

        [TestMethod]
        public void KeyValuesCollection1()
        {
            var dict = new OrderedMultiDictionary<string, string>(false, StringComparer.InvariantCultureIgnoreCase, StringComparer.InvariantCultureIgnoreCase) {
                { "7A", "Gizzle" },
                { "4a", "foo" },
                { "6A", "Foo" },
                { "3a", "FOO" },
                { "3A", "baz" },
                { "3a", "bar" },
                { "4a", "FOo" },
                { "3A", "BAZ" },
                { "5a", "bAZ" },
                { "7a", "hello" },
                { "7A", "foo" }
            };

            ICollection<KeyValuePair<string,string>> pairs = dict.KeyValuePairs;

            string[] expectedKeys = {
                "3a", "3A", "3a", "4a", "5a", "6A", "7A", "7A", "7a"};
            string[] expectedVals = {
                "bar", "BAZ", "FOO", "FOo", "bAZ", "Foo", "foo", "Gizzle", "hello"};
            KeyValuePair<string, string>[] expectedPairs = new KeyValuePair<string, string>[expectedKeys.Length];
            for (int i = 0; i < expectedKeys.Length; ++i)
                expectedPairs[i] = Kvp.Of(expectedKeys[i], expectedVals[i]);

            InterfaceTests.TestReadonlyCollectionGeneric(pairs, expectedPairs, true, null);

            Assert.IsTrue(pairs.Contains(Kvp.Of("3a", "baz")));
            Assert.IsTrue(pairs.Contains(Kvp.Of("3A", "baz")));
            Assert.IsTrue(pairs.Contains(Kvp.Of("6a", "foo")));
            Assert.IsFalse(pairs.Contains(Kvp.Of("7A", "bar")));

        }

        [TestMethod]
        public void KeyValuesCollection2()
        {
            var dict = new OrderedMultiDictionary<string, string>(true, StringComparer.InvariantCultureIgnoreCase, StringComparer.InvariantCultureIgnoreCase) {
                { "7A", "Gizzle" },
                { "4A", "foo" },
                { "6A", "Foo" },
                { "3a", "FOO" },
                { "3A", "baz" },
                { "3a", "bar" },
                { "4a", "FOo" },
                { "3a", "BAZ" },
                { "5a", "bAZ" },
                { "7a", "hello" },
                { "7A", "foo" }
            };

            ICollection<KeyValuePair<string, string>> pairs = dict.KeyValuePairs;

            string[] expectedKeys = {
            "3a", "3A", "3a", "3a", "4A", "4a", "5a", "6A", "7A", "7A", "7a"};
            string[] expectedVals = {
            "bar", "baz", "BAZ", "FOO", "foo", "FOo", "bAZ", "Foo", "foo", "Gizzle", "hello"};
            KeyValuePair<string, string>[] expectedPairs = new KeyValuePair<string, string>[expectedKeys.Length];
            for (int i = 0; i < expectedKeys.Length; ++i)
                expectedPairs[i] = Kvp.Of(expectedKeys[i], expectedVals[i]);

            InterfaceTests.TestReadonlyCollectionGeneric(pairs, expectedPairs, true, null);

            Assert.IsTrue(pairs.Contains(Kvp.Of("3a", "baz")));
            Assert.IsTrue(pairs.Contains(Kvp.Of("3A", "baz")));
            Assert.IsTrue(pairs.Contains(Kvp.Of("6a", "foo")));
            Assert.IsFalse(pairs.Contains(Kvp.Of("7A", "bar")));
        }

        [TestMethod]
        public void Indexer()
        {
            var dict1 = new OrderedMultiDictionary<string, string>(true, StringComparer.InvariantCultureIgnoreCase, StringComparer.InvariantCultureIgnoreCase) {
                { "foo", "BAR" },
                { null, "hello" },
                { "Hello", "sailor" },
                { null, "hi" },
                { "foo", "bar" },
                { "HELLO", null },
                { "foo", "a" },
                { "Foo", "A" },
                { "trail", "mix" }
            };

            InterfaceTests.TestEnumerableElements(dict1[null], new string[] { "hello", "hi" });
            InterfaceTests.TestEnumerableElements(dict1["hELLo"], new string[] { null, "sailor" });
            InterfaceTests.TestEnumerableElements(dict1["foo"], new string[] { "a", "A", "BAR", "bar" });
            InterfaceTests.TestEnumerableElements(dict1["trail"], new string[] { "mix" });
            InterfaceTests.TestEnumerableElements(dict1["nothing"], new string[] {  });
        }

        [TestMethod]
        public void GetValueCount()
        {
            var dict1 = new OrderedMultiDictionary<string, string>(true, StringComparer.InvariantCultureIgnoreCase, StringComparer.InvariantCultureIgnoreCase) {
                { "foo", "BAR" },
                { null, "hello" },
                { "Hello", "sailor" },
                { null, "hi" },
                { "foo", "bar" },
                { "HELLO", null },
                { "foo", "a" },
                { "Foo", "A" },
                { "hello", null },
                { "trail", "mix" }
            };

            Assert.AreEqual(2, dict1[null].Count);
            Assert.AreEqual(3, dict1["hELLo"].Count);
            Assert.AreEqual(4, dict1["foo"].Count);
            Assert.AreEqual(1, dict1["trail"].Count);
            Assert.AreEqual(0, dict1["nothing"].Count);

            dict1 = new OrderedMultiDictionary<string, string>(false, StringComparer.InvariantCultureIgnoreCase, StringComparer.InvariantCultureIgnoreCase) {
                { "foo", "BAR" },
                { null, "hello" },
                { "Hello", "sailor" },
                { null, "hi" },
                { "foo", "bar" },
                { "HELLO", null },
                { "foo", "a" },
                { "Foo", "A" },
                { "hello", null },
                { "trail", "mix" }
            };

            Assert.AreEqual(2, dict1[null].Count);
            Assert.AreEqual(2, dict1["hELLo"].Count);
            Assert.AreEqual(2, dict1["foo"].Count);
            Assert.AreEqual(1, dict1["trail"].Count);
            Assert.AreEqual(0, dict1["nothing"].Count);

        }

        [TestMethod]
        public void IMultiDictionaryInterface()
        {
            var dict1 = new OrderedMultiDictionary<string, string>(true) {
                { "foo", "bar" },
                { null, "hello" },
                { "hello", "sailor" },
                { null, "hi" },
                { "foo", "bar" },
                { "hello", null },
                { "foo", "a" },
                { "foo", "a" },
                { "hello", null },
                { "trail", "mix" }
            };

            CheckOrderedMultiDictionaryContents(dict1,
                new string[] { null, "foo", "hello", "trail" },
                new string[][] { new string[] { "hello", "hi" }, new string[] { "a", "a", "bar", "bar" }, new string[] { null, null, "sailor" }, new string[] { "mix" } },
                "zippy", "pinhead", null, null);

            dict1 = new OrderedMultiDictionary<string, string>(false) {
                { "foo", "bar" },
                { null, "hello" },
                { "hello", "sailor" },
                { null, "hi" },
                { "foo", "bar" },
                { "hello", null },
                { "foo", "a" },
                { "foo", "a" },
                { "hello", null },

                { "trail", "mix" }
            };
            CheckOrderedMultiDictionaryContents(dict1,
                new string[] { null, "foo", "hello", "trail" },
                new string[][] { new string[] { "hello", "hi" }, new string[] { "a", "bar" }, new string[] { null, "sailor" }, new string[] { "mix" } },
                "zippy", "pinhead", null, null);

        }

        [TestMethod]
        public void CustomComparison()
        {
            int ReverseFirstLetter(string x, string y) {
                if (x[0] < y[0])
                    return 1;
                else if (x[0] > y[0])
                    return -1;
                else
                    return 0;
            }

            var dict1 = new OrderedMultiDictionary<string, string>(false, ReverseFirstLetter) {
                { "hello", "AAA" },
                { "hi", "aaa" },
                { "qubert", "hello" },
                { "queztel", "hello" },
                { "alpha", "omega" },
                { "alzabar", "oz" }
            };

            InterfaceTests.TestEnumerableElements(dict1.KeyValuePairs, new KeyValuePair<string, string>[] {
                Kvp.Of("queztel", "hello"),
                Kvp.Of("hi", "aaa"),
                Kvp.Of("hello", "AAA"),
                Kvp.Of("alpha", "omega"),
                Kvp.Of("alzabar", "oz")});

            InterfaceTests.TestEnumerableElements(dict1.Keys, new string[] { "queztel", "hi", "alpha" });

            var dict2 = new OrderedMultiDictionary<string, string>(false, StringComparer.InvariantCultureIgnoreCase.Compare, ReverseFirstLetter) {
                { "qubert", "dinosaur" },
                { "Hello", "AAA" },
                { "Hi", "aaa" },
                { "qubert", "hello" },
                { "queztel", "hello" },
                { "alpha", "omega" },
                { "Alpha", "oz" },
                { "qubert", "hippy" }
            };

            InterfaceTests.TestEnumerableElements(dict2.KeyValuePairs, new KeyValuePair<string, string>[] {
                Kvp.Of("Alpha", "oz"),
                Kvp.Of("Hello", "AAA"),
                Kvp.Of("Hi", "aaa"),
                Kvp.Of("qubert", "hippy"),
                Kvp.Of("qubert", "dinosaur"),
                Kvp.Of("queztel", "hello")});
        }

        [TestMethod]
        public void NotComparable1()
        {
            Assert.ThrowsException<InvalidOperationException>( () =>
            new OrderedMultiDictionary<OrderedDictionaryTests.UncomparableClass1, string>(false)
                );
        }

        [TestMethod]
        public void NotComparable2()
        {
            Assert.ThrowsException<InvalidOperationException>( () =>
            new OrderedMultiDictionary<string, UncomparableClass2>(true)
                );
        }

        [TestMethod]
        public void Clone()
        {
            var dict1 = new OrderedMultiDictionary<string, string>(false, StringComparer.InvariantCultureIgnoreCase.Compare, ReverseFirstLetter) {
                { "qubert", "dinosaur" },
                { "Hello", "AAA" },
                { "Hi", "aaa" },
                { "qubert", "hello" },
                { "queztel", "hello" },
                { "alpha", "omega" },
                { "Alpha", "oz" },
                { "qubert", "hippy" }
            };

            OrderedMultiDictionary<string, string> dict2 = dict1.Clone();

            Assert.IsTrue(dict1 != dict2);

            dict2.Add("qubert", "hoover");
            dict2.Remove("queztel");
            dict2.Add("hello", "banana");

            InterfaceTests.TestEnumerableElements(dict1.KeyValuePairs, new [] {
                Kvp.Of("Alpha", "oz"),
                Kvp.Of("Hello", "AAA"),
                Kvp.Of("Hi", "aaa"),
                Kvp.Of("qubert", "hippy"),
                Kvp.Of("qubert", "dinosaur"),
                Kvp.Of("queztel", "hello")});

            InterfaceTests.TestEnumerableElements(dict2.KeyValuePairs, new [] {
                Kvp.Of("Alpha", "oz"),
                Kvp.Of("hello", "banana"),
                Kvp.Of("Hello", "AAA"),
                Kvp.Of("Hi", "aaa"),
                Kvp.Of("qubert", "hoover"),
                Kvp.Of("qubert", "dinosaur")});

            var dict4 = new OrderedMultiDictionary<string, int>(true);
            OrderedMultiDictionary<string, int> dict5 = dict4.Clone();
            Assert.IsFalse(dict4 == dict5);
            Assert.IsTrue(dict4.Count == 0 && dict5.Count == 0);
            dict4.Add("hello", 1);
            Assert.IsTrue(dict4.Count == 1 && dict5.Count == 0);
            dict5.Add("hi", 7);
            dict4.Clear();
            Assert.IsTrue(dict4.Count == 0 && dict5.Count == 1);
        }

        void CompareClones<K, V>(OrderedMultiDictionary<K, V> d1, OrderedMultiDictionary<K, V> d2)
        {
            IEnumerator<KeyValuePair<K, V>> e1 = d1.KeyValuePairs.GetEnumerator();
            IEnumerator<KeyValuePair<K, V>> e2 = d2.KeyValuePairs.GetEnumerator();

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
            int MyIntComparison(MyInt v1, MyInt v2) {
                if (v1 == null)
                    return (v2 == null) ? 0 : -1;
                else if (v2 == null)
                    return 1;
                else
                    return v2.value.CompareTo(v1.value);
            }

            int ScrewyCompare(int v1, int v2) {
                return -v2.CompareTo(v1);
            }

            var dict1 = new OrderedMultiDictionary<int, MyInt>(true,
                ScrewyCompare,
                MyIntComparison) {
                { 4, new MyInt(143) },
                { 7, new MyInt(2) },
                { 11, new MyInt(9) },
                { 7, new MyInt(119) },
                { 18, null },
                { 4, new MyInt(16) },
                { 7, null },
                { 7, new MyInt(119) }
            };
            OrderedMultiDictionary<int, MyInt> dict2 = dict1.CloneContents();
            CompareClones(dict1, dict2);

            var dict3 = new OrderedMultiDictionary<MyInt, int>(false, MyIntComparison) {
                { new MyInt(4), 143 },
                { new MyInt(7), 2 },
                { new MyInt(11), 9 },
                { new MyInt(7), 119 },
                { new MyInt(18), 0 },
                { new MyInt(4), 16 },
                { null, 11 },
                { new MyInt(7), 119 }
            };

            OrderedMultiDictionary<MyInt, int> dict4 = dict3.CloneContents();
            CompareClones(dict3, dict4);

            int Comparison(CloneableStruct s1, CloneableStruct s2) {
                return s1.value.CompareTo(s2.value);
            }

            var dict5 = new OrderedMultiDictionary<CloneableStruct, CloneableStruct>(true, Comparison, Comparison) {
                { new CloneableStruct(7), new CloneableStruct(-14) },
                { new CloneableStruct(16), new CloneableStruct(13) },
                { new CloneableStruct(7), new CloneableStruct(-14) },
                { new CloneableStruct(7), new CloneableStruct(31415) },
                { new CloneableStruct(1111), new CloneableStruct(0) }
            };
            OrderedMultiDictionary<CloneableStruct, CloneableStruct> dict6 = dict5.CloneContents();

            IEnumerator<KeyValuePair<CloneableStruct, CloneableStruct>> e1 = dict5.KeyValuePairs.GetEnumerator();
            IEnumerator<KeyValuePair<CloneableStruct, CloneableStruct>> e2 = dict6.KeyValuePairs.GetEnumerator();

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

        class NotCloneable: IComparable<NotCloneable>
        { 
            public int  CompareTo(NotCloneable other)
            {
 	            return 0;
            }

            public bool  Equals(NotCloneable other)
            {
 	            return true;
            }
        }

        [TestMethod]
        public void CantCloneContents()
        {
            var dict1 = new OrderedMultiDictionary<int, NotCloneable>(true) {
                [4] = new NotCloneable[] { new NotCloneable() },
                [5] = new NotCloneable[] { new NotCloneable(), new NotCloneable() }
            };

            Assert.ThrowsException<InvalidOperationException>(() => dict1.CloneContents());
        }


        [TestMethod]
        public void FailFastEnumerator()
        {
            var dict1 = new OrderedMultiDictionary<string, int>(true) {
                { "foo", 12 },
                { "foo", 15 },
                { "foo", 3 },
                { "foo", 12 },
                { "bar", 1 },
                { "bar", 17 }
            };

            int iter = 0;
            try {
                foreach (KeyValuePair<string, int> pair in dict1.KeyValuePairs) {
                    if (pair.Key == "foo")
                        dict1.Replace("bar", 19);
                    ++iter;
                }
            }
            catch (Exception e) {
                Assert.IsTrue(e is InvalidOperationException);
                Assert.AreEqual(3, iter);
            }

            iter = 0;
            try {
                foreach (string key in dict1.Keys) {
                    if (key == "foo")
                        dict1.Add("grump", 117);
                    ++iter;
                }
            }
            catch (Exception e) {
                Assert.IsTrue(e is InvalidOperationException);
                Assert.AreEqual(2, iter);
            }

            iter = 0;
            try {
                foreach (int value in dict1["foo"]) {
                    if (value == 12)
                        dict1.Remove("grump", 117);
                    ++iter;
                }
            }
            catch (Exception e) {
                Assert.IsTrue(e is InvalidOperationException);
                Assert.AreEqual(2, iter);
            }

            iter = 0;
            try {
                foreach (string key in dict1.Keys) {
                    if (key == "foo")
                        dict1.Clear();
                    ++iter;
                }
            }
            catch (Exception e) {
                Assert.IsTrue(e is InvalidOperationException);
                Assert.AreEqual(2, iter);
            }

        }

        [TestMethod]
        public void KeyComparerProperty()
        {
            IComparer<int> comparer1 = new GOddEvenComparer();
            var dict1 = new OrderedMultiDictionary<int, string>(false, comparer1);
            Assert.AreSame(comparer1, dict1.KeyComparer);
            var dict2 = new OrderedMultiDictionary<decimal, string>(true);
            Assert.AreSame(Comparer<decimal>.Default, dict2.KeyComparer);
            var dict3 = new OrderedMultiDictionary<string, string>(true, StringComparer.OrdinalIgnoreCase, StringComparer.CurrentCulture);
            Assert.AreSame(StringComparer.OrdinalIgnoreCase, dict3.KeyComparer);

            Comparison<int> comparison1 = ComparersTests.CompareOddEven;
            var dict4 = new OrderedMultiDictionary<int, string>(true, comparison1);

            int ScrewyCompare(string x, string y) {
                return -x.CompareTo(y);
            }

            var dict5 = new OrderedMultiDictionary<int, string>(false, comparison1, ScrewyCompare);
            Assert.AreEqual(dict4.KeyComparer, dict5.KeyComparer);
            Assert.IsFalse(dict4.KeyComparer == dict5.KeyComparer);
            Assert.IsFalse(object.Equals(dict4.KeyComparer, dict1.KeyComparer));
            Assert.IsFalse(object.Equals(dict4.KeyComparer, Comparer<int>.Default));
            Assert.IsTrue(dict4.KeyComparer.Compare(7, 6) < 0);

            Assert.AreSame(dict1.KeyComparer, dict1.Clone().KeyComparer);
            Assert.AreSame(dict2.KeyComparer, dict2.Clone().KeyComparer);
            Assert.AreSame(dict3.KeyComparer, dict3.Clone().KeyComparer);
            Assert.AreSame(dict4.KeyComparer, dict4.Clone().KeyComparer);
            Assert.AreSame(dict5.KeyComparer, dict5.Clone().KeyComparer);
        }

        [TestMethod]
        public void ValueComparerProperty()
        {
            IComparer<int> comparer1 = new GOddEvenComparer();
            var dict1 = new OrderedMultiDictionary<string, int>(true, StringComparer.InvariantCulture, comparer1);
            Assert.AreSame(comparer1, dict1.ValueComparer);
            var dict2 = new OrderedMultiDictionary<string, decimal>(false);
            Assert.AreSame(Comparer<decimal>.Default, dict2.ValueComparer);
            var dict3 = new OrderedMultiDictionary<string, string>(true, StringComparer.InvariantCulture, StringComparer.OrdinalIgnoreCase);
            Assert.AreSame(StringComparer.OrdinalIgnoreCase, dict3.ValueComparer);

            Comparison<int> comparison1 = ComparersTests.CompareOddEven;

            int StringCompare(string x, string y) {
                return x.CompareTo(y);
            }

            var dict4 = new OrderedMultiDictionary<string, int>(true, StringCompare, comparison1);
            var dict5 = new OrderedMultiDictionary<string, int>(false, (x, y) => -x.CompareTo(y), comparison1);
            Assert.AreEqual(dict4.ValueComparer, dict5.ValueComparer);
            Assert.IsFalse(dict4.ValueComparer == dict5.ValueComparer);
            Assert.IsFalse(object.Equals(dict4.ValueComparer, dict1.ValueComparer));
            Assert.IsFalse(object.Equals(dict4.ValueComparer, Comparer<int>.Default));
            Assert.IsTrue(dict4.ValueComparer.Compare(7, 6) < 0);

            Assert.AreSame(dict1.ValueComparer, dict1.Clone().ValueComparer);
            Assert.AreSame(dict2.ValueComparer, dict2.Clone().ValueComparer);
            Assert.AreSame(dict3.ValueComparer, dict3.Clone().ValueComparer);
            Assert.AreSame(dict4.ValueComparer, dict4.Clone().ValueComparer);
            Assert.AreSame(dict5.ValueComparer, dict5.Clone().ValueComparer);
        }


        // Check the contents of a Multi-Dictionary non-destructively. Keys and Values must be in order.
        internal static void CheckView<TKey, TValue>(OrderedMultiDictionary<TKey, TValue>.View dict, TKey[] keys, TValue[][] values, TKey nonKey, TValue nonValue, bool cantAddNonKey, BinaryPredicate<TKey> keyEquals, BinaryPredicate<TValue> valueEquals)
        {
            int iKey, iValue;
            ICollection<TValue> getValues;

            if (keyEquals == null)
                keyEquals = ObjectEquals;
            if (valueEquals == null)
                valueEquals = ObjectEquals;

            // Check Count.
            Assert.AreEqual(keys.Length, dict.Count);

            // Check indexer, ContainsKey, Contains, TryGetValue for each key.
            for (iKey = 0; iKey < keys.Length; ++iKey) {
                Assert.IsTrue(dict.ContainsKey(keys[iKey]));
                Assert.IsTrue(dict.Contains(new KeyValuePair<TKey, ICollection<TValue>>(keys[iKey], values[iKey])));

                bool b = ((IDictionary<TKey, ICollection<TValue>>)dict).TryGetValue(keys[iKey], out getValues);
                Assert.IsTrue(b);
                iValue = 0;
                foreach (TValue val in getValues) {
                    Assert.IsTrue(valueEquals(values[iKey][iValue], val));
                    ++iValue;
                }

                iValue = 0;
                foreach (TValue val in values[iKey]) {
                    Assert.IsTrue(dict.Contains(keys[iKey], val));
                    ++iValue;
                }

                iValue = 0;
                foreach (TValue val in dict[keys[iKey]]) {
                    Assert.IsTrue(valueEquals(values[iKey][iValue], val));
                    ++iValue;
                }
                Assert.IsTrue(iValue == values[iKey].Length);
            }

            // Check Keys collection.
            iKey = 0;
            foreach (TKey key in dict.Keys) {
                Assert.IsTrue(keyEquals(keys[iKey], key));
                ++iKey;
            }
            Assert.IsTrue(iKey == keys.Length);
            InterfaceTests.TestReadonlyCollectionGeneric(dict.Keys, keys, true, null);

            // Check Values collection
            iKey = 0; iValue = 0;
            int valueCount = 0;
            foreach (TValue val in dict.Values) {
                Assert.IsTrue(valueEquals(values[iKey][iValue], val));
                ++iValue;
                if (iValue == values[iKey].Length) {
                    iValue = 0;
                    ++iKey;
                }
                ++valueCount;
            }
            Assert.IsTrue(iKey == keys.Length);

            int a = 0;
            TValue[] vals = new TValue[valueCount];
            for (iKey = 0; iKey < keys.Length; ++iKey) {
                for (iValue = 0; iValue < values[iKey].Length; ++iValue) {
                    vals[a++] = values[iKey][iValue];
                }
            }
            InterfaceTests.TestReadonlyCollectionGeneric(dict.Values, vals, true, null);

            // Check KeyValuePairs collection.
            iKey = 0; iValue = 0;
            valueCount = 0;
            foreach (KeyValuePair<TKey, TValue> pair in dict.KeyValuePairs) {
                Assert.IsTrue(keyEquals(keys[iKey], pair.Key));
                Assert.IsTrue(valueEquals(values[iKey][iValue], pair.Value));
                ++iValue;
                if (iValue == values[iKey].Length) {
                    iValue = 0;
                    ++iKey;
                }
                ++valueCount;
            }
            Assert.IsTrue(iKey == keys.Length);

            a = 0;
            KeyValuePair<TKey, TValue>[] pairs = new KeyValuePair<TKey, TValue>[valueCount];
            for (iKey = 0; iKey < keys.Length; ++iKey) {
                for (iValue = 0; iValue < values[iKey].Length; ++iValue) {
                    pairs[a++] = Kvp.Of(keys[iKey], values[iKey][iValue]);
                }
            }
            InterfaceTests.TestReadonlyCollectionGeneric(dict.KeyValuePairs, pairs, true, null);

            // Tests Contains, ContainsKey, TryGetValue for wrong values.
            Assert.IsFalse(dict.ContainsKey(nonKey));
            Assert.IsFalse(((IDictionary<TKey, ICollection<TValue>>)dict).TryGetValue(nonKey, out getValues));
            Assert.AreEqual(0, dict[nonKey].Count);
            Assert.IsFalse(dict.Remove(nonKey));
            for (iKey = 0; iKey < keys.Length; ++iKey) {
                Assert.IsFalse(dict.Contains(keys[iKey], nonValue));
                Assert.IsFalse(dict.Contains(new KeyValuePair<TKey, ICollection<TValue>>(keys[iKey], new TValue[1] { nonValue })));
            }

            if (cantAddNonKey) {
                // Make sure Add throws exception
                Assert.ThrowsException<ArgumentException>(() => dict[nonKey] = new TValue[1] { nonValue });

                Assert.ThrowsException<ArgumentException>(() => dict.Add(nonKey, nonValue));
            }
            
            // Test IDictionary<TKey,IEnumerable<TValue>> implementation
            InterfaceTests.TestReadWriteMultiDictionaryGeneric(dict, keys, values, nonKey, nonValue, true, "OrderedMultiDictionary", null, null);
        }

        [TestMethod]
        public void RangeFrom()
        {
            var dict1 = new OrderedMultiDictionary<string, double>(false) {
                {"foo", 3.5},
                {"foo", -1.2},
                {null, 11.1},
                {"foo", 8.8},
                {null, 11.1},
                {"bar", 9.8},
                {"foo", 8.8},
                {"gib", 7.1},
                {"S", -9},
                {null, 5.5},
                {"gib", 1.1}
            };

            CheckView(dict1.RangeFrom("bar", true),
                new string[] { "bar", "foo", "gib", "S" },
                new double[][] { new double[] { 9.8 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 1.1, 7.1 }, new double[] { -9 } },
                null, 5.5, true, null, null);

            CheckView(dict1.RangeFrom("bar", false),
                new string[] { "foo", "gib", "S" },
                new double[][] { new double[] { -1.2, 3.5, 8.8 }, new double[] {1.1, 7.1 }, new double[] { -9 } },
                "bar", 9.8, true, null, null);

            CheckView(dict1.RangeFrom("alpha", false),
                new string[] { "bar", "foo", "gib", "S" },
                new double[][] { new double[] { 9.8 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 1.1, 7.1 }, new double[] { -9 } },
                "aaa", 5.5, true, null, null);

            CheckView(dict1.RangeFrom("hello", false),
                new string[] { "S" },
                new double[][] {new double[] { -9 } },
                "foo", 3.5, true, null, null);

            CheckView(dict1.RangeFrom("S", true),
                new string[] { "S" },
                new double[][] { new double[] { -9 } },
                "foo", 3.5, true, null, null);

            CheckView(dict1.RangeFrom("Z", true),
                new string[] {  },
                new double[][] { },
                "foo", 3.5, true, null, null);

            CheckView(dict1.RangeFrom("bar", true).Reversed(),
                new string[] { "S", "gib", "foo", "bar" },
                new double[][] {new double[] { -9 }, new double[] { 1.1, 7.1 }, new double[] { -1.2, 3.5, 8.8 },new double[] { 9.8 }   },
                "alpha", 5.5, true, null, null);

            CheckView(dict1.RangeFrom("bar", false). Reversed(),
                new string[] { "S", "gib", "foo" },
                new double[][] { new double[] { -9 }, new double[] { 1.1, 7.1 }, new double[] { -1.2, 3.5, 8.8 } },
                "bar", 9.8, true, null, null);

        }

        [TestMethod]
        public void RangeTo()
        {
            var dict1 = new OrderedMultiDictionary<string, double>(false) {
                {"foo", 3.5},
                {"foo", -1.2},
                {null, 11.1},
                {"foo", 8.8},
                {null, 11.1},
                {"bar", 9.8},
                {"foo", 8.8},
                {"gib", 7.1},
                {"S", -9},
                {null, 5.5},
                {"gib", 1.1}
            };

            CheckView(dict1.RangeTo("gib", true),
                new string[] { null, "bar", "foo", "gib" },
                new double[][] { new double[] {5.5, 11.1}, new double[] { 9.8 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 1.1, 7.1 } },
                "S", -9, true, null, null);

            CheckView(dict1.RangeTo("gib", false),
                new string[] { null, "bar", "foo" },
                new double[][] { new double[] { 5.5, 11.1 }, new double[] { 9.8 }, new double[] { -1.2, 3.5, 8.8 } },
                "gib", 1.1, true, null, null);

            CheckView(dict1.RangeTo("Z", false),
                new string[] { null, "bar", "foo", "gib", "S" },
                new double[][] { new double[] { 5.5, 11.1 }, new double[] { 9.8 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 1.1, 7.1 }, new double[] {-9} },
                "Zelda", -11, true, null, null);

            CheckView(dict1.RangeTo(null, false),
                new string[] {  },
                new double[][] { },
                null, 5.5, true, null, null);

            CheckView(dict1.RangeTo("gib", true).Reversed(),
                new string[] { "gib", "foo", "bar", null },
                new double[][] { new double[] { 1.1, 7.1 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 9.8 }, new double[] { 5.5, 11.1 }  },
                "S", -9, true, null, null);

            CheckView(dict1.RangeTo("gib", false).Reversed(),
                new string[] { "foo", "bar", null },
                new double[][] { new double[] { -1.2, 3.5, 8.8 }, new double[] { 9.8 }, new double[] { 5.5, 11.1 } },
                "gib", 1.1, true, null, null);

        }

        [TestMethod]
        public void Range()
        {
            var dict1 = new OrderedMultiDictionary<string, double>(false) {
                {"foo", 3.5},
                {"foo", -1.2},
                {null, 11.1},
                {"foo", 8.8},
                {null, 11.1},
                {"bar", 9.8},
                {"foo", 8.8},
                {"gib", 7.1},
                {"S", -9},
                {null, 5.5},
                {"gib", 1.1}
            };

            CheckView(dict1.Range(null, true, "S", true),
                new string[] { null, "bar", "foo", "gib", "S" },
                new double[][] { new double[] { 5.5, 11.1 }, new double[] { 9.8 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 1.1, 7.1 }, new double[] { -9 } },
                "Speedo", -14, true, null, null);

            CheckView(dict1.Range(null, false, "S", true),
                new string[] { "bar", "foo", "gib", "S" },
                new double[][] { new double[] { 9.8 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 1.1, 7.1 }, new double[] { -9 } },
                null, -14, true, null, null);

            CheckView(dict1.Range(null, true, "S", false),
                new string[] { null, "bar", "foo", "gib"},
                new double[][] { new double[] { 5.5, 11.1 }, new double[] { 9.8 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 1.1, 7.1 } },
                "S", -9, true, null, null);

            CheckView(dict1.Range(null, false, "S", false),
                new string[] { "bar", "foo", "gib" },
                new double[][] { new double[] { 9.8 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 1.1, 7.1 } },
                "Speedo", -14, true, null, null);


            CheckView(dict1.Range(null, true, "S", true).Reversed(),
                new string[] {"S", "gib", "foo", "bar", null },
                new double[][] { new double[] { -9 }, new double[] { 1.1, 7.1 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 9.8 }, new double[] { 5.5, 11.1 } },
                "Speedo", -14, true, null, null);

            CheckView(dict1.Range(null, false, "S", true).Reversed(),
                new string[] { "S", "gib", "foo", "bar" },
                new double[][] { new double[] { -9 }, new double[] { 1.1, 7.1 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 9.8 } },
                null, -14, true, null, null);

            CheckView(dict1.Range(null, true, "S", false).Reversed(),
                new string[] { "gib", "foo", "bar", null },
                new double[][] { new double[] { 1.1, 7.1 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 9.8 }, new double[] { 5.5, 11.1 } },
                "S", -9, true, null, null);

            CheckView(dict1.Range(null, false, "S", false).Reversed(),
                new string[] { "gib", "foo", "bar" },
                new double[][] { new double[] { 1.1, 7.1 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 9.8 } },
               "Speedo", -14, true, null, null);
        }

        [TestMethod]
        public void Reversed()
        {
            var dict1 = new OrderedMultiDictionary<string, double>(false) {
                {"foo", 3.5},
                {"foo", -1.2},
                {null, 11.1},
                {"foo", 8.8},
                {null, 11.1},
                {"bar", 9.8},
                {"foo", 8.8},
                {"gib", 7.1},
                {"S", -9},
                {null, 5.5},
                {"gib", 1.1}
            };

            CheckView(dict1.Reversed(),
                new string[] {"S", "gib", "foo", "bar", null },
                new double[][] { new double[] { -9 }, new double[] { 1.1, 7.1 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 9.8 }, new double[] { 5.5, 11.1 } },
                "Speedo", -14, false, null, null);

            CheckView(dict1.Reversed().Reversed(),
                new string[] { null, "bar", "foo", "gib", "S" },
                new double[][] { new double[] { 5.5, 11.1 }, new double[] { 9.8 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 1.1, 7.1 }, new double[] { -9 } },
                "Speedo", -14, false, null, null);
        }

        [TestMethod]
        public void RangeClear()
        {
            var dict1 = new OrderedMultiDictionary<string, double>(false);
            OrderedMultiDictionary<string, double> dict2;

            dict1.Add("foo", 3.5);
            dict1.Add("foo", -1.2);
            dict1.Add(null, 11.1);
            dict1.Add("foo", 8.8);
            dict1.Add(null, 11.1);
            dict1.Add("bar", 9.8);
            dict1.Add("foo", 8.8);
            dict1.Add("gib", 7.1);
            dict1.Add("S", -9);
            dict1.Add(null, 5.5);
            dict1.Add("gib", 1.1);

            dict2 = dict1.Clone();
            dict2.Range("bar", false, "gib", true).Clear();
            CheckOrderedMultiDictionaryContents(dict2,
                new string[] { null, "bar", "S" },
                new double[][] { new double[] { 5.5, 11.1 }, new double[] { 9.8 }, new double[] { -9 } },
                "foo", 3.5, null, null);

            dict2 = dict1.Clone();
            dict2.RangeTo("gib", true).Clear();
            CheckOrderedMultiDictionaryContents(dict2,
                new string[] { "S" },
                new double[][] {new double[] { -9 } },
                "foo", 3.5, null, null);

            dict2 = dict1.Clone();
            dict2.RangeTo("gib", false).Clear();
            CheckOrderedMultiDictionaryContents(dict2,
                new string[] { "gib", "S" },
                new double[][] {new double[] { 1.1, 7.1 }, new double[] { -9 } },
                "foo", 3.5, null, null);

            dict2 = dict1.Clone();
            dict2.RangeFrom("gib", false).Clear();
            CheckOrderedMultiDictionaryContents(dict2,
                new string[] { null, "bar", "foo", "gib"},
                new double[][] { new double[] { 5.5, 11.1 }, new double[] { 9.8 }, new double[] { -1.2, 3.5, 8.8 }, new double[] { 1.1, 7.1 } },
                "S", 3.7, null, null);

            dict2 = dict1.Clone();
            dict2.RangeFrom("gib", true).Clear();
            CheckOrderedMultiDictionaryContents(dict2,
                new string[] { null, "bar", "foo" },
                new double[][] { new double[] { 5.5, 11.1 }, new double[] { 9.8 }, new double[] { -1.2, 3.5, 8.8 } },
                "S", 3.7, null, null);

            dict2 = dict1.Clone();
            dict2.Range(null, true, "S", true).Clear();
            CheckOrderedMultiDictionaryContents(dict2,
                new string[] { },
                new double[][] { },
                "S", 3.7, null, null);

            dict2 = dict1.Clone();
            dict2.Reversed().Clear();
            CheckOrderedMultiDictionaryContents(dict2,
                new string[] { },
                new double[][] {  },
                "S", 3.7, null, null);
        }

        [TestMethod]
        public void SerializeStrings()
        {
            var d = new OrderedMultiDictionary<string, double>(true, StringComparer.InvariantCultureIgnoreCase) {
                {"hEllo", 13},
                {"foo", 7},
                {"world", -9.5},
                {"hello", 11},
                {"elvis", 0.9},
                {"ELVIS", 1.4},
                {null, 1.4},
                {"FOO", 7},
                {"hello", 12}
            };

            var result = (OrderedMultiDictionary<string, double>)InterfaceTests.SerializeRoundTrip(d);

            CheckOrderedMultiDictionaryContents(result,
                new string[] { null, "eLVis", "FOO", "Hello", "WORLD" },
                new double[][] { new double[] { 1.4 }, new double[] { 0.9, 1.4 }, new double[] { 7, 7 }, new double[] { 11, 12, 13 }, new double[] { -9.5 } },
                "zippy", 123, StringComparer.InvariantCultureIgnoreCase.Equals, null);
        }

    }

}

