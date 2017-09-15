//******************************
// Written by Peter Golde
// Copyright (c) 2004-2007, Wintellect
//
// Use and restribution of this code is subject to the license agreement 
// contained in the file "License.txt" accompanying this file.
//******************************

using System.Reflection;
using System.Runtime.CompilerServices;

// Make internals of this library available to the unit test framework.
// NOTE: If you are building the PowerCollections with your own key you will need to change the public key below.
[assembly: InternalsVisibleTo("Wintellect.PowerCollections.Tests")]

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("Wintellect.PowerCollections")]
[assembly: AssemblyDescription("The Power Collections group of collection classes.")]
#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("")]
#endif
[assembly: AssemblyCompany("Wintellect")]
[assembly: AssemblyProduct("Power Collections")]
[assembly: AssemblyCopyright("Copyright (c) 2004-2007, Wintellect")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("1.0.*")]

