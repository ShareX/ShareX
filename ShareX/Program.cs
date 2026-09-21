#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

using ShareX.AvaloniaUI.Integration;
using ShareX.HelpersLib;
using ShareX.HistoryLib;
using ShareX.ImageEditor.Integration;
using ShareX.Localization;
using ShareX.UploadersLib;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageBox = ShareX.AvaloniaUI.MessageBox;
using MessageBoxButtons = ShareX.AvaloniaUI.MessageBoxButtons;
using MessageBoxIcon = ShareX.AvaloniaUI.MessageBoxIcon;

#if MicrosoftStore
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
#endif

namespace ShareX
{
    internal static class Program
    {
        public const string AppName = "ShareX";
        public const string MutexName = "82E6AC09-0FEF-4390-AD9F-0DD3F5561EFC";
        public static readonly string PipeName = $"{Environment.MachineName}-{Environment.UserName}-{AppName}";

        public const ShareXBuild Build =
#if RELEASE
            ShareXBuild.Release;
#elif STEAM
            ShareXBuild.Steam;
#elif MicrosoftStore
            ShareXBuild.MicrosoftStore;
#elif DEBUG
            ShareXBuild.Debug;
#else
            ShareXBuild.Unknown;
#endif

        public static string VersionText
        {
            get
            {
                StringBuilder sbVersionText = new StringBuilder();
                Version version = Version.Parse(Application.ProductVersion);
                sbVersionText.Append(version.Major + "." + version.Minor);
                if (version.Build > 0 || version.Revision > 0) sbVersionText.Append("." + version.Build);
                if (version.Revision > 0) sbVersionText.Append("." + version.Revision);
                if (Dev) sbVersionText.Append(" Dev");
                if (Portable) sbVersionText.Append(" Portable");
                return sbVersionText.ToString();
            }
        }

        public static string Title
        {
            get
            {
                string title = $"{AppName} {VersionText}";

                if (Settings != null && Settings.DevMode)
                {
                    string info = Build.ToString();

                    if (IsAdmin)
                    {
                        info += ", Admin";
                    }

                    title += $" ({info})";
                }

                return title;
            }
        }

        public static string TitleShort
        {
            get
            {
                if (Settings != null && Settings.DevMode)
                {
                    return Title;
                }

                return AppName;
            }
        }

        public static bool Dev { get; } = true;
        public static bool MultiInstance { get; private set; }
        public static bool Portable { get; private set; }
        public static bool SilentRun { get; private set; }
        public static bool Sandbox { get; private set; }
        public static bool IsAdmin { get; private set; }
        public static bool IgnoreHotkeyWarning { get; private set; }

        internal static ApplicationConfig Settings { get; set; }
        internal static TaskSettings DefaultTaskSettings { get; set; }
        internal static UploadersConfig UploadersConfig { get; set; }
        internal static HotkeysConfig HotkeysConfig { get; set; }
        internal static HistoryManagerSQLite HistoryManager { get; set; }

        internal static Stopwatch StartTimer { get; private set; }
        internal static HotkeyManager HotkeyManager { get; set; }
        internal static WatchFolderManager WatchFolderManager { get; set; }
        internal static ShareXUpdateManager UpdateManager { get; private set; }
        internal static ShareXCLIManager CLI { get; private set; }

        private static MainForm hotkeyForm;

        #region Paths

        private const string PersonalPathConfigFileName = "PersonalPath.cfg";

        public static readonly string DefaultPersonalFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppName);
        public static readonly string PortablePersonalFolder = FileHelpers.GetAbsolutePath(AppName);

        private static string PersonalPathConfigFilePath
        {
            get
            {
                string relativePath = FileHelpers.GetAbsolutePath(PersonalPathConfigFileName);

                if (File.Exists(relativePath))
                {
                    return relativePath;
                }

                return CurrentPersonalPathConfigFilePath;
            }
        }

        private static readonly string CurrentPersonalPathConfigFilePath = Path.Combine(DefaultPersonalFolder, PersonalPathConfigFileName);

