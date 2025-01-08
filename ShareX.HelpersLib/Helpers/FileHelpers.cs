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

using Microsoft.VisualBasic.FileIO;
using ShareX.HelpersLib.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static class FileHelpers
    {
        public static readonly string[] ImageFileExtensions = new string[] { "jpg", "jpeg", "png", "gif", "bmp", "ico", "tif", "tiff" };
        public static readonly string[] TextFileExtensions = new string[] { "txt", "log", "nfo", "c", "cpp", "cc", "cxx", "h", "hpp", "hxx", "cs", "vb",
            "html", "htm", "xhtml", "xht", "xml", "css", "js", "php", "bat", "java", "lua", "py", "pl", "cfg", "ini", "dart", "go", "gohtml" };
        public static readonly string[] VideoFileExtensions = new string[] { "mp4", "webm", "mkv", "avi", "vob", "ogv", "ogg", "mov", "qt", "wmv", "m4p",
            "m4v", "mpg", "mp2", "mpeg", "mpe", "mpv", "m2v", "m4v", "flv", "f4v" };

        public static string GetFileNameExtension(string filePath, bool includeDot = false, bool checkSecondExtension = true)
        {
            string extension = "";

            if (!string.IsNullOrEmpty(filePath))
            {
                int pos = filePath.LastIndexOf('.');

                if (pos >= 0)
                {
                    extension = filePath.Substring(pos + 1);

                    if (checkSecondExtension)
                    {
                        filePath = filePath.Remove(pos);
                        string extension2 = GetFileNameExtension(filePath, false, false);

                        if (!string.IsNullOrEmpty(extension2))
                        {
                            foreach (string knownExtension in new string[] { "tar" })
                            {
                                if (extension2.Equals(knownExtension, StringComparison.OrdinalIgnoreCase))
                                {
                                    extension = extension2 + "." + extension;
                                    break;
                                }
                            }
                        }
                    }

                    if (includeDot)
                    {
                        extension = "." + extension;
                    }
                }
            }

            return extension;
        }

        public static string GetFileNameSafe(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                int pos = filePath.LastIndexOf('\\');

                if (pos < 0)
                {
                    pos = filePath.LastIndexOf('/');
                }

                if (pos >= 0)
                {
                    return filePath.Substring(pos + 1);
                }
            }

            return filePath;
        }

        public static string ChangeFileNameExtension(string fileName, string extension)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                int pos = fileName.LastIndexOf('.');

                if (pos >= 0)
                {
                    fileName = fileName.Remove(pos);
                }

                if (!string.IsNullOrEmpty(extension))
                {
                    pos = extension.LastIndexOf('.');

                    if (pos >= 0)
                    {
                        extension = extension.Substring(pos + 1);
                    }

                    return fileName + "." + extension;
                }
            }

            return fileName;
        }

        public static string AppendTextToFileName(string filePath, string text)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                int pos = filePath.LastIndexOf('.');

                if (pos >= 0)
                {
                    return filePath.Substring(0, pos) + text + filePath.Substring(pos);
                }
            }

            return filePath + text;
        }

        public static string AppendExtension(string filePath, string extension)
        {
            return filePath.TrimEnd('.') + '.' + extension.TrimStart('.');
        }

        public static bool CheckExtension(string filePath, IEnumerable<string> extensions)
        {
            string ext = GetFileNameExtension(filePath);

            if (!string.IsNullOrEmpty(ext))
            {
                return extensions.Any(x => ext.Equals(x, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        public static bool IsImageFile(string filePath)
        {
            return CheckExtension(filePath, ImageFileExtensions);
        }

        public static bool IsTextFile(string filePath)
        {
            return CheckExtension(filePath, TextFileExtensions);
        }

        public static bool IsVideoFile(string filePath)
        {
            return CheckExtension(filePath, VideoFileExtensions);
        }

        public static EDataType FindDataType(string filePath)
        {
            if (IsImageFile(filePath))
            {
                return EDataType.Image;
            }

            if (IsTextFile(filePath))
            {
                return EDataType.Text;
            }

            return EDataType.File;
        }

        public static string GetAbsolutePath(string path)
        {
            path = ExpandFolderVariables(path);

            if (!Path.IsPathRooted(path)) // Is relative path?
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }

            return Path.GetFullPath(path);
        }

        public static string GetPathRoot(string path)
        {
            int separator = path.IndexOf(":\\");

            if (separator > 0)
            {
                return path.Substring(0, separator + 2);
            }

            return "";
        }

        public static string SanitizeFileName(string fileName, string replaceWith = "")
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return SanitizeFileName(fileName, replaceWith, invalidChars);
        }

        private static string SanitizeFileName(string fileName, string replaceWith, char[] invalidChars)
        {
            fileName = fileName.Trim();

            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c.ToString(), replaceWith);
            }

            return fileName;
        }

        public static string SanitizePath(string path, string replaceWith = "")
        {
            string root = GetPathRoot(path);

            if (!string.IsNullOrEmpty(root))
            {
                path = path.Substring(root.Length);
            }

            char[] invalidChars = Path.GetInvalidFileNameChars().Except(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }).ToArray();
            path = SanitizeFileName(path, replaceWith, invalidChars);

            return root + path;
        }

        public static bool OpenFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                    using (Process process = new Process())
                    {
                        ProcessStartInfo psi = new ProcessStartInfo()
                        {
                            FileName = filePath
                        };

                        process.StartInfo = psi;
                        process.Start();
                    }

                    DebugHelper.WriteLine("File opened: " + filePath);

                    return true;
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, $"OpenFile({filePath}) failed.");
                }
            }
            else
            {
                MessageBox.Show(Resources.Helpers_OpenFile_File_not_exist_ + Environment.NewLine + filePath, "ShareX",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return false;
        }

        public static bool OpenFolder(string folderPath, bool allowMessageBox = true)
        {
            if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
            {
                if (!folderPath.EndsWith(@"\"))
                {
                    folderPath += @"\";
                }

                try
                {
                    using (Process process = new Process())
                    {
                        ProcessStartInfo psi = new ProcessStartInfo()
                        {
                            FileName = folderPath
                        };

                        process.StartInfo = psi;
                        process.Start();
                    }

                    DebugHelper.WriteLine("Folder opened: " + folderPath);

                    return true;
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, $"OpenFolder({folderPath}) failed.");
                }
            }
            else if (allowMessageBox)
            {
                MessageBox.Show(Resources.Helpers_OpenFolder_Folder_not_exist_ + Environment.NewLine + folderPath, "ShareX",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return false;
        }

        public static bool OpenFolderWithFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                    NativeMethods.OpenFolderAndSelectFile(filePath);

                    DebugHelper.WriteLine("Folder opened with file: " + filePath);

                    return true;
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, $"OpenFolderWithFile({filePath}) failed.");
                }
            }
            else
            {
                MessageBox.Show(Resources.Helpers_OpenFile_File_not_exist_ + Environment.NewLine + filePath, "ShareX",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return false;
        }

        public static string GetUniqueFilePath(string filePath)
        {
            if (File.Exists(filePath))
            {
                string folderPath = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string fileExtension = Path.GetExtension(filePath);
                int number = 1;

                Match regex = Regex.Match(fileName, @"^(.+) \((\d+)\)$");

                if (regex.Success)
                {
                    fileName = regex.Groups[1].Value;
                    number = int.Parse(regex.Groups[2].Value);
                }

                do
                {
                    number++;
                    string newFileName = $"{fileName} ({number}){fileExtension}";
                    filePath = Path.Combine(folderPath, newFileName);
                }
                while (File.Exists(filePath));
            }

            return filePath;
        }

        public static bool BrowseFile(TextBox tb, string initialDirectory = "", bool detectSpecialFolders = false, string filter = "")
        {
            return BrowseFile("ShareX - " + Resources.Helpers_BrowseFile_Choose_file, tb, initialDirectory, detectSpecialFolders, filter);
        }

        public static bool BrowseFile(string title, TextBox tb, string initialDirectory = "", bool detectSpecialFolders = false, string filter = "")
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = title;
                ofd.Filter = filter;

                try
                {
                    string path = tb.Text;

                    if (detectSpecialFolders)
                    {
                        path = ExpandFolderVariables(path);
                    }

                    if (!string.IsNullOrEmpty(path))
                    {
                        path = Path.GetDirectoryName(path);

                        if (Directory.Exists(path))
                        {
                            ofd.InitialDirectory = path;
                        }
                    }
                }
                finally
                {
                    if (string.IsNullOrEmpty(ofd.InitialDirectory) && !string.IsNullOrEmpty(initialDirectory))
                    {
                        ofd.InitialDirectory = initialDirectory;
                    }
                }

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = ofd.FileName;

                    if (detectSpecialFolders)
                    {
                        fileName = GetVariableFolderPath(fileName);
                    }

                    tb.Text = fileName;

                    return true;
                }
            }

            return false;
        }

        public static bool BrowseFolder(TextBox tb, string initialDirectory = "", bool detectSpecialFolders = false)
        {
            return BrowseFolder("ShareX - " + Resources.Helpers_BrowseFolder_Choose_folder, tb, initialDirectory, detectSpecialFolders);
        }

        public static bool BrowseFolder(string title, TextBox tb, string initialDirectory = "", bool detectSpecialFolders = false)
        {
            using (FolderSelectDialog fsd = new FolderSelectDialog())
            {
                fsd.Title = title;

                string path = tb.Text;

                if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                {
                    fsd.InitialDirectory = path;
                }
                else if (!string.IsNullOrEmpty(initialDirectory))
                {
                    fsd.InitialDirectory = initialDirectory;
                }

                if (fsd.ShowDialog())
                {
                    tb.Text = detectSpecialFolders ? GetVariableFolderPath(fsd.FileName) : fsd.FileName;
                    return true;
                }
            }

            return false;
        }

        public static string GetVariableFolderPath(string path, bool supportCustomSpecialFolders = false)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    if (supportCustomSpecialFolders)
                    {
                        foreach (KeyValuePair<string, string> specialFolder in HelpersOptions.ShareXSpecialFolders)
                        {
                            path = path.Replace(specialFolder.Value, $"%{specialFolder.Key}%", StringComparison.OrdinalIgnoreCase);
                        }
                    }

                    foreach (Environment.SpecialFolder specialFolder in Helpers.GetEnums<Environment.SpecialFolder>())
                    {
                        path = path.Replace(Environment.GetFolderPath(specialFolder), $"%{specialFolder}%", StringComparison.OrdinalIgnoreCase);
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }

            return path;
        }

        public static string ExpandFolderVariables(string path, bool supportCustomSpecialFolders = false)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    if (supportCustomSpecialFolders)
                    {
                        foreach (KeyValuePair<string, string> specialFolder in HelpersOptions.ShareXSpecialFolders)
                        {
                            path = path.Replace($"%{specialFolder.Key}%", specialFolder.Value, StringComparison.OrdinalIgnoreCase);
                        }
                    }

                    foreach (Environment.SpecialFolder specialFolder in Helpers.GetEnums<Environment.SpecialFolder>())
                    {
                        path = path.Replace($"%{specialFolder}%", Environment.GetFolderPath(specialFolder), StringComparison.OrdinalIgnoreCase);
                    }

                    path = Environment.ExpandEnvironmentVariables(path);
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }

            return path;
        }

        public static string OutputSpecialFolders()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Environment.SpecialFolder specialFolder in Helpers.GetEnums<Environment.SpecialFolder>())
            {
                sb.AppendLine(string.Format("{0,-25}{1}", specialFolder, Environment.GetFolderPath(specialFolder)));
            }

            return sb.ToString();
        }

        public static bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    fs.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }

        public static long GetFileSize(string filePath)
        {
            try
            {
                return new FileInfo(filePath).Length;
            }
            catch
            {
            }

            return -1;
        }

        public static string GetFileSizeReadable(string filePath, bool binaryUnits = false)
        {
            long fileSize = GetFileSize(filePath);

            if (fileSize >= 0)
            {
                return fileSize.ToSizeString(binaryUnits);
            }

            return "";
        }

        public static void CreateDirectory(string directoryPath)
        {
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                    MessageBox.Show(Resources.Helpers_CreateDirectoryIfNotExist_Create_failed_ + "\r\n\r\n" + e, "ShareX - " + Resources.Error,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void CreateDirectoryFromFilePath(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string directoryPath = Path.GetDirectoryName(filePath);
                CreateDirectory(directoryPath);
            }
        }

        public static bool IsValidFilePath(string filePath)
        {
            FileInfo fi = null;

            try
            {
                fi = new FileInfo(filePath);
            }
            catch (ArgumentException) { }
            catch (PathTooLongException) { }
            catch (NotSupportedException) { }

            return fi != null;
        }

        public static string CopyFile(string filePath, string destinationFolder, bool overwrite = true)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath) && !string.IsNullOrEmpty(destinationFolder))
            {
                string fileName = Path.GetFileName(filePath);
                string destinationFilePath = Path.Combine(destinationFolder, fileName);
                CreateDirectory(destinationFolder);
                File.Copy(filePath, destinationFilePath, overwrite);
                return destinationFilePath;
            }

            return null;
        }

        public static void CopyFiles(string filePath, string destinationFolder)
        {
            CopyFiles(new string[] { filePath }, destinationFolder);
        }

        public static void CopyFiles(string[] files, string destinationFolder)
        {
            if (files != null && files.Length > 0)
            {
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileName(filePath);
                    string destinationFilePath = Path.Combine(destinationFolder, fileName);
                    File.Copy(filePath, destinationFilePath);
                }
            }
        }

        public static void CopyFiles(string sourceFolder, string destinationFolder, string searchPattern = "*", string[] ignoreFiles = null)
        {
            string[] files = Directory.GetFiles(sourceFolder, searchPattern);

            if (ignoreFiles != null)
            {
                List<string> newFiles = new List<string>();

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);

                    if (ignoreFiles.All(x => !fileName.Equals(x, StringComparison.OrdinalIgnoreCase)))
                    {
                        newFiles.Add(file);
                    }
                }

                files = newFiles.ToArray();
            }

            CopyFiles(files, destinationFolder);
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

        public static string MoveFile(string filePath, string destinationFolder, bool overwrite = true)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath) && !string.IsNullOrEmpty(destinationFolder))
            {
                string fileName = Path.GetFileName(filePath);
                string destinationFilePath = Path.Combine(destinationFolder, fileName);
                CreateDirectory(destinationFolder);

                if (overwrite && File.Exists(destinationFilePath))
                {
                    File.Delete(destinationFilePath);
                }

                File.Move(filePath, destinationFilePath);
                return destinationFilePath;
            }

            return null;
        }

        public static string RenameFile(string filePath, string newFileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    string directory = Path.GetDirectoryName(filePath);
                    string newFilePath = Path.Combine(directory, newFileName);
                    File.Move(filePath, newFilePath);
                    return newFilePath;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Rename file error:\r\n" + e.ToString(), "ShareX - " + Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return filePath;
        }

        public static bool DeleteFile(string filePath, bool sendToRecycleBin = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    if (sendToRecycleBin)
                    {
                        FileSystem.DeleteFile(filePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                    else
                    {
                        File.Delete(filePath);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        public static string BackupFileWeekly(string filePath, string destinationFolder)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                DateTime dateTime = DateTime.Now;
                string extension = Path.GetExtension(filePath);
                string newFileName = string.Format("{0}-{1:yyyy-MM}-W{2:00}{3}", fileName, dateTime, dateTime.WeekOfYear(), extension);
                string newFilePath = Path.Combine(destinationFolder, newFileName);

                if (!File.Exists(newFilePath))
                {
                    CreateDirectory(destinationFolder);
                    File.Copy(filePath, newFilePath, false);
                    return newFilePath;
                }
            }

            return null;
        }

        public static void BackupFileMonthly(string filePath, string destinationFolder)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string extension = Path.GetExtension(filePath);
                string newFileName = string.Format("{0}-{1:yyyy-MM}{2}", fileName, DateTime.Now, extension);
                string newFilePath = Path.Combine(destinationFolder, newFileName);

                if (!File.Exists(newFilePath))
                {
                    CreateDirectory(destinationFolder);
                    File.Copy(filePath, newFilePath, false);
                }
            }
        }

        public static string GetTempFilePath(string extension)
        {
            string path = Path.GetTempFileName();
            return Path.ChangeExtension(path, extension);
        }

        public static void CreateEmptyFile(string filePath)
        {
            File.Create(filePath).Dispose();
        }

        public static IEnumerable<string> GetFilesByExtensions(string directoryPath, params string[] extensions)
        {
            return GetFilesByExtensions(new DirectoryInfo(directoryPath), extensions);
        }

        public static IEnumerable<string> GetFilesByExtensions(DirectoryInfo directoryInfo, params string[] extensions)
        {
            HashSet<string> allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);
            return directoryInfo.EnumerateFiles().Where(f => allowedExtensions.Contains(f.Extension)).Select(x => x.FullName);
        }
    }
}