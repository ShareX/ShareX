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

using SevenZip;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ShareX
{
    public static class ExportImportManager
    {
        public static bool Export(string exportPath)
        {
            try
            {
                Set7ZipLibraryPath();

                SevenZipCompressor zip = new SevenZipCompressor();
                zip.ArchiveFormat = OutArchiveFormat.SevenZip;
                zip.CompressionLevel = CompressionLevel.Normal;
                zip.CompressionMethod = CompressionMethod.Lzma2;

                Dictionary<string, string> files = new Dictionary<string, string>();
                if (Program.Settings.ExportSettings)
                {
                    AddFileToDictionary(files, Program.ApplicationConfigFilePath);
                    AddFileToDictionary(files, Program.HotkeysConfigFilePath);
                    AddFileToDictionary(files, Program.UploadersConfigFilePath);
                    AddFileToDictionary(files, Program.GreenshotImageEditorConfigFilePath);
                }

                if (Program.Settings.ExportHistory)
                {
                    AddFileToDictionary(files, Program.HistoryFilePath);
                }

                if (Program.Settings.ExportLogs)
                {
                    foreach (string file in Directory.GetFiles(Program.LogsFolder, "*.txt", SearchOption.TopDirectoryOnly))
                    {
                        AddFileToDictionary(files, file, Path.GetFileName(Program.LogsFolder));
                    }
                }

                zip.CompressFileDictionary(files, exportPath);

                return true;
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                MessageBox.Show("Error while exporting backup:\r\n\r\n" + e, "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private static void AddFileToDictionary(Dictionary<string, string> files, string filePath, string subFolder = null)
        {
            if (File.Exists(filePath))
            {
                string destinationPath = Path.GetFileName(filePath);

                if (!string.IsNullOrEmpty(subFolder))
                {
                    destinationPath = Path.Combine(subFolder, destinationPath);
                }

                files.Add(destinationPath, filePath);
            }
        }

        public static bool Import(string importPath)
        {
            try
            {
                Set7ZipLibraryPath();

                using (SevenZipExtractor zip = new SevenZipExtractor(importPath))
                {
                    zip.ExtractArchive(Program.PersonalFolder);

                    return true;
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                MessageBox.Show("Error while importing backup:\r\n\r\n" + e, "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private static void Set7ZipLibraryPath()
        {
            if (NativeMethods.Is64Bit())
            {
                SevenZipBase.SetLibraryPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z-x64.dll"));
            }
            else
            {
                SevenZipBase.SetLibraryPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll"));
            }
        }
    }
}