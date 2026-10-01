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

namespace ShareX.HelpersLib
{
    /// <summary>
    /// Straight (not premultiplied) BGRA pixels, the layout GDI+ Format32bppArgb used, so per-pixel algorithms ported from the
    /// GDI+ code produce the same numbers. Each pixel is a uint 0xAARRGGBB.
    /// </summary>
    public sealed unsafe class SkiaPixels : IDisposable
    {
        private readonly SKBitmap bitmap;
        private readonly uint* pixels;

        private SkiaPixels(SKBitmap bitmap)
        {
            this.bitmap = bitmap;
            pixels = (uint*)bitmap.GetPixels();
            Width = bitmap.Width;
            Height = bitmap.Height;
        }

        public int Width { get; }

        public int Height { get; }

        public int PixelCount => Width * Height;

        /// <summary>A straight alpha copy of the bitmap.</summary>
        public static SkiaPixels From(SKBitmap source)
        {
            SKBitmap copy = new SKBitmap(new SKImageInfo(source.Width, source.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul));

            using (SKPixmap pixmap = source.PeekPixels())
            {
                pixmap.ReadPixels(copy.Info, copy.GetPixels(), copy.RowBytes, 0, 0);
            }

            return new SkiaPixels(copy);
        }

        /// <summary>A transparent buffer.</summary>
        public static SkiaPixels Create(int width, int height)
        {
            SKBitmap bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul));
            bitmap.Erase(SKColors.Transparent);
            return new SkiaPixels(bitmap);
        }

        public uint this[int x, int y]
        {
            get => pixels[(y * Width) + x];
            set => pixels[(y * Width) + x] = value;
        }

        public uint this[int index]
        {
            get => pixels[index];
            set => pixels[index] = value;
        }

        public static byte A(uint bgra) => (byte)(bgra >> 24);

        public static byte R(uint bgra) => (byte)(bgra >> 16);

        public static byte G(uint bgra) => (byte)(bgra >> 8);

        public static byte B(uint bgra) => (byte)bgra;

        public static uint Pack(byte b, byte g, byte r, byte a) => ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;

        /// <summary>A premultiplied bitmap, the format SkiaSharp draws with, holding the current pixels.</summary>
        public SKBitmap ToBitmap()
        {
            SKBitmap result = new SKBitmap(new SKImageInfo(Width, Height, SKColorType.Bgra8888, SKAlphaType.Premul));

            using (SKPixmap pixmap = bitmap.PeekPixels())
            {
                pixmap.ReadPixels(result.Info, result.GetPixels(), result.RowBytes, 0, 0);
            }

            return result;
        }

        public void Dispose() => bitmap.Dispose();
    }
}
