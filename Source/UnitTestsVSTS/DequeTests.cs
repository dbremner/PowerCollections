﻿//******************************
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
using static Wintellect.PowerCollections.Tests.UtilTests;

namespace Wintellect.PowerCollections.Tests
{
    [TestClass]
    public class DequeTests
    {
        private void CheckListAndDeque<T>(List<T> list, Deque<T> deque)
        {
            Assert.AreEqual(list.Count, deque.Count);

            for (int i = 0; i < list.Count; ++i) {
                Assert.AreEqual(list[i], deque[i]);
            }
        }

        [TestMethod]
        public void RandomInsertDelete()
        {
            const int ITER = 2000, LOOP = 15;
            var rand = new Random(13);

            for (int loop = 0; loop < LOOP; ++loop) {
                var deque = new Deque<int>();
                var list = new List<int>();
                for (int iter = 0; iter < ITER; ++iter) {
                    //Console.Write("Loop {0}, Iteration {1}: ", loop, iter);
                    if (rand.Next(100) < 45) {
                        // remove an item.
                        if (list.Count > 0) {
                            int index = rand.Next(list.Count);
                            //Console.WriteLine("RemoveAt({0})", index);
                            list.RemoveAt(index);
                            deque.RemoveAt(index);
                        }
                    }
                    else {
                        // Add an item.
                        int item = rand.Next(1, 1000);
                        int index = rand.Next(list.Count + 1);
                        //Console.WriteLine("Insert({0}, {1})", index, item);
                        list.Insert(index, item);
                        deque.Insert(index, item);
                    }

                    //deque.Print();
                    CheckListAndDeque(list, deque);
                }

                InterfaceTests.TestReadWriteList(deque, list.ToArray());
            }
        }

        [TestMethod]
        public void RandomInsertDeleteRange()
        {
            const int ITER = 2000, LOOP = 15;
            var rand = new Random(13);

            for (int loop = 0; loop < LOOP; ++loop) {
                var deque = new Deque<int>();
                var list = new List<int>();
                for (int iter = 0; iter < ITER; ++iter) {
                   //Console.Write("Loop {0}, Iteration {1}: ", loop, iter);
                    if (rand.Next(100) < 45) {
                        // remove a range.
                        if (list.Count > 0) {
                            int index = rand.Next(list.Count);
                            int count = rand.Next(list.Count - index);
                            //Console.WriteLine("RemoveAt({0}, {1})", index, count);
                            list.RemoveRange(index, count);
                            deque.RemoveRange(index, count);
                        }
                    }
                    else {
                        // Add an range.
                        int index = rand.Next(list.Count + 1);
                        int count = rand.Next(10);
                        int[] items = new int[count];
                        for (int i = 0; i < count; ++i)
                            items[i] = rand.Next(1000);

                        /*Console.Write("Insert({0}, {{", index);
                        for (int i = 0; i < count; ++i) {
                            if (i > 0)
                                Console.Write(", ");
                            Console.Write(items[i]);
                        }
                        Console.WriteLine("})"); */

                        IEnumerable<int> e = (rand.Next(2) == 0) ? AlgorithmsTests.EnumerableFromArray(items) : items;
                        list.InsertRange(index, e);
                        deque.InsertRange(index, e);
                    }

                    //deque.Print();
                    CheckListAndDeque(list, deque);
                }

                InterfaceTests.TestReadWriteList(deque, list.ToArray());
            }
        }

