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
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;
using ShareX.AvaloniaUI.Localization;
using ShareX.AvaloniaUI.Imaging;
using System.Buffers.Binary;

namespace ShareX.AvaloniaUI.Input
{
    public static class CursorAssetLoader
    {
        public enum CustomCursorKind
        {
            ClosedHand,
            Crosshair,
            OpenHand
        }

        private static readonly Uri ClosedHandCursorUri = new("avares://ShareX.Avalonia/Assets/closedhand.cur");
        private static readonly Uri CrosshairCursorUri = new("avares://ShareX.Avalonia/Assets/Crosshair.cur");
        private static readonly Uri OpenHandCursorUri = new("avares://ShareX.Avalonia/Assets/openhand.cur");
        private static readonly Cursor FallbackClosedHandCursor = new(StandardCursorType.SizeAll);
        private static readonly Cursor FallbackCrosshairCursor = new(StandardCursorType.Cross);
        private static readonly Cursor FallbackOpenHandCursor = new(StandardCursorType.Hand);
        private static readonly object CursorCacheSyncRoot = new();
        private static readonly Dictionary<(CustomCursorKind CursorKind, int ScaleKey), LoadedCursor?> CursorCache = new();

        public static Cursor GetCrosshairCursor()
            => GetCrosshairCursor(1.0);

        public static Cursor GetCrosshairCursor(double renderScaling)
        {
            return GetCursor(CustomCursorKind.Crosshair, renderScaling);
        }

        public static Cursor GetOpenHandCursor()
            => GetOpenHandCursor(1.0);

        public static Cursor GetOpenHandCursor(double renderScaling)
        {
            return GetCursor(CustomCursorKind.OpenHand, renderScaling);
        }

        public static Cursor GetClosedHandCursor()
            => GetClosedHandCursor(1.0);

        public static Cursor GetClosedHandCursor(double renderScaling)
        {
            return GetCursor(CustomCursorKind.ClosedHand, renderScaling);
        }

        public static Cursor GetCursor(CustomCursorKind cursorKind, double renderScaling = 1.0)
        {
            return TryLoadCursor(cursorKind, renderScaling)?.Cursor ?? GetFallbackCursor(cursorKind);
        }

        private static LoadedCursor? TryLoadCursor(CustomCursorKind cursorKind, double renderScaling)
        {
            int scaleKey = NormalizeScaleKey(renderScaling);

            lock (CursorCacheSyncRoot)
            {
                if (!CursorCache.TryGetValue((cursorKind, scaleKey), out LoadedCursor? loadedCursor))
                {
                    loadedCursor = TryLoadCursorCore(GetCursorUri(cursorKind), scaleKey / 100.0);
                    CursorCache[(cursorKind, scaleKey)] = loadedCursor;
                }

                return loadedCursor;
            }
        }

        private static LoadedCursor? TryLoadCursorCore(Uri cursorUri, double renderScaling)
        {
            try
            {
                using Stream cursorStream = AssetLoader.Open(cursorUri);
                return LoadCursor(cursorStream, renderScaling);
            }
            catch
            {
                return null;
            }
        }

        private static LoadedCursor LoadCursor(Stream cursorStream, double renderScaling)
        {
            using var memoryStream = new MemoryStream();
            cursorStream.CopyTo(memoryStream);

            byte[] data = memoryStream.ToArray();
            CursorDirectoryEntry entry = ReadCursorDirectoryEntry(data);
            PixelPoint hotSpot = new(entry.HotspotX, entry.HotspotY);

            Bitmap bitmap = IsPng(data, entry.ImageOffset)
                ? LoadPngBitmap(data, entry.ImageOffset, entry.ImageLength)
                : LoadBitmapInfoCursor(data, entry.ImageOffset);

            if (TryGetScaledCursorSize(bitmap.PixelSize, renderScaling, out PixelSize scaledSize))
            {
                PixelPoint scaledHotSpot = ScaleHotSpot(hotSpot, bitmap.PixelSize, scaledSize);
                Bitmap scaledBitmap = ScaleBitmap(bitmap, scaledSize);
                bitmap.Dispose();
                bitmap = scaledBitmap;
                hotSpot = scaledHotSpot;
            }

            return new LoadedCursor(bitmap, new Cursor(bitmap, hotSpot));
        }

