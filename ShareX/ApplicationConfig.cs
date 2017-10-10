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
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
using ShareX.UploadersLib.OtherServices;
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

        public string FileUploadDefaultDirectory = "";
        public bool ShowUploadWarning = true; // First time upload warning
        public bool ShowMultiUploadWarning = true; // More than 10 files upload warning
        public int NameParserAutoIncrementNumber = 0;
        public bool DisableHotkeys = false;
        public List<QuickTaskInfo> QuickTaskPresets = QuickTaskInfo.DefaultPresets;
        public bool ShowPatreonButton { get; set; } = true;
        public bool ShowDiscordButton { get; set; } = true;

        public ApplicationConfig()
        {
            this.ApplyDefaultPropertyValues();
        }

        #region Main Form

        public bool ShowMenu = true;
        public bool ShowColumns = true;
        public ImagePreviewVisibility ImagePreview = ImagePreviewVisibility.Automatic;
        public int PreviewSplitterDistance = 335;
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

        #region Export / Import

        public bool ExportSettings = true;
        public bool ExportHistory = true;
        public bool ExportLogs = false;

        #endregion Export / Import

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

        public WindowState HistoryWindowState = new WindowState();
        public int HistoryMaxItemCount = 0;
        public int HistorySplitterDistance = 550;

        public WindowState ImageHistoryWindowState = new WindowState();
        public int ImageHistoryViewMode = 3;
        public Size ImageHistoryThumbnailSize = new Size(150, 150);
        public int ImageHistoryMaxItemCount = 250;

        #endregion History

        #region Print

        public bool DontShowPrintSettingsDialog = false;
        public PrintSettings PrintSettings = new PrintSettings();

        #endregion Print

        #region Profiles

        public List<VideoEncoder> VideoEncoders = new List<VideoEncoder>();

        #endregion Profiles

        #region Advanced

        [Category("Application"), DefaultValue(false), Description("Calculate and show file sizes in binary units (KiB, MiB etc.)")]
        public bool BinaryUnits { get; set; }

        [Category("Application"), DefaultValue(false), Description("Show most recent task first in main window.")]
        public bool ShowMostRecentTaskFirst { get; set; }

        [Category("Application"), DefaultValue(true), Description("Default .NET method can't copy image with alpha channel to clipboard. Alternatively, when this setting is false, ShareX copies \"PNG\" and 32 bit \"DIB\" to clipboard in order to retain image transparency. If you are experiencing issues then set this setting to true to use the default .NET method.")]
        public bool UseDefaultClipboardCopyImage { get; set; }

        [Category("Application"), DefaultValue(true), Description("Default .NET method can't get image with alpha channel from clipboard. Alternatively, when this setting is false, ShareX checks if clipboard contains \"PNG\" or 32 bit \"DIB\" in order to retain image transparency. If you are experiencing issues then set this setting to true to use the default .NET method.")]
        public bool UseDefaultClipboardGetImage { get; set; }

        [Category("Application"), DefaultValue(true), Description("Because default .NET image copying not supports alpha channel, background of image will be black. This option will fill background white.")]
        public bool DefaultClipboardCopyImageFillBackground { get; set; }

        [Category("Application"), DefaultValue(false), Description("Show only customized tasks in main window workflows.")]
        public bool WorkflowsOnlyShowEdited { get; set; }

        [Category("Application"), DefaultValue(true), Description("Automatically check updates.")]
#if STEAM || WindowsStore
        [Browsable(false)]
#endif
        public bool AutoCheckUpdate { get; set; }

        [Category("Application"), DefaultValue(true), Description("Automatically expand capture menu when you open the tray menu.")]
        public bool TrayAutoExpandCaptureMenu { get; set; }

        [Category("Application"), DefaultValue(true), Description("Show tips in main window list when list is empty.")]
        public bool ShowMainWindowTip { get; set; }

        [Category("Application"), DefaultValue(100), Description("Large file size defined in MiB or MB. ShareX will warn before uploading large files. 0 disables this feature.")]
        public int LargeFileSizeWarning { get; set; }

        [Category("Application"), DefaultValue(""), Description("URLs will open using this path instead of default browser. Example path: chrome.exe")]
        [Editor(typeof(ExeFileNameEditor), typeof(UITypeEditor))]
        public string BrowserPath { get; set; }

        [Category("Application"), DefaultValue(false), Description("Show version and build info in tray text so if you are running more than one ShareX build you can differentiate them in tray bar.")]
        public bool TrayTextMoreInfo { get; set; }

        [Category("Upload"), DefaultValue(false), Description("Can be used to disable uploading application wide.")]
        public bool DisableUpload { get; set; }

        [Category("Upload"), DefaultValue(false), Description("Accept invalid SSL certificates when uploading.")]
        public bool AcceptInvalidSSLCertificates { get; set; }

        [Category("Application"), DefaultValue(true), Description("Save settings after task completed but only if there is no other active tasks. This setting will be handy for situations where setting save fails when Windows shutdown and not let ShareX to save in time.")]
        public bool SaveSettingsAfterTaskCompleted { get; set; }

        [Category("Clipboard upload"), DefaultValue(true), Description("Show clipboard content viewer when using clipboard upload in main window.")]
        public bool ShowClipboardContentViewer { get; set; }

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

        #region Webpage Capture Form

        public WebpageCaptureOptions WebpageCaptureOptions = new WebpageCaptureOptions();

        #endregion Webpage Capture Form

        #region OCR Form

        public OCRSpaceLanguages OCRLanguage = OCRSpaceLanguages.eng;

        #endregion OCR Form

        #region Actions toolbar

        public List<HotkeyType> ActionsToolbarList = new List<HotkeyType>() { HotkeyType.RectangleRegion, HotkeyType.PrintScreen, HotkeyType.ScreenRecorder,
            HotkeyType.None, HotkeyType.FileUpload, HotkeyType.ClipboardUploadWithContentViewer };

        public bool ActionsToolbarRunAtStartup = false;

        public Point ActionsToolbarPosition = Point.Empty;

        public bool ActionsToolbarLockPosition = false;

        public bool ActionsToolbarStayTopMost = true;

        #endregion Actions toolbar
    }
}