        [TestMethod]
        public void RandomAddRemoveFrontBack()
        {
            const int ITER = 2000, LOOP = 15;
            var rand = new Random(13);

            for (int loop = 0; loop < LOOP; ++loop) {
                var deque = new Deque<int>();
                var list = new List<int>();
                for (int iter = 0; iter < ITER; ++iter) {
                    //Console.Write("Loop {0}, Iteration {1}: ", loop, iter);
                    if (rand.Next(100) < 45 && list.Count > 0) {
                        int r = rand.Next(10);
                        if (r < 4) {
                            //Console.WriteLine("RemoveFront()");
                            int removedList = list[0];
                            list.RemoveAt(0);
                            int removedDeque = deque.RemoveFromFront();
                            Assert.AreEqual(removedList, removedDeque);
                        }
                        else if (r < 8) {
                            //Console.WriteLine("RemoveBack()");
                            int removedList = list[list.Count - 1];
                            list.RemoveAt(list.Count - 1);
                            int removedDeque = deque.RemoveFromBack();
                            Assert.AreEqual(removedList, removedDeque);
                        }
                        else {
                            int index = rand.Next(list.Count);
                            //Console.WriteLine("RemoveAt({0})", index);
                            list.RemoveAt(index);
                            deque.RemoveAt(index);
                        }
                    }
                    else {
                        int r = rand.Next(10);
                        int item = rand.Next(1, 1000);
                        if (r < 4) {
                            //Console.WriteLine("AddFront({0})", item);
                            list.Insert(0, item);
                            deque.AddToFront(item);
                        }
                        else if (r < 8) {
                            //Console.WriteLine("AddBack({0})", item);
                            list.Add(item);
                            deque.AddToBack(item);
                        }
                        else {
                            // Add an item.
                            int index = rand.Next(list.Count + 1);
                            //Console.WriteLine("Insert({0}, {1})", index, item);
                            list.Insert(index, item);
                            deque.Insert(index, item);
                        }
                    }

                    //deque.Print();
                    CheckListAndDeque(list, deque);
                }

                InterfaceTests.TestReadWriteList(deque, list.ToArray());
            }
        }

        [TestMethod]
        public void GenericIListInterface()
        {
            var d = new Deque<string>();

            d.AddToFront("foo");
            d.AddToBack("world");
            d.AddToFront("hello");
            d.AddToBack("elvis");
            d.AddToFront("elvis");
            d.AddToBack(null);
            d.AddToFront("cool");

            InterfaceTests.TestReadWriteListGeneric((IList<string>)d, new string[] { "cool", "elvis", "hello", "foo", "world", "elvis", null });
        }

        [TestMethod]
        public void IListInterface()
        {
            var d = new Deque<string>();

            d.AddToFront("foo");
            d.AddToBack("world");
            d.AddToFront("hello");
            d.AddToBack("elvis");
            d.AddToFront("elvis");
            d.AddToBack(null);
            d.AddToFront("cool");

            InterfaceTests.TestReadWriteList((IList)d, new string[] { "cool", "elvis", "hello", "foo", "world", "elvis", null });
        }

        [TestMethod]
        public void Insert()
        {
            var d = new Deque<string>();

            d.Insert(0, "a");
            d.Insert(1, "b");
            d.Insert(0, "c");
            d.Insert(2, "d");
            d.Insert(1, "e");
            d.Insert(4, "f");
            d.Insert(3, "g");
            d.Insert(2, "h");
            d.Insert(0, "i");
            d.Insert(2, "j");
            d.Insert(4, "k");
            d.Insert(3, "l");
            d.Insert(4, "m");
            d.Insert(2, "n");
            InterfaceTests.TestEnumerableElements(d, new string[] { "i", "c", "n", "j", "l", "m", "e", "k", "h", "a", "g", "d", "f", "b" });
            d.RemoveFromBack();
            d.RemoveFromBack();
            d.RemoveFromBack();
            d.RemoveFromBack();
            d.RemoveFromBack();
            d.Insert(4, "o");
            d.Insert(10, "p");
            InterfaceTests.TestEnumerableElements(d, new string[] { "i", "c", "n", "j", "o", "l", "m", "e", "k", "h", "p" });
            InterfaceTests.TestReadWriteListGeneric(d, new string[] { "i", "c", "n", "j", "o", "l", "m", "e", "k", "h", "p" });
        }

