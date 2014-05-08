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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenCaptureLib
{
    public class FFmpegCLIHelper : CLIEncoder
    {
        public ScreencastOptions Options { get; private set; }

        public FFmpegCLIHelper(ScreencastOptions options)
        {
            Options = options;

            if (string.IsNullOrEmpty(Options.FFmpeg.CLIPath))
            {
                Options.FFmpeg.CLIPath = Path.Combine(Application.StartupPath, "ffmpeg.exe");
            }

            Helpers.CreateDirectoryIfNotExist(Options.OutputPath);

            // It is actually output data
            ErrorDataReceived += FFmpegCLIHelper_ErrorDataReceived;
        }

        private void FFmpegCLIHelper_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //DebugHelper.WriteLine(e.Data);
        }

        public override bool Record()
        {
            StringBuilder args = new StringBuilder();
            args.Append("-f dshow -i video=\"screen-capture-recorder\"");
            if (Options.FPS > 0)
            {
                args.Append(string.Format(" -r {0}", Options.FPS));
            }
            args.Append(string.Format(" -c:v libx264 -crf 23 -preset medium -pix_fmt yuv420p -y \"{0}\"", Options.OutputPath));

            int result = Open(Options.FFmpeg.CLIPath, args.ToString());
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