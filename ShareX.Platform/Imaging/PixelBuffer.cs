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

using System;

namespace ShareX.Platform.Imaging;

/// <summary>A 32 bit BGRA image with straight (not premultiplied) alpha, laid out top down.</summary>
/// <remarks>BGRA matches GDI DIBs, X11 ZPixmaps and CoreGraphics little endian bitmaps so captures can be copied without conversion.</remarks>
public sealed class PixelBuffer
{
    public PixelBuffer(int width, int height)
        : this(width, height, new byte[checked(width * height * 4)])
    {
    }

    public PixelBuffer(int width, int height, byte[] pixels)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        ArgumentNullException.ThrowIfNull(pixels);
        if (pixels.Length < width * height * 4) throw new ArgumentException("Pixel array is smaller than width * height * 4.", nameof(pixels));

        Width = width;
        Height = height;
        Pixels = pixels;
    }

    public int Width { get; }

    public int Height { get; }

    public int Stride => Width * 4;

    public byte[] Pixels { get; }

    /// <summary>Copies rows from a native buffer whose stride may include padding.</summary>
    public static unsafe PixelBuffer FromBgra(IntPtr source, int width, int height, int sourceStride, bool forceOpaque)
    {
        PixelBuffer buffer = new PixelBuffer(width, height);
        byte* src = (byte*)source;

        fixed (byte* dst = buffer.Pixels)
        {
            for (int y = 0; y < height; y++)
            {
                Buffer.MemoryCopy(src + (long)y * sourceStride, dst + (long)y * buffer.Stride, buffer.Stride, buffer.Stride);
            }
        }

        if (forceOpaque)
        {
            buffer.MakeOpaque();
        }

        return buffer;
    }

    public void MakeOpaque()
    {
        for (int i = 3; i < Pixels.Length; i += 4)
        {
            Pixels[i] = 255;
        }
    }

    public PixelBuffer Crop(PlatformRectangle area)
    {
        PlatformRectangle bounds = new PlatformRectangle(0, 0, Width, Height).Intersect(area);

        if (bounds.IsEmpty)
        {
            throw new ArgumentException("Crop area does not intersect the image.", nameof(area));
        }

        PixelBuffer result = new PixelBuffer(bounds.Width, bounds.Height);

        for (int y = 0; y < bounds.Height; y++)
        {
            Buffer.BlockCopy(Pixels, (bounds.Y + y) * Stride + bounds.X * 4, result.Pixels, y * result.Stride, result.Stride);
        }

        return result;
    }

    /// <summary>Draws <paramref name="source"/> at the given position, clipping to this buffer. Alpha is copied, not blended.</summary>
    public void CopyFrom(PixelBuffer source, int x, int y)
    {
        PlatformRectangle target = new PlatformRectangle(0, 0, Width, Height).Intersect(new PlatformRectangle(x, y, source.Width, source.Height));

        for (int row = 0; row < target.Height; row++)
        {
            int sourceOffset = (target.Y - y + row) * source.Stride + (target.X - x) * 4;
            int targetOffset = (target.Y + row) * Stride + target.X * 4;
            Buffer.BlockCopy(source.Pixels, sourceOffset, Pixels, targetOffset, target.Width * 4);
        }
    }

    /// <summary>Alpha blends <paramref name="overlay"/> over this image ("source over"), for example a cursor image, at the given position.</summary>
    public void BlendFrom(PixelBuffer overlay, int x, int y)
    {
        PlatformRectangle target = new PlatformRectangle(0, 0, Width, Height).Intersect(new PlatformRectangle(x, y, overlay.Width, overlay.Height));

        for (int row = 0; row < target.Height; row++)
        {
            for (int column = 0; column < target.Width; column++)
            {
                int s = (target.Y - y + row) * overlay.Stride + (target.X - x + column) * 4;
                int d = (target.Y + row) * Stride + (target.X + column) * 4;
                int sourceAlpha = overlay.Pixels[s + 3];

                if (sourceAlpha == 0) continue;

                // Straight alpha: weight each colour by its own alpha, then divide by the combined alpha.
                int destinationWeight = Pixels[d + 3] * (255 - sourceAlpha);
                int outputAlpha = sourceAlpha * 255 + destinationWeight;

                for (int channel = 0; channel < 3; channel++)
                {
                    Pixels[d + channel] = (byte)((overlay.Pixels[s + channel] * sourceAlpha * 255 + Pixels[d + channel] * destinationWeight + outputAlpha / 2) / outputAlpha);
                }

                Pixels[d + 3] = (byte)((outputAlpha + 127) / 255);
            }
        }
    }
}
