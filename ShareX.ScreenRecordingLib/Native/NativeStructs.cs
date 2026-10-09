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

using System.Runtime.InteropServices;

namespace ShareX.ScreenRecordingLib.Native;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct HRESULT(int value)
{
    public readonly int Value = value;
    public bool Succeeded => Value >= 0;
    public bool Failed => Value < 0;
    public void ThrowOnFailure() { if (Failed) Marshal.ThrowExceptionForHR(Value); }
    public static implicit operator int(HRESULT value) => value.Value;
}

[StructLayout(LayoutKind.Sequential)]
internal readonly struct BOOL(int value)
{
    public readonly int Value = value;
    public static implicit operator BOOL(bool value) => new(value ? 1 : 0);
    public static implicit operator bool(BOOL value) => value.Value != 0;
}

[StructLayout(LayoutKind.Sequential)]
internal struct RECT(int left, int top, int right, int bottom)
{
    public int left = left, top = top, right = right, bottom = bottom;
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct WAVEFORMATEX
{
    public ushort wFormatTag, nChannels;
    public uint nSamplesPerSec, nAvgBytesPerSec;
    public ushort nBlockAlign, wBitsPerSample, cbSize;
}

// VARIANT is 24 bytes on the supported 64-bit Windows architectures (x64 and ARM64).
[StructLayout(LayoutKind.Explicit, Size = 24)]
internal struct VARIANT
{
    [FieldOffset(0)] public VARENUM vt;
    [FieldOffset(8)] public short boolVal;
    [FieldOffset(8)] public uint ulVal;
}
