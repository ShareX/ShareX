// SPDX-License-Identifier: GPL-3.0-or-later
using System.Drawing;
using Vortice;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.MediaFoundation;

namespace ShareX.ScreenRecordingLib.Video;

/// <summary>Converts only new camera frames to a reusable small BGRA tile, entirely on the GPU.</summary>
internal sealed unsafe class CameraOverlay : IDisposable
{
    private readonly GraphicsDevice graphics;
    private readonly CameraCaptureInfo info;
    private readonly int defaultStride;
    private readonly Format format;
    private readonly int rowBytes, imageBytes;
    private ID3D11VideoDevice videoDevice = null!;
    private ID3D11VideoContext videoContext = null!;
    private ID3D11VideoProcessorEnumerator enumerator = null!;
    private ID3D11VideoProcessor processor = null!;
    private ID3D11VideoProcessorOutputView output = null!;
    private CameraCircleShader? circleShader;
    private ID3D11Texture2D? upload;
    private byte[]? uploadBytes;
    private IMFSample? retainedSample;
    private uint frameNumber;
    private readonly VideoProcessorStream[] streams = new VideoProcessorStream[1];
    public ID3D11Texture2D Texture { get; private set; } = null!;
    public Rectangle Bounds { get; }
    public bool HasFrame { get; private set; }

    public CameraOverlay(GraphicsDevice graphics, CameraCaptureInfo info, Guid subtype, int stride, uint matrix, uint range,
        RecordingOptions options, int recordingWidth, int recordingHeight)
    {
        this.graphics = graphics;
        this.info = info;
        defaultStride = stride;
        format = subtype == VideoFormatGuids.NV12 ? Format.NV12 : subtype == VideoFormatGuids.YUY2 ? Format.YUY2 : Format.B8G8R8A8_UNorm;
        rowBytes = checked(info.Width * (format == Format.NV12 ? 1 : format == Format.YUY2 ? 2 : 4));
        imageBytes = checked(rowBytes * info.Height * (format == Format.NV12 ? 3 : 2) / 2);
        Bounds = GetBounds(options, info, recordingWidth, recordingHeight);
        try
        {
            videoDevice = graphics.Device.QueryInterface<ID3D11VideoDevice>();
            videoContext = graphics.Context.QueryInterface<ID3D11VideoContext>();
            enumerator = videoDevice.CreateVideoProcessorEnumerator(new VideoProcessorContentDescription
            {
                InputFrameFormat = VideoFrameFormat.Progressive,
                InputFrameRate = new((uint)options.CameraFramesPerSecond, 1),
                OutputFrameRate = new((uint)options.CameraFramesPerSecond, 1),
                InputWidth = (uint)info.Width,
                InputHeight = (uint)info.Height,
                OutputWidth = (uint)Bounds.Width,
                OutputHeight = (uint)Bounds.Height,
                Usage = VideoUsage.PlaybackNormal
            });
            if ((enumerator.CheckVideoProcessorFormat(format) & VideoProcessorFormatSupport.Input) == 0 ||
                (enumerator.CheckVideoProcessorFormat(Format.B8G8R8A8_UNorm) & VideoProcessorFormatSupport.Output) == 0)
                throw new NotSupportedException($"The GPU cannot process the camera's {info.PixelFormat} format.");
            processor = videoDevice.CreateVideoProcessor(enumerator, 0);
            bool circular = options.CameraShape == CameraOverlayShape.Circle;
            Texture = graphics.CreateTexture(Bounds.Width, Bounds.Height, Format.B8G8R8A8_UNorm,
                BindFlags.RenderTarget | (circular ? BindFlags.ShaderResource : BindFlags.None));
            output = videoDevice.CreateVideoProcessorOutputView(Texture, enumerator,
                new VideoProcessorOutputViewDescription { ViewDimension = VideoProcessorOutputViewDimension.Texture2D });
            videoContext.VideoProcessorSetStreamFrameFormat(processor, 0, VideoFrameFormat.Progressive);
            videoContext.VideoProcessorSetStreamAutoProcessingMode(processor, 0, false);
            videoContext.VideoProcessorSetStreamColorSpace(processor, 0,
                // Media Foundation and D3D11 assign opposite numeric values to full/limited range.
                new VideoProcessorColorSpace { RGB_Range = 0, YCbCr_Matrix = matrix == 1 ? 1u : 0u, Nominal_Range = range == 1 ? 2u : 1u });
            videoContext.VideoProcessorSetOutputColorSpace(processor, new VideoProcessorColorSpace { RGB_Range = 0, Nominal_Range = 2 });
            videoContext.VideoProcessorSetOutputAlphaFillMode(processor, VideoProcessorAlphaFillMode.Opaque, 0);
            RawRect source = new(0, 0, info.Width, info.Height);
            if (circular)
            {
                int side = Math.Min(info.Width, info.Height);
                // Align subsampled camera formats to their chroma grid.
                if (format is Format.NV12 or Format.YUY2) side &= ~1;
                int left = (info.Width - side) / 2, top = (info.Height - side) / 2;
                if (format is Format.NV12 or Format.YUY2) left &= ~1;
                if (format == Format.NV12) top &= ~1;
                source = new(left, top, left + side, top + side);
            }
            videoContext.VideoProcessorSetStreamSourceRect(processor, 0, true, source);
            RawRect destination = new(0, 0, Bounds.Width, Bounds.Height);
            videoContext.VideoProcessorSetStreamDestRect(processor, 0, true, destination);
            videoContext.VideoProcessorSetOutputTargetRect(processor, true, destination);
            if (circular) circleShader = new(graphics, Texture);
        }
        catch { Dispose(); throw; }
    }

