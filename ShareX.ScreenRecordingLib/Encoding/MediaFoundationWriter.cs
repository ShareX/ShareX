// SPDX-License-Identifier: GPL-3.0-or-later
using ShareX.ScreenRecordingLib.Native;
using ShareX.ScreenRecordingLib.Video;

namespace ShareX.ScreenRecordingLib.Encoding;

internal sealed unsafe class MediaFoundationWriter : IDisposable
{
    private ComPtr<IMFSinkWriter> writer = null!;
    private ComPtr<IMFDXGIDeviceManager> deviceManager = null!;
    private uint videoStream, audioStream;
    private bool started, finalized;
    public VideoEncoderInfo Encoder { get; private set; } = null!;
    public D3D11_BIND_FLAG EncoderBindFlags { get; private set; }

    public MediaFoundationWriter(GraphicsDevice graphics, RecordingOptions options, int width, int height)
    {
        try
        {
            uint token;
            IMFDXGIDeviceManager* rawManager;
            NativeMethods.MFCreateDXGIDeviceManager(&token, &rawManager).ThrowOnFailure();
            deviceManager = new(rawManager);
            deviceManager.Pointer->ResetDevice((IUnknown*)graphics.Device.Pointer, token).ThrowOnFailure();
            IMFAttributes* rawAttributes;
            NativeMethods.MFCreateAttributes(&rawAttributes, 5).ThrowOnFailure();
            using ComPtr<IMFAttributes> attributes = new(rawAttributes);
            Set(attributes.Pointer, NativeMethods.MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, 1);
            Set(attributes.Pointer, NativeMethods.MF_LOW_LATENCY, 1);
            // The fixed-size GPU pool bounds in-flight samples. Do not make WriteSample wait for presentation time.
            Set(attributes.Pointer, NativeMethods.MF_SINK_WRITER_DISABLE_THROTTLING, 1);
            Set(attributes.Pointer, NativeMethods.MF_TRANSCODE_CONTAINERTYPE, NativeMethods.MFTranscodeContainerType_MPEG4);
            Guid managerKey = NativeMethods.MF_SINK_WRITER_D3D_MANAGER;
            attributes.Pointer->SetUnknown(&managerKey, (IUnknown*)deviceManager.Pointer).ThrowOnFailure();
            IMFSinkWriter* rawWriter;
            NativeMethods.MFCreateSinkWriterFromURL(options.OutputPath, null, attributes.Pointer, &rawWriter).ThrowOnFailure();
            writer = new(rawWriter);
            using ComPtr<IMFMediaType> videoOut = CreateVideoType(NativeMethods.MFVideoFormat_H264, width, height, options.FramesPerSecond);
            Set((IMFAttributes*)videoOut.Pointer, NativeMethods.MF_MT_AVG_BITRATE, (uint)options.VideoBitrate);
            Set((IMFAttributes*)videoOut.Pointer, NativeMethods.MF_MT_MPEG2_PROFILE, 77); // H.264 Main
            uint index;
            writer.Pointer->AddStream(videoOut.Pointer, &index).ThrowOnFailure();
            videoStream = index;
            using ComPtr<IMFMediaType> videoIn = CreateVideoType(NativeMethods.MFVideoFormat_NV12, width, height, options.FramesPerSecond);
            writer.Pointer->SetInputMediaType(videoStream, videoIn.Pointer, null).ThrowOnFailure();
            if (options.HasAudio) AddAudio(options.AudioBitrate);
            Encoder = InspectVideoEncoder();
            if (options.RequireHardwareEncoder && (!Encoder.IsHardwareAccelerated || !Encoder.IsD3D11Aware))
                throw new NotSupportedException($"Windows selected '{Encoder.Name}', which does not provide D3D11 hardware video encoding on this GPU. Disable RequireHardwareEncoder to allow Windows software encoding.");
            writer.Pointer->BeginWriting().ThrowOnFailure();
            started = true;
        }
        catch { Dispose(); throw; }
    }

    private static ComPtr<IMFMediaType> CreateType(Guid major, Guid subtype)
    {
        IMFMediaType* type;
        NativeMethods.MFCreateMediaType(&type).ThrowOnFailure();
        ComPtr<IMFMediaType> result = new(type);
        try
        {
            Set((IMFAttributes*)type, NativeMethods.MF_MT_MAJOR_TYPE, major);
            Set((IMFAttributes*)type, NativeMethods.MF_MT_SUBTYPE, subtype);
            return result;
        }
        catch { result.Dispose(); throw; }
    }

