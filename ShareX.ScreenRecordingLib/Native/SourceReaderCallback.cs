#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using SharpGen.Runtime;
using System.ComponentModel;
using System.Runtime.InteropServices;
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