        [TestMethod]
        public void InsertExceptions()
        {
            var d = new Deque<string>();

            d.AddToFront("foo");
            d.AddToBack("world");
            d.AddToFront("hello");
            d.AddToBack("elvis");
            d.AddToFront("elvis");
            d.AddToBack(null);
            d.AddToFront("cool");

            var invalidIndices = new[] {-1, 8, int.MinValue, int.MaxValue};
            foreach (var invalidIndex in invalidIndices) {
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => d.Insert(invalidIndex, "hi"));
            }
        }

        [TestMethod]
        public void InsertRange()
        {
            var deque = new Deque<string>();
            Assert.ThrowsException<ArgumentNullException>(() => deque.InsertRange(1, null));
        }

        [TestMethod]
        public void RemoveAt()
        {
            var d = new Deque<string>();

            d.Insert(0, "a");
            d.Insert(1, "b");
            d.Insert(0, "c");
            d.Insert(2, "d");
            d.Insert(1, "e");
            d.Insert(4, "f");
            d.Insert(3, "g");
            d.Insert(2, "h");
            d.Insert(0, "i");
            d.Insert(2, "j");
            d.Insert(4, "k");
            d.RemoveAt(4);
            d.RemoveAt(3);
            d.RemoveAt(2);
            d.RemoveAt(5);
            d.RemoveAt(2);
            InterfaceTests.TestReadWriteListGeneric(d, new string[] { "i", "c", "a", "g", "f", "b" });

            d.Clear();
            d.AddToBack("f");
            d.AddToBack("g");
            d.AddToFront("e");
            d.AddToFront("d");
            d.AddToFront("c");
            d.AddToFront("b");
            d.AddToFront("a");
            d.RemoveAt(3);
            d.RemoveAt(4);
            d.RemoveAt(4);
            InterfaceTests.TestReadWriteListGeneric(d, new string[] {"a", "b", "c", "e" });
        }

