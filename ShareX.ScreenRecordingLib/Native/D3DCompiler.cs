// SPDX-License-Identifier: GPL-3.0-or-later
using System.Runtime.InteropServices;

namespace ShareX.ScreenRecordingLib.Native;

internal static unsafe partial class NativeMethods
{
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("d3dcompiler_47.dll", ExactSpelling = true, CharSet = CharSet.Ansi)]
    public static extern HRESULT D3DCompile(void* source, nuint sourceLength, string? sourceName,
        nint defines, nint include, string entryPoint, string target, uint flags, uint effectFlags,
        out nint code, out nint errors);
}
