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

namespace ShareX.Tools;

/// <summary>Exposes a native file source's video stream without constructing an audio decoder.</summary>
internal sealed class WindowsVideoOnlySource : IDisposable
{
    private readonly NativeVideoSource _source;
    private readonly NativeExtension _extension;
    private int _disposed;
    public ComObject Extension { get; }

    public WindowsVideoOnlySource(string path)
    {
        MediaFactory.MFStartup(true).CheckError();
        try
        {
            using IMFSourceResolver resolver = MediaFactory.MFCreateSourceResolver();
            IMFMediaSource source = resolver.CreateObjectFromURL(path, SourceResolverFlags.MediaSource);
            try { _source = new NativeVideoSource(source); }
            catch
            {
                try { source.Shutdown(); }
                finally { source.Dispose(); }
                throw;
            }
            _extension = new NativeExtension(_source);
            Extension = new ComObject(Marshal.GetComInterfaceForObject(_extension, typeof(INativeVideoSourceExtension)));
        }
        catch
        {
            _extension?.Dispose();
            _source?.Shutdown();
            MediaFactory.MFShutdown();
            throw;
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        Extension.Dispose();
        _extension.Dispose();
        _source.Shutdown();
        MediaFactory.MFShutdown();
    }

    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    private sealed unsafe class NativeExtension : INativeVideoSourceExtension, IDisposable
    {
        private readonly object _gate = new();
        private ComObject? _source;
        public NativeExtension(NativeVideoSource source) =>
            _source = new ComObject(Marshal.GetComInterfaceForObject(source, typeof(INativeVideoMediaSource)));

        public int CanPlayType(int audioOnly, string mimeType, out int answer) { answer = 0; return 0; }

        public int BeginCreateObject(string url, nint byteStream, int type, out nint cancelCookie, nint callback, nint state)
        {
            cancelCookie = 0;
            if (type != (int)ObjectType.MediaSource) return unchecked((int)0x80004001); // E_NOTIMPL
            nint source;
            lock (_gate)
            {
                if (_source == null) return ResultCode.Shutdown.Code;
                source = _source.NativePointer;
                Marshal.AddRef(source);
            }
            try
            {
                int status = MFCreateAsyncResult(source, callback, state, out nint result);
                if (status < 0) return status;
                try { return MFInvokeCallback(result); }
                finally { Marshal.Release(result); }
            }
            finally { Marshal.Release(source); }
        }

        public int CancelObjectCreation(nint cookie) => 0;

        public int EndCreateObject(nint result, out nint source)
        {
            source = 0;
            // The async result owns its source reference, even if shutdown happened after Begin.
            nint* vtable = *(nint**)result;
            int status = ((delegate* unmanaged[Stdcall]<nint, int>)vtable[4])(result); // IMFAsyncResult::GetStatus
            if (status < 0) return status;
            fixed (nint* output = &source)
                return ((delegate* unmanaged[Stdcall]<nint, nint*, int>)vtable[6])(result, output); // GetObject
        }

        public void Dispose()
        {
            lock (_gate) { _source?.Dispose(); _source = null; }
        }

        [DllImport("mfplat.dll", ExactSpelling = true)]
        private static extern int MFCreateAsyncResult(nint source, nint callback, nint state, out nint result);
        [DllImport("mfplat.dll", ExactSpelling = true)]
        private static extern int MFInvokeCallback(nint result);
    }

    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    private sealed unsafe class NativeVideoSource : INativeVideoMediaSource, INativeVideoEventGenerator, INativeVideoSourceService
    {
        private readonly object _gate = new();
        private IMFMediaSource? _source;
        private IMFPresentationDescriptor? _videoDescriptor;
        private readonly int _streamId;

        public NativeVideoSource(IMFMediaSource source)
        {
            using IMFPresentationDescriptor original = source.CreatePresentationDescriptor();
            IMFStreamDescriptor? video = null;
            bool hasAudio = false, selectedVideo = false;
            try
            {
                for (int i = 0; i < original.StreamDescriptorCount; i++)
                {
                    original.GetStreamDescriptorByIndex(i, out RawBool selected, out IMFStreamDescriptor nativeStream);
                    using IMFStreamDescriptor stream = nativeStream;
                    using IMFMediaTypeHandler handler = stream.MediaTypeHandler;
                    hasAudio |= handler.MajorType == MediaTypeGuids.Audio;
                    if (handler.MajorType == MediaTypeGuids.Video && (video == null || (selected && !selectedVideo)))
                    {
                        video?.Dispose();
                        stream.AddRef();
                        video = new IMFStreamDescriptor(stream.NativePointer);
                        selectedVideo = selected;
                    }
                }
                if (video == null || !hasAudio) throw new InvalidOperationException(Localization.Strings.VideoTrimmer_InvalidVideo);
                _streamId = video.StreamIdentifier;
                MediaFactory.MFCreatePresentationDescriptor(1, [video], out IMFPresentationDescriptor descriptor).CheckError();
                try
                {
                    original.CopyAllItems(descriptor).CheckError();
                    descriptor.SelectStream(0);
                    _videoDescriptor = descriptor;
                }
                catch { descriptor.Dispose(); throw; }
                _source = source;
            }
            finally { video?.Dispose(); }
        }

        // Each native call takes its own reference so shutdown can safely race a queued callback.
        private IMFMediaSource? Acquire()
        {
            lock (_gate)
            {
                if (_source == null) return null;
                _source.AddRef();
                return new IMFMediaSource(_source.NativePointer);
            }
        }

        public int GetEvent(uint flags, out nint mediaEvent)
        {
            mediaEvent = 0;
            using IMFMediaSource? source = Acquire();
            if (source == null) return ResultCode.Shutdown.Code;
            try
            {
                using IMFMediaEvent value = source.GetEvent((int)flags);
                value.AddRef(); mediaEvent = value.NativePointer;
                return 0;
            }
            catch (Exception ex) { return ex.HResult; }
        }

        public int BeginGetEvent(nint callback, nint state)
        {
            using IMFMediaSource? source = Acquire();
            if (source == null) return ResultCode.Shutdown.Code;
            nint instance = source.NativePointer;
            return ((delegate* unmanaged[Stdcall]<nint, nint, nint, int>)(*(nint**)instance)[4])(instance, callback, state);
        }

        public int EndGetEvent(nint result, out nint mediaEvent)
        {
            mediaEvent = 0;
            using IMFMediaSource? source = Acquire();
            if (source == null) return ResultCode.Shutdown.Code;
            nint instance = source.NativePointer;
            fixed (nint* output = &mediaEvent)
                return ((delegate* unmanaged[Stdcall]<nint, nint, nint*, int>)(*(nint**)instance)[5])(instance, result, output);
        }

        public int QueueEvent(int type, ref Guid extendedType, int status, nint value)
        {
            using IMFMediaSource? source = Acquire();
            if (source == null) return ResultCode.Shutdown.Code;
            nint instance = source.NativePointer;
            fixed (Guid* extended = &extendedType)
                return ((delegate* unmanaged[Stdcall]<nint, int, Guid*, int, nint, int>)(*(nint**)instance)[6])(instance, type, extended, status, value);
        }

        public int GetCharacteristics(out uint characteristics)
        {
            characteristics = 0;
            using IMFMediaSource? source = Acquire();
            if (source == null) return ResultCode.Shutdown.Code;
            try { characteristics = (uint)source.Characteristics; return 0; }
            catch (Exception ex) { return ex.HResult; }
        }

        public int CreatePresentationDescriptor(out nint descriptor)
        {
            descriptor = 0;
            lock (_gate)
            {
                if (_source == null) return ResultCode.Shutdown.Code;
                try
                {
                    using IMFPresentationDescriptor value = _videoDescriptor!.Clone();
                    value.AddRef(); descriptor = value.NativePointer;
                    return 0;
                }
                catch (Exception ex) { return ex.HResult; }
            }
        }

        public int Start(nint descriptor, nint timeFormat, nint startPosition)
        {
            using IMFMediaSource? source = Acquire();
            if (source == null) return ResultCode.Shutdown.Code;
            try
            {
                Marshal.AddRef(descriptor);
                using IMFPresentationDescriptor requested = new(descriptor);
                requested.GetStreamDescriptorByIndex(0, out RawBool selected, out IMFStreamDescriptor video);
                using (video)
                    if (video.StreamIdentifier != _streamId) return unchecked((int)0x80070057);
                // The native source still expects its original stream list. Disable every other stream.
                using IMFPresentationDescriptor original = source.CreatePresentationDescriptor();
                for (int i = 0; i < original.StreamDescriptorCount; i++)
                {
                    original.GetStreamDescriptorByIndex(i, out _, out IMFStreamDescriptor stream);
                    using (stream)
                    {
                        if (selected && stream.StreamIdentifier == _streamId) original.SelectStream(i);
                        else original.DeselectStream(i);
                    }
                }
                nint instance = source.NativePointer;
                return ((delegate* unmanaged[Stdcall]<nint, nint, nint, nint, int>)(*(nint**)instance)[9])
                    (instance, original.NativePointer, timeFormat, startPosition);
            }
            catch (Exception ex) { return ex.HResult; }
        }

        public int Stop() => ChangeState(false);
        public int Pause() => ChangeState(true);
        private int ChangeState(bool pause)
        {
            using IMFMediaSource? source = Acquire();
            if (source == null) return ResultCode.Shutdown.Code;
            try { if (pause) source.Pause(); else source.Stop(); return 0; }
            catch (Exception ex) { return ex.HResult; }
        }

        public int Shutdown()
        {
            IMFMediaSource? source;
            lock (_gate)
            {
                source = _source; _source = null;
                _videoDescriptor?.Dispose(); _videoDescriptor = null;
            }
            if (source == null) return 0;
            try { source.Shutdown(); return 0; }
            catch (Exception ex) { return ex.HResult; }
            finally { source.Dispose(); }
        }

        public int GetService(ref Guid service, ref Guid iid, out nint result)
        {
            result = 0;
            using IMFMediaSource? source = Acquire();
            if (source == null) return ResultCode.Shutdown.Code;
            using IMFGetService? provider = source.QueryInterfaceOrNull<IMFGetService>();
            if (provider == null) return unchecked((int)0xC00D36BA); // MF_E_UNSUPPORTED_SERVICE
            nint instance = provider.NativePointer;
            fixed (Guid* serviceId = &service, interfaceId = &iid)
            fixed (nint* output = &result)
                return ((delegate* unmanaged[Stdcall]<nint, Guid*, Guid*, nint*, int>)(*(nint**)instance)[3])(instance, serviceId, interfaceId, output);
        }
    }
}

// CLR COM marshaling requires public interfaces. These are native bridges, not managed player APIs.
[ComVisible(true), Guid("2f69d622-20b5-41e9-afdf-89ced1dda04e"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), EditorBrowsable(EditorBrowsableState.Never)]
public interface INativeVideoSourceExtension
{
    [PreserveSig] int CanPlayType(int audioOnly, [MarshalAs(UnmanagedType.LPWStr)] string mimeType, out int answer);
    [PreserveSig] int BeginCreateObject([MarshalAs(UnmanagedType.BStr)] string url, nint byteStream, int type, out nint cancelCookie, nint callback, nint state);
    [PreserveSig] int CancelObjectCreation(nint cookie);
    [PreserveSig] int EndCreateObject(nint result, out nint source);
}

[ComVisible(true), Guid("2cd0bd52-bcd5-4b89-b62c-eadc0c031e7d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), EditorBrowsable(EditorBrowsableState.Never)]
public interface INativeVideoEventGenerator
{
    [PreserveSig] int GetEvent(uint flags, out nint mediaEvent);
    [PreserveSig] int BeginGetEvent(nint callback, nint state);
    [PreserveSig] int EndGetEvent(nint result, out nint mediaEvent);
    [PreserveSig] int QueueEvent(int type, ref Guid extendedType, int status, nint value);
}

[ComVisible(true), Guid("279a808d-aec7-40c8-9c6b-a6b492c78a66"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), EditorBrowsable(EditorBrowsableState.Never)]
public interface INativeVideoMediaSource
{
    [PreserveSig] int GetEvent(uint flags, out nint mediaEvent);
    [PreserveSig] int BeginGetEvent(nint callback, nint state);
    [PreserveSig] int EndGetEvent(nint result, out nint mediaEvent);
    [PreserveSig] int QueueEvent(int type, ref Guid extendedType, int status, nint value);
    [PreserveSig] int GetCharacteristics(out uint characteristics);
    [PreserveSig] int CreatePresentationDescriptor(out nint descriptor);
    [PreserveSig] int Start(nint descriptor, nint timeFormat, nint startPosition);
    [PreserveSig] int Stop();
    [PreserveSig] int Pause();
    [PreserveSig] int Shutdown();
}

[ComVisible(true), Guid("fa993888-4383-415a-a930-dd472a8cf6f7"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), EditorBrowsable(EditorBrowsableState.Never)]
public interface INativeVideoSourceService
{
    [PreserveSig] int GetService(ref Guid service, ref Guid iid, out nint result);
}