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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShareX
{
    public static class CleanupManager
    {
        public static void Cleanup(int keepFileCount)
        {
            Task.Run(() =>
            {
                CleanupFolder(SettingManager.BackupFolder, "ApplicationConfig-*.json", keepFileCount);
                CleanupFolder(SettingManager.BackupFolder, "HotkeysConfig-*.json", keepFileCount);
                CleanupFolder(SettingManager.BackupFolder, "UploadersConfig-*.json", keepFileCount);
                CleanupFolder(SettingManager.BackupFolder, "History-*.json", keepFileCount);
                CleanupFolder(Program.LogsFolder, "ShareX-Log-*.txt", keepFileCount);
                CleanupTempFiles();
            });
        }

        private static void CleanupFolder(string folderPath, string fileNamePattern, int keepFileCount)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

            IEnumerable<FileInfo> files = directoryInfo.GetFiles(fileNamePattern).
                OrderByDescending(f => f.LastWriteTime.Year <= 1601 ? f.CreationTime : f.LastWriteTime).Skip(keepFileCount);

            foreach (FileInfo file in files)
            {
                file.Delete();
            }
        }

        private static void CleanupTempFiles()
        {
            try
            {
                string tempFolder = Path.GetTempPath();

                if (!string.IsNullOrEmpty(tempFolder))
                {
                    string folderPath = Path.Combine(tempFolder, "ShareX");

                    if (Directory.Exists(folderPath))
                    {
                        Directory.Delete(folderPath, true);

                        DebugHelper.WriteLine($"Temp files cleaned: {folderPath}");
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }
    }
}