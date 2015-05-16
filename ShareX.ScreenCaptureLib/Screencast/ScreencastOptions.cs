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

using ShareX.HelpersLib;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace ShareX.ScreenCaptureLib
{
    public class ScreencastOptions
    {
        public ScreenRecordOutput OutputType { get; set; }
        public string OutputPath { get; set; }
        public int GIFFPS { get; set; }
        public int ScreenRecordFPS { get; set; }
        public Rectangle CaptureArea { get; set; }
        public float Duration { get; set; }
        public bool DrawCursor { get; set; }
        public FFmpegOptions FFmpeg { get; set; }

        public ScreencastOptions()
        {
            FFmpeg = new FFmpegOptions();
        }

        public string GetFFmpegCommands()
        {
            string commands;

            if (!string.IsNullOrEmpty(FFmpeg.VideoSource) && FFmpeg.VideoSource.Equals("screen-capture-recorder", StringComparison.InvariantCultureIgnoreCase))
            {
                // https://github.com/rdp/screen-capture-recorder-to-video-windows-free
                string registryPath = "Software\\screen-capture-recorder";
                RegistryHelpers.CreateRegistry(registryPath, "start_x", CaptureArea.X);
                RegistryHelpers.CreateRegistry(registryPath, "start_y", CaptureArea.Y);
                RegistryHelpers.CreateRegistry(registryPath, "capture_width", CaptureArea.Width);
                RegistryHelpers.CreateRegistry(registryPath, "capture_height", CaptureArea.Height);
                RegistryHelpers.CreateRegistry(registryPath, "default_max_fps", 60);
                RegistryHelpers.CreateRegistry(registryPath, "capture_mouse_default_1", DrawCursor ? 1 : 0);
            }

            if (FFmpeg.UseCustomCommands && !string.IsNullOrEmpty(FFmpeg.CustomCommands))
            {
                commands = FFmpeg.CustomCommands.
                    Replace("$fps$", ScreenRecordFPS.ToString(), StringComparison.InvariantCultureIgnoreCase).
                    Replace("$area_x$", CaptureArea.X.ToString(), StringComparison.InvariantCultureIgnoreCase).
                    Replace("$area_y$", CaptureArea.Y.ToString(), StringComparison.InvariantCultureIgnoreCase).
                    Replace("$area_width$", CaptureArea.Width.ToString(), StringComparison.InvariantCultureIgnoreCase).
                    Replace("$area_height$", CaptureArea.Height.ToString(), StringComparison.InvariantCultureIgnoreCase).
                    Replace("$cursor$", DrawCursor ? "1" : "0", StringComparison.InvariantCultureIgnoreCase).
                    Replace("$duration$", Duration.ToString("0.0", CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase).
                    Replace("$output$", Path.ChangeExtension(OutputPath, FFmpeg.Extension), StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                commands = GetFFmpegArgs();
            }

            return commands.Trim();
        }

        public string GetFFmpegArgs(bool isCustom = false)
        {
            if (!FFmpeg.IsVideoSourceSelected && !FFmpeg.IsAudioSourceSelected)
            {
                return null;
            }

            StringBuilder args = new StringBuilder();
            args.Append("-y "); // -y for overwrite file
            args.Append("-rtbufsize 100M "); // default real time buffer size was 3041280 (3M)

            string fps = isCustom ? "$fps$" : ScreenRecordFPS.ToString();

            if (FFmpeg.IsVideoSourceSelected)
            {
                if (FFmpeg.VideoSource.Equals(FFmpegHelper.SourceGDIGrab, StringComparison.InvariantCultureIgnoreCase))
                {
                    // http://ffmpeg.org/ffmpeg-devices.html#gdigrab
                    args.AppendFormat("-f gdigrab -framerate {0} -offset_x {1} -offset_y {2} -video_size {3}x{4} -draw_mouse {5} -i desktop ",
                        fps, isCustom ? "$area_x$" : CaptureArea.X.ToString(), isCustom ? "$area_y$" : CaptureArea.Y.ToString(),
                        isCustom ? "$area_width$" : CaptureArea.Width.ToString(), isCustom ? "$area_height$" : CaptureArea.Height.ToString(),
                        isCustom ? "$cursor$" : DrawCursor ? "1" : "0");

                    if (FFmpeg.IsAudioSourceSelected)
                    {
                        args.AppendFormat("-f dshow -i audio=\"{0}\" ", FFmpeg.AudioSource);
                    }
                }
                else
                {
                    args.AppendFormat("-f dshow -framerate {0} -i video=\"{1}\"", fps, FFmpeg.VideoSource);

                    if (FFmpeg.IsAudioSourceSelected)
                    {
                        args.AppendFormat(":audio=\"{0}\" ", FFmpeg.AudioSource);
                    }
                    else
                    {
                        args.Append(" ");
                    }
                }
            }
            else if (FFmpeg.IsAudioSourceSelected)
            {
                args.AppendFormat("-f dshow -i audio=\"{0}\" ", FFmpeg.AudioSource);
            }

            if (!string.IsNullOrEmpty(FFmpeg.UserArgs))
            {
                args.Append(FFmpeg.UserArgs + " ");
            }

            if (FFmpeg.IsVideoSourceSelected)
            {
                args.AppendFormat("-c:v {0} ", FFmpeg.VideoCodec);
                args.AppendFormat("-r {0} ", fps); // output FPS

                switch (FFmpeg.VideoCodec)
                {
                    case FFmpegVideoCodec.libx264: // https://trac.ffmpeg.org/wiki/Encode/H.264
                        args.AppendFormat("-crf {0} ", FFmpeg.x264_CRF);
                        args.AppendFormat("-preset {0} ", FFmpeg.Preset);
                        args.AppendFormat("-tune {0} ", "zerolatency");
                        args.Append("-pix_fmt yuv420p "); // -pix_fmt yuv420p required otherwise can't stream in Chrome
                        break;
                    case FFmpegVideoCodec.libvpx: // https://trac.ffmpeg.org/wiki/Encode/VP8
                        args.AppendFormat("-deadline {0} ", "realtime");
                        args.AppendFormat("-b:v {0}K ", FFmpeg.VPx_bitrate);
                        break;
                    case FFmpegVideoCodec.libxvid: // https://trac.ffmpeg.org/wiki/Encode/MPEG-4
                        args.AppendFormat("-qscale:v {0} ", FFmpeg.XviD_qscale);
                        break;
                }
            }

            if (FFmpeg.IsAudioSourceSelected)
            {
                switch (FFmpeg.AudioCodec)
                {
                    case FFmpegAudioCodec.libvoaacenc: // http://trac.ffmpeg.org/wiki/Encode/AAC
                        args.AppendFormat("-c:a libvo_aacenc -ac 2 -b:a {0}k ", FFmpeg.AAC_bitrate); // -ac 2 required otherwise failing with 7.1
                        break;
                    case FFmpegAudioCodec.libvorbis: // http://trac.ffmpeg.org/wiki/TheoraVorbisEncodingGuide
                        args.AppendFormat("-c:a {0} -qscale:a {1} ", FFmpegAudioCodec.libvorbis, FFmpeg.Vorbis_qscale);
                        break;
                    case FFmpegAudioCodec.libmp3lame: // http://trac.ffmpeg.org/wiki/Encode/MP3
                        args.AppendFormat("-c:a {0} -qscale:a {1} ", FFmpegAudioCodec.libmp3lame, FFmpeg.MP3_qscale);
                        break;
                }
            }

            if (Duration > 0)
            {
                args.AppendFormat("-t {0} ", isCustom ? "$duration$" : Duration.ToString("0.0", CultureInfo.InvariantCulture)); // duration limit
            }

            args.AppendFormat("\"{0}\"", isCustom ? "$output$" : Path.ChangeExtension(OutputPath, FFmpeg.Extension));

            return args.ToString();
        }
    }
}