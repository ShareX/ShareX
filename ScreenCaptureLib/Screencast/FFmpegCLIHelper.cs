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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenCaptureLib
{
    public class FFmpegCLIHelper : ExternalCLIManager
    {
        public ScreencastOptions Options { get; private set; }

        public FFmpegCLIHelper(ScreencastOptions options)
        {
            Options = options;

            if (string.IsNullOrEmpty(Options.FFmpegCLI.CLIPath))
            {
                Options.FFmpegCLI.CLIPath = Path.Combine(Application.StartupPath, "ffmpeg.exe");
            }

            Helpers.CreateDirectoryIfNotExist(Options.OutputPath);

            // It is actually output data
            ErrorDataReceived += FFmpegCLIHelper_OutputDataReceived;
        }

        private void FFmpegCLIHelper_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //DebugHelper.WriteLine(e.Data);
        }

        public bool Record(Rectangle captureRectangle)
        {
            // https://github.com/rdp/screen-capture-recorder-to-video-windows-free configuration section
            string dshowRegistryPath = "Software\\screen-capture-recorder";
            RegistryHelpers.CreateRegistry(dshowRegistryPath, "start_x", captureRectangle.X);
            RegistryHelpers.CreateRegistry(dshowRegistryPath, "start_y", captureRectangle.Y);
            RegistryHelpers.CreateRegistry(dshowRegistryPath, "capture_width", captureRectangle.Width);
            RegistryHelpers.CreateRegistry(dshowRegistryPath, "capture_height", captureRectangle.Height);
            RegistryHelpers.CreateRegistry(dshowRegistryPath, "default_max_fps", Options.FPS);

            StringBuilder args = new StringBuilder();

            // input FPS
            args.AppendFormat("-r {0} ", Options.FPS);

            args.Append("-f dshow -i ");

            // dshow audio/video device: https://github.com/rdp/screen-capture-recorder-to-video-windows-free
            //args.AppendFormat("audio=\"{0}\":", "virtual-audio-capturer");
            args.AppendFormat("video=\"{0}\" ", "screen-capture-recorder");

            // output FPS
            args.AppendFormat("-r {0} ", Options.FPS);

            args.AppendFormat("-c:v {0} ", Options.FFmpegCLI.VideoCodec.ToString());

            // https://trac.ffmpeg.org/wiki/x264EncodingGuide
            switch (Options.FFmpegCLI.VideoCodec)
            {
                case FFmpegVideoCodec.libx264:
                    args.AppendFormat("-crf {0} ", Options.FFmpegCLI.Quantizer);
                    args.AppendFormat("-preset {0} ", Options.FFmpegCLI.Preset.ToString());
                    break;
                case FFmpegVideoCodec.libvpx:
                    args.AppendFormat("-crf {0} ", Options.FFmpegCLI.Quantizer);
                    break;
                case FFmpegVideoCodec.libxvid:
                    args.AppendFormat("-qscale:v {0} ", Options.FFmpegCLI.Quantizer);
                    break;
                case FFmpegVideoCodec.mpeg4:
                    args.Append("-vtag xvid ");
                    args.AppendFormat("-qscale:v {0} ", Options.FFmpegCLI.Quantizer);
                    break;
            }

            // -pix_fmt yuv420p required otherwise can't stream in Chrome
            args.Append("-pix_fmt yuv420p ");

            if (Options.Duration > 0)
            {
                args.AppendFormat("-t {0} ", Options.Duration);
            }

            // -y for overwrite file
            args.AppendFormat("-y \"{0}\"", Path.ChangeExtension(Options.OutputPath, Options.FFmpegCLI.Extension));

            int result = Open(Options.FFmpegCLI.CLIPath, args.ToString());
            return result == 0;
        }

        public void ListDevices()
        {
            WriteInput("-list_devices true -f dshow -i dummy");
        }

        public override void Close()
        {
            WriteInput("q");
        }
    }
}