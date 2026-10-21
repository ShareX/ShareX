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
using System.Drawing;
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
        using AnimatedGifCreator creator = new(outputPath, 0) { UseGlobalPalette = true };
        IndexedImage? previous = null;
        IndexedImage? pending = null;
        Point offset = Point.Empty;
        int transparentIndex = -1;
        long writtenCentiseconds = 0;
        for (long index = 0; index < frameCount; index++)
        {
            if (aborted()) throw new OperationCanceledException("GIF encoding aborted.");
            long timestamp = ReadFrame(reader, bitmap);
            IndexedImage image = quantizer.Quantize(bitmap);
            // Shared palette indices identify all changed pixels, including restored background.
            // Skip duplicate frames and encode the rectangle containing the changes.
            Rectangle bounds = previous == null ? new(0, 0, image.Width, image.Height) : GetChangedBounds(previous, image);
            if (!bounds.IsEmpty)
            {
                if (pending != null) WriteFrame(creator, pending, offset, transparentIndex, timestamp, ref writtenCentiseconds);
                pending = CropFrame(image, previous, bounds, out transparentIndex);
                offset = bounds.Location;
            }
            previous = image;
            if (index + 1 < frameCount) ReportProgress(20 + (int)((index + 1) * 80 / frameCount));
        }
        if (aborted()) throw new OperationCanceledException("GIF encoding aborted.");
        if (pending != null) WriteFrame(creator, pending, offset, transparentIndex, duration, ref writtenCentiseconds);
        ReportProgress(100);
        if (aborted()) throw new OperationCanceledException("GIF encoding aborted.");
    }

    private long ReadFrame(BinaryReader reader, SKBitmap bitmap)
    {
        long timestamp = reader.ReadInt64();
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
        return timestamp;
    }

    private static Rectangle GetChangedBounds(IndexedImage previous, IndexedImage image)
    {
        int left = image.Width, top = image.Height, right = -1, bottom = -1;
        for (int y = 0; y < image.Height; y++)
        {
            ReadOnlySpan<byte> current = image.Pixels.AsSpan(y * image.Width, image.Width);
            ReadOnlySpan<byte> before = previous.Pixels.AsSpan(y * image.Width, image.Width);
            int first = current.CommonPrefixLength(before);
            if (first == image.Width) continue;
            // Only search for a new right edge when pixels beyond the known edge differ.
            // Span comparisons skip unchanged rows and margins using vectorized operations.
            if (!current[(right + 1)..].SequenceEqual(before[(right + 1)..]))
            {
                int last = image.Width - 1;
                while (last > first && current[last] == before[last]) last--;
                right = last;
            }
            left = Math.Min(left, first);
            top = Math.Min(top, y);
            bottom = y;
        }
        return bottom < 0 ? Rectangle.Empty : Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
    }

    private static IndexedImage CropFrame(IndexedImage image, IndexedImage? previous, Rectangle bounds, out int transparentIndex)
    {
        transparentIndex = -1;
        if (previous != null)
        {
            Span<bool> usedColors = stackalloc bool[256];
            usedColors.Clear();
            int usedColorCount = 0;
            bool hasUnchangedPixels = false;
            for (int y = bounds.Top; y < bounds.Bottom && usedColorCount < image.Palette.Length; y++)
            {
                int start = y * image.Width + bounds.Left;
                for (int x = 0; x < bounds.Width; x++)
                {
                    byte color = image.Pixels[start + x];
                    if (color == previous.Pixels[start + x]) hasUnchangedPixels = true;
                    else if (!usedColors[color]) { usedColors[color] = true; usedColorCount++; }
                }
            }
            // Transparency is per frame. Borrow a palette index unused by changed pixels;
            // never remove a color or make pixels restoring old content transparent.
            if (hasUnchangedPixels && usedColorCount < image.Palette.Length)
                for (int color = 0; color < image.Palette.Length; color++)
                    if (!usedColors[color]) { transparentIndex = color; break; }
        }
        if (transparentIndex >= 0)
        {
            byte[] patch = new byte[checked(bounds.Width * bounds.Height)];
            for (int y = 0; y < bounds.Height; y++)
            {
                int source = (bounds.Y + y) * image.Width + bounds.X, destination = y * bounds.Width;
                for (int x = 0; x < bounds.Width; x++)
                    patch[destination + x] = image.Pixels[source + x] == previous!.Pixels[source + x]
                        ? (byte)transparentIndex : image.Pixels[source + x];
            }
            return new(bounds.Width, bounds.Height, patch, image.Palette);
        }
        if (bounds.Width == image.Width && bounds.Height == image.Height) return image;
        byte[] pixels = new byte[checked(bounds.Width * bounds.Height)];
        for (int y = 0; y < bounds.Height; y++)
            image.Pixels.AsSpan((bounds.Y + y) * image.Width + bounds.X, bounds.Width)
                .CopyTo(pixels.AsSpan(y * bounds.Width, bounds.Width));
        return new(bounds.Width, bounds.Height, pixels, image.Palette);
    }

    private static void WriteFrame(AnimatedGifCreator creator, IndexedImage image, Point offset, int transparentIndex,
        long until, ref long writtenCentiseconds)
    {
        // Round cumulative timestamps to avoid drift at rates such as 15 FPS, preserving dropped slots.
        const long ticksPerCentisecond = TimeSpan.TicksPerMillisecond * 10;
        long end = Math.Max(writtenCentiseconds + 1, (until + ticksPerCentisecond / 2) / ticksPerCentisecond);
        while (writtenCentiseconds < end)
        {
            int delay = (int)Math.Min(end - writtenCentiseconds, ushort.MaxValue);
            creator.AddFrame(image, delay * 10, offset.X, offset.Y, restoreBackground: false, transparentIndex: transparentIndex);
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
