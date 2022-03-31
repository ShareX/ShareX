#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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
using ShareX.ImageEffectsLib;
using ShareX.IndexerLib;
using ShareX.MediaLib;
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
using ShareX.UploadersLib.OtherServices;
using ShareX.UploadersLib.SharingServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Rendering;

namespace ShareX
{
    public static class TaskHelpers
    {
        public static void ExecuteJob(HotkeyType job, CLICommand command = null)
        {
            ExecuteJob(Program.DefaultTaskSettings, job, command);
        }

        public static void ExecuteJob(TaskSettings taskSettings)
        {
            ExecuteJob(taskSettings, taskSettings.Job);
        }

        public static void ExecuteJob(TaskSettings taskSettings, HotkeyType job, CLICommand command = null)
        {
            if (job == HotkeyType.None) return;

            DebugHelper.WriteLine("Executing: " + job.GetLocalizedDescription());

            TaskSettings safeTaskSettings = TaskSettings.GetSafeTaskSettings(taskSettings);

            switch (job)
            {
                // Upload
                case HotkeyType.FileUpload:
                    UploadManager.UploadFile(safeTaskSettings);
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
                    UploadManager.ShowTextUploadDialog(safeTaskSettings);
                    break;
                case HotkeyType.UploadURL:
                    UploadManager.UploadURL(safeTaskSettings);
                    break;
                case HotkeyType.DragDropUpload:
                    OpenDropWindow(safeTaskSettings);
                    break;
                case HotkeyType.ShortenURL:
                    UploadManager.ShowShortenURLDialog(safeTaskSettings);
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
                case HotkeyType.LastRegion:
                    new CaptureLastRegion().Capture(safeTaskSettings);
                    break;
                case HotkeyType.ScrollingCapture:
                    OpenScrollingCapture(safeTaskSettings, true);
                    break;
                case HotkeyType.TextCapture:
                    _ = OCRImage(safeTaskSettings);
                    break;
                case HotkeyType.AutoCapture:
                    OpenAutoCapture(safeTaskSettings);
                    break;
                case HotkeyType.StartAutoCapture:
                    StartAutoCapture(safeTaskSettings);
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
                case HotkeyType.ImageEditor:
                    if (command != null && !string.IsNullOrEmpty(command.Parameter) && File.Exists(command.Parameter))
                    {
                        AnnotateImageFromFile(command.Parameter, safeTaskSettings);
                    }
                    else
                    {
                        OpenImageEditor(safeTaskSettings);
                    }
                    break;
                case HotkeyType.ImageEffects:
                    if (command != null && !string.IsNullOrEmpty(command.Parameter) && File.Exists(command.Parameter))
                    {
                        OpenImageEffects(command.Parameter, taskSettings);
                    }
                    else
                    {
                        OpenImageEffects(taskSettings);
                    }
                    break;
                case HotkeyType.ImageViewer:
                    if (command != null && !string.IsNullOrEmpty(command.Parameter) && File.Exists(command.Parameter))
                    {
                        OpenImageViewer(command.Parameter);
                    }
                    else
                    {
                        OpenImageViewer();
                    }
                    break;
                case HotkeyType.HashCheck:
                    OpenHashCheck();
                    break;
                case HotkeyType.DNSChanger:
                    OpenDNSChanger();
                    break;
                case HotkeyType.QRCode:
                    OpenQRCode();
                    break;
                case HotkeyType.QRCodeDecodeFromScreen:
                    OpenQRCodeDecodeFromScreen();
                    break;
                case HotkeyType.Ruler:
                    OpenRuler(safeTaskSettings);
                    break;
                case HotkeyType.IndexFolder:
                    UploadManager.IndexFolder();
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
                    OpenVideoConverter(safeTaskSettings);
                    break;
                case HotkeyType.VideoThumbnailer:
                    OpenVideoThumbnailer(safeTaskSettings);
                    break;
                case HotkeyType.InspectWindow:
                    OpenInspectWindow();
                    break;
                case HotkeyType.ClipboardViewer:
                    OpenClipboardViewer();
                    break;
                case HotkeyType.TweetMessage:
                    TweetMessage();
                    break;
                case HotkeyType.MonitorTest:
                    OpenMonitorTest();
                    break;
                // Other
                case HotkeyType.DisableHotkeys:
                    ToggleHotkeys();
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

            NameParser nameParser = new NameParser(NameParserType.FolderPath);

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

            return Helpers.GetAbsolutePath(screenshotsFolder);
        }

        public static bool ShowAfterCaptureForm(TaskSettings taskSettings, out string fileName, TaskMetadata metadata = null, string filePath = null)
        {
            fileName = null;

            if (taskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.ShowAfterCaptureWindow))
            {
                AfterCaptureForm afterCaptureForm = null;

                try
                {
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        afterCaptureForm = new AfterCaptureForm(filePath, taskSettings);
                    }
                    else
                    {
                        afterCaptureForm = new AfterCaptureForm(metadata, taskSettings);
                    }

                    if (afterCaptureForm.ShowDialog() == DialogResult.Cancel)
                    {
                        metadata?.Dispose();

                        return false;
                    }

                    fileName = afterCaptureForm.FileName;
                }
                finally
                {
                    afterCaptureForm.Dispose();
                }
            }

