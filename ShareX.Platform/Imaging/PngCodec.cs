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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ShareX.Platform.Imaging;

/// <summary>
/// Minimal PNG encoder and decoder so platform code can exchange images without System.Drawing,
/// which is Windows only, or a native imaging library.
/// </summary>
/// <remarks>The decoder handles non interlaced 8 and 16 bit images of every colour type, which covers what screenshot tools and clipboards produce.</remarks>
public static class PngCodec
{
    private static readonly byte[] Signature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly uint[] CrcTable = CreateCrcTable();

    public static byte[] Encode(PixelBuffer image, CompressionLevel compressionLevel = CompressionLevel.Fastest)
    {
        ArgumentNullException.ThrowIfNull(image);

        using MemoryStream output = new MemoryStream();
        output.Write(Signature);

        byte[] header = new byte[13];
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(0), image.Width);
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(4), image.Height);
        header[8] = 8; // Bit depth
        header[9] = 6; // Colour type RGBA
        WriteChunk(output, "IHDR", header);

        using (MemoryStream compressed = new MemoryStream())
        {
            using (ZLibStream zlib = new ZLibStream(compressed, compressionLevel, true))
            {
                int rowLength = image.Width * 4;
                byte[] row = new byte[rowLength + 1];
                byte[] pixels = image.Pixels;

                for (int y = 0; y < image.Height; y++)
                {
                    int offset = y * image.Stride;
                    // Filter type 1 (Sub) compresses screenshots far better than no filter at almost no cost.
                    row[0] = 1;

                    for (int i = 0; i < rowLength; i += 4)
                    {
                        byte r = pixels[offset + i + 2], g = pixels[offset + i + 1], b = pixels[offset + i], a = pixels[offset + i + 3];

                        if (i == 0)
                        {
                            row[1] = r; row[2] = g; row[3] = b; row[4] = a;
                        }
                        else
                        {
                            row[i + 1] = (byte)(r - pixels[offset + i - 2]);
                            row[i + 2] = (byte)(g - pixels[offset + i - 3]);
                            row[i + 3] = (byte)(b - pixels[offset + i - 4]);
                            row[i + 4] = (byte)(a - pixels[offset + i - 1]);
                        }
                    }

                    zlib.Write(row);
                }
            }

            WriteChunk(output, "IDAT", compressed.ToArray());
        }

        WriteChunk(output, "IEND", Array.Empty<byte>());
        return output.ToArray();
    }

    public static bool IsPng(ReadOnlySpan<byte> data) => data.Length >= Signature.Length && data[..Signature.Length].SequenceEqual(Signature);

    /// <summary>Reads width and height from the IHDR chunk without decoding pixels.</summary>
    public static PlatformSize ReadSize(ReadOnlySpan<byte> png)
    {
        if (!IsPng(png) || png.Length < 24)
        {
            throw new InvalidDataException("Not a PNG image.");
        }

        return new PlatformSize(BinaryPrimitives.ReadInt32BigEndian(png[16..]), BinaryPrimitives.ReadInt32BigEndian(png[20..]));
    }

    public static PixelBuffer Decode(byte[] png)
    {
        ArgumentNullException.ThrowIfNull(png);

        if (!IsPng(png))
        {
            throw new InvalidDataException("Not a PNG image.");
        }

        int width = 0, height = 0, bitDepth = 0, colorType = 0;
        byte[]? palette = null;
        byte[]? transparency = null;
        using MemoryStream idat = new MemoryStream();
        int position = Signature.Length;

        while (position + 8 <= png.Length)
        {
            int length = BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(position));
            string type = Encoding.ASCII.GetString(png, position + 4, 4);
            int dataStart = position + 8;

            if (length < 0 || dataStart + length > png.Length)
            {
                throw new InvalidDataException("Truncated PNG chunk.");
            }

            ReadOnlySpan<byte> data = png.AsSpan(dataStart, length);

            switch (type)
            {
                case "IHDR":
                    width = BinaryPrimitives.ReadInt32BigEndian(data);
                    height = BinaryPrimitives.ReadInt32BigEndian(data[4..]);
                    bitDepth = data[8];
                    colorType = data[9];

                    if (data[12] != 0)
                    {
                        throw new NotSupportedException("Interlaced PNG images are not supported.");
                    }

                    if (bitDepth != 8 && bitDepth != 16 && colorType != 3)
                    {
                        throw new NotSupportedException($"PNG bit depth {bitDepth} is not supported.");
                    }

                    break;
                case "PLTE":
                    palette = data.ToArray();
                    break;
                case "tRNS":
                    transparency = data.ToArray();
                    break;
                case "IDAT":
                    idat.Write(data);
                    break;
                case "IEND":
                    position = png.Length;
                    continue;
            }

            position = dataStart + length + 4;
        }

        if (width <= 0 || height <= 0)
        {
            throw new InvalidDataException("PNG image has no IHDR chunk.");
        }

        int channels = colorType switch
        {
            0 => 1,
            2 => 3,
            3 => 1,
            4 => 2,
            6 => 4,
            _ => throw new InvalidDataException($"Unknown PNG colour type {colorType}.")
        };

        int bitsPerPixel = channels * bitDepth;
        int bytesPerPixel = Math.Max(1, bitsPerPixel / 8);
        int rowLength = (width * bitsPerPixel + 7) / 8;
        byte[] raw = new byte[(rowLength + 1) * height];

        idat.Position = 0;

        using (ZLibStream zlib = new ZLibStream(idat, CompressionMode.Decompress))
        {
            zlib.ReadExactly(raw);
        }

        byte[] previous = new byte[rowLength];
        byte[] current = new byte[rowLength];
        PixelBuffer result = new PixelBuffer(width, height);
        byte[] output = result.Pixels;

        for (int y = 0; y < height; y++)
        {
            int rowStart = y * (rowLength + 1);
            byte filter = raw[rowStart];
            Array.Copy(raw, rowStart + 1, current, 0, rowLength);
            Unfilter(filter, current, previous, bytesPerPixel);

            int o = y * result.Stride;

            for (int x = 0; x < width; x++, o += 4)
            {
                byte r, g, b, a = 255;

                switch (colorType)
                {
                    case 0:
                        r = g = b = Sample(current, x, 0, 1, bitDepth);
                        if (transparency is { Length: >= 2 } && bitDepth == 8 && r == transparency[1]) a = 0;
                        break;
                    case 2:
                        r = Sample(current, x, 0, 3, bitDepth);
                        g = Sample(current, x, 1, 3, bitDepth);
                        b = Sample(current, x, 2, 3, bitDepth);
                        if (transparency is { Length: >= 6 } && bitDepth == 8 && r == transparency[1] && g == transparency[3] && b == transparency[5]) a = 0;
                        break;
                    case 3:
                        int index = PaletteIndex(current, x, bitDepth);
                        if (palette == null || index * 3 + 2 >= palette.Length) throw new InvalidDataException("PNG palette index out of range.");
                        r = palette[index * 3];
                        g = palette[index * 3 + 1];
                        b = palette[index * 3 + 2];
                        if (transparency != null && index < transparency.Length) a = transparency[index];
                        break;
                    case 4:
                        r = g = b = Sample(current, x, 0, 2, bitDepth);
                        a = Sample(current, x, 1, 2, bitDepth);
                        break;
                    default:
                        r = Sample(current, x, 0, 4, bitDepth);
                        g = Sample(current, x, 1, 4, bitDepth);
                        b = Sample(current, x, 2, 4, bitDepth);
                        a = Sample(current, x, 3, 4, bitDepth);
                        break;
                }

                output[o] = b;
                output[o + 1] = g;
                output[o + 2] = r;
                output[o + 3] = a;
            }

            (previous, current) = (current, previous);
        }

        return result;
    }

    /// <summary>Crops a PNG image and returns it re-encoded.</summary>
    public static byte[] Crop(byte[] png, PlatformRectangle area) => Encode(Decode(png).Crop(area));

    private static byte Sample(byte[] row, int x, int channel, int channels, int bitDepth)
    {
        // For 16 bit images keep the most significant byte.
        return bitDepth == 16 ? row[(x * channels + channel) * 2] : row[x * channels + channel];
    }

    private static int PaletteIndex(byte[] row, int x, int bitDepth)
    {
        if (bitDepth == 8) return row[x];

        int pixelsPerByte = 8 / bitDepth;
        int shift = (pixelsPerByte - 1 - x % pixelsPerByte) * bitDepth;
        return (row[x / pixelsPerByte] >> shift) & ((1 << bitDepth) - 1);
    }

    private static void Unfilter(byte filter, byte[] current, byte[] previous, int bpp)
    {
        switch (filter)
        {
            case 0:
                break;
            case 1:
                for (int i = bpp; i < current.Length; i++) current[i] += current[i - bpp];
                break;
            case 2:
                for (int i = 0; i < current.Length; i++) current[i] += previous[i];
                break;
            case 3:
                for (int i = 0; i < current.Length; i++)
                {
                    int left = i >= bpp ? current[i - bpp] : 0;
                    current[i] += (byte)((left + previous[i]) >> 1);
                }
                break;
            case 4:
                for (int i = 0; i < current.Length; i++)
                {
                    int left = i >= bpp ? current[i - bpp] : 0;
                    int upLeft = i >= bpp ? previous[i - bpp] : 0;
                    current[i] += Paeth(left, previous[i], upLeft);
                }
                break;
            default:
                throw new InvalidDataException($"Unknown PNG filter type {filter}.");
        }
    }

    private static byte Paeth(int a, int b, int c)
    {
        int p = a + b - c;
        int pa = Math.Abs(p - a), pb = Math.Abs(p - b), pc = Math.Abs(p - c);
        if (pa <= pb && pa <= pc) return (byte)a;
        return pb <= pc ? (byte)b : (byte)c;
    }

    private static void WriteChunk(Stream output, string type, byte[] data)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(buffer, data.Length);
        output.Write(buffer);

        byte[] typeBytes = Encoding.ASCII.GetBytes(type);
        output.Write(typeBytes);
        output.Write(data);

        uint crc = UpdateCrc(0xFFFFFFFFu, typeBytes);
        crc = UpdateCrc(crc, data) ^ 0xFFFFFFFFu;
        BinaryPrimitives.WriteUInt32BigEndian(buffer, crc);
        output.Write(buffer);
    }

    private static uint UpdateCrc(uint crc, ReadOnlySpan<byte> data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            crc = CrcTable[(crc ^ data[i]) & 0xFF] ^ (crc >> 8);
        }

        return crc;
    }

    private static uint[] CreateCrcTable()
    {
        uint[] table = new uint[256];

        for (uint n = 0; n < 256; n++)
        {
            uint c = n;

            for (int k = 0; k < 8; k++)
            {
                c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1;
            }

            table[n] = c;
        }

        return table;
    }
}
