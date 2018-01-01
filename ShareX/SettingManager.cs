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
        private const string ApplicationConfigFilename = "ApplicationConfig.json";

        private static string ApplicationConfigFilePath
        {
            get
            {
                if (Program.Sandbox) return null;

                return Path.Combine(Program.PersonalFolder, ApplicationConfigFilename);
            }
        }

        private const string UploadersConfigFilename = "UploadersConfig.json";

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

                return Path.Combine(uploadersConfigFolder, UploadersConfigFilename);
            }
        }

        private const string HotkeysConfigFilename = "HotkeysConfig.json";

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

                return Path.Combine(hotkeysConfigFolder, HotkeysConfigFilename);
            }
        }

        private static string BackupFolder => Path.Combine(Program.PersonalFolder, "Backup");

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
                        if (endpoint.Region != null && endpoint.Region.Equals(UploadersConfig.AmazonS3Settings.Endpoint, StringComparison.InvariantCultureIgnoreCase))
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
        }

        public static bool Export(string archivePath)
        {
            try
            {
                if (File.Exists(archivePath))
                {
                    File.Delete(archivePath);
                }

                List<string> files = new List<string>();

                if (Settings.ExportSettings)
                {
                    files.Add(ApplicationConfigFilename);
                    files.Add(HotkeysConfigFilename);
                    files.Add(UploadersConfigFilename);
                }

                if (Settings.ExportHistory)
                {
                    files.Add(Program.HistoryFilename);
                }

                if (Settings.ExportLogs)
                {
                    files.Add($"{Program.LogsFoldername}\\*.txt");
                }

                SevenZipManager sevenZipManager = new SevenZipManager();
                return sevenZipManager.Compress(archivePath, files, Program.PersonalFolder);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                MessageBox.Show("Error while exporting backup:\r\n" + e, "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public static bool Import(string archivePath)
        {
            try
            {
                SevenZipManager sevenZipManager = new SevenZipManager();
                return sevenZipManager.Extract(archivePath, Program.PersonalFolder);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                MessageBox.Show("Error while importing backup:\r\n" + e, "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }
    }
}