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

using System.Runtime.InteropServices;

namespace ShareX.PixelEditor.Core;

/// <summary>
/// A rectangle of straight ARGB pixels. Buffers captured by the history are frozen: they are shared between
/// undo states, so a frozen buffer is cloned before it is written to.
/// </summary>
public sealed class PixelBuffer
{
    public PixelBuffer(int width, int height)
    {
        Width = Math.Max(1, width);
        Height = Math.Max(1, height);
        Pixels = new uint[Width * Height];
    }

    public PixelBuffer(int width, int height, uint[] pixels)
    {
        Width = width;
        Height = height;
        Pixels = pixels;
    }

    public int Width { get; }

    public int Height { get; }

    public uint[] Pixels { get; }

    /// <summary>True once an undo state references this buffer; it must not be modified any more.</summary>
    public bool Frozen { get; set; }

    public SKRectI Bounds => new(0, 0, Width, Height);

    public long ByteSize => (long)Width * Height * 4;

    public bool Contains(int x, int y) => (uint)x < (uint)Width && (uint)y < (uint)Height;

    public uint this[int x, int y]
    {
        get => Pixels[y * Width + x];
        set => Pixels[y * Width + x] = value;
    }

    public uint GetOrDefault(int x, int y) => Contains(x, y) ? Pixels[y * Width + x] : 0;

    public PixelBuffer Clone() => new(Width, Height, (uint[])Pixels.Clone());

    public void Clear() => Array.Clear(Pixels);

    public void Fill(uint color) => Array.Fill(Pixels, color);

    public SKRectI Clip(SKRectI area)
    {
        int l = Math.Max(0, area.Left), t = Math.Max(0, area.Top), r = Math.Min(Width, area.Right), b = Math.Min(Height, area.Bottom);
        return r > l && b > t ? new SKRectI(l, t, r, b) : SKRectI.Empty;
    }

    /// <summary>Copies an area from a buffer of the same size.</summary>
    public void CopyFrom(PixelBuffer source, SKRectI area)
    {
        area = Clip(area);
        if (area.IsEmpty) return;
        for (int y = area.Top; y < area.Bottom; y++)
        {
            Array.Copy(source.Pixels, y * Width + area.Left, Pixels, y * Width + area.Left, area.Width);
        }
    }

    public bool IsEmpty
    {
        get
        {
            foreach (uint p in Pixels)
                if (p >> 24 != 0) return false;
            return true;
        }
    }

    /// <summary>The smallest rectangle holding every non-transparent pixel, or null when the buffer is empty.</summary>
    public SKRectI? ContentBounds()
    {
        int minX = Width, minY = Height, maxX = -1, maxY = -1;
        for (int y = 0; y < Height; y++)
        {
            int row = y * Width;
            for (int x = 0; x < Width; x++)
            {
                if (Pixels[row + x] >> 24 == 0) continue;
                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                maxY = y;
            }
        }
        return maxX < 0 ? null : new SKRectI(minX, minY, maxX + 1, maxY + 1);
    }

    /// <summary>Copies an area into a new buffer; parts outside this buffer stay transparent.</summary>
    public PixelBuffer Crop(SKRectI area)
    {
        PixelBuffer result = new(area.Width, area.Height);
        result.Blit(this, -area.Left, -area.Top, false);
        return result;
    }

    /// <summary>Draws another buffer at an offset, either replacing pixels or compositing over them.</summary>
    public void Blit(PixelBuffer source, int dx, int dy, bool blend, int opacity = 255)
    {
        int x0 = Math.Max(0, dx), y0 = Math.Max(0, dy), x1 = Math.Min(Width, dx + source.Width), y1 = Math.Min(Height, dy + source.Height);
        for (int y = y0; y < y1; y++)
        {
            int srcRow = (y - dy) * source.Width - dx, dstRow = y * Width;
            if (!blend)
            {
                Array.Copy(source.Pixels, srcRow + x0, Pixels, dstRow + x0, x1 - x0);
                continue;
            }
            for (int x = x0; x < x1; x++)
            {
                uint s = source.Pixels[srcRow + x];
                if (s >> 24 != 0) Pixels[dstRow + x] = Argb.Over(Pixels[dstRow + x], s, opacity);
            }
        }
    }

