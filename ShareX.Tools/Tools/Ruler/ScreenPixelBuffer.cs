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

using Avalonia;
using ShareX.Platform;
using ShareX.Platform.Imaging;
using SkiaSharp;

using System.Runtime.InteropServices;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX.Tools.Ruler;

internal sealed class ScreenPixelBuffer
{
    private readonly int[] _pixels;

    public PixelRect Bounds { get; }

    private ScreenPixelBuffer(PixelRect bounds, int[] pixels)
    {
        Bounds = bounds;
        _pixels = pixels;
    }

    public static async Task<ScreenPixelBuffer> CaptureAsync(IScreenCaptureService capture, PixelRect bounds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(capture);
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bounds));
        }

        if (!capture.Support.IsSupported)
        {
            throw new PlatformNotSupportedException(capture.Support.Reason);
        }

        cancellationToken.ThrowIfCancellationRequested();
        ScreenCaptureResult result = await capture.CaptureAsync(ScreenCaptureRequest.ForRegion(
            new PlatformRectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height)), cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        return FromCapture(result);
    }

    internal static ScreenPixelBuffer FromCapture(ScreenCaptureResult capture)
    {
        ArgumentNullException.ThrowIfNull(capture);
        PlatformRectangle area = capture.Bounds;
        if (area.IsEmpty)
        {
            throw new ArgumentException("The captured screen bounds are empty.", nameof(capture));
        }

        PixelBuffer image;
        if (capture.Pixels is PixelBuffer pixels)
        {
            image = pixels;
        }
        else
        {
            using SKBitmap bitmap = SKBitmap.Decode(capture.Png)
                ?? throw new InvalidOperationException("The captured image could not be decoded.");
            using SKBitmap bgra = new(new SKImageInfo(bitmap.Width, bitmap.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul));
            using SKPixmap source = bitmap.PeekPixels();
            if (!source.ReadPixels(bgra.Info, bgra.GetPixels(), bgra.RowBytes))
            {
                throw new InvalidOperationException("The captured image pixels could not be read.");
            }

            image = new PixelBuffer(bgra.Width, bgra.Height);
            for (int y = 0; y < bgra.Height; y++)
            {
                Marshal.Copy(IntPtr.Add(bgra.GetPixels(), y * bgra.RowBytes), image.Pixels, y * image.Stride, image.Stride);
            }
        }

        int[] samples = new int[checked(area.Width * area.Height)];
        if (image.Width == area.Width && image.Height == area.Height)
        {
            Buffer.BlockCopy(image.Pixels, 0, samples, 0, checked(samples.Length * 4));
        }
        else
        {
            // Some compositors return a scaled image. Keep sampling in the desktop coordinate system,
            // without interpolating colours across the edges the ruler needs to detect.
            ReadOnlySpan<int> source = MemoryMarshal.Cast<byte, int>(image.Pixels);
            for (int y = 0; y < area.Height; y++)
            {
                int sourceRow = (int)((long)y * image.Height / area.Height) * image.Width;
                for (int x = 0; x < area.Width; x++)
                {
                    samples[y * area.Width + x] = source[sourceRow + (int)((long)x * image.Width / area.Width)];
                }
            }
        }

        return new ScreenPixelBuffer(new PixelRect(area.X, area.Y, area.Width, area.Height), samples);
    }

    public DrawingRectangle FindColorRun(PixelPoint point, bool horizontal, int tolerance)
    {
        if (!Bounds.Contains(point))
        {
            return default;
        }

        int reference = GetPixel(point.X, point.Y);

        if (horizontal)
        {
            int left = point.X;
            while (left > Bounds.X && ColorsAreClose(reference, GetPixel(left - 1, point.Y), tolerance))
            {
                left--;
            }

            int right = point.X + 1;
            while (right < Bounds.Right && ColorsAreClose(reference, GetPixel(right, point.Y), tolerance))
            {
                right++;
            }

            return new DrawingRectangle(left, point.Y, right - left, 1);
        }

        int top = point.Y;
        while (top > Bounds.Y && ColorsAreClose(reference, GetPixel(point.X, top - 1), tolerance))
        {
            top--;
        }

        int bottom = point.Y + 1;
        while (bottom < Bounds.Bottom && ColorsAreClose(reference, GetPixel(point.X, bottom), tolerance))
        {
            bottom++;
        }

        return new DrawingRectangle(point.X, top, 1, bottom - top);
    }

    public DrawingRectangle FindContentBounds(DrawingRectangle selection, int tolerance)
    {
        selection = Clamp(selection);
        if (selection.Width <= 1 || selection.Height <= 1)
        {
            return selection;
        }

        int topLeftColor = GetPixel(selection.Left, selection.Top);
        int bottomRightColor = GetPixel(selection.Right - 1, selection.Bottom - 1);

        if (FindFirstContentColumn(selection, topLeftColor, tolerance) is not int left)
        {
            return selection;
        }

        if (FindFirstContentRow(selection, topLeftColor, tolerance) is not int top ||
            FindLastContentColumn(selection, bottomRightColor, tolerance) is not int right ||
            FindLastContentRow(selection, bottomRightColor, tolerance) is not int bottom ||
            right < left || bottom < top)
        {
            return selection;
        }

        return DrawingRectangle.FromLTRB(left, top, right + 1, bottom + 1);
    }

    public DrawingRectangle Clamp(DrawingRectangle rectangle)
    {
        int left = Math.Clamp(rectangle.Left, Bounds.X, Bounds.Right);
        int top = Math.Clamp(rectangle.Top, Bounds.Y, Bounds.Bottom);
        int right = Math.Clamp(rectangle.Right, Bounds.X, Bounds.Right);
        int bottom = Math.Clamp(rectangle.Bottom, Bounds.Y, Bounds.Bottom);
        return DrawingRectangle.FromLTRB(Math.Min(left, right), Math.Min(top, bottom),
            Math.Max(left, right), Math.Max(top, bottom));
    }

    public DrawingRectangle ClampPreservingSize(DrawingRectangle rectangle)
    {
        int width = Math.Min(rectangle.Width, Bounds.Width);
        int height = Math.Min(rectangle.Height, Bounds.Height);
        int x = Math.Clamp(rectangle.X, Bounds.X, Bounds.Right - width);
        int y = Math.Clamp(rectangle.Y, Bounds.Y, Bounds.Bottom - height);
        return new DrawingRectangle(x, y, width, height);
    }

    public PixelPoint Clamp(PixelPoint point) => new(
        Math.Clamp(point.X, Bounds.X, Bounds.Right - 1),
        Math.Clamp(point.Y, Bounds.Y, Bounds.Bottom - 1));

    private int? FindFirstContentColumn(DrawingRectangle selection, int background, int tolerance)
    {
        for (int x = selection.Left; x < selection.Right; x++)
        {
            for (int y = selection.Top; y < selection.Bottom; y++)
            {
                if (!ColorsAreClose(background, GetPixel(x, y), tolerance))
                {
                    return x;
                }
            }
        }

        return null;
    }

    private int? FindFirstContentRow(DrawingRectangle selection, int background, int tolerance)
    {
        for (int y = selection.Top; y < selection.Bottom; y++)
        {
            for (int x = selection.Left; x < selection.Right; x++)
            {
                if (!ColorsAreClose(background, GetPixel(x, y), tolerance))
                {
                    return y;
                }
            }
        }

        return null;
    }

    private int? FindLastContentColumn(DrawingRectangle selection, int background, int tolerance)
    {
        for (int x = selection.Right - 1; x >= selection.Left; x--)
        {
            for (int y = selection.Top; y < selection.Bottom; y++)
            {
                if (!ColorsAreClose(background, GetPixel(x, y), tolerance))
                {
                    return x;
                }
            }
        }

        return null;
    }

    private int? FindLastContentRow(DrawingRectangle selection, int background, int tolerance)
    {
        for (int y = selection.Bottom - 1; y >= selection.Top; y--)
        {
            for (int x = selection.Left; x < selection.Right; x++)
            {
                if (!ColorsAreClose(background, GetPixel(x, y), tolerance))
                {
                    return y;
                }
            }
        }

        return null;
    }

    private int GetPixel(int screenX, int screenY)
    {
        int x = screenX - Bounds.X;
        int y = screenY - Bounds.Y;
        return _pixels[x + y * Bounds.Width];
    }

    private static bool ColorsAreClose(int first, int second, int tolerance)
    {
        int red = Math.Abs(((first >> 16) & 0xFF) - ((second >> 16) & 0xFF));
        int green = Math.Abs(((first >> 8) & 0xFF) - ((second >> 8) & 0xFF));
        int blue = Math.Abs((first & 0xFF) - (second & 0xFF));
        return red + green + blue <= tolerance;
    }
}
