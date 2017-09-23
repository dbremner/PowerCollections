using System;
using System.Collections.Generic;

namespace Wintellect.PowerCollections.Tests {
    internal static class Bag {
        public static Bag<T> Create<T>(IEnumerable<T> collection) {
            return new Bag<T>(collection);
        }
    }

    internal static class BigList {
        public static BigList<T> Create<T>(IEnumerable<T> collection) {
            return new BigList<T>(collection);
        }
    }

    internal static class Deque {
        public static Deque<T> Create<T>(IEnumerable<T> collection) {
            return new Deque<T>(collection);
        }
    }

    internal static class OrderedBag {
        public static OrderedBag<T> Create<T>(IEnumerable<T> collection) {
            return new OrderedBag<T>(collection);
        }
    }

    internal static class OrderedSet {
        public static OrderedSet<T> Create<T>(IEnumerable<T> collection) {
            return new OrderedSet<T>(collection);
        }
    }

    internal static class Set {
        public static Set<T> Create<T>(IEnumerable<T> sequence) {
            return new Set<T>(sequence);
        }
    }
}
