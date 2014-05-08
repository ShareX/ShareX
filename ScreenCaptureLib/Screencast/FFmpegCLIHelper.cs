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
        public string FileName = "ffmpeg.exe";
        public ScreencastOptions Options { get; private set; }

        public FFmpegCLIHelper(ScreencastOptions options)
        {
            Options = options;

            if (string.IsNullOrEmpty(options.FFmpeg.CLIPath))
            {
                options.FFmpeg.CLIPath = Path.Combine(Application.StartupPath, FileName);
            }

            Helpers.CreateDirectoryIfNotExist(Options.OutputPath);

            this.ErrorDataReceived += Close;
        }

        public override void Record()
        {
            StringBuilder args = new StringBuilder();
            args.Append("-f dshow -i video=\"screen-capture-recorder\"");
            if (Options.FPS > 0)
            {
                args.Append(string.Format(" -r {0}", Options.FPS));
            }
            args.Append(string.Format(" -c:v libx264 -crf 23 -preset medium -pix_fmt yuv420p -y \"{0}\"", Options.OutputPath));

            Open(Options.FFmpeg.CLIPath, args.ToString());
        }

        public void ListDevices()
        {
            SendCommand("-list_devices true -f dshow -i dummy");
        }

        public override void Close()
        {
            SendCommand("q");
        }
    }
}