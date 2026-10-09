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

// SPDX-License-Identifier: GPL-3.0-or-later
using ShareX.ScreenRecordingLib.Native;
using SharpGen.Runtime;
using System.Runtime.InteropServices;
using Vortice.MediaFoundation;

namespace ShareX.ScreenRecordingLib.Video;

/// <summary>Asynchronous camera capture with one pending frame. The recording worker never waits for it.</summary>
internal sealed class CameraCapture : IDisposable
{
    private readonly object sync = new();
    private IMFMediaSource? source;
    private IMFSourceReader? reader;
    private IMFDXGIDeviceManager? manager;
    private SourceReaderCallback? callback;
    private IMFSample? latest;
    private Exception? failure;
    private bool stopped, started;
    public CameraCaptureInfo Info { get; private set; } = null!;
    public Guid Subtype { get; private set; }
    public int Stride { get; private set; }
    public uint YuvMatrix { get; private set; }
    public uint NominalRange { get; private set; }
    public Exception? Failure { get { lock (sync) return failure; } }

    public CameraCapture(GraphicsDevice graphics, RecordingOptions options)
    {
        try
        {
            using var devices = MediaFactory.MFEnumVideoDeviceSources();
            IMFActivate? selected = devices.FirstOrDefault(device => string.IsNullOrEmpty(options.CameraDeviceId) ||
                string.Equals(device.GetString(CaptureDeviceAttributeKeys.SourceTypeVidcapSymbolicLink), options.CameraDeviceId, StringComparison.OrdinalIgnoreCase));
            if (selected == null)
                throw new NotSupportedException(string.IsNullOrEmpty(options.CameraDeviceId)
                    ? "No camera is available. Connect a camera or turn off the camera overlay."
                    : "The selected camera is unavailable. Select another camera or turn off the camera overlay.");
            string name = selected.GetString(CaptureDeviceAttributeKeys.FriendlyName);
            source = selected.ActivateObject<IMFMediaSource>();
            manager = MediaFactory.MFCreateDXGIDeviceManager();
            manager.ResetDevice(graphics.Device).CheckError();
            callback = new(OnReadSample);
            using IMFAttributes attributes = MediaFactory.MFCreateAttributes(3);
            attributes.Set(SourceReaderAttributeKeys.AsyncCallback, callback).CheckError();
            attributes.Set(SourceReaderAttributeKeys.D3DManager, manager).CheckError();
            attributes.Set(SinkWriterAttributeKeys.ReadwriteEnableHardwareTransforms, 1u).CheckError();
            // Do not enable the Source Reader's software RGB conversion. Color conversion is on the GPU.
            reader = MediaFactory.MFCreateSourceReaderFromMediaSource(source, attributes);
            reader.SetStreamSelection(SourceReaderIndex.AllStreams, false);
            reader.SetStreamSelection(SourceReaderIndex.FirstVideoStream, true);
            ConfigureFormat(name, options);
        }
        catch (Exception ex)
        {
            Dispose();
            if (ex is NotSupportedException) throw;
            throw new IOException("Windows could not initialize the camera. Check camera permissions, whether another application is using it, and the selected capture mode.", ex);
        }
    }

