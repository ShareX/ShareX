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
    public static class SettingManager
    {
        public static string ApplicationConfigFilePath
        {
            get
            {
                if (Program.Sandbox) return null;

                return Path.Combine(Program.PersonalFolder, "ApplicationConfig.json");
            }
        }

        public static string UploadersConfigFilePath
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

        public static string HotkeysConfigFilePath
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

        public static string HistoryFilePath
        {
            get
            {
                if (Program.Sandbox) return null;

                return Path.Combine(Program.PersonalFolder, "History.xml");
            }
        }

        public static string BackupFolder => Path.Combine(Program.PersonalFolder, "Backup");

        public static string GreenshotImageEditorConfigFilePath => Path.Combine(Program.PersonalFolder, "GreenshotImageEditor.ini");

        private static ApplicationConfig Settings { get => Program.Settings; set => Program.Settings = value; }
        private static TaskSettings DefaultTaskSettings { get => Program.DefaultTaskSettings; set => Program.DefaultTaskSettings = value; }
        private static UploadersConfig UploadersConfig { get => Program.UploadersConfig; set => Program.UploadersConfig = value; }
        private static HotkeysConfig HotkeysConfig { get => Program.HotkeysConfig; set => Program.HotkeysConfig = value; }

        private static ManualResetEvent uploadersConfigResetEvent = new ManualResetEvent(false);
        private static ManualResetEvent hotkeysConfigResetEvent = new ManualResetEvent(false);
        private static FileSystemWatcher uploaderConfigWatcher;
        private static WatchFolderDuplicateEventTimer uploaderConfigWatcherTimer;

        public static void LoadSettings()
        {
            LoadUploadersConfig();
            uploadersConfigResetEvent.Set();

            RunUploaderBackwardCompatibilityTasks();

            LoadHotkeySettings();
            hotkeysConfigResetEvent.Set();

            ConfigureUploadersConfigWatcher();
        }

        public static void WaitUploadersConfig()
        {
            if (Program.UploadersConfig == null)
            {
                uploadersConfigResetEvent.WaitOne();
            }
        }

        public static void WaitHotkeysConfig()
        {
            if (Program.HotkeysConfig == null)
            {
                hotkeysConfigResetEvent.WaitOne();
            }
        }

        public static void LoadProgramSettings()
        {
            Settings = ApplicationConfig.Load(ApplicationConfigFilePath);
            DefaultTaskSettings = Settings.DefaultTaskSettings;

            RunBackwardCompatibilityTasks();
        }

        private static void RunBackwardCompatibilityTasks()
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

        private static void RunUploaderBackwardCompatibilityTasks()
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

        public static void LoadUploadersConfig()
        {
            UploadersConfig = UploadersConfig.Load(UploadersConfigFilePath);
        }

        public static void LoadHotkeySettings()
        {
            HotkeysConfig = HotkeysConfig.Load(HotkeysConfigFilePath);
        }

        public static void LoadAllSettings()
        {
            LoadProgramSettings();
            LoadUploadersConfig();
            LoadHotkeySettings();
        }

        public static void SaveAllSettings()
        {
            if (Settings != null) Settings.Save(ApplicationConfigFilePath);
            if (UploadersConfig != null) UploadersConfig.Save(UploadersConfigFilePath);
            if (HotkeysConfig != null) HotkeysConfig.Save(HotkeysConfigFilePath);
        }

        public static void SaveAllSettingsAsync()
        {
            if (Settings != null) Settings.SaveAsync(ApplicationConfigFilePath);
            UploadersConfigSaveAsync();
            if (HotkeysConfig != null) HotkeysConfig.SaveAsync(HotkeysConfigFilePath);
        }

        public static void BackupSettings()
        {
            Helpers.BackupFileWeekly(ApplicationConfigFilePath, BackupFolder);
            Helpers.BackupFileWeekly(HotkeysConfigFilePath, BackupFolder);
            Helpers.BackupFileWeekly(UploadersConfigFilePath, BackupFolder);
            Helpers.BackupFileWeekly(HistoryFilePath, BackupFolder);
        }

        public static void ConfigureUploadersConfigWatcher()
        {
            if (Settings.DetectUploaderConfigFileChanges && uploaderConfigWatcher == null)
            {
                uploaderConfigWatcher = new FileSystemWatcher(Path.GetDirectoryName(UploadersConfigFilePath), Path.GetFileName(UploadersConfigFilePath));
                uploaderConfigWatcher.Changed += uploaderConfigWatcher_Changed;
                uploaderConfigWatcherTimer = new WatchFolderDuplicateEventTimer(UploadersConfigFilePath);
                uploaderConfigWatcher.EnableRaisingEvents = true;
            }
            else if (uploaderConfigWatcher != null)
            {
                uploaderConfigWatcher.Dispose();
                uploaderConfigWatcher = null;
            }
        }

        private static void ReloadUploadersConfig(string filePath)
        {
            UploadersConfig = UploadersConfig.Load(filePath);
        }

        private static void uploaderConfigWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!uploaderConfigWatcherTimer.IsDuplicateEvent(e.FullPath))
            {
                Action onCompleted = () => ReloadUploadersConfig(e.FullPath);
                Helpers.WaitWhileAsync(() => Helpers.IsFileLocked(e.FullPath), 250, 5000, onCompleted, 1000);
                uploaderConfigWatcherTimer = new WatchFolderDuplicateEventTimer(e.FullPath);
            }
        }

        public static void UploadersConfigSaveAsync()
        {
            if (UploadersConfig != null)
            {
                if (uploaderConfigWatcher != null) uploaderConfigWatcher.EnableRaisingEvents = false;

                TaskEx.Run(() =>
                {
                    UploadersConfig.Save(UploadersConfigFilePath);
                },
                () =>
                {
                    if (uploaderConfigWatcher != null) uploaderConfigWatcher.EnableRaisingEvents = true;
                });
            }
        }

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
                    AddFileToDictionary(files, ApplicationConfigFilePath);
                    AddFileToDictionary(files, HotkeysConfigFilePath);
                    AddFileToDictionary(files, UploadersConfigFilePath);
                    AddFileToDictionary(files, GreenshotImageEditorConfigFilePath);
                }

                if (Program.Settings.ExportHistory)
                {
                    AddFileToDictionary(files, HistoryFilePath);
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