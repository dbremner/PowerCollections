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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Wintellect.PowerCollections.Tests.TestPredicates;

#endregion

namespace Wintellect.PowerCollections.Tests
{
    // A simple read-write list using an array.
    public class ReadWriteArrayList<T> : ListBase<T>
    {
        T[] array;

        public ReadWriteArrayList(T[] items)
        {
            array = (T[])(items.Clone());
        }

        public override int Count
        {
            get { return array.Length; }
        }

        public override void Clear()
        {
            array = new T[0];
        }

        public override T this[int index]
        {
            get
            {
                if (index >= 0 && index < array.Length)
                    return array[index];
                else
                    throw new ArgumentOutOfRangeException(nameof(index));
            }

            set
            {
                if (index >= 0 && index < array.Length)
                    array[index] = value;
                else
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public override void Insert(int index, T item)
        {
            if (index >= 0 && index <= array.Length) {
                T[] newArray = new T[array.Length + 1];
                Array.Copy(array, 0, newArray, 0, index);
                Array.Copy(array, index, newArray, index + 1, array.Length - index);
                newArray[index] = item;
                array = newArray;
            }
            else
                throw new ArgumentOutOfRangeException(nameof(index));
        }

        public override void RemoveAt(int index)
        {
            if (index >= 0 && index < array.Length) {
                T[] newArray = new T[array.Length - 1];
                Array.Copy(array, 0, newArray, 0, index);
                Array.Copy(array, index + 1, newArray, index, array.Length - index - 1);
                array = newArray;
            }
            else
                throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    // A simple read-only list using an array.
    public class ReadOnlyArrayList<T> : ReadOnlyListBase<T>
    {
        T[] array;

        public ReadOnlyArrayList(T[] items)
        {
            array = (T[])(items.Clone());
        }

        public override int Count
        {
            get { return array.Length; }
        }

        public override T this[int index]
        {
            get
            {
                if (index >= 0 && index < array.Length)
                    return array[index];
                else
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }

    [TestClass]
    public class ListBaseTests
    {
        [TestMethod]
        public void ReadOnlyListBase()
        {
            string[] s1 = {"Hello", "Goodbye", "Eric", null, "Clapton", "Hello", "Rules" };
            IList<string> list1 = new ReadOnlyArrayList<string>(s1);
            InterfaceTests.TestReadOnlyListGeneric(list1, s1, "ReadOnlyArrayList");

            string[] s2 = {  };
            IList<string> list2 = new ReadOnlyArrayList<string>(s2);
            InterfaceTests.TestReadOnlyListGeneric(list2, s2, "ReadOnlyArrayList");

            string[] s3 = { "foo" };
            IList<string> list3 = new ReadOnlyArrayList<string>(s3);
            InterfaceTests.TestReadOnlyListGeneric(list3, s3, "ReadOnlyArrayList");

            string[] s4 = { null, null };
            IList<string> list4 = new ReadOnlyArrayList<string>(s4);
            InterfaceTests.TestReadOnlyListGeneric(list4, s4, "ReadOnlyArrayList");

            string[] s5 = { "Hello", "Goodbye", "Eric", null, "Clapton", "Hello", "Rules" };
            IList list5 = new ReadOnlyArrayList<string>(s5);
            InterfaceTests.TestReadOnlyList(list5, s5, "ReadOnlyArrayList");

            string[] s6 = {  };
            IList list6 = new ReadOnlyArrayList<string>(s6);
            InterfaceTests.TestReadOnlyList(list6, s6, "ReadOnlyArrayList");

            string[] s7 = { "foo" };
            IList list7 = new ReadOnlyArrayList<string>(s7);
            InterfaceTests.TestReadOnlyList(list7, s7, "ReadOnlyArrayList");

            string[] s8 = { null, null };
            IList list8 = new ReadOnlyArrayList<string>(s8);
            InterfaceTests.TestReadOnlyList(list8, s8, "ReadOnlyArrayList");
        }

        [TestMethod]
        public void ReadWriteListBase()
        {
            string[] s1 = { "Hello", "Goodbye", "Eric", null, "Clapton", "Hello", "Rules" };
            IList<string> list1 = new ReadWriteArrayList<string>(s1);
            InterfaceTests.TestReadWriteListGeneric((IList<string>)list1, s1);

            string[] s2 = {  };
            IList<string> list2 = new ReadWriteArrayList<string>(s2);
            InterfaceTests.TestReadWriteListGeneric((IList<string>)list2, s2);

            string[] s3 = { "foo" };
            IList<string> list3 = new ReadWriteArrayList<string>(s3);
            InterfaceTests.TestReadWriteListGeneric((IList<string>)list3, s3);

            string[] s4 = { null, null };
            IList<string> list4 = new ReadWriteArrayList<string>(s4);
            InterfaceTests.TestReadWriteListGeneric((IList<string>)list4, s4);

            string[] s5 = { "Hello", "Goodbye", "Eric", null, "Clapton", "Hello", "Rules" };
            IList<string> list5 = new ReadWriteArrayList<string>(s5);
            InterfaceTests.TestReadWriteList((IList)list5, s5);

            string[] s6 = {  };
            IList<string> list6 = new ReadWriteArrayList<string>(s6);
            InterfaceTests.TestReadWriteList((IList)list6, s6);

            string[] s7 = { "foo" };
            IList<string> list7 = new ReadWriteArrayList<string>(s7);
            InterfaceTests.TestReadWriteList((IList)list7, s7);

            string[] s8 = { null, null };
            IList<string> list8 = new ReadWriteArrayList<string>(s8);
            InterfaceTests.TestReadWriteList((IList)list8, s8);
        }

        // Check that InterfaceTests is reasonable by checking against the built in List class.
        [TestMethod]
        public void CheckList()
        {
            string[] s1 = { "Hello", "Goodbye", "Eric", null, "Clapton", "Hello", "Rules" };

            var list1 = new List<string>(s1);
            InterfaceTests.TestReadWriteListGeneric((IList<string>)list1, s1);
            var list2 = new List<string>(s1);
            InterfaceTests.TestReadWriteList((IList)list2, s1);
            var list3 = new ArrayList(s1);
            InterfaceTests.TestReadWriteList<object>((IList)list3, s1);

            var list4 = new List<string>();
            InterfaceTests.TestReadWriteListGeneric((IList<string>)list4, new string[0]);
            var list5 = new List<string>();
            InterfaceTests.TestReadWriteList((IList)list5, new string[0]);
            var list6 = new ArrayList();
            InterfaceTests.TestReadWriteList<object>((IList)list6, new string[0]);

            IList<string> ro1 = new List<string>(s1).AsReadOnly();
            InterfaceTests.TestReadOnlyListGeneric((IList<string>)ro1, s1, null);
            IList<string> ro2 = new List<string>().AsReadOnly();
            InterfaceTests.TestReadOnlyListGeneric((IList<string>)ro2, new string[0], null);
            IList ro3 = ArrayList.ReadOnly(new ArrayList(s1));
            InterfaceTests.TestReadOnlyList<object>(ro3, s1, null);
            IList ro4 = ArrayList.ReadOnly(new ArrayList());
            InterfaceTests.TestReadOnlyList<object>(ro4, new string[0], null);

        }

        [TestMethod]
        public void Range()
        {
            var main = new ReadWriteArrayList<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            IList<int> range = main.Range(2, 4);

            InterfaceTests.TestReadWriteListGeneric(range, new int[] { 2, 3, 4, 5 }, null);

            main = new ReadWriteArrayList<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            range = main.Range(2, 4);
            range[1] = 7;
            range.Add(99);
            Assert.AreEqual(5, range.Count);
            range.RemoveAt(0);
            Assert.AreEqual(4, range.Count);
            InterfaceTests.TestEnumerableElements(main, new int[] { 0, 1, 7, 4, 5, 99, 6, 7 });
            main[3] = 11;
            InterfaceTests.TestEnumerableElements(range, new int[] { 7, 11, 5, 99 });

            main = new ReadWriteArrayList<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            range = main.Range(5, 3);
            Assert.AreEqual(3, range.Count);
            main.Remove(6);
            main.Remove(5);
            Assert.AreEqual(1, range.Count);
            Assert.AreEqual(7, range[0]);

            main = new ReadWriteArrayList<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            range = main.Range(8, 0);
            range.Add(8);
            range.Add(9);
            InterfaceTests.TestEnumerableElements(main, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            InterfaceTests.TestEnumerableElements(range, new int[] { 8, 9 });

            main = new ReadWriteArrayList<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            range = main.Range(0, 4);
            range.Clear();
            Assert.AreEqual(0, range.Count);
            InterfaceTests.TestEnumerableElements(main, new int[] { 4, 5, 6, 7 });
            range.Add(100);
            range.Add(101);
            InterfaceTests.TestEnumerableElements(main, new int[] { 100, 101, 4, 5, 6, 7 });

            main = new ReadWriteArrayList<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            range = main.Range(8, 0);
            InterfaceTests.TestListGeneric(range, new int[] { }, null);
        }

        [TestMethod]
        public void ReadOnlyRange()
        {
            var main = new ReadOnlyArrayList<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            IList<int> range = main.Range(2, 4);

            InterfaceTests.TestReadOnlyListGeneric(range, new int[] { 2, 3, 4, 5 }, null);

            main = new ReadOnlyArrayList<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            range = main.Range(8, 0);
            InterfaceTests.TestReadOnlyListGeneric(range, new int[] { }, null);
        }

        [TestMethod]
        public void RangeExceptions()
        {
            var list = new ReadWriteArrayList<int>(new int[0]);

            for (int i = 0; i < 50; ++i)
                list.Add(i);
            for (int i = 0; i < 50; ++i)
                list.Add(i);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(3, 98);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(-1, 1);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(0, int.MaxValue);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(1, int.MinValue);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(45, int.MinValue);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(0, 101);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(100, 1);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(int.MinValue, 1);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(int.MaxValue, 1);});
        }

        [TestMethod]
        public void ReadOnlyRangeExceptions()
        {
            int[] array = new int[100];
            var list = new ReadOnlyArrayList<int>(array);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(3, 98);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(-1, 1);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(0, int.MaxValue);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(1, int.MinValue);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(45, int.MinValue);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(0, 101);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(100, 1);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(int.MinValue, 1);});

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => {var unused = list.Range(int.MaxValue, 1);});
        }

        [TestMethod]
        public void AsReadOnly()
        {
            int[] elements = new int[400];
            for (int i = 0; i < 400; ++i)
                elements[i] = i;

            var list1 = new ReadWriteArrayList<int>(elements);
            IList<int> list2 = list1.AsReadOnly();

            InterfaceTests.TestReadOnlyListGeneric(list2, elements, null);

            list1.Add(27);
            list1.Insert(0, 98);
            list1[17] = 9;

            elements = new int[402];
            list2 = list1.AsReadOnly();

            for (int i = 0; i < 401; ++i)
                elements[i] = i - 1;

            elements[0] = 98;
            elements[401] = 27;
            elements[17] = 9;

            InterfaceTests.TestReadOnlyListGeneric(list2, elements, null);

            list1 = new ReadWriteArrayList<int>(new int[0]);
            list2 = list1.AsReadOnly();
            InterfaceTests.TestReadOnlyListGeneric(list2, new int[0], null);
            list1.Add(4);
            InterfaceTests.TestReadOnlyListGeneric(list2, new int[] { 4 }, null);
        }

        private void CheckArray<T>(T[] actual, T[] expected)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; ++i) {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public void CopyTo1()
        {
            string[] array1 = { "foo", "bar", "baz", "smell", "the", "glove" };
            var list1 = new ReadWriteArrayList<string>(new string[] { "hello", "Sailor" });
            list1.CopyTo(array1);
            CheckArray(array1, new string[] { "hello", "Sailor", "baz", "smell", "the", "glove" });

            var list2 = new ReadWriteArrayList<string>(new string[0]);
            list2.CopyTo(array1);
            CheckArray(array1, new string[] { "hello", "Sailor", "baz", "smell", "the", "glove" });

            var list3 = new ReadWriteArrayList<string>(new string[] { "a1", "a2", "a3", "a4" });
            list3.CopyTo(array1);
            CheckArray(array1, new string[] { "a1", "a2", "a3", "a4", "the", "glove" });

            var list4 = new ReadWriteArrayList<string>(new string[] { "b1", "b2", "b3", "b4", "b5", "b6" });
            list4.CopyTo(array1);
            CheckArray(array1, new string[] { "b1", "b2", "b3", "b4", "b5", "b6" });

            list1.CopyTo(array1);
            CheckArray(array1, new string[] { "hello", "Sailor", "b3", "b4", "b5", "b6" });

            var list5 = new ReadWriteArrayList<string>(new string[0]);
            string[] array2 = new string[0];
            list5.CopyTo(array2);
            CheckArray(array2, new string[] { });
        }

        [TestMethod]
        public void CopyTo2()
        {
            string[] array1 = { "foo", "bar", "baz", "smell", "the", "glove" };
            var list1 = new ReadWriteArrayList<string>(new string[] { "hello", "Sailor" });
            list1.CopyTo(array1, 3);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "hello", "Sailor", "glove" });

            var list2 = new ReadWriteArrayList<string>(new string[0]);
            list2.CopyTo(array1, 1);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "hello", "Sailor", "glove" });

