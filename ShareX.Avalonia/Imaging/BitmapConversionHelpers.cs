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
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace ShareX.AvaloniaUI.Imaging
{
    /// <summary>
    /// Helper class for converting between Avalonia Bitmap and SKBitmap
    /// </summary>
    /// <summary>
    /// Helper class for converting between Avalonia Bitmap and SKBitmap
    /// </summary>
    public static class BitmapConversionHelpers
    {
        /// <summary>Creates a preview that fits the requested pixel size without enlarging the source.</summary>
        public static Bitmap CreatePreview(SKBitmap source, PixelSize maximumSize)
        {
            ArgumentNullException.ThrowIfNull(source);
            if (maximumSize.Width <= 0 || maximumSize.Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumSize));

            double scale = Math.Min(1, Math.Min((double)maximumSize.Width / source.Width,
                (double)maximumSize.Height / source.Height));
            int width = Math.Max(1, (int)Math.Round(source.Width * scale));
            int height = Math.Max(1, (int)Math.Round(source.Height * scale));
            if (width == source.Width && height == source.Height)
                return ToAvaloniBitmap(source);

            // Resize the pixels before transferring them to Avalonia. Encoding a full-size
            // PNG for a small preview makes large captures slow and blocks notification UI.
            using SKBitmap resized = source.Resize(
                new SKImageInfo(width, height, SKColorType.Bgra8888,
                    source.AlphaType == SKAlphaType.Opaque ? SKAlphaType.Opaque : SKAlphaType.Premul),
                new SKSamplingOptions(SKCubicResampler.Mitchell)) ??
                throw new InvalidOperationException("Unable to resize the image preview.");
            return ToAvaloniBitmap(resized);
        }

        /// <summary>
        /// Convert Avalonia Bitmap to SKBitmap.
        /// Warning: This is expensive if the input is not a WriteableBitmap.
        /// </summary>
        public static SKBitmap ToSKBitmap(Bitmap avaloniaBitmap)
        {
            if (avaloniaBitmap == null)
                throw new ArgumentNullException(nameof(avaloniaBitmap));

            // Optimized path for WriteableBitmap
            if (avaloniaBitmap is WriteableBitmap writeableBitmap)
            {
                using (var locked = writeableBitmap.Lock())
                {
                    var pixelSize = writeableBitmap.PixelSize;
                    var info = new SKImageInfo(
                        pixelSize.Width,
                        pixelSize.Height,
                        SKColorType.Bgra8888, // Avalonia usually uses BGRA
                        SKAlphaType.Premul);

                    var skBitmap = new SKBitmap(info);
                    unsafe
                    {
                        var srcPtr = locked.Address;
                        var dstPtr = skBitmap.GetPixels();

                        // Copy row by row to handle stride differences if any
                        var height = info.Height;
                        var srcStride = locked.RowBytes;
                        var dstStride = skBitmap.RowBytes;
                        var bytesPerRow = Math.Min(srcStride, dstStride); // Safe copy width

                        if (srcStride == dstStride)
                        {
                            // Full block copy if strides match (most common)
                            long totalBytes = (long)height * srcStride;
                            Buffer.MemoryCopy((void*)srcPtr, (void*)dstPtr, totalBytes, totalBytes);
                        }
                        else
                        {
                            for (int y = 0; y < height; y++)
                            {
                                var srcRow = (byte*)srcPtr + (y * srcStride);
                                var dstRow = (byte*)dstPtr + (y * dstStride);
                                Buffer.MemoryCopy(srcRow, dstRow, bytesPerRow, bytesPerRow);
                            }
                        }
                    }
                    return skBitmap;
                }
            }
            else
            {
                // Fast path for any Avalonia Bitmap: direct pixel copy via CopyPixels (no PNG encode/decode)
                var pixelSize = avaloniaBitmap.PixelSize;
                var info = new SKImageInfo(pixelSize.Width, pixelSize.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
                var skBitmap = new SKBitmap(info);
                var pixels = skBitmap.GetPixels(out IntPtr length);
                avaloniaBitmap.CopyPixels(new PixelRect(0, 0, pixelSize.Width, pixelSize.Height), pixels, (int)length, info.RowBytes);
                return skBitmap;
            }
        }

        /// <summary>
        /// Convert SKBitmap to Avalonia Bitmap using WriteableBitmap for high performance.
        /// </summary>
        public static Bitmap ToAvaloniBitmap(SKBitmap skBitmap)
        {
            ArgumentNullException.ThrowIfNull(skBitmap);
            var bitmap = new WriteableBitmap(
                new PixelSize(skBitmap.Width, skBitmap.Height), new Vector(96, 96),
                Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul);
            try
            {
                using var locked = bitmap.Lock();
                using SKPixmap pixels = skBitmap.PeekPixels();
                var info = new SKImageInfo(skBitmap.Width, skBitmap.Height, SKColorType.Bgra8888,
                    skBitmap.AlphaType == SKAlphaType.Opaque ? SKAlphaType.Opaque : SKAlphaType.Premul);
                if (pixels == null || !pixels.ReadPixels(info, locked.Address, locked.RowBytes))
                    throw new InvalidOperationException("Unable to convert the image pixels.");
                if (skBitmap.AlphaType == SKAlphaType.Opaque)
                {
                    // Opaque Skia buffers may leave alpha bytes undefined.
                    unsafe
                    {
                        for (int y = 0; y < info.Height; y++)
                        {
                            byte* row = (byte*)locked.Address + y * locked.RowBytes;
                            for (int x = 0; x < info.Width; x++) row[x * 4 + 3] = 255;
                        }
                    }
                }
                return bitmap;
            }
            catch
            {
                bitmap.Dispose();
                throw;
            }
        }
    }
}
