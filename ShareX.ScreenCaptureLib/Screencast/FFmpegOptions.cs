#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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

namespace ShareX.ScreenCaptureLib
{
    public class FFmpegOptions
    {
        // General
        public string VideoSource { get; set; }
        public string AudioSource { get; set; }
        public FFmpegVideoCodec VideoCodec { get; set; }
        public FFmpegAudioCodec AudioCodec { get; set; }
        public string CLIPath { get; set; }
        public string UserArgs { get; set; }
        public bool UseCustomCommands { get; set; }
        public string CustomCommands { get; set; }
        public bool ShowError { get; set; }

        // Video
        public FFmpegPreset x264_Preset { get; set; }
        public int x264_CRF { get; set; }
        public int VPx_bitrate { get; set; }  // kbit/s
        public int XviD_qscale { get; set; }
        public FFmpegPaletteGenStatsMode GIFStatsMode { get; set; }
        public FFmpegPaletteUseDither GIFDither { get; set; }

        // Audio
        public int AAC_bitrate { get; set; }  // kbit/s
        public int Vorbis_qscale { get; set; }
        public int MP3_qscale { get; set; }

        public string Extension
        {
            get
            {
                switch (VideoCodec)
                {
                    default:
                    case FFmpegVideoCodec.libx264:
                    case FFmpegVideoCodec.libx265:
                    case FFmpegVideoCodec.gif:
                        return "mp4";
                        break;
                    case FFmpegVideoCodec.libvpx:
                        return "webm";
                    case FFmpegVideoCodec.libxvid:
                        return "avi";
                }
            }
        }

        public bool IsSourceSelected
        {
            get
            {
                return IsVideoSourceSelected || IsAudioSourceSelected;
            }
        }

        public bool IsVideoSourceSelected
        {
            get
            {
                return !string.IsNullOrEmpty(VideoSource) && !VideoSource.Equals(FFmpegHelper.SourceNone, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public bool IsAudioSourceSelected
        {
            get
            {
                return !string.IsNullOrEmpty(AudioSource) && !AudioSource.Equals(FFmpegHelper.SourceNone, StringComparison.InvariantCultureIgnoreCase) && VideoCodec != FFmpegVideoCodec.gif;
            }
        }

        public FFmpegOptions()
        {
            // General
            VideoSource = FFmpegHelper.SourceGDIGrab;
            AudioSource = FFmpegHelper.SourceNone;
            VideoCodec = FFmpegVideoCodec.libx264;
            AudioCodec = FFmpegAudioCodec.libvoaacenc;
            CLIPath = "ffmpeg.exe";
            UserArgs = "";
            ShowError = true;

            // Video
            x264_CRF = 30;
            x264_Preset = FFmpegPreset.veryfast;
            VPx_bitrate = 3000;
            XviD_qscale = 10;
            GIFStatsMode = FFmpegPaletteGenStatsMode.full;
            GIFDither = FFmpegPaletteUseDither.sierra2_4a;

            // Audio
            AAC_bitrate = 128;
            Vorbis_qscale = 3;
            MP3_qscale = 4;
        }
    }
}