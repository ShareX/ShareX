#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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
using System.IO;
using System.Linq;

namespace ShareX.Setup
{
    internal class SetupHelpers
    {
        public static string DownloadFile(string url)
        {
            string fileName = Path.GetFileName(url);
            string filePath = Path.GetFullPath(fileName);

            Console.WriteLine($"Downloading: \"{url}\" -> \"{filePath}\"");

            URLHelpers.DownloadFile(url, filePath);

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

            foreach (string filePath in files)
            {
                string fileName = Path.GetFileName(filePath);
                string dest = Path.Combine(toFolder, fileName);
                File.Copy(filePath, dest);
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
                    string fileName = Path.GetFileName(file);

                    if (ignoreFiles.All(x => !fileName.Equals(x, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        newFiles.Add(file);
                    }
                }

                files = newFiles.ToArray();
            }

            CopyFiles(files, toFolder);
        }
    }
}