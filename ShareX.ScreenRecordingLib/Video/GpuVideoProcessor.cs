// SPDX-License-Identifier: GPL-3.0-or-later
using ShareX.ScreenRecordingLib.Native;

namespace ShareX.ScreenRecordingLib.Video;

/// <summary>Crops/composes BGRA surfaces and converts to limited-range BT.709 NV12 on the GPU.</summary>
internal sealed unsafe class GpuVideoProcessor : IDisposable
{
    private readonly GraphicsDevice graphics;
    private ComPtr<ID3D11VideoDevice> videoDevice = null!;
    private ComPtr<ID3D11VideoContext> videoContext = null!;
    private ComPtr<ID3D11VideoProcessorEnumerator> enumerator = null!;
    private ComPtr<ID3D11VideoProcessor> processor = null!;
    private ComPtr<ID3D11VideoProcessorInputView> input = null!;
    private ComPtr<ID3D11RenderTargetView> canvasView = null!;
    public ComPtr<ID3D11Texture2D> Canvas { get; private set; } = null!;
    public int Width { get; }
    public int Height { get; }

    public GpuVideoProcessor(GraphicsDevice graphics, int width, int height, int fps)
    {
        this.graphics = graphics;
        Width = width;
        Height = height;
        try
        {
            videoDevice = graphics.Device.Query<ID3D11VideoDevice>();
            videoContext = graphics.Context.Query<ID3D11VideoContext>();
            D3D11_VIDEO_PROCESSOR_CONTENT_DESC desc = new()
            {
                InputFrameFormat = D3D11_VIDEO_FRAME_FORMAT.D3D11_VIDEO_FRAME_FORMAT_PROGRESSIVE,
                InputFrameRate = new() { Numerator = (uint)fps, Denominator = 1 },
                OutputFrameRate = new() { Numerator = (uint)fps, Denominator = 1 },
                InputWidth = (uint)width, InputHeight = (uint)height,
                OutputWidth = (uint)width, OutputHeight = (uint)height,
                Usage = D3D11_VIDEO_USAGE.D3D11_VIDEO_USAGE_PLAYBACK_NORMAL
            };
            ID3D11VideoProcessorEnumerator* rawEnumerator;
            videoDevice.Pointer->CreateVideoProcessorEnumerator(&desc, &rawEnumerator).ThrowOnFailure();
            enumerator = new(rawEnumerator);
            uint formatFlags;
            enumerator.Pointer->CheckVideoProcessorFormat(DXGI_FORMAT.DXGI_FORMAT_NV12, &formatFlags).ThrowOnFailure();
            if ((formatFlags & 2) == 0) throw new NotSupportedException("The GPU cannot produce NV12 video surfaces.");
            ID3D11VideoProcessor* rawProcessor;
            videoDevice.Pointer->CreateVideoProcessor(enumerator.Pointer, 0, &rawProcessor).ThrowOnFailure();
            processor = new(rawProcessor);
            Canvas = graphics.CreateTexture(width, height, DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM, D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET);
            ID3D11RenderTargetView* rawCanvasView;
            graphics.Device.Pointer->CreateRenderTargetView((ID3D11Resource*)Canvas.Pointer, null, &rawCanvasView).ThrowOnFailure();
            canvasView = new(rawCanvasView);
            Clear();
            D3D11_VIDEO_PROCESSOR_INPUT_VIEW_DESC inputDesc = new() { ViewDimension = D3D11_VPIV_DIMENSION.D3D11_VPIV_DIMENSION_TEXTURE2D };
            ID3D11VideoProcessorInputView* rawInput;
            videoDevice.Pointer->CreateVideoProcessorInputView((ID3D11Resource*)Canvas.Pointer, enumerator.Pointer, &inputDesc, &rawInput).ThrowOnFailure();
            input = new(rawInput);
            D3D11_VIDEO_PROCESSOR_COLOR_SPACE rgb = new() { RGB_Range = false, YCbCr_Matrix = true, Nominal_Range = 2 };
            D3D11_VIDEO_PROCESSOR_COLOR_SPACE yuv = new() { YCbCr_Matrix = true, Nominal_Range = 1 };
            videoContext.Pointer->VideoProcessorSetStreamColorSpace(processor.Pointer, 0, &rgb);
            videoContext.Pointer->VideoProcessorSetOutputColorSpace(processor.Pointer, &yuv);
            videoContext.Pointer->VideoProcessorSetStreamFrameFormat(processor.Pointer, 0, D3D11_VIDEO_FRAME_FORMAT.D3D11_VIDEO_FRAME_FORMAT_PROGRESSIVE);
            videoContext.Pointer->VideoProcessorSetStreamAutoProcessingMode(processor.Pointer, 0, false);
            RECT rect = new(0, 0, width, height);
            videoContext.Pointer->VideoProcessorSetStreamSourceRect(processor.Pointer, 0, true, &rect);
            videoContext.Pointer->VideoProcessorSetStreamDestRect(processor.Pointer, 0, true, &rect);
            videoContext.Pointer->VideoProcessorSetOutputTargetRect(processor.Pointer, true, &rect);
        }
        catch { Dispose(); throw; }
    }

    public void Clear()
    {
        float* black = stackalloc float[] { 0, 0, 0, 1 };
        graphics.Context.Pointer->ClearRenderTargetView(canvasView.Pointer, black);
    }

    public ComPtr<ID3D11VideoProcessorOutputView> CreateOutputView(ComPtr<ID3D11Texture2D> texture)
    {
        D3D11_VIDEO_PROCESSOR_OUTPUT_VIEW_DESC desc = new() { ViewDimension = D3D11_VPOV_DIMENSION.D3D11_VPOV_DIMENSION_TEXTURE2D };
        ID3D11VideoProcessorOutputView* view;
        videoDevice.Pointer->CreateVideoProcessorOutputView((ID3D11Resource*)texture.Pointer, enumerator.Pointer, &desc, &view).ThrowOnFailure();
        return new(view);
    }

    public void Convert(ComPtr<ID3D11VideoProcessorOutputView> output, uint frameNumber)
    {
        D3D11_VIDEO_PROCESSOR_STREAM stream = new() { Enable = true, pInputSurface = input.Pointer };
        videoContext.Pointer->VideoProcessorBlt(processor.Pointer, output.Pointer, frameNumber, 1, &stream).ThrowOnFailure();
    }

    public void Dispose()
    {
        input?.Dispose(); canvasView?.Dispose(); Canvas?.Dispose(); processor?.Dispose();
        enumerator?.Dispose(); videoContext?.Dispose(); videoDevice?.Dispose();
    }
}