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

using System;
using System.ComponentModel;

namespace ShareX
{
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
        [Description("한국어 (Korean)")]
        Korean,
        [Description("简体中文 (Simplified Chinese)")]
        SimplifiedChinese,
        [Description("Español (Spanish)")]
        Spanish,
        [Description("Türkçe (Turkish)")]
        Turkish
    }

    public enum EImageFormat
    {
        [Description("png")]
        PNG,
        [Description("jpg")]
        JPEG,
        [Description("gif")]
        GIF,
        [Description("bmp")]
        BMP,
        [Description("tif")]
        TIFF
    }

    public enum TaskJob
    {
        Job,
        DataUpload,
        FileUpload,
        TextUpload,
        ShortenURL,
        ShareURL
    }

    [Flags]
    public enum AfterCaptureTasks // Localized
    {
        None = 0,
        AddImageEffects = 1,
        AnnotateImage = 1 << 1,
        CopyImageToClipboard = 1 << 2,
        SendImageToPrinter = 1 << 3,
        SaveImageToFile = 1 << 4,
        SaveImageToFileWithDialog = 1 << 5,
        SaveThumbnailImageToFile = 1 << 6,
        PerformActions = 1 << 7,
        CopyFileToClipboard = 1 << 8,
        CopyFilePathToClipboard = 1 << 9,
        UploadImageToHost = 1 << 10,
        DeleteFile = 1 << 11
    }

    [Flags]
    public enum AfterUploadTasks // Localized
    {
        None = 0,
        UseURLShortener = 1,
        ShareURL = 1 << 1,
        CopyURLToClipboard = 1 << 2,
        OpenURL = 1 << 3,
        ShowQRCode = 1 << 4
    }

    public enum AfterCaptureFormResult
    {
        Cancel,
        Continue,
        Copy
    }

    public enum CaptureType
    {
        Screen,
        Monitor,
        ActiveMonitor,
        Window,
        ActiveWindow,
        RectangleWindow,
        Rectangle,
        RoundedRectangle,
        Ellipse,
        Triangle,
        Diamond,
        Polygon,
        Freehand,
        CustomRegion,
        LastRegion
    }

    public enum HotkeyType // Localized
    {
        None,
        FileUpload,
        FolderUpload,
        ClipboardUpload,
        ClipboardUploadWithContentViewer,
        UploadURL,
        DragDropUpload,
        StopUploads,
        PrintScreen,
        ActiveWindow,
        ActiveMonitor,
        RectangleRegion,
        WindowRectangle,
        RectangleAnnotate,
        RectangleLight,
        RectangleTransparent,
        RoundedRectangleRegion,
        EllipseRegion,
        TriangleRegion,
        DiamondRegion,
        PolygonRegion,
        FreeHandRegion,
        CustomRegion,
        LastRegion,
        ScreenRecorder,
        StartScreenRecorder,
        ScreenRecorderGIF,
        StartScreenRecorderGIF,
        AutoCapture,
        StartAutoCapture,
        OpenScreenshotsFolder,
        ColorPicker,
        ScreenColorPicker,
        Ruler,
        FTPClient,
        HashCheck,
        IndexFolder,
        ImageEffects,
        QRCode,
        TweetMessage,
        Automate
    }

    public enum HotkeyStatus
    {
        Registered,
        Failed,
        NotConfigured
    }

    public enum PopUpNotificationType // Localized
    {
        None,
        BalloonTip,
        ToastNotification
    }

    [DefaultValue(OpenUrl)]
    public enum ToastClickAction
    {
        [Description("Annotate image")]
        AnnotateImage,
        [Description("Copy image to clipboard")]
        CopyImageToClipboard,
        [Description("Open file")]
        OpenFile,
        [Description("Open folder")]
        OpenFolder,
        [Description("Open URL")]
        OpenUrl,
        [Description("Upload")]
        Upload,
    }

    public enum FileExistAction // Localized
    {
        Ask,
        Overwrite,
        UniqueName,
        Cancel
    }

    public enum ImagePreviewVisibility
    {
        Show, Hide, Automatic
    }

    public enum ColorPickerFormat
    {
        RGB, Hexadecimal
    }
}