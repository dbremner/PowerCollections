//******************************
// Written by Peter Golde
// Copyright (c) 2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement
// contained in the file "License.txt" accompanying this file.
//******************************

using System;

[assembly: CLSCompliant(true)]

namespace Wintellect.PowerCollections.Tests
{
    internal class MainProgram
    {
        public static void Main()
        {
            new OrderedMultiDictionaryTests().SerializeStrings();
        }
    }
}
