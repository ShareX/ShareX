#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.Linq;
using System.Collections.Generic;

namespace ShareX.Setup
{
    internal class Helpers
    {
        public static string DownloadFile(string url)
        {
            Console.WriteLine("Downloading: " + url);

            using (WebClient wc = new WebClient())
            {
                string filename = Path.GetFileName(url);
                wc.DownloadFile(url, filename);
                return filename;
            }
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

        public static void CopyAll(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (!Directory.Exists(target.FullName))
            {
                Directory.CreateDirectory(target.FullName);
            }

            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
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
            Console.WriteLine("Extracting: " + source);

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

        public static void CreateEmptyFile(string path)
        {
            File.Create(path).Dispose();
        }
    }
}