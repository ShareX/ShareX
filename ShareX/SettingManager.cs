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

using ShareX.HelpersLib;
using ShareX.HistoryLib;
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
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
        private const string ApplicationConfigFileName = "ApplicationConfig.json";

        private static string ApplicationConfigFilePath
        {
            get
            {
                if (Program.Sandbox) return null;

                return Path.Combine(Program.PersonalFolder, ApplicationConfigFileName);
            }
        }

        private const string UploadersConfigFileName = "UploadersConfig.json";

        private static string UploadersConfigFilePath
        {
            get
            {
                if (Program.Sandbox) return null;

                string uploadersConfigFolder;

                if (Settings != null && !string.IsNullOrEmpty(Settings.CustomUploadersConfigPath))
                {
                    uploadersConfigFolder = FileHelpers.ExpandFolderVariables(Settings.CustomUploadersConfigPath);
                }
                else
                {
                    uploadersConfigFolder = Program.PersonalFolder;
                }

                return Path.Combine(uploadersConfigFolder, UploadersConfigFileName);
            }
        }

        private const string HotkeysConfigFileName = "HotkeysConfig.json";

        private static string HotkeysConfigFilePath
        {
            get
            {
                if (Program.Sandbox) return null;

                string hotkeysConfigFolder;

                if (Settings != null && !string.IsNullOrEmpty(Settings.CustomHotkeysConfigPath))
                {
                    hotkeysConfigFolder = FileHelpers.ExpandFolderVariables(Settings.CustomHotkeysConfigPath);
                }
                else
                {
                    hotkeysConfigFolder = Program.PersonalFolder;
                }

                return Path.Combine(hotkeysConfigFolder, HotkeysConfigFileName);
            }
        }

        public static string BackupFolder => Path.Combine(Program.PersonalFolder, "Backup");

        private static ApplicationConfig Settings { get => Program.Settings; set => Program.Settings = value; }
        private static TaskSettings DefaultTaskSettings { get => Program.DefaultTaskSettings; set => Program.DefaultTaskSettings = value; }
        private static UploadersConfig UploadersConfig { get => Program.UploadersConfig; set => Program.UploadersConfig = value; }
        private static HotkeysConfig HotkeysConfig { get => Program.HotkeysConfig; set => Program.HotkeysConfig = value; }

        private static ManualResetEvent uploadersConfigResetEvent = new ManualResetEvent(false);
        private static ManualResetEvent hotkeysConfigResetEvent = new ManualResetEvent(false);

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

        public static void LoadApplicationConfig(bool fallbackSupport = true)
        {
            Settings = ApplicationConfig.Load(ApplicationConfigFilePath, BackupFolder, fallbackSupport);
            Settings.CreateBackup = true;
            Settings.CreateWeeklyBackup = true;
            Settings.SettingsSaveFailed += Settings_SettingsSaveFailed;
            DefaultTaskSettings = Settings.DefaultTaskSettings;
            ApplicationConfigBackwardCompatibilityTasks();
            MigrateHistoryFile();
        }

        private static void Settings_SettingsSaveFailed(Exception e)
        {
            string message;

            if (e is UnauthorizedAccessException || e is FileNotFoundException)
            {
                message = Resources.YourAntiVirusSoftwareOrTheControlledFolderAccessFeatureInWindowsCouldBeBlockingShareX;
            }
            else
            {
                message = e.Message;
            }

            TaskHelpers.ShowNotificationTip(message, "ShareX - " + Resources.FailedToSaveSettings, 5000);
        }

        public static void LoadUploadersConfig(bool fallbackSupport = true)
        {
            UploadersConfig = UploadersConfig.Load(UploadersConfigFilePath, BackupFolder, fallbackSupport);
            UploadersConfig.CreateBackup = true;
            UploadersConfig.CreateWeeklyBackup = true;
            UploadersConfig.SupportDPAPIEncryption = true;
            UploadersConfigBackwardCompatibilityTasks();
        }

        public static void LoadHotkeysConfig(bool fallbackSupport = true)
        {
            HotkeysConfig = HotkeysConfig.Load(HotkeysConfigFilePath, BackupFolder, fallbackSupport);
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
            if (SystemOptions.DisableUpload)
            {
                DefaultTaskSettings.AfterCaptureJob = DefaultTaskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.UploadImageToHost);
            }

            if (Settings.IsUpgradeFrom("14.1.1"))
            {
                if (Helpers.IsDefaultSettings(Settings.Themes, ShareXTheme.GetDefaultThemes(), (x, y) => x.Name == y.Name))
                {
                    if (!Settings.Themes.IsValidIndex(Settings.SelectedTheme))
                    {
                        Settings.SelectedTheme = 0;
                    }

                    ShareXTheme selectedTheme = Settings.Themes[Settings.SelectedTheme];

                    Settings.Themes = ShareXTheme.GetDefaultThemes();

                    int index = Settings.Themes.FindIndex(x => x.Name.Equals(selectedTheme.Name, StringComparison.OrdinalIgnoreCase));

                    if (index >= 0)
                    {
                        Settings.SelectedTheme = index;
                    }
                    else
                    {
                        Settings.SelectedTheme = 0;
                    }
                }
            }

            if (Settings.IsUpgradeFrom("14.1.2"))
            {
                if (!Environment.Is64BitOperatingSystem && !string.IsNullOrEmpty(DefaultTaskSettings.CaptureSettings.FFmpegOptions.CLIPath))
                {
                    DefaultTaskSettings.CaptureSettings.FFmpegOptions.OverrideCLIPath = true;
                }
            }

            if (Settings.IsUpgradeFrom("15.0.1"))
            {
                DefaultTaskSettings.CaptureSettings.ScrollingCaptureOptions = new ScrollingCaptureOptions();
                DefaultTaskSettings.CaptureSettings.FFmpegOptions.FixSources();
            }

            if (Settings.IsUpgradeFrom("16.0.2"))
            {
                if (Settings.CheckPreReleaseUpdates)
                {
                    Settings.UpdateChannel = UpdateChannel.PreRelease;
                }

                if (!DefaultTaskSettings.CaptureSettings.SurfaceOptions.UseDimming)
                {
                    DefaultTaskSettings.CaptureSettings.SurfaceOptions.BackgroundDimStrength = 0;
                }
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

                FileHelpers.MoveFile(Program.HistoryFilePathOld, BackupFolder);
            }
        }

        private static void UploadersConfigBackwardCompatibilityTasks()
        {
            if (UploadersConfig.CustomUploadersList != null)
            {
                foreach (CustomUploaderItem cui in UploadersConfig.CustomUploadersList)
                {
                    try
                    {
                        cui.CheckBackwardCompatibility();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static void HotkeysConfigBackwardCompatibilityTasks()
        {
            if (SystemOptions.DisableUpload)
            {
                foreach (TaskSettings taskSettings in HotkeysConfig.Hotkeys.Select(x => x.TaskSettings))
                {
                    if (taskSettings != null)
                    {
                        taskSettings.AfterCaptureJob = taskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.UploadImageToHost);
                    }
                }
            }

            if (Settings.IsUpgradeFrom("15.0.1"))
            {
                foreach (TaskSettings taskSettings in HotkeysConfig.Hotkeys.Select(x => x.TaskSettings))
                {
                    if (taskSettings != null && taskSettings.CaptureSettings != null)
                    {
                        taskSettings.CaptureSettings.ScrollingCaptureOptions = new ScrollingCaptureOptions();
                        taskSettings.CaptureSettings.FFmpegOptions.FixSources();
                    }
                }
            }
        }

        public static void CleanupHotkeysConfig()
        {
            foreach (TaskSettings taskSettings in HotkeysConfig.Hotkeys.Select(x => x.TaskSettings))
            {
                taskSettings.Cleanup();
            }
        }

        public static void SaveAllSettings()
        {
            if (Settings != null)
            {
                Settings.Save(ApplicationConfigFilePath);
            }

            if (UploadersConfig != null)
            {
                UploadersConfig.Save(UploadersConfigFilePath);
            }

            if (HotkeysConfig != null)
            {
                CleanupHotkeysConfig();
                HotkeysConfig.Save(HotkeysConfigFilePath);
            }
        }

        public static void SaveApplicationConfigAsync()
        {
            if (Settings != null)
            {
                Settings.SaveAsync(ApplicationConfigFilePath);
            }
        }

        public static void SaveUploadersConfigAsync()
        {
            if (UploadersConfig != null)
            {
                UploadersConfig.SaveAsync(UploadersConfigFilePath);
            }
        }

        public static void SaveHotkeysConfigAsync()
        {
            if (HotkeysConfig != null)
            {
                CleanupHotkeysConfig();
                HotkeysConfig.SaveAsync(HotkeysConfigFilePath);
            }
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
            LoadApplicationConfig(false);

            if (File.Exists(UploadersConfigFilePath)) File.Delete(UploadersConfigFilePath);
            LoadUploadersConfig(false);

            if (File.Exists(HotkeysConfigFilePath)) File.Delete(HotkeysConfigFilePath);
            LoadHotkeysConfig(false);
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
                    entries.Add(new ZipEntryInfo(msApplicationConfig, ApplicationConfigFileName));

                    msUploadersConfig = UploadersConfig.SaveToMemoryStream(false);
                    entries.Add(new ZipEntryInfo(msUploadersConfig, UploadersConfigFileName));

                    msHotkeysConfig = HotkeysConfig.SaveToMemoryStream(false);
                    entries.Add(new ZipEntryInfo(msHotkeysConfig, HotkeysConfigFileName));
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
                    return FileHelpers.CheckExtension(entry.Name, new string[] { "json", "xml" });
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