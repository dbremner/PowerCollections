//******************************
// Written by Peter Golde
// Copyright (c) 2004-2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement 
// contained in the file "License.txt" accompanying this file.
//******************************

#region Using directives

using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Wintellect.PowerCollections.Tests.TestPredicates;
using static Wintellect.PowerCollections.Tests.UtilTests;
#endregion

namespace Wintellect.PowerCollections.Tests
{
    [TestClass]
    public class OrderedBagTests
    {
        class ComparableClass1 : IComparable<ComparableClass1>
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

        class ComparableClass2 : IComparable
        {
            public int Value = 0;
            int IComparable.CompareTo(object other)
            {
                if (other is ComparableClass2) {
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
        class UncomparableClass1 : IComparable<ComparableClass1>
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

        [TestMethod]
        public void RandomAddDelete()
        {
            const int SIZE = 5000;
            int[] count = new int[SIZE];
            var rand = new Random();
            var bag1 = new OrderedBag<int>();
            bool b;

            // Add and delete values at random.
            for (int i = 0; i < SIZE * 10; ++i) {
                int v = rand.Next(SIZE);

                // Check that number of copies is equal.
                Assert.AreEqual(count[v], bag1.NumberOfCopies(v));
                if (count[v] > 0)
                    Assert.IsTrue(bag1.Contains(v));

                if (count[v] == 0 || rand.Next(2) == 1) {
                    // Add to the bag.
                    bag1.Add(v);
                    count[v] += 1;
                }
                else {
                    // Remove from the bag.
                    b = bag1.Remove(v);
                    Assert.IsTrue(b);
                    count[v] -= 1;
                }
            }

            // Make sure the bag has all the correct values in order.
            int c = 0;
            foreach (int x in count)
                c += x;
            Assert.AreEqual(c, bag1.Count);

            int[] vals = new int[c];
            int j = 0;
            for (int i = 0; i < count.Length; ++i) {
                for (int x = 0; x < count[i]; ++x)
                    vals[j++] = i;
            }

            int last = -1;
            int index = 0;
            foreach (int v in bag1) {
                Assert.IsTrue(v >= last);
                Assert.AreEqual(v, bag1[index]);
                if (v > last) {
                    Assert.AreEqual(index, bag1.IndexOf(v));
                    if (last > 0)
                        Assert.AreEqual(index - 1, bag1.LastIndexOf(last));
                }
                for (int i = last; i < v; ++i)
                    Assert.IsTrue(i < 0 || count[i] == 0);
                Assert.IsTrue(count[v] > 0);
                --count[v];
                last = v;
                ++index;
            }

            InterfaceTests.TestReadOnlyListGeneric(bag1.AsList(), vals, null);

            int[] array = bag1.ToArray();
            Assert.IsTrue(Algorithms.EqualCollections(array, vals));
        }

        [TestMethod]
        public void ICollectionInterface()
        {
            string[] s_array = { "Foo", "hello", "Eric", null, "Clapton", "hello", "goodbye", "C#", null };
            var bag1 = new OrderedBag<string>();

            foreach (string s in s_array)
                bag1.Add(s);

            Array.Sort(s_array);
            InterfaceTests.TestCollection((ICollection)bag1, s_array, true);
        }


        [TestMethod]
        public void GenericICollectionInterface()
        {
            string[] s_array = { "Foo", "hello", "Eric", null, "Clapton", "hello", "goodbye", "C#", null };
            var bag1 = new OrderedBag<string>();

            foreach (string s in s_array)
                bag1.Add(s);

            Array.Sort(s_array);
            InterfaceTests.TestReadWriteCollectionGeneric((ICollection<string>)bag1, s_array, true, null);
        }

        [TestMethod]
        public void Add()
        {
            var bag1 = new OrderedBag<string>(StringComparer.InvariantCultureIgnoreCase) {
                "Hello",
                "foo",
                "",
                "HELLO",
                "foo",
                null,
                "hello",
                "Eric",
                null
            };

            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new string[] { null, null, "", "Eric", "foo", "foo", "Hello", "HELLO", "hello" }, true, null);
        }

        [TestMethod]
        public void GetItemByIndex()
        {
            var bag1 = new OrderedBag<string>(StringComparer.InvariantCultureIgnoreCase) {
                "Hello",
                "foo",
                "",
                "HELLO",
                "foo",
                null,
                "hello",
                "Eric",
                null
            };

            Assert.AreEqual(bag1[0], null);
            Assert.AreEqual(bag1[1], null);
            Assert.AreEqual(bag1[2], "");
            Assert.AreEqual(bag1[3], "Eric");
            Assert.AreEqual(bag1[4], "foo");
            Assert.AreEqual(bag1[5], "foo");
            Assert.AreEqual(bag1[6], "Hello");
            Assert.AreEqual(bag1[7], "HELLO");
            Assert.AreEqual(bag1[8], "hello");

            var invalidIndices = new[] { -1, 9, Int32.MaxValue, Int32.MinValue, };
            foreach (var invalidIndex in invalidIndices) {
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => bag1[invalidIndex]);
            }
        }

        [TestMethod]
        public void IndexOf()
        {
            var bag1 = new OrderedBag<string>(StringComparer.InvariantCultureIgnoreCase) {
                "Hello",
                "foo",
                "",
                "HELLO",
                "foo",
                null,
                "hello",
                "Eric",
                null
            };

            Assert.AreEqual(0, bag1.IndexOf(null));
            Assert.AreEqual(1, bag1.LastIndexOf(null));
            Assert.AreEqual(2, bag1.IndexOf(""));
            Assert.AreEqual(2, bag1.LastIndexOf(""));
            Assert.AreEqual(3, bag1.IndexOf("eric"));
            Assert.AreEqual(3, bag1.LastIndexOf("Eric"));
            Assert.AreEqual(4, bag1.IndexOf("foo"));
            Assert.AreEqual(5, bag1.LastIndexOf("Foo"));
            Assert.AreEqual(6, bag1.IndexOf("heLlo"));
            Assert.AreEqual(8, bag1.LastIndexOf("hEllo"));
        }

        [TestMethod]
        public void AsList()
        {
            var bag1 = new OrderedBag<string>(StringComparer.InvariantCultureIgnoreCase) {
                "Hello",
                "foo",
                "",
                "HELLO",
                "foo",
                null,
                "hello",
                "Eric",
                null
            };

            InterfaceTests.TestReadOnlyListGeneric(bag1.AsList(), new string[] { null, null, "", "Eric", "foo", "foo", "Hello", "HELLO", "hello" }, null, StringComparer.InvariantCultureIgnoreCase.Equals);

            var bag2 = new OrderedBag<string>(StringComparer.InvariantCultureIgnoreCase);
            InterfaceTests.TestReadOnlyListGeneric(bag2.AsList(), new string[] { }, null);

        }

