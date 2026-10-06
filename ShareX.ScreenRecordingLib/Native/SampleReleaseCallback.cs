// SPDX-License-Identifier: GPL-3.0-or-later
using System.ComponentModel;
using System.Runtime.InteropServices;
using SharpGen.Runtime;
using Vortice.MediaFoundation;

namespace ShareX.ScreenRecordingLib.Native;

/// <summary>Media Foundation notifies the pool after releasing the tracked GPU sample.</summary>
internal sealed class SampleReleaseCallback : ComObject, IMFAsyncCallback
{
    private readonly NativeCallback callback;

    public SampleReleaseCallback(Action released) : this(new NativeCallback(released)) { }

    private SampleReleaseCallback(NativeCallback callback)
        : base(Marshal.GetComInterfaceForObject(callback, typeof(INativeAsyncCallback)))
    {
        this.callback = callback;
    }

    public Result GetParameters(out AsyncCallbackFlags flags, out int queue)
    {
        flags = AsyncCallbackFlags.None;
        queue = 0;
        return new(unchecked((int)0x80004001)); // E_NOTIMPL: use the default MF work queue.
    }

    public Result Invoke(IMFAsyncResult result) => new(callback.Invoke(result.NativePointer));

    // A CLR COM callable wrapper roots the callback until its last native reference is released.
    // SharpGen's CallbackBase uses a weak handle, which is insufficient for queued callbacks
    // surviving pool disposal on a failed recording.
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    private sealed class NativeCallback(Action released) : INativeAsyncCallback
    {
        public int GetParameters(out uint flags, out uint queue)
        {
            flags = queue = 0;
            return unchecked((int)0x80004001);
        }

        public int Invoke(nint result)
        {
            try { released(); return 0; }
            catch (Exception ex) { return Marshal.GetHRForException(ex); }
        }
    }
}

[ComVisible(true)]
[Guid("a27003cf-2354-4f2a-8d6a-ab7cff15437e")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[EditorBrowsable(EditorBrowsableState.Never)]
// CLR COM marshaling requires a public interface; this is not part of the recorder's managed API.
public interface INativeAsyncCallback
{
    [PreserveSig] int GetParameters(out uint flags, out uint queue);
    [PreserveSig] int Invoke(nint result);
}
