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

using ShareX.HelpersLib;
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShareX
{
    internal static class Program
    {
        public static ShareXBuild Build
        {
            get
            {
#if STEAM
                return ShareXBuild.Steam;
#elif RELEASE
                return ShareXBuild.Release;
#elif DEBUG
                return ShareXBuild.Debug;
#else
                return ShareXBuild.Unknown;
#endif
            }
        }

        public static string Title
        {
            get
            {
                Version version = Version.Parse(Application.ProductVersion);
                string title = string.Format("ShareX {0}.{1}", version.Major, version.Minor);
                if (version.Build > 0) title += "." + version.Build;
                if (version.Revision > 0) title += "." + version.Revision;
                if (Beta) title += " Beta";
                if (Portable) title += " Portable";
                return title;
            }
        }

        public static string TitleLong => $"{Title} ({Build})";

        public static bool Beta { get; } = false;
        public static bool MultiInstance { get; private set; }
        public static bool Portable { get; private set; }
        public static bool PortableApps { get; private set; }
        public static bool SilentRun { get; private set; }
        public static bool Sandbox { get; private set; }
        public static bool SteamFirstTimeConfig { get; private set; }
        public static bool IgnoreHotkeyWarning { get; private set; }
        public static bool PuushMode { get; private set; }

        public static ApplicationConfig Settings { get; private set; }
        public static TaskSettings DefaultTaskSettings { get; private set; }
        public static UploadersConfig UploadersConfig { get; private set; }
        public static HotkeysConfig HotkeysConfig { get; private set; }

        public static ManualResetEvent UploaderSettingsResetEvent { get; private set; }
        public static ManualResetEvent HotkeySettingsResetEvent { get; private set; }

        public static MainForm MainForm { get; private set; }
        public static Stopwatch StartTimer { get; private set; }
        public static HotkeyManager HotkeyManager { get; set; }
        public static WatchFolderManager WatchFolderManager { get; set; }
        public static GitHubUpdateManager UpdateManager { get; private set; }
        public static CLIManager CLI { get; private set; }

        private static bool restarting;
        private static FileSystemWatcher uploaderConfigWatcher;
        private static WatchFolderDuplicateEventTimer uploaderConfigWatcherTimer;

        #region Paths

        private const string AppName = "ShareX";
        private const string PersonalPathConfigFileName = "PersonalPath.cfg";

        public static readonly string DefaultPersonalFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppName);
        public static readonly string PortablePersonalFolder = Helpers.GetAbsolutePath(AppName);
        public static readonly string PortableAppsPersonalFolder = Helpers.GetAbsolutePath("../../Data");

        private static string PersonalPathConfigFilePath
        {
            get
            {
                string oldPath = Helpers.GetAbsolutePath(PersonalPathConfigFileName);

                if (Portable || File.Exists(oldPath))
                {
                    return oldPath;
                }

                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppName, PersonalPathConfigFileName);
            }
        }

        private static readonly string PortableCheckFilePath = Helpers.GetAbsolutePath("Portable");
        private static readonly string PortableAppsCheckFilePath = Helpers.GetAbsolutePath("PortableApps");
        public static readonly string NativeMessagingHostFilePath = Helpers.GetAbsolutePath("ShareX_NativeMessagingHost.exe");
        public static readonly string SteamInAppFilePath = Helpers.GetAbsolutePath("Steam");

        private static string CustomPersonalPath { get; set; }

        public static string PersonalFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(CustomPersonalPath))
                {
                    return Helpers.ExpandFolderVariables(CustomPersonalPath);
                }

                return DefaultPersonalFolder;
            }
        }

        public static string ApplicationConfigFilePath
        {
            get
            {
                if (!Sandbox)
                {
                    return Path.Combine(PersonalFolder, "ApplicationConfig.json");
                }

                return null;
            }
        }

        public static string UploadersConfigFilePath
        {
            get
            {
                if (!Sandbox)
                {
                    string uploadersConfigFolder;

                    if (Settings != null && !string.IsNullOrEmpty(Settings.CustomUploadersConfigPath))
                    {
                        uploadersConfigFolder = Helpers.ExpandFolderVariables(Settings.CustomUploadersConfigPath);
                    }
                    else
                    {
                        uploadersConfigFolder = PersonalFolder;
                    }

                    return Path.Combine(uploadersConfigFolder, "UploadersConfig.json");
                }

                return null;
            }
        }

        public static string HotkeysConfigFilePath
        {
            get
            {
                if (!Sandbox)
                {
                    string hotkeysConfigFolder;

                    if (Settings != null && !string.IsNullOrEmpty(Settings.CustomHotkeysConfigPath))
                    {
                        hotkeysConfigFolder = Helpers.ExpandFolderVariables(Settings.CustomHotkeysConfigPath);
                    }
                    else
                    {
                        hotkeysConfigFolder = PersonalFolder;
                    }

                    return Path.Combine(hotkeysConfigFolder, "HotkeysConfig.json");
                }

                return null;
            }
        }

        public static string HistoryFilePath
        {
            get
            {
                if (!Sandbox)
                {
                    return Path.Combine(PersonalFolder, "History.xml");
                }

                return null;
            }
        }

        public static string LogsFolder => Path.Combine(PersonalFolder, "Logs");

        public static string LogsFilePath
        {
            get
            {
                string filename = string.Format("ShareX-Log-{0:yyyy-MM}.txt", DateTime.Now);
                return Path.Combine(LogsFolder, filename);
            }
        }

        public static string ScreenshotsParentFolder
        {
            get
            {
                if (Settings != null && Settings.UseCustomScreenshotsPath)
                {
                    string path = Settings.CustomScreenshotsPath;
                    string path2 = Settings.CustomScreenshotsPath2;

                    if (!string.IsNullOrEmpty(path))
                    {
                        path = Helpers.ExpandFolderVariables(path);

                        if (string.IsNullOrEmpty(path2) || Directory.Exists(path))
                        {
                            return path;
                        }
                    }

                    if (!string.IsNullOrEmpty(path2))
                    {
                        path2 = Helpers.ExpandFolderVariables(path2);

                        if (Directory.Exists(path2))
                        {
                            return path2;
                        }
                    }
                }

                return Path.Combine(PersonalFolder, "Screenshots");
            }
        }

        public static string ScreenshotsFolder
        {
            get
            {
                string subFolderName = NameParser.Parse(NameParserType.FolderPath, Settings.SaveImageSubFolderPattern);
                return Path.Combine(ScreenshotsParentFolder, subFolderName);
            }
        }

        public static string BackupFolder => Path.Combine(PersonalFolder, "Backup");
        public static string ToolsFolder => Path.Combine(PersonalFolder, "Tools");
        public static string GreenshotImageEditorConfigFilePath => Path.Combine(PersonalFolder, "GreenshotImageEditor.ini");
        public static string ScreenRecorderCacheFilePath => Path.Combine(PersonalFolder, "ScreenRecorder.avi");
        public static string DefaultFFmpegFilePath => Path.Combine(ToolsFolder, "ffmpeg.exe");
        public static string ChromeHostManifestFilePath => Path.Combine(ToolsFolder, "Chrome-host-manifest.json");
        public static string FirefoxHostManifestFilePath => Path.Combine(ToolsFolder, "Firefox-host-manifest.json");

        #endregion Paths

        [STAThread]
        private static void Main(string[] args)
        {
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            StartTimer = Stopwatch.StartNew(); // For be able to show startup time

            CLI = new CLIManager(args);
            CLI.ParseCommands();

#if STEAM
            if (CheckUninstall()) return; // Steam will run ShareX with -Uninstall when uninstalling
#endif

            if (CheckAdminTasks()) return; // If ShareX opened just for be able to execute task as Admin

            UpdatePersonalPath();

            DebugHelper.Init(LogsFilePath);

            MultiInstance = CLI.IsCommandExist("multi", "m");

            using (ApplicationInstanceManager instanceManager = new ApplicationInstanceManager(!MultiInstance, args, SingleInstanceCallback))
            {
                Run();
            }

            if (restarting)
            {
                Process.Start(Application.ExecutablePath);
            }
        }

        private static void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DebugHelper.WriteLine(Title);
            DebugHelper.WriteLine("Build: " + Build);
            DebugHelper.WriteLine("Command line: " + Environment.CommandLine);
            DebugHelper.WriteLine("Personal path: " + PersonalFolder);
            DebugHelper.WriteLine("Operating system: " + Helpers.GetWindowsProductName());

            SilentRun = CLI.IsCommandExist("silent", "s");

