#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using System.IO;
using System.IO.Compression;

namespace ShareX.HelpersLib
{
    public static class ZipManager
    {
        public static void Extract(string archivePath, string destination, bool retainDirectoryStructure = true, List<string> fileFilter = null)
        {
            using (ZipArchive archive = ZipFile.OpenRead(archivePath))
            {
                string fullName = Directory.CreateDirectory(Path.GetFullPath(destination)).FullName;

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string entryName = entry.Name;

                    if (fileFilter != null)
                    {
                        bool match = false;

                        foreach (string file in fileFilter)
                        {
                            if (file.Equals(entryName, StringComparison.OrdinalIgnoreCase))
                            {
                                match = true;
                                break;
                            }
                        }

                        if (!match)
                        {
                            continue;
                        }
                    }

                    if (retainDirectoryStructure)
                    {
                        entryName = entry.FullName;
                    }

                    string fullPath = Path.GetFullPath(Path.Combine(fullName, entryName));

                    if (fullPath.StartsWith(fullName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (Path.GetFileName(fullPath).Length == 0)
                        {
                            if (entry.Length == 0)
                            {
                                Directory.CreateDirectory(fullPath);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                            entry.ExtractToFile(fullPath, true);
                        }
                    }
                }
            }
        }

        public static void Compress(string source, string archivePath, CompressionLevel compression = CompressionLevel.Optimal)
        {
            if (File.Exists(archivePath))
            {
                File.Delete(archivePath);
            }

            ZipFile.CreateFromDirectory(source, archivePath, compression, false);
        }

        public static void Compress(string archivePath, List<string> files, string workingDirectory = "", CompressionLevel compression = CompressionLevel.Optimal)
        {
            if (File.Exists(archivePath))
            {
                File.Delete(archivePath);
            }

            using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
            {
                foreach (string file in files)
                {
                    string filePath = Path.Combine(workingDirectory, file);

                    if (File.Exists(filePath))
                    {
                        archive.CreateEntryFromFile(filePath, file, compression);
                    }
                }
            }
        }
    }
}