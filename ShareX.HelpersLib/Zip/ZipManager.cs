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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ShareX.HelpersLib
{
    public static class ZipManager
    {
        public static void Extract(string archivePath, string destination, bool retainDirectoryStructure = true, Func<ZipArchiveEntry, bool> filter = null,
            long maxUncompressedSize = 0)
        {
            using (ZipArchive archive = ZipFile.OpenRead(archivePath))
            {
                if (maxUncompressedSize > 0)
                {
                    long totalUncompressedSize = archive.Entries.Sum(entry => entry.Length);

                    if (totalUncompressedSize > maxUncompressedSize)
                    {
                        throw new Exception("Uncompressed file size of this archive is bigger than the maximum allowed file size.\r\n\r\n" +
                            $"Archive uncompressed file size: {totalUncompressedSize.ToSizeString()}\r\n" +
                            $"Maximum allowed file size: {maxUncompressedSize.ToSizeString()}");
                    }
                }

                string fullName = Directory.CreateDirectory(Path.GetFullPath(destination)).FullName;

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (filter != null && !filter(entry))
                    {
                        continue;
                    }

                    string entryName;

                    if (retainDirectoryStructure)
                    {
                        entryName = entry.FullName;
                    }
                    else
                    {
                        entryName = entry.Name;
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
                            ExtractToFile(entry, fullPath, true);
                        }
                    }
                }
            }
        }

        private static void ExtractToFile(ZipArchiveEntry source, string destinationFileName, bool overwrite)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destinationFileName == null)
            {
                throw new ArgumentNullException(nameof(destinationFileName));
            }

            FileMode fMode = overwrite ? FileMode.Create : FileMode.CreateNew;

            using (FileStream fs = new FileStream(destinationFileName, fMode, FileAccess.Write, FileShare.None, bufferSize: 0x1000, useAsync: false))
            using (Stream es = source.Open())
            using (MaxLengthStream maxLengthStream = new MaxLengthStream(es, source.Length))
            {
                maxLengthStream.CopyTo(fs);
            }

            File.SetLastWriteTime(destinationFileName, source.LastWriteTime.DateTime);
        }

        public static void Compress(string source, string archivePath, CompressionLevel compression = CompressionLevel.Optimal)
        {
            if (File.Exists(archivePath))
            {
                File.Delete(archivePath);
            }

            ZipFile.CreateFromDirectory(source, archivePath, compression, false);
        }

        public static void Compress(string archivePath, List<ZipEntryInfo> entries, CompressionLevel compression = CompressionLevel.Optimal)
        {
            FileHelpers.CreateDirectoryFromFilePath(archivePath);

            if (File.Exists(archivePath))
            {
                File.Delete(archivePath);
            }

            using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
            {
                foreach (ZipEntryInfo entry in entries)
                {
                    archive.CreateEntry(entry, compression);
                }
            }
        }

        private static ZipArchiveEntry CreateEntry(this ZipArchive archive, ZipEntryInfo entryInfo, CompressionLevel compressionLevel)
        {
            if (entryInfo == null)
            {
                throw new ArgumentNullException(nameof(entryInfo));
            }

            if (entryInfo.Data != null)
            {
                using (entryInfo.Data)
                {
                    return archive.CreateEntryFromStream(entryInfo.Data, entryInfo.EntryName, compressionLevel);
                }
            }
            else if (!string.IsNullOrEmpty(entryInfo.SourcePath) && File.Exists(entryInfo.SourcePath))
            {
                return archive.CreateEntryFromFile(entryInfo.SourcePath, entryInfo.EntryName, compressionLevel);
            }

            return null;
        }

        private static ZipArchiveEntry CreateEntryFromStream(this ZipArchive archive, Stream stream, string entryName, CompressionLevel compressionLevel)
        {
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (entryName == null)
            {
                throw new ArgumentNullException(nameof(entryName));
            }

            ZipArchiveEntry entry = archive.CreateEntry(entryName, compressionLevel);
            entry.LastWriteTime = DateTime.Now;

            using (Stream entryStream = entry.Open())
            {
                stream.Position = 0;
                stream.CopyTo(entryStream);
            }

            return entry;
        }
    }
}