    internal static Rectangle GetBounds(RecordingOptions options, CameraCaptureInfo info, int width, int height)
    {
        int margin = Math.Min(options.CameraMargin, Math.Max(0, Math.Min(width, height) / 2 - 1));
        int availableWidth = width - margin * 2, availableHeight = height - margin * 2;
        int tileWidth = Math.Clamp(width * options.CameraWidthPercent / 100, 2, availableWidth);
        bool circular = options.CameraShape == CameraOverlayShape.Circle;
        int tileHeight = circular ? tileWidth : Math.Max(2, (int)((long)tileWidth * info.Height / info.Width));
        if (tileHeight > availableHeight)
        {
            tileHeight = availableHeight;
            tileWidth = circular ? tileHeight : Math.Clamp((int)((long)tileHeight * info.Width / info.Height), 2, availableWidth);
        }
        int x = options.CameraPosition is CameraOverlayPosition.TopRight or CameraOverlayPosition.BottomRight ? width - margin - tileWidth : margin;
        int y = options.CameraPosition is CameraOverlayPosition.BottomLeft or CameraOverlayPosition.BottomRight ? height - margin - tileHeight : margin;
        return new(x, y, tileWidth, tileHeight);
    }

    // The circle is blended into a camera-sized copy of the screen. Its opaque corners
    // work with both video-processor composition and the GPU-copy fallback.
    public ID3D11Texture2D GetCompositionTexture(ID3D11Texture2D canvas) =>
        circleShader?.Compose(canvas, Bounds) ?? Texture;

