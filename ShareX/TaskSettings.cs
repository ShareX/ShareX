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

using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.ImageEffectsLib;
using ShareX.IndexerLib;
using ShareX.MediaLib;
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;

namespace ShareX
{
    public class TaskSettings
    {
        [JsonIgnore]
        public TaskSettings TaskSettingsReference { get; private set; }

        [JsonIgnore]
        public bool IsSafeTaskSettings => TaskSettingsReference != null;

        public string Description = "";

        public HotkeyType Job = HotkeyType.None;

        public bool UseDefaultAfterCaptureJob = true;
        public AfterCaptureTasks AfterCaptureJob = AfterCaptureTasks.CopyImageToClipboard | AfterCaptureTasks.SaveImageToFile | AfterCaptureTasks.UploadImageToHost;

        public bool UseDefaultAfterUploadJob = true;
        public AfterUploadTasks AfterUploadJob = AfterUploadTasks.CopyURLToClipboard;

        public bool UseDefaultDestinations = true;
        public ImageDestination ImageDestination = ImageDestination.Imgur;
        public FileDestination ImageFileDestination = FileDestination.Dropbox;
        public TextDestination TextDestination = TextDestination.Pastebin;
        public FileDestination TextFileDestination = FileDestination.Dropbox;
        public FileDestination FileDestination = FileDestination.Dropbox;
        public UrlShortenerType URLShortenerDestination = UrlShortenerType.BITLY;
        public URLSharingServices URLSharingServiceDestination = URLSharingServices.Twitter;

        public bool OverrideFTP = false;
        public int FTPIndex = 0;

        public bool OverrideCustomUploader = false;
        public int CustomUploaderIndex = 0;

        public bool OverrideScreenshotsFolder = false;
        public string ScreenshotsFolder = "";

        public bool UseDefaultGeneralSettings = true;
        public TaskSettingsGeneral GeneralSettings = new TaskSettingsGeneral();

        public bool UseDefaultImageSettings = true;
        public TaskSettingsImage ImageSettings = new TaskSettingsImage();

        [JsonIgnore]
        public TaskSettingsImage ImageSettingsReference
        {
            get
            {
                if (UseDefaultImageSettings)
                {
                    return Program.DefaultTaskSettings.ImageSettings;
                }

                return TaskSettingsReference.ImageSettings;
            }
        }

        public bool UseDefaultCaptureSettings = true;
        public TaskSettingsCapture CaptureSettings = new TaskSettingsCapture();

        [JsonIgnore]
        public TaskSettingsCapture CaptureSettingsReference
        {
            get
            {
                if (UseDefaultCaptureSettings)
                {
                    return Program.DefaultTaskSettings.CaptureSettings;
                }

                return TaskSettingsReference.CaptureSettings;
            }
        }

        public bool UseDefaultUploadSettings = true;
        public TaskSettingsUpload UploadSettings = new TaskSettingsUpload();

        public bool UseDefaultActions = true;
        public List<ExternalProgram> ExternalPrograms = new List<ExternalProgram>();

        public bool UseDefaultToolsSettings = true;
        public TaskSettingsTools ToolsSettings = new TaskSettingsTools();

        [JsonIgnore]
        public TaskSettingsTools ToolsSettingsReference
        {
            get
            {
                if (UseDefaultToolsSettings)
                {
                    return Program.DefaultTaskSettings.ToolsSettings;
                }

                return TaskSettingsReference.ToolsSettings;
            }
        }

        public bool UseDefaultAdvancedSettings = true;
        public TaskSettingsAdvanced AdvancedSettings = new TaskSettingsAdvanced();

        public bool WatchFolderEnabled = false;
        public List<WatchFolderSettings> WatchFolderList = new List<WatchFolderSettings>();

        public override string ToString()
        {
            return !string.IsNullOrEmpty(Description) ? Description : Job.GetLocalizedDescription();
        }

