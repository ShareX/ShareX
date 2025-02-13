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

using ShareX.HelpersLib;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ShareX.MediaLib
{
    public class VideoConverterOptions
    {
        public string InputFilePath { get; set; }
        public string OutputFolderPath { get; set; }
        public string OutputFileName { get; set; }

        public string OutputFilePath
        {
            get
            {
                string path = "";

                if (!string.IsNullOrEmpty(OutputFolderPath) && !string.IsNullOrEmpty(OutputFileName))
                {
                    path = Path.Combine(OutputFolderPath, OutputFileName);

                    if (!Path.HasExtension(OutputFileName))
                    {
                        string extension = GetFileExtension();
                        path = Path.ChangeExtension(path, extension);
                    }
                }

                return path;
            }
        }

        private static readonly string[] AnimationOnlyFiles = new string[] { "gif", "webp", "png", "apng" };

        public bool IsInputFileAnimationOnly
        {
            get
            {
                return AnimationOnlyFiles.Any(x => InputFilePath.EndsWith("." + x, StringComparison.OrdinalIgnoreCase));
            }
        }

        public ConverterVideoCodecs VideoCodec { get; set; } = ConverterVideoCodecs.x264;
        public int VideoQuality { get; set; } = 23;
        public bool VideoQualityUseBitrate { get; set; } = false;
        public int VideoQualityBitrate { get; set; } = 3000;

        public bool UseCustomArguments { get; set; } = false;
        public string CustomArguments { get; set; } = "";

        public bool AutoOpenFolder { get; set; } = true;

        public string Arguments
        {
            get
            {
                if (UseCustomArguments)
                {
                    return CustomArguments;
                }

                return GetFFmpegArgs();
            }
        }

        public string GetFFmpegArgs()
        {
            StringBuilder args = new StringBuilder();

            // Input file path
            args.Append($"-i \"{InputFilePath}\" ");

            // Video encoder
            switch (VideoCodec)
            {
                case ConverterVideoCodecs.x264: // https://trac.ffmpeg.org/wiki/Encode/H.264
                    args.Append("-c:v libx264 ");
                    args.Append("-preset medium ");
                    if (VideoQualityUseBitrate)
                    {
                        args.Append($"-b:v {VideoQualityBitrate}k ");
                    }
                    else
                    {
                        args.Append($"-crf {VideoQuality.Clamp(FFmpegCLIManager.x264_min, FFmpegCLIManager.x264_max)} ");
                    }
                    args.Append("-pix_fmt yuv420p ");
                    args.Append("-movflags +faststart ");
                    break;
                case ConverterVideoCodecs.x265: // https://trac.ffmpeg.org/wiki/Encode/H.265
                    args.Append("-c:v libx265 ");
                    args.Append("-preset medium ");
                    if (VideoQualityUseBitrate)
                    {
                        args.Append($"-b:v {VideoQualityBitrate}k ");
                    }
                    else
                    {
                        args.Append($"-crf {VideoQuality.Clamp(FFmpegCLIManager.x265_min, FFmpegCLIManager.x265_max)} ");
                    }
                    break;
                case ConverterVideoCodecs.h264_nvenc: // https://trac.ffmpeg.org/wiki/HWAccelIntro#NVENC
                    args.Append("-c:v h264_nvenc ");
                    args.Append("-preset p4 ");
                    args.Append("-tune hq ");
                    args.Append("-profile:v high ");
                    args.Append($"-b:v {VideoQualityBitrate}k ");
                    break;
                case ConverterVideoCodecs.hevc_nvenc: // https://trac.ffmpeg.org/wiki/HWAccelIntro#NVENC
                    args.Append("-c:v hevc_nvenc ");
                    args.Append("-preset p4 ");
                    args.Append("-tune hq ");
                    args.Append("-profile:v main ");
                    args.Append($"-b:v {VideoQualityBitrate}k ");
                    break;
                case ConverterVideoCodecs.h264_amf:
                    args.Append("-c:v h264_amf ");
                    args.Append("-usage transcoding ");
                    args.Append("-profile main ");
                    args.Append("-quality balanced ");
                    args.Append($"-b:v {VideoQualityBitrate}k ");
                    break;
                case ConverterVideoCodecs.hevc_amf:
                    args.Append("-c:v hevc_amf ");
                    args.Append("-usage transcoding ");
                    args.Append("-profile main ");
                    args.Append("-quality balanced ");
                    args.Append($"-b:v {VideoQualityBitrate}k ");
                    break;
                case ConverterVideoCodecs.h264_qsv: // https://trac.ffmpeg.org/wiki/Hardware/QuickSync
                    args.Append("-c:v h264_qsv ");
                    args.Append("-preset medium ");
                    args.Append($"-b:v {VideoQualityBitrate}k ");
                    break;
                case ConverterVideoCodecs.hevc_qsv: // https://trac.ffmpeg.org/wiki/Hardware/QuickSync
                    args.Append("-c:v hevc_qsv ");
                    args.Append("-preset medium ");
                    args.Append($"-b:v {VideoQualityBitrate}k ");
                    break;
                case ConverterVideoCodecs.vp8: // https://trac.ffmpeg.org/wiki/Encode/VP8
                    args.Append("-c:v libvpx ");
                    if (VideoQualityUseBitrate)
                    {
                        args.Append($"-b:v {VideoQualityBitrate}k ");
                    }
                    else
                    {
                        args.Append($"-crf {VideoQuality.Clamp(FFmpegCLIManager.vp8_min, FFmpegCLIManager.vp8_max)} ");
                        args.Append("-b:v 100M ");
                    }
                    break;
                case ConverterVideoCodecs.vp9: // https://trac.ffmpeg.org/wiki/Encode/VP9
                    args.Append("-c:v libvpx-vp9 ");
                    if (VideoQualityUseBitrate)
                    {
                        args.Append($"-b:v {VideoQualityBitrate}k ");
                    }
                    else
                    {
                        args.Append($"-crf {VideoQuality.Clamp(FFmpegCLIManager.vp9_min, FFmpegCLIManager.vp9_max)} ");
                        args.Append("-b:v 0 ");
                    }
                    break;
                case ConverterVideoCodecs.av1: // https://trac.ffmpeg.org/wiki/Encode/AV1
                    args.Append("-c:v libsvtav1 ");
                    if (VideoQualityUseBitrate)
                    {
                        args.Append($"-b:v {VideoQualityBitrate}k ");
                    }
                    else
                    {
                        args.Append($"-crf {VideoQuality.Clamp(FFmpegCLIManager.av1_min, FFmpegCLIManager.av1_max)} ");
                    }
                    break;
                case ConverterVideoCodecs.xvid: // https://trac.ffmpeg.org/wiki/Encode/MPEG-4
                    args.Append("-c:v libxvid ");
                    if (VideoQualityUseBitrate)
                    {
                        args.Append($"-b:v {VideoQualityBitrate}k ");
                    }
                    else
                    {
                        args.Append($"-q:v {VideoQuality.Clamp(FFmpegCLIManager.xvid_min, FFmpegCLIManager.xvid_max)} ");
                    }
                    break;
                case ConverterVideoCodecs.gif: // https://ffmpeg.org/ffmpeg-filters.html#palettegen-1
                    args.Append("-lavfi \"palettegen=stats_mode=full[palette],[0:v][palette]paletteuse=dither=sierra2_4a\" ");
                    break;
                case ConverterVideoCodecs.webp: // https://www.ffmpeg.org/ffmpeg-codecs.html#libwebp
                    args.Append("-c:v libwebp ");
                    args.Append("-lossless 0 ");
                    args.Append("-preset default ");
                    args.Append("-loop 0 ");
                    break;
                case ConverterVideoCodecs.apng:
                    args.Append("-f apng ");
                    args.Append("-plays 0 ");
                    break;
            }

            if (!IsInputFileAnimationOnly)
            {
                // Audio encoder
                switch (VideoCodec)
                {
                    case ConverterVideoCodecs.x264: // https://trac.ffmpeg.org/wiki/Encode/AAC
                    case ConverterVideoCodecs.x265:
                    case ConverterVideoCodecs.h264_nvenc:
                    case ConverterVideoCodecs.hevc_nvenc:
                    case ConverterVideoCodecs.h264_amf:
                    case ConverterVideoCodecs.hevc_amf:
                    case ConverterVideoCodecs.h264_qsv:
                    case ConverterVideoCodecs.hevc_qsv:
                        args.Append("-c:a aac ");
                        args.Append("-b:a 128k ");
                        break;
                    case ConverterVideoCodecs.vp8: // https://trac.ffmpeg.org/wiki/TheoraVorbisEncodingGuide
                    case ConverterVideoCodecs.vp9:
                        args.Append("-c:a libvorbis ");
                        args.Append("-q:a 3 ");
                        break;
                    case ConverterVideoCodecs.av1: // https://ffmpeg.org/ffmpeg-codecs.html#libopus-1
                        args.Append("-c:a libopus ");
                        args.Append("-b:a 128k ");
                        break;
                    case ConverterVideoCodecs.xvid: // https://trac.ffmpeg.org/wiki/Encode/MP3
                        args.Append("-c:a libmp3lame ");
                        args.Append("-q:a 4 ");
                        break;
                }
            }

            // Overwrite output files without asking
            args.Append($"-y ");

            // Output file path
            args.Append($"\"{OutputFilePath}\"");

            return args.ToString();
        }

        public string GetFileExtension()
        {
            switch (VideoCodec)
            {
                default:
                case ConverterVideoCodecs.x264:
                case ConverterVideoCodecs.x265:
                case ConverterVideoCodecs.h264_nvenc:
                case ConverterVideoCodecs.hevc_nvenc:
                case ConverterVideoCodecs.h264_amf:
                case ConverterVideoCodecs.hevc_amf:
                case ConverterVideoCodecs.h264_qsv:
                case ConverterVideoCodecs.hevc_qsv:
                    return "mp4";
                case ConverterVideoCodecs.vp8:
                case ConverterVideoCodecs.vp9:
                    return "webm";
                case ConverterVideoCodecs.av1:
                    return "mkv";
                case ConverterVideoCodecs.xvid:
                    return "avi";
                case ConverterVideoCodecs.gif:
                    return "gif";
                case ConverterVideoCodecs.webp:
                    return "webp";
                case ConverterVideoCodecs.apng:
                    return "apng";
            }
        }
    }
}