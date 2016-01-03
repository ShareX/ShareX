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

using System;
using System.Diagnostics;
using System.IO;

namespace ShareX.HelpersLib
{
    public class ExternalProgram
    {
        public bool IsActive { get; set; }
        public bool HiddenWindow { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Args { get; set; }
        public string Extensions { get; set; }
        public string OutputExtension { get; set; }

        public ExternalProgram()
        {
            IsActive = false;
            Args = "%input";
        }

        public ExternalProgram(string name, string path)
            : this()
        {
            Name = name;
            Path = path;
        }

        public ExternalProgram(string name, string path, string args)
            : this(name, path)
        {
            if (!string.IsNullOrEmpty(args))
            {
                Args += " " + args;
            }
        }

        public string Run(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && CheckExtensions(filePath) && !string.IsNullOrEmpty(Path) && File.Exists(Path))
            {
                filePath = filePath.Trim('"');

                try
                {
                    string newFilePath = "";

                    using (Process process = new Process())
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(Path);

                        if (string.IsNullOrEmpty(Args))
                        {
                            psi.Arguments = '"' + filePath + '"';
                        }
                        else
                        {
                            string args = Args.Replace("%filepath%", '"' + filePath + '"').Replace("%input", '"' + filePath + '"');

                            if (!string.IsNullOrEmpty(OutputExtension))
                            {
                                newFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath), System.IO.Path.GetFileNameWithoutExtension(filePath));

                                if (!OutputExtension.Contains("."))
                                {
                                    OutputExtension = "." + OutputExtension;
                                }

                                newFilePath += OutputExtension;
                                args = args.Replace("%output", '"' + newFilePath + '"');
                            }

                            psi.Arguments = args;
                        }

                        if (HiddenWindow)
                        {
                            psi.WindowStyle = ProcessWindowStyle.Hidden;
                            psi.CreateNoWindow = true;
                        }

                        process.StartInfo = psi;

                        DebugHelper.WriteLine(string.Format("Running {0} with arguments: {1}", Path, psi.Arguments));

                        process.Start();
                        process.WaitForExit();
                    }

                    if (!string.IsNullOrEmpty(newFilePath) && File.Exists(newFilePath))
                    {
                        return newFilePath;
                    }

                    return filePath;
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }

            return filePath;
        }

        private bool CheckExtensions(string path)
        {
            if (string.IsNullOrEmpty(Extensions) || string.IsNullOrEmpty(path)) return true;
            int idx = 0;
            for (int i = 0; i <= Extensions.Length; ++i)
            {
                if (i == Extensions.Length || !char.IsLetterOrDigit(Extensions[i]))
                {
                    if (idx < i && path.EndsWith(Extensions.Substring(idx, i - idx))) return true;
                    idx = i + 1;
                }
            }
            return false;
        }
    }
}