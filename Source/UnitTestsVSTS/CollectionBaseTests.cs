//******************************
// Written by Peter Golde
// Copyright (c) 2004-2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement
// contained in the file "License.txt" accompanying this file.
//******************************

using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Wintellect.PowerCollections.Tests.TestPredicates;

namespace Wintellect.PowerCollections.Tests
{
    // A simple read-write collection.
    internal class ReadWriteTestCollection<T> : CollectionBase<T>
    {
        private List<T> items;

        public ReadWriteTestCollection(T[] items)
        {
            this.items = new List<T>(items);
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public override void Add(T item)
        {
            items.Add(item);
        }

        public override bool Remove(T item)
        {
            return items.Remove(item);
        }

        public override void Clear()
        {
            items.Clear();
        }
    }

    [TestClass]
    public class CollectionBaseTests
    {
        // A simple read-only collection.
        private class ReadOnlyTestCollection<T> : ReadOnlyCollectionBase<T>
        {
            private T[] items;

            public ReadOnlyTestCollection(T[] items)
            {
                this.items = items;
            }

            public override int Count
            {
                get { return items.Length; }
            }

            public override IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < items.Length; ++i)
                    yield return items[i];
            }
        }

        [TestMethod]
        public void ReadOnlyCollection()
        {
            string[] s = { "Hello", "Goodbye", "Eric", "Clapton", "Rules" };

            var coll = new ReadOnlyTestCollection<string>(s);

            InterfaceTests.TestCollection((ICollection)coll, s, true);
            InterfaceTests.TestReadonlyCollectionGeneric((ICollection<string>)coll, s, true, "ReadOnlyTestCollection");
        }

        [TestMethod]
        public void ReadWriteCollection()
        {
            string[] s = { "Hello", "Goodbye", "Eric", "Clapton", "Rules" };

            var coll = new ReadWriteTestCollection<string>(s);

            InterfaceTests.TestCollection((ICollection)coll, s, true);
            InterfaceTests.TestReadWriteCollectionGeneric((ICollection<string>)coll, s, true);
        }

        [TestMethod]
        public void ConvertToString()
        {
            string[] array = { "Hello", "Goodbye", null, "Clapton", "Rules" };

            var coll1 = new ReadWriteTestCollection<string>(array);
            string s = coll1.ToString();
            Assert.AreEqual("{Hello,Goodbye,null,Clapton,Rules}", s);

            var coll2 = new ReadOnlyTestCollection<string>(array);
            s = coll2.ToString();
            Assert.AreEqual("{Hello,Goodbye,null,Clapton,Rules}", s);

            var coll3 = new ReadWriteTestCollection<string>(new string[0]);
            s = coll3.ToString();
            Assert.AreEqual("{}", s);

            var coll4= new ReadOnlyTestCollection<string>(new string[0]);
            s = coll4.ToString();
            Assert.AreEqual("{}", s);

            var coll5 = new ReadWriteTestCollection<int>(new int[] { 1, 2, 3 });
            s = coll5.ToString();
            Assert.AreEqual("{1,2,3}", s);

            var coll6 = new ReadOnlyTestCollection<int>(new int[] { 1, 2, 3 });
            s = coll6.ToString();
            Assert.AreEqual("{1,2,3}", s);
        }

        [TestMethod]
        public void DebuggerDisplay()
        {
            string[] array = { "Hello", "Goodbye", null, "Clapton", "Rules" };
            string s;

            var coll1 = new ReadWriteTestCollection<string>(array);
            s = coll1.DebuggerDisplayString();
            Assert.AreEqual("{Hello,Goodbye,null,Clapton,Rules}", s);

            var coll2 = new ReadOnlyTestCollection<string>(array);
            s = coll2.DebuggerDisplayString();
            Assert.AreEqual("{Hello,Goodbye,null,Clapton,Rules}", s);

            var coll3 = new ReadWriteTestCollection<string>(new string[0]);
            s = coll3.DebuggerDisplayString();
            Assert.AreEqual("{}", s);

            var coll4 = new ReadOnlyTestCollection<string>(new string[0]);
            s = coll4.DebuggerDisplayString();
            Assert.AreEqual("{}", s);

            var coll5 = new ReadWriteTestCollection<int>(new int[] { 1, 2, 3 });
            s = coll5.DebuggerDisplayString();
            Assert.AreEqual("{1,2,3}", s);

            var coll6 = new ReadOnlyTestCollection<int>(new int[] { 1, 2, 3 });
            s = coll6.DebuggerDisplayString();
            Assert.AreEqual("{1,2,3}", s);

            int[] bigarray = new int[1000];
            for (int i = 0; i < bigarray.Length; ++i)
                bigarray[i] = i;

            string expected = "{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,...}";

            var coll7 = new ReadWriteTestCollection<int>(bigarray);
            s = coll7.DebuggerDisplayString();
            Assert.AreEqual(expected, s);

            var coll8 = new ReadOnlyTestCollection<int>(bigarray);
            s = coll8.DebuggerDisplayString();
            Assert.AreEqual(expected, s);
        }

