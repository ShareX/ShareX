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

using Avalonia;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DrawingPoint = System.Drawing.Point;
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

    public static ScreenPixelBuffer Capture(PixelRect bounds)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bounds));
        }

        using Bitmap bitmap = new(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
        }

        DrawingRectangle bitmapBounds = new(0, 0, bitmap.Width, bitmap.Height);
        BitmapData data = bitmap.LockBits(bitmapBounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        try
        {
            int[] pixels = new int[bitmap.Width * bitmap.Height];
            for (int y = 0; y < bitmap.Height; y++)
            {
                Marshal.Copy(IntPtr.Add(data.Scan0, y * data.Stride), pixels, y * bitmap.Width, bitmap.Width);
            }

            return new ScreenPixelBuffer(bounds, pixels);
        }
        finally
        {
            bitmap.UnlockBits(data);
        }
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

        int left = FindFirstContentColumn(selection, topLeftColor, tolerance);
        if (left < 0)
        {
            return selection;
        }

        int top = FindFirstContentRow(selection, topLeftColor, tolerance);
        int right = FindLastContentColumn(selection, bottomRightColor, tolerance);
        int bottom = FindLastContentRow(selection, bottomRightColor, tolerance);

        if (top < 0 || right < left || bottom < top)
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

    private int FindFirstContentColumn(DrawingRectangle selection, int background, int tolerance)
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

        return -1;
    }

    private int FindFirstContentRow(DrawingRectangle selection, int background, int tolerance)
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

        return -1;
    }

    private int FindLastContentColumn(DrawingRectangle selection, int background, int tolerance)
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

        return -1;
    }

    private int FindLastContentRow(DrawingRectangle selection, int background, int tolerance)
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

        return -1;
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
