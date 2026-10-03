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
using ShareX.AvaloniaUI.Localization;
using SkiaSharp;
using System.Buffers.Binary;

namespace ShareX.AvaloniaUI.Input;

/// <summary>Decodes the image and hotspot of a CUR asset without needing a desktop or Avalonia render service.</summary>
internal static class CursorBitmapDecoder
{
    public static SKBitmap Decode(Stream stream, double renderScaling, out PixelPoint hotSpot)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using MemoryStream memory = new();
        stream.CopyTo(memory);
        byte[] data = memory.ToArray();
        if (data.Length < 22)
            throw new InvalidDataException(Strings.CursorAssetLoader_Cursor_asset_is_too_small);
        if (ReadUInt16(data, 2) != 2)
            throw new NotSupportedException(Strings.CursorAssetLoader_Only_cur_cursor_assets_are_supported);
        if (ReadUInt16(data, 4) == 0)
            throw new InvalidDataException(Strings.CursorAssetLoader_Cursor_asset_does_not_contain_any_images);

        int imageLength = checked((int)ReadUInt32(data, 14));
        int imageOffset = checked((int)ReadUInt32(data, 18));
        RequireRange(data.Length, imageOffset, imageLength);
        ReadOnlySpan<byte> payload = data.AsSpan(imageOffset, imageLength);
        hotSpot = new PixelPoint(ReadUInt16(data, 10), ReadUInt16(data, 12));
        SKBitmap bitmap = payload.StartsWith(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })
            ? SKBitmap.Decode(payload.ToArray()) ?? throw new InvalidDataException(Strings.CursorAssetLoader_Cursor_image_payload_is_out_of_range)
            : DecodeDib(payload);

        try
        {
            double scale = double.IsFinite(renderScaling) && renderScaling > 1 ? renderScaling : 1;
            int width = Math.Max(1, checked((int)Math.Round(bitmap.Width * scale)));
            int height = Math.Max(1, checked((int)Math.Round(bitmap.Height * scale)));
            hotSpot = new PixelPoint(
                Math.Clamp((int)Math.Round(hotSpot.X * (double)width / bitmap.Width), 0, width - 1),
                Math.Clamp((int)Math.Round(hotSpot.Y * (double)height / bitmap.Height), 0, height - 1));
            if (width != bitmap.Width || height != bitmap.Height)
            {
                SKBitmap scaled = bitmap.Resize(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul),
                    new SKSamplingOptions(SKCubicResampler.CatmullRom))
                    ?? throw new InvalidOperationException(Strings.CursorAssetLoader_Failed_to_resize_cursor_bitmap);
                bitmap.Dispose();
                bitmap = scaled;
            }
            return bitmap;
        }
        catch
        {
            bitmap.Dispose();
            throw;
        }
    }

    private static SKBitmap DecodeDib(ReadOnlySpan<byte> data)
    {
        RequireRange(data.Length, 0, 40);
        int headerSize = ReadInt32(data, 0);
        if (headerSize < 40)
            throw new NotSupportedException(Strings.CursorAssetLoader_Unsupported_cursor_bitmap_header);
        RequireRange(data.Length, 0, headerSize);
        if (ReadUInt32(data, 16) != 0)
            throw new NotSupportedException(Strings.CursorAssetLoader_Compressed_cursor_bitmaps_are_not_supported);
        int width = Math.Abs(ReadInt32(data, 4));
        int rawHeight = ReadInt32(data, 8);
        int height = Math.Abs(rawHeight) / 2;
        int bits = ReadUInt16(data, 14);
        if (width <= 0 || height <= 0 || Math.Abs(rawHeight) % 2 != 0)
            throw new InvalidDataException(Strings.CursorAssetLoader_Cursor_image_payload_is_out_of_range);
        if (bits != 1 && bits != 32)
            throw new NotSupportedException(string.Format(Strings.CursorAssetLoader_Unsupported_cursor_bit_depth, bits));

        int paletteCount = bits == 1 ? checked((int)ReadUInt32(data, 32)) : 0;
        if (bits == 1 && paletteCount == 0) paletteCount = 2;
        if (bits == 1 && paletteCount < 2)
            throw new InvalidDataException(Strings.CursorAssetLoader_Cursor_image_payload_is_out_of_range);
        int xorOffset = checked(headerSize + paletteCount * 4);
        int xorStride = AlignToDword(bits == 1 ? checked((width + 7) / 8) : checked(width * 4));
        int xorLength = checked(xorStride * height);
        RequireRange(data.Length, headerSize, checked(paletteCount * 4));
        RequireRange(data.Length, xorOffset, xorLength);
        int andOffset = checked(xorOffset + xorLength);
        int andStride = AlignToDword(checked((width + 7) / 8));
        int andLength = checked(andStride * height);
        bool hasAlpha = bits == 32 && HasAlpha(data.Slice(xorOffset, xorLength), width, height, xorStride);
        // A 32-bit cursor with explicit alpha may omit the legacy AND mask.
        if (!hasAlpha) RequireRange(data.Length, andOffset, andLength);

        SKBitmap bitmap = new(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul));
        try
        {
            SKColor background = bits == 1 ? ReadPaletteColor(data, headerSize) : default;
            SKColor foreground = bits == 1 ? ReadPaletteColor(data, headerSize + 4) : default;
            for (int y = 0; y < height; y++)
            {
                int sourceRow = rawHeight > 0 ? height - 1 - y : y;
                int xorRow = xorOffset + sourceRow * xorStride;
                int andRow = andOffset + sourceRow * andStride;
                for (int x = 0; x < width; x++)
                {
                    SKColor color;
                    if (bits == 1)
                    {
                        bool xor = ReadMaskBit(data, xorRow, x);
                        bool and = ReadMaskBit(data, andRow, x);
                        color = and ? (xor ? SKColors.White : SKColors.Transparent) : (xor ? foreground : background);
                    }
                    else
                    {
                        int pixel = xorRow + x * 4;
                        byte alpha = hasAlpha ? data[pixel + 3] : (ReadMaskBit(data, andRow, x) ? (byte)0 : (byte)255);
                        color = new SKColor(data[pixel + 2], data[pixel + 1], data[pixel], alpha);
                    }
                    // SetPixel takes straight-alpha colours and stores the required premultiplied channels.
                    bitmap.SetPixel(x, y, color);
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

    private static bool HasAlpha(ReadOnlySpan<byte> data, int width, int height, int stride)
    {
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (data[y * stride + x * 4 + 3] != 0) return true;
        return false;
    }

    private static void RequireRange(int length, int offset, int count)
    {
        if (offset < 0 || count < 0 || offset > length - count)
            throw new InvalidDataException(Strings.CursorAssetLoader_Cursor_image_payload_is_out_of_range);
    }

    private static int AlignToDword(int value) => checked((value + 3) & ~3);
    private static bool ReadMaskBit(ReadOnlySpan<byte> data, int row, int x) => (data[row + x / 8] & (1 << (7 - x % 8))) != 0;
    private static SKColor ReadPaletteColor(ReadOnlySpan<byte> data, int offset) => new(data[offset + 2], data[offset + 1], data[offset], 255);
    private static ushort ReadUInt16(ReadOnlySpan<byte> data, int offset) => BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
    private static uint ReadUInt32(ReadOnlySpan<byte> data, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
    private static int ReadInt32(ReadOnlySpan<byte> data, int offset) => BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4));
}