        private static Cursor GetFallbackCursor(CustomCursorKind cursorKind)
        {
            return cursorKind switch
            {
                CustomCursorKind.ClosedHand => FallbackClosedHandCursor,
                CustomCursorKind.Crosshair => FallbackCrosshairCursor,
                _ => FallbackOpenHandCursor
            };
        }

        private static Uri GetCursorUri(CustomCursorKind cursorKind)
        {
            return cursorKind switch
            {
                CustomCursorKind.ClosedHand => ClosedHandCursorUri,
                CustomCursorKind.Crosshair => CrosshairCursorUri,
                _ => OpenHandCursorUri
            };
        }

        private static int NormalizeScaleKey(double renderScaling)
        {
            double safeRenderScaling = double.IsFinite(renderScaling) && renderScaling > 1.0
                ? renderScaling
                : 1.0;

            return Math.Max(100, (int)Math.Round(safeRenderScaling * 100.0));
        }

        private static bool TryGetScaledCursorSize(PixelSize pixelSize, double renderScaling, out PixelSize scaledSize)
        {
            int targetWidth = Math.Max(1, (int)Math.Round(pixelSize.Width * renderScaling));
            int targetHeight = Math.Max(1, (int)Math.Round(pixelSize.Height * renderScaling));
            scaledSize = new PixelSize(targetWidth, targetHeight);

            return targetWidth != pixelSize.Width || targetHeight != pixelSize.Height;
        }

        private static PixelPoint ScaleHotSpot(PixelPoint hotSpot, PixelSize sourceSize, PixelSize targetSize)
        {
            int scaledHotSpotX = Math.Clamp((int)Math.Round(hotSpot.X * (double)targetSize.Width / sourceSize.Width), 0, targetSize.Width - 1);
            int scaledHotSpotY = Math.Clamp((int)Math.Round(hotSpot.Y * (double)targetSize.Height / sourceSize.Height), 0, targetSize.Height - 1);

            return new PixelPoint(scaledHotSpotX, scaledHotSpotY);
        }

