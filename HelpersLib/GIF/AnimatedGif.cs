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
using System.Drawing.Imaging;
using System.IO;

namespace HelpersLib
{
    public class AnimatedGif : IDisposable
    {
        private byte[] buf1, buf2, buf3;
        private MemoryStream gifStream;
        private BinaryWriter binaryWriter;
        private int frameNum;

        public AnimatedGif(int delay, int repeat = 0)
        {
            buf2 = new byte[19];
            buf2[0] = 0x21; // Extension introducer
            buf2[1] = 0xFF; // Application extension
            buf2[2] = 0x0B; // Size of block
            buf2[3] = 78; // N
            buf2[4] = 69; // E
            buf2[5] = 84; // T
            buf2[6] = 83; // S
            buf2[7] = 67; // C
            buf2[8] = 65; // A
            buf2[9] = 80; // P
            buf2[10] = 69; // E
            buf2[11] = 50; // 2
            buf2[12] = 46; // .
            buf2[13] = 48; // 0
            buf2[14] = 0x03; // Size of block
            buf2[15] = 0x01; // Loop indicator
            buf2[16] = (byte)(repeat % 0x100); // Number of repetitions
            buf2[17] = (byte)(repeat / 0x100); // 0 for endless loop
            buf2[18] = 0x00; // Block terminator

            buf3 = new byte[8];
            buf3[0] = 0x21; // Extension introducer
            buf3[1] = 0xF9; // Graphic control extension
            buf3[2] = 0x04; // Size of block
            buf3[3] = 0x09; // Flags: reserved, disposal method, user input, transparent color
            buf3[4] = (byte)(delay / 10 % 0x100); // Delay time low byte
            buf3[5] = (byte)(delay / 10 / 0x100); // Delay time high byte
            buf3[6] = 0xFF; // Transparent color index
            buf3[7] = 0x00; // Block terminator

            gifStream = new MemoryStream();
            binaryWriter = new BinaryWriter(gifStream);
        }

        public void AddFrame(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Gif);
                buf1 = ms.ToArray();
            }

            if (frameNum == 0)
            {
                // only write these the first time
                binaryWriter.Write(buf1, 0, 781); // Header & global color table
                binaryWriter.Write(buf2, 0, 19); // Application extension
            }

            binaryWriter.Write(buf3, 0, 8); // Graphic extension
            binaryWriter.Write(buf1, 789, buf1.Length - 790); // Image data

            frameNum++;
        }

        public void AddFrame(string path)
        {
            using (Image img = Helpers.GetImageFromFile(path))
            {
                AddFrame(img);
            }
        }

        public void Finish()
        {
            // only write this one the last time
            binaryWriter.Write(59); // Image terminator
        }

        public void Save(string filePath)
        {
            gifStream.WriteToFile(filePath);
        }

        public void Dispose()
        {
            if (gifStream != null) gifStream.Dispose();
            if (binaryWriter != null) binaryWriter.Close();
        }
    }
}