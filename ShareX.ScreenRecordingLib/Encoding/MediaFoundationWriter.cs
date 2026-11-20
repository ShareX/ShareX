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

using ShareX.ScreenRecordingLib.Native;
using ShareX.ScreenRecordingLib.Video;
using SharpGen.Runtime;
using Vortice.Direct3D11;
using Vortice.MediaFoundation;

namespace ShareX.ScreenRecordingLib.Encoding;

internal sealed unsafe class MediaFoundationWriter : IDisposable
{
    private const int H264MaxWidth = 4096;
    private const int H264MaxHeight = 2304;
    private const long H264MaxPixels = (long)H264MaxWidth * H264MaxHeight; // H.264 Level 5.2 MaxFS (36,864 MB)
    private const long H264MaxPixelsPerSecond = 2_073_600L * 256; // H.264 Level 5.2 MaxMBPS
    private const int HevcMaxWidth = 8192;
    private const int HevcMaxHeight = 4352;
    private const long HevcMaxPixels = (long)HevcMaxWidth * HevcMaxHeight; // HEVC Level 6.2 MaxLumaPS
    private const long HevcMaxPixelsPerSecond = 4_278_190_080L; // HEVC Level 6.2 MaxLumaSR
    private const int MinEncoderWidth = 160; // Multiple of 16; satisfies NVENC (145), AMD AMF (128), Intel QSV and MS H.264 minimums
    private const int MinEncoderHeight = 128; // Multiple of 16; satisfies AMD AMF (128), NVENC, Intel QSV and MS H.264 minimums
    private const int HighCompatibilityBitrate = 50_000_000;
    private const int CompatibilityBitrate = 20_000_000;
    private const int BaselineCompatibilityBitrate = 10_000_000;

    internal readonly record struct EncoderAttempt(
        Guid Codec,
        int Width,
        int Height,
        int FramesPerSecond,
        int VideoBitrate,
        bool FullMediaType,
        bool EnableHardwareTransforms);

    private IMFSinkWriter writer = null!;
    private IMFDXGIDeviceManager deviceManager = null!;
    private int videoStream, audioStream;
    private bool started, finalized;
    public VideoEncoderInfo Encoder { get; private set; } = null!;
    public BindFlags EncoderBindFlags { get; private set; }
    public int Width { get; }
    public int Height { get; }
    public int FramesPerSecond { get; }
    public string? AdjustmentNote { get; private set; }

    /// <summary>H.264 when the capture fits its limits. Otherwise HEVC at full size, or negotiated fallbacks
    /// when a Windows Media Foundation encoder rejects the media type with MF_E_INVALIDMEDIATYPE.</summary>
    public static MediaFoundationWriter Create(GraphicsDevice graphics, RecordingOptions options, int width, int height)
    {
        IReadOnlyList<EncoderAttempt> attempts = BuildAttempts(width, height, options.FramesPerSecond, options.VideoBitrate, options.RequireHardwareEncoder);
        SharpGenException? lastInvalidMediaType = null;
        for (int i = 0; i < attempts.Count; i++)
        {
            EncoderAttempt attempt = attempts[i];
            try
            {
                MediaFoundationWriter created = new(graphics, options, attempt);
                if (i > 0 || attempt.Width != width || attempt.Height != height || attempt.FramesPerSecond != options.FramesPerSecond ||
                    attempt.VideoBitrate != options.VideoBitrate || attempt.Codec != VideoFormatGuids.H264 || !attempt.FullMediaType || !attempt.EnableHardwareTransforms)
                {
                    string codecName = attempt.Codec == VideoFormatGuids.Hevc ? "HEVC" : "H.264";
                    created.AdjustmentNote = $"Encoder negotiated {codecName} {attempt.Width}x{attempt.Height} @ {attempt.FramesPerSecond} FPS, {attempt.VideoBitrate / 1000} kbps"
                        + (attempt.FullMediaType ? "" : " (minimal media type)")
                        + (attempt.EnableHardwareTransforms ? "" : " (software MFT)");
                }
                return created;
            }
            catch (SharpGenException ex) when (attempt.Codec == VideoFormatGuids.Hevc || IsInvalidMediaType(ex))
            {
                if (IsInvalidMediaType(ex)) lastInvalidMediaType = ex;
            }
            catch (NotSupportedException) when (attempt.Codec == VideoFormatGuids.Hevc)
            {
                // No usable HEVC encoder for this size/rate. Fall back to H.264 attempts below.
            }
        }
        throw new NotSupportedException(
            $"Windows Media Foundation rejected the video format ({width}x{height} at {options.FramesPerSecond} FPS, {options.VideoBitrate / 1000} kbps). " +
            "Update your graphics driver, install the Windows Media Feature Pack if using a Windows N edition, or disable the built-in screen recorder to use FFmpeg.",
            lastInvalidMediaType);
    }

