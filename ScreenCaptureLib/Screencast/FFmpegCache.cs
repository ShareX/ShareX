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

using AForge.Video.FFMPEG;
using HelpersLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenCaptureLib
{
    public class FFmpegCache : ImageCache
    {
        private static readonly string[] ffmpegFiles = new string[] { "avcodec-53.dll", "avdevice-53.dll", "avfilter-2.dll", "avformat-53.dll", "avutil-51.dll", "swresample-0.dll", "swscale-2.dll" };

        private VideoFileWriter ffmpegWriter;

        public FFmpegCache(ScreencastOptions options)
        {
            Options = options;
            Helpers.CreateDirectoryIfNotExist(Options.OutputPath);
            ffmpegWriter = new VideoFileWriter();
            ffmpegWriter.Open(options.OutputPath, options.Size.Width, options.Size.Height, options.FPS, (AForge.Video.FFMPEG.VideoCodec)options.FFmpeg.VideoCodec, options.FFmpeg.BitRate * 1000);
        }

        protected override void WriteFrame(Image img)
        {
            ffmpegWriter.WriteVideoFrame((Bitmap)img);
        }

        public override void Dispose()
        {
            if (ffmpegWriter != null)
            {
                ffmpegWriter.Dispose();
            }

            base.Dispose();
        }

        public static bool HasDependencies()
        {
            return ffmpegFiles.Select(x => Path.Combine(Application.StartupPath, x)).All(x => File.Exists(x));
        }
    }
}