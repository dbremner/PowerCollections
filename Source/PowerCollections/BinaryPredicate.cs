using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wintellect.PowerCollections
{
    /// <summary>
    /// The BinaryPredicate delegate type  encapsulates a method that takes two
    /// items of the same type, and returns a boolean value representating 
    /// some relationship between them. For example, checking whether two
    /// items are equal or equivalent is one kind of binary predicate.
    /// </summary>
    /// <param name="item1">The first item.</param>
    /// <param name="item2">The second item.</param>
    /// <returns>Whether item1 and item2 satisfy the relationship that the BinaryPredicate defines.</returns>
    public delegate bool BinaryPredicate<T>(T item1, T item2);
}