        private static readonly string PreviousPersonalPathConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            AppName, PersonalPathConfigFileName);

        private static readonly string PortableCheckFilePath = FileHelpers.GetAbsolutePath("Portable");
        public static readonly string SteamInAppFilePath = FileHelpers.GetAbsolutePath("Steam");

        private static string CustomPersonalPath { get; set; }

        public static string PersonalFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(CustomPersonalPath))
                {
                    return FileHelpers.ExpandFolderVariables(CustomPersonalPath);
                }

                return DefaultPersonalFolder;
            }
        }

        public const string HistoryFileName = "History.db";

        public static string HistoryFilePath
        {
            get
            {
                if (Sandbox) return null;

                return Path.Combine(PersonalFolder, HistoryFileName);
            }
        }

        public const string HistoryFileNameOld = "History.json";

        public static string HistoryFilePathOld
        {
            get
            {
                if (Sandbox) return null;

                return Path.Combine(PersonalFolder, HistoryFileNameOld);
            }
        }

        public const string LogsFolderName = "Logs";

        public static string LogsFolder => Path.Combine(PersonalFolder, LogsFolderName);

        public static string LogsFilePath
        {
            get
            {
                if (SystemOptions.DisableLogging)
                {
                    return null;
                }

                string fileName = string.Format("ShareX-Log-{0:yyyy-MM}.txt", DateTime.Now);
                return Path.Combine(LogsFolder, fileName);
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
                        path = FileHelpers.ExpandFolderVariables(path);

                        if (string.IsNullOrEmpty(path2) || Directory.Exists(path))
                        {
                            return path;
                        }
                    }

                    if (!string.IsNullOrEmpty(path2))
                    {
                        path2 = FileHelpers.ExpandFolderVariables(path2);

                        if (Directory.Exists(path2))
                        {
                            return path2;
                        }
                    }
                }

                return Path.Combine(PersonalFolder, "Screenshots");
            }
        }

        public static string ImageEffectsFolder => Path.Combine(PersonalFolder, "ImageEffects");
        public static string ModelsFolder => Path.Combine(PersonalFolder, "Models");

        private static string PersonalPathDetectionMethod;

        #endregion Paths

        private static volatile bool applicationReady;
        private static bool closeSequenceStarted, exitStarted, restartRequested, restartAsAdmin;

        internal static bool IsClosing => closeSequenceStarted || exitStarted;

        [STAThread]
        private static void Main(string[] args)
        {
            StartupWork();

            StartTimer = Stopwatch.StartNew();

            CLI = new ShareXCLIManager(args);
            CLI.ParseCommands();

#if STEAM
            if (CheckUninstall()) return; // Steam will run ShareX with -Uninstall when uninstalling
#endif

            SystemOptions.UpdateSystemOptions();
            UpdatePersonalPath();

            DebugHelper.Init(LogsFilePath);

            IsAdmin = Helpers.IsAdministrator();
            MultiInstance = CLI.IsCommandExist("multi", "m");

            using (SingleInstanceManager singleInstanceManager = new SingleInstanceManager(MutexName, PipeName, !MultiInstance, args))
            {
                if (!singleInstanceManager.IsSingleInstance || singleInstanceManager.IsFirstInstance)
                {
                    singleInstanceManager.ArgumentsReceived += SingleInstanceManager_ArgumentsReceived;

                    Run(args);
                }
            }

            HandleRestart();

            DebugHelper.Flush();
        }

        private static void Run(string[] args)
        {
            ApplicationConfiguration.Initialize();

            DebugHelper.WriteLine("ShareX starting.");
            DebugHelper.WriteLine("Version: " + VersionText);
            DebugHelper.WriteLine("Build: " + Build);
            DebugHelper.WriteLine("Command line: " + Environment.CommandLine);
            DebugHelper.WriteLine("Personal path: " + PersonalFolder);
            if (!string.IsNullOrEmpty(PersonalPathDetectionMethod))
            {
                DebugHelper.WriteLine("Personal path detection method: " + PersonalPathDetectionMethod);
            }
            DebugHelper.WriteLine("Operating system: " + Helpers.GetOperatingSystemProductName(true));
            DebugHelper.WriteLine(".NET version: " + Environment.Version);
            DebugHelper.WriteLine("Running as elevated process: " + IsAdmin);

            SilentRun = CLI.IsCommandExist("silent", "s");
#if MicrosoftStore
            SilentRun = SilentRun || AppInstance.GetActivatedEventArgs()?.Kind == ActivationKind.StartupTask;
#endif

            IgnoreHotkeyWarning = CLI.IsCommandExist("NoHotkeys");

            CreateParentFolders();
            RegisterExtensions();
            DebugWriteFlags();

            DebugHelper.WriteLine("Avalonia application initializing.");
            AvaloniaBootstrapper.Initialize(args, StartApplication, StopApplication);

            SettingManager.LoadInitialSettings();

            UpdateManager = new ShareXUpdateManager();
            LanguageHelper.ChangeLanguage(Settings.Language);
            CleanupManager.CleanupAsync();

            DebugHelper.WriteLine("Avalonia application starting.");
            AvaloniaBootstrapper.Run();

            CloseSequence();
        }

        private static async Task StartApplication()
        {
            ImageEditorIntegration.Initialize();

            if (Settings.ShowStartScreen)
            {
                DebugHelper.WriteLine("Start screen opening.");
                StartScreenWindow startScreen = new StartScreenWindow();
                await startScreen.ShowAsync();
                DebugHelper.WriteLine("Start screen closed.");
            }

            DebugHelper.WriteLine("Hotkey host init started.");
            hotkeyForm = new MainForm();
            hotkeyForm.Initialize();

            await UpdateApplicationAsync();

            bool showMainWindow = !(SilentRun || Settings.SilentRun) || !Settings.ShowTray;
            MainWindowIntegration.Initialize(hotkeyForm.TrayIconService, showMainWindow);

            ShareX.Tools.MouseHighlighterManager.ActivateOnStartup(DefaultTaskSettings.ToolsSettings.MouseHighlighterOptions);

            if (showMainWindow)
            {
                MainWindowIntegration.Activate();
            }

            DebugHelper.WriteLine("Startup time: {0} ms", StartTimer.ElapsedMilliseconds);

            await CLI.UseCommandLineArgs();

            if (Settings.ActionsToolbarRunAtStartup)
            {
                TaskHelpers.OpenActionsToolbar();
            }

            DebugHelper.WriteLine("Hotkey host init finished.");
        }

        private static void StopApplication()
        {
            if (hotkeyForm is { IsDisposed: false })
            {
                hotkeyForm.ExitApplication();
            }
        }

        internal static async Task UpdateApplicationAsync()
        {
            applicationReady = false;

            TaskManager.RecentManager.InitItems();
            TaskbarManager.Enabled = Settings.TaskbarProgressEnabled;

            ApplyApplicationSettings();
            await hotkeyForm.UpdateHotkeysAsync();

            WatchFolderManager ??= new WatchFolderManager();
            WatchFolderManager.UpdateWatchFolders();
            DebugHelper.WriteLine("WatchFolderManager started.");

            MainWindowIntegration.RefreshMenus();
            applicationReady = true;
        }

        internal static void ApplyApplicationSettings()
        {
            HelpersOptions.CurrentProxy = Settings.ProxySettings;
            HelpersOptions.URLEncodeIgnoreEmoji = Settings.URLEncodeIgnoreEmoji;
            HelpersOptions.DefaultCopyImageFillBackground = Settings.DefaultClipboardCopyImageFillBackground;
            HelpersOptions.UseAlternativeClipboardCopyImage = Settings.UseAlternativeClipboardCopyImage;
            HelpersOptions.UseAlternativeClipboardGetImage = Settings.UseAlternativeClipboardGetImage;
            HelpersOptions.RotateImageByExifOrientationData = Settings.RotateImageByExifOrientationData;
            HelpersOptions.BrowserPath = Settings.BrowserPath;
            HelpersOptions.RecentColors = Settings.RecentColors;
            HelpersOptions.DevMode = Settings.DevMode;
            UpdateHelpersSpecialFolders();

            TaskManager.RecentManager.MaxCount = Settings.RecentTasksMaxCount;
            hotkeyForm?.ApplySettings();

            UpdateManager.AllowAutoUpdate = !SystemOptions.DisableUpdateCheck && Settings.AutoCheckUpdate;
            UpdateManager.UpdateChannel = Settings.UpdateChannel;
            UpdateManager.ConfigureAutoUpdate();

            MainWindowIntegration.SetTitle(Title);
            MainWindowIntegration.SetTrayVisible(Settings.ShowTray);
            MainWindowIntegration.RefreshMenus();
        }

        internal static void UpdateTrayIcon() => hotkeyForm?.UpdateTrayIcon();

        internal static void OnMainFormClosed()
        {
            exitStarted = true;
            ShareX.Tools.MouseHighlighterManager.Shutdown();
            MainWindowIntegration.Close();
            TaskManager.StopAllTasks();
            AvaloniaBootstrapper.Shutdown();
        }

        public static void CloseSequence()
        {
            if (!closeSequenceStarted)
            {
                closeSequenceStarted = true;

                DebugHelper.WriteLine("ShareX closing.");

                WatchFolderManager?.Dispose();
                SettingManager.HistoryClose();
                SettingManager.SaveAllSettings();

                DebugHelper.WriteLine("ShareX closed.");
            }
        }

        private static void HandleRestart()
        {
            if (restartRequested)
            {
                DebugHelper.WriteLine("ShareX restarting.");

                if (restartAsAdmin)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Application.ExecutablePath,
                        Arguments = "-silent",
                        UseShellExecute = true,
                        Verb = "runas"
                    });
                }
                else
                {
                    Process.Start(Application.ExecutablePath);
                }
            }
        }

        public static void Restart(bool asAdmin = false)
        {
            restartRequested = true;
            restartAsAdmin = asAdmin;
            Exit();
        }

        public static void Exit()
        {
            void ExitCore()
            {
                exitStarted = true;

                if (hotkeyForm is { IsDisposed: false })
                {
                    hotkeyForm.ExitApplication();
                }
                else
                {
                    AvaloniaBootstrapper.Shutdown();
                }
            }

            if (Dispatcher.UIThread.CheckAccess())
            {
                ExitCore();
            }
            else
            {
                Dispatcher.UIThread.Post(ExitCore);
            }
        }

        public static void ForceClose()
        {
            if (!Dispatcher.UIThread.CheckAccess())
            {
                Dispatcher.UIThread.Post(ForceClose);
                return;
            }

            if (ScreenRecordManager.IsRecording)
            {
                if (MessageBox.Show(Strings.ShareXCannotBeClosedWhileScreenRecordingIsActive, AppName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == ShareX.AvaloniaUI.DialogResult.Yes)
                {
                    ScreenRecordManager.AbortRecording();
                }

                return;
            }

            Exit();
        }

        private static void SingleInstanceManager_ArgumentsReceived(string[] arguments)
        {
            string message = "Arguments received: ";

            if (arguments == null)
            {
                message += "null";
            }
            else
            {
                message += "\"" + string.Join(" ", arguments) + "\"";
            }

            DebugHelper.WriteLine(message);

            if (WaitFormLoad(5000))
            {
                Dispatcher.UIThread.Post(async () =>
                {
                    await UseCommandLineArgs(arguments);
                });
            }
        }

        private static bool WaitFormLoad(int wait)
        {
            Stopwatch timer = Stopwatch.StartNew();

            while (timer.ElapsedMilliseconds < wait)
            {
                if (applicationReady) return true;

                Thread.Sleep(10);
            }

            return false;
        }

        private static async Task UseCommandLineArgs(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                if (Program.Settings.ShowTray)
                {
                    // Workaround for Windows startup tray icon bug
                    MainWindowIntegration.SetTrayVisible(false);
                    MainWindowIntegration.SetTrayVisible(true);
                }

                MainWindowIntegration.Activate();
            }
            else if (MainWindowIntegration.IsVisible)
            {
                MainWindowIntegration.Activate();
            }

            CLIManager cli = new CLIManager(args);
            cli.ParseCommands();

            await CLI.UseCommandLineArgs(cli.Commands);
        }

        private static void UpdatePersonalPath()
        {
            Sandbox = CLI.IsCommandExist("sandbox");

            if (!Sandbox)
            {
                if (CLI.IsCommandExist("portable", "p"))
                {
                    Portable = true;
                    CustomPersonalPath = PortablePersonalFolder;
                    PersonalPathDetectionMethod = "Portable CLI flag";
                }
                else if (File.Exists(PortableCheckFilePath))
                {
                    Portable = true;
                    CustomPersonalPath = PortablePersonalFolder;
                    PersonalPathDetectionMethod = $"Portable file ({PortableCheckFilePath})";
                }
                else if (!string.IsNullOrEmpty(SystemOptions.PersonalPath))
                {
                    CustomPersonalPath = SystemOptions.PersonalPath;
                    PersonalPathDetectionMethod = "Registry";
                }
                else
                {
#if !MicrosoftStore
                    MigratePersonalPathConfig();
#endif

                    string customPersonalPath = ReadPersonalPathConfig();

                    if (!string.IsNullOrEmpty(customPersonalPath))
                    {
                        CustomPersonalPath = FileHelpers.GetAbsolutePath(customPersonalPath);
                        PersonalPathDetectionMethod = $"PersonalPath.cfg file ({PersonalPathConfigFilePath})";
                    }
                }

                if (!Directory.Exists(PersonalFolder))
                {
                    try
                    {
                        Directory.CreateDirectory(PersonalFolder);
                    }
                    catch (Exception e)
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.AppendFormat("{0} \"{1}\"", Strings.Program_Run_Unable_to_create_folder_, PersonalFolder);
                        sb.AppendLine();

                        if (!string.IsNullOrEmpty(PersonalPathDetectionMethod))
                        {
                            sb.AppendLine("Personal path detection method: " + PersonalPathDetectionMethod);
                        }

                        sb.AppendLine();
                        sb.Append(e);

                        MessageBox.Show(sb.ToString(), "ShareX - " + Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        CustomPersonalPath = "";
                    }
                }
            }
        }

        private static void CreateParentFolders()
        {
            if (!Sandbox && Directory.Exists(PersonalFolder))
            {
                FileHelpers.CreateDirectory(SettingManager.BackupFolder);
                FileHelpers.CreateDirectory(ImageEffectsFolder);
                FileHelpers.CreateDirectory(ScreenshotsParentFolder);
            }
        }

        private static void RegisterExtensions()
        {
#if !MicrosoftStore
            if (!Portable)
            {
                if (!IntegrationHelpers.CheckCustomUploaderExtension())
                {
                    IntegrationHelpers.CreateCustomUploaderExtension(true);
                }

                if (!IntegrationHelpers.CheckImageEffectExtension())
                {
                    IntegrationHelpers.CreateImageEffectExtension(true);
                }
            }
#endif
        }

        public static void UpdateHelpersSpecialFolders()
        {
            Dictionary<string, string> specialFolders = new Dictionary<string, string>();
            specialFolders.Add("ShareXImageEffects", ImageEffectsFolder);
            HelpersOptions.ShareXSpecialFolders = specialFolders;
        }

        private static void MigratePersonalPathConfig()
        {
            if (File.Exists(PreviousPersonalPathConfigFilePath))
            {
                try
                {
                    if (!File.Exists(CurrentPersonalPathConfigFilePath))
                    {
                        FileHelpers.CreateDirectoryFromFilePath(CurrentPersonalPathConfigFilePath);
                        File.Move(PreviousPersonalPathConfigFilePath, CurrentPersonalPathConfigFilePath);
                    }

                    File.Delete(PreviousPersonalPathConfigFilePath);
                    Directory.Delete(Path.GetDirectoryName(PreviousPersonalPathConfigFilePath));
                }
                catch (Exception e)
                {
                    e.ShowError();
                }
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

        public static bool WritePersonalPathConfig(string path)
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

                if (!path.Equals(currentPath, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        FileHelpers.CreateDirectoryFromFilePath(PersonalPathConfigFilePath);
                        File.WriteAllText(PersonalPathConfigFilePath, path, Encoding.UTF8);
                        return true;
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        DebugHelper.WriteException(e);
                        MessageBox.Show(string.Format(Strings.Program_WritePersonalPathConfig_Cant_access_to_file, PersonalPathConfigFilePath),
                            "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (Exception e)
                    {
                        DebugHelper.WriteException(e);
                        e.ShowError();
                    }
                }
            }

            return false;
        }

        private static void StartupWork()
        {
            AssemblyLoadContext.Default.Resolving += (ctx, asmName) =>
            {
                if (string.IsNullOrEmpty(asmName.CultureName) || !asmName.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                string baseDir = AppContext.BaseDirectory;
                string path = Path.Combine(baseDir, "Languages", asmName.CultureName, asmName.Name + ".dll");
                return File.Exists(path) ? ctx.LoadFromAssemblyPath(path) : null;
            };

#if DEBUG
            if (Debugger.IsAttached)
            {
                return;
            }
#endif

            // Add the event handler for handling UI thread exceptions to the event
            Application.ThreadException += Application_ThreadException;

            // Set the unhandled exception mode to force all Windows Forms errors to go through our handler
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
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
            ErrorWindowIntegration.Show(e.Message, $"{e}\r\n\r\n{Title}", LogsFilePath, Links.GitHubIssues);
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

        private static void DebugWriteFlags()
        {
            List<string> flags = new List<string>();

            if (Dev) flags.Add(nameof(Dev));
            if (MultiInstance) flags.Add(nameof(MultiInstance));
            if (Portable) flags.Add(nameof(Portable));
            if (SilentRun) flags.Add(nameof(SilentRun));
            if (Sandbox) flags.Add(nameof(Sandbox));
            if (IgnoreHotkeyWarning) flags.Add(nameof(IgnoreHotkeyWarning));
            if (SystemOptions.DisableUpdateCheck) flags.Add(nameof(SystemOptions.DisableUpdateCheck));
            if (SystemOptions.DisableUpload) flags.Add(nameof(SystemOptions.DisableUpload));
            if (SystemOptions.DisableLogging) flags.Add(nameof(SystemOptions.DisableLogging));

            string output = string.Join(", ", flags);

            if (!string.IsNullOrEmpty(output))
            {
                DebugHelper.WriteLine("Flags: " + output);
            }
        }
    }
}
