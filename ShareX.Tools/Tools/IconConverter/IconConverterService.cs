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
using System.IO.Compression;

namespace ShareX.Tools;

public enum IconBitDepth
{
    Palette8 = 8,
    TrueColor32 = 32
}

public static class IconConverterService
{
    public static readonly int[] SupportedSizes = [16, 32, 48, 64, 128, 256];

    public static byte[] Convert(SKBitmap source, IEnumerable<int> sizes, IconBitDepth bitDepth)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source.Width <= 0 || source.Height <= 0)
        {
            throw new ArgumentException(Localization.Strings.IconConverterService_Source_image_empty, nameof(source));
        }

        int[] selectedSizes = sizes.Distinct().Order().ToArray();
        if (selectedSizes.Length == 0)
        {
            throw new ArgumentException(Localization.Strings.IconConverterService_Select_icon_size, nameof(sizes));
        }

        if (selectedSizes.Any(size => !SupportedSizes.Contains(size)))
        {
            throw new ArgumentOutOfRangeException(nameof(sizes), Localization.Strings.IconConverterService_Unsupported_icon_size);
        }

        List<IconEntry> entries = new(selectedSizes.Length);

        foreach (int size in selectedSizes)
        {
            using SKBitmap bitmap = ResizeToSquare(source, size);
            byte[] data = size == 256
                ? EncodePng(bitmap, bitDepth)
                : EncodeBitmap(bitmap, bitDepth);

            entries.Add(new IconEntry(size, bitDepth, data));
        }

        return WriteIcon(entries);
    }

    private static SKBitmap ResizeToSquare(SKBitmap source, int size)
    {
        SKBitmap result = new(new SKImageInfo(size, size, SKColorType.Bgra8888, SKAlphaType.Premul));
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        float scale = Math.Min((float)size / source.Width, (float)size / source.Height);
        float width = source.Width * scale;
        float height = source.Height * scale;
        float left = (size - width) / 2f;
        float top = (size - height) / 2f;

        using SKImage image = SKImage.FromBitmap(source);
        using SKPaint paint = new() { IsAntialias = true };
        canvas.DrawImage(image, new SKRect(left, top, left + width, top + height),
            new SKSamplingOptions(SKCubicResampler.CatmullRom), paint);
        canvas.Flush();

        return result;
    }

    private static byte[] EncodePng(SKBitmap bitmap, IconBitDepth bitDepth)
    {
        if (bitDepth == IconBitDepth.Palette8)
        {
            return EncodeIndexedPng(bitmap);
        }

        using SKImage image = SKImage.FromBitmap(bitmap);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private static byte[] EncodeBitmap(SKBitmap bitmap, IconBitDepth bitDepth)
    {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);

        int xorStride = bitDepth == IconBitDepth.Palette8
            ? AlignToFourBytes(bitmap.Width)
            : bitmap.Width * 4;
        int maskStride = AlignToFourBytes((bitmap.Width + 7) / 8);
        int paletteSize = bitDepth == IconBitDepth.Palette8 ? 256 * 4 : 0;
        int imageSize = xorStride * bitmap.Height + maskStride * bitmap.Height;

        writer.Write(40); // BITMAPINFOHEADER size
        writer.Write(bitmap.Width);
        writer.Write(bitmap.Height * 2); // XOR image plus AND transparency mask
        writer.Write((ushort)1);
        writer.Write((ushort)bitDepth);
        writer.Write(0); // BI_RGB
        writer.Write(imageSize);
        writer.Write(0); // horizontal pixels per meter
        writer.Write(0); // vertical pixels per meter
        writer.Write(bitDepth == IconBitDepth.Palette8 ? 256 : 0);
        writer.Write(0); // important colors

        if (paletteSize > 0)
        {
            WriteBitmapPalette(writer);
        }

        byte[] row = new byte[xorStride];
        for (int y = bitmap.Height - 1; y >= 0; y--)
        {
            Array.Clear(row);

            for (int x = 0; x < bitmap.Width; x++)
            {
                SKColor color = bitmap.GetPixel(x, y);

                if (bitDepth == IconBitDepth.Palette8)
                {
                    row[x] = GetPaletteIndex(color);
                }
                else
                {
                    int offset = x * 4;
                    row[offset] = color.Blue;
                    row[offset + 1] = color.Green;
                    row[offset + 2] = color.Red;
                    row[offset + 3] = color.Alpha;
                }
            }

            writer.Write(row);
        }

        WriteTransparencyMask(writer, bitmap, maskStride);
        return stream.ToArray();
    }

    private static void WriteBitmapPalette(BinaryWriter writer)
    {
        for (int i = 0; i < 256; i++)
        {
            SKColor color = GetPaletteColor(i);
            writer.Write(color.Blue);
            writer.Write(color.Green);
            writer.Write(color.Red);
            writer.Write((byte)0);
        }
    }

    private static void WriteTransparencyMask(BinaryWriter writer, SKBitmap bitmap, int stride)
    {
        byte[] row = new byte[stride];

        for (int y = bitmap.Height - 1; y >= 0; y--)
        {
            Array.Clear(row);

            for (int x = 0; x < bitmap.Width; x++)
            {
                if (bitmap.GetPixel(x, y).Alpha < 128)
                {
                    row[x / 8] |= (byte)(0x80 >> (x % 8));
                }
            }

            writer.Write(row);
        }
    }

    private static byte[] EncodeIndexedPng(SKBitmap bitmap)
    {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);
        writer.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });

        using (MemoryStream header = new())
        using (BinaryWriter headerWriter = new(header))
        {
            WriteBigEndian(headerWriter, bitmap.Width);
            WriteBigEndian(headerWriter, bitmap.Height);
            headerWriter.Write((byte)8); // bit depth
            headerWriter.Write((byte)3); // indexed color
            headerWriter.Write((byte)0); // compression
            headerWriter.Write((byte)0); // filter
            headerWriter.Write((byte)0); // interlace
            WritePngChunk(writer, "IHDR", header.ToArray());
        }

        byte[] palette = new byte[256 * 3];
        for (int i = 0; i < 256; i++)
        {
            SKColor color = GetPaletteColor(i);
            palette[i * 3] = color.Red;
            palette[i * 3 + 1] = color.Green;
            palette[i * 3 + 2] = color.Blue;
        }
        WritePngChunk(writer, "PLTE", palette);

        byte[] alpha = Enumerable.Repeat((byte)255, 256).ToArray();
        alpha[0] = 0;
        WritePngChunk(writer, "tRNS", alpha);

        using MemoryStream compressed = new();
        using (ZLibStream zlib = new(compressed, CompressionLevel.Optimal, leaveOpen: true))
        {
            byte[] row = new byte[bitmap.Width + 1];
            for (int y = 0; y < bitmap.Height; y++)
            {
                row[0] = 0; // no PNG filter
                for (int x = 0; x < bitmap.Width; x++)
                {
                    row[x + 1] = GetPaletteIndex(bitmap.GetPixel(x, y));
                }
                zlib.Write(row);
            }
        }
        WritePngChunk(writer, "IDAT", compressed.ToArray());
        WritePngChunk(writer, "IEND", []);

        return stream.ToArray();
    }

    private static byte GetPaletteIndex(SKColor color)
    {
        if (color.Alpha < 128)
        {
            return 0;
        }

        int red = (color.Red * 5 + 127) / 255;
        int green = (color.Green * 6 + 127) / 255;
        int blue = (color.Blue * 5 + 127) / 255;
        return (byte)(1 + red * 42 + green * 6 + blue);
    }

    private static SKColor GetPaletteColor(int index)
    {
        if (index == 0)
        {
            return SKColors.Transparent;
        }

        if (index > 252)
        {
            byte gray = (byte)((index - 252) * 255 / 3);
            return new SKColor(gray, gray, gray);
        }

        int value = index - 1;
        int red = value / 42;
        int green = value / 6 % 7;
        int blue = value % 6;
        return new SKColor((byte)(red * 255 / 5), (byte)(green * 255 / 6), (byte)(blue * 255 / 5));
    }

    private static void WritePngChunk(BinaryWriter writer, string type, byte[] data)
    {
        byte[] typeBytes = System.Text.Encoding.ASCII.GetBytes(type);
        WriteBigEndian(writer, data.Length);
        writer.Write(typeBytes);
        writer.Write(data);

        uint crc = 0xFFFFFFFF;
        foreach (byte value in typeBytes.Concat(data))
        {
            crc ^= value;
            for (int i = 0; i < 8; i++)
            {
                crc = (crc & 1) != 0 ? 0xEDB88320 ^ (crc >> 1) : crc >> 1;
            }
        }

        WriteBigEndian(writer, ~crc);
    }

    private static byte[] WriteIcon(IReadOnlyList<IconEntry> entries)
    {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);

        writer.Write((ushort)0);
        writer.Write((ushort)1);
        writer.Write((ushort)entries.Count);

        int offset = 6 + entries.Count * 16;
        foreach (IconEntry entry in entries)
        {
            writer.Write(entry.Size == 256 ? (byte)0 : (byte)entry.Size);
            writer.Write(entry.Size == 256 ? (byte)0 : (byte)entry.Size);
            writer.Write((byte)0); // 0 means 256 or more colors
            writer.Write((byte)0);
            writer.Write((ushort)1);
            writer.Write((ushort)entry.BitDepth);
            writer.Write(entry.Data.Length);
            writer.Write(offset);
            offset += entry.Data.Length;
        }

        foreach (IconEntry entry in entries)
        {
            writer.Write(entry.Data);
        }

        return stream.ToArray();
    }

    private static int AlignToFourBytes(int value) => (value + 3) & ~3;

    private static void WriteBigEndian(BinaryWriter writer, int value) => WriteBigEndian(writer, unchecked((uint)value));

    private static void WriteBigEndian(BinaryWriter writer, uint value)
    {
        writer.Write((byte)(value >> 24));
        writer.Write((byte)(value >> 16));
        writer.Write((byte)(value >> 8));
        writer.Write((byte)value);
    }

    private sealed record IconEntry(int Size, IconBitDepth BitDepth, byte[] Data);
}
