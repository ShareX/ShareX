// SPDX-License-Identifier: GPL-3.0-or-later
using System.Runtime.InteropServices;

namespace ShareX.ScreenRecordingLib.Native;

[Guid("2cd2d921-c447-44a7-a13c-4adabfc247e3")]
internal unsafe struct IMFAttributes
{
    public void** Vtable;
    public HRESULT GetUINT32(Guid* key, uint* value)
    {
        fixed (IMFAttributes* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFAttributes*, Guid*, uint*, HRESULT>)Vtable[7])(self, key, value);
    }
    public HRESULT GetStringLength(Guid* key, uint* length)
    {
        fixed (IMFAttributes* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFAttributes*, Guid*, uint*, HRESULT>)Vtable[11])(self, key, length);
    }
    public HRESULT GetString(Guid* key, char* value, uint capacity, uint* length)
    {
        fixed (IMFAttributes* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFAttributes*, Guid*, char*, uint, uint*, HRESULT>)Vtable[12])(self, key, value, capacity, length);
    }
    public HRESULT SetUINT32(Guid* key, uint value)
    {
        fixed (IMFAttributes* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFAttributes*, Guid*, uint, HRESULT>)Vtable[21])(self, key, value);
    }
    public HRESULT SetUINT64(Guid* key, ulong value)
    {
        fixed (IMFAttributes* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFAttributes*, Guid*, ulong, HRESULT>)Vtable[22])(self, key, value);
    }
    public HRESULT SetGUID(Guid* key, Guid* value)
    {
        fixed (IMFAttributes* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFAttributes*, Guid*, Guid*, HRESULT>)Vtable[24])(self, key, value);
    }
    public HRESULT SetUnknown(Guid* key, IUnknown* value)
    {
        fixed (IMFAttributes* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFAttributes*, Guid*, IUnknown*, HRESULT>)Vtable[27])(self, key, value);
    }
}

[Guid("44ae0fa8-ea31-4109-8d2e-4cae4997c555")]
internal unsafe struct IMFMediaType
{
    public void** Vtable;
    public HRESULT SetGUID(Guid* key, Guid* value)
    {
        fixed (IMFMediaType* self = &this) return ((IMFAttributes*)self)->SetGUID(key, value);
    }
}

[Guid("c40a00f2-b93a-4d80-ae8c-5a1c634f58e4")]
internal unsafe struct IMFSample
{
    public void** Vtable;
    public HRESULT SetSampleTime(long timestamp)
    {
        fixed (IMFSample* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFSample*, long, HRESULT>)Vtable[36])(self, timestamp);
    }
    public HRESULT SetSampleDuration(long duration)
    {
        fixed (IMFSample* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFSample*, long, HRESULT>)Vtable[38])(self, duration);
    }
    public HRESULT ConvertToContiguousBuffer(IMFMediaBuffer** buffer)
    {
        fixed (IMFSample* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFSample*, IMFMediaBuffer**, HRESULT>)Vtable[41])(self, buffer);
    }
    public HRESULT AddBuffer(IMFMediaBuffer* buffer)
    {
        fixed (IMFSample* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFSample*, IMFMediaBuffer*, HRESULT>)Vtable[42])(self, buffer);
    }
}

[Guid("045fa593-8799-42b8-bc8d-8968c6453507")]
internal unsafe struct IMFMediaBuffer
{
    public void** Vtable;
    public HRESULT Lock(byte** data, uint* maximumLength, uint* currentLength)
    {
        fixed (IMFMediaBuffer* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFMediaBuffer*, byte**, uint*, uint*, HRESULT>)Vtable[3])(self, data, maximumLength, currentLength);
    }
    public HRESULT Unlock()
    {
        fixed (IMFMediaBuffer* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFMediaBuffer*, HRESULT>)Vtable[4])(self);
    }
    public HRESULT SetCurrentLength(uint length)
    {
        fixed (IMFMediaBuffer* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFMediaBuffer*, uint, HRESULT>)Vtable[6])(self, length);
    }
}