    // Takes ownership of the sample. Retain it until the next GPU operation so native camera
    // buffers are not returned while we are still submitting their video-processing commands.
    public void Update(IMFSample sample)
    {
        IMFSample? previous = retainedSample;
        retainedSample = sample;
        try
        {
            using IMFMediaBuffer buffer = sample.BufferCount == 1 ? sample.GetBufferByIndex(0) : sample.ConvertToContiguousBuffer();
            using IMFDXGIBuffer? dxgi = buffer.QueryInterfaceOrNull<IMFDXGIBuffer>();
            ID3D11Texture2D? cameraTexture = null;
            try
            {
                uint subresource = 0;
                if (dxgi != null)
                {
                    cameraTexture = new(dxgi.GetResource(typeof(ID3D11Texture2D).GUID));
                    using ID3D11Device cameraDevice = cameraTexture.Device;
                    // A camera can choose another adapter. In that case only its image uses the CPU bridge.
                    if (cameraDevice.NativePointer != graphics.Device.NativePointer)
                    {
                        cameraTexture.Dispose(); cameraTexture = null;
                    }
                    else subresource = dxgi.SubresourceIndex;
                }
                if (cameraTexture == null)
                {
                    UploadCameraBuffer(buffer);
                    upload!.AddRef();
                    cameraTexture = new(upload.NativePointer);
                }
                Texture2DDescription desc = cameraTexture.Description;
                using ID3D11VideoProcessorInputView input = videoDevice.CreateVideoProcessorInputView(cameraTexture, enumerator,
                    new VideoProcessorInputViewDescription
                    {
                        ViewDimension = VideoProcessorInputViewDimension.Texture2D,
                        Texture2D = new() { MipSlice = subresource % desc.MipLevels, ArraySlice = subresource / desc.MipLevels }
                    });
                streams[0] = new() { Enable = true, InputSurface = input };
                videoContext.VideoProcessorBlt(processor, output, frameNumber++, streams).CheckError();
                HasFrame = true;
            }
            finally { cameraTexture?.Dispose(); }
        }
        finally { previous?.Dispose(); }
    }

    private void UploadCameraBuffer(IMFMediaBuffer buffer)
    {
        upload ??= graphics.CreateTexture(info.Width, info.Height, format, BindFlags.None);
        uploadBytes ??= new byte[imageBytes];
        using IMF2DBuffer? twoDimensional = buffer.QueryInterfaceOrNull<IMF2DBuffer>();
        if (twoDimensional != null)
        {
            twoDimensional.Lock2D(out nint scanline, out int pitch);
            try { CopyRows(scanline, pitch); }
            finally { twoDimensional.Unlock2D(); }
        }
        else
        {
            buffer.Lock(out nint data, out _, out int length);
            try
            {
                int pitch = defaultStride;
                int absolutePitch = checked(Math.Abs(pitch));
                int rows = format == Format.NV12 ? info.Height * 3 / 2 : info.Height;
                if (absolutePitch < rowBytes || (long)absolutePitch * rows > length)
                    throw new IOException("The camera delivered a truncated video frame or invalid stride.");
                nint top = pitch < 0 ? data + (info.Height - 1) * absolutePitch : data;
                CopyRows(top, pitch);
            }
            finally { buffer.Unlock(); }
        }
        fixed (byte* bytes = uploadBytes)
            graphics.Context.UpdateSubresource(upload, 0, null, (nint)bytes, (uint)rowBytes, 0);
    }

    private void CopyRows(nint top, int pitch)
    {
        if (Math.Abs((long)pitch) < rowBytes || (format == Format.NV12 && pitch < 0))
            throw new IOException("The camera delivered an unsupported video stride.");
        for (int row = 0; row < info.Height; row++)
            new ReadOnlySpan<byte>((void*)(top + row * pitch), rowBytes).CopyTo(uploadBytes.AsSpan(row * rowBytes, rowBytes));
        if (format == Format.NV12)
            for (int row = 0; row < info.Height / 2; row++)
                new ReadOnlySpan<byte>((void*)(top + (info.Height + row) * pitch), rowBytes)
                    .CopyTo(uploadBytes.AsSpan((info.Height + row) * rowBytes, rowBytes));
    }

    public void Disable()
    {
        HasFrame = false;
        // Return the last native camera buffer before shutting down its source.
        retainedSample?.Dispose(); retainedSample = null;
    }
    public void Dispose()
    {
        retainedSample?.Dispose(); retainedSample = null;
        circleShader?.Dispose(); output?.Dispose(); Texture?.Dispose(); upload?.Dispose(); processor?.Dispose();
        enumerator?.Dispose(); videoContext?.Dispose(); videoDevice?.Dispose();
    }
}
