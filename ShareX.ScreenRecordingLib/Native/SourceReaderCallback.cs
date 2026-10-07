// SPDX-License-Identifier: GPL-3.0-or-later
using System.ComponentModel;
using System.Runtime.InteropServices;
using SharpGen.Runtime;
using Vortice.MediaFoundation;

namespace ShareX.ScreenRecordingLib.Native;

// Vortice's ReadSample overloads require out arguments. Asynchronous reads require every out
// pointer to be NULL, so this small bridge uses the native method without changing the wrapper.
internal static unsafe class SourceReaderOperations
{
    public static void RequestSample(IMFSourceReader reader)
    {
        void* instance = (void*)reader.NativePointer;
        void** vtable = *(void***)instance;
        new Result(((delegate* unmanaged[Stdcall]<void*, uint, uint, void*, void*, void*, void*, int>)vtable[9])
            (instance, 0xfffffffc, 0, null, null, null, null)).CheckError();
    }
}

internal sealed class SourceReaderCallback : ComObject, IMFSourceReaderCallback
{
    private readonly NativeCallback callback;
    public SourceReaderCallback(Action<int, SourceReaderFlag, nint> received)
        : this(new NativeCallback(received)) { }
    private SourceReaderCallback(NativeCallback callback)
        : base(Marshal.GetComInterfaceForObject(callback, typeof(INativeSourceReaderCallback))) => this.callback = callback;

    public void Detach() => callback.Detach();
    public void OnReadSample(Result status, int stream, int flags, long timestamp, IMFSample sample) =>
        callback.OnReadSample(status.Code, (uint)stream, (uint)flags, timestamp, sample?.NativePointer ?? 0);
    public void OnFlush(int stream) { }
    public void OnEvent(int stream, IMFMediaEvent mediaEvent) { }

    // The CLR COM callable wrapper keeps queued native callbacks alive, including after shutdown.
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    private sealed class NativeCallback(Action<int, SourceReaderFlag, nint> received) : INativeSourceReaderCallback
    {
        private Action<int, SourceReaderFlag, nint>? received = received;
        public void Detach() => Interlocked.Exchange(ref received, null);
        public int OnReadSample(int status, uint stream, uint flags, long timestamp, nint sample)
        {
            try { Volatile.Read(ref received)?.Invoke(status, (SourceReaderFlag)flags, sample); return 0; }
            catch (Exception ex) { return Marshal.GetHRForException(ex); }
        }
        public int OnFlush(uint stream) => 0;
        public int OnEvent(uint stream, nint mediaEvent) => 0;
    }
}

[ComVisible(true), Guid("deec8d99-fa1d-4d82-84c2-2c8969944867")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown), EditorBrowsable(EditorBrowsableState.Never)]
// CLR COM marshaling requires a public interface; this is not a managed recorder API.
public interface INativeSourceReaderCallback
{
    [PreserveSig] int OnReadSample(int status, uint stream, uint flags, long timestamp, nint sample);
    [PreserveSig] int OnFlush(uint stream);
    [PreserveSig] int OnEvent(uint stream, nint mediaEvent);
}
