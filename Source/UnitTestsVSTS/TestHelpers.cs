using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wintellect.PowerCollections.Tests {
    internal static class TestHelpers {
        /// <summary>
        /// Helper function that needs to go away.
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action"></param>
        public static void Throws<TException>(Action action) where TException : Exception {
            Assert.ThrowsException<TException>(action);
        }

        public static void ThrowsInvalid(Action action) =>
            Assert.ThrowsException<InvalidOperationException>(action);

        public static void ThrowsInvalidResult<T>(Func<T> func) =>
            Assert.ThrowsException<InvalidOperationException>(() => func());

        public static void ThrowsOutOfRange(Action action) =>
            Assert.ThrowsException<ArgumentOutOfRangeException>(action);

        public static void ThrowsOutOfRangeResult<T>(Func<T> func) =>
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => func());
    }
}