        private static Bitmap ScaleBitmap(Bitmap bitmap, PixelSize scaledSize)
        {
            using SKBitmap sourceBitmap = BitmapConversionHelpers.ToSKBitmap(bitmap);
            SKImageInfo scaledInfo = new(scaledSize.Width, scaledSize.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            using SKBitmap? scaledBitmap = sourceBitmap.Resize(scaledInfo, new SKSamplingOptions(SKCubicResampler.CatmullRom));

            if (scaledBitmap == null)
            {
                throw new InvalidOperationException(Strings.CursorAssetLoader_Failed_to_resize_cursor_bitmap);
            }

            return BitmapConversionHelpers.ToAvaloniBitmap(scaledBitmap);
        }

        private static CursorDirectoryEntry ReadCursorDirectoryEntry(byte[] data)
        {
            if (data.Length < 22)
            {
                throw new InvalidDataException(Strings.CursorAssetLoader_Cursor_asset_is_too_small);
            }

            ushort type = ReadUInt16(data, 2);
            ushort count = ReadUInt16(data, 4);

            if (type != 2)
            {
                throw new NotSupportedException(Strings.CursorAssetLoader_Only_cur_cursor_assets_are_supported);
            }

            if (count == 0)
            {
                throw new InvalidDataException(Strings.CursorAssetLoader_Cursor_asset_does_not_contain_any_images);
            }

            int width = data[6] == 0 ? 256 : data[6];
            int height = data[7] == 0 ? 256 : data[7];
            int hotSpotX = ReadUInt16(data, 10);
            int hotSpotY = ReadUInt16(data, 12);
            int imageLength = checked((int)ReadUInt32(data, 14));
            int imageOffset = checked((int)ReadUInt32(data, 18));

            if (imageOffset < 0 || imageLength <= 0 || imageOffset + imageLength > data.Length)
            {
                throw new InvalidDataException(Strings.CursorAssetLoader_Cursor_image_payload_is_out_of_range);
            }

            return new CursorDirectoryEntry(width, height, hotSpotX, hotSpotY, imageLength, imageOffset);
        }

        private static bool IsPng(byte[] data, int imageOffset)
        {
            ReadOnlySpan<byte> pngSignature = stackalloc byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

            return data.AsSpan(imageOffset).StartsWith(pngSignature);
        }

        private static Bitmap LoadPngBitmap(byte[] data, int imageOffset, int imageLength)
        {
            using var bitmapStream = new MemoryStream(data, imageOffset, imageLength, writable: false);
            return new Bitmap(bitmapStream);
        }

        private static Bitmap LoadBitmapInfoCursor(byte[] data, int imageOffset)
        {
            int headerSize = ReadInt32(data, imageOffset);
            int rawWidth = ReadInt32(data, imageOffset + 4);
            int rawHeight = ReadInt32(data, imageOffset + 8);
            int bitsPerPixel = ReadUInt16(data, imageOffset + 14);
            uint compression = ReadUInt32(data, imageOffset + 16);
            uint colorsUsed = ReadUInt32(data, imageOffset + 32);

            if (headerSize < 40)
            {
                throw new NotSupportedException(Strings.CursorAssetLoader_Unsupported_cursor_bitmap_header);
            }

            if (compression != 0)
            {
                throw new NotSupportedException(Strings.CursorAssetLoader_Compressed_cursor_bitmaps_are_not_supported);
            }

            int width = Math.Abs(rawWidth);
            int height = Math.Abs(rawHeight) / 2;
            bool bottomUp = rawHeight > 0;

            return bitsPerPixel switch
            {
                1 => LoadMonochromeCursorBitmap(data, imageOffset, headerSize, width, height, bottomUp, colorsUsed),
                32 => LoadArgbCursorBitmap(data, imageOffset, headerSize, width, height, bottomUp),
                _ => throw new NotSupportedException(string.Format(Strings.CursorAssetLoader_Unsupported_cursor_bit_depth, bitsPerPixel))
            };
        }

        private static Bitmap LoadMonochromeCursorBitmap(byte[] data, int imageOffset, int headerSize, int width, int height, bool bottomUp, uint colorsUsed)
        {
            int paletteCount = colorsUsed > 0 ? checked((int)colorsUsed) : 2;
            int paletteOffset = imageOffset + headerSize;
            int xorStride = AlignToDword((width + 7) / 8);
            int xorOffset = paletteOffset + (paletteCount * 4);
            int andStride = AlignToDword((width + 7) / 8);
            int andOffset = xorOffset + (xorStride * height);

            var bitmap = new WriteableBitmap(
                new PixelSize(width, height),
                new Vector(96, 96),
                Avalonia.Platform.PixelFormat.Bgra8888,
                Avalonia.Platform.AlphaFormat.Premul);

            var background = ReadPaletteColor(data, paletteOffset);
            var foreground = ReadPaletteColor(data, paletteOffset + 4);

            using var framebuffer = bitmap.Lock();

            unsafe
            {
                for (int y = 0; y < height; y++)
                {
                    int sourceRow = GetSourceRow(y, height, bottomUp);
                    int xorRowOffset = xorOffset + (sourceRow * xorStride);
                    int andRowOffset = andOffset + (sourceRow * andStride);
                    byte* destinationRow = (byte*)framebuffer.Address + (y * framebuffer.RowBytes);

                    for (int x = 0; x < width; x++)
                    {
                        bool xorBit = ReadMaskBit(data, xorRowOffset, x);
                        bool andBit = ReadMaskBit(data, andRowOffset, x);
                        int pixelOffset = x * 4;

                        if (andBit && !xorBit)
                        {
                            destinationRow[pixelOffset + 0] = 0;
                            destinationRow[pixelOffset + 1] = 0;
                            destinationRow[pixelOffset + 2] = 0;
                            destinationRow[pixelOffset + 3] = 0;
                            continue;
                        }

                        RgbaColor color = xorBit ? foreground : background;

                        if (andBit && xorBit)
                        {
                            // Monochrome cursors can request screen inversion here.
                            // Avalonia cursors are static bitmaps, so use a visible fallback color instead.
                            color = new RgbaColor(255, 255, 255, 255);
                        }

                        destinationRow[pixelOffset + 0] = color.B;
                        destinationRow[pixelOffset + 1] = color.G;
                        destinationRow[pixelOffset + 2] = color.R;
                        destinationRow[pixelOffset + 3] = color.A;
                    }
                }
            }

            return bitmap;
        }

        private static Bitmap LoadArgbCursorBitmap(byte[] data, int imageOffset, int headerSize, int width, int height, bool bottomUp)
        {
            int xorOffset = imageOffset + headerSize;
            int xorStride = AlignToDword(width * 4);
            int andStride = AlignToDword((width + 7) / 8);
            int andOffset = xorOffset + (xorStride * height);
            bool hasExplicitAlpha = HasExplicitAlpha(data, xorOffset, xorStride, width, height);

            var bitmap = new WriteableBitmap(
                new PixelSize(width, height),
                new Vector(96, 96),
                Avalonia.Platform.PixelFormat.Bgra8888,
                Avalonia.Platform.AlphaFormat.Premul);

            using var framebuffer = bitmap.Lock();

            unsafe
            {
                for (int y = 0; y < height; y++)
                {
                    int sourceRow = GetSourceRow(y, height, bottomUp);
                    int xorRowOffset = xorOffset + (sourceRow * xorStride);
                    int andRowOffset = andOffset + (sourceRow * andStride);
                    byte* destinationRow = (byte*)framebuffer.Address + (y * framebuffer.RowBytes);

                    for (int x = 0; x < width; x++)
                    {
                        int sourcePixelOffset = xorRowOffset + (x * 4);
                        int destinationPixelOffset = x * 4;

                        destinationRow[destinationPixelOffset + 0] = data[sourcePixelOffset + 0];
                        destinationRow[destinationPixelOffset + 1] = data[sourcePixelOffset + 1];
                        destinationRow[destinationPixelOffset + 2] = data[sourcePixelOffset + 2];

                        byte alpha = data[sourcePixelOffset + 3];
                        if (!hasExplicitAlpha)
                        {
                            alpha = ReadMaskBit(data, andRowOffset, x) ? (byte)0 : (byte)255;
                        }

                        destinationRow[destinationPixelOffset + 3] = alpha;
                    }
                }
            }

            return bitmap;
        }

        private static bool HasExplicitAlpha(byte[] data, int xorOffset, int xorStride, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                int rowOffset = xorOffset + (y * xorStride);
                for (int x = 0; x < width; x++)
                {
                    if (data[rowOffset + (x * 4) + 3] != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static int GetSourceRow(int destinationRow, int height, bool bottomUp)
        {
            return bottomUp ? (height - 1 - destinationRow) : destinationRow;
        }

        private static bool ReadMaskBit(byte[] data, int rowOffset, int x)
        {
            int byteOffset = rowOffset + (x / 8);
            int bitShift = 7 - (x % 8);

            return (data[byteOffset] & (1 << bitShift)) != 0;
        }

        private static int AlignToDword(int value)
        {
            return (value + 3) & ~3;
        }

        private static RgbaColor ReadPaletteColor(byte[] data, int offset)
        {
            return new RgbaColor(data[offset + 2], data[offset + 1], data[offset + 0], 255);
        }

        private static ushort ReadUInt16(byte[] data, int offset)
        {
            return BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(offset, 2));
        }

        private static uint ReadUInt32(byte[] data, int offset)
        {
            return BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));
        }

        private static int ReadInt32(byte[] data, int offset)
        {
            return BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(offset, 4));
        }

        private sealed class LoadedCursor
        {
            public LoadedCursor(Bitmap bitmap, Cursor cursor)
            {
                Bitmap = bitmap;
                Cursor = cursor;
            }

            public Bitmap Bitmap { get; }
            public Cursor Cursor { get; }
        }

        private readonly struct CursorDirectoryEntry
        {
            public CursorDirectoryEntry(int width, int height, int hotSpotX, int hotSpotY, int imageLength, int imageOffset)
            {
                Width = width;
                Height = height;
                HotspotX = hotSpotX;
                HotspotY = hotSpotY;
                ImageLength = imageLength;
                ImageOffset = imageOffset;
            }

            public int Width { get; }
            public int Height { get; }
            public int HotspotX { get; }
            public int HotspotY { get; }
            public int ImageLength { get; }
            public int ImageOffset { get; }
        }

        private readonly struct RgbaColor
        {
            public RgbaColor(byte r, byte g, byte b, byte a)
            {
                R = r;
                G = g;
                B = b;
                A = a;
            }

            public byte R { get; }
            public byte G { get; }
            public byte B { get; }
            public byte A { get; }
        }
    }
}
