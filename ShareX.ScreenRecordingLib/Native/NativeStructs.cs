// SPDX-License-Identifier: GPL-3.0-or-later
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