        [TestMethod]
        public void CountAndClear()
        {
            var bag1 = new OrderedBag<string>(StringComparer.InvariantCultureIgnoreCase);

            Assert.AreEqual(0, bag1.Count);
            bag1.Add("hello"); Assert.AreEqual(1, bag1.Count);
            bag1.Add("foo"); Assert.AreEqual(2, bag1.Count);
            bag1.Add(""); Assert.AreEqual(3, bag1.Count);
            bag1.Add("HELLO"); Assert.AreEqual(4, bag1.Count);
            bag1.Add("foo"); Assert.AreEqual(5, bag1.Count);
            bag1.Remove(""); Assert.AreEqual(4, bag1.Count);
            bag1.Add(null); Assert.AreEqual(5, bag1.Count);
            bag1.Add("Hello"); Assert.AreEqual(6, bag1.Count);
            bag1.Add("Eric"); Assert.AreEqual(7, bag1.Count);
            bag1.RemoveAllCopies("hElLo"); Assert.AreEqual(4, bag1.Count);
            bag1.Add(null); Assert.AreEqual(5, bag1.Count);
            bag1.Clear();
            Assert.AreEqual(0, bag1.Count);

            bool found = false;
            foreach (string unused in bag1)
                found = true;

            Assert.IsFalse(found);
        }

        [TestMethod]
        public void Remove()
        {
            var bag1 = new OrderedBag<string>(StringComparer.InvariantCultureIgnoreCase);
            bool b;

            b = bag1.Remove("Eric"); Assert.IsFalse(b);
            bag1.Add("hello"); 
            bag1.Add("foo"); 
            bag1.Add(null);
            bag1.Add(null);
            bag1.Add("HELLO");
            bag1.Add("Hello");
            b = bag1.Remove("HELLO"); Assert.IsTrue(b);
            InterfaceTests.TestEnumerableElements(bag1, new string[] {null, null, "foo", "hello", "HELLO"});
            b = bag1.Remove("Hello"); Assert.IsTrue(b);
            b = bag1.Remove(null); Assert.IsTrue(b);
            b = bag1.Remove(null); Assert.IsTrue(b);
            b = bag1.Remove(null); Assert.IsFalse(b);
            bag1.Add("Hello");
            bag1.Add("Eric"); 
            bag1.Add(null); 
            b = bag1.Remove(null); Assert.IsTrue(b);
            bag1.Add("ERIC"); 
            b = bag1.Remove("eRic"); Assert.IsTrue(b);
            b = bag1.Remove("eRic"); Assert.IsTrue(b);
            bag1.Clear();
            b = bag1.Remove(""); Assert.IsFalse(b);
        }

        [TestMethod]
        public void RemoveAllCopies()
        {
            var bag1 = new OrderedBag<string>(StringComparer.InvariantCultureIgnoreCase);
            int i;

            i = bag1.RemoveAllCopies("Eric"); Assert.AreEqual(0, i);
            bag1.Add("hello");
            bag1.Add("foo");
            bag1.Add(null);
            bag1.Add(null);
            bag1.Add("hello");
            bag1.Add(null);
            i = bag1.RemoveAllCopies("HELLO"); Assert.AreEqual(2, i);
            i = bag1.RemoveAllCopies("Hello"); Assert.AreEqual(0, i);
            i = bag1.RemoveAllCopies(null); Assert.AreEqual(3, i);
            bag1.Add("Hello");
            bag1.Add("Eric");
            bag1.Add(null);
            i = bag1.RemoveAllCopies(null); Assert.AreEqual(1, i);
            bag1.Add("ERIC");
            i = bag1.RemoveAllCopies("eRic"); Assert.AreEqual(2, i);
        }

        [TestMethod]
        public void GetEqualItems()
        {
            var bag1 = new OrderedBag<string>(
                new string[] { "foo", null, "FOO", "Eric", "eric", "bar", null, "foO", "ERIC", "eric", null },
                StringComparer.InvariantCultureIgnoreCase);

            InterfaceTests.TestEnumerableElements(bag1.GetEqualItems("foo"), new string[] { "foo", "FOO", "foO" });
            InterfaceTests.TestEnumerableElements(bag1.GetEqualItems(null), new string[] { null, null, null });
            InterfaceTests.TestEnumerableElements(bag1.GetEqualItems("silly"), new string[] {  });
            InterfaceTests.TestEnumerableElements(bag1.GetEqualItems("ERic"), new string[] { "Eric", "eric", "ERIC", "eric" });
        }


        [TestMethod]
        public void ToArray()
        {
            string[] s_array = { null, "Foo", "Eric", null, "Clapton", "hello", "Clapton", "goodbye", "C#" };
            var bag1 = new OrderedBag<string>();

            string[] a1 = bag1.ToArray();
            Assert.IsNotNull(a1);
            Assert.AreEqual(0, a1.Length);

            foreach (string s in s_array)
                bag1.Add(s);
            string[] a2 = bag1.ToArray();

            Array.Sort(s_array);

            Assert.AreEqual(s_array.Length, a2.Length);
            for (int i = 0; i < s_array.Length; ++i)
                Assert.AreEqual(s_array[i], a2[i]);
        }

        [TestMethod]
        public void AddMany()
        {
            var bag1 = new OrderedBag<string>(StringComparer.InvariantCultureIgnoreCase) {
                "foo",
                "Eric",
                "Clapton"
            };
            string[] s_array = { "FOO", "x", "elmer", "fudd", "Clapton", null };
            bag1.AddMany(s_array);

            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new string[] { null, "Clapton", "Clapton", "elmer", "Eric", "foo", "FOO", "fudd", "x" }, true, null);

            bag1.Clear();
            bag1.Add("foo");
            bag1.Add("Eric");
            bag1.AddMany(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new string[] { "Eric", "Eric", "foo", "foo" }, true, null);
        }

        [TestMethod]
        public void RemoveMany()
        {
            var bag1 = new OrderedBag<string>(StringComparer.InvariantCultureIgnoreCase) {
                "foo",
                "Eric",
                "Clapton",
                null,
                "Foo",
                "fudd",
                "elmer"
            };
            string[] s_array = { "FOO", "jasmine", "eric", null };
            int count = bag1.RemoveMany(s_array);
            Assert.AreEqual(3, count);

            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new string[] { "Clapton", "elmer", "foo", "fudd" }, true, null);

