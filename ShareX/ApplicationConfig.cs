#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace ShareX
{
    public class ApplicationConfig : SettingsBase<ApplicationConfig>
    {
        public TaskSettings DefaultTaskSettings = new TaskSettings();

        public DateTime FirstTimeRunDate = DateTime.Now;
        public string FileUploadDefaultDirectory = "";
        public int NameParserAutoIncrementNumber = 0;
        public List<QuickTaskInfo> QuickTaskPresets = QuickTaskInfo.DefaultPresets;

        public ApplicationConfig()
        {
            this.ApplyDefaultPropertyValues();
        }

        #region Main Form

        public TaskViewMode TaskViewMode = TaskViewMode.ListView;
        public bool ShowMenu = true;
        public bool ShowColumns = true;
        public ImagePreviewVisibility ImagePreview = ImagePreviewVisibility.Automatic;
        public ImagePreviewLocation ImagePreviewLocation = ImagePreviewLocation.Side;
        public int PreviewSplitterDistance = 335;
        public List<int> TaskListViewColumnWidths = new List<int>();
        public DateTime NewsLastReadDate;

        #endregion Main Form

        #region Settings Form

        #region General

        public SupportedLanguage Language = SupportedLanguage.Automatic;
        public bool ShowTray = true;
        public bool SilentRun = false;
        public bool TrayIconProgressEnabled = true;
        public bool TaskbarProgressEnabled = true;
        public bool RememberMainFormPosition = false;
        public Point MainFormPosition = Point.Empty;
        public bool RememberMainFormSize = false;
        public Size MainFormSize = Size.Empty;

        public HotkeyType TrayLeftClickAction = HotkeyType.RectangleRegion;
        public HotkeyType TrayLeftDoubleClickAction = HotkeyType.OpenMainWindow;
        public HotkeyType TrayMiddleClickAction = HotkeyType.PrintScreen;

        public bool CheckPreReleaseUpdates = false;

        #endregion General

        #region Paths

        public bool UseCustomScreenshotsPath = false;
        public string CustomScreenshotsPath = "";

        public string SaveImageSubFolderPattern = "%y-%mo";

        #endregion Paths

        #region Proxy

        public ProxyInfo ProxySettings = new ProxyInfo();

        #endregion Proxy

        #region Upload

        public int UploadLimit = 5;
        public int BufferSizePower = 5;
        public List<ClipboardFormat> ClipboardContentFormats = new List<ClipboardFormat>();

        public int MaxUploadFailRetry = 1;
        public bool UseSecondaryUploaders = false;
        public List<ImageDestination> SecondaryImageUploaders = new List<ImageDestination>();
        public List<TextDestination> SecondaryTextUploaders = new List<TextDestination>();
        public List<FileDestination> SecondaryFileUploaders = new List<FileDestination>();

        #endregion Upload

        #region History

        public bool HistorySaveTasks = true;
        public bool HistoryCheckURL = false;

        public RecentTask[] RecentTasks = null;
        public bool RecentTasksSave = true;
        public int RecentTasksMaxCount = 10;
        public bool RecentTasksShowInMainWindow = true;
        public bool RecentTasksShowInTrayMenu = true;
        public bool RecentTasksTrayMenuMostRecentFirst = false;

        public HistorySettings HistorySettings = new HistorySettings();
        public ImageHistorySettings ImageHistorySettings = new ImageHistorySettings();

        #endregion History

        #region Print

        public bool DontShowPrintSettingsDialog = false;
        public PrintSettings PrintSettings = new PrintSettings();

        #endregion Print

        #region Advanced

        [Category("Application"), DefaultValue(true), Description("Use dark theme. Currently only main window and tray menu supports it.")]
        public bool UseDarkTheme { get; set; }

        [Category("Application"), DefaultValue(false), Description("Use white version of ShareX icon.")]
        public bool UseWhiteShareXIcon { get; set; }

        [Category("Application"), DefaultValue(true), Description("Automatically check updates.")]
#if STEAM || WindowsStore
        [Browsable(false)]
#endif
        public bool AutoCheckUpdate { get; set; }

        [Category("Application"), DefaultValue(false), Description("Calculate and show file sizes in binary units (KiB, MiB etc.)")]
        public bool BinaryUnits { get; set; }

        [Category("Application"), DefaultValue(false), Description("Show most recent task first in main window.")]
        public bool ShowMostRecentTaskFirst { get; set; }

        [Category("Application"), DefaultValue(false), Description("Show only customized tasks in main window workflows.")]
        public bool WorkflowsOnlyShowEdited { get; set; }

        [Category("Application"), DefaultValue(true), Description("Automatically expand capture menu when you open the tray menu.")]
        public bool TrayAutoExpandCaptureMenu { get; set; }

        [Category("Application"), DefaultValue(true), Description("Show tips and hotkeys in main window when task list is empty.")]
        public bool ShowMainWindowTip { get; set; }

        [Category("Application"), DefaultValue(true), Description("Show support us button in main window when task list is empty.")]
        public bool ShowSupportUsButton { get; set; }

        [Category("Application"), DefaultValue(true), Description("Show Discord button in main window when task list is empty.")]
        public bool ShowDiscordButton { get; set; }

        [Category("Application"), DefaultValue(""), Description("URLs will open using this path instead of default browser. Example path: chrome.exe")]
        [Editor(typeof(ExeFileNameEditor), typeof(UITypeEditor))]
        public string BrowserPath { get; set; }

        [Category("Application"), DefaultValue(false), Description("Show version and build info in tray text so if you are running more than one ShareX build you can differentiate them in tray bar.")]
        public bool TrayTextMoreInfo { get; set; }

        [Category("Application"), DefaultValue(false), Description("Save settings after task completed but only if there is no other active tasks.")]
        public bool SaveSettingsAfterTaskCompleted { get; set; }

        [Category("Application"), DefaultValue(false), Description("In main window when task is completed automatically select it.")]
        public bool AutoSelectLastCompletedTask { get; set; }

        [Category("Hotkey"), DefaultValue(false), Description("Disables hotkeys.")]
        public bool DisableHotkeys { get; set; }

        [Category("Hotkey"), DefaultValue(false), Description("If active window is fullscreen then hotkeys won't be executed.")]
        public bool DisableHotkeysOnFullscreen { get; set; }

        private int hotkeyRepeatLimit;

        [Category("Hotkey"), DefaultValue(1000), Description("If you hold hotkeys then it will only trigger every this milliseconds.")]
        public int HotkeyRepeatLimit
        {
            get
            {
                return hotkeyRepeatLimit;
            }
            set
            {
                hotkeyRepeatLimit = Math.Max(value, 200);
            }
        }

        [Category("Clipboard"), DefaultValue(true), Description("Show clipboard content viewer when using clipboard upload in main window.")]
        public bool ShowClipboardContentViewer { get; set; }

        [Category("Clipboard"), DefaultValue(true), Description("Because default .NET image copying not supports alpha channel, background of image will be black. This option will fill background white.")]
        public bool DefaultClipboardCopyImageFillBackground { get; set; }

        [Category("Image"), DefaultValue(true), Description("If JPEG exif contains orientation data then rotate image accordingly.")]
        public bool RotateImageByExifOrientationData { get; set; }

        [Category("Image"), DefaultValue(false), Description("Strip color space information chunks from PNG image.")]
        public bool PNGStripColorSpaceInformation { get; set; }

        [Category("Upload"), DefaultValue(false), Description("Can be used to disable uploading application wide.")]
        public bool DisableUpload { get; set; }

        [Category("Upload"), DefaultValue(false), Description("Accept invalid SSL certificates when uploading.")]
        public bool AcceptInvalidSSLCertificates { get; set; }

        [Category("Upload"), DefaultValue(true), Description("Show first time upload warning.")]
        public bool ShowUploadWarning { get; set; }

        [Category("Upload"), DefaultValue(true), Description("Show more than 10 files upload warning.")]
        public bool ShowMultiUploadWarning { get; set; }

        [Category("Upload"), DefaultValue(100), Description("Large file size defined in MB. ShareX will warn before uploading large files. 0 disables this feature.")]
        public int ShowLargeFileSizeWarning { get; set; }

        [Category("Paths"), Description("Custom uploaders configuration path. If you have already configured this setting in another device and you are attempting to use the same location, then backup the file before configuring this setting and restore after exiting ShareX.")]
        [Editor(typeof(DirectoryNameEditor), typeof(UITypeEditor))]
        public string CustomUploadersConfigPath { get; set; }

        [Category("Paths"), Description("Custom hotkeys configuration path. If you have already configured this setting in another device and you are attempting to use the same location, then backup the file before configuring this setting and restore after exiting ShareX.")]
        [Editor(typeof(DirectoryNameEditor), typeof(UITypeEditor))]
        public string CustomHotkeysConfigPath { get; set; }

        [Category("Paths"), Description("Custom screenshot path (secondary location). If custom screenshot path is temporarily unavailable (e.g. network share), ShareX will use this location (recommended to be a local path).")]
        [Editor(typeof(DirectoryNameEditor), typeof(UITypeEditor))]
        public string CustomScreenshotsPath2 { get; set; }

        [Category("Drag and drop window"), DefaultValue(150), Description("Size of drop window.")]
        public int DropSize { get; set; }

        [Category("Drag and drop window"), DefaultValue(5), Description("Position offset of drop window.")]
        public int DropOffset { get; set; }

        [Category("Drag and drop window"), DefaultValue(ContentAlignment.BottomRight), Description("Where drop window will open.")]
        public ContentAlignment DropAlignment { get; set; }

        [Category("Drag and drop window"), DefaultValue(100), Description("Opacity of drop window.")]
        public int DropOpacity { get; set; }

        [Category("Drag and drop window"), DefaultValue(255), Description("When you drag file to drop window then opacity will change to this.")]
        public int DropHoverOpacity { get; set; }

        #endregion Advanced

        #endregion Settings Form

        #region AutoCapture Form

        public Rectangle AutoCaptureRegion = Rectangle.Empty;
        public decimal AutoCaptureRepeatTime = 60;
        public bool AutoCaptureMinimizeToTray = true;
        public bool AutoCaptureWaitUpload = true;

        #endregion AutoCapture Form

        #region ScreenRecord Form

        public Rectangle ScreenRecordRegion = Rectangle.Empty;

        #endregion ScreenRecord Form

        #region Actions toolbar

        public List<HotkeyType> ActionsToolbarList = new List<HotkeyType>() { HotkeyType.RectangleRegion, HotkeyType.PrintScreen, HotkeyType.ScreenRecorder,
            HotkeyType.None, HotkeyType.FileUpload, HotkeyType.ClipboardUploadWithContentViewer };

        public bool ActionsToolbarRunAtStartup = false;

        public Point ActionsToolbarPosition = Point.Empty;

        public bool ActionsToolbarLockPosition = false;

        public bool ActionsToolbarStayTopMost = true;

        #endregion Actions toolbar

        #region Color Picker Form

        public List<Color> RecentColors = new List<Color>();

        #endregion Color Picker Form
    }
}