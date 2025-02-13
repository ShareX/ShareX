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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ShareX.HelpersLib
{
    public class SevenZipManager
    {
        public string SevenZipPath { get; set; }

        public SevenZipManager()
        {
            SevenZipPath = FileHelpers.GetAbsolutePath("7za.exe");
        }

        public SevenZipManager(string sevenZipPath)
        {
            SevenZipPath = sevenZipPath;
        }

        public bool Extract(string archivePath, string destination)
        {
            string arguments = $"x \"{archivePath}\" -o\"{destination}\" -y";
            return Run(arguments) == 0;
        }

        public bool Extract(string archivePath, string destination, List<string> files)
        {
            string fileArgs = string.Join(" ", files.Select(x => $"\"{x}\""));
            string arguments = $"e \"{archivePath}\" -o\"{destination}\" {fileArgs} -r -y";
            return Run(arguments) == 0;
        }

        public bool Compress(string archivePath, List<string> files, string workingDirectory = "")
        {
            if (File.Exists(archivePath))
            {
                File.Delete(archivePath);
            }

            string fileArgs = string.Join(" ", files.Select(x => $"\"{x}\""));
            string arguments = $"a -tzip \"{archivePath}\" {fileArgs} -mx=9";
            return Run(arguments, workingDirectory) == 0;
        }

        private int Run(string arguments, string workingDirectory = "")
        {
            using (Process process = new Process())
            {
                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = SevenZipPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                if (!string.IsNullOrEmpty(workingDirectory))
                {
                    psi.WorkingDirectory = workingDirectory;
                }

                process.StartInfo = psi;
                process.Start();
                process.WaitForExit();

                return process.ExitCode;
            }
        }
    }
}