            bag1.Clear();
            bag1.Add("foo");
            bag1.Add("Eric");
            bag1.Add("Clapton");
            bag1.Add(null);
            bag1.Add("Foo");
            count = bag1.RemoveMany(bag1);
            Assert.AreEqual(5, count);
            Assert.AreEqual(0, bag1.Count);
        }

        [TestMethod]
        public void RemoveAll()
        {
            var bag1 = new OrderedBag<double>(new double[] { 4.5, 187.4, 1.2, 7.6, -7.6, -0.04, 1.2, 1.78, 10.11, 187.4 });

            bag1.RemoveAll(AbsOver5);
            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new double[] { -0.04, 1.2, 1.2, 1.78, 4.5 }, true, null);

            bag1 = new OrderedBag<double>(new double[] { 4.5, 187.4, 1.2, 7.6, -7.6, -0.04, 1.2, 1.78, 10.11, 187.4 });
            bag1.RemoveAll(IsZero);
            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new double[] { -7.6, -0.04, 1.2, 1.2, 1.78, 4.5, 7.6, 10.11, 187.4, 187.4 }, true, null);

            bag1 = new OrderedBag<double>(new double[] { 4.5, 187.4, 1.2, 7.6, -7.6, -0.04, 1.2, 1.78, 10.11, 187.4 });
            bag1.RemoveAll(Under200);
            Assert.AreEqual(0, bag1.Count);
        }

        [TestMethod]
        public void IsDisjointFrom()
        {
            var bag1 = new OrderedBag<int>(new int[] { 3, 6, 7, 1, 1, 11, 9, 3, 8 });
            var bag2 = new OrderedBag<int>();
            var bag3 = new OrderedBag<int>();
            var bag4 = new OrderedBag<int>(new int[] { 8, 9, 1, 8, 3, 7, 6, 11, 7 });
            var bag5 = new OrderedBag<int>(new int[] { 17, 3, 12, 10, 22 });
            var bag6 = new OrderedBag<int>(new int[] { 14, 19, 14, 0, 2, 14 });

            Assert.IsFalse(bag1.IsDisjointFrom(bag1));
            Assert.IsTrue(bag2.IsDisjointFrom(bag2));

            Assert.IsTrue(bag1.IsDisjointFrom(bag2));
            Assert.IsTrue(bag2.IsDisjointFrom(bag1));

            Assert.IsTrue(bag2.IsDisjointFrom(bag3));
            Assert.IsTrue(bag3.IsDisjointFrom(bag2));

            Assert.IsFalse(bag1.IsDisjointFrom(bag4));
            Assert.IsFalse(bag4.IsDisjointFrom(bag1));

            Assert.IsFalse(bag1.IsDisjointFrom(bag5));
            Assert.IsFalse(bag5.IsDisjointFrom(bag1));

            Assert.IsTrue(bag1.IsDisjointFrom(bag6));
            Assert.IsTrue(bag6.IsDisjointFrom(bag1));

            Assert.IsTrue(bag5.IsDisjointFrom(bag6));
            Assert.IsTrue(bag6.IsDisjointFrom(bag5));
        }

        [TestMethod]
        public void Intersection()
        {
            var bagOdds = new OrderedBag<int>(new int[] { 1, 1, 1, 3, 3, 3, 5, 7, 7, 9, 11, 11, 13, 15, 17, 17, 19 });
            var bagDigits = new OrderedBag<int>(new int[] { 1, 2, 2, 3, 3, 3, 4, 5, 5, 6, 7, 7, 7, 7, 7, 7, 8, 9 });
            OrderedBag<int> bag1, bag2, bag3;

            // Algorithms work different depending on sizes, so try both ways.
            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag1.IntersectionWith(bag2);
            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new int[] { 1, 3, 3, 3, 5, 7, 7, 9 }, true, null);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag2.IntersectionWith(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag2, new int[] { 1, 3, 3, 3, 5, 7, 7, 9 }, true, null);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag3 = bag1.Intersection(bag2);
            InterfaceTests.TestReadWriteCollectionGeneric(bag3, new int[] { 1, 3, 3, 3, 5, 7, 7, 9 }, true, null);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag3 = bag2.Intersection(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag3, new int[] { 1, 3, 3, 3, 5, 7, 7, 9 }, true, null);

            // Make sure intersection with itself works.
            bag1 = bagDigits.Clone();
            bag1.IntersectionWith(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new int[] { 1, 2, 2, 3, 3, 3, 4, 5, 5, 6, 7, 7, 7, 7, 7, 7, 8, 9 }, true, null);

            bag1 = bagDigits.Clone();
            bag3 = bag1.Intersection(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag3, new int[] { 1, 2, 2, 3, 3, 3, 4, 5, 5, 6, 7, 7, 7, 7, 7, 7, 8, 9 }, true, null);
        }

        [TestMethod]
        public void Union()
        {
            var bagOdds = new OrderedBag<int>(new int[] { 1, 1, 1, 3, 3, 3, 5, 7, 7, 9, 11, 11, 13, 15, 17, 17, 19 });
            var bagDigits = new OrderedBag<int>(new int[] { 1, 2, 2, 3, 3, 3, 4, 5, 5, 6, 7, 7, 7, 7, 7, 7, 8, 9 });
            OrderedBag<int> bag1, bag2, bag3;

            // Algorithms work different depending on sizes, so try both ways.
            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag1.UnionWith(bag2);
            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new int[] { 1, 1, 1, 2, 2, 3, 3, 3, 4, 5, 5, 6, 7, 7, 7, 7, 7, 7, 8, 9, 11, 11, 13, 15, 17, 17, 19 }, true, null);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag2.UnionWith(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag2, new int[] { 1, 1, 1, 2, 2, 3, 3, 3, 4, 5, 5, 6, 7, 7, 7, 7, 7, 7, 8, 9, 11, 11, 13, 15, 17, 17, 19 }, true, null);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag3 = bag1.Union(bag2);
            InterfaceTests.TestReadWriteCollectionGeneric(bag3, new int[] { 1, 1, 1, 2, 2, 3, 3, 3, 4, 5, 5, 6, 7, 7, 7, 7, 7, 7, 8, 9, 11, 11, 13, 15, 17, 17, 19 }, true, null);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag3 = bag2.Union(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag3, new int[] { 1, 1, 1, 2, 2, 3, 3, 3, 4, 5, 5, 6, 7, 7, 7, 7, 7, 7, 8, 9, 11, 11, 13, 15, 17, 17, 19 }, true);

            // Make sure intersection with itself works.
            bag1 = bagDigits.Clone();
            bag1.UnionWith(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new int[] { 1, 2, 2, 3, 3, 3, 4, 5, 5, 6, 7, 7, 7, 7, 7, 7, 8, 9 }, true);

            bag1 = bagDigits.Clone();
            bag3 = bag1.Union(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag3, new int[] { 1, 2, 2, 3, 3, 3, 4, 5, 5, 6, 7, 7, 7, 7, 7, 7, 8, 9 }, true);
        }

        [TestMethod]
        public void Sum()
        {
            var bagOdds = new OrderedBag<int>(new int[] { 1, 1, 1, 3, 3, 3, 5, 7, 7, 9, 11, 11, 13, 15, 17, 17, 19 });
            var bagDigits = new OrderedBag<int>(new int[] { 1, 2, 2, 3, 3, 3, 4, 5, 5, 6, 7, 7, 7, 7, 7, 7, 8, 9 });
            OrderedBag<int> bag1, bag2, bag3;

            // Algorithms work different depending on sizes, so try both ways.
            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag1.SumWith(bag2);
            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new int[] { 1, 1, 1, 1, 2, 2, 3, 3, 3, 3, 3, 3, 4, 5, 5, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9, 9, 11, 11, 13, 15, 17, 17, 19 }, true);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag2.SumWith(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag2, new int[] { 1, 1, 1, 1, 2, 2, 3, 3, 3, 3, 3, 3, 4, 5, 5, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9, 9, 11, 11, 13, 15, 17, 17, 19 }, true);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag3 = bag1.Sum(bag2);
            InterfaceTests.TestReadWriteCollectionGeneric(bag3, new int[] { 1, 1, 1, 1, 2, 2, 3, 3, 3, 3, 3, 3, 4, 5, 5, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9, 9, 11, 11, 13, 15, 17, 17, 19 }, true);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag3 = bag2.Sum(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag3, new int[] { 1, 1, 1, 1, 2, 2, 3, 3, 3, 3, 3, 3, 4, 5, 5, 5, 6, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9, 9, 11, 11, 13, 15, 17, 17, 19 }, true);

            // Make sure intersection with itself works.
            bag1 = bagDigits.Clone();
            bag1.SumWith(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new int[] { 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 5, 5, 5, 5, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 8, 9, 9 }, true);

            bag1 = bagDigits.Clone();
            bag3 = bag1.Sum(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag3, new int[] { 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 5, 5, 5, 5, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 8, 9, 9 }, true);
        }

        [TestMethod]
        public void SymmetricDifference()
        {
            var bagOdds = new OrderedBag<int>(new int[] { 1, 1, 1, 3, 3, 3, 5, 7, 7, 9, 11, 11, 13, 15, 17, 17, 19 });
            var bagDigits = new OrderedBag<int>(new int[] { 1, 2, 2, 3, 3, 3, 4, 5, 5, 6, 7, 7, 7, 7, 7, 7, 8, 9 });
            OrderedBag<int> bag1, bag2, bag3;

            // Algorithms work different depending on sizes, so try both ways.
            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag1.SymmetricDifferenceWith(bag2);
            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new int[] { 1, 1, 2, 2, 4, 5, 6, 7, 7, 7, 7, 8, 11, 11, 13, 15, 17, 17, 19 }, true);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag2.SymmetricDifferenceWith(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag2, new int[] { 1, 1, 2, 2, 4, 5, 6, 7, 7, 7, 7, 8, 11, 11, 13, 15, 17, 17, 19 }, true);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag3 = bag1.SymmetricDifference(bag2);
            InterfaceTests.TestReadWriteCollectionGeneric(bag3, new int[] { 1, 1, 2, 2, 4, 5, 6, 7, 7, 7, 7, 8, 11, 11, 13, 15, 17, 17, 19 }, true);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag3 = bag2.SymmetricDifference(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag3, new int[] { 1, 1, 2, 2, 4, 5, 6, 7, 7, 7, 7, 8, 11, 11, 13, 15, 17, 17, 19 }, true);

            // Make sure intersection with itself works.
            bag1 = bagDigits.Clone();
            bag1.SymmetricDifferenceWith(bag1);
            Assert.AreEqual(0, bag1.Count);

            bag1 = bagDigits.Clone();
            bag3 = bag1.SymmetricDifference(bag1);
            Assert.AreEqual(0, bag3.Count);
        }

        [TestMethod]
        public void Difference()
        {
            var bagOdds = new OrderedBag<int>(new int[] { 1, 1, 1, 3, 3, 3, 5, 7, 7, 9, 11, 11, 13, 15, 17, 17, 19 });
            var bagDigits = new OrderedBag<int>(new int[] { 1, 2, 2, 3, 3, 3, 4, 5, 5, 6, 7, 7, 7, 7, 7, 7, 8, 9 });
            OrderedBag<int> bag1, bag2, bag3;

            // Algorithms work different depending on sizes, so try both ways.
            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag1.DifferenceWith(bag2);
            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new int[] { 1, 1, 11, 11, 13, 15, 17, 17, 19 }, true);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag2.DifferenceWith(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag2, new int[] { 2, 2, 4, 5, 6, 7, 7, 7, 7, 8 }, true);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag3 = bag1.Difference(bag2);
            InterfaceTests.TestReadWriteCollectionGeneric(bag3, new int[] { 1, 1, 11, 11, 13, 15, 17, 17, 19 }, true);

            bag1 = bagOdds.Clone(); bag2 = bagDigits.Clone();
            bag3 = bag2.Difference(bag1);
            InterfaceTests.TestReadWriteCollectionGeneric(bag3, new int[] { 2, 2, 4, 5, 6, 7, 7, 7, 7, 8 }, true);

            // Make sure intersection with itself works.
            bag1 = bagDigits.Clone();
            bag1.DifferenceWith(bag1);
            Assert.AreEqual(0, bag1.Count);

            bag1 = bagDigits.Clone();
            bag3 = bag1.Difference(bag1);
            Assert.AreEqual(0, bag3.Count);
        }

        [TestMethod]
        public void Subset()
        {
            var set1 = new OrderedBag<int>(new int[] { 1, 1, 3, 6, 6, 6, 6, 7, 8, 9, 9 });
            var set2 = new OrderedBag<int>();
            var set3 = new OrderedBag<int>(new int[] { 1, 6, 6, 9, 9 });
            var set4 = new OrderedBag<int>(new int[] { 1, 6, 6, 9, 9 });
            var set5 = new OrderedBag<int>(new int[] { 1, 1, 3, 6, 6, 6, 7, 7, 8, 9, 9 });

            Assert.IsTrue(set1.IsSupersetOf(set2));
            Assert.IsTrue(set2.IsSubsetOf(set1));
            Assert.IsTrue(set1.IsProperSupersetOf(set2));
            Assert.IsTrue(set2.IsProperSubsetOf(set1));

            Assert.IsTrue(set1.IsSupersetOf(set3));
            Assert.IsTrue(set3.IsSubsetOf(set1));
            Assert.IsTrue(set1.IsProperSupersetOf(set3));
            Assert.IsTrue(set3.IsProperSubsetOf(set1));

            Assert.IsFalse(set3.IsSupersetOf(set1));
            Assert.IsFalse(set1.IsSubsetOf(set3));
            Assert.IsFalse(set3.IsProperSupersetOf(set1));
            Assert.IsFalse(set1.IsProperSubsetOf(set3));

            Assert.IsFalse(set1.IsSupersetOf(set5));
            Assert.IsFalse(set5.IsSupersetOf(set1));
            Assert.IsFalse(set1.IsSubsetOf(set5));
            Assert.IsFalse(set5.IsSubsetOf(set1));
            Assert.IsFalse(set1.IsProperSupersetOf(set5));
            Assert.IsFalse(set5.IsProperSupersetOf(set1));
            Assert.IsFalse(set1.IsProperSubsetOf(set5));
            Assert.IsFalse(set5.IsProperSubsetOf(set1));

            Assert.IsTrue(set3.IsSupersetOf(set4));
            Assert.IsTrue(set3.IsSubsetOf(set4));
            Assert.IsFalse(set3.IsProperSupersetOf(set4));
            Assert.IsFalse(set3.IsProperSubsetOf(set4));

            Assert.IsTrue(set1.IsSupersetOf(set1));
            Assert.IsTrue(set1.IsSubsetOf(set1));
            Assert.IsFalse(set1.IsProperSupersetOf(set1));
            Assert.IsFalse(set1.IsProperSubsetOf(set1));

            Assert.ThrowsException<ArgumentNullException>(() => set1.IsSubsetOf(null));

            Assert.ThrowsException<ArgumentNullException>(() => set1.IsProperSubsetOf(null));
        }

        [TestMethod]
        public void IsEqualTo()
        {
            var bag1 = new OrderedBag<int>(new int[] { 11, 6, 9, 7, 1, 11, 9, 3, 7, 8, 7 });
            var bag2 = new OrderedBag<int>();
            var bag3 = new OrderedBag<int>();
            var bag4 = new OrderedBag<int>(new int[] { 9, 11, 1, 3, 7, 6, 7, 8, 9, 14, 7 });
            var bag5 = new OrderedBag<int>(new int[] { 11, 7, 6, 9, 8, 3, 7, 1, 11, 9, 3 });
            var bag6 = new OrderedBag<int>(new int[] { 11, 1, 9, 3, 6, 7, 8, 7, 10, 7, 11, 9 });
            var bag7 = new OrderedBag<int>(new int[] { 9, 7, 1, 9, 11, 8, 3, 7, 7, 6, 11 });

            Assert.IsTrue(bag1.IsEqualTo(bag1));
            Assert.IsTrue(bag2.IsEqualTo(bag2));

            Assert.IsTrue(bag2.IsEqualTo(bag3));
            Assert.IsTrue(bag3.IsEqualTo(bag2));

            Assert.IsTrue(bag1.IsEqualTo(bag7));
            Assert.IsTrue(bag7.IsEqualTo(bag1));

            Assert.IsFalse(bag1.IsEqualTo(bag2));
            Assert.IsFalse(bag2.IsEqualTo(bag1));

            Assert.IsFalse(bag1.IsEqualTo(bag4));
            Assert.IsFalse(bag4.IsEqualTo(bag1));

            Assert.IsFalse(bag1.IsEqualTo(bag5));
            Assert.IsFalse(bag5.IsEqualTo(bag1));

            Assert.IsFalse(bag1.IsEqualTo(bag6));
            Assert.IsFalse(bag6.IsEqualTo(bag1));

            Assert.IsFalse(bag5.IsEqualTo(bag6));
            Assert.IsFalse(bag6.IsEqualTo(bag5));

            Assert.IsFalse(bag5.IsEqualTo(bag7));
            Assert.IsFalse(bag7.IsEqualTo(bag5));
        }

        [TestMethod]
        public void Clone()
        {
            var bag1 = new OrderedBag<int>(new int[] { 1, 7, 9, 11, 7, 13, 15, -17, 19, -21, 1 });

            OrderedBag<int> bag2 = bag1.Clone();

            Assert.IsFalse(bag2 == bag1);

            // Modify bag1, make sure bag2, bag3 don't change.
            bag1.Remove(9);
            bag1.Remove(-17);
            bag1.Add(8);

            InterfaceTests.TestReadWriteCollectionGeneric(bag2, new int[] { -21, -17, 1, 1, 7, 7, 9, 11, 13, 15, 19 }, true);
        }

        [TestMethod]
        public void InconsistentComparisons1()
        {
            var bagOdds = new OrderedBag<int>(new int[] { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25 });
            var bagDigits = new OrderedBag<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, ComparersTests.CompareOddEven);
            Assert.ThrowsException<InvalidOperationException>(() => bagOdds.UnionWith(bagDigits));
        }

        [TestMethod]
        public void InconsistentComparisons2()
        {
            var bagOdds = new OrderedBag<int>(new int[] { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25 });
            var bagDigits = new OrderedBag<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new GOddEvenComparer());
            Assert.ThrowsException<InvalidOperationException>(() => bagOdds.SymmetricDifferenceWith(bagDigits));
        }

        [TestMethod]
        public void InconsistentComparisons3()
        {
            var bag1 = new OrderedBag<string>(new string[] { "foo", "Bar" }, StringComparer.CurrentCulture);
            var bag2 = new OrderedBag<string>(new string[] { "bada", "bing" }, StringComparer.InvariantCulture);
            Assert.ThrowsException<InvalidOperationException>(() => bag1.Intersection(bag2));
        }

        [TestMethod]
        public void ConsistentComparisons()
        {
            var bagOdds = new OrderedBag<int>(new int[] { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25 }, ComparersTests.CompareOddEven);
            var bagDigits = new OrderedBag<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, ComparersTests.CompareOddEven);
            bagOdds.UnionWith(bagDigits);

            var bag1 = new OrderedBag<string>(new string[] { "foo", "Bar" }, StringComparer.InvariantCulture);
            var bag2 = new OrderedBag<string>(new string[] { "bada", "bing" }, StringComparer.InvariantCulture);
            bag1.Difference(bag2);
        }


        [TestMethod]
        public void NotComparable1()
        {
            Assert.ThrowsException<InvalidOperationException>(
                () => new OrderedBag<UncomparableClass1>());
        }

        [TestMethod]
        public void NotComparable2()
        {
            Assert.ThrowsException<InvalidOperationException>(
                () => new OrderedBag<UncomparableClass2>());
        }

        [TestMethod]
        public void FailFastEnumerator1()
        {
            var bag1 = new OrderedBag<double>();

            double d = 1.218034;
            for (int i = 0; i < 50; ++i) {
                bag1.Add(d);
                d = d * 1.3451 - .31;
            }

            // should throw once the bag is modified.
            void InvalidOperation()
            {
                foreach (double k in bag1) {
                    if (k > 3.0)
                        bag1.Add(1.0);
                }
            }

            Assert.ThrowsException<InvalidOperationException>(() => InvalidOperation());
        }

        [TestMethod]
        public void FailFastEnumerator2()
        {
            var bag1 = new OrderedBag<double>();

            double d = 1.218034;
            for (int i = 0; i < 50; ++i) {
                bag1.Add(d);
                d = d * 1.3451 - .31;
            }

            // should throw once the bag is modified.
            void InvalidOperation()
            {
                foreach (double k in bag1) {
                    if (k > 3.0)
                        bag1.Clear();
                }
            }

            Assert.ThrowsException<InvalidOperationException>(() => InvalidOperation());
        }

        // Check a View to make sure it has the right stuff.
        private void CheckView<T>(OrderedBag<T>.View view, T[] items, T nonItem)
        {
            Assert.AreEqual(items.Length, view.Count);

            T[] array = view.ToArray();      // Check ToArray
            Assert.AreEqual(items.Length, array.Length);
            for (int i = 0; i < items.Length; ++i) {
                Assert.AreEqual(items[i], array[i]);
                Assert.AreEqual(items[i], view[i]);
                int index = view.IndexOf(items[i]);
                Assert.IsTrue(i == index || index < i && object.Equals(items[index], items[i]));
                index = view.LastIndexOf(items[i]);
                Assert.IsTrue(i == index || index > i && object.Equals(items[index], items[i]));
            }

            if (items.Length > 0) {
                Assert.AreEqual(items[0], view.GetFirst());
                Assert.AreEqual(items[items.Length - 1], view.GetLast());
            }
            else {
                Assert.ThrowsException<InvalidOperationException>(() => view.GetFirst());

                Assert.ThrowsException<InvalidOperationException>(() => view.GetLast());
            }

            Assert.IsFalse(view.Contains(nonItem));
            Assert.IsTrue(view.IndexOf(nonItem) < 0);
            Assert.IsTrue(view.LastIndexOf(nonItem) < 0);
            InterfaceTests.TestCollection((ICollection)view, items, true);
            InterfaceTests.TestReadOnlyListGeneric(view.AsList(), items, null);
            Array.Reverse(items);
            InterfaceTests.TestCollection((ICollection)view.Reversed(), items, true);
            InterfaceTests.TestReadOnlyListGeneric(view.Reversed().AsList(), items, null);
            Array.Reverse(items);
            InterfaceTests.TestReadWriteCollectionGeneric((ICollection<T>)view, items, true);
        }

        // Check Range methods.
        [TestMethod]
        public void Range()
        {
            var bag1 = new OrderedBag<int>(new int[] { 1, 1, 1, 3, 4, 4, 6, 8, 8, 9, 11, 14, 22 });

            CheckView(bag1.Clone().Range(4, true, 11, false), new int[] { 4, 4, 6, 8, 8, 9 }, 11);
            CheckView(bag1.Clone().Range(4, false, 11, false), new int[] { 6, 8, 8, 9 }, 11);
            CheckView(bag1.Clone().Range(4, true, 11, true), new int[] { 4, 4, 6, 8, 8, 9, 11 }, 14);
            CheckView(bag1.Clone().Range(4, false, 11, true), new int[] { 6, 8, 8, 9, 11 }, 4);
            CheckView(bag1.Clone().Range(4, true, 4, false), new int[] { }, 4);
            CheckView(bag1.Clone().Range(4, true, 4, true), new int[] { 4, 4}, 6);
            CheckView(bag1.Clone().Range(4, false, 4, true), new int[] {  }, 4);
            CheckView(bag1.Clone().Range(11, true, 4, false), new int[] { }, 6);
            CheckView(bag1.Clone().Range(0, true, 100, false), new int[] { 1, 1, 1, 3, 4, 4, 6, 8, 8, 9, 11, 14, 22 }, 0);
            CheckView(bag1.Clone().Range(0, false, 100, true), new int[] { 1, 1, 1, 3, 4, 4, 6, 8, 8, 9, 11, 14, 22 }, 0);
            CheckView(bag1.Clone().Range(1, true, 14, false), new int[] { 1, 1, 1, 3, 4, 4, 6, 8, 8, 9, 11 }, 14);
            CheckView(bag1.Clone().Range(1, true, 14, true), new int[] { 1, 1, 1, 3, 4, 4, 6, 8, 8, 9, 11, 14 }, 22);
            CheckView(bag1.Clone().Range(1, false, 14, true), new int[] { 3, 4, 4, 6, 8, 8, 9, 11, 14 }, 22);
            CheckView(bag1.Clone().Range(1, true, 15, false), new int[] { 1, 1, 1, 3, 4, 4, 6, 8, 8, 9, 11, 14 }, 22);
            CheckView(bag1.Clone().Range(2, true, 15, false), new int[] { 3, 4, 4, 6, 8, 8, 9, 11, 14 }, 1);
            CheckView(bag1.Clone().RangeFrom(9, true), new int[] { 9, 11, 14, 22 }, 8);
            CheckView(bag1.Clone().RangeFrom(9, false), new int[] { 11, 14, 22 }, 9);
            CheckView(bag1.Clone().RangeFrom(1, true), new int[] { 1, 1, 1, 3, 4, 4, 6, 8, 8, 9, 11, 14, 22 }, 0);
            CheckView(bag1.Clone().RangeFrom(1, false), new int[] { 3, 4, 4, 6, 8, 8, 9, 11, 14, 22 }, 1);
            CheckView(bag1.Clone().RangeFrom(100, true), new int[] { }, 1);
            CheckView(bag1.Clone().RangeTo(9, false), new int[] { 1, 1, 1, 3, 4, 4, 6, 8, 8 }, 9);
            CheckView(bag1.Clone().RangeTo(9, true), new int[] { 1, 1, 1, 3, 4, 4, 6, 8, 8, 9 }, 11);
            CheckView(bag1.Clone().RangeTo(1, false), new int[] { }, 1);
            CheckView(bag1.Clone().RangeTo(1, true), new int[] { 1, 1, 1}, 3);
            CheckView(bag1.Clone().RangeTo(100, false), new int[] { 1, 1, 1, 3, 4, 4, 6, 8, 8, 9, 11, 14, 22 }, 0);
            CheckView(bag1.Clone().RangeTo(100, true), new int[] { 1, 1, 1, 3, 4, 4, 6, 8, 8, 9, 11, 14, 22 }, 0);
        }

        // Check Range methods.
        [TestMethod]
        public void Reversed()
        {
            var bag1 = new OrderedBag<int>(new int[] { 1, 1, 1, 3, 4, 4, 6, 8, 8, 9, 11, 14, 22 });

            CheckView(bag1.Reversed(), new int[] { 22, 14, 11, 9, 8, 8, 6, 4, 4, 3, 1, 1, 1 }, 0);
        }

        [TestMethod]
        public void ViewClear()
        {
            var bag1 = new OrderedBag<int>(new int[] { 1, 1, 1, 3, 4, 4, 6, 8, 8, 9, 11, 14, 22 });

            bag1.Range(6, true, 11, false).Clear();
            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new int[] { 1, 1, 1, 3, 4, 4, 11, 14, 22 }, true);
        }

        [TestMethod]
        public void ViewAddException1()
        {
            var bag1 = new OrderedBag<int>(new int[] { 1, 1, 3, 4, 6, 6, 6, 8, 9, 11, 14, 22 });

            var range = bag1.Range(3, true, 8, false);
            Assert.ThrowsException<ArgumentException>(() => range.Add(8));
        }

        [TestMethod]
        public void ViewAddException2()
        {
            var bag1 = new OrderedBag<int>(new int[] { 1, 1, 3, 4, 6, 6, 6, 8, 9, 11, 14, 22 });

            var range = bag1.Range(3, true, 8, false);
            Assert.ThrowsException<ArgumentException>(() => range.Add(2));
        }

        [TestMethod]
        public void ViewAddRemove()
        {
            var bag1 = new OrderedBag<int>(new int[] { 1, 1, 1, 3, 4, 4, 6, 8, 8, 9, 11, 14, 22 });

            Assert.IsFalse(bag1.Range(3, true, 8, false).Remove(9));
            Assert.IsTrue(bag1.Contains(9));
            bag1.Range(3, true, 8, false).Add(7);
            bag1.Range(3, true, 8, false).Add(4);
            Assert.IsTrue(bag1.Contains(7));
            Assert.AreEqual(3, bag1.NumberOfCopies(4));
            Assert.IsTrue(bag1.Range(3, true, 11, false).Reversed().Remove(4));
            Assert.AreEqual(2, bag1.NumberOfCopies(4));
        }

        // Simple class for testing cloning.
        class MyInt : ICloneable
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

        void CompareClones<T>(OrderedBag<T> s1, OrderedBag<T> s2)
        {
            IEnumerator<T> e1 = s1.GetEnumerator();
            IEnumerator<T> e2 = s2.GetEnumerator();

            // Check that the bags are equal, but not reference equals (e.g., have been cloned).
            while (e1.MoveNext()) {
                e2.MoveNext();
                if (e1.Current == null) {
                    Assert.IsNull(e2.Current);
                }
                else {
                    Assert.IsTrue(e1.Current.Equals(e2.Current));
                    Assert.IsFalse(object.ReferenceEquals(e1.Current, e2.Current));
                }
            }
        }

        [TestMethod]
        public void CloneContents()
        {
            var bag1 = new OrderedBag<MyInt>(
                delegate(MyInt v1, MyInt v2) {
                if (v1 == null) {
                    return (v2 == null) ? 0 : -1;
                }
                else if (v2 == null)
                    return 1;
                else
                    return v2.value.CompareTo(v1.value);
            });

            var mi = new MyInt(9);
            bag1.Add(new MyInt(14));
            bag1.Add(new MyInt(143));
            bag1.Add(new MyInt(2));
            bag1.Add(mi);
            bag1.Add(null);
            bag1.Add(new MyInt(14));
            bag1.Add(new MyInt(111));
            bag1.Add(mi);
            OrderedBag<MyInt> bag2 = bag1.CloneContents();
            CompareClones(bag1, bag2);

            var bag3 = new OrderedBag<int>(new int[] { 144, 1, 5, 23, 1, 8 });
            OrderedBag<int> bag4 = bag3.CloneContents();
            CompareClones(bag3, bag4);

            int Comparison(CloneableStruct s1, CloneableStruct s2) {
                return s1.value.CompareTo(s2.value);
            }

            var bag5 = new OrderedBag<CloneableStruct>(Comparison) {
                new CloneableStruct(143),
                new CloneableStruct(1),
                new CloneableStruct(23),
                new CloneableStruct(1),
                new CloneableStruct(8)
            };
            OrderedBag<CloneableStruct> bag6 = bag5.CloneContents();

            Assert.AreEqual(bag5.Count, bag6.Count);

            // Check that the bags are equal, but not identical (e.g., have been cloned via ICloneable).
            IEnumerator<CloneableStruct> e1 = bag5.GetEnumerator();
            IEnumerator<CloneableStruct> e2 = bag6.GetEnumerator();

            // Check that the bags are equal, but not reference equals (e.g., have been cloned).
            while (e1.MoveNext()) {
                e2.MoveNext();
                Assert.IsTrue(e1.Current.Equals(e2.Current));
                Assert.IsFalse(e1.Current.Identical(e2.Current));
            }
        }

        [TestMethod]
        public void CantCloneContents()
        {
            var bag1 = new OrderedBag<GenericComparable> {
                new GenericComparable(1),
                new GenericComparable(2)
            };

            Assert.ThrowsException<InvalidOperationException>(() => bag1.CloneContents());
        }

        [TestMethod]
        public void CustomComparison()
        {
            Comparison<int> myOrdering = ComparersTests.CompareOddEven;

            var bag1 = new OrderedBag<int>(myOrdering) {
                8,
                12,
                9,
                9,
                3
            };
            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new int[] { 3, 9, 9, 8, 12 }, true);
        }

        [TestMethod]
        public void CustomIComparer()
        {
            IComparer<int> myComparer = new GOddEvenComparer();

            var bag1 = new OrderedBag<int>(myComparer) {
                3,
                8,
                12,
                9,
                3
            };
            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new int[] { 3, 3, 9, 8, 12 }, true);
        }

        [TestMethod]
        public void ComparerProperty()
        {
            IComparer<int> comparer1 = new GOddEvenComparer();
            var bag1 = new OrderedBag<int>(comparer1);
            Assert.AreSame(comparer1, bag1.Comparer);
            var bag2 = new OrderedBag<decimal>();
            Assert.AreSame(Comparer<decimal>.Default, bag2.Comparer);
            var bag3 = new OrderedBag<string>(StringComparer.OrdinalIgnoreCase);
            Assert.AreSame(StringComparer.OrdinalIgnoreCase, bag3.Comparer);

            Comparison<int> comparison1 = ComparersTests.CompareOddEven;
            var bag4 = new OrderedBag<int>(comparison1);
            var bag5 = new OrderedBag<int>(comparison1);
            Assert.AreEqual(bag4.Comparer, bag5.Comparer);
            Assert.IsFalse(bag4.Comparer == bag5.Comparer);
            Assert.IsFalse(object.Equals(bag4.Comparer, bag1.Comparer));
            Assert.IsFalse(object.Equals(bag4.Comparer, Comparer<int>.Default));
            Assert.IsTrue(bag4.Comparer.Compare(7, 6) < 0);
        }

        [TestMethod]
        public void Initialize()
        {
            Comparison<int> myOrdering = ComparersTests.CompareOddEven;
            IComparer<int> myComparer = new GOddEvenComparer();
            var list = new List<int>(new int[] { 12, 3, 9, 8, 9, 3 });
            var bag1 = new OrderedBag<int>(list);
            var bag2 = new OrderedBag<int>(list, myOrdering);
            var bag3 = new OrderedBag<int>(list, myComparer);

            InterfaceTests.TestReadWriteCollectionGeneric(bag1, new int[] { 3, 3, 8, 9, 9, 12 }, true);
            InterfaceTests.TestReadWriteCollectionGeneric(bag2, new int[] { 3, 3, 9, 9, 8, 12 }, true);
            InterfaceTests.TestReadWriteCollectionGeneric(bag3, new int[] { 3, 3, 9, 9, 8, 12 }, true);
        }

        [TestMethod]
        public void DistinctItems()
        {
            var bag1 = new OrderedBag<string>(
                new string[] { "foo", null, "Foo", "Eric", "FOO", "eric", "bar" }, StringComparer.InvariantCultureIgnoreCase);

            InterfaceTests.TestEnumerableElements(bag1.DistinctItems(), new string[] { null, "bar", "Eric", "foo" });

            // Make sure enumeration stops on change.
            int count = 0;
            Assert.ThrowsException<InvalidOperationException>(() => {
                foreach (string unused in bag1.DistinctItems()) {
                    if (count == 2)
                        bag1.Add("zippy");
                    ++count;
                }
            });
            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void Smallest()
        {
            var bag1 = new OrderedBag<string>(
                new string[] { "foo", null, "Foo", "Eric", "FOO", "eric", "bar" }, StringComparer.InvariantCultureIgnoreCase);

            string s;

            Assert.AreEqual(7, bag1.Count);

            s = bag1.GetFirst();
            Assert.IsNull(s);
            s = bag1.RemoveFirst();
            Assert.IsNull(s);
            Assert.AreEqual(6, bag1.Count);

            s = bag1.GetFirst();
            Assert.AreEqual("bar", s);
            s = bag1.RemoveFirst();
            Assert.AreEqual("bar", s);
            Assert.AreEqual(5, bag1.Count);

            s = bag1.GetFirst();
            Assert.AreEqual("Eric", s);
            s = bag1.RemoveFirst();
            Assert.AreEqual("Eric", s);
            Assert.AreEqual(4, bag1.Count);

            s = bag1.GetFirst();
            Assert.AreEqual("eric", s);
            s = bag1.RemoveFirst();
            Assert.AreEqual("eric", s);
            Assert.AreEqual(3, bag1.Count);
        }

        [TestMethod]
        public void Largest()
        {
            var bag1 = new OrderedBag<string>(
                new string[] { "foo", null, "Foo", "Eric", "FOO", "eric", "bar" }, StringComparer.InvariantCultureIgnoreCase);

            string s;

            Assert.AreEqual(7, bag1.Count);

            s = bag1.GetLast();
            Assert.AreEqual("FOO", s);
            s = bag1.RemoveLast();
            Assert.AreEqual("FOO", s);
            Assert.AreEqual(6, bag1.Count);

            s = bag1.GetLast();
            Assert.AreEqual("Foo", s);
            s = bag1.RemoveLast();
            Assert.AreEqual("Foo", s);
            Assert.AreEqual(5, bag1.Count);

            s = bag1.GetLast();
            Assert.AreEqual("foo", s);
            s = bag1.RemoveLast();
            Assert.AreEqual("foo", s);
            Assert.AreEqual(4, bag1.Count);

            s = bag1.GetLast();
            Assert.AreEqual("eric", s);
            s = bag1.RemoveLast();
            Assert.AreEqual("eric", s);
            Assert.AreEqual(3, bag1.Count);
        }

        [TestMethod]
        public void SmallestLargestException()
        {
            var bag1 = new OrderedBag<string>(StringComparer.InvariantCultureIgnoreCase);

            Assert.ThrowsException<InvalidOperationException>(() => bag1.GetFirst());

            Assert.ThrowsException<InvalidOperationException>(() => bag1.GetLast());

            Assert.ThrowsException<InvalidOperationException>(() => bag1.RemoveFirst());

            Assert.ThrowsException<InvalidOperationException>(() => bag1.RemoveLast());
        }

        [TestMethod]
        public void SerializeStrings()
        {
            var d = new OrderedBag<string>(StringComparer.InvariantCultureIgnoreCase) {
                null,
                "hello",
                "foo",
                "WORLD",
                "Hello",
                "eLVIs",
                "elvis",
                null,
                "cool"
            };
            d.AddMany(new string[] { "1", "2", "3", "4", "5", "6" });
            d.AddMany(new string[] { "7", "8", "9", "10", "11", "12" });

            var result = (OrderedBag<string>)InterfaceTests.SerializeRoundTrip(d);

            InterfaceTests.TestReadWriteCollectionGeneric((ICollection<string>)result, 
                new string[] { null, null, "1", "10", "11", "12", "2", "3", "4", "5", "6", "7", "8", "9", "cool", "eLVIs", "elvis", "foo", "hello", "Hello", "WORLD" }, true);
        }


    }
}

