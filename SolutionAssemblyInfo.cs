using System.Reflection;
using System.Runtime.InteropServices;

#if DEBUG

[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyDescription("Debug build, https://github.com/SleepyBag/Paletteau")]
#else
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("Release build, https://github.com/SleepyBag/Paletteau")]
#endif

[assembly: AssemblyCompany("Paletteau")]
[assembly: AssemblyProduct("Paletteau")]
[assembly: AssemblyCopyright("The MIT License (MIT)")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.5.0")]
[assembly: AssemblyFileVersion("1.5.0")]
[assembly: AssemblyInformationalVersion("1.5.0")]