        public bool IsUsingDefaultSettings
        {
            get
            {
                return UseDefaultAfterCaptureJob && UseDefaultAfterUploadJob && UseDefaultDestinations && !OverrideFTP && !OverrideCustomUploader &&
                    !OverrideScreenshotsFolder && UseDefaultGeneralSettings && UseDefaultImageSettings && UseDefaultCaptureSettings && UseDefaultUploadSettings &&
                    UseDefaultActions && UseDefaultToolsSettings && UseDefaultAdvancedSettings && !WatchFolderEnabled;
            }
        }

        public static TaskSettings GetDefaultTaskSettings()
        {
            TaskSettings taskSettings = new TaskSettings();
            taskSettings.SetDefaultSettings();
            taskSettings.TaskSettingsReference = Program.DefaultTaskSettings;
            return taskSettings;
        }

        public static TaskSettings GetSafeTaskSettings(TaskSettings taskSettings)
        {
            TaskSettings safeTaskSettings;

            if (taskSettings.IsUsingDefaultSettings && Program.DefaultTaskSettings != null)
            {
                safeTaskSettings = Program.DefaultTaskSettings.Copy();
                safeTaskSettings.Description = taskSettings.Description;
                safeTaskSettings.Job = taskSettings.Job;
            }
            else
            {
                safeTaskSettings = taskSettings.Copy();
                safeTaskSettings.SetDefaultSettings();
            }

            safeTaskSettings.TaskSettingsReference = taskSettings;
            return safeTaskSettings;
        }

        public void SetDefaultSettings()
        {
            if (Program.DefaultTaskSettings != null)
            {
                TaskSettings defaultTaskSettings = Program.DefaultTaskSettings.Copy();

                if (UseDefaultAfterCaptureJob)
                {
                    AfterCaptureJob = defaultTaskSettings.AfterCaptureJob;
                }

                if (UseDefaultAfterUploadJob)
                {
                    AfterUploadJob = defaultTaskSettings.AfterUploadJob;
                }

                if (UseDefaultDestinations)
                {
                    ImageDestination = defaultTaskSettings.ImageDestination;
                    ImageFileDestination = defaultTaskSettings.ImageFileDestination;
                    TextDestination = defaultTaskSettings.TextDestination;
                    TextFileDestination = defaultTaskSettings.TextFileDestination;
                    FileDestination = defaultTaskSettings.FileDestination;
                    URLShortenerDestination = defaultTaskSettings.URLShortenerDestination;
                    URLSharingServiceDestination = defaultTaskSettings.URLSharingServiceDestination;
                }

                if (UseDefaultGeneralSettings)
                {
                    GeneralSettings = defaultTaskSettings.GeneralSettings;
                }

                if (UseDefaultImageSettings)
                {
                    ImageSettings = defaultTaskSettings.ImageSettings;
                }

                if (UseDefaultCaptureSettings)
                {
                    CaptureSettings = defaultTaskSettings.CaptureSettings;
                }

                if (UseDefaultUploadSettings)
                {
                    UploadSettings = defaultTaskSettings.UploadSettings;
                }

                if (UseDefaultActions)
                {
                    ExternalPrograms = defaultTaskSettings.ExternalPrograms;
                }

                if (UseDefaultToolsSettings)
                {
                    ToolsSettings = defaultTaskSettings.ToolsSettings;
                }

                if (UseDefaultAdvancedSettings)
                {
                    AdvancedSettings = defaultTaskSettings.AdvancedSettings;
                }
            }
        }

        public void Cleanup()
        {
            if (UseDefaultGeneralSettings)
            {
                GeneralSettings = null;
            }

            if (UseDefaultImageSettings)
            {
                ImageSettings = null;
            }

            if (UseDefaultCaptureSettings)
            {
                CaptureSettings = null;
            }

            if (UseDefaultUploadSettings)
            {
                UploadSettings = null;
            }

            if (UseDefaultActions)
            {
                ExternalPrograms = null;
            }

            if (UseDefaultToolsSettings)
            {
                ToolsSettings = null;
            }

            if (UseDefaultAdvancedSettings)
            {
                AdvancedSettings = null;
            }
        }

