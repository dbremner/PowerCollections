﻿//******************************
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

#endregion

namespace Wintellect.PowerCollections.Tests
{
    [TestClass]
    public class UtilTests
    {
#pragma warning disable 649
        private struct StructType {
            public int i;
        }

        private class ClassType {
            public int i;
        }

        public struct CloneableStruct : ICloneable
        {
            public int value;
            public int tweak;

            public CloneableStruct(int v)
            {
                value = v;
                tweak = 1;
            }

            public object Clone()
            {
                CloneableStruct newStruct;
                newStruct.value = value;
                newStruct.tweak = tweak + 1;
                return newStruct;
            }

            public bool Identical(CloneableStruct other)
            {
                return value == other.value && tweak == other.tweak;
            }

            public override bool Equals(object obj)
            {
                if (! (obj is CloneableStruct))
                    return false;
                var o = (CloneableStruct)obj;

                return (o.value == value);
            }

            public override int GetHashCode()
            {
                return value.GetHashCode();
            }
        }

#pragma warning restore 649

        [TestMethod]
        public void IsCloneableType()
        {
            bool isValue;

            bool isCloneable = Util.IsCloneableType(typeof(int), out isValue);
            Assert.IsTrue(isCloneable); Assert.IsTrue(isValue);

            isCloneable = Util.IsCloneableType(typeof(ICloneable), out isValue);
            Assert.IsTrue(isCloneable); Assert.IsFalse(isValue);

            isCloneable = Util.IsCloneableType(typeof(StructType), out isValue);
            Assert.IsTrue(isCloneable); Assert.IsTrue(isValue);

            isCloneable = Util.IsCloneableType(typeof(ClassType), out isValue);
            Assert.IsFalse(isCloneable); Assert.IsFalse(isValue);

            isCloneable = Util.IsCloneableType(typeof(ArrayList), out isValue);
            Assert.IsTrue(isCloneable); Assert.IsFalse(isValue);

            isCloneable = Util.IsCloneableType(typeof(CloneableStruct), out isValue);
            Assert.IsTrue(isCloneable); Assert.IsFalse(isValue);
        }

        [TestMethod]
        public void WrapEnumerable()
        {
            IEnumerable<int> enum1 = new List<int>(new int[] { 1, 4, 5, 6, 9, 1 });
            IEnumerable<int> enum2 = Util.CreateEnumerableWrapper(enum1);
            InterfaceTests.TestEnumerableElements(enum2, new int[] { 1, 4, 5, 6, 9, 1 });
        }

        [TestMethod]
        public void TestGetHashCode()
        {
            int result = Util.GetHashCode("foo", EqualityComparer<string>.Default);
            Assert.AreEqual(result, "foo".GetHashCode());
            result = Util.GetHashCode(null, EqualityComparer<string>.Default);
            Assert.AreEqual(result, 0x1786E23C);
            int r1 = Util.GetHashCode("Banana", StringComparer.InvariantCultureIgnoreCase);
            int r2 = Util.GetHashCode("banANA", StringComparer.InvariantCultureIgnoreCase);
            Assert.AreEqual(r1, r2);
        }
    }
}

