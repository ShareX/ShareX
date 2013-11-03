#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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
using System.Drawing;
using System.IO;

namespace HelpersLib
{
    public class GifCreator : IDisposable
    {
        private byte[] graphicControlExtensionBlock, loopBlock;
        private MemoryStream gifStream;
        private BinaryWriter binaryWriter;
        private int frameNum;

        public GifCreator(int delay, int repeat = 0)
        {
            byte[] signature = new byte[] { (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'9', (byte)'a' };
            graphicControlExtensionBlock = CreateGraphicControlExtensionBlock(delay);
            loopBlock = CreateLoopBlock(repeat);
            gifStream = new MemoryStream();
            binaryWriter = new BinaryWriter(gifStream);
            binaryWriter.Write(signature);
        }

        public void AddFrame(Image img, GIFQuality quality = GIFQuality.Default)
        {
            GifClass gif = new GifClass();
            gif.LoadGifPicture(img, quality);

            if (frameNum == 0)
            {
                binaryWriter.Write(gif.m_ScreenDescriptor.ToArray());
            }

            binaryWriter.Write(graphicControlExtensionBlock);
            binaryWriter.Write(gif.m_ImageDescriptor.ToArray());
            binaryWriter.Write(gif.m_ColorTable.ToArray());
            binaryWriter.Write(gif.m_ImageData.ToArray());

            frameNum++;
        }

        public void AddFrame(string path, GIFQuality quality = GIFQuality.Default)
        {
            using (Image img = Helpers.GetImageFromFile(path))
            {
                AddFrame(img, quality);
            }
        }

        public void Finish()
        {
            binaryWriter.Write(CreateLoopBlock());
            binaryWriter.Write(0x3B); // Image terminator
        }

        public void Save(string filePath)
        {
            gifStream.WriteToFile(filePath);
        }

        private byte[] CreateGraphicControlExtensionBlock(int delay)
        {
            byte[] buffer = new byte[8];
            buffer[0] = 0x21; // Extension introducer
            buffer[1] = 0xF9; // Graphic control extension
            buffer[2] = 0x04; // Size of block
            buffer[3] = 0x09; // Flags: reserved, disposal method, user input, transparent color
            buffer[4] = (byte)(delay / 10 % 0x100); // Delay time low byte
            buffer[5] = (byte)(delay / 10 / 0x100); // Delay time high byte
            buffer[6] = 0xFF; // Transparent color index
            buffer[7] = 0x00; // Block terminator
            return buffer;
        }

        private byte[] CreateLoopBlock(int repeat = 0)
        {
            byte[] buffer = new byte[19];
            buffer[0] = 0x21; // Extension introducer
            buffer[1] = 0xFF; // Application extension
            buffer[2] = 0x0B; // Size of block
            buffer[3] = (byte)'N';
            buffer[4] = (byte)'E';
            buffer[5] = (byte)'T';
            buffer[6] = (byte)'S';
            buffer[7] = (byte)'C';
            buffer[8] = (byte)'A';
            buffer[9] = (byte)'P';
            buffer[10] = (byte)'E';
            buffer[11] = (byte)'2';
            buffer[12] = (byte)'.';
            buffer[13] = (byte)'0';
            buffer[14] = 0x03; // Size of block
            buffer[15] = 0x01; // Loop indicator
            buffer[16] = (byte)(repeat % 0x100); // Number of repetitions
            buffer[17] = (byte)(repeat / 0x100); // 0 for endless loop
            buffer[18] = 0x00; // Block terminator
            return buffer;
        }

        public void Dispose()
        {
            if (gifStream != null) gifStream.Dispose();
            if (binaryWriter != null) binaryWriter.Close();
        }
    }
}