    internal static IReadOnlyList<EncoderAttempt> BuildAttempts(int width, int height, int fps, int bitrate, bool requireHardwareEncoder)
    {
        List<EncoderAttempt> attempts = [];
        HashSet<EncoderAttempt> seen = [];
        void Add(Guid codec, int w, int h, int rate, int bps, bool fullMediaType, bool enableHardware)
        {
            EncoderAttempt candidate = new(codec, w, h, rate, bps, fullMediaType, enableHardware);
            if (seen.Add(candidate)) attempts.Add(candidate);
        }

        (int h264Width, int h264Height) = FitSize(width, height, H264MaxWidth, H264MaxHeight, H264MaxPixels, fps, H264MaxPixelsPerSecond);
        bool exceedsH264Limits = h264Width != width || h264Height != height;

        // 1. If the capture exceeds H.264 Level 5.2 (size, height, or macroblock rate), try HEVC at full resolution/FPS first.
        if (exceedsH264Limits)
        {
            (int hevcWidth, int hevcHeight) = FitSize(width, height, HevcMaxWidth, HevcMaxHeight, HevcMaxPixels, fps, HevcMaxPixelsPerSecond);
            Add(VideoFormatGuids.Hevc, hevcWidth, hevcHeight, fps, bitrate, true, true);
            Add(VideoFormatGuids.Hevc, hevcWidth, hevcHeight, fps, bitrate, false, true);
        }

        // 2. Primary H.264 attempt at target dimensions/FPS/bitrate, then without optional profile/colorimetry attributes.
        Add(VideoFormatGuids.H264, h264Width, h264Height, fps, bitrate, true, true);
        Add(VideoFormatGuids.H264, h264Width, h264Height, fps, bitrate, false, true);

        // 3. Pad small regions to hardware minimums (160x128) and align to 16-pixel macroblock boundaries.
        (int alignedWidth, int alignedHeight) = AlignAndPad(h264Width, h264Height, H264MaxWidth, H264MaxHeight, H264MaxPixels, fps, H264MaxPixelsPerSecond);
        Add(VideoFormatGuids.H264, alignedWidth, alignedHeight, fps, bitrate, true, true);
        Add(VideoFormatGuids.H264, alignedWidth, alignedHeight, fps, bitrate, false, true);

        // 4. For high-resolution or high-FPS captures that fit Level 5.2 on paper but were rejected by the GPU's H.264 MFT, try HEVC before degrading.
        if (!exceedsH264Limits && (width > 1920 || height > 1088 || fps > 60) && width >= MinEncoderWidth && height >= MinEncoderHeight)
        {
            Add(VideoFormatGuids.Hevc, width, height, fps, bitrate, true, true);
            Add(VideoFormatGuids.Hevc, width, height, fps, bitrate, false, true);
        }

        // 5. Clamp high bitrates that exceed hardware/level limits.
        int highCompatBitrate = Math.Min(bitrate, HighCompatibilityBitrate);
        if (highCompatBitrate < bitrate)
        {
            Add(VideoFormatGuids.H264, h264Width, h264Height, fps, highCompatBitrate, false, true);
            Add(VideoFormatGuids.H264, alignedWidth, alignedHeight, fps, highCompatBitrate, false, true);
        }
        int compatBitrate = Math.Min(bitrate, CompatibilityBitrate);
        if (compatBitrate < highCompatBitrate)
        {
            Add(VideoFormatGuids.H264, h264Width, h264Height, fps, compatBitrate, false, true);
            Add(VideoFormatGuids.H264, alignedWidth, alignedHeight, fps, compatBitrate, false, true);
        }

        // 6. Cap frame rate at 60 FPS for encoders that reject >60 FPS, restoring full resolution when possible.
        if (fps > 60)
        {
            (int w60, int h60) = FitSize(width, height, H264MaxWidth, H264MaxHeight, H264MaxPixels, 60, H264MaxPixelsPerSecond);
            (int aw60, int ah60) = AlignAndPad(w60, h60, H264MaxWidth, H264MaxHeight, H264MaxPixels, 60, H264MaxPixelsPerSecond);
            Add(VideoFormatGuids.H264, w60, h60, 60, compatBitrate, true, true);
            Add(VideoFormatGuids.H264, aw60, ah60, 60, compatBitrate, false, true);
        }

        // 7. Step down to common lower hardware ceilings (4K UHD 3840x2160, then 1080p 1920x1088).
        int fallbackFps = Math.Min(fps, 60);
        foreach ((int capWidth, int capHeight) in new[] { (3840, 2160), (1920, 1088) })
        {
            (int cw, int ch) = FitSize(width, height, capWidth, capHeight, (long)capWidth * capHeight, fallbackFps, H264MaxPixelsPerSecond);
            (int acw, int ach) = AlignAndPad(cw, ch, capWidth, capHeight, (long)capWidth * capHeight, fallbackFps, H264MaxPixelsPerSecond);
            Add(VideoFormatGuids.H264, acw, ach, fallbackFps, compatBitrate, false, true);
        }
        if (fallbackFps > 30)
        {
            (int cw30, int ch30) = FitSize(width, height, 1920, 1088, 1920L * 1088, 30, H264MaxPixelsPerSecond);
            (int acw30, int ach30) = AlignAndPad(cw30, ch30, 1920, 1088, 1920L * 1088, 30, H264MaxPixelsPerSecond);
            Add(VideoFormatGuids.H264, acw30, ach30, 30, Math.Min(bitrate, BaselineCompatibilityBitrate), false, true);
        }

        // 8. When software encoding is allowed, bypass hardware MFTs if a driver's hardware MFT rejected all media types.
        if (!requireHardwareEncoder)
        {
            (int swW, int swH) = FitSize(width, height, H264MaxWidth, H264MaxHeight, H264MaxPixels, fallbackFps, H264MaxPixelsPerSecond);
            (int aswW, int aswH) = AlignAndPad(swW, swH, H264MaxWidth, H264MaxHeight, H264MaxPixels, fallbackFps, H264MaxPixelsPerSecond);
            Add(VideoFormatGuids.H264, swW, swH, fallbackFps, compatBitrate, true, false);
            Add(VideoFormatGuids.H264, aswW, aswH, fallbackFps, compatBitrate, false, false);
            (int sw1080W, int sw1080H) = FitSize(width, height, 1920, 1088, 1920L * 1088, Math.Min(fps, 30), H264MaxPixelsPerSecond);
            (int asw1080W, int asw1080H) = AlignAndPad(sw1080W, sw1080H, 1920, 1088, 1920L * 1088, Math.Min(fps, 30), H264MaxPixelsPerSecond);
            Add(VideoFormatGuids.H264, asw1080W, asw1080H, Math.Min(fps, 30), Math.Min(bitrate, BaselineCompatibilityBitrate), false, false);
        }

        return attempts;
    }

