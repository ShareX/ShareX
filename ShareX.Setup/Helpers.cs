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
using System.Net;

namespace ShareX.Setup
{
    internal class Helpers
    {
        public static string DownloadFile(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                string filename = Path.GetFileName(url);
                webClient.DownloadFile(url, filename);
                return filename;
            }
        }

        public static void CopyFiles(string[] files, string toFolder)
        {
            if (!Directory.Exists(toFolder))
            {
                Directory.CreateDirectory(toFolder);
            }

            foreach (string filepath in files)
            {
                string filename = Path.GetFileName(filepath);
                string dest = Path.Combine(toFolder, filename);
                File.Copy(filepath, dest);
            }
        }

        public static void CopyFile(string path, string toFolder)
        {
            CopyFiles(new string[] { path }, toFolder);
        }

        public static void CopyFiles(string directory, string searchPattern, string toFolder)
        {
            CopyFiles(Directory.GetFiles(directory, searchPattern), toFolder);
        }

        public static void Zip(string source, string target)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = Program.ZipPath,
                Arguments = $"a -tzip \"{target}\" \"{source}\" -r -mx=9",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(startInfo).WaitForExit();
        }

        public static void Unzip(string source, string extract)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = Program.ZipPath,
                Arguments = $"e \"{source}\" \"{extract}\" -r",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(startInfo).WaitForExit();
        }

        public static bool CheckArguments(string[] args, string check)
        {
            if (!string.IsNullOrEmpty(check))
            {
                foreach (string arg in args)
                {
                    if (!string.IsNullOrEmpty(arg) && arg.Equals(check, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}