        public FileDestination GetFileDestinationByDataType(EDataType dataType)
        {
            switch (dataType)
            {
                case EDataType.Image:
                    return ImageFileDestination;
                case EDataType.Text:
                    return TextFileDestination;
                default:
                case EDataType.File:
                    return FileDestination;
            }
        }
    }

    public class TaskSettingsGeneral
    {
        #region General / Notifications

        public bool PlaySoundAfterCapture = true;
        public bool PlaySoundAfterUpload = true;
        public bool PlaySoundAfterAction = true;
        public bool ShowToastNotificationAfterTaskCompleted = true;
        public float ToastWindowDuration = 3f;
        public float ToastWindowFadeDuration = 1f;
        public ContentAlignment ToastWindowPlacement = ContentAlignment.BottomRight;
        public Size ToastWindowSize = new Size(400, 300);
        public ToastClickAction ToastWindowLeftClickAction = ToastClickAction.OpenUrl;
        public ToastClickAction ToastWindowRightClickAction = ToastClickAction.CloseNotification;
        public ToastClickAction ToastWindowMiddleClickAction = ToastClickAction.AnnotateImage;
        public bool ToastWindowAutoHide = true;
        public bool DisableNotificationsOnFullscreen = false;
        public bool UseCustomCaptureSound = false;
        public string CustomCaptureSoundPath = "";
        public bool UseCustomTaskCompletedSound = false;
        public string CustomTaskCompletedSoundPath = "";
        public bool UseCustomActionCompletedSound = false;
        public string CustomActionCompletedSoundPath = "";
        public bool UseCustomErrorSound = false;
        public string CustomErrorSoundPath = "";

        #endregion
    }

    public class TaskSettingsImage
    {
        #region Image / General

        public EImageFormat ImageFormat = EImageFormat.PNG;
        public PNGBitDepth ImagePNGBitDepth = PNGBitDepth.Default;
        public int ImageJPEGQuality = 90;
        public GIFQuality ImageGIFQuality = GIFQuality.Default;
        public bool ImageAutoUseJPEG = true;
        public int ImageAutoUseJPEGSize = 2048;
        public bool ImageAutoJPEGQuality = false;
        public FileExistAction FileExistAction = FileExistAction.Ask;

        #endregion Image / General

        #region Image / Effects

        public List<ImageEffectPreset> ImageEffectPresets = new List<ImageEffectPreset>() { ImageEffectPreset.GetDefaultPreset() };
        public int SelectedImageEffectPreset = 0;

        public bool ShowImageEffectsWindowAfterCapture = false;
        public bool ImageEffectOnlyRegionCapture = false;
        public bool UseRandomImageEffect = false;

        #endregion Image / Effects

        #region Image / Thumbnail

        public int ThumbnailWidth = 200;
        public int ThumbnailHeight = 0;
        public string ThumbnailName = "-thumbnail";
        public bool ThumbnailCheckSize = false;

        #endregion Image / Thumbnail
    }

    public class TaskSettingsCapture
    {
        #region Capture / General

        public bool ShowCursor = true;
        public decimal ScreenshotDelay = 0;
        public bool CaptureTransparent = false;
        public bool CaptureShadow = true;
        public int CaptureShadowOffset = 100;
        public bool CaptureClientArea = false;
        public bool CaptureAutoHideTaskbar = false;
        public Rectangle CaptureCustomRegion = new Rectangle(0, 0, 0, 0);
        public string CaptureCustomWindow = "";

        #endregion Capture / General

        #region Capture / Region capture

        public RegionCaptureOptions SurfaceOptions = new RegionCaptureOptions();

        #endregion Capture / Region capture

        #region Capture / Screen recorder

