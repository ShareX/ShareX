// SPDX-License-Identifier: GPL-3.0-or-later
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace ShareX.ScreenRecordingLib.Native;

/// <summary>A small native IMFAsyncCallback CCW. Native references root the callback until the sample is released.</summary>
internal sealed unsafe class SampleReleaseCallback : IDisposable
{
    private struct CallbackData { public void** Vtable; public int References; public nint ActionHandle; }
    private static readonly void** vtable = CreateVtable();
    private nint pointer;
    public IMFAsyncCallback* Pointer => (IMFAsyncCallback*)pointer;

    public SampleReleaseCallback(Action action)
    {
        CallbackData* data = (CallbackData*)NativeMemory.AllocZeroed((nuint)sizeof(CallbackData));
        data->Vtable = vtable;
        data->References = 1;
        data->ActionHandle = GCHandle.ToIntPtr(GCHandle.Alloc(action));
        pointer = (nint)data;
    }

    private static void** CreateVtable()
    {
        void** table = (void**)NativeMemory.Alloc((nuint)(5 * sizeof(nint)));
        table[0] = (delegate* unmanaged[Stdcall]<CallbackData*, Guid*, void**, int>)&QueryInterface;
        table[1] = (delegate* unmanaged[Stdcall]<CallbackData*, uint>)&AddRef;
        table[2] = (delegate* unmanaged[Stdcall]<CallbackData*, uint>)&Release;
        table[3] = (delegate* unmanaged[Stdcall]<CallbackData*, uint*, uint*, int>)&GetParameters;
        table[4] = (delegate* unmanaged[Stdcall]<CallbackData*, nint, int>)&Invoke;
        return table;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static int QueryInterface(CallbackData* self, Guid* iid, void** result)
    {
        *result = null;
        if (*iid != typeof(IMFAsyncCallback).GUID && *iid != new Guid("00000000-0000-0000-c000-000000000046")) return unchecked((int)0x80004002);
        *result = self;
        Interlocked.Increment(ref self->References);
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static uint AddRef(CallbackData* self) => (uint)Interlocked.Increment(ref self->References);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static uint Release(CallbackData* self) => ReleaseReference(self);

    private static uint ReleaseReference(CallbackData* self)
    {
        int remaining = Interlocked.Decrement(ref self->References);
        if (remaining == 0) { GCHandle.FromIntPtr(self->ActionHandle).Free(); NativeMemory.Free(self); }
        return (uint)remaining;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static int GetParameters(CallbackData* self, uint* flags, uint* queue)
    {
        *flags = *queue = 0;
        return unchecked((int)0x80004001); // E_NOTIMPL: use the default MF work queue.
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static int Invoke(CallbackData* self, nint result)
    {
        try { ((Action)GCHandle.FromIntPtr(self->ActionHandle).Target!)(); return 0; }
        catch (Exception ex) { return Marshal.GetHRForException(ex); }
    }

    public void Dispose()
    {
        nint previous = Interlocked.Exchange(ref pointer, 0);
        if (previous != 0) ReleaseReference((CallbackData*)previous);
        GC.SuppressFinalize(this);
    }

    ~SampleReleaseCallback() => Dispose();
}