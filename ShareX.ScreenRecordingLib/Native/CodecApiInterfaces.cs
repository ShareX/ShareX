// SPDX-License-Identifier: GPL-3.0-or-later
using System.Runtime.InteropServices;

namespace ShareX.ScreenRecordingLib.Native;

// Vortice.MediaFoundation does not currently expose ICodecAPI.
[Guid("901db4c7-31ce-41a2-85dc-8fa0bf41b8da")]
internal unsafe struct ICodecAPI
{
    public void** Vtable;
    public HRESULT IsSupported(Guid* key)
    {
        fixed (ICodecAPI* self = &this)
            return ((delegate* unmanaged[Stdcall]<ICodecAPI*, Guid*, HRESULT>)Vtable[3])(self, key);
    }
    public HRESULT SetValue(Guid* key, VARIANT* value)
    {
        fixed (ICodecAPI* self = &this)
            return ((delegate* unmanaged[Stdcall]<ICodecAPI*, Guid*, VARIANT*, HRESULT>)Vtable[9])(self, key, value);
    }
}
