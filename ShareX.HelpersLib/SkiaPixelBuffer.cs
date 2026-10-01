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

using SkiaSharp;
using System;

namespace ShareX.HelpersLib;

public enum PixelAccess { ReadWrite, ReadOnly, WriteOnly }

/// <summary>Exposes tightly packed, straight-alpha BGRA pixels to the existing image algorithms.</summary>
public unsafe sealed class SkiaPixelBuffer : IDisposable
{
    private readonly SKBitmap bitmap;
    private SKBitmap pixels;
    private PixelAccess access;
    public ColorBgra* Pointer { get; private set; }
    public bool IsLocked => pixels != null;
    public int Width => bitmap.Width;
    public int Height => bitmap.Height;
    public int PixelCount => checked(Width * Height);

    public SkiaPixelBuffer(SKBitmap bitmap, bool lockBitmap = false, PixelAccess access = PixelAccess.ReadWrite)
    {
        this.bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
        if (lockBitmap) Lock(access);
    }

    public void Lock(PixelAccess access = PixelAccess.ReadWrite)
    {
        if (IsLocked) return;
        this.access = access;
        pixels = new SKBitmap(new SKImageInfo(Width, Height, SKColorType.Bgra8888, SKAlphaType.Unpremul));
        if (access != PixelAccess.WriteOnly)
        {
            using SKPixmap source = bitmap.PeekPixels();
            if (!source.ReadPixels(pixels.Info, pixels.GetPixels(), pixels.RowBytes))
            {
                pixels.Dispose();
                pixels = null;
                throw new InvalidOperationException("Unable to read image pixels.");
            }
        }
        else pixels.Erase(SKColors.Transparent);
        Pointer = (ColorBgra*)pixels.GetPixels();
    }

    public void Unlock()
    {
        if (!IsLocked) return;
        try
        {
            if (access != PixelAccess.ReadOnly)
            {
                using SKPixmap source = pixels.PeekPixels();
                if (!source.ReadPixels(bitmap.Info, bitmap.GetPixels(), bitmap.RowBytes))
                    throw new InvalidOperationException("Unable to write image pixels.");
                bitmap.NotifyPixelsChanged();
            }
        }
        finally
        {
            pixels.Dispose();
            pixels = null;
            Pointer = null;
        }
    }

    public ColorBgra GetPixel(int index) => Pointer[index];
    public ColorBgra GetPixel(int x, int y) => Pointer[x + y * Width];
    public void SetPixel(int index, ColorBgra color) => Pointer[index] = color;
    public void SetPixel(int index, uint color) => Pointer[index] = color;
    public void SetPixel(int x, int y, ColorBgra color) => Pointer[x + y * Width] = color;
    public void SetPixel(int x, int y, uint color) => Pointer[x + y * Width] = color;
    public void ClearPixel(int index) => Pointer[index] = 0;
    public void ClearPixel(int x, int y) => Pointer[x + y * Width] = 0;

    public bool IsTransparent()
    {
        for (int index = 0; index < PixelCount; index++) if (Pointer[index].Alpha < 255) return true;
        return false;
    }

    public static bool Compare(SkiaPixelBuffer first, SkiaPixelBuffer second)
    {
        if (first.Width != second.Width || first.Height != second.Height) return false;
        first.Lock(PixelAccess.ReadOnly);
        second.Lock(PixelAccess.ReadOnly);
        return new ReadOnlySpan<ColorBgra>(first.Pointer, first.PixelCount)
            .SequenceEqual(new ReadOnlySpan<ColorBgra>(second.Pointer, second.PixelCount));
    }

    public void Dispose() => Unlock();
}