            var list3 = new ReadWriteArrayList<string>(new string[] { "a1", "a2", "a3", "a4" });
            list3.CopyTo(array1, 2);
            CheckArray(array1, new string[] { "foo", "bar", "a1", "a2", "a3", "a4" });

            var list4 = new ReadWriteArrayList<string>(new string[] { "b1", "b2", "b3", "b4", "b5", "b6" });
            list4.CopyTo(array1, 0);
            CheckArray(array1, new string[] { "b1", "b2", "b3", "b4", "b5", "b6" });

            list1.CopyTo(array1, 4);
            CheckArray(array1, new string[] { "b1", "b2", "b3", "b4", "hello", "Sailor" });

            var list5 = new ReadWriteArrayList<string>(new string[0]);
            string[] array2 = new string[0];
            list5.CopyTo(array2, 0);
            CheckArray(array2, new string[] { });
        }

        [TestMethod]
        public void CopyTo3()
        {
            string[] array1 = { "foo", "bar", "baz", "smell", "the", "glove" };
            var list1 = new ReadWriteArrayList<string>(new string[] { "hello", "Sailor" });
            list1.CopyTo(1, array1, 3, 1);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "Sailor", "the", "glove" });
            list1.CopyTo(0, array1, 5, 1);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "Sailor", "the", "hello" });
            list1.CopyTo(2, array1, 6, 0);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "Sailor", "the", "hello" });

            var list2 = new ReadWriteArrayList<string>(new string[0]);
            list2.CopyTo(0, array1, 1, 0);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "Sailor", "the", "hello" });
            list2.CopyTo(0, array1, 0, 0);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "Sailor", "the", "hello" });
            list2.CopyTo(0, array1, 6, 0);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "Sailor", "the", "hello" });

            var list3 = new ReadWriteArrayList<string>(new string[] { "a1", "a2", "a3", "a4" });
            list3.CopyTo(1, array1, 4, 2);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "Sailor", "a2", "a3" });

            var list4 = new ReadWriteArrayList<string>(new string[] { "b1", "b2", "b3", "b4", "b5", "b6" });
            list4.CopyTo(0, array1, 0, 6);
            CheckArray(array1, new string[] { "b1", "b2", "b3", "b4", "b5", "b6" });

            var list5 = new ReadWriteArrayList<string>(new string[0]);
            string[] array2 = new string[0];
            list5.CopyTo(0, array2, 0, 0);
            CheckArray(array2, new string[] { });
        }


        [TestMethod]
        public void CopyTo1ReadOnly()
        {
            string[] array1 = { "foo", "bar", "baz", "smell", "the", "glove" };
            var list1 = new ReadOnlyArrayList<string>(new string[] { "hello", "Sailor" });
            list1.CopyTo(array1);
            CheckArray(array1, new string[] { "hello", "Sailor", "baz", "smell", "the", "glove" });

            var list2 = new ReadOnlyArrayList<string>(new string[0]);
            list2.CopyTo(array1);
            CheckArray(array1, new string[] { "hello", "Sailor", "baz", "smell", "the", "glove" });

            var list3 = new ReadOnlyArrayList<string>(new string[] { "a1", "a2", "a3", "a4" });
            list3.CopyTo(array1);
            CheckArray(array1, new string[] { "a1", "a2", "a3", "a4", "the", "glove" });

            var list4 = new ReadOnlyArrayList<string>(new string[] { "b1", "b2", "b3", "b4", "b5", "b6" });
            list4.CopyTo(array1);
            CheckArray(array1, new string[] { "b1", "b2", "b3", "b4", "b5", "b6" });

            list1.CopyTo(array1);
            CheckArray(array1, new string[] { "hello", "Sailor", "b3", "b4", "b5", "b6" });

            var list5 = new ReadOnlyArrayList<string>(new string[0]);
            string[] array2 = new string[0];
            list5.CopyTo(array2);
            CheckArray(array2, new string[] { });
        }

        [TestMethod]
        public void CopyTo2ReadOnly()
        {
            string[] array1 = { "foo", "bar", "baz", "smell", "the", "glove" };
            var list1 = new ReadOnlyArrayList<string>(new string[] { "hello", "Sailor" });
            list1.CopyTo(array1, 3);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "hello", "Sailor", "glove" });

            var list2 = new ReadOnlyArrayList<string>(new string[0]);
            list2.CopyTo(array1, 1);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "hello", "Sailor", "glove" });

            var list3 = new ReadOnlyArrayList<string>(new string[] { "a1", "a2", "a3", "a4" });
            list3.CopyTo(array1, 2);
            CheckArray(array1, new string[] { "foo", "bar", "a1", "a2", "a3", "a4" });

            var list4 = new ReadOnlyArrayList<string>(new string[] { "b1", "b2", "b3", "b4", "b5", "b6" });
            list4.CopyTo(array1, 0);
            CheckArray(array1, new string[] { "b1", "b2", "b3", "b4", "b5", "b6" });

            list1.CopyTo(array1, 4);
            CheckArray(array1, new string[] { "b1", "b2", "b3", "b4", "hello", "Sailor" });

            var list5 = new ReadOnlyArrayList<string>(new string[0]);
            string[] array2 = new string[0];
            list5.CopyTo(array2, 0);
            CheckArray(array2, new string[] { });
        }

        [TestMethod]
        public void CopyTo3ReadOnly()
        {
            string[] array1 = { "foo", "bar", "baz", "smell", "the", "glove" };
            var list1 = new ReadOnlyArrayList<string>(new string[] { "hello", "Sailor" });
            list1.CopyTo(1, array1, 3, 1);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "Sailor", "the", "glove" });
            list1.CopyTo(0, array1, 5, 1);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "Sailor", "the", "hello" });
            list1.CopyTo(2, array1, 6, 0);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "Sailor", "the", "hello" });

            var list2 = new ReadOnlyArrayList<string>(new string[0]);
            list2.CopyTo(0, array1, 1, 0);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "Sailor", "the", "hello" });
            list2.CopyTo(0, array1, 0, 0);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "Sailor", "the", "hello" });
            list2.CopyTo(0, array1, 6, 0);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "Sailor", "the", "hello" });

            var list3 = new ReadOnlyArrayList<string>(new string[] { "a1", "a2", "a3", "a4" });
            list3.CopyTo(1, array1, 4, 2);
            CheckArray(array1, new string[] { "foo", "bar", "baz", "Sailor", "a2", "a3" });

            var list4 = new ReadOnlyArrayList<string>(new string[] { "b1", "b2", "b3", "b4", "b5", "b6" });
            list4.CopyTo(0, array1, 0, 6);
            CheckArray(array1, new string[] { "b1", "b2", "b3", "b4", "b5", "b6" });

            var list5 = new ReadOnlyArrayList<string>(new string[0]);
            string[] array2 = new string[0];
            list5.CopyTo(0, array2, 0, 0);
            CheckArray(array2, new string[] { });
        }

        [TestMethod]
        public void Find()
        {
            var list1 = new ReadWriteArrayList<int>(new int[] { 4, 8, 1, 3, 4, 9 });
            bool found;
            int result;

            Assert.AreEqual(1, Enumerable.FirstOrDefault(list1, Odd));
            found = list1.TryFind(Odd, out result);
            Assert.IsTrue(found);
            Assert.AreEqual(1, result);

            Assert.AreEqual(4, Enumerable.FirstOrDefault(list1, Even));
            found = list1.TryFind(Even, out result);
            Assert.IsTrue(found);
            Assert.AreEqual(4, result);

            Assert.AreEqual(0, Enumerable.FirstOrDefault(list1, Over10));
            found = list1.TryFind(Over10, out result);
            Assert.IsFalse(found);
            Assert.AreEqual(0, result);

            list1 = new ReadWriteArrayList<int>(new int[] { 4, 0, 1, 3, 4, 9 });

            Assert.AreEqual(0, Enumerable.FirstOrDefault(list1, Under3));
            found = list1.TryFind(Under3, out result);
            Assert.IsTrue(found);
            Assert.AreEqual(0, result);

            Assert.AreEqual(0, Enumerable.FirstOrDefault(list1, Over10));
            found = list1.TryFind(Over10, out result);
            Assert.IsFalse(found);
            Assert.AreEqual(0, result);

            list1 = new ReadWriteArrayList<int>(new int[0]);

            Assert.AreEqual(0, Enumerable.FirstOrDefault(list1, Under3));
            found = list1.TryFind(Under3, out result);
            Assert.IsFalse(found);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void FindLast()
        {
            var list1 = new ReadWriteArrayList<int>(new int[] { 4, 8, 1, 3, 2, 9 });
            bool found;
            int result;

            Assert.AreEqual(9, Enumerable.LastOrDefault(list1, Odd));
            found = list1.TryFindLast(Odd, out result);
            Assert.IsTrue(found);
            Assert.AreEqual(9, result);

            Assert.AreEqual(2, Enumerable.LastOrDefault(list1, Even));
            found = list1.TryFindLast(Even, out result);
            Assert.IsTrue(found);
            Assert.AreEqual(2, result);

            Assert.AreEqual(0, Enumerable.LastOrDefault(list1, Over10));
            found = list1.TryFindLast(Over10, out result);
            Assert.IsFalse(found);
            Assert.AreEqual(0, result);

            list1 = new ReadWriteArrayList<int>(new int[] { 4, 8, 1, 3, 0, 9 });

            Assert.AreEqual(0, Enumerable.LastOrDefault(list1, Under3));
            found = list1.TryFindLast(Under3, out result);
            Assert.IsTrue(found);
            Assert.AreEqual(0, result);

            Assert.AreEqual(0, Enumerable.LastOrDefault(list1, Over10));
            found = list1.TryFindLast(Over10, out result);
            Assert.IsFalse(found);
            Assert.AreEqual(0, result);

            list1 = new ReadWriteArrayList<int>(new int[0]);

            Assert.AreEqual(0, Enumerable.LastOrDefault(list1, Under3));
            found = list1.TryFindLast(Under3, out result);
            Assert.IsFalse(found);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void FindIndex()
        {
            var list1 = new ReadWriteArrayList<int>(new int[] { 4, 2, 1, 3, 9, 4 });

            Assert.AreEqual(2, list1.FindIndex(Odd));
            Assert.AreEqual(0, list1.FindIndex(Even));
            Assert.AreEqual(-1, list1.FindIndex(Over10));
            Assert.AreEqual(4, list1.FindIndex(Over5));
            Assert.AreEqual(3, list1.FindIndex(3, Odd));
            Assert.AreEqual(5, list1.FindIndex(3, Even));
            Assert.AreEqual(-1, list1.FindIndex(5, Odd));


            Assert.AreEqual(2, list1.FindIndex(1, 4, Odd));
            Assert.AreEqual(3, list1.FindIndex(3, 2, Odd));
            Assert.AreEqual(-1, list1.FindIndex(3, 2, Even));
            Assert.AreEqual(-1, list1.FindIndex(3, 0, Odd));

            list1 = new ReadWriteArrayList<int>(new int[] { 4, 0, 1, 3, 4, 9 });

            Assert.AreEqual(1, list1.FindIndex(Under3));
            Assert.AreEqual(-1, list1.FindIndex(Over10));

            list1 = new ReadWriteArrayList<int>(new int[0]);

            Assert.AreEqual(-1, list1.FindIndex(Under3));
        }

        [TestMethod]
        public void FindLastIndex()
        {
            var list1 = new ReadWriteArrayList<int>(new int[] { 4, 2, 1, 3, 9, 4 });

            Assert.AreEqual(4, list1.FindLastIndex(Odd));
            Assert.AreEqual(5, list1.FindLastIndex(Even));
            Assert.AreEqual(-1, list1.FindLastIndex(Over10));
            Assert.AreEqual(2, list1.FindLastIndex(Under3));
            Assert.AreEqual(3, list1.FindLastIndex(3, Odd));
            Assert.AreEqual(1, list1.FindLastIndex(3, Even));
            Assert.AreEqual(-1, list1.FindLastIndex(1, Odd));
            Assert.AreEqual(1, list1.FindLastIndex(1, Even));
            Assert.AreEqual(-1, list1.FindLastIndex(3, Over10));
            Assert.AreEqual(0, list1.FindLastIndex(3, Over3));

            list1 = new ReadWriteArrayList<int>(new int[] { 4, 0, 8, 3, 4, 9 });

            Assert.AreEqual(3, list1.FindLastIndex(Under4));
            Assert.AreEqual(1, list1.FindLastIndex(Under1));
            Assert.AreEqual(-1, list1.FindLastIndex(Over10));
            Assert.AreEqual(3, list1.FindLastIndex(3, 1, Odd));
            Assert.AreEqual(-1, list1.FindLastIndex(3, 1, Even));
            Assert.AreEqual(4, list1.FindLastIndex(5, 3, Even));
            Assert.AreEqual(2, list1.FindLastIndex(3, 3, Even));
            Assert.AreEqual(2, list1.FindLastIndex(4, 4, Over7));
            Assert.AreEqual(-1, list1.FindLastIndex(3, 4, Over8));

            list1 = new ReadWriteArrayList<int>(new int[0]);

            Assert.AreEqual(-1, list1.FindLastIndex(Under3));
        }

        [TestMethod]
        public void IndexOf()
        {
            var list = new ReadWriteArrayList<int>(new int[] { 4, 8, 1, 1, 4, 9, 7, 11, 4, 9, 1, 7, 19, 1, 7 });
            int index;

            index = list.IndexOf(1);
            Assert.AreEqual(2, index);

            index = list.IndexOf(4);
            Assert.AreEqual(0, index);

            index = list.IndexOf(9);
            Assert.AreEqual(5, index);

            index = list.IndexOf(12);
            Assert.AreEqual(-1, index);

            index = list.IndexOf(1, 10);
            Assert.AreEqual(10, index);

            index = list.IndexOf(1, 11);
            Assert.AreEqual(13, index);

            index = list.IndexOf(1, 10);
            Assert.AreEqual(10, index);

            index = list.IndexOf(7, 12);
            Assert.AreEqual(14, index);

            index = list.IndexOf(9, 3, 3);
            Assert.AreEqual(5, index);

            index = list.IndexOf(9, 3, 2);
            Assert.AreEqual(-1, index);

            index = list.IndexOf(4, 5, 5);
            Assert.AreEqual(8, index);

            index = list.IndexOf(7, 11, 4);
            Assert.AreEqual(11, index);

            index = list.IndexOf(7, 12, 3);
            Assert.AreEqual(14, index);


            list = new ReadWriteArrayList<int>(new int[0]);
            index = list.IndexOf(1);
            Assert.AreEqual(-1, index);
        }

        // Just overrides equal -- no comparison, ordering, or hash code.
#pragma warning disable 659  // missing GetHashCode
        sealed class MyDouble
        {
            double val;

            public MyDouble(double value)
            {
                val = value;
            }

            public override bool Equals(object obj)
            {
                return (obj is MyDouble && ((MyDouble)obj).val == this.val);
            }
        }
#pragma warning restore 659

        [TestMethod]
        public void IndexOf2()
        {
            var list = new ReadWriteArrayList<MyDouble>(new MyDouble[] { new MyDouble(4), new MyDouble(8), new MyDouble(1), new MyDouble(1), new MyDouble(4), new MyDouble(9) });
            int index;

            index = list.IndexOf(new MyDouble(1));
            Assert.AreEqual(2, index);

            index = list.IndexOf(new MyDouble(1.1));
            Assert.IsTrue(index < 0);

            index = list.IndexOf(new MyDouble(4), 2);
            Assert.AreEqual(4, index);

            index = list.IndexOf(new MyDouble(8), 2);
            Assert.IsTrue(index < 0);

            index = list.IndexOf(new MyDouble(1), 1, 3);
            Assert.AreEqual(2, index);

            index = list.IndexOf(new MyDouble(4), 1, 3);
            Assert.IsTrue(index < 0);
        }

        [TestMethod]
        public void LastIndexOf()
        {
            //                                                                        0  1  2  3  4  5  6  7   8  9 10 11 12 13 14
            var list = new ReadWriteArrayList<int>(new int[] { 4, 8, 1, 1, 4, 9, 7, 11, 4, 9, 1, 7, 19, 1, 7 });
            int index;

            index = list.LastIndexOf(1);
            Assert.AreEqual(13, index);

            index = list.LastIndexOf(4);
            Assert.AreEqual(8, index);

            index = list.LastIndexOf(9);
            Assert.AreEqual(9, index);

            index = list.LastIndexOf(12);
            Assert.AreEqual(-1, index);

            index = list.LastIndexOf(1, 13);
            Assert.AreEqual(13, index);

            index = list.LastIndexOf(1, 12);
            Assert.AreEqual(10, index);

            index = list.LastIndexOf(1, 1);
            Assert.AreEqual(-1, index);

            index = list.LastIndexOf(7, 12);
            Assert.AreEqual(11, index);

            index = list.LastIndexOf(7, 6);
            Assert.AreEqual(6, index);

            index = list.LastIndexOf(9, 5, 3);
            Assert.AreEqual(5, index);

            index = list.LastIndexOf(9, 8, 5);
            Assert.AreEqual(5, index);

            index = list.LastIndexOf(9, 3, 2);
            Assert.AreEqual(-1, index);

            index = list.LastIndexOf(4, 5, 6);
            Assert.AreEqual(4, index);

            index = list.LastIndexOf(4, 3, 4);
            Assert.AreEqual(0, index);

            index = list.LastIndexOf(1, 14, 3);
            Assert.AreEqual(13, index);

            index = list.LastIndexOf(1, 0, 0);
            Assert.AreEqual(-1, index);

            index = list.LastIndexOf(1, 14, 0);
            Assert.AreEqual(-1, index);

            list = new ReadWriteArrayList<int>(new int[0]);
            index = list.LastIndexOf(1);
            Assert.AreEqual(-1, index);
        }

        [TestMethod]
        public void LastIndexOf2()
        {
            var list = new ReadWriteArrayList<MyDouble>(new MyDouble[] { new MyDouble(4), new MyDouble(8), new MyDouble(1), new MyDouble(1), new MyDouble(4), new MyDouble(9) });
            int index;

            index = list.LastIndexOf(new MyDouble(1));
            Assert.AreEqual(3, index);

            index = list.LastIndexOf(new MyDouble(1.1));
            Assert.IsTrue(index < 0);

            index = list.LastIndexOf(new MyDouble(4), 2);
            Assert.AreEqual(0, index);

            index = list.LastIndexOf(new MyDouble(9), 2);
            Assert.IsTrue(index < 0);

            index = list.LastIndexOf(new MyDouble(1), 4, 3);
            Assert.AreEqual(3, index);

            index = list.LastIndexOf(new MyDouble(8), 4, 3);
            Assert.IsTrue(index < 0);
        }


        [TestMethod]
        public void FindReadOnly()
        {
            var list1 = new ReadOnlyArrayList<int>(new int[] { 4, 8, 1, 3, 4, 9 });
            bool found;
            int result;

            Assert.AreEqual(1, Enumerable.FirstOrDefault(list1, Odd));
            found = list1.TryFind(Odd, out result);
            Assert.IsTrue(found);
            Assert.AreEqual(1, result);

            Assert.AreEqual(4, Enumerable.FirstOrDefault(list1, Even));
            found = list1.TryFind(Even, out result);
            Assert.IsTrue(found);
            Assert.AreEqual(4, result);

            Assert.AreEqual(0, Enumerable.FirstOrDefault(list1, Over10));
            found = list1.TryFind(Over10, out result);
            Assert.IsFalse(found);
            Assert.AreEqual(0, result);

            list1 = new ReadOnlyArrayList<int>(new int[] { 4, 0, 1, 3, 4, 9 });

            Assert.AreEqual(0, Enumerable.FirstOrDefault(list1, Under3));
            found = list1.TryFind(Under3, out result);
            Assert.IsTrue(found);
            Assert.AreEqual(0, result);

            Assert.AreEqual(0, Enumerable.FirstOrDefault(list1, Over10));
            found = list1.TryFind(Over10, out result);
            Assert.IsFalse(found);
            Assert.AreEqual(0, result);

            list1 = new ReadOnlyArrayList<int>(new int[0]);

            Assert.AreEqual(0, Enumerable.FirstOrDefault(list1, Under3));
            found = list1.TryFind(Under3, out result);
            Assert.IsFalse(found);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void FindLastReadOnly()
        {
            var list1 = new ReadOnlyArrayList<int>(new int[] { 4, 8, 1, 3, 2, 9 });
            bool found;
            int result;

            Assert.AreEqual(9, Enumerable.LastOrDefault(list1, Odd));
            found = list1.TryFindLast(Odd, out result);
            Assert.IsTrue(found);
            Assert.AreEqual(9, result);

            Assert.AreEqual(2, Enumerable.LastOrDefault(list1, Even));
            found = list1.TryFindLast(Even, out result);
            Assert.IsTrue(found);
            Assert.AreEqual(2, result);

            Assert.AreEqual(0, Enumerable.LastOrDefault(list1, Over10));
            found = list1.TryFindLast(Over10, out result);
            Assert.IsFalse(found);
            Assert.AreEqual(0, result);

            list1 = new ReadOnlyArrayList<int>(new int[] { 4, 8, 1, 3, 0, 9 });

            Assert.AreEqual(0, Enumerable.LastOrDefault(list1, Under3));
            found = list1.TryFindLast(Under3, out result);
            Assert.IsTrue(found);
            Assert.AreEqual(0, result);

            Assert.AreEqual(0, Enumerable.LastOrDefault(list1, Over10));
            found = list1.TryFindLast(Over10, out result);
            Assert.IsFalse(found);
            Assert.AreEqual(0, result);

            list1 = new ReadOnlyArrayList<int>(new int[0]);

            Assert.AreEqual(0, Enumerable.LastOrDefault(list1, Under3));
            found = list1.TryFindLast(Under3, out result);
            Assert.IsFalse(found);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void FindIndexReadOnly()
        {
            var list1 = new ReadOnlyArrayList<int>(new int[] { 4, 2, 1, 3, 9, 4 });

            Assert.AreEqual(2, list1.FindIndex(Odd));
            Assert.AreEqual(0, list1.FindIndex(Even));
            Assert.AreEqual(-1, list1.FindIndex(Over10));
            Assert.AreEqual(4, list1.FindIndex(Over5));
            Assert.AreEqual(3, list1.FindIndex(3, Odd));
            Assert.AreEqual(5, list1.FindIndex(3, Even));
            Assert.AreEqual(-1, list1.FindIndex(5, Odd));
            Assert.AreEqual(2, list1.FindIndex(1, 4, Odd));
            Assert.AreEqual(3, list1.FindIndex(3, 2, Odd));
            Assert.AreEqual(-1, list1.FindIndex(3, 2, Even));
            Assert.AreEqual(-1, list1.FindIndex(3, 0, Odd));

            list1 = new ReadOnlyArrayList<int>(new int[] { 4, 0, 1, 3, 4, 9 });

            Assert.AreEqual(1, list1.FindIndex(Under3));
            Assert.AreEqual(-1, list1.FindIndex(Over10));

            list1 = new ReadOnlyArrayList<int>(new int[0]);

            Assert.AreEqual(-1, list1.FindIndex(Under3));
        }

        [TestMethod]
        public void FindLastIndexReadOnly()
        {
            var list1 = new ReadOnlyArrayList<int>(new int[] { 4, 2, 1, 3, 9, 4 });

            Assert.AreEqual(4, list1.FindLastIndex(Odd));
            Assert.AreEqual(5, list1.FindLastIndex(Even));
            Assert.AreEqual(-1, list1.FindLastIndex(Over10));
            Assert.AreEqual(2, list1.FindLastIndex(Under3));
            Assert.AreEqual(3, list1.FindLastIndex(3, Odd));
            Assert.AreEqual(1, list1.FindLastIndex(3, Even));
            Assert.AreEqual(-1, list1.FindLastIndex(1, Odd));
            Assert.AreEqual(1, list1.FindLastIndex(1, Even));
            Assert.AreEqual(-1, list1.FindLastIndex(3, Over10));
            Assert.AreEqual(0, list1.FindLastIndex(3, Over3));

            list1 = new ReadOnlyArrayList<int>(new int[] { 4, 0, 8, 3, 4, 9 });

            Assert.AreEqual(3, list1.FindLastIndex(Under4));
            Assert.AreEqual(1, list1.FindLastIndex(Under1));
            Assert.AreEqual(-1, list1.FindLastIndex(Over10));
            Assert.AreEqual(3, list1.FindLastIndex(3, 1, Odd));
            Assert.AreEqual(-1, list1.FindLastIndex(3, 1, Even));
            Assert.AreEqual(4, list1.FindLastIndex(5, 3, Even));
            Assert.AreEqual(2, list1.FindLastIndex(3, 3, Even));
            Assert.AreEqual(2, list1.FindLastIndex(4, 4, Over7));
            Assert.AreEqual(-1, list1.FindLastIndex(3, 4, Over8));

            list1 = new ReadOnlyArrayList<int>(new int[0]);

            Assert.AreEqual(-1, list1.FindLastIndex(Under3));
        }

        [TestMethod]
        public void IndexOfReadOnly()
        {
            var list = new ReadOnlyArrayList<int>(new int[] { 4, 8, 1, 1, 4, 9, 7, 11, 4, 9, 1, 7, 19, 1, 7 });
            int index;

            index = list.IndexOf(1);
            Assert.AreEqual(2, index);

            index = list.IndexOf(4);
            Assert.AreEqual(0, index);

            index = list.IndexOf(9);
            Assert.AreEqual(5, index);

            index = list.IndexOf(12);
            Assert.AreEqual(-1, index);

            index = list.IndexOf(1, 10);
            Assert.AreEqual(10, index);

            index = list.IndexOf(1, 11);
            Assert.AreEqual(13, index);

            index = list.IndexOf(1, 10);
            Assert.AreEqual(10, index);

            index = list.IndexOf(7, 12);
            Assert.AreEqual(14, index);

            index = list.IndexOf(9, 3, 3);
            Assert.AreEqual(5, index);

            index = list.IndexOf(9, 3, 2);
            Assert.AreEqual(-1, index);

            index = list.IndexOf(4, 5, 5);
            Assert.AreEqual(8, index);

            index = list.IndexOf(7, 11, 4);
            Assert.AreEqual(11, index);

            index = list.IndexOf(7, 12, 3);
            Assert.AreEqual(14, index);


            list = new ReadOnlyArrayList<int>(new int[0]);
            index = list.IndexOf(1);
            Assert.AreEqual(-1, index);
        }

        [TestMethod]
        public void IndexOf2ReadOnly()
        {
            var list = new ReadOnlyArrayList<MyDouble>(new MyDouble[] { new MyDouble(4), new MyDouble(8), new MyDouble(1), new MyDouble(1), new MyDouble(4), new MyDouble(9) });
            int index;

            index = list.IndexOf(new MyDouble(1));
            Assert.AreEqual(2, index);

            index = list.IndexOf(new MyDouble(1.1));
            Assert.IsTrue(index < 0);

            index = list.IndexOf(new MyDouble(4), 2);
            Assert.AreEqual(4, index);

            index = list.IndexOf(new MyDouble(8), 2);
            Assert.IsTrue(index < 0);

            index = list.IndexOf(new MyDouble(1), 1, 3);
            Assert.AreEqual(2, index);

            index = list.IndexOf(new MyDouble(4), 1, 3);
            Assert.IsTrue(index < 0);
        }

        [TestMethod]
        public void LastIndexOfReadOnly()
        {
            //                                                                        0  1  2  3  4  5  6  7   8  9 10 11 12 13 14
            var list = new ReadOnlyArrayList<int>(new int[] { 4, 8, 1, 1, 4, 9, 7, 11, 4, 9, 1, 7, 19, 1, 7 });
            int index;

            index = list.LastIndexOf(1);
            Assert.AreEqual(13, index);

            index = list.LastIndexOf(4);
            Assert.AreEqual(8, index);

            index = list.LastIndexOf(9);
            Assert.AreEqual(9, index);

            index = list.LastIndexOf(12);
            Assert.AreEqual(-1, index);

            index = list.LastIndexOf(1, 13);
            Assert.AreEqual(13, index);

            index = list.LastIndexOf(1, 12);
            Assert.AreEqual(10, index);

            index = list.LastIndexOf(1, 1);
            Assert.AreEqual(-1, index);

            index = list.LastIndexOf(7, 12);
            Assert.AreEqual(11, index);

            index = list.LastIndexOf(7, 6);
            Assert.AreEqual(6, index);

            index = list.LastIndexOf(9, 5, 3);
            Assert.AreEqual(5, index);

            index = list.LastIndexOf(9, 8, 5);
            Assert.AreEqual(5, index);

            index = list.LastIndexOf(9, 3, 2);
            Assert.AreEqual(-1, index);

            index = list.LastIndexOf(4, 5, 6);
            Assert.AreEqual(4, index);

            index = list.LastIndexOf(4, 3, 4);
            Assert.AreEqual(0, index);

            index = list.LastIndexOf(1, 14, 3);
            Assert.AreEqual(13, index);

            index = list.LastIndexOf(1, 0, 0);
            Assert.AreEqual(-1, index);

            index = list.LastIndexOf(1, 14, 0);
            Assert.AreEqual(-1, index);

            list = new ReadOnlyArrayList<int>(new int[0]);
            index = list.LastIndexOf(1);
            Assert.AreEqual(-1, index);
        }

        [TestMethod]
        public void LastIndexOf2ReadOnly()
        {
            var list = new ReadOnlyArrayList<MyDouble>(new MyDouble[] { new MyDouble(4), new MyDouble(8), new MyDouble(1), new MyDouble(1), new MyDouble(4), new MyDouble(9) });
            int index;

            index = list.LastIndexOf(new MyDouble(1));
            Assert.AreEqual(3, index);

            index = list.LastIndexOf(new MyDouble(1.1));
            Assert.IsTrue(index < 0);

            index = list.LastIndexOf(new MyDouble(4), 2);
            Assert.AreEqual(0, index);

            index = list.LastIndexOf(new MyDouble(9), 2);
            Assert.IsTrue(index < 0);

            index = list.LastIndexOf(new MyDouble(1), 4, 3);
            Assert.AreEqual(3, index);

            index = list.LastIndexOf(new MyDouble(8), 4, 3);
            Assert.IsTrue(index < 0);
        }

    }
}