        public FFmpegOptions FFmpegOptions = new FFmpegOptions();
        public int ScreenRecordFPS = 30;
        public int GIFFPS = 15;
        public bool ScreenRecordShowCursor = true;
        public bool ScreenRecordAutoStart = true;
        public float ScreenRecordStartDelay = 0f;
        public bool ScreenRecordFixedDuration = false;
        public float ScreenRecordDuration = 3f;
        public bool ScreenRecordTwoPassEncoding = false;
        public bool ScreenRecordAskConfirmationOnAbort = false;
        public bool ScreenRecordTransparentRegion = false;

        #endregion Capture / Screen recorder

        #region Capture / Scrolling capture

        public ScrollingCaptureOptions ScrollingCaptureOptions = new ScrollingCaptureOptions();

        #endregion Capture / Scrolling capture

        #region Capture / OCR

        public OCROptions OCROptions = new OCROptions();

        #endregion Capture / OCR
    }

    public class TaskSettingsUpload
    {
        #region Upload / File naming

        public bool UseCustomTimeZone = false;
        public TimeZoneInfo CustomTimeZone = TimeZoneInfo.Utc;
        public string NameFormatPattern = "%ra{10}";
        public string NameFormatPatternActiveWindow = "%pn_%ra{10}";
        public bool FileUploadUseNamePattern = false;
        public bool FileUploadReplaceProblematicCharacters = false;
        public bool URLRegexReplace = false;
        public string URLRegexReplacePattern = "^https?://(.+)$";
        public string URLRegexReplaceReplacement = "https://$1";

        #endregion Upload / File naming

        #region Upload / Clipboard upload

        public bool ClipboardUploadURLContents = false;
        public bool ClipboardUploadShortenURL = false;
        public bool ClipboardUploadShareURL = false;
        public bool ClipboardUploadAutoIndexFolder = false;

        #endregion Upload / Clipboard upload

        #region Upload / Uploader filters

        public List<UploaderFilter> UploaderFilters = new List<UploaderFilter>();

        #endregion Upload / Uploader filters
    }

    public class TaskSettingsTools
    {
        public string ScreenColorPickerFormat = "$hex";
        public string ScreenColorPickerFormatCtrl = "$r255, $g255, $b255";
        public string ScreenColorPickerInfoText = "RGB: $r255, $g255, $b255$nHex: $hex$nX: $x Y: $y";
        public PinToScreenOptions PinToScreenOptions = new PinToScreenOptions();
        public IndexerSettings IndexerSettings = new IndexerSettings();
        public ImageBeautifierOptions ImageBeautifierOptions = new ImageBeautifierOptions();
        public ImageCombinerOptions ImageCombinerOptions = new ImageCombinerOptions();
        public VideoConverterOptions VideoConverterOptions = new VideoConverterOptions();
        public VideoThumbnailOptions VideoThumbnailOptions = new VideoThumbnailOptions();
        public BorderlessWindowSettings BorderlessWindowSettings = new BorderlessWindowSettings();
    }

    public class TaskSettingsAdvanced
    {
        [Category("General"), DefaultValue(false), Description("Allow after capture tasks for image files by loading them as bitmap when files are handled during file upload, clipboard file upload, drag && drop file upload, watch folder and other image file tasks.")]
        public bool ProcessImagesDuringFileUpload { get; set; }

        [Category("General"), DefaultValue(false), Description("Use after capture tasks for clipboard image uploads.")]
        public bool ProcessImagesDuringClipboardUpload { get; set; }

        [Category("General"), DefaultValue(false), Description("Use after capture tasks for browser extension image uploads.")]
        public bool ProcessImagesDuringExtensionUpload { get; set; }

        [Category("General"), DefaultValue(true), Description("Allows file related after capture tasks (\"Perform actions\", \"Copy file to clipboard\" etc.) to be used when doing file upload.")]
        public bool UseAfterCaptureTasksDuringFileUpload { get; set; }

