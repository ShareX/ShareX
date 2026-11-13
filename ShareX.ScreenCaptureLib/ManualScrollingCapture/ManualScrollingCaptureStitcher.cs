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

using ShareX.HelpersLib;
using System;
using System.Drawing;
using Bitmap = SkiaSharp.SKBitmap;

namespace ShareX.ScreenCaptureLib;

public enum ManualScrollingFrameResult
{
    /// <summary>The frame continued the image and new rows were appended.</summary>
    Appended,
    /// <summary>The frame shows nothing below what was already captured, for example while the user pauses or scrolls up.</summary>
    NoNewContent,
    /// <summary>The frame shares no rows with the end of the image, usually because the user scrolled too far at once.</summary>
    NoOverlap
}

/// <summary>
/// Stitches frames of content that the user scrolls by hand. Unlike automatic scrolling capture, the scroll distance
/// differs between frames, so each frame is placed only where it truly overlaps the image and is skipped otherwise.
/// </summary>
public sealed class ManualScrollingCaptureStitcher : IDisposable
{
    /// <summary>Keeps very long captures within a size that SkiaSharp and image viewers handle.</summary>
    public const int MaxHeight = 60000;

    private int ignoreBottomOffset;

    public Bitmap? Result { get; private set; }

    public int Height => Result?.Height ?? 0;

    public ManualScrollingFrameResult AddFrame(Bitmap frame)
    {
        if (Result == null)
        {
            Result = frame.Copy();
            return ManualScrollingFrameResult.Appended;
        }

        if (frame.Width != Result.Width || frame.Info.ColorType != Result.Info.ColorType || frame.BytesPerPixel != 4)
        {
            return ManualScrollingFrameResult.NoOverlap;
        }

        Bitmap? combined = Combine(Result, frame, out ManualScrollingFrameResult result);
        if (combined != null)
        {
            Result.Dispose();
            Result = combined;
        }

        return result;
    }

    /// <summary>Marks rows that are one solid color across the compared width.</summary>
    private static unsafe bool[] FindBlankRows(IntPtr scan0, int height, int stride, int width)
    {
        bool[] blank = new bool[height];

        for (int y = 0; y < height; y++)
        {
            ReadOnlySpan<uint> row = new((byte*)scan0 + y * stride, width);
            blank[y] = !row.ContainsAnyExcept(row[0]);
        }

        return blank;
    }

    private static unsafe int CompareRows(IntPtr first, IntPtr second, int count)
        => new ReadOnlySpan<byte>(first.ToPointer(), count).SequenceEqual(new ReadOnlySpan<byte>(second.ToPointer(), count)) ? 0 : 1;

