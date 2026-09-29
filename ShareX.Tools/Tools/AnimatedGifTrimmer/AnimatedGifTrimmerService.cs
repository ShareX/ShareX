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

using ShareX.Tools.Localization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ShareX.Tools;

/// <summary>A GIF's cut points are frame boundaries; all times are stored as GIF centiseconds.</summary>
internal sealed class AnimatedGifTrimmerDocument : IDisposable
{
    private readonly Image _image;
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

    private AnimatedGifTrimmerDocument(string filePath, Image image, GifLosslessTrimmer gif,
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
        Image image = Image.FromFile(filePath);
        try
        {
            if (image.RawFormat.Guid != ImageFormat.Gif.Guid)
                throw new InvalidDataException(Strings.AnimatedGifTrimmer_InvalidGif);

            int count = image.GetFrameCount(FrameDimension.Time);
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

    public byte[] RenderFrame(int index, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        lock (_imageLock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            token.ThrowIfCancellationRequested();
            return RenderFrameCore(index);
        }
    }

    public byte[]? TryRenderFrame(int index, CancellationToken token)
    {
        if (token.IsCancellationRequested) return null;
        lock (_imageLock)
        {
            if (_disposed || token.IsCancellationRequested) return null;
            return RenderFrameCore(index);
        }
    }

    private byte[] RenderFrameCore(int index)
    {
        _image.SelectActiveFrame(FrameDimension.Time, index);
        using Bitmap frame = DrawFrame(_image, 640);
        using MemoryStream stream = new();
        frame.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }

    public void Export(string output, int firstFrame, int endFrameExclusive, IProgress<double>? progress,
        CancellationToken token)
    {
        if (firstFrame < 0 || endFrameExclusive > FrameCount || firstFrame >= endFrameExclusive)
            throw new ArgumentOutOfRangeException(nameof(firstFrame));
        if (string.Equals(Path.GetFullPath(FilePath), Path.GetFullPath(output), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(Strings.AnimatedGifTrimmer_SourceOverwrite);
        if (!string.Equals(Path.GetExtension(output), ".gif", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(Strings.AnimatedGifTrimmer_OutputExtension);
        if (!CanCopySelection(firstFrame, endFrameExclusive))
            throw new InvalidOperationException(Strings.AnimatedGifTrimmer_DependentFrame);

        string temporary = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(output))!,
            $".sharex-gif-trim-{Guid.NewGuid():N}.gif");
        try
        {
            _gif.Copy(FilePath, temporary, firstFrame, endFrameExclusive, progress, token);
            token.ThrowIfCancellationRequested();
            if (_gif.NeedsVisualValidation(firstFrame) && !FirstFrameMatches(temporary, firstFrame, token))
                throw new InvalidOperationException(Strings.AnimatedGifTrimmer_DependentFrame);
            File.Move(temporary, output, true);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }

    private static Bitmap DrawFrame(Image source, int maxDimension = int.MaxValue)
    {
        double scale = Math.Min(1, maxDimension / (double)Math.Max(source.Width, source.Height));
        int width = Math.Max(1, (int)Math.Round(source.Width * scale));
        int height = Math.Max(1, (int)Math.Round(source.Height * scale));
        Bitmap frame = new(width, height, PixelFormat.Format32bppArgb);
        using Graphics graphics = Graphics.FromImage(frame);
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.DrawImage(source, new Rectangle(0, 0, width, height), 0, 0, source.Width, source.Height,
            GraphicsUnit.Pixel);
        return frame;
    }

    private bool FirstFrameMatches(string trimmedPath, int originalIndex, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        using Image original = Image.FromFile(FilePath);
        using Image trimmed = Image.FromFile(trimmedPath);
        original.SelectActiveFrame(FrameDimension.Time, originalIndex);
        trimmed.SelectActiveFrame(FrameDimension.Time, 0);
        using Bitmap originalFrame = DrawFrame(original);
        using Bitmap trimmedFrame = DrawFrame(trimmed);
        return SamePixels(originalFrame, trimmedFrame, token);
    }

    private static unsafe bool SamePixels(Bitmap first, Bitmap second, CancellationToken token)
    {
        if (first.Size != second.Size) return false;
        Rectangle bounds = new(Point.Empty, first.Size);
        BitmapData firstData = first.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        try
        {
            BitmapData secondData = second.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                for (int y = 0; y < first.Height; y++)
                {
                    token.ThrowIfCancellationRequested();
                    byte* firstRow = (byte*)firstData.Scan0 + y * firstData.Stride;
                    byte* secondRow = (byte*)secondData.Scan0 + y * secondData.Stride;
                    if (!new ReadOnlySpan<byte>(firstRow, first.Width * 4).SequenceEqual(
                        new ReadOnlySpan<byte>(secondRow, second.Width * 4))) return false;
                }
                return true;
            }
            finally { second.UnlockBits(secondData); }
        }
        finally { first.UnlockBits(firstData); }
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