    private static ComPtr<IMFMediaType> CreateVideoType(Guid subtype, int width, int height, int fps)
    {
        ComPtr<IMFMediaType> type = CreateType(NativeMethods.MFMediaType_Video, subtype);
        try
        {
            IMFAttributes* attrs = (IMFAttributes*)type.Pointer;
            Set(attrs, NativeMethods.MF_MT_INTERLACE_MODE, 2); // progressive
            SetRatio(attrs, NativeMethods.MF_MT_FRAME_SIZE, (uint)width, (uint)height);
            SetRatio(attrs, NativeMethods.MF_MT_FRAME_RATE, (uint)fps, 1);
            SetRatio(attrs, NativeMethods.MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
            Set(attrs, NativeMethods.MF_MT_VIDEO_PRIMARIES, 2); // BT.709
            Set(attrs, NativeMethods.MF_MT_TRANSFER_FUNCTION, 5); // BT.709
            Set(attrs, NativeMethods.MF_MT_YUV_MATRIX, 1); // BT.709
            Set(attrs, NativeMethods.MF_MT_VIDEO_NOMINAL_RANGE, 2); // 16–235
            return type;
        }
        catch { type.Dispose(); throw; }
    }

    private void AddAudio(int bitrate)
    {
        using ComPtr<IMFMediaType> output = CreateType(NativeMethods.MFMediaType_Audio, NativeMethods.MFAudioFormat_AAC);
        IMFAttributes* attrs = (IMFAttributes*)output.Pointer;
        Set(attrs, NativeMethods.MF_MT_AUDIO_NUM_CHANNELS, 2);
        Set(attrs, NativeMethods.MF_MT_AUDIO_SAMPLES_PER_SECOND, 48_000);
        Set(attrs, NativeMethods.MF_MT_AUDIO_BITS_PER_SAMPLE, 16);
        Set(attrs, NativeMethods.MF_MT_AUDIO_AVG_BYTES_PER_SECOND, (uint)(bitrate / 8));
        Set(attrs, NativeMethods.MF_MT_AAC_PAYLOAD_TYPE, 0);
        Set(attrs, NativeMethods.MF_MT_AAC_AUDIO_PROFILE_LEVEL_INDICATION, 0x29);
        uint index;
        writer.Pointer->AddStream(output.Pointer, &index).ThrowOnFailure();
        audioStream = index;
        using ComPtr<IMFMediaType> input = CreateType(NativeMethods.MFMediaType_Audio, NativeMethods.MFAudioFormat_PCM);
        attrs = (IMFAttributes*)input.Pointer;
        Set(attrs, NativeMethods.MF_MT_AUDIO_NUM_CHANNELS, 2);
        Set(attrs, NativeMethods.MF_MT_AUDIO_SAMPLES_PER_SECOND, 48_000);
        Set(attrs, NativeMethods.MF_MT_AUDIO_BITS_PER_SAMPLE, 16);
        Set(attrs, NativeMethods.MF_MT_AUDIO_BLOCK_ALIGNMENT, 4);
        Set(attrs, NativeMethods.MF_MT_AUDIO_AVG_BYTES_PER_SECOND, 192_000);
        writer.Pointer->SetInputMediaType(audioStream, input.Pointer, null).ThrowOnFailure();
    }

    private VideoEncoderInfo InspectVideoEncoder()
    {
        using ComPtr<IMFSinkWriterEx> extended = writer.Query<IMFSinkWriterEx>();
        for (uint i = 0; i < 8; i++)
        {
            Guid category;
            IMFTransform* rawTransform;
            if (extended.Pointer->GetTransformForStream(videoStream, i, &category, &rawTransform).Failed) break;
            using ComPtr<IMFTransform> transform = new(rawTransform);
            if (category != NativeMethods.MFT_CATEGORY_VIDEO_ENCODER) continue;
            ConfigureLowLatency(transform);
            IMFAttributes* rawAttributes;
            transform.Pointer->GetAttributes(&rawAttributes).ThrowOnFailure();
            using ComPtr<IMFAttributes> attributes = new(rawAttributes);
            Guid hardwareKey = NativeMethods.MFT_ENUM_HARDWARE_URL_Attribute;
            uint length;
            bool hardware = attributes.Pointer->GetStringLength(&hardwareKey, &length).Succeeded && length > 0;
            Guid awareKey = NativeMethods.MF_SA_D3D11_AWARE;
            uint aware;
            bool d3d11 = attributes.Pointer->GetUINT32(&awareKey, &aware).Succeeded && aware != 0;
            string name = GetString(attributes.Pointer, NativeMethods.MFT_FRIENDLY_NAME_Attribute) ?? (hardware ? "Windows hardware H.264 encoder" : "Windows H.264 encoder");
            IMFAttributes* rawInputAttributes;
            if (transform.Pointer->GetInputStreamAttributes(0, &rawInputAttributes).Succeeded)
            {
                using ComPtr<IMFAttributes> inputAttributes = new(rawInputAttributes);
                Guid bindKey = NativeMethods.MF_SA_D3D11_BINDFLAGS;
                uint bindFlags;
                if (inputAttributes.Pointer->GetUINT32(&bindKey, &bindFlags).Succeeded) EncoderBindFlags = (D3D11_BIND_FLAG)bindFlags;
            }
            return new(name, hardware, d3d11);
        }
        throw new NotSupportedException("Media Foundation did not expose the selected H.264 encoder.");
    }

    private static void ConfigureLowLatency(ComPtr<IMFTransform> transform)
    {
        Guid iid = typeof(ICodecAPI).GUID;
        void* rawCodec;
        if (transform.Pointer->QueryInterface(&iid, &rawCodec).Failed) return;
        using ComPtr<ICodecAPI> codec = new((ICodecAPI*)rawCodec);
        // Codec options vary by GPU driver. Unsupported latency hints do not make recording unavailable.
        Guid key = NativeMethods.CODECAPI_AVEncCommonLowLatency;
        VARIANT value = default;
        value.vt = VARENUM.VT_BOOL;
        value.boolVal = -1;
        if (codec.Pointer->IsSupported(&key).Succeeded) codec.Pointer->SetValue(&key, &value);
        key = NativeMethods.CODECAPI_AVEncMPVDefaultBPictureCount;
        value = default;
        value.vt = VARENUM.VT_UI4;
        value.ulVal = 0;
        if (codec.Pointer->IsSupported(&key).Succeeded) codec.Pointer->SetValue(&key, &value);
    }

    private static string? GetString(IMFAttributes* attrs, Guid key)
    {
        uint length;
        if (attrs->GetStringLength(&key, &length).Failed || length == 0) return null;
        char[] value = new char[length + 1];
        fixed (char* chars = value)
        {
            attrs->GetString(&key, chars, (uint)value.Length, null).ThrowOnFailure();
            return new string(chars, 0, (int)length);
        }
    }

    internal static void Set(IMFAttributes* attrs, Guid key, uint value) => attrs->SetUINT32(&key, value).ThrowOnFailure();
    internal static void Set(IMFAttributes* attrs, Guid key, Guid value) => attrs->SetGUID(&key, &value).ThrowOnFailure();
    private static void SetRatio(IMFAttributes* attrs, Guid key, uint first, uint second) => attrs->SetUINT64(&key, ((ulong)first << 32) | second).ThrowOnFailure();

    public void WriteVideo(IMFSample* sample, long timestamp, long duration, bool discontinuity)
    {
        sample->SetSampleTime(timestamp).ThrowOnFailure();
        sample->SetSampleDuration(duration).ThrowOnFailure();
        if (discontinuity) Set((IMFAttributes*)sample, NativeMethods.MFSampleExtension_Discontinuity, 1);
        writer.Pointer->WriteSample(videoStream, sample).ThrowOnFailure();
    }

    public void WriteAudio(ReadOnlySpan<short> pcm, long frameOffset)
    {
        IMFSample* rawSample;
        NativeMethods.MFCreateSample(&rawSample).ThrowOnFailure();
        using ComPtr<IMFSample> sample = new(rawSample);
        uint bytes = (uint)(pcm.Length * sizeof(short));
        IMFMediaBuffer* rawBuffer;
        NativeMethods.MFCreateMemoryBuffer(bytes, &rawBuffer).ThrowOnFailure();
        using ComPtr<IMFMediaBuffer> buffer = new(rawBuffer);
        byte* destination;
        buffer.Pointer->Lock(&destination, null, null).ThrowOnFailure();
        try { pcm.CopyTo(new Span<short>(destination, pcm.Length)); }
        finally { buffer.Pointer->Unlock().ThrowOnFailure(); }
        buffer.Pointer->SetCurrentLength(bytes).ThrowOnFailure();
        sample.Pointer->AddBuffer(buffer.Pointer).ThrowOnFailure();
        sample.Pointer->SetSampleTime(frameOffset * TimeSpan.TicksPerSecond / 48_000).ThrowOnFailure();
        sample.Pointer->SetSampleDuration((frameOffset + pcm.Length / 2) * TimeSpan.TicksPerSecond / 48_000 - frameOffset * TimeSpan.TicksPerSecond / 48_000).ThrowOnFailure();
        writer.Pointer->WriteSample(audioStream, sample.Pointer).ThrowOnFailure();
    }

    public void Finish()
    {
        if (started && !finalized)
        {
            writer.Pointer->NotifyEndOfSegment(videoStream).ThrowOnFailure();
            writer.Pointer->FinalizeWriting().ThrowOnFailure();
            finalized = true;
        }
    }

    public void Dispose()
    {
        // Finalization is explicit: failures must reach the caller, never be hidden during disposal.
        writer?.Dispose(); deviceManager?.Dispose();
    }
}