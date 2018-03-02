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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public static class ZipManager
    {
        public static void Extract(string archivePath, string destination)
        {
            ZipFile.ExtractToDirectory(archivePath, destination);
        }

        public static void Extract(string archivePath, string destination, List<string> files)
        {
            using (ZipArchive archive = ZipFile.OpenRead(archivePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string entryName = entry.Name;

                    foreach (string file in files)
                    {
                        if (file.Equals(entryName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            entry.ExtractToFile(Path.Combine(destination, entryName));
                            break;
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
                    archive.CreateEntryFromFile(Path.Combine(workingDirectory, file), file, compression);
                }
            }
        }
    }
}