    private static bool IsInvalidMediaType(SharpGenException ex) =>
        ex.ResultCode.Code == Vortice.MediaFoundation.ResultCode.Invalidmediatype.Code;

    private MediaFoundationWriter(GraphicsDevice graphics, RecordingOptions options, EncoderAttempt attempt)
    {
        Width = attempt.Width;
        Height = attempt.Height;
        FramesPerSecond = attempt.FramesPerSecond;
        try
        {
            deviceManager = MediaFactory.MFCreateDXGIDeviceManager();
            deviceManager.ResetDevice(graphics.Device).CheckError();
            using IMFAttributes attributes = MediaFactory.MFCreateAttributes(5);
            if (attempt.EnableHardwareTransforms)
                attributes.Set(SinkWriterAttributeKeys.ReadwriteEnableHardwareTransforms, 1u).CheckError();
            if (attempt.FullMediaType)
                attributes.Set(SinkWriterAttributeKeys.LowLatency, 1u).CheckError();
            // The fixed-size GPU pool bounds in-flight samples. Do not make WriteSample wait for presentation time.
            attributes.Set(SinkWriterAttributeKeys.DisableThrottling, 1u).CheckError();
            attributes.Set(TranscodeAttributeKeys.TranscodeContainertype, TranscodeContainerTypeGuids.Mpeg4).CheckError();
            attributes.Set(SinkWriterAttributeKeys.D3DManager, deviceManager).CheckError();
            writer = MediaFactory.MFCreateSinkWriterFromURL(options.OutputPath, null!, attributes);
            using IMFMediaType videoOut = CreateVideoType(attempt.Codec, attempt.Width, attempt.Height, attempt.FramesPerSecond, attempt.FullMediaType);
            videoOut.Set(MediaTypeAttributeKeys.AvgBitrate, (uint)attempt.VideoBitrate).CheckError();
            if (attempt.FullMediaType)
                videoOut.Set(MediaTypeAttributeKeys.Mpeg2Profile, attempt.Codec == VideoFormatGuids.Hevc ? 1u : 77u).CheckError(); // HEVC Main / H.264 Main
            videoStream = writer.AddStream(videoOut);
            using IMFMediaType videoIn = CreateVideoType(VideoFormatGuids.NV12, attempt.Width, attempt.Height, attempt.FramesPerSecond, attempt.FullMediaType);
            writer.SetInputMediaType(videoStream, videoIn, null!);
            if (options.HasAudio) AddAudio(options.AudioBitrate, attempt.FullMediaType);
            Encoder = InspectVideoEncoder();
            if (options.RequireHardwareEncoder && (!Encoder.IsHardwareAccelerated || !Encoder.IsD3D11Aware))
                throw new NotSupportedException($"Windows selected '{Encoder.Name}', which does not provide D3D11 hardware video encoding on this GPU. Disable RequireHardwareEncoder to allow Windows software encoding.");
            writer.BeginWriting();
            started = true;
        }
        catch { Dispose(); throw; }
    }

