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

namespace ShareX.PhotoEditor.Core;

/// <summary>
/// Owns a bitmap that can be shared between the live document and history states.
/// Shared surfaces are frozen and must be copied before they are written to.
/// </summary>
public sealed class PixelSurface : IDisposable
{
    public PixelSurface(SKBitmap bitmap)
    {
        Bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
    }

    public SKBitmap Bitmap { get; private set; }

    public bool Frozen { get; private set; }

    public bool IsDisposed { get; private set; }

    public int Width => Bitmap.Width;

    public int Height => Bitmap.Height;

    public bool IsMask => Bitmap.ColorType == SKColorType.Alpha8;

    public SKRectI Bounds => new(0, 0, Width, Height);

    public int RowBytes => Bitmap.RowBytes;

    public static PixelSurface CreateColor(int width, int height, SKColor? fill = null)
    {
        SKBitmap bitmap = new(new SKImageInfo(Math.Max(1, width), Math.Max(1, height), SKColorType.Bgra8888, SKAlphaType.Premul));
        bitmap.Erase(fill ?? SKColors.Transparent);
        return new PixelSurface(bitmap);
    }

    public static PixelSurface CreateMask(int width, int height, byte fill = 0)
    {
        SKBitmap bitmap = new(new SKImageInfo(Math.Max(1, width), Math.Max(1, height), SKColorType.Alpha8, SKAlphaType.Premul));
        PixelSurface surface = new(bitmap);
        surface.GetSpan().Fill(fill);
        return surface;
    }

    /// <summary>Copies any bitmap into a premultiplied BGRA surface.</summary>
    public static PixelSurface FromBitmap(SKBitmap source)
    {
        SKBitmap copy = new(new SKImageInfo(source.Width, source.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
        using (SKCanvas canvas = new(copy))
        {
            canvas.Clear(SKColors.Transparent);
            canvas.DrawBitmap(source, 0, 0);
        }
        return new PixelSurface(copy);
    }

    public void Freeze() => Frozen = true;

    public PixelSurface Clone()
    {
        SKBitmap copy = new(Bitmap.Info);
        unsafe
        {
            Buffer.MemoryCopy((void*)Bitmap.GetPixels(), (void*)copy.GetPixels(), (long)copy.RowBytes * copy.Height, (long)Bitmap.RowBytes * Bitmap.Height);
        }
        return new PixelSurface(copy);
    }

    public unsafe Span<byte> GetSpan() => new((void*)Bitmap.GetPixels(), Bitmap.RowBytes * Bitmap.Height);

    public Span<uint> GetPixelSpan() => MemoryMarshal.Cast<byte, uint>(GetSpan());

    public void Dispose()
    {
        if (IsDisposed) return;
        IsDisposed = true;
        Bitmap.Dispose();
    }
}
