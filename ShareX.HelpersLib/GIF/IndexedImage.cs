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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace ShareX.HelpersLib;

/// <summary>Palette pixels used by GIF encoders, independent of a platform's bitmap implementation.</summary>
public sealed class IndexedImage
{
    public int Width { get; }
    public int Height { get; }
    public byte[] Pixels { get; }
    public Color[] Palette { get; }

    public IndexedImage(int width, int height, byte[] pixels, Color[] palette)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        if (pixels.Length != checked(width * height) || palette.Length is < 1 or > 256)
            throw new ArgumentException("Invalid palette image dimensions or palette.");
        Width = width; Height = height; Pixels = pixels; Palette = palette;
    }

    public void SaveGif(Stream stream)
    {
        WriteHeader(stream);
        WriteFrame(stream, 0);
        stream.WriteByte(0x3B);
    }

    internal void WriteHeader(Stream stream)
    {
        ValidateGifSize();
        using BinaryWriter writer = new(stream, Encoding.ASCII, true);
        writer.Write(Encoding.ASCII.GetBytes("GIF89a"));
        writer.Write((ushort)Width); writer.Write((ushort)Height);
        writer.Write((byte)0x70); writer.Write((byte)0); writer.Write((byte)0);
    }

    internal void WriteFrame(Stream stream, int delay)
    {
        ValidateGifSize();
        int transparent = Array.FindIndex(Palette, color => color.A < 128);
        int bits = 1;
        while ((1 << bits) < Palette.Length) bits++;
        using BinaryWriter writer = new(stream, Encoding.ASCII, true);
        writer.Write((byte)0x21); writer.Write((byte)0xF9); writer.Write((byte)4);
        writer.Write((byte)(8 | (transparent >= 0 ? 1 : 0))); // Restore background after this complete frame.
        writer.Write((ushort)Math.Clamp(delay / 10, 0, ushort.MaxValue));
        writer.Write((byte)Math.Max(0, transparent)); writer.Write((byte)0);
        writer.Write((byte)0x2C); writer.Write((ushort)0); writer.Write((ushort)0);
        writer.Write((ushort)Width); writer.Write((ushort)Height); writer.Write((byte)(0x80 | (bits - 1)));
        for (int index = 0; index < (1 << bits); index++)
        {
            Color color = index < Palette.Length ? Palette[index] : Color.Black;
            writer.Write(color.R); writer.Write(color.G); writer.Write(color.B);
        }
        int minimumCodeSize = Math.Max(2, bits);
        writer.Write((byte)minimumCodeSize);
        byte[] data = Compress(Pixels, minimumCodeSize);
        for (int offset = 0; offset < data.Length; offset += 255)
        {
            int count = Math.Min(255, data.Length - offset);
            writer.Write((byte)count); writer.Write(data, offset, count);
        }
        writer.Write((byte)0);
    }

    private void ValidateGifSize()
    {
        if (Width > ushort.MaxValue || Height > ushort.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(Width), "GIF dimensions cannot exceed 65535 pixels.");
    }

    // GIF LZW uses little-endian bit packing, with 12-bit dictionary codes at most.
    private static byte[] Compress(byte[] pixels, int minimumCodeSize)
    {
        using MemoryStream output = new();
        Dictionary<int, int> dictionary = new();
        int clear = 1 << minimumCodeSize, end = clear + 1;
        int next = end + 1, size = minimumCodeSize + 1;
        uint buffer = 0; int bufferedBits = 0;
        void WriteCode(int code)
        {
            buffer |= (uint)code << bufferedBits;
            bufferedBits += size;
            while (bufferedBits >= 8)
            {
                output.WriteByte((byte)buffer); buffer >>= 8; bufferedBits -= 8;
            }
        }
        WriteCode(clear);
        int prefix = pixels[0];
        foreach (byte pixel in pixels.Skip(1))
        {
            int key = (prefix << 8) | pixel;
            if (dictionary.TryGetValue(key, out int code)) { prefix = code; continue; }
            WriteCode(prefix);
            if (next < 4096)
            {
                dictionary.Add(key, next++);
                // The decoder creates an entry one emitted code later than the encoder.
                if (next > (1 << size) && size < 12) size++;
            }
            else
            {
                WriteCode(clear); dictionary.Clear(); next = end + 1; size = minimumCodeSize + 1;
            }
            prefix = pixel;
        }
        WriteCode(prefix);
        if (next == (1 << size) && size < 12) size++;
        WriteCode(end);
        if (bufferedBits > 0) output.WriteByte((byte)buffer);
        return output.ToArray();
    }
}