#if STEAM
            SteamFirstTimeConfig = CLI.IsCommandExist("SteamConfig");
#endif

            IgnoreHotkeyWarning = CLI.IsCommandExist("NoHotkeys");

            CheckPuushMode();
            DebugWriteFlags();
            CleanTempFiles();
            LoadProgramSettings();

            UploaderSettingsResetEvent = new ManualResetEvent(false);
            HotkeySettingsResetEvent = new ManualResetEvent(false);
            TaskEx.Run(LoadSettings);

            Uploader.UpdateServicePointManager();
            UpdateManager = new GitHubUpdateManager("ShareX", "ShareX", Beta, Portable);

            LanguageHelper.ChangeLanguage(Settings.Language);

            DebugHelper.WriteLine("MainForm init started");
            MainForm = new MainForm();
            DebugHelper.WriteLine("MainForm init finished");

            Application.Run(MainForm);

            if (WatchFolderManager != null) WatchFolderManager.Dispose();
            SaveAllSettings();
            BackupSettings();

            DebugHelper.Logger.Async = false;
            DebugHelper.WriteLine("ShareX closing");
        }

        public static void Restart()
        {
            restarting = true;
            Application.Exit();
        }

        private static void SingleInstanceCallback(object sender, InstanceCallbackEventArgs args)
        {
            if (WaitFormLoad(5000))
            {
                Action d = () =>
                {
                    if (args.CommandLineArgs == null || args.CommandLineArgs.Length < 1)
                    {
                        if (MainForm.niTray != null && MainForm.niTray.Visible)
                        {
                            // Workaround for Windows startup tray icon bug
                            MainForm.niTray.Visible = false;
                            MainForm.niTray.Visible = true;
                        }

                        MainForm.ForceActivate();
                    }
                    else if (MainForm.Visible)
                    {
                        MainForm.ForceActivate();
                    }

                    CLIManager cli = new CLIManager(args.CommandLineArgs);
                    cli.ParseCommands();
                    MainForm.UseCommandLineArgs(cli.Commands);
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

        public static void LoadSettings()
        {
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

            // TODO: Remove this next version
            if (Settings.IsUpgrade)
            {
                RegionCaptureOptions regionCaptureOptions = DefaultTaskSettings.CaptureSettings.SurfaceOptions;
                regionCaptureOptions.AnnotationOptions = new AnnotationOptions();
                regionCaptureOptions.LastRegionTool = ShapeType.RegionRectangle;
                regionCaptureOptions.LastAnnotationTool = ShapeType.DrawingRectangle;
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

        private static void UpdatePersonalPath()
        {
            Sandbox = CLI.IsCommandExist("sandbox");

            if (!Sandbox)
            {
                Portable = CLI.IsCommandExist("portable", "p");

                if (Portable)
                {
                    CustomPersonalPath = PortablePersonalFolder;
                }
                else
                {
                    PortableApps = File.Exists(PortableAppsCheckFilePath);
                    Portable = PortableApps || File.Exists(PortableCheckFilePath);
                    CheckPersonalPathConfig();
                }

                if (!Directory.Exists(PersonalFolder))
                {
                    try
                    {
                        Directory.CreateDirectory(PersonalFolder);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(Resources.Program_Run_Unable_to_create_folder_ + string.Format(" \"{0}\"\r\n\r\n{1}", PersonalFolder, e),
                            "ShareX - " + Resources.Program_Run_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        CustomPersonalPath = "";
                    }
                }
            }
        }

        private static void CheckPersonalPathConfig()
        {
            string customPersonalPath = ReadPersonalPathConfig();

            if (!string.IsNullOrEmpty(customPersonalPath))
            {
                customPersonalPath = Helpers.ExpandFolderVariables(customPersonalPath);
                CustomPersonalPath = Helpers.GetAbsolutePath(customPersonalPath);
            }
            else if (PortableApps)
            {
                CustomPersonalPath = PortableAppsPersonalFolder;
            }
            else if (Portable)
            {
                CustomPersonalPath = PortablePersonalFolder;
            }
        }

        public static string ReadPersonalPathConfig()
        {
            if (File.Exists(PersonalPathConfigFilePath))
            {
                return File.ReadAllText(PersonalPathConfigFilePath, Encoding.UTF8).Trim();
            }

            return "";
        }

        public static void WritePersonalPathConfig(string path)
        {
            if (path == null)
            {
                path = "";
            }
            else
            {
                path = path.Trim();
            }

            bool isDefaultPath = string.IsNullOrEmpty(path) && !File.Exists(PersonalPathConfigFilePath);

            if (!isDefaultPath)
            {
                string currentPath = ReadPersonalPathConfig();

                if (!path.Equals(currentPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    try
                    {
                        Helpers.CreateDirectoryFromFilePath(PersonalPathConfigFilePath);
                        File.WriteAllText(PersonalPathConfigFilePath, path, Encoding.UTF8);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show(string.Format(Resources.Program_WritePersonalPathConfig_Cant_access_to_file, PersonalPathConfigFilePath),
                            "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            using (ErrorForm errorForm = new ErrorForm(e.Message, $"{e}\r\n\r\n{TitleLong}", LogsFilePath, Links.URL_ISSUES))
            {
                errorForm.ShowDialog();
            }
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

        private static void uploaderConfigWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!uploaderConfigWatcherTimer.IsDuplicateEvent(e.FullPath))
            {
                Action onCompleted = () => ReloadUploadersConfig(e.FullPath);
                Helpers.WaitWhileAsync(() => Helpers.IsFileLocked(e.FullPath), 250, 5000, onCompleted, 1000);
                uploaderConfigWatcherTimer = new WatchFolderDuplicateEventTimer(e.FullPath);
            }
        }

        private static void ReloadUploadersConfig(string filePath)
        {
            UploadersConfig = UploadersConfig.Load(filePath);
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

        private static bool CheckAdminTasks()
        {
            if (CLI.IsCommandExist("dnschanger"))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new DNSChangerForm());
                return true;
            }

            return false;
        }

        private static bool CheckUninstall()
        {
            if (CLI.IsCommandExist("uninstall"))
            {
                try
                {
                    IntegrationHelpers.Uninstall();
                }
                catch
                {
                }

                return true;
            }

            return false;
        }

        private static bool CheckPuushMode()
        {
            string puushPath = Helpers.GetAbsolutePath("puush");
            PuushMode = File.Exists(puushPath);
            return PuushMode;
        }

        private static void DebugWriteFlags()
        {
            List<string> flags = new List<string>();

            if (Beta) flags.Add(nameof(Beta));
            if (MultiInstance) flags.Add(nameof(MultiInstance));
            if (Portable) flags.Add(nameof(Portable));
            if (PortableApps) flags.Add(nameof(PortableApps));
            if (SilentRun) flags.Add(nameof(SilentRun));
            if (Sandbox) flags.Add(nameof(Sandbox));
            if (SteamFirstTimeConfig) flags.Add(nameof(SteamFirstTimeConfig));
            if (IgnoreHotkeyWarning) flags.Add(nameof(IgnoreHotkeyWarning));
            if (PuushMode) flags.Add(nameof(PuushMode));

            string output = string.Join(", ", flags);

            if (!string.IsNullOrEmpty(output))
            {
                DebugHelper.WriteLine("Flags: " + output);
            }
        }

        private static void CleanTempFiles()
        {
            new Thread(() =>
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

                            DebugHelper.WriteLine("Temp files cleaned: " + folderPath);
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }).Start();
        }
    }
}