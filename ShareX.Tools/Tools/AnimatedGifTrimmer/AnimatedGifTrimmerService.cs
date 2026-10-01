#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using ShareX.Tools.Localization;
using System.Drawing;
using SkiaSharp;


namespace ShareX.Tools;

/// <summary>A GIF's cut points are frame boundaries; all times are stored as GIF centiseconds.</summary>
internal sealed class AnimatedGifTrimmerDocument : IDisposable
{
    private readonly SkiaAnimatedImage _image;
    private readonly GifLosslessTrimmer _gif;
    private readonly object _imageLock = new();
    private readonly long[] _starts;
    private readonly int[] _delays;
    private bool _disposed;

    public string FilePath { get; }
    public int FrameCount => _delays.Length;
    public double Duration => _starts[^1] / 100d;
    public double FrameStart(int index) => _starts[index] / 100d;
    public double FrameEnd(int index) => _starts[index + 1] / 100d;
    public bool CanCopySelection(int firstFrame, int endFrameExclusive) =>
        _gif.CanCopySelection(firstFrame, endFrameExclusive);

    private AnimatedGifTrimmerDocument(string filePath, SkiaAnimatedImage image, GifLosslessTrimmer gif,
        long[] starts, int[] delays)
    {
        FilePath = filePath;
        _image = image;
        _gif = gif;
        _starts = starts;
        _delays = delays;
    }

