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

using ShareX.AvaloniaUI.Theming;
using ShareX.AvaloniaUI.Windows;
using ShareX.HelpersLib;
using ShareX.HistoryLib;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEffectsLib;
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using ShareX.Tools;
using ShareX.Tools.Integration;
using ShareX.UploadersLib;
using ShareX.UploadersLib.SharingServices;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace ShareX
{
    public static class TaskHelpers
    {
        public static async Task ExecuteJob(HotkeyType job, string filePath = null)
        {
            await ExecuteJob(Program.DefaultTaskSettings, job, filePath);
        }

        public static async Task ExecuteJob(TaskSettings taskSettings)
        {
            await ExecuteJob(taskSettings, taskSettings.Job);
        }

        public static async Task ExecuteJob(TaskSettings taskSettings, HotkeyType job, string filePath = null)
        {
            if (job == HotkeyType.None) return;

            DebugHelper.WriteLine("Executing: " + job.GetLocalizedDescription());

            TaskSettings safeTaskSettings = TaskSettings.GetSafeTaskSettings(taskSettings);

            switch (job)
            {
                // Upload
                case HotkeyType.FileUpload:
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        UploadManager.UploadFile(filePath, safeTaskSettings);
                    }
                    else
                    {
                        UploadManager.UploadFile(safeTaskSettings);
                    }
                    break;
                case HotkeyType.FolderUpload:
                    UploadManager.UploadFolder(safeTaskSettings);
                    break;
                case HotkeyType.ClipboardUpload:
                    UploadManager.ClipboardUpload(safeTaskSettings);
                    break;
                case HotkeyType.ClipboardUploadWithContentViewer:
                    UploadManager.ClipboardUploadWithContentViewer(safeTaskSettings);
                    break;
                case HotkeyType.UploadText:
                    await UploadManager.ShowTextUploadDialog(safeTaskSettings);
                    break;
                case HotkeyType.UploadURL:
                    await UploadManager.UploadURL(safeTaskSettings);
                    break;
                case HotkeyType.DragDropUpload:
                    OpenDropWindow(safeTaskSettings);
                    break;
                case HotkeyType.ShortenURL:
                    await UploadManager.ShowShortenURLDialog(safeTaskSettings);
                    break;
                case HotkeyType.StopUploads:
                    TaskManager.StopAllTasks();
                    break;
                // Screen capture
                case HotkeyType.PrintScreen:
                    new CaptureFullscreen().Capture(safeTaskSettings);
                    break;
                case HotkeyType.ActiveWindow:
                    new CaptureActiveWindow().Capture(safeTaskSettings);
                    break;
                case HotkeyType.ActiveMonitor:
                    new CaptureActiveMonitor().Capture(safeTaskSettings);
                    break;
                case HotkeyType.RectangleRegion:
                    new CaptureRegion().Capture(safeTaskSettings);
                    break;
                case HotkeyType.RectangleLight:
                    new CaptureRegion(RegionCaptureType.Light).Capture(safeTaskSettings);
                    break;
                case HotkeyType.RectangleTransparent:
                    new CaptureRegion(RegionCaptureType.Transparent).Capture(safeTaskSettings);
                    break;
                case HotkeyType.CustomRegion:
                    new CaptureCustomRegion().Capture(safeTaskSettings);
                    break;
                case HotkeyType.CustomWindow:
                    new CaptureCustomWindow().Capture(safeTaskSettings);
                    break;
                case HotkeyType.LastRegion:
                    new CaptureLastRegion().Capture(safeTaskSettings);
                    break;
                case HotkeyType.ScrollingCapture:
                    await OpenScrollingCapture(safeTaskSettings);
                    break;
                case HotkeyType.AutoCapture:
                    OpenAutoCapture(safeTaskSettings);
                    break;
                case HotkeyType.StartAutoCapture:
                    StartAutoCapture(safeTaskSettings);
                    break;
                case HotkeyType.StopAutoCapture:
                    StopAutoCapture();
                    break;
                // Screen record
                case HotkeyType.ScreenRecorder:
                    StartScreenRecording(ScreenRecordOutput.FFmpeg, ScreenRecordStartMethod.Region, safeTaskSettings);
                    break;
                case HotkeyType.ScreenRecorderActiveWindow:
                    StartScreenRecording(ScreenRecordOutput.FFmpeg, ScreenRecordStartMethod.ActiveWindow, safeTaskSettings);
                    break;
                case HotkeyType.ScreenRecorderCustomRegion:
                    StartScreenRecording(ScreenRecordOutput.FFmpeg, ScreenRecordStartMethod.CustomRegion, safeTaskSettings);
                    break;
                case HotkeyType.StartScreenRecorder:
                    StartScreenRecording(ScreenRecordOutput.FFmpeg, ScreenRecordStartMethod.LastRegion, safeTaskSettings);
                    break;
                case HotkeyType.ScreenRecorderGIF:
                    StartScreenRecording(ScreenRecordOutput.GIF, ScreenRecordStartMethod.Region, safeTaskSettings);
                    break;
                case HotkeyType.ScreenRecorderGIFActiveWindow:
                    StartScreenRecording(ScreenRecordOutput.GIF, ScreenRecordStartMethod.ActiveWindow, safeTaskSettings);
                    break;
                case HotkeyType.ScreenRecorderGIFCustomRegion:
                    StartScreenRecording(ScreenRecordOutput.GIF, ScreenRecordStartMethod.CustomRegion, safeTaskSettings);
                    break;
                case HotkeyType.StartScreenRecorderGIF:
                    StartScreenRecording(ScreenRecordOutput.GIF, ScreenRecordStartMethod.LastRegion, safeTaskSettings);
                    break;
                case HotkeyType.StopScreenRecording:
                    StopScreenRecording();
                    break;
                case HotkeyType.PauseScreenRecording:
                    PauseScreenRecording();
                    break;
                case HotkeyType.AbortScreenRecording:
                    AbortScreenRecording();
                    break;
                // Tools
                case HotkeyType.ColorPicker:
                    ShowScreenColorPickerDialog(safeTaskSettings);
                    break;
                case HotkeyType.ScreenColorPicker:
                    OpenScreenColorPicker(safeTaskSettings);
                    break;
                case HotkeyType.Ruler:
                    OpenRuler(safeTaskSettings);
                    break;
                case HotkeyType.PinToScreen:
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        PinToScreen(filePath, safeTaskSettings);
                    }
                    else
                    {
                        PinToScreen(safeTaskSettings);
                    }
                    break;
                case HotkeyType.PinToScreenFromScreen:
                    PinToScreenFromScreen(safeTaskSettings);
                    break;
                case HotkeyType.PinToScreenFromClipboard:
                    PinToScreenFromClipboard(safeTaskSettings);
                    break;
                case HotkeyType.PinToScreenFromFile:
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        PinToScreen(filePath, safeTaskSettings);
                    }
                    else
                    {
                        PinToScreenFromFile(safeTaskSettings);
                    }
                    break;
                case HotkeyType.PinToScreenCloseAll:
                    PinToScreenCloseAll(safeTaskSettings);
                    break;
                case HotkeyType.ImageEditor:
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        AnnotateImageFromFile(filePath, safeTaskSettings);
                    }
                    else
                    {
                        OpenImageEditor(safeTaskSettings);
                    }
                    break;
                case HotkeyType.ImageBeautifier:
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        OpenImageBeautifier(filePath, safeTaskSettings);
                    }
                    else
                    {
                        OpenImageBeautifier(safeTaskSettings);
                    }
                    break;
                case HotkeyType.ImageEffects:
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        OpenImageEffects(filePath, safeTaskSettings);
                    }
                    else
                    {
                        OpenImageEffects(safeTaskSettings);
                    }
                    break;
                case HotkeyType.ImageViewer:
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        OpenImageViewer(filePath);
                    }
                    else
                    {
                        OpenImageViewer();
                    }
                    break;
                case HotkeyType.BackgroundRemover:
                    OpenBackgroundRemover(safeTaskSettings);
                    break;
                case HotkeyType.ImageComparer:
                    OpenImageComparer();
                    break;
                case HotkeyType.IconConverter:
                    OpenIconConverter();
                    break;
                case HotkeyType.ImageCombiner:
                    OpenImageCombiner(null, safeTaskSettings);
                    break;
                case HotkeyType.ImageSplitter:
                    OpenImageSplitter();
                    break;
                case HotkeyType.ImageThumbnailer:
                    OpenImageThumbnailer();
                    break;
                case HotkeyType.VideoConverter:
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        OpenVideoConverter(filePath, safeTaskSettings);
                    }
                    else
                    {
                        OpenVideoConverter(safeTaskSettings);
                    }
                    break;
                case HotkeyType.VideoThumbnailer:
                    OpenVideoThumbnailer(safeTaskSettings);
                    break;
                case HotkeyType.AnalyzeImage:
                    AnalyzeImage(safeTaskSettings);
                    break;
                case HotkeyType.OCR:
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        await OCRImage(filePath, safeTaskSettings);
                    }
                    else
                    {
                        await OCRImage(safeTaskSettings);
                    }
                    break;
                case HotkeyType.QRCode:
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        OpenQRCodeScanFromImageFile(filePath);
                    }
                    else
                    {
                        OpenQRCode();
                    }
                    break;
                case HotkeyType.QRCodeDecodeFromScreen:
                    OpenQRCodeScanScreen();
                    break;
                case HotkeyType.QRCodeScanRegion:
                    OpenQRCodeScanRegion();
                    break;
                case HotkeyType.HashCheck:
                    OpenHashCheck(filePath, safeTaskSettings);
                    break;
                case HotkeyType.Metadata:
                    OpenMetadataWindow(filePath);
                    break;
                case HotkeyType.StripMetadata:
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        StripMetadata(filePath, safeTaskSettings);
                    }
                    else
                    {
                        StripMetadata(safeTaskSettings);
                    }
                    break;
                case HotkeyType.IndexFolder:
                    UploadManager.IndexFolder();
                    break;
                case HotkeyType.ClipboardViewer:
                    OpenClipboardViewer();
                    break;
                case HotkeyType.BorderlessWindow:
                    OpenBorderlessWindow(safeTaskSettings);
                    break;
                case HotkeyType.ActiveWindowBorderless:
                    MakeActiveWindowBorderless(safeTaskSettings);
                    break;
                case HotkeyType.ActiveWindowTopMost:
                    MakeActiveWindowTopMost(safeTaskSettings);
                    break;
                case HotkeyType.InspectWindow:
                    OpenInspectWindow();
                    break;
                case HotkeyType.MonitorTest:
                    OpenMonitorTest();
                    break;
                // Other
                case HotkeyType.DisableHotkeys:
                    ToggleHotkeys(safeTaskSettings);
                    break;
                case HotkeyType.OpenMainWindow:
                    Program.MainForm.ForceActivate();
                    break;
                case HotkeyType.OpenScreenshotsFolder:
                    OpenScreenshotsFolder();
                    break;
                case HotkeyType.OpenHistory:
                    OpenHistory();
                    break;
                case HotkeyType.OpenImageHistory:
                    OpenImageHistory();
                    break;
                case HotkeyType.ToggleActionsToolbar:
                    ToggleActionsToolbar();
                    break;
                case HotkeyType.ToggleTrayMenu:
                    ToggleTrayMenu();
                    break;
                case HotkeyType.ExitShareX:
                    Program.MainForm.ForceClose();
                    break;
            }
        }

        public static ImageData PrepareImage(Image img, TaskSettings taskSettings)
        {
            ImageData imageData = new ImageData();
            imageData.ImageStream = SaveImageAsStream(img, taskSettings.ImageSettings.ImageFormat, taskSettings);
            imageData.ImageFormat = taskSettings.ImageSettings.ImageFormat;

            if (taskSettings.ImageSettings.ImageAutoUseJPEG && taskSettings.ImageSettings.ImageFormat != EImageFormat.JPEG &&
                imageData.ImageStream.Length > taskSettings.ImageSettings.ImageAutoUseJPEGSize * 1000)
            {
                imageData.ImageStream.Dispose();

                using (Bitmap newImage = ImageHelpers.FillBackground(img, Color.White))
                {
                    if (taskSettings.ImageSettings.ImageAutoJPEGQuality)
                    {
                        imageData.ImageStream = ImageHelpers.SaveJPEGAutoQuality(newImage, taskSettings.ImageSettings.ImageAutoUseJPEGSize * 1000, 2, 70, 100);
                    }
                    else
                    {
                        imageData.ImageStream = ImageHelpers.SaveJPEG(newImage, taskSettings.ImageSettings.ImageJPEGQuality);
                    }
                }

                imageData.ImageFormat = EImageFormat.JPEG;
            }

            return imageData;
        }

        public static string CreateThumbnail(Bitmap bmp, string folder, string fileName, TaskSettings taskSettings)
        {
            if ((taskSettings.ImageSettings.ThumbnailWidth > 0 || taskSettings.ImageSettings.ThumbnailHeight > 0) && (!taskSettings.ImageSettings.ThumbnailCheckSize ||
                (bmp.Width > taskSettings.ImageSettings.ThumbnailWidth && bmp.Height > taskSettings.ImageSettings.ThumbnailHeight)))
            {
                string thumbnailFileName = Path.GetFileNameWithoutExtension(fileName) + taskSettings.ImageSettings.ThumbnailName + ".jpg";
                string thumbnailFilePath = HandleExistsFile(folder, thumbnailFileName, taskSettings);

                if (!string.IsNullOrEmpty(thumbnailFilePath))
                {
                    using (Bitmap thumbnail = (Bitmap)bmp.Clone())
                    using (Bitmap resizedImage = new Resize(taskSettings.ImageSettings.ThumbnailWidth, taskSettings.ImageSettings.ThumbnailHeight).Apply(thumbnail))
                    using (Bitmap newImage = ImageHelpers.FillBackground(resizedImage, Color.White))
                    {
                        ImageHelpers.SaveJPEG(newImage, thumbnailFilePath, 90);
                        return thumbnailFilePath;
                    }
                }
            }

            return null;
        }

        public static MemoryStream SaveImageAsStream(Image img, EImageFormat imageFormat, TaskSettings taskSettings)
        {
            return SaveImageAsStream(img, imageFormat, taskSettings.ImageSettings.ImagePNGBitDepth,
                taskSettings.ImageSettings.ImageJPEGQuality, taskSettings.ImageSettings.ImageGIFQuality);
        }

        public static MemoryStream SaveImageAsStream(Image img, EImageFormat imageFormat, PNGBitDepth pngBitDepth = PNGBitDepth.Automatic,
            int jpegQuality = 90, GIFQuality gifQuality = GIFQuality.Default)
        {
            MemoryStream ms = new MemoryStream();

            try
            {
                switch (imageFormat)
                {
                    case EImageFormat.PNG:
                        ImageHelpers.SavePNG(img, ms, pngBitDepth);

                        if (Program.Settings.PNGStripColorSpaceInformation)
                        {
                            using (ms)
                            {
                                return ImageHelpers.PNGStripColorSpaceInformation(ms);
                            }
                        }
                        break;
                    case EImageFormat.JPEG:
                        using (Bitmap newImage = ImageHelpers.FillBackground(img, Color.White))
                        {
                            ImageHelpers.SaveJPEG(newImage, ms, jpegQuality);
                        }
                        break;
                    case EImageFormat.GIF:
                        ImageHelpers.SaveGIF(img, ms, gifQuality);
                        break;
                    case EImageFormat.BMP:
                        img.Save(ms, ImageFormat.Bmp);
                        break;
                    case EImageFormat.TIFF:
                        img.Save(ms, ImageFormat.Tiff);
                        break;
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                e.ShowError();
            }

            return ms;
        }

        public static void SaveImageAsFile(Bitmap bmp, TaskSettings taskSettings, bool overwriteFile = false)
        {
            using (ImageData imageData = PrepareImage(bmp, taskSettings))
            {
                string screenshotsFolder = GetScreenshotsFolder(taskSettings);
                string fileName = GetFileName(taskSettings, imageData.ImageFormat.GetDescription(), bmp);
                string filePath = Path.Combine(screenshotsFolder, fileName);

                if (!overwriteFile)
                {
                    filePath = HandleExistsFile(filePath, taskSettings);
                }

                if (!string.IsNullOrEmpty(filePath))
                {
                    imageData.Write(filePath);
                    DebugHelper.WriteLine("Image saved to file: " + filePath);
                }
            }
        }

        public static string GetFileName(TaskSettings taskSettings, string extension, Bitmap bmp)
        {
            TaskMetadata metadata = new TaskMetadata(bmp);
            return GetFileName(taskSettings, extension, metadata);
        }

        public static string GetFileName(TaskSettings taskSettings, string extension = null, TaskMetadata metadata = null)
        {
            string fileName;

            NameParser nameParser = new NameParser(NameParserType.FileName)
            {
                AutoIncrementNumber = Program.Settings.NameParserAutoIncrementNumber,
                MaxNameLength = taskSettings.AdvancedSettings.NamePatternMaxLength,
                MaxTitleLength = taskSettings.AdvancedSettings.NamePatternMaxTitleLength,
                CustomTimeZone = taskSettings.UploadSettings.UseCustomTimeZone ? taskSettings.UploadSettings.CustomTimeZone : null
            };

            if (metadata != null)
            {
                if (metadata.Image != null)
                {
                    nameParser.ImageWidth = metadata.Image.Width;
                    nameParser.ImageHeight = metadata.Image.Height;
                }

                nameParser.WindowText = metadata.WindowTitle;
                nameParser.ProcessName = metadata.ProcessName;
            }

            if (!string.IsNullOrEmpty(taskSettings.UploadSettings.NameFormatPatternActiveWindow) && !string.IsNullOrEmpty(nameParser.WindowText))
            {
                fileName = nameParser.Parse(taskSettings.UploadSettings.NameFormatPatternActiveWindow);
            }
            else
            {
                fileName = nameParser.Parse(taskSettings.UploadSettings.NameFormatPattern);
            }

            Program.Settings.NameParserAutoIncrementNumber = nameParser.AutoIncrementNumber;

            if (!string.IsNullOrEmpty(extension))
            {
                fileName += "." + extension.TrimStart('.');
            }

            return fileName;
        }

        public static string GetScreenshotsFolder(TaskSettings taskSettings = null, TaskMetadata metadata = null)
        {
            string screenshotsFolder;

            NameParser nameParser = new NameParser(NameParserType.FilePath);

            if (metadata != null)
            {
                if (metadata.Image != null)
                {
                    nameParser.ImageWidth = metadata.Image.Width;
                    nameParser.ImageHeight = metadata.Image.Height;
                }

                nameParser.WindowText = metadata.WindowTitle;
                nameParser.ProcessName = metadata.ProcessName;
            }

            if (taskSettings != null && taskSettings.OverrideScreenshotsFolder && !string.IsNullOrEmpty(taskSettings.ScreenshotsFolder))
            {
                screenshotsFolder = nameParser.Parse(taskSettings.ScreenshotsFolder);
            }
            else
            {
                string subFolderPattern;

                if (!string.IsNullOrEmpty(Program.Settings.SaveImageSubFolderPatternWindow) && !string.IsNullOrEmpty(nameParser.WindowText))
                {
                    subFolderPattern = Program.Settings.SaveImageSubFolderPatternWindow;
                }
                else
                {
                    subFolderPattern = Program.Settings.SaveImageSubFolderPattern;
                }

                string subFolderPath = nameParser.Parse(subFolderPattern);
                screenshotsFolder = Path.Combine(Program.ScreenshotsParentFolder, subFolderPath);
            }

            return FileHelpers.GetAbsolutePath(screenshotsFolder);
        }

        public static void ShowAfterCaptureWindow(TaskSettings taskSettings, Action<AfterCaptureWindowResult> completed,
            TaskMetadata metadata = null, string filePath = null)
        {
            if (!taskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.ShowAfterCaptureWindow))
            {
                completed(new AfterCaptureWindowResult(true, null));
                return;
            }

            AfterCaptureWindowIntegration.Show(taskSettings, metadata, filePath, result =>
            {
                if (!result.Accepted)
                {
                    metadata?.Dispose();
                }

                completed(result);
            });
        }

        public static void PrintImage(Image img)
        {
            if (Program.Settings.DontShowPrintSettingsDialog)
            {
                using (PrintHelper printHelper = new PrintHelper(img))
                {
                    printHelper.Settings = Program.Settings.PrintSettings;
                    printHelper.Print();
                }
            }
            else
            {
                PrintWindowIntegration.Show(
                    img,
                    Program.Settings.PrintSettings,
                    owner: MainWindowIntegration.Instance);
            }
        }

        public static Bitmap ApplyImageEffects(Bitmap bmp, TaskSettingsImage taskSettingsImage)
        {
            if (bmp != null)
            {
                bmp = ImageHelpers.NonIndexedBitmap(bmp);

                if (taskSettingsImage.ShowImageEffectsWindowAfterCapture)
                {
                    ImageEffectsDialogResult result = ImageEffectsIntegration.ShowDialog(bmp,
                        taskSettingsImage.ImageEffectPresets, taskSettingsImage.SelectedImageEffectPreset,
                        ImageEffectsWindowMode.Editor);
                    taskSettingsImage.SelectedImageEffectPreset = result.SelectedPresetIndex;
                }

                ImageEffectPreset imageEffect = null;

                if (taskSettingsImage.UseRandomImageEffect)
                {
                    imageEffect = RandomFast.Pick(taskSettingsImage.ImageEffectPresets);
                }
                else if (taskSettingsImage.ImageEffectPresets.IsValidIndex(taskSettingsImage.SelectedImageEffectPreset))
                {
                    imageEffect = taskSettingsImage.ImageEffectPresets[taskSettingsImage.SelectedImageEffectPreset];
                }

                if (imageEffect != null)
                {
                    using (bmp)
                    {
                        return imageEffect.ApplyEffects(bmp);
                    }
                }
            }

            return bmp;
        }

        public static void AddDefaultExternalPrograms(TaskSettings taskSettings)
        {
            if (taskSettings.ExternalPrograms == null)
            {
                taskSettings.ExternalPrograms = new List<ExternalProgram>();
            }

            AddExternalProgramFromRegistry(taskSettings, "Paint", "mspaint.exe");
            AddExternalProgramFromRegistry(taskSettings, "Paint.NET", "PaintDotNet.exe");
            AddExternalProgramFromRegistry(taskSettings, "Adobe Photoshop", "Photoshop.exe");
            AddExternalProgramFromRegistry(taskSettings, "IrfanView", "i_view32.exe");
            AddExternalProgramFromRegistry(taskSettings, "XnView", "xnview.exe");
        }

        private static void AddExternalProgramFromRegistry(TaskSettings taskSettings, string name, string fileName)
        {
            if (!taskSettings.ExternalPrograms.Exists(x => x.Name == name))
            {
                try
                {
                    string filePath = RegistryHelpers.SearchProgramPath(fileName);

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        ExternalProgram externalProgram = new ExternalProgram(name, filePath);
                        taskSettings.ExternalPrograms.Add(externalProgram);
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }
        }

        public static string HandleExistsFile(string folder, string fileName, TaskSettings taskSettings)
        {
            string filePath = Path.Combine(folder, fileName);
            return HandleExistsFile(filePath, taskSettings);
        }

        public static string HandleExistsFile(string filePath, TaskSettings taskSettings)
        {
            if (File.Exists(filePath))
            {
                switch (taskSettings.ImageSettings.FileExistAction)
                {
                    case FileExistAction.Ask:
                        filePath = FileExistWindowIntegration.Show(filePath);
                        break;
                    case FileExistAction.UniqueName:
                        filePath = FileHelpers.GetUniqueFilePath(filePath);
                        break;
                    case FileExistAction.Cancel:
                        filePath = "";
                        break;
                }
            }

            return filePath;
        }

        public static void OpenDropWindow(TaskSettings taskSettings = null)
        {
            DragDropUploadWindowIntegration.Show(
                Program.Settings.DropSize,
                Program.Settings.DropOffset,
                Program.Settings.DropAlignment,
                Program.Settings.DropOpacity,
                Program.Settings.DropHoverOpacity,
                taskSettings);
        }

        public static void StartScreenRecording(ScreenRecordOutput outputType, ScreenRecordStartMethod startMethod, TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            ScreenRecordManager.StartStopRecording(outputType, startMethod, taskSettings);
        }

        public static void StopScreenRecording()
        {
            ScreenRecordManager.StopRecording();
        }

        public static void PauseScreenRecording()
        {
            ScreenRecordManager.PauseScreenRecording();
        }

        public static void AbortScreenRecording()
        {
            ScreenRecordManager.AbortRecording();
        }

        public static async Task OpenScrollingCapture(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            await ScrollingCaptureWindowIntegration.StartStopAsync(taskSettings.CaptureSettingsReference.ScrollingCaptureOptions,
                img => UploadManager.RunImageTask(img, taskSettings),
                () => PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings));
        }

        public static void OpenAutoCapture(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            AutoCaptureWindowIntegration.Show(taskSettings);
        }

        public static void StartAutoCapture(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            AutoCaptureWindowIntegration.Start(taskSettings);
        }

        public static void StopAutoCapture()
        {
            AutoCaptureWindowIntegration.Stop();
        }

        public static void OpenScreenshotsFolder()
        {
            string screenshotsFolder = GetScreenshotsFolder();

            if (Directory.Exists(screenshotsFolder))
            {
                FileHelpers.OpenFolder(screenshotsFolder);
            }
            else
            {
                FileHelpers.OpenFolder(Program.ScreenshotsParentFolder);
            }
        }

        public static void OpenHistory()
        {
            HistoryIntegration.ShowHistoryWindow(Program.HistoryManager, Program.Settings.HistorySettings,
                new HistoryWindowServices
                {
                    UploadFile = filePath => UploadManager.UploadFile(filePath),
                    EditImage = filePath => AnnotateImageFromFile(filePath),
                    PinToScreen = filePath => PinToScreen(filePath),
                    AnalyzeImage = filePath => AnalyzeImage(filePath),
                    ShowImage = filePath => OpenImageViewer(filePath)
                });
        }

        public static void OpenImageHistory()
        {
            HistoryIntegration.ShowImageHistoryWindow(Program.HistoryManager, Program.Settings.ImageHistorySettings,
                new HistoryWindowServices
                {
                    UploadFile = filePath => UploadManager.UploadFile(filePath),
                    EditImage = filePath => AnnotateImageFromFile(filePath),
                    PinToScreen = filePath => PinToScreen(filePath),
                    AnalyzeImage = filePath => AnalyzeImage(filePath),
                    ShowImage = filePath => OpenImageViewer(filePath),
                    ShowImages = (filePaths, selectedIndex) =>
                        ToolsIntegration.ShowImageViewerWindow(filePaths, selectedIndex)
                });
        }

        public static void OpenDebugLog()
        {
            DebugLogWindowIntegration.Show(
                DebugHelper.Logger,
                text => UploadManager.UploadText(text),
                Resources.MainForm_UploadDebugLogWarning);
        }

        public static void ShowScreenColorPickerDialog(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();
            ToolsIntegration.ShowColorPickerWindow(
                taskSettings.CaptureSettingsReference.SurfaceOptions.ColorPickerOptions,
                taskSettings.ToolsSettingsReference.ScreenColorPickerOptions);
        }

        public static async void OpenScreenColorPicker(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();
            ScreenColorPickerOptions options = taskSettings.ToolsSettingsReference.ScreenColorPickerOptions;
            ScreenColorPickerResult result = await ToolsIntegration.PickScreenColorAsync(options);

            if (result != null)
            {
                string input = result.ControlPressed ? options.FormatCtrl : options.Format;

                if (!string.IsNullOrEmpty(input))
                {
                    Color color = Color.FromArgb(result.Color.A, result.Color.R, result.Color.G, result.Color.B);
                    Point position = new Point(result.Position.X, result.Position.Y);
                    string text = CodeMenuEntryPixelInfo.Parse(input, color, position);
                    ClipboardHelpers.CopyText(text);

                    PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings);

                    if (taskSettings.GeneralSettings.ShowToastNotificationAfterTaskCompleted)
                    {
                        ShowNotificationTip(string.Format(Resources.TaskHelpers_OpenQuickScreenColorPicker_Copied_to_clipboard___0_, text),
                            "ShareX - " + Resources.ScreenColorPicker);
                    }
                }
            }
        }

        public static void OpenHashCheck(string filePath = null, TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            ToolsIntegration.ShowHashCheckerWindow(
                CalculateFileHashAsync,
                () => PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings),
                filePath);
        }

        private static async Task<string> CalculateFileHashAsync(
            string filePath,
            HashCheckerAlgorithm algorithm,
            IProgress<double> progress,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            HashChecker hashChecker = new HashChecker();
            hashChecker.FileCheckProgressChanged += value => progress.Report(value);
            using CancellationTokenRegistration registration = cancellationToken.Register(hashChecker.Stop);
            return await hashChecker.Start(filePath, (HashType)algorithm);
        }

        public static void OpenMetadataWindow(string filePath = null)
        {
            if (!CheckExifTool())
            {
                return;
            }

            ToolsIntegration.ShowMetadataWindow(filePath,
                () => PlayNotificationSoundAsync(NotificationSound.ActionCompleted));
        }

        public static bool StripMetadata(TaskSettings taskSettings = null)
        {
            string filePath = FileHelpers.BrowseFile();

            if (!string.IsNullOrEmpty(filePath))
            {
                return StripMetadata(filePath, taskSettings);
            }

            return false;
        }

        public static bool StripMetadata(string filePath = null, TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (!CheckExifTool())
            {
                return false;
            }

            try
            {
                MetadataService.StripFileMetadata(filePath);

                PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                e.ShowError();

                return false;
            }

            return true;
        }

        public static void OpenDirectoryIndexer(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            IndexerSettings indexerSettings = taskSettings.ToolsSettingsReference.IndexerSettings;
            indexerSettings.BinaryUnits = Program.Settings.BinaryUnits;
            ToolsIntegration.ShowDirectoryIndexerWindow(indexerSettings, (source, output) =>
            {
                WorkerTask task = WorkerTask.CreateTextUploaderTask(source, taskSettings);
                task.Info.FileName = Path.ChangeExtension(task.Info.FileName, output.ToString().ToLowerInvariant());
                TaskManager.Start(task);
                return Task.CompletedTask;
            });
        }

        public static void OpenImageCombiner(IEnumerable<string> imageFiles = null, TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            TaskSettings activeTaskSettings = taskSettings;
            ToolsIntegration.ShowImageCombinerWindow(
                taskSettings.ToolsSettingsReference.ImageCombinerOptions,
                new ImageCombinerServices
                {
                    CreatePreviewAsync = CreateImageCombinerPreviewAsync,
                    ProcessAsync = request => ProcessCombinedImagesAsync(request, activeTaskSettings)
                },
                imageFiles?.ToArray());
        }

        private static Task<byte[]> CreateImageCombinerPreviewAsync(ImageCombineRequest request)
        {
            return Task.Run(() =>
            {
                using Bitmap output = CombineImages(request);
                if (output == null)
                {
                    return null;
                }

                using MemoryStream stream = new MemoryStream();
                output.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            });
        }

        private static async Task ProcessCombinedImagesAsync(ImageCombineRequest request, TaskSettings taskSettings)
        {
            Bitmap output = await Task.Run(() => CombineImages(request));
            if (output != null)
            {
                UploadManager.RunImageTask(output, taskSettings);
            }
        }

        private static Bitmap CombineImages(ImageCombineRequest request)
        {
            return ImageHelpers.CombineImages(
                request.ImageFiles,
                (Orientation)request.Options.Orientation,
                (ShareX.HelpersLib.ImageCombinerAlignment)request.Options.Alignment,
                request.Options.Space,
                request.Options.WrapAfter,
                request.Options.AutoFillBackground);
        }

        public static void OpenImageComparer()
        {
            ToolsIntegration.ShowImageComparerWindow();
        }

        public static void OpenIconConverter()
        {
            ToolsIntegration.ShowIconConverterWindow();
        }

        public static void OpenBackgroundRemover(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            ToolsIntegration.ShowBackgroundRemoverWindow(Program.ModelsFolder, taskSettings.ToolsSettingsReference.BackgroundRemoverOptions);
        }

        public static void CombineImages(IEnumerable<string> imageFiles, Orientation orientation, TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            Bitmap output = ImageHelpers.CombineImages(imageFiles, orientation,
                (ShareX.HelpersLib.ImageCombinerAlignment)taskSettings.ToolsSettings.ImageCombinerOptions.Alignment,
                taskSettings.ToolsSettings.ImageCombinerOptions.Space, taskSettings.ToolsSettings.ImageCombinerOptions.WrapAfter,
                taskSettings.ToolsSettings.ImageCombinerOptions.AutoFillBackground);

            if (output != null)
            {
                UploadManager.RunImageTask(output, taskSettings);
            }
        }

        public static void OpenImageSplitter()
        {
            ToolsIntegration.ShowImageSplitterWindow();
        }

        public static void OpenImageThumbnailer()
        {
            ToolsIntegration.ShowImageThumbnailerWindow();
        }

        public static void OpenVideoConverter(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (!CheckFFmpeg(taskSettings))
            {
                return;
            }

            ShowVideoConverter(taskSettings);
        }

        public static void OpenVideoConverter(string filePath, TaskSettings taskSettings = null)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

                if (!CheckFFmpeg(taskSettings))
                {
                    return;
                }

                ShowVideoConverter(taskSettings, filePath);
            }
        }

        private static void ShowVideoConverter(TaskSettings taskSettings, string inputFilePath = null)
        {
            string ffmpegFilePath = taskSettings.CaptureSettings.FFmpegOptions.FFmpegPath;
            ToolsIntegration.ShowVideoConverterWindow(
                taskSettings.ToolsSettingsReference.VideoConverterOptions,
                (request, progress, cancellationToken) => RunVideoConversionAsync(ffmpegFilePath, request, progress, cancellationToken),
                inputFilePath);
        }

        private static Task<VideoConversionResult> RunVideoConversionAsync(
            string ffmpegFilePath,
            VideoConversionRequest request,
            IProgress<double> progress,
            CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using FFmpegCLIManager ffmpeg = new FFmpegCLIManager(ffmpegFilePath)
                {
                    ShowError = false,
                    TrackEncodeProgress = true
                };

                ffmpeg.EncodeProgressChanged += percentage => progress.Report(percentage);
                using CancellationTokenRegistration registration = cancellationToken.Register(ffmpeg.Close);
                bool succeeded = ffmpeg.Run(request.Arguments);
                bool wasCancelled = cancellationToken.IsCancellationRequested || ffmpeg.StopRequested;

                if (succeeded && !wasCancelled && request.AutoOpenFolder)
                {
                    FileHelpers.OpenFolderWithFile(request.OutputFilePath);
                }

                string errorMessage = null;
                if (!succeeded && !wasCancelled)
                {
                    errorMessage = ffmpeg.Output.ToString()
                        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .LastOrDefault();
                }

                return new VideoConversionResult(succeeded, wasCancelled, errorMessage);
            }, cancellationToken);
        }

        public static void OpenVideoThumbnailer(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (!CheckFFmpeg(taskSettings))
            {
                return;
            }

            taskSettings.ToolsSettingsReference.VideoThumbnailOptions.DefaultOutputDirectory = GetScreenshotsFolder(taskSettings);
            ToolsIntegration.ShowVideoThumbnailerWindow(
                taskSettings.CaptureSettings.FFmpegOptions.FFmpegPath,
                taskSettings.ToolsSettingsReference.VideoThumbnailOptions,
                thumbnails =>
                {
                    if (taskSettings.ToolsSettingsReference.VideoThumbnailOptions.UploadThumbnails)
                    {
                        foreach (VideoThumbnailInfo thumbnailInfo in thumbnails)
                        {
                            UploadManager.UploadFile(thumbnailInfo.FilePath, taskSettings);
                        }
                    }
                });
        }

        public static void OpenBorderlessWindow(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            BorderlessWindowSettings settings = taskSettings.ToolsSettingsReference.BorderlessWindowSettings;

            ToolsIntegration.ShowBorderlessWindow(
                settings,
                BorderlessWindowManager.ToggleBorderlessWindow,
                playNotificationSound: () => PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings));
        }

        public static void MakeActiveWindowBorderless(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            try
            {
                IntPtr handle = NativeMethods.GetForegroundWindow();

                if (handle.ToInt32() > 0)
                {
                    BorderlessWindowManager.ToggleBorderlessWindow(handle, taskSettings.ToolsSettings.BorderlessWindowSettings.ExcludeTaskbarArea);

                    PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings);
                }
            }
            catch (Exception e)
            {
                e.ShowError();
            }
        }

        public static void MakeActiveWindowTopMost(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            try
            {
                IntPtr handle = NativeMethods.GetForegroundWindow();

                if (handle.ToInt32() > 0)
                {
                    WindowInfo windowInfo = new WindowInfo(handle);
                    windowInfo.TopMost = !windowInfo.TopMost;

                    PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings);
                }
            }
            catch (Exception e)
            {
                e.ShowError();
            }
        }

        public static void OpenInspectWindow()
        {
            ToolsIntegration.ShowInspectWindowWindow();
        }

        public static void OpenClipboardViewer()
        {
            ToolsIntegration.ShowClipboardViewerWindow();
        }

        private static void ShowImageEditorSelector(TaskSettings taskSettings)
        {
            if (taskSettings.ToolsSettingsReference.ShowImageEditorSelector)
            {
                bool? useLegacyImageEditor = ImageEditorSelectorWindowIntegration.Show();

                if (useLegacyImageEditor.HasValue)
                {
                    taskSettings.ToolsSettingsReference.UseLegacyImageEditor = useLegacyImageEditor.Value;
                    taskSettings.ToolsSettingsReference.ShowImageEditorSelector = false;
                }
            }
        }

        public static void OpenImageEditor(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            ShowImageEditorSelector(taskSettings);

            if (taskSettings.ToolsSettingsReference.UseLegacyImageEditor)
            {
                using (EditorStartupForm editorStartupForm = new EditorStartupForm(taskSettings.CaptureSettingsReference.SurfaceOptions))
                {
                    if (editorStartupForm.ShowDialog() == DialogResult.OK)
                    {
                        AnnotateImageAsync(editorStartupForm.Image, editorStartupForm.ImageFilePath, taskSettings);
                    }
                }
            }
            else
            {
                AnnotateImageAsync(null, null, taskSettings);
            }
        }

        public static void AnnotateImageFromFile(string filePath, TaskSettings taskSettings = null)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

                Bitmap bmp = ImageHelpers.LoadImage(filePath);

                AnnotateImageAsync(bmp, filePath, taskSettings);
            }
            else
            {
                MessageBox.Show("File does not exist:" + Environment.NewLine + filePath, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static void AnnotateImageAsync(Bitmap bmp, string filePath, TaskSettings taskSettings)
        {
            ThreadWorker worker = new ThreadWorker();

            worker.DoWork += () =>
            {
                bmp = AnnotateImage(bmp, filePath, taskSettings);
            };

            worker.Completed += () =>
            {
                if (bmp != null)
                {
                    UploadManager.RunImageTask(bmp, taskSettings);
                }
            };

            worker.Start(ApartmentState.STA);
        }

        public static Bitmap AnnotateImage(Bitmap bmp, string filePath, TaskSettings taskSettings, bool taskMode = false)
        {
            ShowImageEditorSelector(taskSettings);

            if (taskSettings.ToolsSettingsReference.UseLegacyImageEditor)
            {
                return AnnotateImageLegacy(bmp, filePath, taskSettings, taskMode);
            }

            return AnnotateImageModern(bmp, filePath, taskSettings, taskMode);
        }

        private static Bitmap AnnotateImageLegacy(Bitmap bmp, string filePath, TaskSettings taskSettings, bool taskMode = false)
        {
            if (bmp != null)
            {
                bmp = ImageHelpers.NonIndexedBitmap(bmp);

                using (bmp)
                {
                    RegionCaptureMode mode = taskMode ? RegionCaptureMode.TaskEditor : RegionCaptureMode.Editor;
                    RegionCaptureOptions options = taskSettings.CaptureSettingsReference.SurfaceOptions;

                    using (RegionCaptureForm form = new RegionCaptureForm(mode, options, bmp))
                    {
                        form.ImageFilePath = filePath;

                        form.SaveImageRequested += (output, newFilePath) =>
                        {
                            using (output)
                            {
                                if (string.IsNullOrEmpty(newFilePath))
                                {
                                    string screenshotsFolder = GetScreenshotsFolder(taskSettings);
                                    string fileName = GetFileName(taskSettings, taskSettings.ImageSettings.ImageFormat.GetDescription(), output);
                                    newFilePath = Path.Combine(screenshotsFolder, fileName);
                                }

                                ImageHelpers.SaveImage(output, newFilePath);
                            }

                            return newFilePath;
                        };

                        form.SaveImageAsRequested += (output, newFilePath) =>
                        {
                            using (output)
                            {
                                if (string.IsNullOrEmpty(newFilePath))
                                {
                                    string screenshotsFolder = GetScreenshotsFolder(taskSettings);
                                    string fileName = GetFileName(taskSettings, taskSettings.ImageSettings.ImageFormat.GetDescription(), output);
                                    newFilePath = Path.Combine(screenshotsFolder, fileName);
                                }

                                newFilePath = ImageHelpers.SaveImageFileDialog(output, newFilePath);
                            }

                            return newFilePath;
                        };

                        form.CopyImageRequested += MainFormCopyImage;
                        form.UploadImageRequested += output => MainFormUploadImage(output, taskSettings);
                        form.PrintImageRequested += MainFormPrintImage;
                        form.ShowDialog();

                        switch (form.Result)
                        {
                            case RegionResult.Close: // Esc
                            case RegionResult.AnnotateCancelTask:
                                return null;
                            case RegionResult.Region: // Enter
                            case RegionResult.AnnotateRunAfterCaptureTasks:
                                return form.GetResultImage();
                            case RegionResult.Fullscreen: // Space or right click
                            case RegionResult.AnnotateContinueTask:
                                return (Bitmap)form.Canvas.Clone();
                        }
                    }
                }
            }

            return null;
        }

        private static Bitmap AnnotateImageModern(Bitmap bmp, string filePath, TaskSettings taskSettings, bool taskMode = false,
            bool openBackgroundPanel = false)
        {
            Bitmap bmpResult = null;

            ImageEditorCallbacks events = new ImageEditorCallbacks
            {
                CopyImageRequested = (skBitmap) =>
                {
                    using Bitmap img = skBitmap.ToBitmap();
                    MainFormCopyImage(img);
                },
                SaveImageRequested = (skBitmap, newFilePath) =>
                {
                    using Bitmap img = skBitmap.ToBitmap();

                    if (string.IsNullOrEmpty(newFilePath))
                    {
                        string screenshotsFolder = GetScreenshotsFolder(taskSettings);
                        string fileName = GetFileName(taskSettings, taskSettings.ImageSettings.ImageFormat.GetDescription(), img);
                        newFilePath = Path.Combine(screenshotsFolder, fileName);
                    }

                    ImageHelpers.SaveImage(img, newFilePath);
                    return newFilePath;
                },
                SaveImageAsRequested = (skBitmap, newFilePath) =>
                {
                    using Bitmap img = skBitmap.ToBitmap();

                    if (string.IsNullOrEmpty(newFilePath))
                    {
                        string screenshotsFolder = GetScreenshotsFolder(taskSettings);
                        string fileName = GetFileName(taskSettings, taskSettings.ImageSettings.ImageFormat.GetDescription(), img);
                        newFilePath = Path.Combine(screenshotsFolder, fileName);
                    }

                    newFilePath = ImageHelpers.SaveImageFileDialog(img, newFilePath);
                    return newFilePath;
                },
                PrintImageRequested = (skBitmap) =>
                {
                    Bitmap bmp = skBitmap.ToBitmap();
                    MainFormPrintImage(bmp);
                },
                PinImageRequested = (skBitmap) =>
                {
                    Bitmap bmp = skBitmap.ToBitmap();
                    PinToScreen(bmp, taskSettings);
                },
                UploadImageRequested = (skBitmap) =>
                {
                    Bitmap bmp = skBitmap.ToBitmap();
                    MainFormUploadImage(bmp, taskSettings);
                }
            };

            SKBitmap skBitmapResult = null;

            if (bmp != null)
            {
                using SKBitmap skBitmap = GdiBitmapToSkBitmap(bmp);
                skBitmapResult = ImageEditorIntegration.ShowEditorDialog(skBitmap, taskSettings.ToolsSettingsReference.ImageEditorOptions,
                    events, taskMode, filePath, openBackgroundPanel);
            }
            else
            {
                skBitmapResult = ImageEditorIntegration.ShowEditorDialog(taskSettings.ToolsSettingsReference.ImageEditorOptions,
                    events, taskMode, filePath, openBackgroundPanel);
            }

            if (skBitmapResult != null)
            {
                using (skBitmapResult)
                {
                    bmpResult = skBitmapResult.ToBitmap();
                }
            }

            return bmpResult;
        }

        // Avoid the slow PNG re-encode path for large captures while still bypassing
        // the WindowsForms Bitmap->SKBitmap conversion that regressed post-effects opens.
        private static SKBitmap GdiBitmapToSkBitmap(Bitmap bitmap)
        {
            Bitmap sourceBitmap = bitmap;
            bool disposeSourceBitmap = false;
            PixelFormat pixelFormat = bitmap.PixelFormat;

            if (pixelFormat != PixelFormat.Format32bppArgb && pixelFormat != PixelFormat.Format32bppPArgb)
            {
                sourceBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppPArgb);
                sourceBitmap.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

                using (Graphics graphics = Graphics.FromImage(sourceBitmap))
                {
                    graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    graphics.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
                }

                disposeSourceBitmap = true;
                pixelFormat = sourceBitmap.PixelFormat;
            }

            Rectangle rect = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            BitmapData bmpData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, pixelFormat);

            try
            {
                SKAlphaType alphaType = pixelFormat == PixelFormat.Format32bppPArgb ? SKAlphaType.Premul : SKAlphaType.Unpremul;
                SKBitmap skBitmap = new SKBitmap(new SKImageInfo(sourceBitmap.Width, sourceBitmap.Height, SKColorType.Bgra8888, alphaType));

                IntPtr dstPtr = skBitmap.GetPixels();
                int dstStride = skBitmap.RowBytes;
                int srcStride = bmpData.Stride;
                int srcStrideAbs = Math.Abs(srcStride);
                int height = sourceBitmap.Height;
                int rowBytes = sourceBitmap.Width * 4;
                IntPtr srcStart = bmpData.Scan0;

                if (srcStride < 0)
                {
                    srcStart = IntPtr.Add(srcStart, srcStride * (height - 1));
                }

                if (srcStrideAbs == dstStride)
                {
                    int copyLength = dstStride * height;
                    byte[] pixels = new byte[copyLength];
                    Marshal.Copy(srcStart, pixels, 0, copyLength);
                    Marshal.Copy(pixels, 0, dstPtr, copyLength);
                }
                else
                {
                    byte[] row = new byte[rowBytes];

                    for (int y = 0; y < height; y++)
                    {
                        IntPtr srcRow = IntPtr.Add(srcStart, y * srcStrideAbs);
                        IntPtr dstRow = IntPtr.Add(dstPtr, y * dstStride);
                        Marshal.Copy(srcRow, row, 0, rowBytes);
                        Marshal.Copy(row, 0, dstRow, rowBytes);
                    }
                }

                return skBitmap;
            }
            finally
            {
                sourceBitmap.UnlockBits(bmpData);

                if (disposeSourceBitmap)
                {
                    sourceBitmap.Dispose();
                }
            }
        }

        public static void MainFormCopyImage(Bitmap bmp)
        {
            Program.MainForm.InvokeSafe(() =>
            {
                ClipboardHelpers.CopyImage(bmp);
            });
        }

        public static void MainFormUploadImage(Bitmap bmp, TaskSettings taskSettings = null)
        {
            Program.MainForm.InvokeSafe(() =>
            {
                UploadManager.UploadImage(bmp, taskSettings);
            });
        }

        public static void MainFormPrintImage(Bitmap bmp)
        {
            Program.MainForm.InvokeSafe(() =>
            {
                using (bmp)
                {
                    PrintImage(bmp);
                }
            });
        }

        public static void OpenImageBeautifier(TaskSettings taskSettings = null)
        {
            string filePath = ImageHelpers.OpenImageFileDialog();

            OpenImageBeautifier(filePath, taskSettings);
        }

        public static void OpenImageBeautifier(string filePath, TaskSettings taskSettings = null)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

                Bitmap bmp = ImageHelpers.LoadImage(filePath);
                ThreadWorker worker = new ThreadWorker();

                worker.DoWork += () =>
                {
                    using (bmp)
                    {
                        AnnotateImageModern(bmp, filePath, taskSettings, openBackgroundPanel: true)?.Dispose();
                    }
                };

                worker.Start(ApartmentState.STA);
            }
        }

        public static Bitmap BeautifyImage(Bitmap bmp, TaskSettings taskSettings = null)
        {
            if (bmp != null)
            {
                if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

                try
                {
                    return AnnotateImageModern(bmp, null, taskSettings, taskMode: true, openBackgroundPanel: true);
                }
                finally
                {
                    bmp.Dispose();
                }
            }

            return null;
        }

        public static void OpenImageEffects(TaskSettings taskSettings = null)
        {
            string filePath = ImageHelpers.OpenImageFileDialog();

            OpenImageEffects(filePath, taskSettings);
        }

        public static void OpenImageEffects(string filePath, TaskSettings taskSettings = null)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                Bitmap bmp = ImageHelpers.LoadImage(filePath);

                if (bmp != null)
                {
                    bmp = ImageHelpers.NonIndexedBitmap(bmp);

                    if (taskSettings == null) taskSettings = Program.DefaultTaskSettings;

                    using (bmp)
                    {
                        ImageEffectsIntegration.ShowToolWindow(bmp,
                            taskSettings.ImageSettingsReference.ImageEffectPresets,
                            taskSettings.ImageSettings.SelectedImageEffectPreset,
                            CreateImageEffectsCallbacks(taskSettings), filePath,
                            selectedIndex => taskSettings.ImageSettingsReference.SelectedImageEffectPreset = selectedIndex);
                    }
                }
            }
        }

        public static void OpenImageEffectsSingleton(TaskSettings taskSettings = null, string importJson = null)
        {
            if (taskSettings == null) taskSettings = Program.DefaultTaskSettings;

            ImageEffectsIntegration.ShowPresetWindow(taskSettings.ImageSettings.ImageEffectPresets,
                taskSettings.ImageSettings.SelectedImageEffectPreset,
                selectedIndex => taskSettings.ImageSettings.SelectedImageEffectPreset = selectedIndex,
                importJson, CreateImageEffectsCallbacks(taskSettings));
        }

        private static ImageEffectsCallbacks CreateImageEffectsCallbacks(TaskSettings taskSettings)
        {
            return new ImageEffectsCallbacks
            {
                LoadImageFromFile = () =>
                {
                    string path = ImageHelpers.OpenImageFileDialog();
                    Bitmap image = !string.IsNullOrWhiteSpace(path) ? ImageHelpers.LoadImage(path) : null;
                    return image != null ? new ImageEffectsSource(image, path) : null;
                },
                LoadImageFromClipboard = () =>
                {
                    Bitmap image = ClipboardHelpers.GetImage();
                    return image != null ? new ImageEffectsSource(image) : null;
                },
                SaveImage = (image, path) => ImageHelpers.SaveImageFileDialog(image, path),
                UploadImage = image => UploadManager.RunImageTask(image, taskSettings),
                OpenImageEffectsPage = () => URLHelpers.OpenURL(Links.ImageEffects)
            };
        }

        public static void OpenImageViewer()
        {
            ToolsIntegration.ShowImageViewerWindow();
        }

        public static void OpenImageViewer(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                ToolsIntegration.ShowImageViewerWindow(filePath);
            }
        }

        public static void OpenMonitorTest()
        {
            ToolsIntegration.ShowMonitorTestWindow();
        }

        public static void RunShareXAsAdmin(string arguments = null)
        {
            try
            {
                string exePath = Application.ExecutablePath;

                string cmdArgs = $"/c timeout /t 1 & powershell -Command \"Start-Process '{exePath}' -Verb runAs";

                if (!string.IsNullOrEmpty(arguments))
                {
                    cmdArgs += $" -ArgumentList '{arguments}'";
                }

                cmdArgs += "\"";

                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = cmdArgs,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
            }
            catch
            {
            }
        }

        public static void OpenQRCode(string text = null)
        {
            if (text == null)
            {
                string clipboardText = ClipboardHelpers.GetText(true);
                if (CheckQRCodeContent(clipboardText))
                {
                    text = clipboardText;
                }
            }

            ShowQRCodeWindow(new QRCodeWindowOptions { InitialText = text });
        }

        public static void OpenQRCodeScanFromImageFile(string filePath)
        {
            ShowQRCodeWindow(new QRCodeWindowOptions { InitialImageFilePath = filePath });
        }

        public static void OpenQRCodeScanScreen()
        {
            ShowQRCodeWindow(new QRCodeWindowOptions { InitialScanMode = QRCodeScanMode.Screen });
        }

        public static void OpenQRCodeScanRegion()
        {
            ShowQRCodeWindow(new QRCodeWindowOptions { InitialScanMode = QRCodeScanMode.Region });
        }

        private static void ShowQRCodeWindow(QRCodeWindowOptions options)
        {
            ToolsIntegration.ShowQRCodeWindow(new QRCodeServices
            {
                GeneratePreviewAsync = GenerateQRCodePreviewAsync,
                ScanAsync = ScanQRCodeAsync,
                SaveAsync = SaveQRCodeAsync,
                CopyImage = CopyQRCodeImage,
                UploadImage = UploadQRCodeImage,
                PlayNotificationSound = () => PlayNotificationSoundAsync(NotificationSound.ActionCompleted)
            }, options);
        }

        private static Task<byte[]> GenerateQRCodePreviewAsync(string text, int size)
        {
            return Task.Run(() =>
            {
                using Image image = GenerateQRCode(text, size);
                if (image == null)
                {
                    return null;
                }

                using MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            });
        }

        private static Task<string[]> ScanQRCodeAsync(QRCodeScanMode mode, string filePath)
        {
            using Bitmap bitmap = mode switch
            {
                QRCodeScanMode.Screen => new Screenshot().CaptureFullscreen(),
                QRCodeScanMode.Region => RegionCaptureTasks.GetRegionImage(
                    TaskSettings.GetDefaultTaskSettings().CaptureSettings.SurfaceOptions),
                QRCodeScanMode.ImageFile when !string.IsNullOrWhiteSpace(filePath) => ImageHelpers.LoadImage(filePath),
                _ => null
            };

            return Task.FromResult(bitmap != null ? BarcodeScan(bitmap) : null);
        }

        private static Task SaveQRCodeAsync(string text, int size, string filePath)
        {
            return Task.Run(() =>
            {
                if (filePath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    BarcodeWriterSvg writer = new BarcodeWriterSvg
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Options = new QrCodeEncodingOptions
                        {
                            Width = size,
                            Height = size,
                            CharacterSet = "UTF-8"
                        }
                    };
                    var svgImage = writer.Write(text);
                    File.WriteAllText(filePath, svgImage.Content, Encoding.UTF8);
                }
                else
                {
                    using Image image = GenerateQRCode(text, size);
                    if (image != null)
                    {
                        ImageHelpers.SaveImage(image, filePath);
                    }
                }
            });
        }

        private static void CopyQRCodeImage(string text, int size)
        {
            using Image image = GenerateQRCode(text, size);
            if (image != null)
            {
                ClipboardHelpers.CopyImage(image);
            }
        }

        private static void UploadQRCodeImage(string text, int size)
        {
            using Image image = GenerateQRCode(text, size);
            if (image != null)
            {
                MainFormUploadImage(new Bitmap(image));
            }
        }

        public static void OpenRuler(TaskSettings taskSettings = null)
        {
            ToolsIntegration.ShowRulerWindow();
        }

        public static void SearchImageUsingGoogleLens(string url)
        {
            new GoogleLensSharingService().CreateSharer(null, null).ShareURL(url);
        }

        public static void SearchImageUsingBing(string url)
        {
            new BingVisualSearchSharingService().CreateSharer(null, null).ShareURL(url);
        }

        public static void AnalyzeImage(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            ShowAnalyzeImageWindow(null, taskSettings);
        }

        public static void AnalyzeImage(string filePath, TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            ShowAnalyzeImageWindow(filePath, taskSettings);
        }

        private static void ShowAnalyzeImageWindow(string filePath, TaskSettings taskSettings)
        {
            AIOptions options = taskSettings.ToolsSettingsReference.AIOptions;

            ToolsIntegration.ShowAnalyzeImageWindow(
                filePath,
                options,
                () =>
                {
                    using Bitmap region = RegionCaptureTasks.GetRegionImage(taskSettings.CaptureSettings.SurfaceOptions);
                    if (region == null)
                    {
                        return Task.FromResult<byte[]>(null);
                    }

                    using MemoryStream stream = new MemoryStream();
                    region.Save(stream, ImageFormat.Png);
                    return Task.FromResult(stream.ToArray());
                },
                () => PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings));
        }

        public static async Task OCRImage(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            using (Bitmap bmp = RegionCaptureTasks.GetRegionImage(taskSettings.CaptureSettings.SurfaceOptions))
            {
                await OCRImage(bmp, taskSettings);
            }
        }

        public static async Task OCRImage(string filePath, TaskSettings taskSettings = null)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                using (Bitmap bmp = ImageHelpers.LoadImage(filePath))
                {
                    await OCRImage(bmp, filePath, taskSettings);
                }
            }
        }

        public static async Task OCRImage(Bitmap bmp, TaskSettings taskSettings = null)
        {
            await OCRImage(bmp, null, taskSettings);
        }

        public static async Task OCRImage(Bitmap bmp, string filePath = null, TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            OCROptions options = taskSettings.CaptureSettingsReference.OCROptions;

            try
            {
                OCRHelper.ThrowIfNotSupported();

                if (bmp != null)
                {
                    if (options.Silent)
                    {
                        await AsyncOCRImage(bmp, filePath, taskSettings);
                    }
                    else
                    {
                        if (Helpers.IsDefaultSettings(options.ServiceLinks, OCROptions.DefaultServiceLinks, (x, y) => x.Name == y.Name))
                        {
                            options.ServiceLinks = OCROptions.DefaultServiceLinks;
                        }

                        using MemoryStream imageStream = new MemoryStream();
                        bmp.Save(imageStream, ImageFormat.Png);

                        OCRLanguageOption[] languages = OCRHelper.AvailableLanguages;

                        string result = await ToolsIntegration.ShowOCRWindowAsync(
                            imageStream.ToArray(),
                            languages,
                            options,
                            async (imageData, language, scaleFactor, singleLine) =>
                            {
                                using Bitmap source = ImageHelpers.ByteArrayToBitmap(imageData);
                                return await OCRHelper.OCR(source, language, scaleFactor, singleLine);
                            },
                            () =>
                            {
                                using Bitmap region = RegionCaptureTasks.GetRegionImage(taskSettings.CaptureSettings.SurfaceOptions);
                                if (region == null)
                                {
                                    return Task.FromResult<byte[]>(null);
                                }

                                using MemoryStream regionStream = new MemoryStream();
                                region.Save(regionStream, ImageFormat.Png);
                                return Task.FromResult(regionStream.ToArray());
                            },
                            openHelp: () => URLHelpers.OpenURL(Links.DocsOCR));

                        if (!string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(filePath))
                        {
                            string textFilePath = Path.ChangeExtension(filePath, "txt");
                            File.WriteAllText(textFilePath, result, Encoding.UTF8);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.ShowError(false);
            }
        }

        private static async Task AsyncOCRImage(Bitmap bmp, string filePath = null, TaskSettings taskSettings = null)
        {
            if (bmp != null)
            {
                if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

                OCROptions options = taskSettings.CaptureSettingsReference.OCROptions;

                string result = await OCRHelper.OCR(bmp, options.Language, options.ScaleFactor, options.SingleLine);

                if (!string.IsNullOrEmpty(result))
                {
                    Program.MainForm.InvokeSafe(() =>
                    {
                        ClipboardHelpers.CopyText(result);
                    });

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        string textFilePath = Path.ChangeExtension(filePath, "txt");
                        File.WriteAllText(textFilePath, result, Encoding.UTF8);
                    }
                }
                else
                {
                    Program.MainForm.InvokeSafe(() =>
                    {
                        ClipboardHelpers.Clear();
                    });
                }

                PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings);
            }
        }

        public static void PinToScreen(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            PinToScreenOptions options = taskSettings.ToolsSettingsReference.PinToScreenOptions;
            ToolsIntegration.ShowPinToScreenWindow(new PinToScreenServices
            {
                CaptureRegionAsync = () =>
                {
                    using Image image = RegionCaptureTasks.GetRegionImage(out Rectangle rect);
                    return Task.FromResult(CreatePinToScreenSource(image, rect.Location));
                },
                GetClipboardImageAsync = () =>
                {
                    using Image image = ClipboardHelpers.TryGetImage();
                    return Task.FromResult(CreatePinToScreenSource(image));
                },
                SelectImageFileAsync = () =>
                {
                    using Image image = ImageHelpers.LoadImageWithFileDialog();
                    return Task.FromResult(CreatePinToScreenSource(image));
                },
                CopyImage = CopyPinnedImage,
                ImagePinned = () => PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings)
            }, options);
        }

        public static void PinToScreen(Image image, TaskSettings taskSettings = null)
        {
            PinToScreen(image, null, taskSettings);
        }

        public static void PinToScreen(Image image, Point? location, TaskSettings taskSettings = null)
        {
            if (image != null)
            {
                if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

                PinToScreenOptions options = taskSettings.ToolsSettingsReference.PinToScreenOptions;
                PinToScreenSource source;
                using (image)
                {
                    source = CreatePinToScreenSource(image, location);
                }

                if (source == null)
                {
                    return;
                }

                ToolsIntegration.PinToScreen(source.ImageData, options, CopyPinnedImage, source.Location);

                PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings);
            }
        }

        public static void PinToScreen(string filePath, TaskSettings taskSettings = null)
        {
            Image image = ImageHelpers.LoadImage(filePath);

            PinToScreen(image, taskSettings);
        }

        public static void PinToScreenFromScreen(TaskSettings taskSettings = null)
        {
            Image image = RegionCaptureTasks.GetRegionImage(out Rectangle rect);

            PinToScreen(image, rect.Location, taskSettings);
        }

        public static void PinToScreenFromClipboard(TaskSettings taskSettings = null)
        {
            Image image = ClipboardHelpers.TryGetImage();

            if (image != null)
            {
                PinToScreen(image, taskSettings);
            }
            else
            {
                MessageBox.Show(Resources.ClipboardDoesNotContainAnImage, "ShareX - " + Resources.PinToScreen, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static void PinToScreenFromFile(TaskSettings taskSettings = null)
        {
            Image image = ImageHelpers.LoadImageWithFileDialog();

            if (image != null)
            {
                PinToScreen(image, taskSettings);
            }
        }

        public static void PinToScreenCloseAll(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            ToolsIntegration.CloseAllPinnedImages();

            PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings);
        }

        private static PinToScreenSource CreatePinToScreenSource(Image image, Point? location = null)
        {
            if (image == null)
            {
                return null;
            }

            using MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            return new PinToScreenSource(stream.ToArray(), location);
        }

        private static void CopyPinnedImage(byte[] imageData)
        {
            using Bitmap image = ImageHelpers.ByteArrayToBitmap(imageData);
            ClipboardHelpers.CopyImage(image);
        }

        public static EDataType FindDataType(string filePath, TaskSettings taskSettings)
        {
            if (FileHelpers.CheckExtension(filePath, taskSettings.AdvancedSettings.ImageExtensions))
            {
                return EDataType.Image;
            }

            if (FileHelpers.CheckExtension(filePath, taskSettings.AdvancedSettings.TextExtensions))
            {
                return EDataType.Text;
            }

            return EDataType.File;
        }

        public static bool ToggleHotkeys(TaskSettings taskSettings = null)
        {
            bool disableHotkeys = !Program.Settings.DisableHotkeys;
            ToggleHotkeys(disableHotkeys, taskSettings);
            return disableHotkeys;
        }

        public static void ToggleHotkeys(bool disableHotkeys, TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            Program.Settings.DisableHotkeys = disableHotkeys;
            Program.HotkeyManager.ToggleHotkeys(disableHotkeys);
            MainWindowIntegration.RefreshMenus();

            PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings);

            if (taskSettings.GeneralSettings.ShowToastNotificationAfterTaskCompleted)
            {
                ShowNotificationTip(disableHotkeys ? Resources.TaskHelpers_ToggleHotkeys_Hotkeys_disabled_ : Resources.TaskHelpers_ToggleHotkeys_Hotkeys_enabled_);
            }
        }

        public static bool CheckFFmpeg(TaskSettings taskSettings)
        {
            if (!Environment.Is64BitOperatingSystem && !taskSettings.CaptureSettings.FFmpegOptions.OverrideCLIPath)
            {
                MessageBox.Show(Resources.FFmpegOnlySupports64BitOperatingSystems,
                    "ShareX - " + Resources.FFmpegIsMissing, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            string ffmpegPath = taskSettings.CaptureSettings.FFmpegOptions.FFmpegPath;

            if (!File.Exists(ffmpegPath))
            {
                MessageBox.Show(Resources.FFmpegDoesNotExistAtTheFollowingPath + "\r\n" + ffmpegPath,
                    "ShareX - " + Resources.FFmpegIsMissing, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        public static bool CheckExifTool()
        {
            string exifToolPath = FileHelpers.GetAbsolutePath("exiftool.exe");

            if (!File.Exists(exifToolPath))
            {
                // TODO: Translate
                MessageBox.Show("ExifTool does not exist at the following path:" + "\r\n" + exifToolPath,
                    "ShareX - " + "ExifTool is missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        public static void PlayNotificationSoundAsync(NotificationSound notificationSound, TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            switch (notificationSound)
            {
                case NotificationSound.Capture:
                    if (taskSettings.GeneralSettings.PlaySoundAfterCapture)
                    {
                        if (taskSettings.GeneralSettings.UseCustomCaptureSound && !string.IsNullOrEmpty(taskSettings.GeneralSettings.CustomCaptureSoundPath))
                        {
                            Helpers.PlaySoundAsync(taskSettings.GeneralSettings.CustomCaptureSoundPath);
                        }
                        else
                        {
                            Helpers.PlaySoundAsync(Resources.CaptureSound);
                        }
                    }
                    break;
                case NotificationSound.TaskCompleted:
                    if (taskSettings.GeneralSettings.PlaySoundAfterUpload)
                    {
                        if (taskSettings.GeneralSettings.UseCustomTaskCompletedSound && !string.IsNullOrEmpty(taskSettings.GeneralSettings.CustomTaskCompletedSoundPath))
                        {
                            Helpers.PlaySoundAsync(taskSettings.GeneralSettings.CustomTaskCompletedSoundPath);
                        }
                        else
                        {
                            Helpers.PlaySoundAsync(Resources.TaskCompletedSound);
                        }
                    }
                    break;
                case NotificationSound.ActionCompleted:
                    if (taskSettings.GeneralSettings.PlaySoundAfterAction)
                    {
                        if (taskSettings.GeneralSettings.UseCustomActionCompletedSound && !string.IsNullOrEmpty(taskSettings.GeneralSettings.CustomActionCompletedSoundPath))
                        {
                            Helpers.PlaySoundAsync(taskSettings.GeneralSettings.CustomActionCompletedSoundPath);
                        }
                        else
                        {
                            Helpers.PlaySoundAsync(Resources.ActionCompletedSound);
                        }
                    }
                    break;
                case NotificationSound.Error:
                    if (taskSettings.GeneralSettings.PlaySoundAfterUpload)
                    {
                        if (taskSettings.GeneralSettings.UseCustomErrorSound && !string.IsNullOrEmpty(taskSettings.GeneralSettings.CustomErrorSoundPath))
                        {
                            Helpers.PlaySoundAsync(taskSettings.GeneralSettings.CustomErrorSoundPath);
                        }
                        else
                        {
                            Helpers.PlaySoundAsync(Resources.ErrorSound);
                        }
                    }
                    break;
            }
        }

        public static void OpenUploadersConfigWindow(IUploaderService uploaderService = null)
        {
            SettingManager.WaitUploadersConfig();
            DestinationSettingsIntegration.Show(Program.UploadersConfig, uploaderService,
                () => SettingManager.SaveUploadersConfigAsync());
        }

        public static void OpenCustomUploaderSettingsWindow()
        {
            CustomUploaderSettingsIntegration.Show();
        }

        public static string FindMenuLucideIcon(HotkeyType hotkeyType)
        {
            return hotkeyType switch
            {
                HotkeyType.None => LucideIcons.circle_dashed,

                // Upload
                HotkeyType.FileUpload => LucideIcons.file_up,
                HotkeyType.FolderUpload => LucideIcons.folder_up,
                HotkeyType.ClipboardUpload => LucideIcons.clipboard,
                HotkeyType.ClipboardUploadWithContentViewer => LucideIcons.clipboard_list,
                HotkeyType.UploadText => LucideIcons.file_text,
                HotkeyType.UploadURL => LucideIcons.link,
                HotkeyType.DragDropUpload => LucideIcons.mouse_pointer_2,
                HotkeyType.ShortenURL => LucideIcons.link_2,
                HotkeyType.StopUploads => LucideIcons.circle_stop,

                // Screen capture
                HotkeyType.PrintScreen => LucideIcons.monitor,
                HotkeyType.ActiveWindow => LucideIcons.app_window,
                HotkeyType.CustomWindow => LucideIcons.scan,
                HotkeyType.ActiveMonitor => LucideIcons.monitor,
                HotkeyType.RectangleRegion => LucideIcons.scan,
                HotkeyType.RectangleLight => LucideIcons.square,
                HotkeyType.RectangleTransparent => LucideIcons.square_dashed,
                HotkeyType.CustomRegion => LucideIcons.scan_line,
                HotkeyType.LastRegion => LucideIcons.layers,
                HotkeyType.ScrollingCapture => LucideIcons.scroll_text,
                HotkeyType.AutoCapture => LucideIcons.clock,
                HotkeyType.StartAutoCapture => LucideIcons.circle_play,
                HotkeyType.StopAutoCapture => LucideIcons.timer_off,

                // Screen record
                HotkeyType.ScreenRecorder => LucideIcons.video,
                HotkeyType.ScreenRecorderActiveWindow => LucideIcons.app_window,
                HotkeyType.ScreenRecorderCustomRegion => LucideIcons.crop,
                HotkeyType.StartScreenRecorder => LucideIcons.circle_play,
                HotkeyType.ScreenRecorderGIF => LucideIcons.film,
                HotkeyType.ScreenRecorderGIFActiveWindow => LucideIcons.film,
                HotkeyType.ScreenRecorderGIFCustomRegion => LucideIcons.crop,
                HotkeyType.StartScreenRecorderGIF => LucideIcons.circle_play,
                HotkeyType.StopScreenRecording => LucideIcons.square_stop,
                HotkeyType.PauseScreenRecording => LucideIcons.circle_pause,
                HotkeyType.AbortScreenRecording => LucideIcons.circle_x,

                // Tools
                HotkeyType.ColorPicker => LucideIcons.palette,
                HotkeyType.ScreenColorPicker => LucideIcons.pipette,
                HotkeyType.Ruler => LucideIcons.ruler,
                HotkeyType.PinToScreen => LucideIcons.pin,
                HotkeyType.PinToScreenFromScreen => LucideIcons.picture_in_picture,
                HotkeyType.PinToScreenFromClipboard => LucideIcons.clipboard,
                HotkeyType.PinToScreenFromFile => LucideIcons.file_image,
                HotkeyType.PinToScreenCloseAll => LucideIcons.pin_off,
                HotkeyType.ImageEditor => LucideIcons.image,
                HotkeyType.ImageBeautifier => LucideIcons.sparkles,
                HotkeyType.ImageEffects => LucideIcons.wand_sparkles,
                HotkeyType.ImageViewer => LucideIcons.eye,
                HotkeyType.BackgroundRemover => LucideIcons.eraser,
                HotkeyType.ImageComparer => LucideIcons.images,
                HotkeyType.IconConverter => LucideIcons.file_image,
                HotkeyType.ImageCombiner => LucideIcons.combine,
                HotkeyType.ImageSplitter => LucideIcons.split,
                HotkeyType.ImageThumbnailer => LucideIcons.shrink,
                HotkeyType.VideoConverter => LucideIcons.file_video,
                HotkeyType.VideoThumbnailer => LucideIcons.clapperboard,
                HotkeyType.AnalyzeImage => LucideIcons.bot,
                HotkeyType.OCR => LucideIcons.scan_text,
                HotkeyType.QRCode => LucideIcons.qr_code,
                HotkeyType.QRCodeDecodeFromScreen => LucideIcons.scan_eye,
                HotkeyType.QRCodeScanRegion => LucideIcons.scan_line,
                HotkeyType.HashCheck => LucideIcons.hash,
                HotkeyType.Metadata => LucideIcons.tags,
                HotkeyType.StripMetadata => LucideIcons.file_x,
                HotkeyType.IndexFolder => LucideIcons.folder_tree,
                HotkeyType.ClipboardViewer => LucideIcons.clipboard_list,
                HotkeyType.BorderlessWindow => LucideIcons.frame,
                HotkeyType.ActiveWindowBorderless => LucideIcons.maximize,
                HotkeyType.ActiveWindowTopMost => LucideIcons.panel_top,
                HotkeyType.InspectWindow => LucideIcons.scan_search,
                HotkeyType.MonitorTest => LucideIcons.test_tube,

                // Other
                HotkeyType.DisableHotkeys => LucideIcons.keyboard_off,
                HotkeyType.OpenMainWindow => LucideIcons.panel_top_open,
                HotkeyType.OpenScreenshotsFolder => LucideIcons.folder_open,
                HotkeyType.OpenHistory => LucideIcons.history,
                HotkeyType.OpenImageHistory => LucideIcons.images,
                HotkeyType.ToggleActionsToolbar => LucideIcons.panel_top,
                HotkeyType.ToggleTrayMenu => LucideIcons.menu,
                HotkeyType.ExitShareX => LucideIcons.log_out,
                _ => LucideIcons.circle
            };
        }

        public static Image FindMenuIcon<T>(T value) where T : Enum
        {
            if (value is AfterCaptureTasks afterCaptureTask)
            {
                switch (afterCaptureTask)
                {
                    default: throw new Exception("Icon missing for after capture task: " + afterCaptureTask);
                    case AfterCaptureTasks.ShowQuickTaskMenu: return Resources.ui_menu_blue;
                    case AfterCaptureTasks.ShowAfterCaptureWindow: return Resources.application_text_image;
                    case AfterCaptureTasks.BeautifyImage: return Resources.picture_sunset;
                    case AfterCaptureTasks.AddImageEffects: return Resources.image_saturation;
                    case AfterCaptureTasks.AnnotateImage: return Resources.image_pencil;
                    case AfterCaptureTasks.CopyImageToClipboard: return Resources.clipboard_paste_image;
                    case AfterCaptureTasks.PinToScreen: return Resources.pin;
                    case AfterCaptureTasks.SendImageToPrinter: return Resources.printer;
                    case AfterCaptureTasks.SaveImageToFile: return Resources.disk;
                    case AfterCaptureTasks.SaveImageToFileWithDialog: return Resources.disk_rename;
                    case AfterCaptureTasks.SaveThumbnailImageToFile: return Resources.disk_small;
                    case AfterCaptureTasks.PerformActions: return Resources.application_terminal;
                    case AfterCaptureTasks.CopyFileToClipboard: return Resources.clipboard_block;
                    case AfterCaptureTasks.CopyFilePathToClipboard: return Resources.clipboard_list;
                    case AfterCaptureTasks.CopyFolderPathToClipboard: return Resources.folder_bookmark;
                    case AfterCaptureTasks.ShowInExplorer: return Resources.folder_stand;
                    case AfterCaptureTasks.AnalyzeImage: return Resources.robot;
                    case AfterCaptureTasks.ScanQRCode: return ShareXResources.IsDarkTheme ? Resources.barcode_2d_white : Resources.barcode_2d;
                    case AfterCaptureTasks.DoOCR: return ShareXResources.IsDarkTheme ? Resources.edit_drop_cap_white : Resources.edit_drop_cap;
                    case AfterCaptureTasks.ShowBeforeUploadWindow: return Resources.application__arrow;
                    case AfterCaptureTasks.UploadImageToHost: return Resources.upload_cloud;
                    case AfterCaptureTasks.DeleteFile: return Resources.bin;
                }
            }
            else if (value is AfterUploadTasks afterUploadTask)
            {
                switch (afterUploadTask)
                {
                    default: throw new Exception("Icon missing for after upload task: " + afterUploadTask);
                    case AfterUploadTasks.ShowAfterUploadWindow: return Resources.application_browser;
                    case AfterUploadTasks.UseURLShortener: return ShareXResources.IsDarkTheme ? Resources.edit_scale_white : Resources.edit_scale;
                    case AfterUploadTasks.ShareURL: return Resources.globe_share;
                    case AfterUploadTasks.CopyURLToClipboard: return Resources.clipboard_paste_document_text;
                    case AfterUploadTasks.OpenURL: return Resources.globe__arrow;
                    case AfterUploadTasks.ShowQRCode: return ShareXResources.IsDarkTheme ? Resources.barcode_2d_white : Resources.barcode_2d;
                }
            }
            else if (value is HotkeyType hotkeyType)
            {
                switch (hotkeyType)
                {
                    default: throw new Exception("Icon missing for hotkey type: " + hotkeyType);
                    case HotkeyType.None: return null;
                    // Upload
                    case HotkeyType.FileUpload: return Resources.folder_open_document;
                    case HotkeyType.FolderUpload: return Resources.folder;
                    case HotkeyType.ClipboardUpload: return Resources.clipboard;
                    case HotkeyType.ClipboardUploadWithContentViewer: return Resources.clipboard_task;
                    case HotkeyType.UploadText: return Resources.notebook;
                    case HotkeyType.UploadURL: return Resources.drive;
                    case HotkeyType.DragDropUpload: return Resources.inbox;
                    case HotkeyType.ShortenURL: return ShareXResources.IsDarkTheme ? Resources.edit_scale_white : Resources.edit_scale;
                    case HotkeyType.StopUploads: return Resources.cross_button;
                    // Screen capture
                    case HotkeyType.PrintScreen: return Resources.layer_fullscreen;
                    case HotkeyType.ActiveWindow: return Resources.application_blue;
                    case HotkeyType.ActiveMonitor: return Resources.monitor;
                    case HotkeyType.RectangleRegion: return Resources.layer_shape;
                    case HotkeyType.RectangleLight: return Resources.Rectangle;
                    case HotkeyType.RectangleTransparent: return Resources.layer_transparent;
                    case HotkeyType.CustomRegion: return Resources.layer__arrow;
                    case HotkeyType.CustomWindow: return Resources.application__arrow;
                    case HotkeyType.LastRegion: return Resources.layers;
                    case HotkeyType.ScrollingCapture: return Resources.ui_scroll_pane_image;
                    case HotkeyType.AutoCapture: return Resources.clock;
                    case HotkeyType.StartAutoCapture: return Resources.clock__arrow;
                    case HotkeyType.StopAutoCapture: return Resources.clock__minus;
                    // Screen record
                    case HotkeyType.ScreenRecorder: return Resources.camcorder_image;
                    case HotkeyType.ScreenRecorderActiveWindow: return Resources.camcorder__arrow;
                    case HotkeyType.ScreenRecorderCustomRegion: return Resources.camcorder__arrow;
                    case HotkeyType.StartScreenRecorder: return Resources.camcorder__arrow;
                    case HotkeyType.ScreenRecorderGIF: return Resources.film;
                    case HotkeyType.ScreenRecorderGIFActiveWindow: return Resources.film__arrow;
                    case HotkeyType.ScreenRecorderGIFCustomRegion: return Resources.film__arrow;
                    case HotkeyType.StartScreenRecorderGIF: return Resources.film__arrow;
                    case HotkeyType.StopScreenRecording: return Resources.camcorder__minus;
                    case HotkeyType.PauseScreenRecording: return Resources.camcorder_pencil;
                    case HotkeyType.AbortScreenRecording: return Resources.camcorder__exclamation;
                    // Tools
                    case HotkeyType.ColorPicker: return Resources.color;
                    case HotkeyType.ScreenColorPicker: return Resources.pipette;
                    case HotkeyType.Ruler: return Resources.ruler_triangle;
                    case HotkeyType.PinToScreen: return Resources.pin;
                    case HotkeyType.PinToScreenFromScreen: return Resources.pin;
                    case HotkeyType.PinToScreenFromClipboard: return Resources.pin;
                    case HotkeyType.PinToScreenFromFile: return Resources.pin;
                    case HotkeyType.PinToScreenCloseAll: return Resources.pin__minus;
                    case HotkeyType.ImageEditor: return Resources.image_pencil;
                    case HotkeyType.ImageBeautifier: return Resources.picture_sunset;
                    case HotkeyType.ImageEffects: return Resources.image_reflection;
                    case HotkeyType.ImageViewer: return Resources.images_flickr;
                    case HotkeyType.BackgroundRemover: return Resources.wand_magic;
                    case HotkeyType.ImageComparer: return Resources.image_saturation;
                    case HotkeyType.IconConverter: return Resources.image_cast;
                    case HotkeyType.ImageCombiner: return Resources.document_break;
                    case HotkeyType.ImageSplitter: return Resources.image_split;
                    case HotkeyType.ImageThumbnailer: return Resources.image_resize_actual;
                    case HotkeyType.VideoConverter: return Resources.camcorder_pencil;
                    case HotkeyType.VideoThumbnailer: return Resources.images_stack;
                    case HotkeyType.AnalyzeImage: return Resources.robot;
                    case HotkeyType.OCR: return ShareXResources.IsDarkTheme ? Resources.edit_drop_cap_white : Resources.edit_drop_cap;
                    case HotkeyType.QRCode: return ShareXResources.IsDarkTheme ? Resources.barcode_2d_white : Resources.barcode_2d;
                    case HotkeyType.QRCodeDecodeFromScreen: return ShareXResources.IsDarkTheme ? Resources.barcode_2d_white : Resources.barcode_2d;
                    case HotkeyType.QRCodeScanRegion: return ShareXResources.IsDarkTheme ? Resources.barcode_2d_white : Resources.barcode_2d;
                    case HotkeyType.HashCheck: return Resources.application_task;
                    case HotkeyType.Metadata: return Resources.tag_hash;
                    case HotkeyType.StripMetadata: return Resources.tag__minus;
                    case HotkeyType.IndexFolder: return Resources.folder_tree;
                    case HotkeyType.ClipboardViewer: return Resources.clipboard_block;
                    case HotkeyType.BorderlessWindow: return Resources.application_resize_full;
                    case HotkeyType.ActiveWindowBorderless: return Resources.application_resize_full;
                    case HotkeyType.ActiveWindowTopMost: return Resources.pin;
                    case HotkeyType.InspectWindow: return Resources.application_search_result;
                    case HotkeyType.MonitorTest: return Resources.monitor;
                    // Other
                    case HotkeyType.DisableHotkeys: return Resources.keyboard__minus;
                    case HotkeyType.OpenMainWindow: return Resources.application_home;
                    case HotkeyType.OpenScreenshotsFolder: return Resources.folder_open_image;
                    case HotkeyType.OpenHistory: return Resources.application_blog;
                    case HotkeyType.OpenImageHistory: return Resources.application_icon_large;
                    case HotkeyType.ToggleActionsToolbar: return Resources.ui_toolbar__arrow;
                    case HotkeyType.ToggleTrayMenu: return Resources.ui_menu_blue;
                    case HotkeyType.ExitShareX: return Resources.cross;
                }
            }

            return null;
        }

        public static Image FindMenuIcon<T>(int index) where T : Enum
        {
            T value = Helpers.GetEnumFromIndex<T>(index);
            return FindMenuIcon(value);
        }

        public static Screenshot GetScreenshot(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            Screenshot screenshot = new Screenshot()
            {
                CaptureCursor = taskSettings.CaptureSettings.ShowCursor,
                CaptureClientArea = taskSettings.CaptureSettings.CaptureClientArea,
                RemoveOutsideScreenArea = true,
                CaptureShadow = taskSettings.CaptureSettings.CaptureShadow,
                ShadowOffset = taskSettings.CaptureSettings.CaptureShadowOffset,
                AutoHideTaskbar = taskSettings.CaptureSettings.CaptureAutoHideTaskbar,
                HDRScreenshotColorCorrection = taskSettings.CaptureSettings.HDRScreenshotColorCorrection
            };

            return screenshot;
        }

        public static void ImportCustomUploader(string filePath)
        {
            if (Program.UploadersConfig != null)
            {
                try
                {
                    CustomUploaderItem cui = JsonHelpers.DeserializeFromFile<CustomUploaderItem>(filePath);

                    if (cui != null)
                    {
                        bool activate = false;

                        if (cui.DestinationType == CustomUploaderDestinationType.None)
                        {
                            DialogResult result = MessageBox.Show($"Would you like to add \"{cui}\" custom uploader?",
                                "ShareX - Custom uploader confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                            if (result == DialogResult.No)
                            {
                                return;
                            }
                        }
                        else
                        {
                            List<string> destinations = new List<string>();
                            if (cui.DestinationType.HasFlag(CustomUploaderDestinationType.ImageUploader)) destinations.Add("images");
                            if (cui.DestinationType.HasFlag(CustomUploaderDestinationType.TextUploader)) destinations.Add("texts");
                            if (cui.DestinationType.HasFlag(CustomUploaderDestinationType.FileUploader)) destinations.Add("files");
                            if (cui.DestinationType.HasFlag(CustomUploaderDestinationType.URLShortener) ||
                                cui.DestinationType.HasFlag(CustomUploaderDestinationType.URLSharingService)) destinations.Add("urls");

                            string destinationsText = string.Join("/", destinations);

                            DialogResult result = MessageBox.Show($"Would you like to set \"{cui}\" as the active custom uploader for {destinationsText}?",
                                "ShareX - Custom uploader confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                            if (result == DialogResult.Yes)
                            {
                                activate = true;
                            }
                            else if (result == DialogResult.Cancel)
                            {
                                return;
                            }
                        }

                        cui.CheckBackwardCompatibility();
                        Program.UploadersConfig.CustomUploadersList.Add(cui);

                        if (activate)
                        {
                            int index = Program.UploadersConfig.CustomUploadersList.Count - 1;

                            if (cui.DestinationType.HasFlag(CustomUploaderDestinationType.ImageUploader))
                            {
                                Program.UploadersConfig.CustomImageUploaderSelected = index;
                                Program.DefaultTaskSettings.ImageDestination = ImageDestination.CustomImageUploader;
                            }

                            if (cui.DestinationType.HasFlag(CustomUploaderDestinationType.TextUploader))
                            {
                                Program.UploadersConfig.CustomTextUploaderSelected = index;
                                Program.DefaultTaskSettings.TextDestination = TextDestination.CustomTextUploader;
                            }

                            if (cui.DestinationType.HasFlag(CustomUploaderDestinationType.FileUploader))
                            {
                                Program.UploadersConfig.CustomFileUploaderSelected = index;
                                Program.DefaultTaskSettings.FileDestination = FileDestination.CustomFileUploader;
                            }

                            if (cui.DestinationType.HasFlag(CustomUploaderDestinationType.URLShortener))
                            {
                                Program.UploadersConfig.CustomURLShortenerSelected = index;
                                Program.DefaultTaskSettings.URLShortenerDestination = UrlShortenerType.CustomURLShortener;
                            }

                            if (cui.DestinationType.HasFlag(CustomUploaderDestinationType.URLSharingService))
                            {
                                Program.UploadersConfig.CustomURLSharingServiceSelected = index;
                                Program.DefaultTaskSettings.URLSharingServiceDestination = URLSharingServices.CustomURLSharingService;
                            }

                            MainWindowIntegration.RefreshMenus();
                        }

                        CustomUploaderSettingsIntegration.Refresh(true);
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                    e.ShowError(false);
                }
            }
        }

        public static void ImportImageEffect(string filePath)
        {
            string configJson = null;

            try
            {
                configJson = ImageEffectPackager.ExtractPackage(filePath, Program.ImageEffectsFolder);
            }
            catch (Exception ex)
            {
                ex.ShowError(false);
            }

            if (!string.IsNullOrEmpty(configJson))
            {
                OpenImageEffectsSingleton(Program.DefaultTaskSettings, configJson);

                if (!Program.DefaultTaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.AddImageEffects) &&
                    MessageBox.Show(Resources.WouldYouLikeToEnableImageEffects,
                    "ShareX", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Program.DefaultTaskSettings.AfterCaptureJob = Program.DefaultTaskSettings.AfterCaptureJob.Add(AfterCaptureTasks.AddImageEffects);
                    MainWindowIntegration.RefreshMenus();
                }
            }
        }

        public static async Task HandleNativeMessagingInput(string filePath, TaskSettings taskSettings = null)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                NativeMessagingInput nativeMessagingInput = null;

                try
                {
                    nativeMessagingInput = JsonHelpers.DeserializeFromFile<NativeMessagingInput>(filePath);
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
                finally
                {
                    File.Delete(filePath);
                }

                if (nativeMessagingInput != null)
                {
                    if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

                    PlayNotificationSoundAsync(NotificationSound.ActionCompleted, taskSettings);

                    switch (nativeMessagingInput.Action)
                    {
                        // TEMP: For backward compatibility
                        default:
                            if (!string.IsNullOrEmpty(nativeMessagingInput.URL))
                            {
                                UploadManager.DownloadAndUploadFile(nativeMessagingInput.URL, taskSettings);
                            }
                            else if (!string.IsNullOrEmpty(nativeMessagingInput.Text))
                            {
                                UploadManager.UploadText(nativeMessagingInput.Text, taskSettings);
                            }
                            break;
                        case NativeMessagingAction.UploadImage:
                            if (!string.IsNullOrEmpty(nativeMessagingInput.URL))
                            {
                                Bitmap bmp = WebHelpers.DataURLToImage(nativeMessagingInput.URL);

                                if (bmp == null && taskSettings.AdvancedSettings.ProcessImagesDuringExtensionUpload)
                                {
                                    try
                                    {
                                        bmp = await WebHelpers.DownloadImageAsync(nativeMessagingInput.URL);
                                    }
                                    catch
                                    {
                                    }
                                }

                                if (bmp != null)
                                {
                                    UploadManager.RunImageTask(bmp, taskSettings);
                                }
                                else
                                {
                                    UploadManager.DownloadAndUploadFile(nativeMessagingInput.URL, taskSettings);
                                }
                            }
                            break;
                        case NativeMessagingAction.UploadVideo:
                        case NativeMessagingAction.UploadAudio:
                            if (!string.IsNullOrEmpty(nativeMessagingInput.URL))
                            {
                                UploadManager.DownloadAndUploadFile(nativeMessagingInput.URL, taskSettings);
                            }
                            break;
                        case NativeMessagingAction.UploadText:
                            if (!string.IsNullOrEmpty(nativeMessagingInput.Text))
                            {
                                UploadManager.UploadText(nativeMessagingInput.Text, taskSettings);
                            }
                            break;
                        case NativeMessagingAction.ShortenURL:
                            if (!string.IsNullOrEmpty(nativeMessagingInput.URL))
                            {
                                UploadManager.ShortenURL(nativeMessagingInput.URL, taskSettings);
                            }
                            break;
                    }
                }
            }
        }

        public static void OpenActionsToolbar()
        {
            ActionsToolbarWindowIntegration.Show();
        }

        public static void ToggleActionsToolbar()
        {
            ActionsToolbarWindowIntegration.Toggle();
        }

        public static async Task DownloadDevBuild()
        {
            GitHubUpdateChecker updateChecker = new GitHubUpdateChecker("ShareX", "DevBuilds")
            {
                IsDev = true,
                IsPortable = Program.Portable
            };

            await updateChecker.CheckUpdateAsync();

            if (updateChecker.Status == UpdateStatus.UpdateAvailable)
            {
                await UpdateMessageBox.StartAsync(updateChecker);
            }
            else if (updateChecker.Status == UpdateStatus.UpToDate)
            {
                MessageBox.Show(Resources.ShareXIsUpToDate, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static async Task DownloadAppVeyorBuild()
        {
            AppVeyorUpdateChecker updateChecker = new AppVeyorUpdateChecker()
            {
                IsDev = true,
                IsPortable = Program.Portable,
                Branch = "develop"
            };

            await updateChecker.CheckUpdateAsync();

            await UpdateMessageBox.StartAsync(updateChecker);
        }

        public static Image GenerateQRCode(string text, int size)
        {
            if (CheckQRCodeContent(text))
            {
                try
                {
                    BarcodeWriter writer = new BarcodeWriter()
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Options = new QrCodeEncodingOptions
                        {
                            Width = size,
                            Height = size,
                            CharacterSet = "UTF-8",
                            PureBarcode = true,
                            NoPadding = false,
                            Margin = 1
                        },
                        Renderer = new BitmapRenderer()
                    };

                    return writer.Write(text);
                }
                catch (Exception e)
                {
                    e.ShowError();
                }
            }

            return null;
        }

        public static string[] BarcodeScan(Bitmap bmp, bool scanQRCodeOnly = false)
        {
            try
            {
                BarcodeReader barcodeReader = new BarcodeReader()
                {
                    AutoRotate = true,
                    Options = new DecodingOptions
                    {
                        TryHarder = true,
                        TryInverted = true
                    }
                };

                if (scanQRCodeOnly)
                {
                    barcodeReader.Options.PossibleFormats = new List<BarcodeFormat>() { BarcodeFormat.QR_CODE };
                }

                Result[] results = barcodeReader.DecodeMultiple(bmp);

                if (results != null)
                {
                    return results.Where(x => x != null && !string.IsNullOrEmpty(x.Text)).Select(x => x.Text).ToArray();
                }
            }
            catch (Exception e)
            {
                e.ShowError();
            }

            return null;
        }

        public static bool CheckQRCodeContent(string content)
        {
            return !string.IsNullOrEmpty(content) && Encoding.UTF8.GetByteCount(content) <= 2952;
        }

        public static void ShowNotificationTip(string text, string title = "ShareX", int duration = -1)
        {
            if (duration < 0)
            {
                duration = (int)(Program.DefaultTaskSettings.GeneralSettings.ToastWindowDuration * 1000);
            }

            NotificationWindowConfig toastConfig = new NotificationWindowConfig()
            {
                Duration = duration,
                FadeDuration = (int)(Program.DefaultTaskSettings.GeneralSettings.ToastWindowFadeDuration * 1000),
                Placement = Program.DefaultTaskSettings.GeneralSettings.ToastWindowPlacement,
                Size = Program.DefaultTaskSettings.GeneralSettings.ToastWindowSize,
                ActionButtons = NotificationActionButton.CloneButtons(Program.DefaultTaskSettings.GeneralSettings.ToastWindowButtons),
                Title = title,
                Text = text
            };

            Program.MainForm.InvokeSafe(() =>
            {
                NotificationWindow.Show(toastConfig);
            });
        }

        public static void ToggleTrayMenu()
        {
            MainWindowIntegration.ShowTrayMenu();
        }

        public static bool IsUploadAllowed()
        {
            if (SystemOptions.DisableUpload)
            {
                MessageBox.Show(Resources.YourSystemAdminDisabledTheUploadFeature, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return false;
            }

            if (Program.Settings.DisableUpload)
            {
                MessageBox.Show(Resources.ThisFeatureWillNotWorkWhenDisableUploadOptionIsEnabled, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return false;
            }

            return true;
        }
    }
}
