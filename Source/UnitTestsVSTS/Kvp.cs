using System.Collections.Generic;

namespace Wintellect.PowerCollections.Tests {
    internal static class Kvp {
        public static KeyValuePair<TKey, TValue> Of<TKey, TValue>(TKey key, TValue value) {
            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }
}
