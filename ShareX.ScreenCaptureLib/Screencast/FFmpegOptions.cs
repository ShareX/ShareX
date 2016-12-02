#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

using ShareX.HelpersLib;
using System;

namespace ShareX.ScreenCaptureLib
{
    public class FFmpegOptions
    {
        // General
        public bool OverrideCLIPath { get; set; } = false;
        public string CLIPath { get; set; } = "";
        public string VideoSource { get; set; } = FFmpegHelper.SourceGDIGrab;
        public string AudioSource { get; set; } = FFmpegHelper.SourceNone;
        public FFmpegVideoCodec VideoCodec { get; set; } = FFmpegVideoCodec.libx264;
        public FFmpegAudioCodec AudioCodec { get; set; } = FFmpegAudioCodec.libvoaacenc;
        public string UserArgs { get; set; } = "";
        public bool UseCustomCommands { get; set; } = false;
        public string CustomCommands { get; set; } = "";
        public bool ShowError { get; set; } = true;

        // Video
        public FFmpegPreset x264_Preset { get; set; } = FFmpegPreset.ultrafast;
        public int x264_CRF { get; set; } = 28;
        public int VPx_bitrate { get; set; } = 3000; // kbit/s
        public int XviD_qscale { get; set; } = 10;
        public FFmpegNVENCPreset NVENC_preset { get; set; } = FFmpegNVENCPreset.llhp;
        public int NVENC_bitrate { get; set; } = 3000; // kbit/s
        public FFmpegPaletteGenStatsMode GIFStatsMode { get; set; } = FFmpegPaletteGenStatsMode.full;
        public FFmpegPaletteUseDither GIFDither { get; set; } = FFmpegPaletteUseDither.sierra2_4a;

        // Audio
        public int AAC_bitrate { get; set; } = 128; // kbit/s
        public int Vorbis_qscale { get; set; } = 3;
        public int MP3_qscale { get; set; } = 4;

        public string FFmpegPath
        {
            get
            {
#if STEAM
                if (!OverrideCLIPath)
                {
                    if (NativeMethods.Is64Bit())
                    {
                        return Helpers.GetAbsolutePath("ffmpeg-x64.exe");
                    }
                    else
                    {
                        return Helpers.GetAbsolutePath("ffmpeg.exe");
                    }
                }
#endif

                if (!string.IsNullOrEmpty(CLIPath))
                {
                    return Helpers.GetAbsolutePath(CLIPath);
                }

                return "";
            }
        }

        public string Extension
        {
            get
            {
                if (!VideoSource.Equals(FFmpegHelper.SourceNone, StringComparison.InvariantCultureIgnoreCase))
                {
                    switch (VideoCodec)
                    {
                        case FFmpegVideoCodec.libx264:
                        case FFmpegVideoCodec.libx265:
                        case FFmpegVideoCodec.h264_nvenc:
                        case FFmpegVideoCodec.hevc_nvenc:
                        case FFmpegVideoCodec.gif:
                            return "mp4";
                        case FFmpegVideoCodec.libvpx:
                            return "webm";
                        case FFmpegVideoCodec.libxvid:
                            return "avi";
                    }
                }
                else if (!AudioSource.Equals(FFmpegHelper.SourceNone, StringComparison.InvariantCultureIgnoreCase))
                {
                    switch (AudioCodec)
                    {
                        case FFmpegAudioCodec.libvoaacenc:
                            return "m4a";
                        case FFmpegAudioCodec.libvorbis:
                            return "ogg";
                        case FFmpegAudioCodec.libmp3lame:
                            return "mp3";
                    }
                }

                return "mp4";
            }
        }

        public bool IsSourceSelected => IsVideoSourceSelected || IsAudioSourceSelected;

        public bool IsVideoSourceSelected => !string.IsNullOrEmpty(VideoSource) && !VideoSource.Equals(FFmpegHelper.SourceNone, StringComparison.InvariantCultureIgnoreCase);

        public bool IsAudioSourceSelected => !string.IsNullOrEmpty(AudioSource) && !AudioSource.Equals(FFmpegHelper.SourceNone, StringComparison.InvariantCultureIgnoreCase) &&
            (!IsVideoSourceSelected || VideoCodec != FFmpegVideoCodec.gif);

        public FFmpegOptions()
        {
        }

        public FFmpegOptions(string ffmpegPath)
        {
            CLIPath = Helpers.GetVariableFolderPath(ffmpegPath);
        }
    }
}