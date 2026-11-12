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

#nullable enable

using Avalonia;
using Avalonia.Platform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace ShareX.HelpersLib;

/// <summary>
/// Decodes an animated image (GIF, WebP) one frame at a time for the main window's preview pane, its thumbnails and the image viewer.
/// Frames are decoded on demand rather than all at once, so long animations stay light on memory.
/// </summary>
public sealed class AnimatedImagePreview : IDisposable
{
    // Browsers treat frame delays this short as unset and play them at 100 ms, and so do we.
    private const int MinFrameDuration = 20;
    private const int DefaultFrameDuration = 100;

    private readonly SKData data;
    private readonly SKCodec codec;
    private readonly SKCodecFrameInfo[] frames;
    private readonly SKBitmap composed;
    private readonly SKBitmap? scaled;
    private int decodedFrame = -1;

    private AnimatedImagePreview(SKData data, SKCodec codec, int maxSize)
    {
        this.data = data;
        this.codec = codec;
        frames = codec.FrameInfo;
        composed = SkiaImageHelpers.CreateBitmap(codec.Info.Width, codec.Info.Height);

        double scale = Math.Min(1, (double)maxSize / Math.Max(codec.Info.Width, codec.Info.Height));

        if (scale < 1)
        {
            scaled = SkiaImageHelpers.CreateBitmap(Math.Max(1, (int)Math.Round(codec.Info.Width * scale)),
                Math.Max(1, (int)Math.Round(codec.Info.Height * scale)));
        }
    }

    /// <summary>Whether the file is in a format that can be animated, checked by extension so no file is read.</summary>
    public static bool CanBeAnimated(string filePath) => FileHelpers.CheckExtension(filePath, ["gif", "webp"]);

    /// <summary>Returns null when the file is not an animated image with more than one frame.</summary>
    public static AnimatedImagePreview? TryCreate(string filePath, int maxSize)
    {
        SKData? data = null;
        SKCodec? codec = null;

        try
        {
            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                data = SKData.Create(stream);
            }

            codec = data == null ? null : SKCodec.Create(data);

            if (codec != null && codec.FrameCount > 1 && codec.FrameInfo.Length == codec.FrameCount)
            {
                return new AnimatedImagePreview(data!, codec, maxSize);
            }
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e);
        }

        codec?.Dispose();
        data?.Dispose();
        return null;
    }

    /// <summary>
    /// Plays an animated image by passing each frame to <paramref name="showFrame"/>, which takes ownership of it,
    /// until <paramref name="isCurrent"/> returns false. Returns false without showing anything when the file is not
    /// animated, so the caller can show a still image instead. Call it on the UI thread.
    /// </summary>
    public static async Task<bool> PlayAsync(string filePath, int maxSize, Func<bool> isCurrent, Action<AvaloniaBitmap> showFrame,
        Func<bool>? isPaused = null)
    {
        AnimatedImagePreview? animation = await Task.Run(() => TryCreate(filePath, maxSize));

        if (animation == null)
        {
            return false;
        }

        using (animation)
        {
            Stopwatch clock = Stopwatch.StartNew();
            long nextFrameTime = 0;

            while (isCurrent())
            {
                if (isPaused?.Invoke() == true)
                {
                    await Task.Delay(250);
                    nextFrameTime = clock.ElapsedMilliseconds;
                    continue;
                }

                (AvaloniaBitmap Frame, int Duration) next;

                try
                {
                    next = await Task.Run(animation.NextFrame);
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                    break;
                }

                if (!isCurrent())
                {
                    next.Frame.Dispose();
                    break;
                }

                showFrame(next.Frame);

                nextFrameTime = Math.Max(nextFrameTime + next.Duration, clock.ElapsedMilliseconds);
                await Task.Delay(TimeSpan.FromMilliseconds(Math.Max(1, nextFrameTime - clock.ElapsedMilliseconds)));
            }
        }

        return true;
    }

    /// <summary>Decodes the next frame (looping) and returns it with how long it should be shown, in milliseconds.</summary>
    public (AvaloniaBitmap Frame, int Duration) NextFrame()
    {
        int index = (decodedFrame + 1) % frames.Length;
        DecodeFrame(index);

        SKBitmap output = composed;

        if (scaled != null)
        {
            using SKCanvas canvas = new(scaled);
            using SKImage image = SKImage.FromBitmap(composed);
            canvas.Clear(SKColors.Transparent);
            canvas.DrawImage(image, new SKRect(0, 0, scaled.Width, scaled.Height), SkiaImageHelpers.HighQualitySampling);
            output = scaled;
        }

        AvaloniaBitmap frame = new(PixelFormat.Bgra8888, AlphaFormat.Premul, output.GetPixels(),
            new PixelSize(output.Width, output.Height), new Vector(96, 96), output.RowBytes);

        int duration = frames[index].Duration;
        return (frame, duration < MinFrameDuration ? DefaultFrameDuration : duration);
    }

    private void DecodeFrame(int index)
    {
        int required = frames[index].RequiredFrame;

        if (required == -1)
        {
            // An independent frame: start from a clear canvas.
            composed.Erase(SKColors.Transparent);
            Decode(index, -1);
        }
        else if (required == decodedFrame)
        {
            // The usual case: the frame draws on top of the one already in the canvas.
            Decode(index, decodedFrame);
        }
        else
        {
            // The frame builds on an earlier frame than the one in the canvas (a "restore previous" disposal),
            // so rebuild the canvas from the frames it depends on.
            Stack<int> chain = new();

            for (int frame = index; frame >= 0; frame = frames[frame].RequiredFrame)
            {
                chain.Push(frame);
            }

            composed.Erase(SKColors.Transparent);
            int prior = -1;

            while (chain.Count > 0)
            {
                int frame = chain.Pop();
                Decode(frame, prior);
                prior = frame;
            }
        }

        decodedFrame = index;
    }

    private void Decode(int index, int priorFrame)
    {
        SKCodecResult result = codec.GetPixels(composed.Info, composed.GetPixels(), new SKCodecOptions(index, priorFrame));

        if (result is not SKCodecResult.Success and not SKCodecResult.IncompleteInput)
        {
            throw new InvalidDataException($"Frame {index} decoding failed: {result}.");
        }
    }

    public void Dispose()
    {
        scaled?.Dispose();
        composed.Dispose();
        codec.Dispose();
        data.Dispose();
    }
}
