#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System.Globalization;
using System.IO;

namespace ShareX.MediaLib
{
    public class VideoInfo
    {
        public string FilePath { get; set; }

        public TimeSpan Duration { get; set; }
        public TimeSpan Start { get; set; }
        public int Bitrate { get; set; }

        public string VideoCodec { get; set; }
        public Size VideoResolution { get; set; }
        public double VideoFPS { get; set; }

        public string AudioCodec { get; set; }

        public override string ToString()
        {
            string text = string.Format("Filename: {0}, Duration: {1}, Bitrate: {2} kb/s", Path.GetFileName(FilePath), Duration.ToString(@"hh\:mm\:s"), Bitrate);

            if (!string.IsNullOrEmpty(VideoCodec))
            {
                text += string.Format(", Video codec: {0}, Resolution: {1}x{2}, FPS: {3}", VideoCodec, VideoResolution.Width, VideoResolution.Height,
                    VideoFPS.ToString("0.##", CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrEmpty(AudioCodec))
            {
                text += string.Format(", Audio codec: {0}", AudioCodec);
            }

            return text;
        }
    }
}