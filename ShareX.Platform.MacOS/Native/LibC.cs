using System.Runtime.InteropServices;

namespace ShareX.Platform.MacOS.Native;

internal static partial class LibC
{
    [LibraryImport("libc")]
    public static partial uint getuid();
}
