#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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
using System;
using System.ComponentModel;

#if MicrosoftStore
using Windows.ApplicationModel;
#endif

namespace ShareX
{
    public enum ShareXBuild
    {
        Debug,
        Release,
        Steam,
        MicrosoftStore,
        Unknown
    }

    public enum SupportedLanguage
    {
        Automatic, // Localized
        [Description("Nederlands (Dutch)")]
        Dutch,
        [Description("English")]
        English,
        [Description("Français (French)")]
        French,
        [Description("Deutsch (German)")]
        German,
        [Description("Magyar (Hungarian)")]
        Hungarian,
        [Description("Bahasa Indonesia (Indonesian)")]
        Indonesian,
        [Description("Italiano (Italian)")]
        Italian,
        [Description("日本語 (Japanese)")]
        Japanese,
        [Description("한국어 (Korean)")]
        Korean,
        [Description("Español mexicano (Mexican Spanish)")]
        MexicanSpanish,
        [Description("فارسی (Persian)")]
        Persian,
        [Description("Polski (Polish)")]
        Polish,
        [Description("Português (Portuguese)")]
        Portuguese,
        [Description("Português-Brasil (Portuguese-Brazil)")]
        PortugueseBrazil,
        [Description("Română (Romanian)")]
        Romanian,
        [Description("Русский (Russian)")]
        Russian,
        [Description("简体中文 (Simplified Chinese)")]
        SimplifiedChinese,
        [Description("Español (Spanish)")]
        Spanish,
        [Description("繁體中文 (Traditional Chinese)")]
        TraditionalChinese,
        [Description("Türkçe (Turkish)")]
        Turkish,
        [Description("Українська (Ukrainian)")]
        Ukrainian,
        [Description("Tiếng Việt (Vietnamese)")]
        Vietnamese
    }

    public enum TaskJob
    {
        Job,
        DataUpload,
        FileUpload,
        TextUpload,
        ShortenURL,
        ShareURL,
        Download,
        DownloadUpload
    }

    public enum TaskStatus
    {
        InQueue,
        Preparing,
        Working,
        Stopping,
        Stopped,
        Failed,
        Completed,
        History
    }

    [Flags]
    public enum AfterCaptureTasks // Localized
    {
        None = 0,
        ShowQuickTaskMenu = 1,
        ShowAfterCaptureWindow = 1 << 1,
        AddImageEffects = 1 << 2,
        AnnotateImage = 1 << 3,
        CopyImageToClipboard = 1 << 4,
        PinToScreen = 1 << 5,
        SendImageToPrinter = 1 << 6,
        SaveImageToFile = 1 << 7,
        SaveImageToFileWithDialog = 1 << 8,
        SaveThumbnailImageToFile = 1 << 9,
        PerformActions = 1 << 10,
        CopyFileToClipboard = 1 << 11,
        CopyFilePathToClipboard = 1 << 12,
        ShowInExplorer = 1 << 13,
        ScanQRCode = 1 << 14,
        DoOCR = 1 << 15,
        ShowBeforeUploadWindow = 1 << 16,
        UploadImageToHost = 1 << 17,
        DeleteFile = 1 << 18
    }

    [Flags]
    public enum AfterUploadTasks // Localized
    {
        None = 0,
        ShowAfterUploadWindow = 1,
        UseURLShortener = 1 << 1,
        ShareURL = 1 << 2,
        CopyURLToClipboard = 1 << 3,
        OpenURL = 1 << 4,
        ShowQRCode = 1 << 5
    }

    public enum CaptureType
    {
        Fullscreen,
        Monitor,
        ActiveMonitor,
        Window,
        ActiveWindow,
        Region,
        CustomRegion,
        LastRegion
    }

    public enum ScreenRecordStartMethod
    {
        Region,
        ActiveWindow,
        CustomRegion,
        LastRegion
    }

    [JsonConverter(typeof(HotkeyTypeEnumConverter))]
    public enum HotkeyType // Localized + Category
    {
        None,
        // Upload
        FileUpload,
        FolderUpload,
        ClipboardUpload,
        ClipboardUploadWithContentViewer,
        UploadText,
        UploadURL,
        DragDropUpload,
        ShortenURL,
        TweetMessage,
        StopUploads,
        // Screen capture
        PrintScreen,
        ActiveWindow,
        ActiveMonitor,
        RectangleRegion,
        RectangleLight,
        RectangleTransparent,
        CustomRegion,
        LastRegion,
        ScrollingCapture,
        AutoCapture,
        StartAutoCapture,
        // Screen record
        ScreenRecorder,
        ScreenRecorderActiveWindow,
        ScreenRecorderCustomRegion,
        StartScreenRecorder,
        ScreenRecorderGIF,
        ScreenRecorderGIFActiveWindow,
        ScreenRecorderGIFCustomRegion,
        StartScreenRecorderGIF,
        StopScreenRecording,
        PauseScreenRecording,
        AbortScreenRecording,
        // Tools
        ColorPicker,
        ScreenColorPicker,
        Ruler,
        PinToScreen,
        PinToScreenFromScreen,
        PinToScreenFromClipboard,
        PinToScreenFromFile,
        ImageEditor,
        ImageEffects,
        ImageViewer,
        ImageCombiner,
        ImageSplitter,
        ImageThumbnailer,
        VideoConverter,
        VideoThumbnailer,
        OCR,
        QRCode,
        QRCodeDecodeFromScreen,
        HashCheck,
        IndexFolder,
        ClipboardViewer,
        BorderlessWindow,
        InspectWindow,
        MonitorTest,
        DNSChanger,
        // Other
        DisableHotkeys,
        OpenMainWindow,
        OpenScreenshotsFolder,
        OpenHistory,
        OpenImageHistory,
        ToggleActionsToolbar,
        ToggleTrayMenu,
        ExitShareX
    }

    public enum PopUpNotificationType // Localized
    {
        None,
        BalloonTip,
        ToastNotification
    }

    public enum ToastClickAction // Localized
    {
        CloseNotification,
        AnnotateImage,
        CopyImageToClipboard,
        CopyFile,
        CopyFilePath,
        CopyUrl,
        OpenFile,
        OpenFolder,
        OpenUrl,
        Upload,
        PinToScreen
    }

    public enum ThumbnailViewClickAction // Localized
    {
        Default,
        Select,
        OpenImageViewer,
        OpenFile,
        OpenFolder,
        OpenURL,
        EditImage
    }

    public enum FileExistAction // Localized
    {
        Ask,
        Overwrite,
        UniqueName,
        Cancel
    }

    public enum ImagePreviewVisibility // Localized
    {
        Show, Hide, Automatic
    }

    public enum ImagePreviewLocation // Localized
    {
        Side, Bottom
    }

    public enum ThumbnailTitleLocation // Localized
    {
        Top, Bottom
    }

    public enum RegionCaptureType
    {
        Default, Light, Transparent
    }

#if !MicrosoftStore
    public enum StartupState
    {
        Disabled,
        DisabledByUser,
        Enabled,
        DisabledByPolicy,
        EnabledByPolicy
    }
#else
    public enum StartupState
    {
        Disabled = StartupTaskState.Disabled,
        DisabledByUser = StartupTaskState.DisabledByUser,
        Enabled = StartupTaskState.Enabled,
        DisabledByPolicy = StartupTaskState.DisabledByPolicy,
        EnabledByPolicy = StartupTaskState.EnabledByPolicy
    }
#endif

    public enum BalloonTipClickAction
    {
        None,
        OpenURL,
        OpenDebugLog
    }

    public enum TaskViewMode // Localized
    {
        ListView,
        ThumbnailView
    }
}