// SPDX-License-Identifier: GPL-3.0-or-later
using System.Runtime.InteropServices;

namespace ShareX.ScreenRecordingLib.Native;

// These structs describe native COM objects, not managed COM implementations. Use them only through
// pointers owned by ComPtr<T>. Slots include all inherited methods, including those we do not call.
[Guid("00000000-0000-0000-c000-000000000046")]
internal unsafe struct IUnknown
{
    public void** Vtable;
    public HRESULT QueryInterface(Guid* iid, void** result)
    {
        fixed (IUnknown* self = &this)
            return ((delegate* unmanaged[Stdcall]<IUnknown*, Guid*, void**, HRESULT>)Vtable[0])(self, iid, result);
    }
    public uint Release()
    {
        fixed (IUnknown* self = &this)
            return ((delegate* unmanaged[Stdcall]<IUnknown*, uint>)Vtable[2])(self);
    }
}
[Guid("3628e81b-3cac-4c60-b7f4-23ce0e0c3356")]
internal unsafe struct IGraphicsCaptureItemInterop
{
    public void** Vtable;
    public HRESULT CreateForWindow(nint window, Guid* iid, void** result)
    {
        fixed (IGraphicsCaptureItemInterop* self = &this)
            return ((delegate* unmanaged[Stdcall]<IGraphicsCaptureItemInterop*, nint, Guid*, void**, HRESULT>)Vtable[3])(self, window, iid, result);
    }
    public HRESULT CreateForMonitor(nint monitor, Guid* iid, void** result)
    {
        fixed (IGraphicsCaptureItemInterop* self = &this)
            return ((delegate* unmanaged[Stdcall]<IGraphicsCaptureItemInterop*, nint, Guid*, void**, HRESULT>)Vtable[4])(self, monitor, iid, result);
    }
}
