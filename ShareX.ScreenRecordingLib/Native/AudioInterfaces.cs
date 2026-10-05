// SPDX-License-Identifier: GPL-3.0-or-later
using System.Runtime.InteropServices;

namespace ShareX.ScreenRecordingLib.Native;

[Guid("a95664d2-9614-4f35-a746-de8db63617e6")]
internal unsafe struct IMMDeviceEnumerator
{
    public void** Vtable;
    public HRESULT EnumAudioEndpoints(EDataFlow flow, uint stateMask, IMMDeviceCollection** devices)
    {
        fixed (IMMDeviceEnumerator* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMMDeviceEnumerator*, EDataFlow, uint, IMMDeviceCollection**, HRESULT>)Vtable[3])(self, flow, stateMask, devices);
    }
    public HRESULT GetDefaultAudioEndpoint(EDataFlow flow, ERole role, IMMDevice** device)
    {
        fixed (IMMDeviceEnumerator* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMMDeviceEnumerator*, EDataFlow, ERole, IMMDevice**, HRESULT>)Vtable[4])(self, flow, role, device);
    }
    public HRESULT GetDevice(char* id, IMMDevice** device)
    {
        fixed (IMMDeviceEnumerator* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMMDeviceEnumerator*, char*, IMMDevice**, HRESULT>)Vtable[5])(self, id, device);
    }
}

[Guid("d666063f-1587-4e43-81f1-b948e807363f")]
internal unsafe struct IMMDevice
{
    public void** Vtable;
    public HRESULT Activate(Guid* iid, CLSCTX context, void* parameters, void** result)
    {
        fixed (IMMDevice* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMMDevice*, Guid*, CLSCTX, void*, void**, HRESULT>)Vtable[3])(self, iid, context, parameters, result);
    }
    public HRESULT OpenPropertyStore(uint access, IPropertyStore** properties)
    {
        fixed (IMMDevice* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMMDevice*, uint, IPropertyStore**, HRESULT>)Vtable[4])(self, access, properties);
    }
    public HRESULT GetId(char** id)
    {
        fixed (IMMDevice* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMMDevice*, char**, HRESULT>)Vtable[5])(self, id);
    }
}

[Guid("0bd7a1be-7a1a-44db-8397-cc5392387b5e")]
internal unsafe struct IMMDeviceCollection
{
    public void** Vtable;
    public HRESULT GetCount(uint* count)
    {
        fixed (IMMDeviceCollection* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMMDeviceCollection*, uint*, HRESULT>)Vtable[3])(self, count);
    }
    public HRESULT Item(uint index, IMMDevice** device)
    {
        fixed (IMMDeviceCollection* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMMDeviceCollection*, uint, IMMDevice**, HRESULT>)Vtable[4])(self, index, device);
    }
}

[Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99")]
internal unsafe struct IPropertyStore
{
    public void** Vtable;
    public HRESULT GetValue(PROPERTYKEY* key, PROPVARIANT* value)
    {
        fixed (IPropertyStore* self = &this)
            return ((delegate* unmanaged[Stdcall]<IPropertyStore*, PROPERTYKEY*, PROPVARIANT*, HRESULT>)Vtable[5])(self, key, value);
    }
}

[Guid("7ed4ee07-8e67-4cd4-8c1a-2b7a5987ad42")]
internal unsafe struct IAudioClient3
{
    public void** Vtable;
    public HRESULT Initialize(AUDCLNT_SHAREMODE mode, uint flags, long bufferDuration, long periodicity, WAVEFORMATEX* format, Guid* session)
    {
        fixed (IAudioClient3* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioClient3*, AUDCLNT_SHAREMODE, uint, long, long, WAVEFORMATEX*, Guid*, HRESULT>)Vtable[3])(self, mode, flags, bufferDuration, periodicity, format, session);
    }
    public HRESULT Start()
    {
        fixed (IAudioClient3* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioClient3*, HRESULT>)Vtable[10])(self);
    }
    public HRESULT Stop()
    {
        fixed (IAudioClient3* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioClient3*, HRESULT>)Vtable[11])(self);
    }
    public HRESULT SetEventHandle(nint ready)
    {
        fixed (IAudioClient3* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioClient3*, nint, HRESULT>)Vtable[13])(self, ready);
    }
    public HRESULT GetService(Guid* iid, void** result)
    {
        fixed (IAudioClient3* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioClient3*, Guid*, void**, HRESULT>)Vtable[14])(self, iid, result);
    }
    public HRESULT GetSharedModeEnginePeriod(WAVEFORMATEX* format, uint* defaultPeriod, uint* fundamentalPeriod, uint* minimumPeriod, uint* maximumPeriod)
    {
        fixed (IAudioClient3* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioClient3*, WAVEFORMATEX*, uint*, uint*, uint*, uint*, HRESULT>)Vtable[18])(self, format, defaultPeriod, fundamentalPeriod, minimumPeriod, maximumPeriod);
    }
    public HRESULT InitializeSharedAudioStream(uint flags, uint period, WAVEFORMATEX* format, Guid* session)
    {
        fixed (IAudioClient3* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioClient3*, uint, uint, WAVEFORMATEX*, Guid*, HRESULT>)Vtable[20])(self, flags, period, format, session);
    }
}

[Guid("c8adbd64-e71e-48a0-a4de-185c395cd317")]
internal unsafe struct IAudioCaptureClient
{
    public void** Vtable;
    public HRESULT GetBuffer(byte** data, uint* frames, uint* flags, ulong* devicePosition, ulong* qpcPosition)
    {
        fixed (IAudioCaptureClient* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioCaptureClient*, byte**, uint*, uint*, ulong*, ulong*, HRESULT>)Vtable[3])(self, data, frames, flags, devicePosition, qpcPosition);
    }
    public HRESULT ReleaseBuffer(uint frames)
    {
        fixed (IAudioCaptureClient* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioCaptureClient*, uint, HRESULT>)Vtable[4])(self, frames);
    }
    public HRESULT GetNextPacketSize(uint* frames)
    {
        fixed (IAudioCaptureClient* self = &this)
            return ((delegate* unmanaged[Stdcall]<IAudioCaptureClient*, uint*, HRESULT>)Vtable[5])(self, frames);
    }
}