[Guid("eb533d5d-2db6-40f8-97a9-494692014f07")]
internal unsafe struct IMFDXGIDeviceManager
{
    public void** Vtable;
    public HRESULT ResetDevice(IUnknown* device, uint token)
    {
        fixed (IMFDXGIDeviceManager* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFDXGIDeviceManager*, IUnknown*, uint, HRESULT>)Vtable[7])(self, device, token);
    }
}

[Guid("a27003cf-2354-4f2a-8d6a-ab7cff15437e")]
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct IMFAsyncCallback { public void** Vtable; }

[Guid("245bf8e9-0755-40f7-88a5-ae0f18d55e17")]
internal unsafe struct IMFTrackedSample
{
    public void** Vtable;
    public HRESULT SetAllocator(IMFAsyncCallback* callback, IUnknown* state)
    {
        fixed (IMFTrackedSample* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFTrackedSample*, IMFAsyncCallback*, IUnknown*, HRESULT>)Vtable[3])(self, callback, state);
    }
}

[Guid("3137f1cd-fe5e-4805-a5d8-fb477448cb3d")]
internal unsafe struct IMFSinkWriter
{
    public void** Vtable;
    public HRESULT AddStream(IMFMediaType* type, uint* index)
    {
        fixed (IMFSinkWriter* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFSinkWriter*, IMFMediaType*, uint*, HRESULT>)Vtable[3])(self, type, index);
    }
    public HRESULT SetInputMediaType(uint index, IMFMediaType* type, IMFAttributes* parameters)
    {
        fixed (IMFSinkWriter* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFSinkWriter*, uint, IMFMediaType*, IMFAttributes*, HRESULT>)Vtable[4])(self, index, type, parameters);
    }
    public HRESULT BeginWriting()
    {
        fixed (IMFSinkWriter* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFSinkWriter*, HRESULT>)Vtable[5])(self);
    }
    public HRESULT WriteSample(uint index, IMFSample* sample)
    {
        fixed (IMFSinkWriter* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFSinkWriter*, uint, IMFSample*, HRESULT>)Vtable[6])(self, index, sample);
    }
    public HRESULT NotifyEndOfSegment(uint index)
    {
        fixed (IMFSinkWriter* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFSinkWriter*, uint, HRESULT>)Vtable[9])(self, index);
    }
    public HRESULT FinalizeWriting()
    {
        fixed (IMFSinkWriter* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFSinkWriter*, HRESULT>)Vtable[11])(self);
    }
}

[Guid("588d72ab-5bc1-496a-8714-b70617141b25")]
internal unsafe struct IMFSinkWriterEx
{
    public void** Vtable;
    public HRESULT GetTransformForStream(uint stream, uint index, Guid* category, IMFTransform** transform)
    {
        fixed (IMFSinkWriterEx* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFSinkWriterEx*, uint, uint, Guid*, IMFTransform**, HRESULT>)Vtable[14])(self, stream, index, category, transform);
    }
}

[Guid("bf94c121-5b05-4e6f-8000-ba598961414d")]
internal unsafe struct IMFTransform
{
    public void** Vtable;
    public HRESULT QueryInterface(Guid* iid, void** result)
    {
        fixed (IMFTransform* self = &this) return ((IUnknown*)self)->QueryInterface(iid, result);
    }
    public HRESULT GetAttributes(IMFAttributes** attributes)
    {
        fixed (IMFTransform* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFTransform*, IMFAttributes**, HRESULT>)Vtable[8])(self, attributes);
    }
    public HRESULT GetInputStreamAttributes(uint index, IMFAttributes** attributes)
    {
        fixed (IMFTransform* self = &this)
            return ((delegate* unmanaged[Stdcall]<IMFTransform*, uint, IMFAttributes**, HRESULT>)Vtable[9])(self, index, attributes);
    }
}

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