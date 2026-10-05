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

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct DXGI_OUTPUT_DESC
{
    public fixed char DeviceName[32];
    public RECT DesktopCoordinates;
    public BOOL AttachedToDesktop;
    public int Rotation;
    public nint Monitor;
}

[StructLayout(LayoutKind.Sequential)]
internal struct DXGI_SAMPLE_DESC { public uint Count, Quality; }

[StructLayout(LayoutKind.Sequential)]
internal struct DXGI_RATIONAL { public uint Numerator, Denominator; }

[StructLayout(LayoutKind.Sequential)]
internal struct D3D11_TEXTURE2D_DESC
{
    public uint Width, Height, MipLevels, ArraySize;
    public DXGI_FORMAT Format;
    public DXGI_SAMPLE_DESC SampleDesc;
    public D3D11_USAGE Usage;
    public D3D11_BIND_FLAG BindFlags;
    public uint CPUAccessFlags, MiscFlags;
}

[StructLayout(LayoutKind.Sequential)]
internal struct D3D11_BOX { public uint left, top, front, right, bottom, back; }

[StructLayout(LayoutKind.Sequential)]
internal struct D3D11_VIDEO_PROCESSOR_CONTENT_DESC
{
    public D3D11_VIDEO_FRAME_FORMAT InputFrameFormat;
    public DXGI_RATIONAL InputFrameRate;
    public uint InputWidth, InputHeight;
    public DXGI_RATIONAL OutputFrameRate;
    public uint OutputWidth, OutputHeight;
    public D3D11_VIDEO_USAGE Usage;
}

[StructLayout(LayoutKind.Sequential)]
internal struct D3D11_VIDEO_PROCESSOR_INPUT_VIEW_DESC
{
    public uint FourCC;
    public D3D11_VPIV_DIMENSION ViewDimension;
    public uint MipSlice, ArraySlice;
}

[StructLayout(LayoutKind.Sequential)]
internal struct D3D11_VIDEO_PROCESSOR_OUTPUT_VIEW_DESC
{
    public D3D11_VPOV_DIMENSION ViewDimension;
    // The native union also holds a three-UINT Texture2DArray descriptor.
    public uint MipSlice, FirstArraySlice, ArraySize;
}

[StructLayout(LayoutKind.Sequential)]
internal struct D3D11_VIDEO_PROCESSOR_COLOR_SPACE
{
    private uint bits;
    public bool RGB_Range { readonly get => (bits & 2) != 0; set => bits = value ? bits | 2 : bits & ~2u; }
    public bool YCbCr_Matrix { readonly get => (bits & 4) != 0; set => bits = value ? bits | 4 : bits & ~4u; }
    public byte Nominal_Range { readonly get => (byte)((bits >> 4) & 3); set => bits = (bits & ~0x30u) | ((uint)(value & 3) << 4); }
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct D3D11_VIDEO_PROCESSOR_STREAM
{
    public BOOL Enable;
    public uint OutputIndex, InputFrameOrField, PastFrames, FutureFrames;
    public ID3D11VideoProcessorInputView** ppPastSurfaces;
    public ID3D11VideoProcessorInputView* pInputSurface;
    public ID3D11VideoProcessorInputView** ppFutureSurfaces, ppPastSurfacesRight;
    public ID3D11VideoProcessorInputView* pInputSurfaceRight;
    public ID3D11VideoProcessorInputView** ppFutureSurfacesRight;
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