    /// <summary>
    /// Finds the frame row where the image's last rows reappear, and appends the frame's rows below it. Rows that stay
    /// the same at the bottom of every frame, such as a status bar, are left out of the matching and kept only once.
    /// </summary>
    private unsafe Bitmap? Combine(Bitmap result, Bitmap frame, out ManualScrollingFrameResult frameResult)
    {
        int matchLimit = frame.Height / 2;
        int ignoreSideOffset = Math.Min(Math.Max(50, frame.Width / 20), frame.Width / 3);
        Rectangle rect = new(ignoreSideOffset, result.Height - frame.Height, frame.Width - ignoreSideOffset * 2, frame.Height);

        using SkiaPixelBuffer resultPixels = new(result, true, PixelAccess.ReadOnly);
        using SkiaPixelBuffer framePixels = new(frame, true, PixelAccess.ReadOnly);
        int stride = result.Width * 4;
        IntPtr resultScan0 = (IntPtr)resultPixels.Pointer + 4 * ignoreSideOffset;
        IntPtr frameScan0 = (IntPtr)framePixels.Pointer + 4 * ignoreSideOffset;
        int compareLength = 4 * rect.Width;

        int ignoreBottomOffsetMax = frame.Height / 3;
        // Besides the rows that stay the same, a small margin above them is skipped, since the content at a scrolling
        // edge can still be rendering. It is smaller than in automatic scrolling capture, because a hand scroll can
        // move far between frames and every overlapping row helps to find the position.
        int bottomOffset = Math.Max(16, frame.Height / 30);
        IntPtr resultLastRow = resultScan0 + (result.Height - 1) * stride;
        IntPtr frameLastRow = frameScan0 + (frame.Height - 1) * stride;

        for (int i = 0; i <= ignoreBottomOffsetMax; i++)
        {
            if (CompareRows(resultLastRow - i * stride, frameLastRow - i * stride, compareLength) != 0)
            {
                bottomOffset += i;
                break;
            }
        }

        ignoreBottomOffset = Math.Min(Math.Max(bottomOffset, ignoreBottomOffset), ignoreBottomOffsetMax);
        int rectBottom = rect.Bottom - ignoreBottomOffset - 1;

        // Matching walks up from the bottom of the content. A blinking text cursor, a hover highlight or a small
        // animation changes a short run of rows between frames, so a short gap of mismatched rows is allowed. A long
        // gap ends the walk, for example where the content meets a toolbar that does not scroll. Blank rows match at
        // almost any position, so only rows with content count as evidence for a position.
        int maxGap = Math.Max(24, frame.Height / 20);
        bool[] blankRows = FindBlankRows(frameScan0, frame.Height, stride, rect.Width);
        int matchCount = 0;
        int matchIndex = 0;

        for (int frameY = frame.Height - 1; frameY >= 0; frameY--)
        {
            int compared = Math.Min(Math.Min(frameY, rectBottom) + 1, matchLimit);
            int matches = 0, mismatches = 0, gap = 0;

            for (int y = 0; y < compared; y++)
            {
                if (CompareRows(resultScan0 + (rectBottom - y) * stride, frameScan0 + (frameY - y) * stride, compareLength) == 0)
                {
                    if (!blankRows[frameY - y]) matches++;
                    gap = 0;
                }
                else
                {
                    mismatches++;
                    gap++;
                    if (gap > maxGap || mismatches > Math.Max(maxGap, matches / 4)) break;
                }
            }

            // Mismatches after the last matching row only mark where the walk stopped.
            mismatches -= gap;

            if (matches > matchCount && mismatches <= Math.Max(maxGap, matches / 4))
            {
                matchCount = matches;
                matchIndex = frameY;
                if (mismatches == 0 && matchCount == matchLimit) break;
            }
        }

        // A few matching rows can be chance, for example blank lines; require enough to be sure of the position.
        if (matchCount < Math.Min(16, matchLimit))
        {
            frameResult = ManualScrollingFrameResult.NoOverlap;
            return null;
        }

        // The rows below the match, up to the ignored bottom edge, are new. The bottom edge is added back after them.
        int newHeight = frame.Height - ignoreBottomOffset - matchIndex - 1;
        if (newHeight <= 0)
        {
            frameResult = ManualScrollingFrameResult.NoNewContent;
            return null;
        }

        int keptHeight = result.Height - ignoreBottomOffset;
        int appendedHeight = Math.Min(frame.Height - matchIndex - 1, MaxHeight - keptHeight);
        if (appendedHeight <= 0)
        {
            frameResult = ManualScrollingFrameResult.NoNewContent;
            return null;
        }

        // Rows are copied byte for byte. Drawing would resample them, and the next frame must match them exactly.
        Bitmap combined = new(result.Info.WithSize(result.Width, keptHeight + appendedHeight));
        CopyRows(result, 0, combined, 0, keptHeight);
        CopyRows(frame, matchIndex + 1, combined, keptHeight, appendedHeight);

        frameResult = ManualScrollingFrameResult.Appended;
        return combined;
    }

    private static unsafe void CopyRows(Bitmap source, int sourceY, Bitmap destination, int destinationY, int rows)
    {
        int rowLength = source.Width * source.BytesPerPixel;
        byte* sourcePixels = (byte*)source.GetPixels() + sourceY * source.RowBytes;
        byte* destinationPixels = (byte*)destination.GetPixels() + destinationY * destination.RowBytes;

        for (int i = 0; i < rows; i++)
        {
            Buffer.MemoryCopy(sourcePixels + i * source.RowBytes, destinationPixels + i * destination.RowBytes, destination.RowBytes, rowLength);
        }
    }

    /// <summary>Hands over the stitched image; the stitcher no longer owns it.</summary>
    public Bitmap? TakeResult()
    {
        Bitmap? result = Result;
        Result = null;
        return result;
    }

    public void Dispose()
    {
        Result?.Dispose();
        Result = null;
    }
}