            return true;
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
                using (PrintForm printForm = new PrintForm(img, Program.Settings.PrintSettings))
                {
                    printForm.ShowDialog();
                }
            }
        }

        public static Bitmap ApplyImageEffects(Bitmap bmp, TaskSettingsImage taskSettingsImage)
        {
            if (bmp != null)
            {
                bmp = ImageHelpers.NonIndexedBitmap(bmp);

                if (taskSettingsImage.ShowImageEffectsWindowAfterCapture)
                {
                    using (ImageEffectsForm imageEffectsForm = new ImageEffectsForm(bmp, taskSettingsImage.ImageEffectPresets,
                        taskSettingsImage.SelectedImageEffectPreset))
                    {
                        imageEffectsForm.ShowDialog();
                        taskSettingsImage.SelectedImageEffectPreset = imageEffectsForm.SelectedPresetIndex;
                    }
                }

                if (taskSettingsImage.ImageEffectPresets.IsValidIndex(taskSettingsImage.SelectedImageEffectPreset))
                {
                    using (bmp)
                    {
                        return taskSettingsImage.ImageEffectPresets[taskSettingsImage.SelectedImageEffectPreset].ApplyEffects(bmp);
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
                        using (FileExistForm form = new FileExistForm(filePath))
                        {
                            form.ShowDialog();
                            filePath = form.FilePath;
                        }
                        break;
                    case FileExistAction.UniqueName:
                        filePath = Helpers.GetUniqueFilePath(filePath);
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
            DropForm.GetInstance(Program.Settings.DropSize, Program.Settings.DropOffset, Program.Settings.DropAlignment, Program.Settings.DropOpacity,
                Program.Settings.DropHoverOpacity, taskSettings).ForceActivate();
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

        public static void AbortScreenRecording()
        {
            ScreenRecordManager.AbortRecording();
        }

        public static void OpenScrollingCapture(TaskSettings taskSettings = null, bool forceSelection = false)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            ScrollingCaptureForm scrollingCaptureForm = new ScrollingCaptureForm(taskSettings.CaptureSettingsReference.ScrollingCaptureOptions,
                taskSettings.CaptureSettings.SurfaceOptions, forceSelection);
            scrollingCaptureForm.ImageProcessRequested += img => UploadManager.RunImageTask(img, taskSettings);
            scrollingCaptureForm.Show();
        }

        public static void OpenAutoCapture(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            AutoCaptureForm.Instance.TaskSettings = taskSettings;
            AutoCaptureForm.Instance.ForceActivate();
        }

        public static void StartAutoCapture(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (!AutoCaptureForm.IsRunning)
            {
                AutoCaptureForm form = AutoCaptureForm.Instance;
                form.TaskSettings = taskSettings;
                form.Show();
                form.Execute();
            }
        }

        public static void OpenScreenshotsFolder()
        {
            string screenshotsFolder = GetScreenshotsFolder();

            if (Directory.Exists(screenshotsFolder))
            {
                Helpers.OpenFolder(screenshotsFolder);
            }
            else
            {
                Helpers.OpenFolder(Program.ScreenshotsParentFolder);
            }
        }

        public static void OpenHistory()
        {
            HistoryForm historyForm = new HistoryForm(Program.HistoryFilePath, Program.Settings.HistorySettings,
                filePath => UploadManager.UploadFile(filePath), filePath => AnnotateImageFromFile(filePath));
            historyForm.Show();
        }

        public static void OpenImageHistory()
        {
            ImageHistoryForm imageHistoryForm = new ImageHistoryForm(Program.HistoryFilePath, Program.Settings.ImageHistorySettings,
                filePath => UploadManager.UploadFile(filePath), filePath => AnnotateImageFromFile(filePath));
            imageHistoryForm.Show();
        }

        public static void OpenDebugLog()
        {
            DebugForm form = DebugForm.GetFormInstance(DebugHelper.Logger);

            if (!form.HasUploadRequested)
            {
                form.UploadRequested += text =>
                {
                    if (MessageBox.Show(form, Resources.MainForm_UploadDebugLogWarning, "ShareX", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        UploadManager.UploadText(text);
                    }
                };
            }

            form.ForceActivate();
        }

        public static void ShowScreenColorPickerDialog(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();
            taskSettings.CaptureSettings.SurfaceOptions.ScreenColorPickerInfoText = taskSettings.ToolsSettings.ScreenColorPickerInfoText;

            RegionCaptureTasks.ShowScreenColorPickerDialog(taskSettings.CaptureSettingsReference.SurfaceOptions);
        }

        public static void OpenScreenColorPicker(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();
            taskSettings.CaptureSettings.SurfaceOptions.ScreenColorPickerInfoText = taskSettings.ToolsSettings.ScreenColorPickerInfoText;

            PointInfo pointInfo = RegionCaptureTasks.GetPointInfo(taskSettings.CaptureSettings.SurfaceOptions);

            if (pointInfo != null)
            {
                string input;

                if (Control.ModifierKeys == Keys.Control)
                {
                    input = taskSettings.ToolsSettings.ScreenColorPickerFormatCtrl;
                }
                else
                {
                    input = taskSettings.ToolsSettings.ScreenColorPickerFormat;
                }

                if (!string.IsNullOrEmpty(input))
                {
                    string text = CodeMenuEntryPixelInfo.Parse(input, pointInfo.Color, pointInfo.Position);
                    ClipboardHelpers.CopyText(text);

                    ShowNotificationTip(string.Format(Resources.TaskHelpers_OpenQuickScreenColorPicker_Copied_to_clipboard___0_, text),
                        "ShareX - " + Resources.ScreenColorPicker);
                }
            }
        }

        public static void OpenHashCheck()
        {
            new HashCheckForm().Show();
        }

        public static void OpenDirectoryIndexer(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            IndexerSettings indexerSettings = taskSettings.ToolsSettingsReference.IndexerSettings;
            indexerSettings.BinaryUnits = Program.Settings.BinaryUnits;
            DirectoryIndexerForm form = new DirectoryIndexerForm(indexerSettings);
            form.UploadRequested += source =>
            {
                WorkerTask task = WorkerTask.CreateTextUploaderTask(source, taskSettings);
                task.Info.FileName = Path.ChangeExtension(task.Info.FileName, indexerSettings.Output.ToString().ToLowerInvariant());
                TaskManager.Start(task);
            };
            form.Show();
        }

        public static void OpenImageCombiner(IEnumerable<string> imageFiles = null, TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            ImageCombinerForm imageCombinerForm = new ImageCombinerForm(taskSettings.ToolsSettingsReference.ImageCombinerOptions, imageFiles);
            imageCombinerForm.ProcessRequested += bmp => UploadManager.RunImageTask(bmp, taskSettings);
            imageCombinerForm.Show();
        }

        public static void CombineImages(IEnumerable<string> imageFiles, Orientation orientation, TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            Bitmap output = ImageHelpers.CombineImages(imageFiles, orientation, taskSettings.ToolsSettings.ImageCombinerOptions.Alignment,
                taskSettings.ToolsSettings.ImageCombinerOptions.Space, taskSettings.ToolsSettings.ImageCombinerOptions.AutoFillBackground);

            if (output != null)
            {
                UploadManager.RunImageTask(output, taskSettings);
            }
        }

        public static void OpenImageSplitter()
        {
            ImageSplitterForm imageSplitterForm = new ImageSplitterForm();
            imageSplitterForm.Show();
        }

        public static void OpenImageThumbnailer()
        {
            ImageThumbnailerForm imageThumbnailerForm = new ImageThumbnailerForm();
            imageThumbnailerForm.Show();
        }

        public static void OpenVideoConverter(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (!CheckFFmpeg(taskSettings))
            {
                return;
            }

            VideoConverterForm videoConverterForm = new VideoConverterForm(taskSettings.CaptureSettings.FFmpegOptions.FFmpegPath,
                taskSettings.ToolsSettingsReference.VideoConverterOptions);
            videoConverterForm.Show();
        }

        public static void OpenVideoThumbnailer(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (!CheckFFmpeg(taskSettings))
            {
                return;
            }

            taskSettings.ToolsSettingsReference.VideoThumbnailOptions.DefaultOutputDirectory = GetScreenshotsFolder(taskSettings);
            VideoThumbnailerForm thumbnailerForm = new VideoThumbnailerForm(taskSettings.CaptureSettings.FFmpegOptions.FFmpegPath,
                taskSettings.ToolsSettingsReference.VideoThumbnailOptions);
            thumbnailerForm.ThumbnailsTaken += thumbnails =>
            {
                if (taskSettings.ToolsSettingsReference.VideoThumbnailOptions.UploadThumbnails)
                {
                    foreach (VideoThumbnailInfo thumbnailInfo in thumbnails)
                    {
                        UploadManager.UploadFile(thumbnailInfo.FilePath, taskSettings);
                    }
                }
            };
            thumbnailerForm.Show();
        }

        public static void OpenBorderlessWindow(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            BorderlessWindowSettings settings = taskSettings.ToolsSettingsReference.BorderlessWindowSettings;
            BorderlessWindowForm borderlessWindowForm = new BorderlessWindowForm(settings);
            borderlessWindowForm.Show();
        }

        public static void OpenInspectWindow()
        {
            InspectWindowForm inspectWindowForm = new InspectWindowForm();
            inspectWindowForm.Show();
        }

        public static void OpenClipboardViewer()
        {
            ClipboardViewerForm clipboardViewerForm = new ClipboardViewerForm();
            clipboardViewerForm.Show();
        }

        public static void OpenImageEditor(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            using (EditorStartupForm editorStartupForm = new EditorStartupForm(taskSettings.CaptureSettingsReference.SurfaceOptions))
            {
                if (editorStartupForm.ShowDialog() == DialogResult.OK)
                {
                    AnnotateImageAsync(editorStartupForm.Image, editorStartupForm.ImageFilePath, taskSettings);
                }
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

                        form.CopyImageRequested += output =>
                        {
                            Program.MainForm.InvokeSafe(() => ClipboardHelpers.CopyImage(output));
                        };

                        form.UploadImageRequested += output =>
                        {
                            Program.MainForm.InvokeSafe(() =>
                            {
                                UploadManager.UploadImage(output, taskSettings);
                            });
                        };

                        form.PrintImageRequested += output =>
                        {
                            Program.MainForm.InvokeSafe(() =>
                            {
                                using (output) { PrintImage(output); }
                            });
                        };

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

                    using (ImageEffectsForm imageEffectsForm = new ImageEffectsForm(bmp, taskSettings.ImageSettings.ImageEffectPresets,
                        taskSettings.ImageSettings.SelectedImageEffectPreset))
                    {
                        imageEffectsForm.EnableToolMode(x => UploadManager.RunImageTask(x, taskSettings), filePath);
                        imageEffectsForm.ShowDialog();
                        //taskSettings.ImageSettings.SelectedImageEffectPreset = imageEffectsForm.SelectedPresetIndex;
                    }
                }
            }
        }

        public static ImageEffectsForm OpenImageEffectsSingleton(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = Program.DefaultTaskSettings;

            bool firstInstance = !ImageEffectsForm.IsInstanceActive;

            ImageEffectsForm imageEffectsForm = ImageEffectsForm.GetFormInstance(taskSettings.ImageSettings.ImageEffectPresets,
                taskSettings.ImageSettings.SelectedImageEffectPreset);

            if (firstInstance)
            {
                imageEffectsForm.FormClosed += (sender, e) => taskSettings.ImageSettings.SelectedImageEffectPreset = imageEffectsForm.SelectedPresetIndex;
                imageEffectsForm.Show();
            }
            else
            {
                imageEffectsForm.ForceActivate();
            }

            return imageEffectsForm;
        }

        public static void OpenImageViewer()
        {
            string filePath = ImageHelpers.OpenImageFileDialog();
            OpenImageViewer(filePath);
        }

        public static void OpenImageViewer(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                string folderPath = Path.GetDirectoryName(filePath);
                string[] files = Directory.GetFiles(folderPath);

                if (files != null && files.Length > 0)
                {
                    int imageIndex = Array.IndexOf(files, filePath);
                    ImageViewer.ShowImage(files, imageIndex);
                }
            }
        }

        public static void OpenMonitorTest()
        {
            using (MonitorTestForm monitorTestForm = new MonitorTestForm())
            {
                monitorTestForm.ShowDialog();
            }
        }

        public static void OpenDNSChanger()
        {
#if MicrosoftStore
            MessageBox.Show("Not supported in Microsoft Store build.", "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
#else
            if (Helpers.IsAdministrator())
            {
                new DNSChangerForm().Show();
            }
            else
            {
                RunShareXAsAdmin("-dnschanger");
            }
#endif
        }

        public static void RunShareXAsAdmin(string arguments = null)
        {
            try
            {
                using (Process process = new Process())
                {
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = Application.ExecutablePath,
                        Arguments = arguments,
                        UseShellExecute = true,
                        Verb = "runas"
                    };

                    process.StartInfo = psi;
                    process.Start();
                }
            }
            catch
            {
            }
        }

        public static void OpenQRCode()
        {
            QRCodeForm.EncodeClipboard().Show();
        }

        public static void OpenQRCodeDecodeFromScreen()
        {
            QRCodeForm.OpenFormDecodeFromScreen();
        }

        public static void OpenRuler(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            RegionCaptureTasks.ShowScreenRuler(taskSettings.CaptureSettings.SurfaceOptions);
        }

        public static void SearchImageUsingGoogle(string url)
        {
            new GoogleImageSearchSharingService().CreateSharer(null, null).ShareURL(url);
        }

        public static void SearchImageUsingBing(string url)
        {
            new BingVisualSearchSharingService().CreateSharer(null, null).ShareURL(url);
        }

        public static async Task OCRImage(TaskSettings taskSettings = null)
        {
            if (IsUploadAllowed())
            {
                if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

                using (Bitmap bmp = RegionCaptureTasks.GetRegionImage(taskSettings.CaptureSettings.SurfaceOptions))
                {
                    await OCRImage(bmp, taskSettings);
                }
            }
        }

        public static async Task OCRImage(Image img, TaskSettings taskSettings = null)
        {
            if (IsUploadAllowed() && img != null)
            {
                using (Stream stream = SaveImageAsStream(img, EImageFormat.PNG))
                {
                    await OCRImage(stream, "ShareX.png", null, taskSettings);
                }
            }
        }

        public static async Task OCRImage(string filePath, TaskSettings taskSettings = null)
        {
            if (IsUploadAllowed() && File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await OCRImage(fs, Path.GetFileName(filePath), filePath, taskSettings);
                }
            }
        }

        private static async Task OCRImage(Stream stream, string fileName, string filePath = null, TaskSettings taskSettings = null)
        {
            if (stream != null)
            {
                if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

                OCROptions ocrOptions = taskSettings.CaptureSettingsReference.OCROptions;

                if (!ocrOptions.Permission)
                {
                    if (MessageBox.Show(Resources.PleaseNoteThatShareXIsUsingOCRSpaceSOnlineAPIToPerformOpticalCharacterRecognitionDoYouGivePermissionToShareXToUploadImagesToThisService,
                        Resources.ShareXOpticalCharacterRecognition, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        ocrOptions.Permission = true;
                    }
                    else
                    {
                        return;
                    }
                }

                if (ocrOptions.Silent)
                {
                    await AsyncOCRImage(stream, fileName, filePath, ocrOptions);
                }
                else
                {
                    using (OCRSpaceForm form = new OCRSpaceForm(stream, fileName, ocrOptions))
                    {
                        form.ShowDialog();

                        if (!string.IsNullOrEmpty(form.Result) && !string.IsNullOrEmpty(filePath))
                        {
                            string textPath = Path.ChangeExtension(filePath, "txt");
                            File.WriteAllText(textPath, form.Result, Encoding.UTF8);
                        }
                    }
                }
            }
        }

        private static async Task AsyncOCRImage(Stream stream, string fileName, string filePath, OCROptions ocrOptions)
        {
            ShowNotificationTip(Resources.OCRForm_AutoProcessing);

            string result = null;

            if (stream != null && stream.Length > 0 && !string.IsNullOrEmpty(fileName))
            {
                result = await OCRSpace.DoOCRAsync(ocrOptions.DefaultLanguage, stream, fileName);
            }

            if (!string.IsNullOrEmpty(result))
            {
                ClipboardHelpers.CopyText(result);

                if (!string.IsNullOrEmpty(filePath))
                {
                    string textPath = Path.ChangeExtension(filePath, "txt");
                    File.WriteAllText(textPath, result, Encoding.UTF8);
                }

                ShowNotificationTip(Resources.OCRForm_AutoComplete);
            }
            else
            {
                ShowNotificationTip(Resources.OCRForm_AutoCompleteFail);
            }
        }

        public static void TweetMessage()
        {
            if (IsUploadAllowed())
            {
                if (Program.UploadersConfig != null && Program.UploadersConfig.TwitterOAuthInfoList != null)
                {
                    OAuthInfo twitterOAuth = Program.UploadersConfig.TwitterOAuthInfoList.ReturnIfValidIndex(Program.UploadersConfig.TwitterSelectedAccount);

                    if (twitterOAuth != null && OAuthInfo.CheckOAuth(twitterOAuth))
                    {
                        Task.Run(() =>
                        {
                            using (TwitterTweetForm twitter = new TwitterTweetForm(twitterOAuth))
                            {
                                if (twitter.ShowDialog() == DialogResult.OK && twitter.IsTweetSent)
                                {
                                    ShowNotificationTip(Resources.TaskHelpers_TweetMessage_Tweet_successfully_sent_);
                                }
                            }
                        });

                        return;
                    }
                }

                MessageBox.Show(Resources.TaskHelpers_TweetMessage_Unable_to_find_valid_Twitter_account_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static EDataType FindDataType(string filePath, TaskSettings taskSettings)
        {
            if (Helpers.CheckExtension(filePath, taskSettings.AdvancedSettings.ImageExtensions))
            {
                return EDataType.Image;
            }

            if (Helpers.CheckExtension(filePath, taskSettings.AdvancedSettings.TextExtensions))
            {
                return EDataType.Text;
            }

            return EDataType.File;
        }

        public static bool ToggleHotkeys()
        {
            bool disableHotkeys = !Program.Settings.DisableHotkeys;
            ToggleHotkeys(disableHotkeys);
            return disableHotkeys;
        }

        public static void ToggleHotkeys(bool disableHotkeys)
        {
            Program.Settings.DisableHotkeys = disableHotkeys;
            Program.HotkeyManager.ToggleHotkeys(disableHotkeys);
            Program.MainForm.UpdateToggleHotkeyButton();

            ShowNotificationTip(disableHotkeys ? Resources.TaskHelpers_ToggleHotkeys_Hotkeys_disabled_ : Resources.TaskHelpers_ToggleHotkeys_Hotkeys_enabled_);
        }

        public static bool CheckFFmpeg(TaskSettings taskSettings)
        {
            string ffmpegPath = taskSettings.CaptureSettings.FFmpegOptions.FFmpegPath;

            if (string.IsNullOrEmpty(ffmpegPath))
            {
                ffmpegPath = Program.DefaultFFmpegFilePath;
            }

            if (!File.Exists(ffmpegPath))
            {
                if (MessageBox.Show(string.Format(Resources.FFmpeg_does_not_exist, ffmpegPath),
                    "ShareX - " + Resources.FFmpeg_Missing + " ffmpeg.exe", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DialogResult downloadDialogResult = FFmpegGitHubDownloader.DownloadFFmpeg(false, DownloaderForm_InstallRequested);

                    if (downloadDialogResult == DialogResult.OK)
                    {
                        Program.DefaultTaskSettings.CaptureSettings.FFmpegOptions.CLIPath = taskSettings.TaskSettingsReference.CaptureSettings.FFmpegOptions.CLIPath =
                            taskSettings.CaptureSettings.FFmpegOptions.CLIPath = Program.DefaultFFmpegFilePath;

#if STEAM || MicrosoftStore
                        Program.DefaultTaskSettings.CaptureSettings.FFmpegOptions.OverrideCLIPath = taskSettings.TaskSettingsReference.CaptureSettings.FFmpegOptions.OverrideCLIPath =
                          taskSettings.CaptureSettings.FFmpegOptions.OverrideCLIPath = true;
#endif
                    }
                    else if (downloadDialogResult == DialogResult.Cancel)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private static void DownloaderForm_InstallRequested(string filePath)
        {
            bool result = FFmpegGitHubDownloader.ExtractFFmpeg(filePath, Program.ToolsFolder);

            if (result)
            {
                MessageBox.Show(Resources.FFmpeg_FFmpeg_successfully_downloaded, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(Resources.FFmpeg_Download_of_FFmpeg_failed, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void PlayCaptureSound(TaskSettings taskSettings)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (taskSettings.GeneralSettings.UseCustomCaptureSound && !string.IsNullOrEmpty(taskSettings.GeneralSettings.CustomCaptureSoundPath))
            {
                Helpers.PlaySoundAsync(taskSettings.GeneralSettings.CustomCaptureSoundPath);
            }
            else
            {
                Helpers.PlaySoundAsync(Resources.CaptureSound);
            }
        }

        public static void PlayTaskCompleteSound(TaskSettings taskSettings)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (taskSettings.GeneralSettings.UseCustomTaskCompletedSound && !string.IsNullOrEmpty(taskSettings.GeneralSettings.CustomTaskCompletedSoundPath))
            {
                Helpers.PlaySoundAsync(taskSettings.GeneralSettings.CustomTaskCompletedSoundPath);
            }
            else
            {
                Helpers.PlaySoundAsync(Resources.TaskCompletedSound);
            }
        }

        public static void PlayErrorSound(TaskSettings taskSettings)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (taskSettings.GeneralSettings.UseCustomErrorSound && !string.IsNullOrEmpty(taskSettings.GeneralSettings.CustomErrorSoundPath))
            {
                Helpers.PlaySoundAsync(taskSettings.GeneralSettings.CustomErrorSoundPath);
            }
            else
            {
                Helpers.PlaySoundAsync(Resources.ErrorSound);
            }
        }

        public static void OpenUploadersConfigWindow(IUploaderService uploaderService = null)
        {
            SettingManager.WaitUploadersConfig();

            bool firstInstance = !UploadersConfigForm.IsInstanceActive;

            UploadersConfigForm form = UploadersConfigForm.GetFormInstance(Program.UploadersConfig);

            if (firstInstance)
            {
                form.FormClosed += (sender, e) => SettingManager.SaveUploadersConfigAsync();

                if (uploaderService != null)
                {
                    form.NavigateToTabPage(uploaderService.GetUploadersConfigTabPage(form));
                }

                form.Show();
            }
            else
            {
                if (uploaderService != null)
                {
                    form.NavigateToTabPage(uploaderService.GetUploadersConfigTabPage(form));
                }

                form.ForceActivate();
            }
        }

        public static void OpenCustomUploaderSettingsWindow()
        {
            SettingManager.WaitUploadersConfig();

            bool firstInstance = !CustomUploaderSettingsForm.IsInstanceActive;

            CustomUploaderSettingsForm form = CustomUploaderSettingsForm.GetFormInstance(Program.UploadersConfig);

            if (firstInstance)
            {
                form.FormClosed += (sender, e) => SettingManager.SaveUploadersConfigAsync();
                form.Show();
            }
            else
            {
                form.ForceActivate();
            }
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
                    case AfterCaptureTasks.AddImageEffects: return Resources.image_saturation;
                    case AfterCaptureTasks.AnnotateImage: return Resources.image_pencil;
                    case AfterCaptureTasks.CopyImageToClipboard: return Resources.clipboard_paste_image;
                    case AfterCaptureTasks.SendImageToPrinter: return Resources.printer;
                    case AfterCaptureTasks.SaveImageToFile: return Resources.disk;
                    case AfterCaptureTasks.SaveImageToFileWithDialog: return Resources.disk_rename;
                    case AfterCaptureTasks.SaveThumbnailImageToFile: return Resources.disk_small;
                    case AfterCaptureTasks.PerformActions: return Resources.application_terminal;
                    case AfterCaptureTasks.CopyFileToClipboard: return Resources.clipboard_block;
                    case AfterCaptureTasks.CopyFilePathToClipboard: return Resources.clipboard_list;
                    case AfterCaptureTasks.ShowInExplorer: return Resources.folder_stand;
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
                    case HotkeyType.LastRegion: return Resources.layers;
                    case HotkeyType.ScrollingCapture: return Resources.ui_scroll_pane_image;
                    case HotkeyType.TextCapture: return ShareXResources.IsDarkTheme ? Resources.edit_drop_cap_white : Resources.edit_drop_cap;
                    case HotkeyType.AutoCapture: return Resources.clock;
                    case HotkeyType.StartAutoCapture: return Resources.clock__arrow;
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
                    case HotkeyType.AbortScreenRecording: return Resources.camcorder__exclamation;
                    // Tools
                    case HotkeyType.ColorPicker: return Resources.color;
                    case HotkeyType.ScreenColorPicker: return Resources.pipette;
                    case HotkeyType.ImageEditor: return Resources.image_pencil;
                    case HotkeyType.ImageEffects: return Resources.image_saturation;
                    case HotkeyType.ImageViewer: return Resources.images_flickr;
                    case HotkeyType.HashCheck: return Resources.application_task;
                    case HotkeyType.DNSChanger: return Resources.network_ip;
                    case HotkeyType.QRCode: return ShareXResources.IsDarkTheme ? Resources.barcode_2d_white : Resources.barcode_2d;
                    case HotkeyType.QRCodeDecodeFromScreen: return ShareXResources.IsDarkTheme ? Resources.barcode_2d_white : Resources.barcode_2d;
                    case HotkeyType.Ruler: return Resources.ruler_triangle;
                    case HotkeyType.IndexFolder: return Resources.folder_tree;
                    case HotkeyType.ImageCombiner: return Resources.document_break;
                    case HotkeyType.ImageSplitter: return Resources.image_split;
                    case HotkeyType.ImageThumbnailer: return Resources.image_resize_actual;
                    case HotkeyType.VideoConverter: return Resources.camcorder_pencil;
                    case HotkeyType.VideoThumbnailer: return Resources.images_stack;
                    case HotkeyType.InspectWindow: return Resources.application_search_result;
                    case HotkeyType.ClipboardViewer: return Resources.clipboard_block;
                    case HotkeyType.TweetMessage: return Resources.Twitter;
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
                AutoHideTaskbar = taskSettings.CaptureSettings.CaptureAutoHideTaskbar
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

                            Program.MainForm.UpdateCheckStates();
                            Program.MainForm.UpdateUploaderMenuNames();
                        }

                        if (CustomUploaderSettingsForm.IsInstanceActive)
                        {
                            CustomUploaderSettingsForm.CustomUploaderUpdateTab();
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                    e.ShowError();
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
                ImageEffectsForm imageEffectsForm = OpenImageEffectsSingleton(Program.DefaultTaskSettings);

                if (imageEffectsForm != null)
                {
                    imageEffectsForm.ImportImageEffect(configJson);
                }

                if (!Program.DefaultTaskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.AddImageEffects) &&
                    MessageBox.Show(Resources.WouldYouLikeToEnableImageEffects,
                    "ShareX", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Program.DefaultTaskSettings.AfterCaptureJob = Program.DefaultTaskSettings.AfterCaptureJob.Add(AfterCaptureTasks.AddImageEffects);
                    Program.MainForm.UpdateCheckStates();
                }
            }
        }

        public static void HandleNativeMessagingInput(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                NativeMessagingInput nativeMessagingInput = null;

                try
                {
                    nativeMessagingInput = JsonHelpers.DeserializeFromFile<NativeMessagingInput>(filePath);

                    File.Delete(filePath);
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }

                if (nativeMessagingInput != null)
                {
                    if (!string.IsNullOrEmpty(nativeMessagingInput.URL) && URLHelpers.IsValidURL(nativeMessagingInput.URL))
                    {
                        UploadManager.DownloadAndUploadFile(nativeMessagingInput.URL);
                    }
                    else if (!string.IsNullOrEmpty(nativeMessagingInput.Text))
                    {
                        UploadManager.UploadText(nativeMessagingInput.Text);
                    }
                }
            }
        }

        public static void OpenActionsToolbar()
        {
            ActionsToolbarForm.Instance.ForceActivate();
        }

        public static void ToggleActionsToolbar()
        {
            if (ActionsToolbarForm.IsInstanceActive)
            {
                ActionsToolbarForm.Instance.Close();
            }
            else
            {
                ActionsToolbarForm.Instance.ForceActivate();
            }
        }

        public static async Task DownloadAppVeyorBuild()
        {
            AppVeyorUpdateChecker updateChecker = new AppVeyorUpdateChecker()
            {
                IsBeta = Program.Dev,
                IsPortable = Program.Portable,
                Proxy = HelpersOptions.CurrentProxy.GetWebProxy(),
                Branch = "develop"
            };

            await updateChecker.CheckUpdateAsync();

            if (updateChecker.Status == UpdateStatus.UpdateAvailable)
            {
                updateChecker.DownloadUpdate();
            }
        }

        public static Image CreateQRCode(string text, int width, int height)
        {
            if (CheckQRCodeContent(text))
            {
                try
                {
                    BarcodeWriter writer = new BarcodeWriter
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Options = new QrCodeEncodingOptions
                        {
                            Width = width,
                            Height = height,
                            CharacterSet = "UTF-8"
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

        public static Image CreateQRCode(string text, int size)
        {
            return CreateQRCode(text, size, size);
        }

        public static string[] BarcodeScan(Bitmap bmp, bool scanQRCodeOnly = false)
        {
            try
            {
                BarcodeReader barcodeReader = new BarcodeReader
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

        public static void ShowBalloonTip(string text, ToolTipIcon icon, int timeout, string title = "ShareX", BalloonTipAction clickAction = null)
        {
            if (Program.MainForm != null && !Program.MainForm.IsDisposed && Program.MainForm.niTray != null && Program.MainForm.niTray.Visible)
            {
                Program.MainForm.niTray.Tag = clickAction;
                Program.MainForm.niTray.ShowBalloonTip(timeout, title, text, icon);
            }
        }

        public static void ShowNotificationTip(string text, string title = "ShareX", int duration = -1)
        {
            if (duration < 0)
            {
                duration = (int)(Program.DefaultTaskSettings.GeneralSettings.ToastWindowDuration * 1000);
            }

            NotificationFormConfig toastConfig = new NotificationFormConfig()
            {
                Duration = duration,
                FadeDuration = (int)(Program.DefaultTaskSettings.GeneralSettings.ToastWindowFadeDuration * 1000),
                Placement = Program.DefaultTaskSettings.GeneralSettings.ToastWindowPlacement,
                Size = Program.DefaultTaskSettings.GeneralSettings.ToastWindowSize,
                Title = title,
                Text = text
            };

            Program.MainForm.InvokeSafe(() =>
            {
                NotificationForm.Show(toastConfig);
            });
        }

        public static void ToggleTrayMenu()
        {
            ContextMenuStrip cmsTray = Program.MainForm.niTray.ContextMenuStrip;

            if (cmsTray != null && !cmsTray.IsDisposed)
            {
                if (cmsTray.Visible)
                {
                    cmsTray.Close();
                }
                else
                {
                    NativeMethods.SetForegroundWindow(cmsTray.Handle);
                    cmsTray.Show(Cursor.Position);
                }
            }
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