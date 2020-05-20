# Details About the Plug-In

## Architecture
The plug-in is built around the architecture shown in the following diagram:


<p align="center">
  <img src="https://github.com/cwford/TDAmTrade_Zorro_Plugin/blob/master/Documentation/Images/Architecture.png">
</p>

### How the Plug-In Works
The Zorro trading engine expects to be able to call entry points in a 32-bit broker plug-in with standard C calling conventions. <em>DllExport</em>, the 3rd party library referenced below, allows for a C# program to create endpoints in a DLL which can be called with standard conventions code written in C++. Of course, the plug-in must be compiled for an x86 (32-bit) platform.

Even given <em>DllExport</em>, there is also the question of how input and output parameters are passed to the broker plug-in methods called by the Zorro trading engine. Most input parameters to the broker plug-in methods are integer values which can be passed as int32 by the trading engine and received as int32 by the C# methods in the plug-in. But some of the input parameters, and all of the output parameters, are passed as C++ pointers.

C++ pointers, particularly when they point to double values, are sometimes represented as an array of doubles in managed code (C#). The first element of the array is at the address pointed to by the pointer passed to the C# method, so assigning a value to the first element should, in theory, place it at the memory location allocated for it by the Zorro trading engine.

The problem with using arrays of doubles, in lieu of pointers, is that it is difficult, if not impossible, to check for the validity of the addresses they point to. Occassionally, Zorro will pass a null pointer when the C# code is expecting a vaild address for the start of an array. Assigning a value to a null pointer results in a runtime error.

So, instead of reperesenting output pointer variables, and some input pointer variables, as arrays of doubles, the plug-in represents these variables as **IntPtr**. Then, using an **unsafe** code block, the plug-in casts them to the correct pointer type and tests for them being null prior to assigning a value to the address they point to. Thus, the broker plug-in solution must be compiled with the **unsafe** checkbox checked.


### Modifying the C# Code
To modify this broker plug-in do the following:

1. Clone the repository or download the zip file
2. Open the solution in a recent version of Visual Studio.
3. Make any required changes to the plug-in code.
4. Double-check that you are building for an x86 (32-bit( platform.
5. Double-check that the **unsafe** checkbox has been checked so the compiler will compile unmanaged code.
6. Re-build the plug-in and eliminate any errors.
7. Copy the plug-in to the Zorro plug-in folder.
8. Test the plug-in's operation.

### Dependencies
#### DllExport
The plug-in is dependent upon the 3rd party library, <em>DllExport</em>, which can be found on NuGet, or at:

  https://github.com/3F/DllExport/releases

**NOTE:** There is another DllExport library by Robert Giesecke, entitled <em>UnmanagedExports</em> which has  similar namespaces and similar methods. **DO NOT USE this DLL only use the DLL referenced above.**

When installing <em>DllExport</em> a configuration screen will pop-up. You can simply press the **Apply** button to accept the default configuration of <em>DllExport</em>.

#### Sqlite Dependencies
The plug-in uses Sqlite as an embedded database, and Microsoft.Data.Sqlite should be installed as a NuGet package.

**NOTE:** Only Microsoft.Data.Sqlite version 1.1.1 works and has been tested with this plug-in. Later versions will resolve all namespaces but produce runtime dependency errors.

#### DLLs Copied to Zorro
The Visual Studio solution for building the plug-in copies the following three files to the Zorro Plugin Folder:

|**In Visual Studio Solution**|**Copied to Zorro Plugin Folder As**|**Description**
|----------------------|--------------------------------|---------------------|
|TDAmeritradeZorro.dll|TDAmeritrade.dll|Main broker plug-in|
|Microsoft.Data.Sqlite (v 1.1.1)|Microsoft.Data.Sqlite|Microsoft Sqlite ADO.NET library|
|sqlite3.dll|sqlite3.dll|Native Sqlite library|
|DBLib.dll|DbLib.dll|Data access library for Sqlite|

**NOTE:** These three files must be present in the Zorro plug-in folder for this plug-in to work.

### Caution
The developer found several instances where even commonly used methods from standard Windows .NET libraries did not interact well with the DllExport library. Again, no attempt was made to find solutions or workarounds to these issues. Instead, different means of accomplishing the same task were tried until one was found that worked. Other developers should be cautious in using Windows classes or methods they are familiar with in this plug-in. Extensive testing is necessary to confirm that any additions to this plug-in, which rely on classes not already used in this plug-in, actually work. Most often, the DLL containing these new classes must be copied to the Zorro plug-in folder.

### Documentation
The code for this plug-in is extensively documented with one exception. The JSON objects returned from the TD Ameritrade API are not well-documented because there is little documentation to be found about these objects and their many properties. They are, however, converted into .NET class objects by a JSON converter and the information the Zorro trading engine needs from them is then extracted.

**If you plan on making modifications to the plug-in code, please consider retaining the look and feel of the current documentation so that anyone coming after you will have an easy time making modifications.**

