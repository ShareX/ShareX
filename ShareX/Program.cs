#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

using HelpersLib;
using SingleInstanceApplication;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UploadersLib;

namespace ShareX
{
    internal static class Program
    {
        public static readonly string ApplicationName = Application.ProductName;
        public static readonly Version AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

        public static string Title
        {
            get
            {
                string title = string.Format("{0} {1}.{2}", ApplicationName, AssemblyVersion.Major, AssemblyVersion.Minor);
                if (AssemblyVersion.Build > 0) title += "." + AssemblyVersion.Build;
                if (IsPortable) title += " Portable";
                return title;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

                if (attributes.Length == 0)
                {
                    return string.Empty;
                }

                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        #region Paths

        public static readonly string StartupPath = Application.StartupPath;

        public static readonly string DefaultPersonalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ApplicationName);
        private static readonly string PortablePersonalPath = Path.Combine(StartupPath, ApplicationName);
        private static readonly string PersonalPathConfig = Path.Combine(StartupPath, "PersonalPath.cfg");
        private static readonly string ApplicationConfigFilename = "ApplicationConfig.json";
        private static readonly string UploadersConfigFilename = "UploadersConfig.json";
        private static readonly string HotkeysConfigFilename = "HotkeysConfig.json";
        private static readonly string HistoryFilename = "History.xml";
        private static readonly string LogFileName = ApplicationName + "-Log-{0:yyyy-MM}.txt";

        public static string CustomPersonalPath { get; private set; }

        private static FileSystemWatcher uploaderConfigWatcher;
        private static WatchFolderDuplicateEventTimer uploaderConfigWatcherTimer;

        public static string PersonalPath
        {
            get
            {
                if (!string.IsNullOrEmpty(CustomPersonalPath))
                {
                    return CustomPersonalPath;
                }

                return DefaultPersonalPath;
            }
        }

        public static string ApplicationConfigFilePath
        {
            get
            {
                if (!IsSandbox)
                {
                    return Path.Combine(PersonalPath, ApplicationConfigFilename);
                }

                return null;
            }
        }

        private static string UploadersConfigFolder
        {
            get
            {
                if (Settings != null && !string.IsNullOrEmpty(Settings.CustomUploadersConfigPath))
                {
                    return Settings.CustomUploadersConfigPath;
                }

                return PersonalPath;
            }
        }

        public static string UploadersConfigFilePath
        {
            get
            {
                if (!IsSandbox)
                {
                    return Path.Combine(UploadersConfigFolder, UploadersConfigFilename);
                }

                return null;
            }
        }

        private static string HotkeysConfigFolder
        {
            get
            {
                if (Settings != null && !string.IsNullOrEmpty(Settings.CustomHotkeysConfigPath))
                {
                    return Settings.CustomHotkeysConfigPath;
                }

                return PersonalPath;
            }
        }

        public static string HotkeysConfigFilePath
        {
            get
            {
                if (!IsSandbox)
                {
                    return Path.Combine(HotkeysConfigFolder, HotkeysConfigFilename);
                }

                return null;
            }
        }

        public static string HistoryFilePath
        {
            get
            {
                if (!IsSandbox)
                {
                    return Path.Combine(PersonalPath, HistoryFilename);
                }

                return null;
            }
        }

        private static string LogsFolder
        {
            get
            {
                return Path.Combine(PersonalPath, "Logs");
            }
        }

        public static string LogsFilePath
        {
            get
            {
                string filename = string.Format(LogFileName, FastDateTime.Now);
                return Path.Combine(LogsFolder, filename);
            }
        }

        public static string ScreenshotsParentFolder
        {
            get
            {
                if (Settings != null && Settings.UseCustomScreenshotsPath && !string.IsNullOrEmpty(Settings.CustomScreenshotsPath))
                {
                    return Settings.CustomScreenshotsPath;
                }

                return Path.Combine(PersonalPath, "Screenshots");
            }
        }

        public static string ScreenshotsFolder
        {
            get
            {
                string subFolderName = new NameParser(NameParserType.FolderPath).Parse(Settings.SaveImageSubFolderPattern);
                return Path.Combine(ScreenshotsParentFolder, subFolderName);
            }
        }

        public static string ScreenRecorderCacheFilePath
        {
            get
            {
                return Path.Combine(PersonalPath, "ScreenRecorder.avi");
            }
        }

        private static string BackupFolder
        {
            get
            {
                return Path.Combine(PersonalPath, "Backup");
            }
        }

        public static string ToolsFolder
        {
            get
            {
                return Path.Combine(PersonalPath, "Tools");
            }
        }

        #endregion Paths

        public static bool IsMultiInstance { get; private set; }
        public static bool IsPortable { get; private set; }
        public static bool IsSilentRun { get; private set; }
        public static bool IsSandbox { get; private set; }

        public static ApplicationConfig Settings { get; private set; }
        public static TaskSettings DefaultTaskSettings { get; private set; }
        public static UploadersConfig UploadersConfig { get; private set; }
        public static HotkeysConfig HotkeysConfig { get; private set; }

        public static ManualResetEvent SettingsResetEvent { get; private set; }
        public static ManualResetEvent UploaderSettingsResetEvent { get; private set; }
        public static ManualResetEvent HotkeySettingsResetEvent { get; private set; }

        public static MainForm MainForm { get; private set; }
        public static Stopwatch StartTimer { get; private set; }
        public static HotkeyManager HotkeyManager { get; set; }
        public static WatchFolderManager WatchFolderManager { get; set; }

        [STAThread]
        private static void Main(string[] args)
        {
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            StartTimer = Stopwatch.StartNew();

            IsMultiInstance = CLIHelper.CheckArgs(args, "multi", "m");

            if (IsMultiInstance || ApplicationInstanceManager.CreateSingleInstance(SingleInstanceCallback))
            {
                Run(args);
            }
        }

        private static void Run(string[] args)
        {
            string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();

            using (Mutex mutex = new Mutex(false, appGuid)) // Required for installer
            {
                IsSilentRun = CLIHelper.CheckArgs(args, "silent", "s");
                IsSandbox = CLIHelper.CheckArgs(args, "sandbox");

                if (!IsSandbox)
                {
                    IsPortable = CLIHelper.CheckArgs(args, "portable", "p");

                    if (IsPortable)
                    {
                        CustomPersonalPath = PortablePersonalPath;
                    }
                    else
                    {
                        CheckPersonalPathConfig();
                    }

                    if (!string.IsNullOrEmpty(PersonalPath) && !Directory.Exists(PersonalPath))
                    {
                        Directory.CreateDirectory(PersonalPath);
                    }
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                DebugHelper.WriteLine("{0} started", Title);
                DebugHelper.WriteLine("Operating system: " + Environment.OSVersion.VersionString);
                DebugHelper.WriteLine("Command line: " + Environment.CommandLine);
                DebugHelper.WriteLine("Personal path: " + PersonalPath);

                SettingsResetEvent = new ManualResetEvent(false);
                UploaderSettingsResetEvent = new ManualResetEvent(false);
                HotkeySettingsResetEvent = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(state => LoadSettings());

                DebugHelper.WriteLine("MainForm init started");
                MainForm = new MainForm();
                DebugHelper.WriteLine("MainForm init finished");

                if (Settings == null)
                {
                    SettingsResetEvent.WaitOne();
                }

                Application.Run(MainForm);

                if (WatchFolderManager != null) WatchFolderManager.Dispose();
                SaveSettings();
                BackupSettings();

                DebugHelper.WriteLine("ShareX closing");
                DebugHelper.Logger.SaveLog(LogsFilePath);
            }
        }

        public static void LoadSettings()
        {
            LoadProgramSettings();
            SettingsResetEvent.Set();
            LoadUploadersConfig();
            UploaderSettingsResetEvent.Set();
            LoadHotkeySettings();
            HotkeySettingsResetEvent.Set();

            ConfigureUploadersConfigWatcher();
        }

        public static void LoadProgramSettings()
        {
            Settings = ApplicationConfig.Load(ApplicationConfigFilePath);
            DefaultTaskSettings = Settings.DefaultTaskSettings;
            MigrateTaskSettings(DefaultTaskSettings);
        }

        public static void LoadUploadersConfig()
        {
            UploadersConfig = UploadersConfig.Load(UploadersConfigFilePath);
        }

        public static void LoadHotkeySettings()
        {
            HotkeysConfig = HotkeysConfig.Load(HotkeysConfigFilePath);

            if (HotkeysConfig != null && HotkeysConfig.Hotkeys != null)
            {
                HotkeysConfig.Hotkeys.ForEach(x => MigrateTaskSettings(x.TaskSettings));
            }
        }

        public static void MigrateTaskSettings(TaskSettings taskSettings)
        {
            if (!taskSettings.SettingsMigrated)
            {
                taskSettings.SafeAfterTasks.AfterCaptureJob = taskSettings.AfterCaptureJob;
                taskSettings.SafeAfterTasks.AfterUploadJob = taskSettings.AfterUploadJob;

                taskSettings.TaskDestinations.ImageDestination = taskSettings.ImageDestination;
                taskSettings.TaskDestinations.ImageFileDestination = taskSettings.ImageFileDestination;
                taskSettings.TaskDestinations.FileDestination = taskSettings.FileDestination;
                taskSettings.TaskDestinations.TextDestination = taskSettings.TextDestination;
                taskSettings.TaskDestinations.TextFileDestination = taskSettings.TextFileDestination;
                taskSettings.TaskDestinations.URLShortenerDestination = taskSettings.URLShortenerDestination;
                taskSettings.TaskDestinations.SocialNetworkingServiceDestination = taskSettings.SocialNetworkingServiceDestination;

                taskSettings.TaskDestinations.OverrideFTP = taskSettings.OverrideFTP;
                taskSettings.TaskDestinations.FTPIndex = taskSettings.FTPIndex;

                taskSettings.SettingsMigrated = true;
            }
        }

        public static void SaveSettings()
        {
            if (Settings != null) Settings.Save(ApplicationConfigFilePath);
            if (UploadersConfig != null) UploadersConfig.Save(UploadersConfigFilePath);
            if (HotkeysConfig != null) HotkeysConfig.Save(HotkeysConfigFilePath);
        }

        public static void BackupSettings()
        {
            Helpers.BackupFileWeekly(ApplicationConfigFilePath, BackupFolder);
            Helpers.BackupFileWeekly(HotkeysConfigFilePath, BackupFolder);
            Helpers.BackupFileWeekly(UploadersConfigFilePath, BackupFolder);
            Helpers.BackupFileWeekly(HistoryFilePath, BackupFolder);
        }

        private static void CheckPersonalPathConfig()
        {
            string customPersonalPath = ReadPersonalPathConfig();

            if (!string.IsNullOrEmpty(customPersonalPath))
            {
                CustomPersonalPath = Path.GetFullPath(customPersonalPath);

                if (CustomPersonalPath.Equals(PortablePersonalPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    IsPortable = true;
                }
            }
        }

        public static string ReadPersonalPathConfig()
        {
            if (File.Exists(PersonalPathConfig))
            {
                return File.ReadAllText(PersonalPathConfig, Encoding.UTF8).Trim();
            }

            return string.Empty;
        }

        public static void WritePersonalPathConfig(string path)
        {
            if (path == null)
            {
                path = string.Empty;
            }
            else
            {
                path = path.Trim();
            }

            bool isDefaultPath = string.IsNullOrEmpty(path) && !File.Exists(PersonalPathConfig);

            if (!isDefaultPath)
            {
                string currentPath = ReadPersonalPathConfig();

                if (!path.Equals(currentPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    try
                    {
                        File.WriteAllText(PersonalPathConfig, path, Encoding.UTF8);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("Can't access to \"" + PersonalPathConfig + "\" file.\r\nPlease run ShareX as administrator to change personal folder path.", "ShareX",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            OnError(e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            OnError((Exception)e.ExceptionObject);
        }

        private static void OnError(Exception e)
        {
            using (ErrorForm errorForm = new ErrorForm(Application.ProductName, e, DebugHelper.Logger, LogsFilePath, Links.URL_ISSUES))
            {
                errorForm.ShowDialog();
            }
        }

        private static void SingleInstanceCallback(object sender, InstanceCallbackEventArgs args)
        {
            if (WaitFormLoad(5000))
            {
                Action d = () =>
                {
                    if (args.CommandLineArgs == null || args.CommandLineArgs.Length <= 1)
                    {
                        if (MainForm.niTray != null && MainForm.niTray.Visible)
                        {
                            // Workaround for Windows startup tray icon bug
                            MainForm.niTray.Visible = false;
                            MainForm.niTray.Visible = true;
                        }

                        MainForm.ShowActivate();
                    }
                    else if (MainForm.Visible)
                    {
                        MainForm.ShowActivate();
                    }

                    MainForm.UseCommandLineArgs(args.CommandLineArgs);
                };

                MainForm.InvokeSafe(d);
            }
        }

        private static bool WaitFormLoad(int wait)
        {
            Stopwatch timer = Stopwatch.StartNew();

            while (timer.ElapsedMilliseconds < wait)
            {
                if (MainForm != null && MainForm.IsReady) return true;

                Thread.Sleep(10);
            }

            return false;
        }

        public static void ConfigureUploadersConfigWatcher()
        {
            if (Program.Settings.DetectUploaderConfigFileChanges && uploaderConfigWatcher == null)
            {
                uploaderConfigWatcher = new FileSystemWatcher(Path.GetDirectoryName(Program.UploadersConfigFilePath), Path.GetFileName(Program.UploadersConfigFilePath));
                uploaderConfigWatcher.Changed += uploaderConfigWatcher_Changed;
                uploaderConfigWatcherTimer = new WatchFolderDuplicateEventTimer(Program.UploadersConfigFilePath);
                uploaderConfigWatcher.EnableRaisingEvents = true;
            }
            else if (uploaderConfigWatcher != null)
            {
                uploaderConfigWatcher.Dispose();
                uploaderConfigWatcher = null;
            }
        }

        private static void uploaderConfigWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!uploaderConfigWatcherTimer.IsDuplicateEvent(e.FullPath))
            {
                SynchronizationContext context = SynchronizationContext.Current ?? new SynchronizationContext();
                Action onCompleted = () => context.Post(state => ReloadUploadersConfig(e.FullPath), null);
                Helpers.WaitWhileAsync(() => Helpers.IsFileLocked(e.FullPath), 250, 5000, onCompleted, 1000);
                uploaderConfigWatcherTimer = new WatchFolderDuplicateEventTimer(e.FullPath);
            }
        }

        private static void ReloadUploadersConfig(string filePath)
        {
            UploadersConfig = UploadersConfig.Load(filePath);
        }

        public async static void UploadersConfigSaveAsync()
        {
            if (uploaderConfigWatcher != null) uploaderConfigWatcher.EnableRaisingEvents = false;
            await TaskEx.Run(() => UploadersConfig.Save(Program.UploadersConfigFilePath));
            if (uploaderConfigWatcher != null) uploaderConfigWatcher.EnableRaisingEvents = true;
        }
    }
}