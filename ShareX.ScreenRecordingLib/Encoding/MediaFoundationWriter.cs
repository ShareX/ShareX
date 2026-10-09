// SPDX-License-Identifier: GPL-3.0-or-later
using ShareX.ScreenRecordingLib.Native;
using ShareX.ScreenRecordingLib.Video;
using SharpGen.Runtime;
using Vortice.Direct3D11;
using Vortice.MediaFoundation;

namespace ShareX.ScreenRecordingLib.Encoding;

internal sealed unsafe class MediaFoundationWriter : IDisposable
{
    private IMFSinkWriter writer = null!;
    private IMFDXGIDeviceManager deviceManager = null!;
    private int videoStream, audioStream;
    private bool started, finalized;
    public VideoEncoderInfo Encoder { get; private set; } = null!;
    public BindFlags EncoderBindFlags { get; private set; }

    public MediaFoundationWriter(GraphicsDevice graphics, RecordingOptions options, int width, int height)
    {
        try
        {
            deviceManager = MediaFactory.MFCreateDXGIDeviceManager();
            deviceManager.ResetDevice(graphics.Device).CheckError();
            using IMFAttributes attributes = MediaFactory.MFCreateAttributes(5);
            attributes.Set(SinkWriterAttributeKeys.ReadwriteEnableHardwareTransforms, 1u).CheckError();
            attributes.Set(SinkWriterAttributeKeys.LowLatency, 1u).CheckError();
            // The fixed-size GPU pool bounds in-flight samples. Do not make WriteSample wait for presentation time.
            attributes.Set(SinkWriterAttributeKeys.DisableThrottling, 1u).CheckError();
            attributes.Set(TranscodeAttributeKeys.TranscodeContainertype, TranscodeContainerTypeGuids.Mpeg4).CheckError();
            attributes.Set(SinkWriterAttributeKeys.D3DManager, deviceManager).CheckError();
            writer = MediaFactory.MFCreateSinkWriterFromURL(options.OutputPath, null!, attributes);
            using IMFMediaType videoOut = CreateVideoType(VideoFormatGuids.H264, width, height, options.FramesPerSecond);
            videoOut.Set(MediaTypeAttributeKeys.AvgBitrate, (uint)options.VideoBitrate).CheckError();
            videoOut.Set(MediaTypeAttributeKeys.Mpeg2Profile, 77u).CheckError(); // H.264 Main
            videoStream = writer.AddStream(videoOut);
            using IMFMediaType videoIn = CreateVideoType(VideoFormatGuids.NV12, width, height, options.FramesPerSecond);
            writer.SetInputMediaType(videoStream, videoIn, null!);
            if (options.HasAudio) AddAudio(options.AudioBitrate);
            Encoder = InspectVideoEncoder();
            if (options.RequireHardwareEncoder && (!Encoder.IsHardwareAccelerated || !Encoder.IsD3D11Aware))
                throw new NotSupportedException($"Windows selected '{Encoder.Name}', which does not provide D3D11 hardware video encoding on this GPU. Disable RequireHardwareEncoder to allow Windows software encoding.");
            writer.BeginWriting();
            started = true;
        }
        catch { Dispose(); throw; }
    }

    private static IMFMediaType CreateType(Guid major, Guid subtype)
    {
        IMFMediaType type = MediaFactory.MFCreateMediaType();
        try
        {
            type.Set(MediaTypeAttributeKeys.MajorType, major).CheckError();
            type.Set(MediaTypeAttributeKeys.Subtype, subtype).CheckError();
            return type;
        }
        catch { type.Dispose(); throw; }
    }

    private static IMFMediaType CreateVideoType(Guid subtype, int width, int height, int fps)
    {
        IMFMediaType type = CreateType(MediaTypeGuids.Video, subtype);
        try
        {
            type.Set(MediaTypeAttributeKeys.InterlaceMode, (uint)VideoInterlaceMode.Progressive).CheckError();
            SetRatio(type, MediaTypeAttributeKeys.FrameSize, (uint)width, (uint)height);
            SetRatio(type, MediaTypeAttributeKeys.FrameRate, (uint)fps, 1);
            SetRatio(type, MediaTypeAttributeKeys.PixelAspectRatio, 1, 1);
            type.Set(MediaTypeAttributeKeys.VideoPrimaries, 2u).CheckError(); // BT.709
            type.Set(MediaTypeAttributeKeys.TransferFunction, 5u).CheckError(); // BT.709
            type.Set(MediaTypeAttributeKeys.YuvMatrix, 1u).CheckError(); // BT.709
            type.Set(MediaTypeAttributeKeys.VideoNominalRange, 2u).CheckError(); // 16–235
            return type;
        }
        catch { type.Dispose(); throw; }
    }

