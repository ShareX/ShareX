#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

using HelpersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace ScreenCaptureLib
{
    public class ScreencastOptions
    {
        public string OutputPath;
        public int GIFFPS;
        public int ScreenRecordFPS;
        public Rectangle CaptureArea;
        public float Duration;
        public bool DrawCursor;

        public IntPtr ParentWindow;
        public bool ShowAVIOptionsDialog;
        public AVIOptions AVI = new AVIOptions();

        public FFmpegOptions FFmpeg = new FFmpegOptions();

        public string GetFFmpegArgs()
        {
            StringBuilder args = new StringBuilder();

            // http://ffmpeg.org/ffmpeg-devices.html#gdigrab
            args.AppendFormat("-f gdigrab -framerate {0} -offset_x {1} -offset_y {2} -video_size {3}x{4} -draw_mouse {5} -show_region {6} -i desktop ",
                ScreenRecordFPS, CaptureArea.X, CaptureArea.Y, CaptureArea.Width, CaptureArea.Height, DrawCursor ? 1 : 0, 0);

            if (!string.IsNullOrEmpty(FFmpeg.UserArgs))
            {
                args.Append(FFmpeg.UserArgs + " ");
            }

            args.AppendFormat("-c:v {0} ", FFmpeg.VideoCodec.ToString());

            // output FPS
            args.AppendFormat("-r {0} ", ScreenRecordFPS);

            switch (FFmpeg.VideoCodec)
            {
                case FFmpegVideoCodec.libx264: // https://trac.ffmpeg.org/wiki/x264EncodingGuide
                    args.AppendFormat("-crf {0} ", FFmpeg.x264CRF);
                    args.AppendFormat("-preset {0} ", FFmpeg.Preset.ToString());
                    break;
                case FFmpegVideoCodec.libvpx: // https://trac.ffmpeg.org/wiki/vpxEncodingGuide
                    args.AppendFormat("-crf {0} ", FFmpeg.x264CRF);
                    break;
                case FFmpegVideoCodec.libxvid: // https://trac.ffmpeg.org/wiki/How%20to%20encode%20Xvid%20/%20DivX%20video%20with%20ffmpeg
                    args.AppendFormat("-qscale:v {0} ", FFmpeg.qscale);
                    break;
            }

            // -pix_fmt yuv420p required for libx264 otherwise can't stream in Chrome
            if (FFmpeg.VideoCodec == FFmpegVideoCodec.libx264)
            {
                args.Append("-pix_fmt yuv420p ");
            }

            if (Duration > 0)
            {
                args.AppendFormat("-t {0} ", Duration);
            }

            // -y for overwrite file
            args.Append("-y ");

            args.AppendFormat("\"{0}\"", Path.ChangeExtension(OutputPath, FFmpeg.Extension));

            return args.ToString();
        }
    }
}