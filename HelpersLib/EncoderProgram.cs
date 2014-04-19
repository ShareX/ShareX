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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace HelpersLib
{
    public class VideoEncoder
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Args { get; set; }
        public string OutputExtension { get; set; }

        public VideoEncoder()
        {
            Name = "x264 encoder to MP4";
            Path = "x264.exe";
            Args = "--output %output %input";
            OutputExtension = "mp4";
        }

        /// <param name="sourceFilePath">AVI file path</param>
        /// <param name="targetFilePath">Target file path without extension</param>
        public void Encode(string sourceFilePath, string targetFilePath)
        {
            if (IsValid() && !string.IsNullOrEmpty(sourceFilePath) && !string.IsNullOrEmpty(targetFilePath))
            {
                if (!targetFilePath.EndsWith(OutputExtension))
                {
                    targetFilePath += "." + OutputExtension.TrimStart('.');
                }

                Helpers.CreateDirectoryIfNotExist(targetFilePath);

                using (Process process = new Process())
                {
                    ProcessStartInfo psi = new ProcessStartInfo(Path);
                    psi.Arguments = Args.Replace("%input", "\"" + sourceFilePath + "\"").Replace("%output", "\"" + targetFilePath + "\"");
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo = psi;
                    process.Start();
                    process.WaitForExit();
                }
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Path) && File.Exists(Path) && !string.IsNullOrEmpty(OutputExtension);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}