    public bool ContentEquals(PixelBuffer other) => other.Width == Width && other.Height == Height && Pixels.AsSpan().SequenceEqual(other.Pixels);

    /// <summary>An unpremultiplied BGRA bitmap with a copy of the pixels, for encoding and the clipboard.</summary>
    public SKBitmap ToBitmap()
    {
        SKBitmap bitmap = new(new SKImageInfo(Width, Height, SKColorType.Bgra8888, SKAlphaType.Unpremul));
        Marshal.Copy(MemoryMarshal.AsBytes(Pixels.AsSpan()).ToArray(), 0, bitmap.GetPixels(), Width * Height * 4);
        return bitmap;
    }

    public static PixelBuffer FromBitmap(SKBitmap bitmap)
    {
        PixelBuffer buffer = new(bitmap.Width, bitmap.Height);
        SKImageInfo info = new(bitmap.Width, bitmap.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        GCHandle handle = GCHandle.Alloc(buffer.Pixels, GCHandleType.Pinned);
        try
        {
            using SKPixmap? pixmap = bitmap.PeekPixels();
            if (pixmap == null || !pixmap.ReadPixels(info, handle.AddrOfPinnedObject(), info.RowBytes))
            {
                SKColor[] colors = bitmap.Pixels;
                for (int i = 0; i < colors.Length && i < buffer.Pixels.Length; i++) buffer.Pixels[i] = (uint)colors[i];
            }
        }
        finally
        {
            handle.Free();
        }
        // Fully transparent pixels carry no color, so equal buffers compare equal.
        for (int i = 0; i < buffer.Pixels.Length; i++)
            if (buffer.Pixels[i] >> 24 == 0) buffer.Pixels[i] = 0;
        return buffer;
    }

    /// <summary>Scales with nearest neighbor sampling, the only resampling that keeps pixel art crisp.</summary>
    public PixelBuffer ScaleNearest(int width, int height)
    {
        PixelBuffer result = new(width, height);
        for (int y = 0; y < result.Height; y++)
        {
            int sy = Math.Min(Height - 1, (int)((y + 0.5) * Height / result.Height));
            for (int x = 0; x < result.Width; x++)
            {
                int sx = Math.Min(Width - 1, (int)((x + 0.5) * Width / result.Width));
                result.Pixels[y * result.Width + x] = Pixels[sy * Width + sx];
            }
        }
        return result;
    }

    public PixelBuffer FlipHorizontal()
    {
        PixelBuffer result = new(Width, Height);
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                result.Pixels[y * Width + (Width - 1 - x)] = Pixels[y * Width + x];
        return result;
    }

    public PixelBuffer FlipVertical()
    {
        PixelBuffer result = new(Width, Height);
        for (int y = 0; y < Height; y++) Array.Copy(Pixels, y * Width, result.Pixels, (Height - 1 - y) * Width, Width);
        return result;
    }

    /// <summary>Rotates by a multiple of 90 degrees clockwise.</summary>
    public PixelBuffer Rotate90(int quarterTurns)
    {
        quarterTurns = ((quarterTurns % 4) + 4) % 4;
        if (quarterTurns == 0) return Clone();
        bool swap = quarterTurns != 2;
        PixelBuffer result = new(swap ? Height : Width, swap ? Width : Height);
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                (int nx, int ny) = quarterTurns switch
                {
                    1 => (Height - 1 - y, x),
                    2 => (Width - 1 - x, Height - 1 - y),
                    _ => (y, Width - 1 - x)
                };
                result.Pixels[ny * result.Width + nx] = Pixels[y * Width + x];
            }
        }
        return result;
    }
}