    public static AnimatedGifTrimmerDocument Open(string filePath, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (!File.Exists(filePath) || !string.Equals(Path.GetExtension(filePath), ".gif", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException(Strings.AnimatedGifTrimmer_InvalidGif);

        GifLosslessTrimmer gif = GifLosslessTrimmer.Parse(filePath, token);
        SkiaAnimatedImage image = new SkiaAnimatedImage(filePath);
        try
        {
            if (image.Format != SKEncodedImageFormat.Gif)
                throw new InvalidDataException(Strings.AnimatedGifTrimmer_InvalidGif);

            int count = image.FrameCount;
            if (count != gif.FrameCount) throw new InvalidDataException(Strings.AnimatedGifTrimmer_InvalidGif);

            long[] starts = new long[count + 1];
            int[] delays = new int[count];
            for (int i = 0; i < count; i++)
            {
                token.ThrowIfCancellationRequested();
                int delay = gif.GetDelay(i);
                delays[i] = delay;
                starts[i + 1] = checked(starts[i] + delay);
            }

            return new AnimatedGifTrimmerDocument(filePath, image, gif, starts, delays);
        }
        catch
        {
            image.Dispose();
            throw;
        }
    }

    public int FrameAt(double seconds)
    {
        long centiseconds = (long)Math.Clamp(Math.Floor(seconds * 100), 0, _starts[^1] - 1);
        int index = Array.BinarySearch(_starts, centiseconds);
        return Math.Clamp(index >= 0 ? index : ~index - 1, 0, FrameCount - 1);
    }

    public int NearestBoundary(double seconds, bool includeEnd)
    {
        long centiseconds = (long)Math.Clamp(Math.Round(seconds * 100), 0, _starts[^1]);
        int index = Array.BinarySearch(_starts, centiseconds);
        if (index < 0)
        {
            index = ~index;
            if (index > 0 && (index == _starts.Length || centiseconds - _starts[index - 1] < _starts[index] - centiseconds))
                index--;
        }
        return Math.Clamp(index, 0, includeEnd ? FrameCount : FrameCount - 1);
    }

    public byte[] RenderFrame(int index, CancellationToken token, int maxDimension = 640)
    {
        token.ThrowIfCancellationRequested();
        lock (_imageLock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            token.ThrowIfCancellationRequested();
            return RenderFrameCore(index, maxDimension);
        }
    }

    public byte[]? TryRenderFrame(int index, CancellationToken token)
    {
        if (token.IsCancellationRequested) return null;
        lock (_imageLock)
        {
            if (_disposed || token.IsCancellationRequested) return null;
            return RenderFrameCore(index, int.MaxValue);
        }
    }

    private byte[] RenderFrameCore(int index, int maxDimension)
    {
        using SKBitmap decoded = _image.GetFrame(index);
        using SKBitmap frame = DrawFrame(decoded, maxDimension);
        using MemoryStream stream = new();
        frame.Save(stream, SKEncodedImageFormat.Png);
        return stream.ToArray();
    }

    /// <returns>False when the selected lossless cut needs frames before its start.</returns>
    public bool Export(string output, int firstFrame, int endFrameExclusive, bool reencode,
        IProgress<double>? progress, CancellationToken token)
    {
        if (firstFrame < 0 || endFrameExclusive > FrameCount || firstFrame >= endFrameExclusive)
            throw new ArgumentOutOfRangeException(nameof(firstFrame));
        if (string.Equals(Path.GetFullPath(FilePath), Path.GetFullPath(output), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(Strings.AnimatedGifTrimmer_SourceOverwrite);
        if (!string.Equals(Path.GetExtension(output), ".gif", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(Strings.AnimatedGifTrimmer_OutputExtension);
        if (!reencode && !CanCopySelection(firstFrame, endFrameExclusive)) return false;

        string temporary = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(output))!,
            $".sharex-gif-trim-{Guid.NewGuid():N}.gif");
        try
        {
            if (reencode) Reencode(temporary, firstFrame, endFrameExclusive, progress, token);
            else _gif.Copy(FilePath, temporary, firstFrame, endFrameExclusive, progress, token);
            token.ThrowIfCancellationRequested();
            if (!reencode && _gif.NeedsVisualValidation(firstFrame) &&
                !FirstFrameMatches(temporary, firstFrame, token)) return false;
            File.Move(temporary, output, true);
            return true;
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }

    private void Reencode(string temporary, int firstFrame, int endFrameExclusive,
        IProgress<double>? progress, CancellationToken token)
    {
        using SkiaAnimatedImage image = new SkiaAnimatedImage(FilePath);
        using AnimatedGifCreator creator = new(temporary, _delays[firstFrame] * 10,
            _gif.RepeatCount, _gif.Loop);
        for (int i = firstFrame; i < endFrameExclusive; i++)
        {
            token.ThrowIfCancellationRequested();
            using SKBitmap frame = image.GetFrame(i);
            IndexedImage quantized = GifFrameQuantizer.Quantize(frame);
            creator.AddFrame(quantized, _delays[i] * 10);
            progress?.Report((i - firstFrame + 1d) / (endFrameExclusive - firstFrame) * 100);
        }
    }

    private static SKBitmap DrawFrame(SKBitmap source, int maxDimension = int.MaxValue)
    {
        double scale = Math.Min(1, maxDimension / (double)Math.Max(source.Width, source.Height));
        return SkiaImageHelpers.Resize(source, Math.Max(1, (int)Math.Round(source.Width * scale)),
            Math.Max(1, (int)Math.Round(source.Height * scale)));
    }

    private bool FirstFrameMatches(string trimmedPath, int originalIndex, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        using SkiaAnimatedImage original = new SkiaAnimatedImage(FilePath);
        using SkiaAnimatedImage trimmed = new SkiaAnimatedImage(trimmedPath);
        using SKBitmap originalFrame = original.GetFrame(originalIndex);
        using SKBitmap trimmedFrame = trimmed.GetFrame(0);
        return SamePixels(originalFrame, trimmedFrame, token);
    }

    private static unsafe bool SamePixels(SKBitmap first, SKBitmap second, CancellationToken token)
    {
        if (first.GetSize() != second.GetSize()) return false;
        using SkiaPixelBuffer firstPixels = new(first, true, PixelAccess.ReadOnly);
        using SkiaPixelBuffer secondPixels = new(second, true, PixelAccess.ReadOnly);
        for (int y = 0; y < first.Height; y++)
        {
            token.ThrowIfCancellationRequested();
            if (!new ReadOnlySpan<ColorBgra>(firstPixels.Pointer + y * first.Width, first.Width).SequenceEqual(
                new ReadOnlySpan<ColorBgra>(secondPixels.Pointer + y * second.Width, second.Width))) return false;
        }
        return true;
    }

    public void Dispose()
    {
        lock (_imageLock)
        {
            if (_disposed) return;
            _disposed = true;
            _image.Dispose();
        }
    }
}
