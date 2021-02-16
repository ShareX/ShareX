#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using ShareX.MediaLib;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace ShareX.ScreenCaptureLib
{
    public class ScreencastOptions
    {
        public bool IsRecording { get; set; }
        public bool IsLossless { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public int FPS { get; set; }
        public Rectangle CaptureArea { get; set; }
        public float Duration { get; set; }
        public bool DrawCursor { get; set; }
        public FFmpegOptions FFmpeg { get; set; } = new FFmpegOptions();

        public string GetFFmpegCommands()
        {
            string commands;

            if (IsRecording && !string.IsNullOrEmpty(FFmpeg.VideoSource) && FFmpeg.VideoSource.Equals("screen-capture-recorder", StringComparison.InvariantCultureIgnoreCase))
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

            if (!IsLossless && FFmpeg.UseCustomCommands && !string.IsNullOrEmpty(FFmpeg.CustomCommands))
            {
                commands = FFmpeg.CustomCommands.
                    Replace("$fps$", FPS.ToString(), StringComparison.InvariantCultureIgnoreCase).
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
            if (IsRecording && !FFmpeg.IsVideoSourceSelected && !FFmpeg.IsAudioSourceSelected)
            {
                return null;
            }

            StringBuilder args = new StringBuilder();
            args.Append("-rtbufsize 150M "); // default real time buffer size was 3041280 (3M)

            string fps;

            if (isCustom)
            {
                fps = "$fps$";
            }
            else
            {
                fps = FPS.ToString();
            }

            if (IsRecording)
            {
                if (FFmpeg.IsVideoSourceSelected)
                {
                    if (FFmpeg.VideoSource.Equals(FFmpegCLIManager.SourceGDIGrab, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // http://ffmpeg.org/ffmpeg-devices.html#gdigrab
                        args.AppendFormat("-f gdigrab -framerate {0} -offset_x {1} -offset_y {2} -video_size {3}x{4} -draw_mouse {5} -i desktop ",
                            fps, isCustom ? "$area_x$" : CaptureArea.X.ToString(), isCustom ? "$area_y$" : CaptureArea.Y.ToString(),
                            isCustom ? "$area_width$" : CaptureArea.Width.ToString(), isCustom ? "$area_height$" : CaptureArea.Height.ToString(),
                            isCustom ? "$cursor$" : DrawCursor ? "1" : "0");

                        if (FFmpeg.IsAudioSourceSelected)
                        {
                            args.AppendFormat("-f dshow -i audio={0} ", Helpers.EscapeCLIText(FFmpeg.AudioSource));
                        }
                    }
                    else
                    {
                        args.AppendFormat("-f dshow -framerate {0} -i video={1}", fps, Helpers.EscapeCLIText(FFmpeg.VideoSource));

                        if (FFmpeg.IsAudioSourceSelected)
                        {
                            args.AppendFormat(":audio={0} ", Helpers.EscapeCLIText(FFmpeg.AudioSource));
                        }
                        else
                        {
                            args.Append(" ");
                        }
                    }
                }
                else if (FFmpeg.IsAudioSourceSelected)
                {
                    args.AppendFormat("-f dshow -i audio={0} ", Helpers.EscapeCLIText(FFmpeg.AudioSource));
                }
            }
            else
            {
                args.Append($"-i \"{InputPath}\" ");
            }

            if (!string.IsNullOrEmpty(FFmpeg.UserArgs))
            {
                args.Append(FFmpeg.UserArgs + " ");
            }

            if (FFmpeg.IsVideoSourceSelected)
            {
                if (IsLossless || FFmpeg.VideoCodec != FFmpegVideoCodec.apng)
                {
                    string videoCodec;

                    if (IsLossless)
                    {
                        videoCodec = FFmpegVideoCodec.libx264.ToString();
                    }
                    else if (FFmpeg.VideoCodec == FFmpegVideoCodec.libvpx_vp9)
                    {
                        videoCodec = "libvpx-vp9";
                    }
                    else
                    {
                        videoCodec = FFmpeg.VideoCodec.ToString();
                    }

                    args.AppendFormat("-c:v {0} ", videoCodec);
                    args.AppendFormat("-r {0} ", fps); // output FPS
                }

                if (IsLossless)
                {
                    args.AppendFormat("-preset {0} ", FFmpegPreset.ultrafast);
                    args.AppendFormat("-tune {0} ", FFmpegTune.zerolatency);
                    args.AppendFormat("-qp {0} ", 0);
                }
                else
                {
                    switch (FFmpeg.VideoCodec)
                    {
                        case FFmpegVideoCodec.libx264: // https://trac.ffmpeg.org/wiki/Encode/H.264
                        case FFmpegVideoCodec.libx265: // https://trac.ffmpeg.org/wiki/Encode/H.265
                            args.AppendFormat("-preset {0} ", FFmpeg.x264_Preset);
                            if (IsRecording) args.AppendFormat("-tune {0} ", FFmpegTune.zerolatency);
                            args.AppendFormat("-crf {0} ", FFmpeg.x264_CRF);
                            args.AppendFormat("-pix_fmt {0} ", "yuv420p"); // -pix_fmt yuv420p required otherwise can't stream in Chrome
                            args.AppendFormat("-movflags {0} ", "+faststart"); // This will move some information to the beginning of your file and allow the video to begin playing before it is completely downloaded by the viewer
                            break;
                        case FFmpegVideoCodec.libvpx: // https://trac.ffmpeg.org/wiki/Encode/VP8
                        case FFmpegVideoCodec.libvpx_vp9: // https://trac.ffmpeg.org/wiki/Encode/VP9
                            if (IsRecording) args.AppendFormat("-deadline {0} ", "realtime");
                            args.AppendFormat("-b:v {0}k ", FFmpeg.VPx_bitrate);
                            args.AppendFormat("-pix_fmt {0} ", "yuv420p"); // -pix_fmt yuv420p required otherwise causing issues in Chrome related to WebM transparency support
                            break;
                        case FFmpegVideoCodec.libxvid: // https://trac.ffmpeg.org/wiki/Encode/MPEG-4
                            args.AppendFormat("-qscale:v {0} ", FFmpeg.XviD_qscale);
                            break;
                        case FFmpegVideoCodec.h264_nvenc: // https://trac.ffmpeg.org/wiki/HWAccelIntro#NVENC
                        case FFmpegVideoCodec.hevc_nvenc:
                            args.AppendFormat("-preset {0} ", FFmpeg.NVENC_preset);
                            args.AppendFormat("-b:v {0}k ", FFmpeg.NVENC_bitrate);
                            args.AppendFormat("-pix_fmt {0} ", "yuv420p");
                            break;
                        case FFmpegVideoCodec.h264_amf:
                        case FFmpegVideoCodec.hevc_amf:
                            args.AppendFormat("-usage {0} ", FFmpeg.AMF_usage);
                            args.AppendFormat("-quality {0} ", FFmpeg.AMF_quality);
                            args.AppendFormat("-pix_fmt {0} ", "yuv420p");
                            break;
                        case FFmpegVideoCodec.h264_qsv: // https://trac.ffmpeg.org/wiki/Hardware/QuickSync
                        case FFmpegVideoCodec.hevc_qsv:
                            args.AppendFormat("-preset {0} ", FFmpeg.QSV_preset);
                            args.AppendFormat("-b:v {0}k ", FFmpeg.QSV_bitrate);
                            break;
                        case FFmpegVideoCodec.libwebp: // https://www.ffmpeg.org/ffmpeg-codecs.html#libwebp
                            args.AppendFormat("-lossless {0} ", "0");
                            args.AppendFormat("-preset {0} ", "default");
                            args.AppendFormat("-loop {0} ", "0");
                            break;
                        case FFmpegVideoCodec.apng:
                            args.Append("-f apng ");
                            args.AppendFormat("-plays {0} ", "0");
                            break;
                    }
                }
            }

            if (FFmpeg.IsAudioSourceSelected)
            {
                switch (FFmpeg.AudioCodec)
                {
                    case FFmpegAudioCodec.libvoaacenc: // http://trac.ffmpeg.org/wiki/Encode/AAC
                        args.AppendFormat("-c:a aac -ac 2 -b:a {0}k ", FFmpeg.AAC_bitrate); // -ac 2 required otherwise failing with 7.1
                        break;
                    case FFmpegAudioCodec.libopus: // https://www.ffmpeg.org/ffmpeg-codecs.html#libopus-1
                        args.AppendFormat("-c:a libopus -b:a {0}k ", FFmpeg.Opus_bitrate);
                        break;
                    case FFmpegAudioCodec.libvorbis: // http://trac.ffmpeg.org/wiki/TheoraVorbisEncodingGuide
                        args.AppendFormat("-c:a libvorbis -qscale:a {0} ", FFmpeg.Vorbis_qscale);
                        break;
                    case FFmpegAudioCodec.libmp3lame: // http://trac.ffmpeg.org/wiki/Encode/MP3
                        args.AppendFormat("-c:a libmp3lame -qscale:a {0} ", FFmpeg.MP3_qscale);
                        break;
                }
            }

            if (Duration > 0)
            {
                args.AppendFormat("-t {0} ", isCustom ? "$duration$" : Duration.ToString("0.0", CultureInfo.InvariantCulture)); // duration limit
            }

            args.Append("-y "); // overwrite file

            string output;

            if (isCustom)
            {
                output = "$output$";
            }
            else
            {
                output = Path.ChangeExtension(OutputPath, IsLossless ? "mp4" : FFmpeg.Extension);
            }

            args.AppendFormat("\"{0}\"", output);

            return args.ToString();
        }
    }
}