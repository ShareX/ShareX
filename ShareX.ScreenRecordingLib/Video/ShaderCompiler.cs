#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using ShareX.ScreenRecordingLib.Native;
using System.Runtime.InteropServices;
using Vortice.Direct3D;

namespace ShareX.ScreenRecordingLib.Video;

internal static class ShaderCompiler
{
    /// <summary>Compiles HLSL with Windows' built-in compiler, so no shader build step or redistributable is needed.</summary>
    public static unsafe byte[] Compile(string hlsl, string name, string entryPoint, string target)
    {
        byte[] source = System.Text.Encoding.ASCII.GetBytes(hlsl);
        fixed (byte* data = source)
        {
            HRESULT result = NativeMethods.D3DCompile(data, (nuint)source.Length, name, 0, 0,
                entryPoint, target, (1u << 11) | (1u << 15), 0, out nint code, out nint errors); // Strictness, optimization level 3.
            using Blob? bytecode = code == 0 ? null : new(code);
            using Blob? messages = errors == 0 ? null : new(errors);
            if (result.Failed)
                throw new InvalidOperationException($"Could not compile the {name} shader. {Marshal.PtrToStringAnsi(messages?.BufferPointer ?? 0)}",
                    Marshal.GetExceptionForHR(result.Value));
            if (bytecode == null) throw new InvalidOperationException($"Windows did not return {name} shader bytecode.");
            return new ReadOnlySpan<byte>((void*)bytecode.BufferPointer, checked((int)(nuint)bytecode.BufferSize)).ToArray();
        }
    }
}