        [TestMethod]
        public void RemoveAtExceptions()
        {
            var d = new Deque<string>();

            d.AddToFront("foo");
            d.AddToBack("world");
            d.AddToFront("hello");
            d.AddToBack("elvis");
            d.AddToFront("elvis");
            d.AddToBack(null);
            d.AddToFront("cool");

            var invalidIndices = new[] {-1, 7, int.MinValue, int.MaxValue};
            foreach (var invalidIndex in invalidIndices) {
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => d.RemoveAt(invalidIndex));
            }
        }

        [TestMethod]
        public void Indexer()
        {
            var d = new Deque<string>();

            d.AddToFront("c");
            d.AddToFront("b");
            d.AddToFront("a");
            d.AddToBack("d");
            d.AddToBack("e");
            d.AddToBack("f");
            Assert.AreEqual("b", d[1]);
            Assert.AreEqual("e", d[4]);
            d[1] = "q";
            d[4] = "r";
            Assert.AreEqual("q", d[1]);
            Assert.AreEqual("r", d[4]);
            InterfaceTests.TestReadWriteListGeneric(d, new string[] { "a", "q", "c", "d", "r", "f" });
            d.Clear();

            d.AddToBack("a");
            d.AddToBack("b");
            d.AddToBack("c");
            d.AddToBack("d");
            Assert.AreEqual("b", d[1]);
            Assert.AreEqual("d", d[3]);
            d[1] = "q";
            d[3] = "r";
            Assert.AreEqual("q", d[1]);
            Assert.AreEqual("r", d[3]);
            InterfaceTests.TestReadWriteListGeneric(d, new string[] { "a", "q", "c", "r" });
        }

        [TestMethod]
        public void IndexerExceptions()
        {
            string s = "foo";
            var d = new Deque<string>();

            d.AddToFront("c");
            d.AddToFront("b");
            d.AddToFront("a");
            d.AddToBack("d");
            d.AddToBack("e");
            d.AddToBack("f");

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => s = d[-1]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => s = d[6]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => s = d[int.MaxValue]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => s = d[int.MinValue]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => d[-1] = s);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => d[6] = s);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => d[int.MaxValue] = s);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => d[int.MinValue] = s);

            d.Clear();
            d.AddToBack("a");
            d.AddToBack("b");
            d.AddToBack("c");
            d.AddToBack("d");

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => s = d[-1]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => s = d[4]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => s = d[int.MaxValue]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => s = d[int.MinValue]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => d[-1] = s);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => d[4] = s);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => d[int.MaxValue] = s);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => d[int.MinValue] = s);
        }

        [TestMethod]
        public void EmptyExceptions()
        {
            var d = new Deque<double>();

            Assert.ThrowsException<InvalidOperationException>(() => d.GetAtFront());

            Assert.ThrowsException<InvalidOperationException>(() => d.GetAtBack());

            Assert.ThrowsException<InvalidOperationException>(() => d.RemoveFromFront());

            Assert.ThrowsException<InvalidOperationException>(() => d.RemoveFromBack());

            d.AddToBack(2.3);
            d.RemoveFromFront();

            Assert.ThrowsException<InvalidOperationException>(() => d.GetAtFront());

            Assert.ThrowsException<InvalidOperationException>(() => d.GetAtBack());

            Assert.ThrowsException<InvalidOperationException>(() => d.RemoveFromFront());

            Assert.ThrowsException<InvalidOperationException>(() => d.RemoveFromBack());
        }

        [TestMethod]
        public void AddMany()
        {
            var deque1 = new Deque<string>(new string[] { "A", "B", "C", "D" });
            deque1.AddManyToFront(new string[] { "Q", "R", "S" });
            deque1.AddManyToBack(new string[] { "L", "M", "N", "O" });
            InterfaceTests.TestReadWriteListGeneric(deque1, new string[] { "Q", "R", "S", "A", "B", "C", "D", "L", "M", "N", "O" });
            Assert.ThrowsException<ArgumentNullException>(() => deque1.AddManyToFront(null));
            Assert.ThrowsException<ArgumentNullException>(() => deque1.AddManyToBack(null));
        }

        [TestMethod]
        public void FailFastEnumerator()
        {
            var deque1 = new Deque<string>(new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" });
            int i = 0;

            Assert.ThrowsException<InvalidOperationException>(() => {
                foreach (string unused in deque1) {
                    ++i;
                    Assert.IsTrue(i < 4);
                    if (i == 3)
                        deque1.AddToBack("hi");
                }
            });

            i = 0;
            Assert.ThrowsException<InvalidOperationException>(() => {
                foreach (string unused in deque1) {
                    ++i;
                    Assert.IsTrue(i < 4);
                    if (i == 3)
                        deque1.AddToFront("hi");
                }
            });

            i = 0;
            Assert.ThrowsException<InvalidOperationException>(() => {
                foreach (string unused in deque1) {
                    ++i;
                    Assert.IsTrue(i < 4);
                    if (i == 3)
                        deque1.RemoveRange(2, 4);
                }
            });

            i = 0;
            Assert.ThrowsException<InvalidOperationException>(() => {
                foreach (string unused in deque1) {
                    ++i;
                    Assert.IsTrue(i < 4);
                    if (i == 3)
                        deque1[5] = "hi";
                }
            });

            i = 0;
            Assert.ThrowsException<InvalidOperationException>(() => {
                foreach (string unused in deque1) {
                    ++i;
                    Assert.IsTrue(i < 4);
                    if (i == 3)
                        deque1.Clear();
                }
            });
        }

        [TestMethod]
        public void Initialize()
        {
            var deque1 = new Deque<string>(new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" });
            InterfaceTests.TestReadWriteListGeneric(deque1, new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" });
            var deque2 = new Deque<string>(new string[] {});
            InterfaceTests.TestReadWriteListGeneric(deque2, new string[] {});
            var deque3 = new Deque<string>();
            InterfaceTests.TestReadWriteListGeneric(deque3, new string[] {});
        }

        [TestMethod]
        public void Capacity()
        {
            var deque1 = new Deque<int>();

            Assert.AreEqual(0, deque1.Capacity);
            deque1.Add(4);
            Assert.AreEqual(7, deque1.Capacity);
            for (int i = 0; i < 100; ++i)
                deque1.Add(i);
            Assert.AreEqual(127, deque1.Capacity);

            deque1.Clear();
            Assert.AreEqual(0, deque1.Capacity);
            deque1.Capacity = 4;
            Assert.AreEqual(4, deque1.Capacity);
            for (int i = 0; i < 12; ++i)
                deque1.Add(i);
            Assert.AreEqual(19, deque1.Capacity);
            deque1.Capacity = 12;
            Assert.AreEqual(deque1.Capacity, 12);
            InterfaceTests.TestReadWriteListGeneric(deque1, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 });

            deque1.Clear();
            for (int i = 0; i < 12; ++i)
                deque1.Add(i);
            var invalidCapacities = new[] {11, -1, Int32.MaxValue, Int32.MinValue,};
            foreach (var invalidCapacity in invalidCapacities) {
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => deque1.Capacity = invalidCapacity);
            }
        }

        [TestMethod]
        public void TrimToSize()
        {
            var deque1 = new Deque<int>();
            deque1.TrimToSize();
            Assert.AreEqual(deque1.Count, deque1.Capacity);

            for (int i = 0; i < 12; ++i)
                deque1.Add(i);

            deque1.TrimToSize();
            Assert.AreEqual(deque1.Count, deque1.Capacity);
            InterfaceTests.TestReadWriteListGeneric(deque1, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 });
        }

        [TestMethod]
        public void RemoveRangeExceptions()
        {
            var deque1 = new Deque<int>();
            for (int i = 0; i < 100; ++i)
                deque1.AddToBack(i);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => deque1.RemoveRange(3, 98));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => deque1.RemoveRange(-1, 1));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => deque1.RemoveRange(0, int.MaxValue));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => deque1.RemoveRange(1, int.MinValue));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => deque1.RemoveRange(45, int.MinValue));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => deque1.RemoveRange(0, 101));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => deque1.RemoveRange(100, 1));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => deque1.RemoveRange(int.MinValue, 1));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => deque1.RemoveRange(int.MaxValue, 1));
        }

        [TestMethod]
        public void Clone()
        {
            var deque1 = new Deque<int>();
            Deque<int> deque2 = deque1.Clone();
            var deque3 = new Deque<int>(deque1);
            InterfaceTests.TestListGeneric(deque2, new int[0], null);
            InterfaceTests.TestListGeneric(deque3, new int[0], null);
            deque1.Add(5);
            InterfaceTests.TestListGeneric(deque2, new int[0], null);
            InterfaceTests.TestListGeneric(deque3, new int[0], null);

            int[] array = new int[100];
            for (int i = 0; i < 100; ++i)
                array[i] = i;
            deque1.Clear();
            for (int i = 63; i < 100; ++i)
                deque1.AddToBack(i);
            for (int i = 62; i >= 0; --i)
                deque1.AddToFront(i);
            deque2 = deque1.Clone();
            deque3 = new Deque<int>(deque1);
            InterfaceTests.TestListGeneric(deque2, array, null);
            InterfaceTests.TestListGeneric(deque3, array, null);
            deque3.Clear();
            InterfaceTests.TestListGeneric(deque1, array, null);
            InterfaceTests.TestListGeneric(deque2, array, null);
        }

        // Simple class for testing cloning.
        private class MyInt : ICloneable
        {
            public readonly int value;
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

        private void CompareClones<T>(Deque<T> s1, Deque<T> s2)
        {
            IEnumerator<T> e1 = s1.GetEnumerator();
            IEnumerator<T> e2 = s2.GetEnumerator();

            Assert.IsTrue(s1.Count == s2.Count);

            // Check that the deques are equal, but not reference equals (e.g., have been cloned).
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
            var deque1 = new Deque<MyInt> {
                new MyInt(143),
                new MyInt(2)
            };
            deque1.AddToFront(new MyInt(9));
            deque1.Add(null);
            deque1.AddToFront(new MyInt(2));
            deque1.Add(new MyInt(111));
            Deque<MyInt> deque2 = deque1.CloneContents();
            CompareClones(deque1, deque2);

            var deque3 = new Deque<int>(new int[] { 144, 5, 23 });
            deque3.InsertRange(1, new int[] { 7, 5, 11, 109 });
            Deque<int> deque4 = deque3.CloneContents();
            CompareClones(deque3, deque4);

            var deque5 = new Deque<CloneableStruct> {
                new CloneableStruct(143)
            };
            deque5.AddToFront(new CloneableStruct(5));
            deque5.Add(new CloneableStruct(23));
            deque5.AddToFront(new CloneableStruct(1));
            deque5.AddToFront(new CloneableStruct(8));
            Deque<CloneableStruct> deque6 = deque5.CloneContents();

            Assert.AreEqual(deque5.Count, deque6.Count);

            // Check that the deques are equal, but not identical (e.g., have been cloned via ICloneable).
            IEnumerator<CloneableStruct> e1 = deque5.GetEnumerator();
            IEnumerator<CloneableStruct> e2 = deque6.GetEnumerator();

            // Check that the deques are equal, but not reference equals (e.g., have been cloned).
            while (e1.MoveNext()) {
                e2.MoveNext();
                Assert.IsTrue(e1.Current.Equals(e2.Current));
                Assert.IsFalse(e1.Current.Identical(e2.Current));
            }
        }

        [TestMethod]
        public void CantCloneContents()
        {
            var deque1 = new Deque<NotCloneable> {
                new NotCloneable(),
                new NotCloneable()
            };

            Assert.ThrowsException<InvalidOperationException>(() => deque1.CloneContents());
        }

        [TestMethod]
        public void Range()
        {
            var main = new Deque<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            IList<int> range = main.Range(2, 4);

            InterfaceTests.TestListGeneric(range, new int[] { 2, 3, 4, 5 }, null);

            main = new Deque<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            range = main.Range(2, 4);
            range[1] = 7;
            range.Add(99);
            Assert.AreEqual(5, range.Count);
            range.RemoveAt(0);
            Assert.AreEqual(4, range.Count);
            InterfaceTests.TestEnumerableElements(main, new int[] { 0, 1, 7, 4, 5, 99, 6, 7 });
            main[3] = 11;
            InterfaceTests.TestEnumerableElements(range, new int[] { 7, 11, 5, 99 });

            main = new Deque<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            range = main.Range(5, 3);
            Assert.AreEqual(3, range.Count);
            main.Remove(6);
            main.Remove(5);
            Assert.AreEqual(1, range.Count);
            Assert.AreEqual(7, range[0]);

            main = new Deque<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            range = main.Range(8, 0);
            range.Add(8);
            range.Add(9);
            InterfaceTests.TestEnumerableElements(main, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            InterfaceTests.TestEnumerableElements(range, new int[] { 8, 9 });

            main = new Deque<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            range = main.Range(0, 4);
            range.Clear();
            Assert.AreEqual(0, range.Count);
            InterfaceTests.TestEnumerableElements(main, new int[] { 4, 5, 6, 7 });
            range.Add(100);
            range.Add(101);
            InterfaceTests.TestEnumerableElements(main, new int[] { 100, 101, 4, 5, 6, 7 });

            main = new Deque<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            range = main.Range(8, 0);
            InterfaceTests.TestListGeneric(range, new int[] { }, null);
        }

        [TestMethod]
        public void RangeExceptions()
        {
            var deque = new Deque<int>();

            for (int i = 0; i < 50; ++i)
                deque.AddToFront(i);
            for (int i = 0; i < 50; ++i)
                deque.AddToBack(i);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = deque.Range(3, 98);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = deque.Range(-1, 1);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = deque.Range(0, int.MaxValue);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = deque.Range(1, int.MinValue);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = deque.Range(45, int.MinValue);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = deque.Range(0, 101);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = deque.Range(100, 1);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = deque.Range(int.MinValue, 1);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = deque.Range(int.MaxValue, 1);});
        }

        [TestMethod]
        public void SerializeStrings()
        {
            var d = new Deque<string>();

            d.AddToFront("foo");
            d.AddToBack("world");
            d.AddToFront("hello");
            d.AddToBack("elvis");
            d.AddToFront("elvis");
            d.AddToBack(null);
            d.AddToFront("cool");
            d.AddManyToFront(new string[] { "1", "2", "3", "4", "5", "6" });
            d.AddManyToBack(new string[] { "7", "8", "9", "10", "11", "12" });

            var result = (Deque<string>) InterfaceTests.SerializeRoundTrip(d);

            InterfaceTests.TestReadWriteListGeneric((IList<string>)result, new string[] { "1", "2", "3", "4", "5", "6", "cool", "elvis", "hello", "foo", "world", "elvis", null, "7", "8", "9", "10", "11", "12" });
        }

        [Serializable]
        private class UniqueStuff
        {
            public InterfaceTests.Unique[] objects;
            public Deque<InterfaceTests.Unique> deque;
        }

        [TestMethod]
        public void SerializeUnique()
        {
            var d = new UniqueStuff();

            d.objects = new InterfaceTests.Unique[] {
                new InterfaceTests.Unique("1"), new InterfaceTests.Unique("2"), new InterfaceTests.Unique("3"), new InterfaceTests.Unique("4"), new InterfaceTests.Unique("5"), new InterfaceTests.Unique("6"),
                new InterfaceTests.Unique("cool"), new InterfaceTests.Unique("elvis"), new InterfaceTests.Unique("hello"), new InterfaceTests.Unique("foo"), new InterfaceTests.Unique("world"), new InterfaceTests.Unique("elvis"), new InterfaceTests.Unique(null), null,
                new InterfaceTests.Unique("7"), new InterfaceTests.Unique("8"), new InterfaceTests.Unique("9"), new InterfaceTests.Unique("10"), new InterfaceTests.Unique("11"), new InterfaceTests.Unique("12") };
            d.deque = new Deque<InterfaceTests.Unique>();

            d.deque.AddToFront(d.objects[9]);
            d.deque.AddToBack(d.objects[10]);
            d.deque.AddToFront(d.objects[8]);
            d.deque.AddToBack(d.objects[11]);
            d.deque.AddToFront(d.objects[7]);
            d.deque.AddToBack(d.objects[12]);
            d.deque.AddToFront(d.objects[6]);
            d.deque.AddToBack(d.objects[13]);
            d.deque.AddManyToFront(new InterfaceTests.Unique[] { d.objects[0], d.objects[1], d.objects[2], d.objects[3], d.objects[4], d.objects[5] });
            d.deque.AddManyToBack(new InterfaceTests.Unique[] { d.objects[14], d.objects[15], d.objects[16], d.objects[17], d.objects[18], d.objects[19] });

            var result = (UniqueStuff)InterfaceTests.SerializeRoundTrip(d);

            InterfaceTests.TestReadWriteListGeneric(result.deque, result.objects);

            for (int i = 0; i < result.objects.Length; ++i) {
                if (result.objects[i] != null)
                    Assert.IsFalse(object.Equals(result.objects[i], d.objects[i]));
            }
        }
    }
}
