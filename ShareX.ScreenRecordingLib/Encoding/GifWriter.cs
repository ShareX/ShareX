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

using ShareX.HelpersLib;
using ShareX.ScreenRecordingLib.Video;
using SkiaSharp;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO.Compression;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace ShareX.ScreenRecordingLib.Encoding;

/// <summary>Caches lossless BGRA frames during capture, then encodes an adaptive GIF after stopping.</summary>
internal sealed unsafe class GifWriter : IDisposable
{
    public static VideoEncoderInfo Encoder { get; } = new("ShareX adaptive GIF encoder", false, false);
    private sealed record CachedFrame(byte[] Pixels, long Timestamp);
    private readonly GraphicsDevice graphics;
    private readonly ID3D11Texture2D staging = null!;
    private readonly SKImageInfo imageInfo;
    private readonly string outputPath;
    private readonly int frameBytes;
    private readonly BlockingCollection<CachedFrame> frames;
    private readonly FileStream cache = null!;
    private readonly Task? caching;
    private long frameCount;
    private bool disposed;

    public GifWriter(GraphicsDevice graphics, RecordingOptions options, int width, int height)
    {
        this.graphics = graphics;
        outputPath = options.OutputPath;
        imageInfo = new(width, height, SKColorType.Bgra8888, SKAlphaType.Opaque);
        frameBytes = checked(width * height * 4);
        frames = new(Math.Clamp(64 * 1024 * 1024 / frameBytes, 1, 8));
        try
        {
            staging = graphics.Device.CreateTexture2D(new Texture2DDescription
            {
                Width = (uint)width,
                Height = (uint)height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new(1, 0),
                Usage = ResourceUsage.Staging,
                CPUAccessFlags = CpuAccessFlags.Read
            });
            string cachePath = Path.Combine(Path.GetDirectoryName(outputPath)!, $".{Path.GetFileName(outputPath)}.{Guid.NewGuid():N}.frames");
            cache = new(cachePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 1024 * 1024,
                FileOptions.SequentialScan | FileOptions.DeleteOnClose);
            caching = Task.Factory.StartNew(CacheFrames, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        catch { Dispose(); throw; }
    }

    public bool TryWriteVideo(ID3D11Texture2D texture, long timestamp)
    {
        if (caching!.IsFaulted) caching.GetAwaiter().GetResult();
        if (frames.Count == frames.BoundedCapacity) return false;
        byte[] pixels = ArrayPool<byte>.Shared.Rent(frameBytes);
        bool queued = false;
        try
        {
            graphics.Context.CopyResource(staging, texture);
            graphics.Context.Map(staging, 0, MapMode.Read, Vortice.Direct3D11.MapFlags.None, out MappedSubresource mapped).CheckError();
            try
            {
                // Copy only the pixels; GPU rows may contain padding. No quantization runs during capture.
                fixed (byte* destination = pixels)
                {
                    int rowBytes = imageInfo.Width * 4;
                    for (int y = 0; y < imageInfo.Height; y++)
                        Buffer.MemoryCopy((byte*)mapped.DataPointer + y * mapped.RowPitch,
                            destination + y * rowBytes, rowBytes, rowBytes);
                }
            }
            finally { graphics.Context.Unmap(staging, 0); }
            queued = frames.TryAdd(new(pixels, timestamp));
            return queued;
        }
        finally { if (!queued) ArrayPool<byte>.Shared.Return(pixels); }
    }

    private void CacheFrames()
    {
        using BinaryWriter writer = new(cache, System.Text.Encoding.UTF8, true);
        foreach (CachedFrame frame in frames.GetConsumingEnumerable())
        {
            try
            {
                writer.Write(frame.Timestamp);
                long lengthPosition = cache.Position;
                writer.Write(0L);
                long start = cache.Position;
                using (DeflateStream compressed = new(cache, CompressionLevel.Fastest, true))
                    compressed.Write(frame.Pixels, 0, frameBytes);
                long end = cache.Position;
                cache.Position = lengthPosition;
                writer.Write(end - start);
                cache.Position = end;
                frameCount++;
            }
            finally { ArrayPool<byte>.Shared.Return(frame.Pixels); }
        }
        cache.Flush();
    }

    public void Finish(long duration, Action<int> progress, Func<bool> aborted)
    {
        frames.CompleteAdding();
        if (aborted()) throw new OperationCanceledException("GIF recording aborted.");
        progress(0);
        caching!.GetAwaiter().GetResult();
        cache.Position = 0;
        using BinaryReader reader = new(cache, System.Text.Encoding.UTF8, true);
        using SKBitmap bitmap = new(imageInfo);
        AdaptiveGifQuantizer quantizer = new();
        int previousProgress = 0;
        void ReportProgress(int percent)
        {
            if (percent != previousProgress) { progress(percent); previousProgress = percent; }
        }
        // Analyze the lossless cache once to build a stable palette for the whole recording.
        // Palette construction and nearest-color caches can then be reused for every frame.
        for (long index = 0; index < frameCount; index++)
        {
            if (aborted()) throw new OperationCanceledException("GIF encoding aborted.");
            ReadFrame(reader, bitmap);
            quantizer.AddFrame(bitmap);
            ReportProgress((int)((index + 1) * 20 / frameCount));
        }
        cache.Position = 0;
        using AnimatedGifCreator creator = new(outputPath, 0);
        long writtenCentiseconds = 0;
        for (long index = 0; index < frameCount; index++)
        {
            if (aborted()) throw new OperationCanceledException("GIF encoding aborted.");
            ReadFrame(reader, bitmap);
            IndexedImage image = quantizer.Quantize(bitmap);
            long next = index + 1 < frameCount ? reader.ReadInt64() : duration;
            if (index + 1 < frameCount) cache.Position -= sizeof(long);
            WriteFrame(creator, image, next, ref writtenCentiseconds);
            ReportProgress(20 + (int)((index + 1) * 80 / frameCount));
        }
        if (aborted()) throw new OperationCanceledException("GIF encoding aborted.");
    }

    private void ReadFrame(BinaryReader reader, SKBitmap bitmap)
    {
        reader.ReadInt64(); // Timestamp of this frame; the next timestamp determines its duration.
        int compressedLength = checked((int)reader.ReadInt64());
        byte[] compressed = ArrayPool<byte>.Shared.Rent(compressedLength);
        try
        {
            cache.ReadExactly(compressed.AsSpan(0, compressedLength));
            using MemoryStream input = new(compressed, 0, compressedLength, false);
            using DeflateStream decompressor = new(input, CompressionMode.Decompress);
            decompressor.ReadExactly(new Span<byte>((void*)bitmap.GetPixels(), frameBytes));
        }
        finally { ArrayPool<byte>.Shared.Return(compressed); }
        // Captured windows can contain unused alpha bytes. Screen recordings are fully opaque.
        Span<uint> pixels = new((void*)bitmap.GetPixels(), frameBytes / 4);
        for (int pixel = 0; pixel < pixels.Length; pixel++) pixels[pixel] |= 0xFF000000;
        bitmap.NotifyPixelsChanged();
    }

    private static void WriteFrame(AnimatedGifCreator creator, IndexedImage image, long until, ref long writtenCentiseconds)
    {
        // Round cumulative timestamps to avoid drift at rates such as 15 FPS, preserving dropped slots.
        const long ticksPerCentisecond = TimeSpan.TicksPerMillisecond * 10;
        long end = Math.Max(writtenCentiseconds + 1, (until + ticksPerCentisecond / 2) / ticksPerCentisecond);
        while (writtenCentiseconds < end)
        {
            int delay = (int)Math.Min(end - writtenCentiseconds, ushort.MaxValue);
            creator.AddFrame(image, delay * 10);
            writtenCentiseconds += delay;
        }
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        frames.CompleteAdding();
        // Disposal also handles startup failure and aborts. Do not encode discarded frames.
        try { caching?.GetAwaiter().GetResult(); }
        catch { } // The producer or Finish reports cache failures; cleanup must still release buffers and files.
        while (frames.TryTake(out CachedFrame? frame)) ArrayPool<byte>.Shared.Return(frame.Pixels);
        frames.Dispose();
        try { cache?.Dispose(); }
        finally { staging?.Dispose(); }
    }
}
