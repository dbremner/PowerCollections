//******************************
// Written by Peter Golde
// Copyright (c) 2004-2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement 
// contained in the file "License.txt" accompanying this file.
//******************************

using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Wintellect.PowerCollections.Tests.TestHelpers;
using static Wintellect.PowerCollections.Tests.TestPredicates;
using static Wintellect.PowerCollections.Tests.UtilTests;

namespace Wintellect.PowerCollections.Tests
{
    [TestClass]
    public class SetTests
    {
        [TestMethod]
        public void RandomAddDelete()
        {
            const int SIZE = 50000;
            bool[] present = new bool[SIZE];
            var rand = new Random();
            var set1 = new Set<int>();
            bool b;

            // Add and delete values at random.
            for (int i = 0; i < SIZE * 10; ++i) {
                int v = rand.Next(SIZE);
                if (present[v]) {
                    Assert.IsTrue(set1.Contains(v));
                    b = set1.Remove(v);
                    Assert.IsTrue(b);
                    present[v] = false;
                }
                else {
                    Assert.IsFalse(set1.Contains(v));
                    b = set1.Add(v);
                    Assert.IsFalse(b);
                    present[v] = true;
                }
            }

            int count = 0;
            foreach (bool x in present)
                if (x)
                    ++count;
            Assert.AreEqual(count, set1.Count);

            // Make sure the set has all the correct values, not in order.
            foreach (int v in set1) {
                Assert.IsTrue(present[v]);
                present[v] = false;
            }

            // Make sure all were found.
            count = 0;
            foreach (bool x in present)
                if (x)
                    ++count;
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void ICollectionInterface()
        {
            string[] s_array = { "Foo", "Eric", "Clapton", "hello", "goodbye", "C#" };
            var set1 = new Set<string>();

            foreach (string s in s_array)
                set1.Add(s);

            Array.Sort(s_array);
            InterfaceTests.TestCollection((ICollection)set1, s_array, false);
        }


        [TestMethod]
        public void GenericICollectionInterface()
        {
            string[] s_array = { "Foo", "Eric", "Clapton", "hello", "goodbye", "C#", "Java" };
            var set1 = new Set<string>();

            foreach (string s in s_array)
                set1.Add(s);

            Array.Sort(s_array);
            InterfaceTests.TestReadWriteCollectionGeneric((ICollection<string>)set1, s_array, false);
        }

        [TestMethod]
        public void Add()
        {
            var set1 = new Set<string>(StringComparer.InvariantCultureIgnoreCase);
            bool b;

            b = set1.Add("hello"); Assert.IsFalse(b);
            b = set1.Add("foo"); Assert.IsFalse(b);
            b = set1.Add(""); Assert.IsFalse(b);
            b = set1.Add("HELLO"); Assert.IsTrue(b);
            b = set1.Add("foo"); Assert.IsTrue(b);
            b = set1.Add(null); Assert.IsFalse(b);
            b = set1.Add("Hello"); Assert.IsTrue(b);
            b = set1.Add("Eric"); Assert.IsFalse(b);
            b = set1.Add(null); Assert.IsTrue(b);

            InterfaceTests.TestReadWriteCollectionGeneric(set1, new string[] { null, "", "Eric", "foo", "Hello" }, false);
        }

        [TestMethod]
        public void CountAndClear()
        {
            var set1 = new Set<string>(StringComparer.InvariantCultureIgnoreCase);

            Assert.AreEqual(0, set1.Count);
            set1.Add("hello"); Assert.AreEqual(1, set1.Count);
            set1.Add("foo"); Assert.AreEqual(2, set1.Count);
            set1.Add(""); Assert.AreEqual(3, set1.Count);
            set1.Add("HELLO"); Assert.AreEqual(3, set1.Count);
            set1.Add("foo"); Assert.AreEqual(3, set1.Count);
            set1.Add(null); Assert.AreEqual(4, set1.Count);
            set1.Add("Hello"); Assert.AreEqual(4, set1.Count);
            set1.Add("Eric"); Assert.AreEqual(5, set1.Count);
            set1.Add(null); Assert.AreEqual(5, set1.Count);
            set1.Clear();
            Assert.AreEqual(0, set1.Count);

            bool found = false;
            foreach (string s in set1)
                found = true;

            Assert.IsFalse(found);
        }

        [TestMethod]
        public void Remove()
        {
            var set1 = new Set<string>(StringComparer.InvariantCultureIgnoreCase);
            bool b;

            b = set1.Remove("Eric"); Assert.IsFalse(b);
            b = set1.Add("hello"); Assert.IsFalse(b);
            b = set1.Add("foo"); Assert.IsFalse(b);
            b = set1.Add(""); Assert.IsFalse(b);
            b = set1.Remove("HELLO"); Assert.IsTrue(b);
            b = set1.Remove("hello"); Assert.IsFalse(b);
            b = set1.Remove(null); Assert.IsFalse(b);
            b = set1.Add("Hello"); Assert.IsFalse(b);
            b = set1.Add("Eric"); Assert.IsFalse(b);
            b = set1.Add(null); Assert.IsFalse(b);
            b = set1.Remove(null); Assert.IsTrue(b);
            b = set1.Add("Eric"); Assert.IsTrue(b);
            b = set1.Remove("eRic"); Assert.IsTrue(b);
            b = set1.Remove("eRic"); Assert.IsFalse(b);
            set1.Clear();
            b = set1.Remove(""); Assert.IsFalse(b);

        }

        [TestMethod]
        public void TryGetItem()
        {
            var set1 = new Set<string>(StringComparer.InvariantCultureIgnoreCase);
            bool b;
            string s;

            b = set1.TryGetItem("Eric", out s); Assert.IsFalse(b); Assert.IsNull(s);
            b = set1.Add(null); Assert.IsFalse(b);
            b = set1.Add("hello"); Assert.IsFalse(b);
            b = set1.Add("foo"); Assert.IsFalse(b);
            b = set1.Add(""); Assert.IsFalse(b);
            b = set1.TryGetItem("HELLO", out s); Assert.IsTrue(b); Assert.AreEqual("hello", s);
            b = set1.Remove("hello"); Assert.IsTrue(b);
            b = set1.TryGetItem("HELLO", out s); Assert.IsFalse(b); Assert.IsNull(s);
            b = set1.TryGetItem("foo", out s); Assert.IsTrue(b); Assert.AreEqual("foo", s);
            b = set1.Add("Eric"); Assert.IsFalse(b);
            b = set1.TryGetItem("eric", out s); Assert.IsTrue(b); Assert.AreEqual("Eric", s);
            b = set1.TryGetItem(null, out s); Assert.IsTrue(b); Assert.IsNull(s);
            set1.Clear();
            b = set1.TryGetItem("foo", out s); Assert.IsFalse(b); Assert.IsNull(s);

        }

        [TestMethod]
        public void AddMany()
        {
            var set1 = new Set<string>(StringComparer.InvariantCultureIgnoreCase) {
                "foo",
                "Eric",
                "Clapton"
            };
            string[] s_array = { "FOO", "x", "elmer", "fudd", "Clapton", null };
            set1.AddMany(s_array);

            InterfaceTests.TestReadWriteCollectionGeneric(set1, new string[] { null, "Clapton", "elmer", "Eric", "FOO", "fudd", "x" }, false);
        }

        [TestMethod]
        public void RemoveMany()
        {
            var set1 = new Set<string>(StringComparer.InvariantCultureIgnoreCase) {
                "foo",
                "Eric",
                "Clapton",
                null,
                "fudd",
                "elmer"
            };

            string[] s_array = { "FOO", "jasmine", "eric", null };
            int count = set1.RemoveMany(s_array);
            Assert.AreEqual(3, count);

            InterfaceTests.TestReadWriteCollectionGeneric(set1, new string[] { "Clapton", "elmer", "fudd" }, false);

            set1.Clear();
            set1.Add("foo");
            set1.Add("Eric");
            set1.Add("Clapton");
            set1.Add(null);
            set1.Add("fudd");
            count = set1.RemoveMany(set1);
            Assert.AreEqual(5, count);
            Assert.AreEqual(0, set1.Count);
        }

        [TestMethod]
        public void RemoveAll()
        {
            var set1 = new Set<double>(new double[] { 4.5, 1.2, 7.6, -0.04, -7.6, 1.78, 10.11, 187.4 });

            set1.RemoveAll(AbsOver5);
            InterfaceTests.TestReadWriteCollectionGeneric(set1, new double[] { -0.04, 1.2, 1.78, 4.5 }, false);

            set1 = new Set<double>(new double[] { 4.5, 1.2, 7.6, -0.04, -7.6, 1.78, 10.11, 187.4 });
            set1.RemoveAll(IsZero);
            InterfaceTests.TestReadWriteCollectionGeneric(set1, new double[] { -7.6, -0.04, 1.2, 1.78, 4.5, 7.6, 10.11, 187.4 }, false);

            set1 = new Set<double>(new double[] { 4.5, 1.2, 7.6, -0.04, -7.6, 1.78, 10.11, 187.4 });
            set1.RemoveAll(Under200);
            Assert.AreEqual(0, set1.Count);
        }

        [TestMethod]
        public void FailFastEnumerator1()
        {
            var set1 = new Set<double>();

            double d = 1.218034;
            for (int i = 0; i < 50; ++i) {
                set1.Add(d);
                d = d * 1.3451 - .31;
            }

            // should throw once the set is modified.
            void InvalidOperation()
            {
                foreach (double k in set1) {
                    if (k > 3.0)
                        set1.Add(1.0);
                }
            }

            ThrowsInvalid(InvalidOperation);
        }

        [TestMethod]
        public void FailFastEnumerator2()
        {
            var set1 = new Set<double>();

            double d = 1.218034;
            for (int i = 0; i < 50; ++i) {
                set1.Add(d);
                d = d * 1.3451 - .31;
            }

            // should throw once the set is modified.
            void InvalidOperation()
            {
                foreach (double k in set1) {
                    if (k > 3.0) {
                        set1.Clear();
                    }
                }
            }

            ThrowsInvalid(InvalidOperation);
        }

        [TestMethod]
        public void Clone()
        {
            var set1 = new Set<int>(new int[] { 1, 7, 9, 11, 13, 15, -17, 19, -21 });
            Set<int> set2, set3;

            set2 = set1.Clone();
            set3 = (Set<int>)((ICloneable)set1).Clone();

            Assert.IsFalse(set2 == set1);
            Assert.IsFalse(set3 == set1);

            // Modify set1, make sure set2, set3 don't change.
            set1.Remove(9);
            set1.Remove(-17);
            set1.Add(8);

            InterfaceTests.TestReadWriteCollectionGeneric(set2, new int[] { -21, -17, 1, 7, 9, 11, 13, 15, 19 }, false);
            InterfaceTests.TestReadWriteCollectionGeneric(set3, new int[] { -21, -17, 1, 7, 9, 11, 13, 15, 19 }, false);

            set1 = new Set<int>();
            set2 = set1.Clone();
            Assert.IsFalse(set2 == set1);
            Assert.IsTrue(set1.Count == 0 && set2.Count == 0);
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
                return (obj != null && obj is MyInt && ((MyInt)obj).value == value);
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

        void CompareClones<T>(Set<T> s1, Set<T> s2)
        {
            Assert.AreEqual(s1.Count, s2.Count);

            // Check that the sets are equal, but not reference equals (e.g., have been cloned).
            foreach (T item in s1) {
                int found = 0;
                foreach (T other in s2) {
                    if (object.Equals(item, other)) {
                        found += 1;
                        if (item != null)
                            Assert.IsFalse(object.ReferenceEquals(item, other));
                    }
                }
                Assert.AreEqual(1, found);
            }
        }



        [TestMethod]
        public void CloneContents()
        {
            var set1 = new Set<MyInt> {
                new MyInt(143),
                new MyInt(2),
                new MyInt(9),
                null,
                new MyInt(14),
                new MyInt(111)
            };
            Set<MyInt> set2 = set1.CloneContents();
            CompareClones(set1, set2);

            var set3 = new Set<int>(new int[] { 144, 5, 23, 1, 8 });
            Set<int> set4 = set3.CloneContents();
            CompareClones(set3, set4);

            var set5 = new Set<CloneableStruct> {
                new CloneableStruct(143),
                new CloneableStruct(5),
                new CloneableStruct(23),
                new CloneableStruct(1),
                new CloneableStruct(8)
            };
            Set<CloneableStruct> set6 = set5.CloneContents();

            Assert.AreEqual(set5.Count, set6.Count);

            // Check that the sets are equal, but not identical (e.g., have been cloned via ICloneable).
            foreach (CloneableStruct item in set5) {
                int found = 0;
                foreach (CloneableStruct other in set6) {
                    if (object.Equals(item, other)) {
                        found += 1;
                        Assert.IsFalse(item.Identical(other));
                    }
                }
                Assert.AreEqual(1, found);
            }

        }

        [TestMethod]
        public void CantCloneContents()
        {
            var set1 = new Set<NotCloneable> {
                new NotCloneable(),
                new NotCloneable()
            };

            ThrowsInvalid(() => set1.CloneContents());
        }

        // Strange comparer that uses modulo arithmetic.
        class ModularComparer: IEqualityComparer<int>
        {
            private int mod;

            public ModularComparer(int mod)
            {
                this.mod = mod;
            }

            public bool Equals(int x, int y)
            {
                return (x % mod) == (y % mod);
            }

            public int GetHashCode(int obj)
            {
                return (obj % mod).GetHashCode();
            }
        }

        [TestMethod]
        public void CustomIComparer()
        {
            var set1 = new Set<int>(new ModularComparer(5));
            bool b;

            b = set1.Add(4); Assert.IsFalse(b);
            b = set1.Add(11); Assert.IsFalse(b);
            b = set1.Add(9); Assert.IsTrue(b);
            b = set1.Add(15); Assert.IsFalse(b);

            Assert.IsTrue(set1.Contains(25));
            Assert.IsTrue(set1.Contains(26));
            Assert.IsFalse(set1.Contains(27));

            InterfaceTests.TestReadWriteCollectionGeneric(set1, new int[] { 11, 9, 15 }, false);
        }

        [TestMethod]
        public void ComparerProperty()
        {
            IEqualityComparer<int> comparer1 = new ModularComparer(5);
            var set1 = new Set<int>(comparer1);
            Assert.AreSame(comparer1, set1.Comparer);
            var set2 = new Set<decimal>();
            Assert.AreSame(EqualityComparer<decimal>.Default, set2.Comparer);
            var set3 = new Set<string>(StringComparer.InvariantCultureIgnoreCase);
            Assert.AreSame(StringComparer.InvariantCultureIgnoreCase, set3.Comparer);
        }

        // Simple class for testing that the generic IEquatable is used.
        class GenComparable : IEquatable<GenComparable>
        {
            public int value;
            public GenComparable(int value)
            {
                this.value = value;
            }

            public object Clone()
            {
                return new MyInt(value);
            }

            public override bool Equals(object obj)
            {
                throw new NotSupportedException();
            }

            public override int GetHashCode()
            {
                return value.GetHashCode();
            }

            public override string ToString()
            {
                return value.ToString();
            }
        
            #region IEquatable<GenComparable> Members

            bool IEquatable<GenComparable>.Equals(GenComparable other)
            {
                return this.value == other.value;
            }

            #endregion

}

        // Make sure that IEquatable<T>.Equals is used for equality comparison.
        [TestMethod]
        public void GenericIEquatable()
        {
            var set1 = new Set<GenComparable>();
            bool b;

            b = set1.Add(new GenComparable(4)); Assert.IsFalse(b);
            b = set1.Add(new GenComparable(11)); Assert.IsFalse(b);
            b = set1.Add(new GenComparable(4)); Assert.IsTrue(b);
            b = set1.Add(new GenComparable(15)); Assert.IsFalse(b);

            Assert.IsTrue(set1.Contains(new GenComparable(4)));
            Assert.IsTrue(set1.Contains(new GenComparable(15)));
            Assert.IsFalse(set1.Contains(new GenComparable(27)));
        }

        [TestMethod]
        public void Initialize()
        {
            var list = new List<int>(new int[] { 12, 3, 9, 8, 9 });
            var set1 = new Set<int>(list);
            var set2 = new Set<int>(list, new ModularComparer(6));

            InterfaceTests.TestReadWriteCollectionGeneric(set1, new int[] { 3, 8, 9, 12 }, false);
            InterfaceTests.TestReadWriteCollectionGeneric(set2, new int[] { 9, 8, 12 }, false);
        }

        [TestMethod]
        public void ToArray()
        {
            string[] s_array = { "Foo", "Eric", "Clapton", "hello", null, "goodbye", "C#" };
            var set1 = new Set<string>();

            string[] a1 = set1.ToArray();
            Assert.IsNotNull(a1);
            Assert.AreEqual(0, a1.Length);

            foreach (string s in s_array)
                set1.Add(s);
            string[] a2 = set1.ToArray();

            Array.Sort(s_array);
            Array.Sort(a2);

            Assert.AreEqual(s_array.Length, a2.Length);
            for (int i = 0; i < s_array.Length; ++i)
                Assert.AreEqual(s_array[i], a2[i]);
        }

        [TestMethod]
        public void Subset()
        {
            var set1 = new Set<int>(new int[] { 1, 3, 6, 7, 8, 9, 10 });
            var set2 = new Set<int>();
            var set3 = new Set<int>(new int[] { 3, 8, 9 });
            var set4 = new Set<int>(new int[] { 3, 8, 9 });
            var set5 = new Set<int>(new int[] { 1, 2, 6, 8, 9, 10 });

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
        }

        [TestMethod]
        public void IsEqualTo()
        {
            var set1 = new Set<int>(new int[] { 6, 7, 1, 11, 9, 3, 8 });
            var set2 = new Set<int>();
            var set3 = new Set<int>();
            var set4 = new Set<int>(new int[] { 9, 11, 1, 3, 6, 7, 8, 14 });
            var set5 = new Set<int>(new int[] { 3, 6, 7, 11, 14, 8, 9 });
            var set6 = new Set<int>(new int[] { 1, 3, 6, 7, 8, 10, 11 });
            var set7 = new Set<int>(new int[] { 9, 1, 8, 3, 7, 6, 11 });

            Assert.IsTrue(set1.IsEqualTo(set1));
            Assert.IsTrue(set2.IsEqualTo(set2));

            Assert.IsTrue(set2.IsEqualTo(set3));
            Assert.IsTrue(set3.IsEqualTo(set2));

            Assert.IsTrue(set1.IsEqualTo(set7));
            Assert.IsTrue(set7.IsEqualTo(set1));

            Assert.IsFalse(set1.IsEqualTo(set2));
            Assert.IsFalse(set2.IsEqualTo(set1));

            Assert.IsFalse(set1.IsEqualTo(set4));
            Assert.IsFalse(set4.IsEqualTo(set1));

            Assert.IsFalse(set1.IsEqualTo(set5));
            Assert.IsFalse(set5.IsEqualTo(set1));

            Assert.IsFalse(set1.IsEqualTo(set6));
            Assert.IsFalse(set6.IsEqualTo(set1));

            Assert.IsFalse(set5.IsEqualTo(set6));
            Assert.IsFalse(set6.IsEqualTo(set5));

            Assert.IsFalse(set5.IsEqualTo(set7));
            Assert.IsFalse(set7.IsEqualTo(set5));
        }

        [TestMethod]
        public void IsDisjointFrom()
        {
            var set1 = new Set<int>(new int[] { 6, 7, 1, 11, 9, 3, 8 });
            var set2 = new Set<int>();
            var set3 = new Set<int>();
            var set4 = new Set<int>(new int[] { 9, 1, 8, 3, 7, 6, 11 });
            var set5 = new Set<int>(new int[] { 17, 3, 12, 10 });
            var set6 = new Set<int>(new int[] { 19, 14, 0, 2});

            Assert.IsFalse(set1.IsDisjointFrom(set1));
            Assert.IsTrue(set2.IsDisjointFrom(set2));

            Assert.IsTrue(set1.IsDisjointFrom(set2));
            Assert.IsTrue(set2.IsDisjointFrom(set1));

            Assert.IsTrue(set2.IsDisjointFrom(set3));
            Assert.IsTrue(set3.IsDisjointFrom(set2));

            Assert.IsFalse(set1.IsDisjointFrom(set4));
            Assert.IsFalse(set4.IsDisjointFrom(set1));

            Assert.IsFalse(set1.IsDisjointFrom(set5));
            Assert.IsFalse(set5.IsDisjointFrom(set1));

            Assert.IsTrue(set1.IsDisjointFrom(set6));
            Assert.IsTrue(set6.IsDisjointFrom(set1));

            Assert.IsTrue(set5.IsDisjointFrom(set6));
            Assert.IsTrue(set6.IsDisjointFrom(set5));
        }

        [TestMethod]
        public void Intersection()
        {
            var setOdds = new Set<int>(new int[] { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25 });
            var setDigits = new Set<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Set<int> set1, set2, set3;

            // Algorithms work different depending on sizes, so try both ways.
            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set1.IntersectionWith(set2);
            InterfaceTests.TestReadWriteCollectionGeneric(set1, new int[] { 1, 3, 5, 7, 9 }, false);

            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set2.IntersectionWith(set1);
            InterfaceTests.TestReadWriteCollectionGeneric(set2, new int[] { 1, 3, 5, 7, 9 }, false);

            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set3 = set1.Intersection(set2);
            InterfaceTests.TestReadWriteCollectionGeneric(set3, new int[] { 1, 3, 5, 7, 9 }, false);

            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set3 = set2.Intersection(set1);
            InterfaceTests.TestReadWriteCollectionGeneric(set3, new int[] { 1, 3, 5, 7, 9 }, false);

            // Make sure intersection with itself works.
            set1 = setDigits.Clone();
            set1.IntersectionWith(set1);
            InterfaceTests.TestReadWriteCollectionGeneric(set1, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, false);

            set1 = setDigits.Clone();
            set3 = set1.Intersection(set1);
            InterfaceTests.TestReadWriteCollectionGeneric(set3, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, false);
        }

        [TestMethod]
        public void Union()
        {
            var setOdds = new Set<int>(new int[] { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25 });
            var setDigits = new Set<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Set<int> set1, set2, set3;

            // Algorithms work different depending on sizes, so try both ways.
            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set1.UnionWith(set2);
            InterfaceTests.TestReadWriteCollectionGeneric(set1, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 13, 15, 17, 19, 21, 23, 25 }, false);

            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set2.UnionWith(set1);
            InterfaceTests.TestReadWriteCollectionGeneric(set2, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 13, 15, 17, 19, 21, 23, 25 }, false);

            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set3 = set1.Union(set2);
            InterfaceTests.TestReadWriteCollectionGeneric(set3, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 13, 15, 17, 19, 21, 23, 25 }, false);

            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set3 = set2.Union(set1);
            InterfaceTests.TestReadWriteCollectionGeneric(set3, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 13, 15, 17, 19, 21, 23, 25 }, false);

