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
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
using ShareX.UploadersLib.FileUploaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ShareX
{
    internal static class SettingManager
    {
        private static string ApplicationConfigFilePath
        {
            get
            {
                if (Program.Sandbox) return null;

                return Path.Combine(Program.PersonalFolder, "ApplicationConfig.json");
            }
        }

        private static string UploadersConfigFilePath
        {
            get
            {
                if (Program.Sandbox) return null;

                string uploadersConfigFolder;

                if (Settings != null && !string.IsNullOrEmpty(Settings.CustomUploadersConfigPath))
                {
                    uploadersConfigFolder = Helpers.ExpandFolderVariables(Settings.CustomUploadersConfigPath);
                }
                else
                {
                    uploadersConfigFolder = Program.PersonalFolder;
                }

                return Path.Combine(uploadersConfigFolder, "UploadersConfig.json");
            }
        }

        private static string HotkeysConfigFilePath
        {
            get
            {
                if (Program.Sandbox) return null;

                string hotkeysConfigFolder;

                if (Settings != null && !string.IsNullOrEmpty(Settings.CustomHotkeysConfigPath))
                {
                    hotkeysConfigFolder = Helpers.ExpandFolderVariables(Settings.CustomHotkeysConfigPath);
                }
                else
                {
                    hotkeysConfigFolder = Program.PersonalFolder;
                }

                return Path.Combine(hotkeysConfigFolder, "HotkeysConfig.json");
            }
        }

        private static string BackupFolder => Path.Combine(Program.PersonalFolder, "Backup");

        private static string GreenshotImageEditorConfigFilePath => Path.Combine(Program.PersonalFolder, "GreenshotImageEditor.ini");

        private static ApplicationConfig Settings { get => Program.Settings; set => Program.Settings = value; }
        private static TaskSettings DefaultTaskSettings { get => Program.DefaultTaskSettings; set => Program.DefaultTaskSettings = value; }
        private static UploadersConfig UploadersConfig { get => Program.UploadersConfig; set => Program.UploadersConfig = value; }
        private static HotkeysConfig HotkeysConfig { get => Program.HotkeysConfig; set => Program.HotkeysConfig = value; }

        private static ManualResetEvent uploadersConfigResetEvent = new ManualResetEvent(false);
        private static ManualResetEvent hotkeysConfigResetEvent = new ManualResetEvent(false);

        public static void LoadInitialSettings()
        {
            LoadApplicationConfig();

            ApplicationConfigBackwardCompatibilityTasks();

            TaskEx.Run(() =>
            {
                LoadUploadersConfig();
                uploadersConfigResetEvent.Set();

                UploadersConfigBackwardCompatibilityTasks();

                LoadHotkeysConfig();
                hotkeysConfigResetEvent.Set();
            });
        }

        public static void WaitUploadersConfig()
        {
            if (UploadersConfig == null)
            {
                uploadersConfigResetEvent.WaitOne();
            }
        }

        public static void WaitHotkeysConfig()
        {
            if (HotkeysConfig == null)
            {
                hotkeysConfigResetEvent.WaitOne();
            }
        }

        public static void LoadApplicationConfig()
        {
            Settings = ApplicationConfig.Load(ApplicationConfigFilePath);
            DefaultTaskSettings = Settings.DefaultTaskSettings;
        }

        public static void LoadUploadersConfig()
        {
            UploadersConfig = UploadersConfig.Load(UploadersConfigFilePath);
        }

        public static void LoadHotkeysConfig()
        {
            HotkeysConfig = HotkeysConfig.Load(HotkeysConfigFilePath);
        }

        public static void LoadAllSettings()
        {
            LoadApplicationConfig();
            LoadUploadersConfig();
            LoadHotkeysConfig();
        }

        private static void ApplicationConfigBackwardCompatibilityTasks()
        {
            if (Settings.IsUpgradeFrom("11.4.1"))
            {
                RegionCaptureOptions regionCaptureOptions = DefaultTaskSettings.CaptureSettings.SurfaceOptions;
                regionCaptureOptions.AnnotationOptions = new AnnotationOptions();
                regionCaptureOptions.LastRegionTool = ShapeType.RegionRectangle;
                regionCaptureOptions.LastAnnotationTool = ShapeType.DrawingRectangle;
            }

            if (Settings.IsUpgradeFrom("11.5.0"))
            {
                if (File.Exists(Program.ChromeHostManifestFilePath))
                {
                    IntegrationHelpers.CreateChromeExtensionSupport(true);
                }
            }
        }

        private static void UploadersConfigBackwardCompatibilityTasks()
        {
            if (UploadersConfig.IsUpgradeFrom("11.6.0"))
            {
                if (UploadersConfig.DropboxURLType == DropboxURLType.Direct)
                {
                    UploadersConfig.DropboxUseDirectLink = true;
                }

                if (!string.IsNullOrEmpty(UploadersConfig.AmazonS3Settings.Endpoint))
                {
                    bool endpointFound = false;

                    foreach (AmazonS3Endpoint endpoint in AmazonS3.Endpoints)
                    {
                        if (endpoint.Region.Equals(UploadersConfig.AmazonS3Settings.Endpoint, StringComparison.InvariantCultureIgnoreCase))
                        {
                            UploadersConfig.AmazonS3Settings.Endpoint = endpoint.Endpoint;
                            UploadersConfig.AmazonS3Settings.Region = endpoint.Region;
                            endpointFound = true;
                            break;
                        }
                    }

                    if (!endpointFound)
                    {
                        UploadersConfig.AmazonS3Settings.Endpoint = "";
                    }
                }
            }
        }

        public static void SaveAllSettings()
        {
            if (Settings != null) Settings.Save(ApplicationConfigFilePath);
            if (UploadersConfig != null) UploadersConfig.Save(UploadersConfigFilePath);
            if (HotkeysConfig != null) HotkeysConfig.Save(HotkeysConfigFilePath);
        }

        public static void SaveApplicationConfigAsync()
        {
            if (Settings != null) Settings.SaveAsync(ApplicationConfigFilePath);
        }

        public static void SaveUploadersConfigAsync()
        {
            if (UploadersConfig != null) UploadersConfig.SaveAsync(UploadersConfigFilePath);
        }

        public static void SaveHotkeysConfigAsync()
        {
            if (HotkeysConfig != null) HotkeysConfig.SaveAsync(HotkeysConfigFilePath);
        }

        public static void SaveAllSettingsAsync()
        {
            SaveApplicationConfigAsync();
            SaveUploadersConfigAsync();
            SaveHotkeysConfigAsync();
        }

        public static void BackupSettings()
        {
            Helpers.BackupFileWeekly(ApplicationConfigFilePath, BackupFolder);
            Helpers.BackupFileWeekly(UploadersConfigFilePath, BackupFolder);
            Helpers.BackupFileWeekly(HotkeysConfigFilePath, BackupFolder);
            Helpers.BackupFileWeekly(Program.HistoryFilePath, BackupFolder);
        }

        public static void ResetSettings()
        {
            Settings = new ApplicationConfig();
            DefaultTaskSettings = Settings.DefaultTaskSettings;
            UploadersConfig = new UploadersConfig();
            HotkeysConfig = new HotkeysConfig();

            if (File.Exists(GreenshotImageEditorConfigFilePath))
            {
                File.Delete(GreenshotImageEditorConfigFilePath);
            }
        }

        public static bool Export(string exportPath)
        {
            try
            {
                Set7ZipLibraryPath();

                SevenZipCompressor zip = new SevenZipCompressor()
                {
                    ArchiveFormat = OutArchiveFormat.SevenZip,
                    CompressionLevel = CompressionLevel.Normal,
                    CompressionMethod = CompressionMethod.Lzma2
                };

                Dictionary<string, string> files = new Dictionary<string, string>();

                if (Settings.ExportSettings)
                {
                    AddFileToDictionary(files, ApplicationConfigFilePath);
                    AddFileToDictionary(files, HotkeysConfigFilePath);
                    AddFileToDictionary(files, UploadersConfigFilePath);
                    AddFileToDictionary(files, GreenshotImageEditorConfigFilePath);
                }

                if (Settings.ExportHistory)
                {
                    AddFileToDictionary(files, Program.HistoryFilePath);
                }

                if (Settings.ExportLogs)
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
    }
}