    /// <summary>Downscale to the encoder's frame and macroblock-rate limits, keeping the aspect ratio.</summary>
    internal static (int Width, int Height) FitSize(int width, int height, int maxWidth, int maxHeight,
        long maxPixels, int fps, long maxPixelsPerSecond, int alignment = 2)
    {
        long effectiveMaxPixels = Math.Min(maxPixels, maxPixelsPerSecond / Math.Max(1, fps));
        double scale = Math.Min(1.0, Math.Min(
            Math.Min((double)maxWidth / width, (double)maxHeight / height),
            Math.Sqrt((double)effectiveMaxPixels / ((long)width * height))));
        if (scale >= 1.0 && width % alignment == 0 && height % alignment == 0)
            return (width, height);
        int mask = ~(alignment - 1);
        int fittedWidth = Math.Max(alignment, (int)(width * scale) & mask);
        int fittedHeight = Math.Max(alignment, (int)(height * scale) & mask);
        while ((long)fittedWidth * fittedHeight > effectiveMaxPixels && (fittedWidth > alignment || fittedHeight > alignment))
        {
            if (fittedWidth >= fittedHeight && fittedWidth > alignment) fittedWidth -= alignment;
            else if (fittedHeight > alignment) fittedHeight -= alignment;
        }
        return (fittedWidth, fittedHeight);
    }

