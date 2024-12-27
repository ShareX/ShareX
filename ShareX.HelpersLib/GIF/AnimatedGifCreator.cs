#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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

using ShareX.HelpersLib.Helpers;

using System;
using System.Drawing;
using System.IO;

namespace ShareX.HelpersLib.GIF;

public class AnimatedGifCreator(string filePath, int delay, int repeat = 0) : IDisposable
{
    public string FilePath { get; private set; } = filePath;
    public int Delay { get; private set; } = delay;
    public int Repeat { get; private set; } = repeat;
    public int FrameCount { get; private set; }

    private FileStream stream;

    public void AddFrame(Image img, GIFQuality quality = GIFQuality.Default)
    {
        GifClass gif = new();
        gif.LoadGifPicture(img, quality);

        if (stream == null)
        {
            stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            stream.Write(CreateHeaderBlock());
            stream.Write(gif.ScreenDescriptor.ToArray());
            stream.Write(CreateApplicationExtensionBlock(Repeat));
        }

        stream.Write(CreateGraphicsControlExtensionBlock(Delay));
        stream.Write(gif.ImageDescriptor.ToArray());
        stream.Write(gif.ColorTable.ToArray());
        stream.Write(gif.ImageData.ToArray());

        FrameCount++;
    }

    public void AddFrame(string path, GIFQuality quality = GIFQuality.Default)
    {
        using Bitmap bmp = ImageHelpers.LoadImage(path);
        AddFrame(bmp, quality);
    }

    private void Finish()
    {
        if (stream != null)
        {
            stream.WriteByte(0x3B); // Image terminator
            stream.Dispose();
        }
    }

    public void Dispose() => Finish();

    private static byte[] CreateHeaderBlock()
    {
        return "GIF89a"u8.ToArray();
    }

    private static byte[] CreateApplicationExtensionBlock(int repeat)
    {
        byte[] buffer =
        [
            0x21, // Extension introducer
            0xFF, // Application extension
            0x0B, // Size of block
            (byte)'N', // NETSCAPE2.0
            (byte)'E',
            (byte)'T',
            (byte)'S',
            (byte)'C',
            (byte)'A',
            (byte)'P',
            (byte)'E',
            (byte)'2',
            (byte)'.',
            (byte)'0',
            0x03, // Size of block
            0x01, // Loop indicator
            (byte)(repeat % 0x100), // Number of repetitions
            (byte)(repeat / 0x100), // 0 for endless loop
            0x00, // Block terminator
        ];
        return buffer;
    }

    private static byte[] CreateGraphicsControlExtensionBlock(int delay)
    {
        byte[] buffer =
        [
            0x21, // Extension introducer
            0xF9, // Graphic control extension
            0x04, // Size of block
            0x09, // Flags: reserved, disposal method, user input, transparent color
            (byte)(delay / 10 % 0x100), // Delay time low byte
            (byte)(delay / 10 / 0x100), // Delay time high byte
            0xFF, // Transparent color index
            0x00, // Block terminator
        ];
        return buffer;
    }
}