        // Tests the built-in List<T> class. Makes sure that our tests are reasonable.
        [TestMethod]
        public void CheckList()
        {
            string[] s = { "Hello", "Goodbye", "Eric", "Clapton", "Rules" };

            var coll = new List<string>(s);

            InterfaceTests.TestCollection((ICollection)coll, s, true);
            InterfaceTests.TestReadWriteCollectionGeneric((ICollection<string>)coll, s, true);

            IList<string> ro = new List<string>(s).AsReadOnly();
            InterfaceTests.TestReadonlyCollectionGeneric(ro, s, true, null);
        }

        // Tests the Keys and Values collections of Dictionary. Makes sure that our tests are reasonable.
        [TestMethod]
        public void CheckDictionaryKeyValues()
        {
            var dict = new Dictionary<string, int> {
                ["Eric"] = 3,
                ["Clapton"] = 1,
                ["Rules"] = 4,
                ["The"] = 1,
                ["Universe"] = 5
            };

            InterfaceTests.TestCollection(dict.Keys, new string[] { "Eric", "Clapton", "Rules", "The", "Universe" }, false);
            InterfaceTests.TestReadonlyCollectionGeneric(dict.Keys, new string[] { "Eric", "Clapton", "Rules", "The", "Universe" }, false, null);
            InterfaceTests.TestCollection(dict.Values, new int[] { 1, 1, 3, 4, 5 }, false);
            InterfaceTests.TestReadonlyCollectionGeneric(dict.Values, new int[] { 1, 1, 3, 4, 5 }, false, null);
        }

        [TestMethod]
        public void RemoveAll()
        {
            var coll1 = new ReadWriteTestCollection<double>(new double[] { 4.5, 1.2, 7.6, -0.04, -7.6, 1.78, 10.11, 187.4 });

            coll1.RemoveAll(AbsOver5);
            InterfaceTests.TestReadWriteCollectionGeneric(coll1, new double[] { 4.5, 1.2,  -0.04, 1.78 }, true, null);

            coll1 = new ReadWriteTestCollection<double>(new double[] { 4.5, 1.2, 7.6, -0.04, -7.6, 1.78, 10.11, 187.4 });
            coll1.RemoveAll(IsZero);
            InterfaceTests.TestReadWriteCollectionGeneric(coll1, new double[] { 4.5, 1.2, 7.6, -0.04, -7.6, 1.78, 10.11, 187.4 }, true, null);

            coll1 = new ReadWriteTestCollection<double>(new double[] { 4.5, 1.2, 7.6, -0.04, -7.6, 1.78, 10.11, 187.4 });
            coll1.RemoveAll(Under200);
            Assert.AreEqual(0, coll1.Count);
        }

        [TestMethod]
        public void AsReadOnly()
        {
            int[] elements = new int[400];
            for (int i = 0; i < 400; ++i)
                elements[i] = i;

            var coll1 = new ReadWriteTestCollection<int>(elements);
            ICollection<int> coll2 = coll1.AsReadOnly();

            InterfaceTests.TestReadonlyCollectionGeneric(coll2, elements, true, null);

            coll1.Add(27);
            coll1.Add(199);

            elements = new int[402];
            coll2 = coll1.AsReadOnly();

            for (int i = 0; i < 400; ++i)
                elements[i] = i;

            elements[400] = 27;
            elements[401] = 199;

            InterfaceTests.TestReadonlyCollectionGeneric(coll2, elements, true, null);

            coll1 = new ReadWriteTestCollection<int>(new int[0]);
            coll2 = coll1.AsReadOnly();
            InterfaceTests.TestReadonlyCollectionGeneric(coll2, new int[0], true, null);
            coll1.Add(4);
            InterfaceTests.TestReadonlyCollectionGeneric(coll2, new int[] { 4 }, true, null);
        }
    }
}

