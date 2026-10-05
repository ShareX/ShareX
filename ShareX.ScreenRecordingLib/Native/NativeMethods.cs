// SPDX-License-Identifier: GPL-3.0-or-later
using System.Runtime.InteropServices;

namespace ShareX.ScreenRecordingLib.Native;

internal static unsafe partial class NativeMethods
{
    [DllImport("ole32.dll", ExactSpelling = true)]
    public static extern HRESULT CoInitializeEx(void* reserved, COINIT flags);

    [DllImport("ole32.dll", ExactSpelling = true)]
    public static extern void CoUninitialize();

    [DllImport("ole32.dll", ExactSpelling = true)]
    public static extern HRESULT CoCreateInstance(Guid* clsid, IUnknown* outer, CLSCTX context, Guid* iid, void** instance);

    [DllImport("d3d11.dll", ExactSpelling = true)]
    public static extern HRESULT D3D11CreateDevice(IDXGIAdapter1* adapter, D3D_DRIVER_TYPE driverType, nint software,
        D3D11_CREATE_DEVICE_FLAG flags, D3D_FEATURE_LEVEL* featureLevels, uint featureLevelCount, uint sdkVersion,
        ID3D11Device** device, D3D_FEATURE_LEVEL* featureLevel, ID3D11DeviceContext** context);

    [DllImport("dxgi.dll", ExactSpelling = true)]
    public static extern HRESULT CreateDXGIFactory1(Guid* iid, void** factory);

    [DllImport("d3d11.dll", ExactSpelling = true)]
    public static extern HRESULT CreateDirect3D11DeviceFromDXGIDevice(IDXGIDevice* device, IUnknown** inspectable);

    [DllImport("combase.dll", ExactSpelling = true)]
    public static extern HRESULT WindowsCreateString(char* value, uint length, nint* result);

    [DllImport("combase.dll", ExactSpelling = true)]
    public static extern HRESULT WindowsDeleteString(nint value);

    [DllImport("combase.dll", ExactSpelling = true)]
    public static extern HRESULT RoGetActivationFactory(nint className, Guid* iid, void** factory);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumDisplayMonitors(nint dc, RECT* clip, delegate* unmanaged[Stdcall]<nint, nint, RECT*, nint, BOOL> callback, nint state);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindow(nint window);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern nint MonitorFromWindow(nint window, MONITOR_FROM_FLAGS flags);

    [DllImport("mfplat.dll", ExactSpelling = true)]
    public static extern HRESULT MFStartup(uint version, uint flags);

    [DllImport("mfplat.dll", ExactSpelling = true)]
    public static extern HRESULT MFShutdown();

    [DllImport("mfplat.dll", ExactSpelling = true)]
    public static extern HRESULT MFCreateAttributes(IMFAttributes** attributes, uint initialSize);

    [DllImport("mfplat.dll", ExactSpelling = true)]
    public static extern HRESULT MFCreateMediaType(IMFMediaType** mediaType);

    [DllImport("mfreadwrite.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    public static extern HRESULT MFCreateSinkWriterFromURL(string url, IUnknown* byteStream, IMFAttributes* attributes, IMFSinkWriter** writer);

    [DllImport("mfplat.dll", ExactSpelling = true)]
    public static extern HRESULT MFCreateDXGIDeviceManager(uint* resetToken, IMFDXGIDeviceManager** deviceManager);

    [DllImport("mfplat.dll", ExactSpelling = true)]
    public static extern HRESULT MFCreateDXGISurfaceBuffer(Guid* iid, IUnknown* surface, uint subresource,
        [MarshalAs(UnmanagedType.Bool)] bool bottomUp, IMFMediaBuffer** buffer);

    [DllImport("evr.dll", ExactSpelling = true)]
    public static extern HRESULT MFCreateVideoSampleFromSurface(IUnknown* surface, IMFSample** sample);

    [DllImport("mfplat.dll", ExactSpelling = true)]
    public static extern HRESULT MFCreateSample(IMFSample** sample);

    [DllImport("mfplat.dll", ExactSpelling = true)]
    public static extern HRESULT MFCreateMemoryBuffer(uint maximumLength, IMFMediaBuffer** buffer);

    [DllImport("avrt.dll", EntryPoint = "AvSetMmThreadCharacteristicsW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern nint AvSetMmThreadCharacteristics(string taskName, ref uint taskIndex);

    [DllImport("avrt.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AvRevertMmThreadCharacteristics(nint task);

    [DllImport("kernel32.dll", EntryPoint = "CreateWaitableTimerExW", ExactSpelling = true, SetLastError = true)]
    public static extern nint CreateWaitableTimerEx(void* attributes, char* name, uint flags, uint access);

    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWaitableTimerEx(nint timer, long* dueTime, int period, void* completionRoutine, void* argument, void* wakeContext, uint tolerableDelay);
}