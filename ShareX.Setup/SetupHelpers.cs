#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace ShareX.Setup
{
    internal class SetupHelpers
    {
        public static string DownloadFile(string url)
        {
            string fileName = Path.GetFileName(url);
            string filePath = Path.GetFullPath(fileName);

            Console.WriteLine($"Downloading: \"{url}\" -> \"{filePath}\"");

            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.UserAgent, ShareXResources.UserAgent);
                wc.DownloadFile(url, filePath);
            }

            return filePath;
        }

        public static void CopyFile(string path, string toFolder)
        {
            CopyFiles(new string[] { path }, toFolder);
        }

        public static void CopyFiles(IEnumerable<string> files, string toFolder)
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

        public static void CopyFiles(string directory, string searchPattern, string toFolder, string[] ignoreFiles = null)
        {
            string[] files = Directory.GetFiles(directory, searchPattern);

            if (ignoreFiles != null)
            {
                List<string> newFiles = new List<string>();

                foreach (string file in files)
                {
                    string filename = Path.GetFileName(file);

                    if (ignoreFiles.All(x => !filename.Equals(x, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        newFiles.Add(file);
                    }
                }

                files = newFiles.ToArray();
            }

            CopyFiles(files, toFolder);
        }

        private static void ProcessStart(string filePath, string arguments)
        {
            Console.WriteLine($"Process starting: {filePath} {arguments}");

            using (Process process = new Process())
            {
                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = filePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process.StartInfo = psi;
                process.Start();
                process.WaitForExit();
            }
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