    private void AddAudio(int bitrate)
    {
        using IMFMediaType output = CreateType(MediaTypeGuids.Audio, AudioFormatGuids.Aac);
        output.Set(MediaTypeAttributeKeys.AudioNumChannels, 2u).CheckError();
        output.Set(MediaTypeAttributeKeys.AudioSamplesPerSecond, 48_000u).CheckError();
        output.Set(MediaTypeAttributeKeys.AudioBitsPerSample, 16u).CheckError();
        output.Set(MediaTypeAttributeKeys.AudioAvgBytesPerSecond, (uint)(bitrate / 8)).CheckError();
        output.Set(MediaTypeAttributeKeys.AacPayloadType, 0u).CheckError();
        output.Set(MediaTypeAttributeKeys.AacAudioProfileLevelIndication, 0x29u).CheckError();
        audioStream = writer.AddStream(output);
        using IMFMediaType input = CreateType(MediaTypeGuids.Audio, AudioFormatGuids.Pcm);
        input.Set(MediaTypeAttributeKeys.AudioNumChannels, 2u).CheckError();
        input.Set(MediaTypeAttributeKeys.AudioSamplesPerSecond, 48_000u).CheckError();
        input.Set(MediaTypeAttributeKeys.AudioBitsPerSample, 16u).CheckError();
        input.Set(MediaTypeAttributeKeys.AudioBlockAlignment, 4u).CheckError();
        input.Set(MediaTypeAttributeKeys.AudioAvgBytesPerSecond, 192_000u).CheckError();
        writer.SetInputMediaType(audioStream, input, null!);
    }

    private VideoEncoderInfo InspectVideoEncoder()
    {
        using IMFSinkWriterEx extended = writer.QueryInterface<IMFSinkWriterEx>();
        for (int i = 0; i < 8; i++)
        {
            Guid category;
            IMFTransform transform;
            try { extended.GetTransformForStream(videoStream, i, out category, out transform); }
            catch (SharpGenException) { break; }
            using (transform)
            {
                if (category != TransformCategoryGuids.VideoEncoder) continue;
                ConfigureLowLatency(transform);
                using IMFAttributes attributes = transform.Attributes;
                bool hardware = !string.IsNullOrEmpty(GetString(attributes, TransformAttributeKeys.MftEnumHardwareUrlAttribute));
                bool d3d11 = attributes.GetUInt32(TransformAttributeKeys.D3D11Aware, out uint aware).Success && aware != 0;
                string name = GetString(attributes, TransformAttributeKeys.MftFriendlyNameAttribute) ?? (hardware ? "Windows hardware H.264 encoder" : "Windows H.264 encoder");
                try
                {
                    using IMFAttributes inputAttributes = transform.GetInputStreamAttributes(0);
                    if (inputAttributes.GetUInt32(TransformAttributeKeys.D3D11Bindflags, out uint flags).Success)
                        EncoderBindFlags = (BindFlags)flags;
                }
                catch (SharpGenException) { } // Optional driver-specific texture requirements.
                return new(name, hardware, d3d11);
            }
        }
        throw new NotSupportedException("Media Foundation did not expose the selected H.264 encoder.");
    }

    private static void ConfigureLowLatency(IMFTransform transform)
    {
        // Vortice does not wrap ICodecAPI. Keep only these optional driver hints as native declarations.
        if (transform.QueryInterface(typeof(ICodecAPI).GUID, out nint rawCodec).Failure) return;
        using ComPtr<ICodecAPI> codec = new((ICodecAPI*)rawCodec);
        Guid key = NativeMethods.CODECAPI_AVEncCommonLowLatency;
        VARIANT value = new() { vt = VARENUM.VT_BOOL, boolVal = -1 };
        if (codec.Pointer->IsSupported(&key).Succeeded) codec.Pointer->SetValue(&key, &value);
        key = NativeMethods.CODECAPI_AVEncMPVDefaultBPictureCount;
        value = new() { vt = VARENUM.VT_UI4, ulVal = 0 };
        if (codec.Pointer->IsSupported(&key).Succeeded) codec.Pointer->SetValue(&key, &value);
    }

    private static string? GetString(IMFAttributes attributes, Guid key)
    {
        try { return attributes.GetString(key); }
        catch (SharpGenException) { return null; }
    }

    private static void SetRatio(IMFAttributes attributes, Guid key, uint first, uint second) =>
        attributes.Set(key, ((ulong)first << 32) | second).CheckError();

    public void WriteVideo(IMFSample sample, long timestamp, long duration, bool discontinuity)
    {
        sample.SampleTime = timestamp;
        sample.SampleDuration = duration;
        if (discontinuity) sample.Set(SampleAttributeKeys.Discontinuity, 1u).CheckError();
        writer.WriteSample(videoStream, sample);
    }

    public void WriteAudio(ReadOnlySpan<short> pcm, long frameOffset)
    {
        using IMFSample sample = MediaFactory.MFCreateSample();
        using IMFMediaBuffer buffer = MediaFactory.MFCreateMemoryBuffer(pcm.Length * sizeof(short));
        buffer.Lock(out nint destination, out _, out _);
        try { pcm.CopyTo(new Span<short>((void*)destination, pcm.Length)); }
        finally { buffer.Unlock(); }
        buffer.CurrentLength = pcm.Length * sizeof(short);
        sample.AddBuffer(buffer);
        sample.SampleTime = frameOffset * TimeSpan.TicksPerSecond / 48_000;
        sample.SampleDuration = (frameOffset + pcm.Length / 2) * TimeSpan.TicksPerSecond / 48_000 - sample.SampleTime;
        writer.WriteSample(audioStream, sample);
    }

    public void Finish()
    {
        if (started && !finalized)
        {
            writer.NotifyEndOfSegment(videoStream);
            writer.Finalize();
            finalized = true;
        }
    }

    public void Dispose()
    {
        // Finalization is explicit: failures must reach the caller, never be hidden during disposal.
        writer?.Dispose(); deviceManager?.Dispose();
    }
}