            // Make sure intersection with itself works.
            set1 = setDigits.Clone();
            set1.UnionWith(set1);
            InterfaceTests.TestReadWriteCollectionGeneric(set1, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, false);

            set1 = setDigits.Clone();
            set3 = set1.Union(set1);
            InterfaceTests.TestReadWriteCollectionGeneric(set3, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, false);
        }

        [TestMethod]
        public void SymmetricDifference()
        {
            var setOdds = new Set<int>(new int[] { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25 });
            var setDigits = new Set<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Set<int> set1, set2, set3;

            // Algorithms work different depending on sizes, so try both ways.
            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set1.SymmetricDifferenceWith(set2);
            InterfaceTests.TestReadWriteCollectionGeneric(set1, new int[] { 2, 4, 6, 8, 11, 13, 15, 17, 19, 21, 23, 25 }, false);

            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set2.SymmetricDifferenceWith(set1);
            InterfaceTests.TestReadWriteCollectionGeneric(set2, new int[] { 2, 4, 6, 8, 11, 13, 15, 17, 19, 21, 23, 25 }, false);

            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set3 = set1.SymmetricDifference(set2);
            InterfaceTests.TestReadWriteCollectionGeneric(set3, new int[] { 2, 4, 6, 8, 11, 13, 15, 17, 19, 21, 23, 25 }, false);

            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set3 = set2.SymmetricDifference(set1);
            InterfaceTests.TestReadWriteCollectionGeneric(set3, new int[] { 2, 4, 6, 8, 11, 13, 15, 17, 19, 21, 23, 25 }, false);

            // Make sure intersection with itself works.
            set1 = setDigits.Clone();
            set1.SymmetricDifferenceWith(set1);
            Assert.AreEqual(0, set1.Count);

            set1 = setDigits.Clone();
            set3 = set1.SymmetricDifference(set1);
            Assert.AreEqual(0, set3.Count);
        }

        [TestMethod]
        public void Difference()
        {
            var setOdds = new Set<int>(new int[] { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25 });
            var setDigits = new Set<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            Set<int> set1, set2, set3;

            // Algorithms work different depending on sizes, so try both ways.
            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set1.DifferenceWith(set2);
            InterfaceTests.TestReadWriteCollectionGeneric(set1, new int[] { 11, 13, 15, 17, 19, 21, 23, 25 }, false);

            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set2.DifferenceWith(set1);
            InterfaceTests.TestReadWriteCollectionGeneric(set2, new int[] { 2, 4, 6, 8 }, false);

            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set3 = set1.Difference(set2);
            InterfaceTests.TestReadWriteCollectionGeneric(set3, new int[] { 11, 13, 15, 17, 19, 21, 23, 25 }, false);

            set1 = setOdds.Clone(); set2 = setDigits.Clone();
            set3 = set2.Difference(set1);
            InterfaceTests.TestReadWriteCollectionGeneric(set3, new int[] { 2, 4, 6, 8 }, false);

            // Make sure intersection with itself works.
            set1 = setDigits.Clone();
            set1.DifferenceWith(set1);
            Assert.AreEqual(0, set1.Count);

            set1 = setDigits.Clone();
            set3 = set1.Difference(set1);
            Assert.AreEqual(0, set3.Count);
        }

        [TestMethod]
        public void InconsistentComparisons1()
        {
            var setOdds = new Set<int>(new int[] { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25 });
            var setDigits = new Set<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new GOddEvenEqualityComparer());
            ThrowsInvalid(() => setOdds.SymmetricDifferenceWith(setDigits));
        }

        [TestMethod]
        public void InconsistentComparisons2()
        {
            var set1 = new Set<string>(new string[] { "foo", "Bar" }, StringComparer.CurrentCulture);
            var set2 = new Set<string>(new string[] { "bada", "bing" }, StringComparer.InvariantCulture);
            ThrowsInvalid(() => set1.Intersection(set2));
        }

        [TestMethod]
        public void ConsistentComparisons()
        {
            var set1 = new Set<string>(new string[] { "foo", "Bar" }, StringComparer.InvariantCulture);
            var set2 = new Set<string>(new string[] { "bada", "bing" }, StringComparer.InvariantCulture);
            set1.Difference(set2);
        }

        [TestMethod]
        public void SerializeStrings()
        {
            var d = new Set<string> {
                "foo",
                "world",
                "hello",
                "elvis",
                "elvis",
                null,
                "cool"
            };
            d.AddMany(new string[] { "1", "2", "3", "4", "5", "6" });
            d.AddMany(new string[] { "7", "8", "9", "10", "11", "12" });

            var result = (Set<string>)InterfaceTests.SerializeRoundTrip(d);

            InterfaceTests.TestReadWriteCollectionGeneric((ICollection<string>)result, new string[] { "1", "2", "3", "4", "5", "6", "cool", "elvis", "hello", "foo", "world", null, "7", "8", "9", "10", "11", "12" }, false);

        }

        [Serializable]
        class UniqueStuff
        {
            public InterfaceTests.Unique[] objects;
            public Set<InterfaceTests.Unique> set;
        }


        [TestMethod]
        public void SerializeUnique()
        {
            var d = new UniqueStuff();

            d.objects = new InterfaceTests.Unique[] { 
                new InterfaceTests.Unique("1"), new InterfaceTests.Unique("2"), new InterfaceTests.Unique("3"), new InterfaceTests.Unique("4"), new InterfaceTests.Unique("5"), new InterfaceTests.Unique("6"), 
                new InterfaceTests.Unique("cool"), new InterfaceTests.Unique("elvis"), new InterfaceTests.Unique("hello"), new InterfaceTests.Unique("foo"), new InterfaceTests.Unique("world"), new InterfaceTests.Unique("elvis"), new InterfaceTests.Unique(null), null,
                new InterfaceTests.Unique("7"), new InterfaceTests.Unique("8"), new InterfaceTests.Unique("9"), new InterfaceTests.Unique("10"), new InterfaceTests.Unique("11"), new InterfaceTests.Unique("12") };
            d.set = new Set<InterfaceTests.Unique> {
                d.objects[9],
                d.objects[10],
                d.objects[8],
                d.objects[11],
                d.objects[7],
                d.objects[12],
                d.objects[6],
                d.objects[13]
            };
            d.set.AddMany(new InterfaceTests.Unique[] { d.objects[0], d.objects[1], d.objects[2], d.objects[3], d.objects[4], d.objects[5] });
            d.set.AddMany(new InterfaceTests.Unique[] { d.objects[14], d.objects[15], d.objects[16], d.objects[17], d.objects[18], d.objects[19] });

            var result = (UniqueStuff)InterfaceTests.SerializeRoundTrip(d);

            InterfaceTests.TestReadWriteCollectionGeneric(result.set, result.objects, false);

            for (int i = 0; i < result.objects.Length; ++i) {
                if (result.objects[i] != null)
                    Assert.IsFalse(object.Equals(result.objects[i], d.objects[i]));
            }
        }


    }
}

