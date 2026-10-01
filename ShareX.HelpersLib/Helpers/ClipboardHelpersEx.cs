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
using System.Buffers.Binary;
using System.IO;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib;

internal static class ClipboardHelpersEx
{
    public static byte[] ConvertToDib(SKBitmap image)
    {
        int rowBytes = checked(image.Width * 4);
        byte[] result = new byte[checked(52 + rowBytes * image.Height)];
        Span<byte> header = result.AsSpan();
        BinaryPrimitives.WriteInt32LittleEndian(header, 40);
        BinaryPrimitives.WriteInt32LittleEndian(header[4..], image.Width);
        BinaryPrimitives.WriteInt32LittleEndian(header[8..], image.Height);
        BinaryPrimitives.WriteInt16LittleEndian(header[12..], 1);
        BinaryPrimitives.WriteInt16LittleEndian(header[14..], 32);
        BinaryPrimitives.WriteInt32LittleEndian(header[16..], 3);
        BinaryPrimitives.WriteInt32LittleEndian(header[20..], rowBytes * image.Height);
        BinaryPrimitives.WriteUInt32LittleEndian(header[40..], 0x00FF0000);
        BinaryPrimitives.WriteUInt32LittleEndian(header[44..], 0x0000FF00);
        BinaryPrimitives.WriteUInt32LittleEndian(header[48..], 0x000000FF);
        using SKBitmap pixels = new(new SKImageInfo(image.Width, image.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
        using SKPixmap source = image.PeekPixels();
        if (!source.ReadPixels(pixels.Info, pixels.GetPixels(), pixels.RowBytes))
            throw new InvalidOperationException("Unable to copy clipboard image pixels.");
        for (int y = 0; y < image.Height; y++)
            Marshal.Copy(IntPtr.Add(pixels.GetPixels(), y * pixels.RowBytes), result, 52 + (image.Height - y - 1) * rowBytes, rowBytes);
        return result;
    }

    public static SKBitmap DIBV5ToBitmap(byte[] data) => ImageFromClipboardDib(data);

    public static SKBitmap ImageFromClipboardDib(byte[] data)
    {
        if (data == null || data.Length < 40) return null;
        try
        {
            ReadOnlySpan<byte> bytes = data;
            int headerSize = BinaryPrimitives.ReadInt32LittleEndian(bytes);
            if (headerSize < 40 || headerSize > data.Length) return null;
            int width = BinaryPrimitives.ReadInt32LittleEndian(bytes[4..]);
            int signedHeight = BinaryPrimitives.ReadInt32LittleEndian(bytes[8..]);
            int height = Math.Abs(signedHeight);
            int bitCount = BinaryPrimitives.ReadUInt16LittleEndian(bytes[14..]);
            int compression = BinaryPrimitives.ReadInt32LittleEndian(bytes[16..]);
            uint colorCount = BinaryPrimitives.ReadUInt32LittleEndian(bytes[32..]);
            int paletteSize = bitCount <= 8 ? checked((int)(colorCount == 0 ? 1u << bitCount : colorCount) * 4) : 0;
            int pixelOffset = checked(headerSize + paletteSize + (headerSize == 40 && compression == 3 ? 12 : headerSize == 40 && compression == 6 ? 16 : 0));
            int rowBytes = checked((width * bitCount + 31) / 32 * 4);
            if (width <= 0 || height <= 0 || pixelOffset > data.Length || (long)rowBytes * height > data.Length - pixelOffset) return null;
            // Legacy clipboard DIBs often contain premultiplied alpha without an alpha mask.
            if (bitCount == 32 && compression is 0 or 3 or 6)
            {
                bool standardMasks = compression == 0 ||
                    BinaryPrimitives.ReadUInt32LittleEndian(bytes[40..]) == 0x00FF0000 &&
                    BinaryPrimitives.ReadUInt32LittleEndian(bytes[44..]) == 0x0000FF00 &&
                    BinaryPrimitives.ReadUInt32LittleEndian(bytes[48..]) == 0x000000FF;
                if (standardMasks)
                {
                    bool hasAlpha = headerSize >= 56 && BinaryPrimitives.ReadUInt32LittleEndian(bytes[52..]) == 0xFF000000;
                    if (!hasAlpha && compression != 0)
                        for (int i = pixelOffset + 3; i < pixelOffset + rowBytes * height; i += 4)
                            if (data[i] != 0) { hasAlpha = true; break; }
                    SKBitmap bitmap = new(new SKImageInfo(width, height, SKColorType.Bgra8888, hasAlpha ? SKAlphaType.Premul : SKAlphaType.Opaque));
                    try
                    {
                        for (int y = 0; y < height; y++)
                            Marshal.Copy(data, pixelOffset + (signedHeight > 0 ? height - y - 1 : y) * rowBytes,
                                IntPtr.Add(bitmap.GetPixels(), y * bitmap.RowBytes), width * 4);
                        bitmap.NotifyPixelsChanged();
                        return bitmap;
                    }
                    catch { bitmap.Dispose(); throw; }
                }
            }
            // Let Skia decode palettes, RGB555/565 and other supported BMP layouts.
            using MemoryStream stream = new();
            using (BinaryWriter writer = new(stream, System.Text.Encoding.UTF8, true))
            {
                writer.Write((ushort)0x4D42);
                writer.Write(checked(14 + data.Length));
                writer.Write(0);
                writer.Write(checked(14 + pixelOffset));
                writer.Write(data);
            }
            stream.Position = 0;
            return SkiaImageHelpers.Decode(stream);
        }
        catch (Exception exception) when (exception is ArgumentException or OverflowException or InvalidDataException)
        {
            DebugHelper.WriteException(exception);
            return null;
        }
    }
}
