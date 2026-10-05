// SPDX-License-Identifier: GPL-3.0-or-later
using System.Collections.Concurrent;
using ShareX.ScreenRecordingLib.Native;

namespace ShareX.ScreenRecordingLib.Video;

/// <summary>Textures are returned by IMFTrackedSample only after the encoder releases the sample.</summary>
internal sealed unsafe class VideoTexturePool : IDisposable
{
    internal sealed class Slot : IDisposable
    {
        public required ComPtr<ID3D11Texture2D> Texture;
        public required ComPtr<ID3D11VideoProcessorOutputView> View;
        public required SampleReleaseCallback Callback;
        public void Dispose() { Callback.Dispose(); View.Dispose(); Texture.Dispose(); }
    }

    private readonly List<Slot> slots = new();
    private readonly ConcurrentQueue<Slot> available = new();
    private int disposed;

    public VideoTexturePool(GraphicsDevice graphics, GpuVideoProcessor processor, D3D11_BIND_FLAG encoderBindFlags)
    {
        try
        {
            for (int i = 0; i < 8; i++)
            {
                ComPtr<ID3D11Texture2D> texture = graphics.CreateTexture(processor.Width, processor.Height,
                    DXGI_FORMAT.DXGI_FORMAT_NV12, encoderBindFlags | D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET);
                ComPtr<ID3D11VideoProcessorOutputView> view;
                try { view = processor.CreateOutputView(texture); }
                catch { texture.Dispose(); throw; }
                Slot slot = new() { Texture = texture, View = view, Callback = null! };
                slot.Callback = new(() => { if (Volatile.Read(ref disposed) == 0) available.Enqueue(slot); });
                slots.Add(slot); available.Enqueue(slot);
            }
        }
        catch { Dispose(); throw; }
    }

    public bool TryRent(out Slot? slot) => available.TryDequeue(out slot);
    public void Return(Slot slot) { if (Volatile.Read(ref disposed) == 0) available.Enqueue(slot); }

    public ComPtr<IMFSample> CreateSample(Slot slot)
    {
        IMFSample* rawSample;
        NativeMethods.MFCreateVideoSampleFromSurface(null, &rawSample).ThrowOnFailure();
        ComPtr<IMFSample> sample = new(rawSample);
        bool allocatorSet = false;
        try
        {
            Guid iid = typeof(ID3D11Texture2D).GUID;
            IMFMediaBuffer* rawBuffer;
            NativeMethods.MFCreateDXGISurfaceBuffer(&iid, (IUnknown*)slot.Texture.Pointer, 0, false, &rawBuffer).ThrowOnFailure();
            using ComPtr<IMFMediaBuffer> buffer = new(rawBuffer);
            buffer.Pointer->SetCurrentLength((uint)(slot.TextureSize())).ThrowOnFailure();
            sample.Pointer->AddBuffer(buffer.Pointer).ThrowOnFailure();
            using ComPtr<IMFTrackedSample> tracked = sample.Query<IMFTrackedSample>();
            tracked.Pointer->SetAllocator(slot.Callback.Pointer, null).ThrowOnFailure();
            allocatorSet = true;
            return sample;
        }
        catch { sample.Dispose(); if (!allocatorSet) Return(slot); throw; }
    }

    public void Dispose()
    {
        Interlocked.Exchange(ref disposed, 1);
        foreach (Slot slot in slots) slot.Dispose();
        slots.Clear();
        available.Clear();
    }
}

internal static unsafe class VideoTextureSlotExtensions
{
    public static int TextureSize(this VideoTexturePool.Slot slot)
    {
        D3D11_TEXTURE2D_DESC desc;
        slot.Texture.Pointer->GetDesc(&desc);
        return checked((int)(desc.Width * desc.Height * 3 / 2));
    }
}