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
            try {
                action();
                Assert.Fail("Should throw");
            }
            catch (Exception e) {
                Assert.IsTrue(e is TException);
            }
        }

        public static void ThrowsInvalid(Action action) =>
            Throws<InvalidProgramException>(action);

        public static void ThrowsOutOfRange(Action action) =>
            Throws<ArgumentOutOfRangeException>(action);
    }
}