    /// <summary>Pad tiny regions to hardware minimums and round up to 16-pixel macroblock alignment within encoder limits.</summary>
    internal static (int Width, int Height) AlignAndPad(int width, int height, int maxWidth, int maxHeight,
        long maxPixels, int fps, long maxPixelsPerSecond)
    {
        int alignedWidth = Math.Max(MinEncoderWidth, (width + 15) & ~15);
        int alignedHeight = Math.Max(MinEncoderHeight, (height + 15) & ~15);
        if (alignedWidth > maxWidth) alignedWidth = Math.Max(MinEncoderWidth, maxWidth & ~15);
        if (alignedHeight > maxHeight) alignedHeight = Math.Max(MinEncoderHeight, maxHeight & ~15);
        long effectiveMaxPixels = Math.Min(maxPixels, maxPixelsPerSecond / Math.Max(1, fps));
        if ((long)alignedWidth * alignedHeight > effectiveMaxPixels)
        {
            (alignedWidth, alignedHeight) = FitSize(alignedWidth, alignedHeight, maxWidth, maxHeight, maxPixels, fps, maxPixelsPerSecond, alignment: 16);
            alignedWidth = Math.Max(MinEncoderWidth, alignedWidth);
            alignedHeight = Math.Max(MinEncoderHeight, alignedHeight);
        }
        return (alignedWidth, alignedHeight);
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

    private static IMFMediaType CreateVideoType(Guid subtype, int width, int height, int fps, bool includeColorInfo)
    {
        IMFMediaType type = CreateType(MediaTypeGuids.Video, subtype);
        try
        {
            type.Set(MediaTypeAttributeKeys.InterlaceMode, (uint)VideoInterlaceMode.Progressive).CheckError();
            SetRatio(type, MediaTypeAttributeKeys.FrameSize, (uint)width, (uint)height);
            SetRatio(type, MediaTypeAttributeKeys.FrameRate, (uint)fps, 1);
            SetRatio(type, MediaTypeAttributeKeys.PixelAspectRatio, 1, 1);
            if (includeColorInfo)
            {
                type.Set(MediaTypeAttributeKeys.VideoPrimaries, 2u).CheckError(); // BT.709
                type.Set(MediaTypeAttributeKeys.TransferFunction, 5u).CheckError(); // BT.709
                type.Set(MediaTypeAttributeKeys.YuvMatrix, 1u).CheckError(); // BT.709
                type.Set(MediaTypeAttributeKeys.VideoNominalRange, 2u).CheckError(); // 16–235
            }
            return type;
        }
        catch { type.Dispose(); throw; }
    }

    private void AddAudio(int bitrate, bool includeProfileLevel)
    {
        using IMFMediaType output = CreateType(MediaTypeGuids.Audio, AudioFormatGuids.Aac);
        output.Set(MediaTypeAttributeKeys.AudioNumChannels, 2u).CheckError();
        output.Set(MediaTypeAttributeKeys.AudioSamplesPerSecond, 48_000u).CheckError();
        output.Set(MediaTypeAttributeKeys.AudioBitsPerSample, 16u).CheckError();
        output.Set(MediaTypeAttributeKeys.AudioAvgBytesPerSecond, (uint)(bitrate / 8)).CheckError();
        output.Set(MediaTypeAttributeKeys.AacPayloadType, 0u).CheckError();
        if (includeProfileLevel)
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
                string name = GetString(attributes, TransformAttributeKeys.MftFriendlyNameAttribute) ?? (hardware ? "Windows hardware video encoder" : "Windows video encoder");
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
        throw new NotSupportedException("Media Foundation did not expose the selected video encoder.");
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
