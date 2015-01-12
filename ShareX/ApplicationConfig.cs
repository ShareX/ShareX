#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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
using ShareX.UploadersLib;
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

        public ApplicationConfig()
        {
            this.ApplyDefaultPropertyValues();
        }

        #region Main Form

        public bool ShowMenu = true;
        public ImagePreviewVisibility ImagePreview = ImagePreviewVisibility.Automatic;
        public int PreviewSplitterDistance = 335;

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

        #endregion General

        #region Paths

        public bool UseCustomScreenshotsPath = false;
        public string CustomScreenshotsPath = string.Empty;
        public string SaveImageSubFolderPattern = "%y-%mo";

        #endregion Paths

        #region Proxy

        public ProxyInfo ProxySettings = new ProxyInfo();

        #endregion Proxy

        #region Upload

        public int UploadLimit = 5;
        public int BufferSizePower = 5;
        public List<ClipboardFormat> ClipboardContentFormats = new List<ClipboardFormat>();

        public int MaxUploadFailRetry = 0;
        public bool UseSecondaryUploaders = false;
        public List<ImageDestination> SecondaryImageUploaders = new List<ImageDestination>();
        public List<TextDestination> SecondaryTextUploaders = new List<TextDestination>();
        public List<FileDestination> SecondaryFileUploaders = new List<FileDestination>();

        #endregion Upload

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

        [Category("Application"), DefaultValue(false), Description("Show most recent task first.")]
        public bool ShowMostRecentTaskFirst { get; set; }

        [Category("Application"), DefaultValue(false), Description("By default copying \"Bitmap\" to clipboard. Alternative method copying \"PNG and DIB\" to clipboard.")]
        public bool UseAlternativeClipboardCopyImage { get; set; }

        [Category("Application"), DefaultValue(false), Description("Show only customized tasks in main window workflows.")]
        public bool WorkflowsOnlyShowEdited { get; set; }

        [Category("Application"), DefaultValue(true), Description("Automatically check updates at startup.")]
        public bool AutoCheckUpdate { get; set; }

        [Category("Application"), DefaultValue(true), Description("Automatically expand capture menu when you open the tray menu.")]
        public bool TrayAutoExpandCaptureMenu { get; set; }

        [Category("Application"), DefaultValue(HotkeyType.RectangleRegion), Description("You can set which action to happen when you middle click tray icon."), TypeConverter(typeof(EnumDescriptionConverter))]
        public HotkeyType TrayMiddleClickAction { get; set; }

        [Category("Application"), DefaultValue(true), Description("Show tips in main window list when list is empty.")]
        public bool ShowMainWindowTip { get; set; }

        [Category("Application"), DefaultValue(""), Description("URLs will open using this path instead of default browser. Example path: chrome.exe")]
        [Editor(typeof(ExeFileNameEditor), typeof(UITypeEditor))]
        public string BrowserPath { get; set; }

        [Category("Application / Config"), DefaultValue(false), Description("Automatically detect external changes to UploaderConfig file and load settigns to memory.")]
        public bool DetectUploaderConfigFileChanges { get; set; }

        [Category("Clipboard upload"), DefaultValue(true), Description("Show clipboard content viewer when using clipboard upload in main window.")]
        public bool ShowClipboardContentViewer { get; set; }

        [Category("Paths"), Description("Custom uploaders configuration path. If you have already configured this setting in another device and you are attempting to use the same location, then backup the file before configuring this setting and restore after exiting ShareX.")]
        [Editor(typeof(DirectoryNameEditor), typeof(UITypeEditor))]
        public string CustomUploadersConfigPath { get; set; }

        [Category("Paths"), Description("Custom hotkeys configuration path. If you have already configured this setting in another device and you are attempting to use the same location, then backup the file before configuring this setting and restore after exiting ShareX.")]
        [Editor(typeof(DirectoryNameEditor), typeof(UITypeEditor))]
        public string CustomHotkeysConfigPath { get; set; }

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

        #region History Form

        public WindowState HistoryWindowState = new WindowState();
        public WindowState ImageHistoryWindowState = new WindowState();
        public int ImageHistoryMaxItemCount = 100;
        public int ImageHistoryViewMode = 3;
        public Size ImageHistoryThumbnailSize = new Size(100, 100);

        #endregion History Form

        #region AutoCapture Form

        public Rectangle AutoCaptureRegion = Rectangle.Empty;
        public decimal AutoCaptureRepeatTime = 60;
        public bool AutoCaptureMinimizeToTray = true;
        public bool AutoCaptureWaitUpload = true;

        #endregion AutoCapture Form

        #region ScreenRecord Form

        public Rectangle ScreenRecordRegion = Rectangle.Empty;

        #endregion ScreenRecord Form
    }
}