#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using System.Linq;
using System.Text;

namespace ScreenCaptureLib
{
    public class FFmpegOptions
    {
        // General
        public string VideoSource { get; set; }
        public string AudioSource { get; set; }
        public FFmpegVideoCodec VideoCodec { get; set; }
        public FFmpegAudioCodec AudioCodec { get; set; }
        public string Extension { get; set; }
        public string CLIPath { get; set; }
        public string UserArgs { get; set; }
        public bool ShowError { get; set; }

        // H.264 - x264
        public FFmpegPreset Preset { get; set; }
        public int x264CRF { get; set; }

        // H.264 - VPx
        public int VPxCRF { get; set; }

        // H.263
        public int qscale { get; set; }

        public FFmpegOptions()
        {
            // General
            VideoSource = "GDI grab";
            AudioSource = "None";
            VideoCodec = FFmpegVideoCodec.libx264;
            AudioCodec = FFmpegAudioCodec.libvorbis;
            Extension = "mp4";
            CLIPath = "ffmpeg.exe";
            UserArgs = "";

            // x264
            x264CRF = 23;
            Preset = FFmpegPreset.medium;

            // VPx
            VPxCRF = 12;

            // XviD
            qscale = 3;
        }

        public bool IsAudioSourceSelected()
        {
            return !string.IsNullOrEmpty(AudioSource) && !AudioSource.Equals("None", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}