        [Category("General"), DefaultValue(true), Description("Save text as file for tasks such as clipboard text upload, drag and drop text upload, index folder etc.")]
        public bool TextTaskSaveAsFile { get; set; }

        [Category("General"), DefaultValue(false), Description("If task contains upload job then this setting will clear clipboard when task start.")]
        public bool AutoClearClipboard { get; set; }

        [Category("Capture"), DefaultValue(false), Description("Disable annotation support in region capture.")]
        public bool RegionCaptureDisableAnnotation { get; set; }

        [Category("Upload"), Description("Files with these file extensions will be uploaded using image uploader."),
        Editor("System.Windows.Forms.Design.StringCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public List<string> ImageExtensions { get; set; }

        [Category("Upload"), Description("Files with these file extensions will be uploaded using text uploader."),
        Editor("System.Windows.Forms.Design.StringCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public List<string> TextExtensions { get; set; }

        [Category("Upload"), DefaultValue(false), Description("Copy URL before start upload. Only works for FTP, FTPS, SFTP, Amazon S3, Google Cloud Storage and Azure Storage.")]
        public bool EarlyCopyURL { get; set; }

        [Category("Upload text"), DefaultValue("txt"), Description("File extension when saving text to the local hard disk.")]
        public string TextFileExtension { get; set; }

        [Category("Upload text"), DefaultValue("text"), Description("Text format e.g. csharp, cpp, etc.")]
        public string TextFormat { get; set; }

        [Category("Upload text"), DefaultValue(""), Description("Custom text input. Use %input for text input. Example you can create web page with your text in it."),
        Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string TextCustom { get; set; }

        [Category("Upload text"), DefaultValue(true), Description("HTML encode custom text input.")]
        public bool TextCustomEncodeInput { get; set; }

        [Category("After upload"), DefaultValue(false), Description("If result URL starts with \"http://\" then replace it with \"https://\".")]
        public bool ResultForceHTTPS { get; set; }

        [Category("After upload"), DefaultValue("$result"),
        Description("Clipboard content format after uploading. Supported variables: $result, $url, $shorturl, $thumbnailurl, $deletionurl, $filepath, $filename, $filenamenoext, $folderpath, $foldername, $uploadtime and other variables such as %y-%mo-%d etc.")]
        public string ClipboardContentFormat { get; set; }

        [Category("After upload"), DefaultValue("$result"), Description("Balloon tip content format after uploading. Supported variables: $result, $url, $shorturl, $thumbnailurl, $deletionurl, $filepath, $filename, $filenamenoext, $folderpath, $foldername, $uploadtime and other variables such as %y-%mo-%d etc.")]
        public string BalloonTipContentFormat { get; set; }

        [Category("After upload"), DefaultValue("$result"), Description("After upload task \"Open URL\" format. Supported variables: $result, $url, $shorturl, $thumbnailurl, $deletionurl, $filepath, $filename, $filenamenoext, $folderpath, $foldername, $uploadtime and other variables such as %y-%mo-%d etc.")]
        public string OpenURLFormat { get; set; }

        [Category("After upload"), DefaultValue(0), Description("Automatically shorten URL if the URL is longer than the specified number of characters. 0 means automatic URL shortening is not active.")]
        public int AutoShortenURLLength { get; set; }

        [Category("After upload"), DefaultValue(false), Description("After upload form will be automatically closed after 60 seconds.")]
        public bool AutoCloseAfterUploadForm { get; set; }

        [Category("Name pattern"), DefaultValue(100), Description("Maximum name pattern length for file name.")]
        public int NamePatternMaxLength { get; set; }

        [Category("Name pattern"), DefaultValue(50), Description("Maximum name pattern title (%t) length for file name.")]
        public int NamePatternMaxTitleLength { get; set; }

        public TaskSettingsAdvanced()
        {
            this.ApplyDefaultPropertyValues();
            ImageExtensions = FileHelpers.ImageFileExtensions.ToList();
            TextExtensions = FileHelpers.TextFileExtensions.ToList();
        }
    }
}