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
using ShareX.HistoryLib;
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
using ShareX.UploadersLib.FileUploaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public static string BackupFolder => Path.Combine(Program.PersonalFolder, "Backup");

        private static ApplicationConfig Settings { get => Program.Settings; set => Program.Settings = value; }
        private static TaskSettings DefaultTaskSettings { get => Program.DefaultTaskSettings; set => Program.DefaultTaskSettings = value; }
        private static UploadersConfig UploadersConfig { get => Program.UploadersConfig; set => Program.UploadersConfig = value; }
        private static HotkeysConfig HotkeysConfig { get => Program.HotkeysConfig; set => Program.HotkeysConfig = value; }

        private static ManualResetEvent uploadersConfigResetEvent = new ManualResetEvent(false);
        private static ManualResetEvent hotkeysConfigResetEvent = new ManualResetEvent(false);

        private const int SettingsSaveFailWarningLimit = 3;
        private static int settingsSaveFailWarningCount;

        public static void LoadInitialSettings()
        {
            LoadApplicationConfig();

            Task.Run(() =>
            {
                LoadUploadersConfig();
                uploadersConfigResetEvent.Set();

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
            Settings = ApplicationConfig.Load(ApplicationConfigFilePath, BackupFolder);
            Settings.CreateBackup = true;
            Settings.CreateWeeklyBackup = true;
            Settings.SettingsSaveFailed += Settings_SettingsSaveFailed;
            DefaultTaskSettings = Settings.DefaultTaskSettings;
            ApplicationConfigBackwardCompatibilityTasks();
            MigrateHistoryFile();
        }

        private static void Settings_SettingsSaveFailed(Exception e)
        {
            if (settingsSaveFailWarningCount == SettingsSaveFailWarningLimit) return;

            string message;

            if (e is UnauthorizedAccessException || e is FileNotFoundException)
            {
                message = Resources.YourAntiVirusSoftwareOrTheControlledFolderAccessFeatureInWindows10CouldBeBlockingShareX;
            }
            else
            {
                message = e.Message;
            }

            TaskHelpers.ShowNotificationTip(message, "ShareX - " + Resources.FailedToSaveSettings, 5000);

            settingsSaveFailWarningCount++;
        }

        public static void LoadUploadersConfig()
        {
            UploadersConfig = UploadersConfig.Load(UploadersConfigFilePath, BackupFolder);
            UploadersConfig.CreateBackup = true;
            UploadersConfig.CreateWeeklyBackup = true;
            UploadersConfig.SupportDPAPIEncryption = true;
            UploadersConfigBackwardCompatibilityTasks();
        }

        public static void LoadHotkeysConfig()
        {
            HotkeysConfig = HotkeysConfig.Load(HotkeysConfigFilePath, BackupFolder);
            HotkeysConfig.CreateBackup = true;
            HotkeysConfig.CreateWeeklyBackup = true;
            HotkeysConfigBackwardCompatibilityTasks();
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

            if (Settings.IsUpgradeFrom("13.0.2"))
            {
                Settings.UseCustomTheme = Settings.UseDarkTheme;
            }

            if (Settings.IsUpgradeFrom("13.3.1") && Settings.Themes != null)
            {
                Settings.Themes.Add(ShareXTheme.NordDarkTheme);
                Settings.Themes.Add(ShareXTheme.NordLightTheme);
                Settings.Themes.Add(ShareXTheme.DraculaTheme);
            }

            if (Settings.IsUpgradeFrom("13.4.0"))
            {
                DefaultTaskSettings.GeneralSettings.ShowToastNotificationAfterTaskCompleted =
                    DefaultTaskSettings.GeneralSettings.PopUpNotification != PopUpNotificationType.None;
            }
        }

        private static void MigrateHistoryFile()
        {
            if (File.Exists(Program.HistoryFilePathOld))
            {
                if (!File.Exists(Program.HistoryFilePath))
                {
                    DebugHelper.WriteLine($"Migrating XML history file \"{Program.HistoryFilePathOld}\" to JSON history file \"{Program.HistoryFilePath}\"");

                    HistoryManagerXML historyManagerXML = new HistoryManagerXML(Program.HistoryFilePathOld);
                    List<HistoryItem> historyItems = historyManagerXML.GetHistoryItems();

                    if (historyItems.Count > 0)
                    {
                        HistoryManagerJSON historyManagerJSON = new HistoryManagerJSON(Program.HistoryFilePath);
                        historyManagerJSON.AppendHistoryItems(historyItems);
                    }
                }

                Helpers.MoveFile(Program.HistoryFilePathOld, BackupFolder);
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

            if (UploadersConfig.CustomUploadersList != null)
            {
                foreach (CustomUploaderItem cui in UploadersConfig.CustomUploadersList)
                {
                    cui.CheckBackwardCompatibility();
                }
            }
        }

        private static void HotkeysConfigBackwardCompatibilityTasks()
        {
            if (HotkeysConfig.IsUpgradeFrom("13.1.1"))
            {
                foreach (TaskSettings taskSettings in HotkeysConfig.Hotkeys.Select(x => x.TaskSettings))
                {
                    if (taskSettings != null && !string.IsNullOrEmpty(taskSettings.AdvancedSettings.CapturePath))
                    {
                        taskSettings.OverrideScreenshotsFolder = true;
                        taskSettings.ScreenshotsFolder = taskSettings.AdvancedSettings.CapturePath;
                        taskSettings.AdvancedSettings.CapturePath = "";
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

        public static void ResetSettings()
        {
            if (File.Exists(ApplicationConfigFilePath)) File.Delete(ApplicationConfigFilePath);
            LoadApplicationConfig();

            if (File.Exists(UploadersConfigFilePath)) File.Delete(UploadersConfigFilePath);
            LoadUploadersConfig();

            if (File.Exists(HotkeysConfigFilePath)) File.Delete(HotkeysConfigFilePath);
            LoadHotkeysConfig();
        }

        public static bool Export(string archivePath, bool settings, bool history)
        {
            MemoryStream msApplicationConfig = null, msUploadersConfig = null, msHotkeysConfig = null;

            try
            {
                List<ZipEntryInfo> entries = new List<ZipEntryInfo>();

                if (settings)
                {
                    msApplicationConfig = Settings.SaveToMemoryStream(false);
                    entries.Add(new ZipEntryInfo(msApplicationConfig, ApplicationConfigFilename));

                    msUploadersConfig = UploadersConfig.SaveToMemoryStream(false);
                    entries.Add(new ZipEntryInfo(msUploadersConfig, UploadersConfigFilename));

                    msHotkeysConfig = HotkeysConfig.SaveToMemoryStream(false);
                    entries.Add(new ZipEntryInfo(msHotkeysConfig, HotkeysConfigFilename));
                }

                if (history)
                {
                    entries.Add(new ZipEntryInfo(Program.HistoryFilePath));
                }

                ZipManager.Compress(archivePath, entries);
                return true;
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                MessageBox.Show("Error while exporting backup:\r\n" + e, "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                msApplicationConfig?.Dispose();
                msUploadersConfig?.Dispose();
                msHotkeysConfig?.Dispose();
            }

            return false;
        }

        public static bool Import(string archivePath)
        {
            try
            {
                ZipManager.Extract(archivePath, Program.PersonalFolder, true, entry =>
                {
                    return Helpers.CheckExtension(entry.Name, new string[] { "json", "xml" });
                }, 1_000_000_000);

                return true;
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