    private void ConfigureFormat(string name, RecordingOptions options)
    {
        (int requestedWidth, int requestedHeight) = options.CameraResolution switch
        {
            CameraCaptureResolution.Size640x480 => (640, 480),
            CameraCaptureResolution.Size1920x1080 => (1920, 1080),
            _ => (1280, 720)
        };
        List<(IMFMediaType Type, double Score)> modes = new();
        Exception? lastError = null;
        try
        {
            for (int index = 0; index < 512; index++)
            {
                IMFMediaType type;
                try { type = reader!.GetNativeMediaType(SourceReaderIndex.FirstVideoStream, index); }
                catch (SharpGenException ex) when (ex.ResultCode.Code == unchecked((int)0xc00d36b9)) { break; } // MF_E_NO_MORE_TYPES
                bool added = false;
                try
                {
                    Guid subtype = type.GetGUID(MediaTypeAttributeKeys.Subtype);
                    if (!IsRaw(subtype) && subtype != VideoFormatGuids.Mjpg && subtype != VideoFormatGuids.H264) continue;
                    ulong size = type.GetUInt64(MediaTypeAttributeKeys.FrameSize);
                    int width = (int)(size >> 32), height = (int)size;
                    if (width < 2 || height < 2 || width > 4096 || height > 4096 || (width & 1) != 0 || (height & 1) != 0) continue;
                    ulong rate = type.GetUInt64(MediaTypeAttributeKeys.FrameRate);
                    double fps = (double)(rate >> 32) / Math.Max(1u, (uint)rate);
                    double score = Math.Abs(Math.Log((double)width * height / ((double)requestedWidth * requestedHeight))) * 2
                        + Math.Abs((double)width / height - (double)requestedWidth / requestedHeight)
                        + Math.Abs(fps - options.CameraFramesPerSecond) / options.CameraFramesPerSecond
                        + (fps > options.CameraFramesPerSecond ? 1 : 0) + (IsRaw(subtype) ? 0 : 0.25);
                    modes.Add((type, score)); added = true;
                }
                finally { if (!added) type.Dispose(); }
            }
            foreach (var mode in modes.OrderBy(mode => mode.Score))
            {
                try
                {
                    reader!.SetCurrentMediaType(SourceReaderIndex.FirstVideoStream, mode.Type);
                    Guid nativeSubtype = mode.Type.GetGUID(MediaTypeAttributeKeys.Subtype);
                    if (IsRaw(nativeSubtype))
                    {
                        ReadSelectedFormat(name);
                        return;
                    }
                    // Decoders expose different raw formats. Preserve the chosen native size/rate
                    // instead of allowing a partial output type to switch the source to its default mode.
                    foreach (Guid decodedSubtype in new[] { VideoFormatGuids.NV12, VideoFormatGuids.YUY2, VideoFormatGuids.Rgb32 })
                    {
                        try
                        {
                            using IMFMediaType decoded = MediaFactory.MFCreateMediaType();
                            decoded.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video).CheckError();
                            decoded.Set(MediaTypeAttributeKeys.Subtype, decodedSubtype).CheckError();
                            decoded.Set(MediaTypeAttributeKeys.FrameSize, mode.Type.GetUInt64(MediaTypeAttributeKeys.FrameSize)).CheckError();
                            decoded.Set(MediaTypeAttributeKeys.FrameRate, mode.Type.GetUInt64(MediaTypeAttributeKeys.FrameRate)).CheckError();
                            reader.SetCurrentMediaType(SourceReaderIndex.FirstVideoStream, decoded);
                            ReadSelectedFormat(name);
                            return;
                        }
                        catch (SharpGenException ex) { lastError = ex; }
                    }
                }
                catch (SharpGenException ex) { lastError = ex; }
            }
            throw new NotSupportedException("The camera did not provide a supported NV12, YUY2, RGB32, MJPEG, or H.264 capture mode.", lastError);
        }
        finally { foreach (var mode in modes) mode.Type.Dispose(); }
    }

    private void ReadSelectedFormat(string name)
    {
        using IMFMediaType current = reader!.GetCurrentMediaType(SourceReaderIndex.FirstVideoStream);
        Subtype = current.GetGUID(MediaTypeAttributeKeys.Subtype);
        ulong size = current.GetUInt64(MediaTypeAttributeKeys.FrameSize);
        ulong rate = current.GetUInt64(MediaTypeAttributeKeys.FrameRate);
        int width = (int)(size >> 32), height = (int)size;
        string format = Subtype == VideoFormatGuids.NV12 ? "NV12" : Subtype == VideoFormatGuids.YUY2 ? "YUY2" : "RGB32";
        Info = new(name, width, height, (double)(rate >> 32) / Math.Max(1u, (uint)rate), format);
        Stride = current.GetUInt32(MediaTypeAttributeKeys.DefaultStride, out uint stride).Success && stride != 0
            ? unchecked((int)stride) : MediaFactory.MFGetStrideForBitmapInfoHeader(BitConverter.ToInt32(Subtype.ToByteArray()), width);
        YuvMatrix = current.GetUInt32(MediaTypeAttributeKeys.YuvMatrix, out uint matrix).Success && matrix != 0 ? matrix : height > 576 ? 1u : 2u;
        NominalRange = current.GetUInt32(MediaTypeAttributeKeys.VideoNominalRange, out uint range).Success ? range : 2u;
    }

    private static bool IsRaw(Guid subtype) => subtype == VideoFormatGuids.NV12 || subtype == VideoFormatGuids.YUY2 || subtype == VideoFormatGuids.Rgb32;

    public void Start()
    {
        lock (sync)
        {
            ObjectDisposedException.ThrowIf(stopped, this);
            if (started) return;
            SourceReaderOperations.RequestSample(reader!);
            started = true;
        }
    }

    private void OnReadSample(int status, SourceReaderFlag flags, nint sample)
    {
        lock (sync)
        {
            if (stopped) return;
            try
            {
                new Result(status).CheckError();
                if ((flags & (SourceReaderFlag.Error | SourceReaderFlag.EndOfStream | SourceReaderFlag.CurrentMediaTypeChanged)) != 0)
                    throw new IOException("The camera stopped delivering frames or changed its capture format.");
                if (sample != 0)
                {
                    // OnReadSample lends us a reference. Keep our own until the recording worker consumes it.
                    Marshal.AddRef(sample);
                    IMFSample incoming = new(sample);
                    latest?.Dispose(); latest = incoming;
                }
                SourceReaderOperations.RequestSample(reader!);
            }
            catch (Exception ex) { failure = ex; }
        }
    }

    public IMFSample? TakeLatestFrame()
    {
        lock (sync) { IMFSample? frame = latest; latest = null; return frame; }
    }

    public void Dispose()
    {
        lock (sync)
        {
            if (stopped) return;
            stopped = true;
            callback?.Detach();
            latest?.Dispose(); latest = null;
        }
        // Never hold the callback lock across shutdown: native code may wait for queued callbacks.
        try { reader?.Flush(SourceReaderIndex.FirstVideoStream); } catch (SharpGenException) { }
        try { source?.Shutdown(); } catch (SharpGenException) { }
        reader?.Dispose(); source?.Dispose(); manager?.Dispose(); callback?.Dispose();
    }
}
