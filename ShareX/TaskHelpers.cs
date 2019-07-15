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
using System.Speech.Synthesis;
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
                    OpenImageEffects(taskSettings);
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
                case HotkeyType.Ruler:
                    OpenRuler(safeTaskSettings);
                    break;
                case HotkeyType.IndexFolder:
                    UploadManager.IndexFolder();
                    break;
                case HotkeyType.ImageCombiner:
                    OpenImageCombiner(safeTaskSettings);
                    break;
                case HotkeyType.VideoThumbnailer:
                    OpenVideoThumbnailer(safeTaskSettings);
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
                imageData.ImageStream = SaveImageAsStream(img, EImageFormat.JPEG, taskSettings);
                imageData.ImageFormat = EImageFormat.JPEG;
            }

            return imageData;
        }

        public static string CreateThumbnail(Image img, string folder, string filename, TaskSettings taskSettings)
        {
            if ((taskSettings.ImageSettings.ThumbnailWidth > 0 || taskSettings.ImageSettings.ThumbnailHeight > 0) && (!taskSettings.ImageSettings.ThumbnailCheckSize ||
                (img.Width > taskSettings.ImageSettings.ThumbnailWidth && img.Height > taskSettings.ImageSettings.ThumbnailHeight)))
            {
                string thumbnailFileName = Path.GetFileNameWithoutExtension(filename) + taskSettings.ImageSettings.ThumbnailName + ".jpg";
                string thumbnailFilePath = HandleExistsFile(folder, thumbnailFileName, taskSettings);

                if (!string.IsNullOrEmpty(thumbnailFilePath))
                {
                    Image thumbImage = null;

                    try
                    {
                        thumbImage = (Image)img.Clone();
                        thumbImage = new Resize(taskSettings.ImageSettings.ThumbnailWidth, taskSettings.ImageSettings.ThumbnailHeight).Apply(thumbImage);
                        thumbImage = ImageHelpers.FillBackground(thumbImage, Color.White);
                        thumbImage.SaveJPG(thumbnailFilePath, 90);
                        return thumbnailFilePath;
                    }
                    finally
                    {
                        if (thumbImage != null)
                        {
                            thumbImage.Dispose();
                        }
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
            MemoryStream stream = new MemoryStream();

            switch (imageFormat)
            {
                case EImageFormat.PNG:
                    SaveImageAsPNGStream(img, stream, pngBitDepth);

                    if (Program.Settings.PNGStripColorSpaceInformation)
                    {
                        using (stream)
                        {
                            return PNGStripColorSpaceInformation(stream);
                        }
                    }
                    break;
                case EImageFormat.JPEG:
                    SaveImageAsJPEGStream(img, stream, jpegQuality);
                    break;
                case EImageFormat.GIF:
                    img.SaveGIF(stream, gifQuality);
                    break;
                case EImageFormat.BMP:
                    img.Save(stream, ImageFormat.Bmp);
                    break;
                case EImageFormat.TIFF:
                    img.Save(stream, ImageFormat.Tiff);
                    break;
            }

            return stream;
        }

        private static void SaveImageAsPNGStream(Image img, Stream stream, PNGBitDepth bitDepth)
        {
            if (bitDepth == PNGBitDepth.Automatic)
            {
                if (ImageHelpers.IsImageTransparent((Bitmap)img))
                {
                    bitDepth = PNGBitDepth.Bit32;
                }
                else
                {
                    bitDepth = PNGBitDepth.Bit24;
                }
            }

            if (bitDepth == PNGBitDepth.Bit32)
            {
                if (img.PixelFormat != PixelFormat.Format32bppArgb && img.PixelFormat != PixelFormat.Format32bppRgb)
                {
                    using (Bitmap bmpNew = ((Bitmap)img).Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format32bppArgb))
                    {
                        bmpNew.Save(stream, ImageFormat.Png);
                        return;
                    }
                }
            }
            else if (bitDepth == PNGBitDepth.Bit24)
            {
                if (img.PixelFormat != PixelFormat.Format24bppRgb)
                {
                    using (Bitmap bmpNew = ((Bitmap)img).Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb))
                    {
                        bmpNew.Save(stream, ImageFormat.Png);
                        return;
                    }
                }
            }

            img.Save(stream, ImageFormat.Png);
        }

        private static MemoryStream PNGStripChunks(MemoryStream stream, params string[] chunks)
        {
            MemoryStream output = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);

            byte[] signature = new byte[8];
            stream.Read(signature, 0, 8);
            output.Write(signature, 0, 8);

            while (true)
            {
                byte[] lenBytes = new byte[4];
                if (stream.Read(lenBytes, 0, 4) != 4)
                {
                    break;
                }

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lenBytes);
                }

                int len = BitConverter.ToInt32(lenBytes, 0);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lenBytes);
                }

                byte[] type = new byte[4];
                stream.Read(type, 0, 4);

                byte[] data = new byte[len + 4];
                stream.Read(data, 0, data.Length);

                string strType = Encoding.ASCII.GetString(type);

                if (!chunks.Contains(strType))
                {
                    output.Write(lenBytes, 0, lenBytes.Length);
                    output.Write(type, 0, type.Length);
                    output.Write(data, 0, data.Length);
                }
            }

            return output;
        }

        private static MemoryStream PNGStripColorSpaceInformation(MemoryStream stream)
        {
            // http://www.libpng.org/pub/png/spec/1.2/PNG-Chunks.html
            // 4.2.2.1. gAMA Image gamma
            // 4.2.2.2. cHRM Primary chromaticities
            // 4.2.2.3. sRGB Standard RGB color space
            // 4.2.2.4. iCCP Embedded ICC profile
            return PNGStripChunks(stream, "gAMA", "cHRM", "sRGB", "iCCP");
        }

        private static void SaveImageAsJPEGStream(Image img, Stream stream, int jpegQuality)
        {
            try
            {
                img = (Image)img.Clone();
                img = ImageHelpers.FillBackground(img, Color.White);
                img.SaveJPG(stream, jpegQuality);
            }
            finally
            {
                if (img != null) img.Dispose();
            }
        }

        public static void SaveImageAsFile(Image img, TaskSettings taskSettings, bool overwriteFile = false)
        {
            using (ImageData imageData = PrepareImage(img, taskSettings))
            {
                string fileName = GetFilename(taskSettings, imageData.ImageFormat.GetDescription(), img);
                string filePath = Path.Combine(taskSettings.CaptureFolder, fileName);

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

        public static string GetFilename(TaskSettings taskSettings, string extension, Image image)
        {
            ImageInfo imageInfo = new ImageInfo(image);
            return GetFilename(taskSettings, extension, imageInfo);
        }

        public static string GetFilename(TaskSettings taskSettings, string extension = null, ImageInfo imageInfo = null)
        {
            string filename;

            NameParser nameParser = new NameParser(NameParserType.FileName)
            {
                AutoIncrementNumber = Program.Settings.NameParserAutoIncrementNumber,
                MaxNameLength = taskSettings.AdvancedSettings.NamePatternMaxLength,
                MaxTitleLength = taskSettings.AdvancedSettings.NamePatternMaxTitleLength,
                CustomTimeZone = taskSettings.UploadSettings.UseCustomTimeZone ? taskSettings.UploadSettings.CustomTimeZone : null
            };

            if (imageInfo != null)
            {
                if (imageInfo.Image != null)
                {
                    nameParser.ImageWidth = imageInfo.Image.Width;
                    nameParser.ImageHeight = imageInfo.Image.Height;
                }

                nameParser.WindowText = imageInfo.WindowTitle;
                nameParser.ProcessName = imageInfo.ProcessName;
            }

            if (!string.IsNullOrEmpty(nameParser.WindowText))
            {
                filename = nameParser.Parse(taskSettings.UploadSettings.NameFormatPatternActiveWindow);
            }
            else
            {
                filename = nameParser.Parse(taskSettings.UploadSettings.NameFormatPattern);
            }

            Program.Settings.NameParserAutoIncrementNumber = nameParser.AutoIncrementNumber;

            if (!string.IsNullOrEmpty(extension))
            {
                filename += "." + extension.TrimStart('.');
            }

            return filename;
        }

        public static bool ShowAfterCaptureForm(TaskSettings taskSettings, out string fileName, ImageInfo imageInfo = null, string filePath = null)
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
                        afterCaptureForm = new AfterCaptureForm(imageInfo, taskSettings);
                    }

                    if (afterCaptureForm.ShowDialog() == DialogResult.Cancel)
                    {
                        if (imageInfo != null)
                        {
                            imageInfo.Dispose();
                        }

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

        public static Image AddImageEffects(Image img, TaskSettingsImage taskSettingsImage)
        {
            if (!img.PixelFormat.HasFlag(PixelFormat.Indexed))
            {
                if (taskSettingsImage.ShowImageEffectsWindowAfterCapture)
                {
                    using (ImageEffectsForm imageEffectsForm = new ImageEffectsForm(img, taskSettingsImage.ImageEffectPresets,
                        taskSettingsImage.SelectedImageEffectPreset))
                    {
                        imageEffectsForm.ShowDialog();
                        taskSettingsImage.SelectedImageEffectPreset = imageEffectsForm.SelectedPresetIndex;
                    }
                }

                if (taskSettingsImage.ImageEffectPresets.IsValidIndex(taskSettingsImage.SelectedImageEffectPreset))
                {
                    using (img)
                    {
                        return taskSettingsImage.ImageEffectPresets[taskSettingsImage.SelectedImageEffectPreset].ApplyEffects(img);
                    }
                }
            }

            return img;
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
            AddExternalProgramFromFile(taskSettings, "OptiPNG", "optipng.exe");
        }

        private static void AddExternalProgramFromFile(TaskSettings taskSettings, string name, string filename, string args = "")
        {
            if (!taskSettings.ExternalPrograms.Exists(x => x.Name == name))
            {
                if (File.Exists(filename))
                {
                    DebugHelper.WriteLine("Found program: " + filename);

                    taskSettings.ExternalPrograms.Add(new ExternalProgram(name, filename, args));
                }
            }
        }

        private static void AddExternalProgramFromRegistry(TaskSettings taskSettings, string name, string filename)
        {
            if (!taskSettings.ExternalPrograms.Exists(x => x.Name == name))
            {
                ExternalProgram externalProgram = RegistryHelpers.FindProgram(name, filename);

                if (externalProgram != null)
                {
                    taskSettings.ExternalPrograms.Add(externalProgram);
                }
            }
        }

        public static Icon GetProgressIcon(int percentage)
        {
            percentage = percentage.Clamp(0, 99);

            Size size = SystemInformation.SmallIconSize;
            using (Bitmap bmp = new Bitmap(size.Width, size.Height))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                int y = (int)(size.Height * (percentage / 100f));

                if (y > 0)
                {
                    using (Brush brush = new SolidBrush(Color.FromArgb(16, 116, 193)))
                    {
                        g.FillRectangle(brush, 0, size.Height - 1 - y, size.Width, y);
                    }
                }

                using (Font font = new Font("Arial", 10))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(percentage.ToString(), font, Brushes.Black, size.Width / 2, size.Height / 2, sf);
                    g.DrawString(percentage.ToString(), font, Brushes.White, size.Width / 2, (size.Height / 2) - 1, sf);
                }

                return Icon.FromHandle(bmp.GetHicon());
            }
        }

        public static string HandleExistsFile(string folder, string filename, TaskSettings taskSettings)
        {
            string filepath = Path.Combine(folder, filename);
            return HandleExistsFile(filepath, taskSettings);
        }

        public static string HandleExistsFile(string filepath, TaskSettings taskSettings)
        {
            if (File.Exists(filepath))
            {
                switch (taskSettings.ImageSettings.FileExistAction)
                {
                    case FileExistAction.Ask:
                        using (FileExistForm form = new FileExistForm(filepath))
                        {
                            form.ShowDialog();
                            filepath = form.Filepath;
                        }
                        break;
                    case FileExistAction.UniqueName:
                        filepath = Helpers.GetUniqueFilePath(filepath);
                        break;
                    case FileExistAction.Cancel:
                        filepath = "";
                        break;
                }
            }

            return filepath;
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
            if (Directory.Exists(Program.ScreenshotsFolder))
            {
                Helpers.OpenFolder(Program.ScreenshotsFolder);
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

            RegionCaptureTasks.ShowScreenColorPickerDialog(taskSettings.CaptureSettings.SurfaceOptions, true);
        }

        public static void OpenScreenColorPicker(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            PointInfo pointInfo = RegionCaptureTasks.GetPointInfo(taskSettings.CaptureSettings.SurfaceOptions);

            if (pointInfo != null)
            {
                string text = CodeMenuEntryPixelInfo.Parse(taskSettings.ToolsSettings.ScreenColorPickerFormat, pointInfo.Color, pointInfo.Position);

                ClipboardHelpers.CopyText(text);

                if (!taskSettings.AdvancedSettings.DisableNotifications && taskSettings.GeneralSettings.PopUpNotification != PopUpNotificationType.None)
                {
                    ShowBalloonTip(string.Format(Resources.TaskHelpers_OpenQuickScreenColorPicker_Copied_to_clipboard___0_, text), ToolTipIcon.Info, 3000);
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

        public static void OpenImageCombiner(TaskSettings taskSettings = null, IEnumerable<string> imageFiles = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            ImageCombinerForm imageCombinerForm = new ImageCombinerForm(taskSettings.ToolsSettingsReference.ImageCombinerOptions, imageFiles);
            imageCombinerForm.ProcessRequested += img => UploadManager.RunImageTask(img, taskSettings);
            imageCombinerForm.Show();
        }

        public static void OpenImageThumbnailer(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            ImageThumbnailerForm imageThumbnailerForm = new ImageThumbnailerForm();
            imageThumbnailerForm.Show();
        }

        public static void OpenVideoThumbnailer(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (!CheckFFmpeg(taskSettings))
            {
                return;
            }

            taskSettings.ToolsSettingsReference.VideoThumbnailOptions.DefaultOutputDirectory = taskSettings.CaptureFolder;
            VideoThumbnailerForm thumbnailerForm = new VideoThumbnailerForm(taskSettings.CaptureSettings.FFmpegOptions.FFmpegPath, taskSettings.ToolsSettingsReference.VideoThumbnailOptions);
            thumbnailerForm.ThumbnailsTaken += thumbnails =>
            {
                if (taskSettings.ToolsSettingsReference.VideoThumbnailOptions.UploadThumbnails)
                {
                    foreach (VideoThumbnailInfo thumbnailInfo in thumbnails)
                    {
                        UploadManager.UploadFile(thumbnailInfo.Filepath, taskSettings);
                    }
                }
            };
            thumbnailerForm.Show();
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

                Image img = ImageHelpers.LoadImage(filePath);

                AnnotateImageAsync(img, filePath, taskSettings);
            }
            else
            {
                MessageBox.Show("File does not exist:" + Environment.NewLine + filePath, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static void AnnotateImageAsync(Image img, string filePath, TaskSettings taskSettings)
        {
            ThreadWorker worker = new ThreadWorker();

            worker.DoWork += () =>
            {
                img = AnnotateImage(img, filePath, taskSettings);
            };

            worker.Completed += () =>
            {
                if (img != null)
                {
                    UploadManager.RunImageTask(img, taskSettings);
                }
            };

            worker.Start(ApartmentState.STA);
        }

        public static Image AnnotateImage(Image img, string filePath, TaskSettings taskSettings, bool taskMode = false)
        {
            if (img != null)
            {
                using (img)
                {
                    RegionCaptureMode mode = taskMode ? RegionCaptureMode.TaskEditor : RegionCaptureMode.Editor;
                    RegionCaptureOptions options = taskSettings.CaptureSettingsReference.SurfaceOptions;

                    using (RegionCaptureForm form = new RegionCaptureForm(mode, options, img))
                    {
                        form.ImageFilePath = filePath;

                        form.SaveImageRequested += (output, newFilePath) =>
                        {
                            using (output)
                            {
                                if (string.IsNullOrEmpty(newFilePath))
                                {
                                    string fileName = GetFilename(taskSettings, taskSettings.ImageSettings.ImageFormat.GetDescription(), output);
                                    newFilePath = Path.Combine(taskSettings.CaptureFolder, fileName);
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
                                    string fileName = GetFilename(taskSettings, taskSettings.ImageSettings.ImageFormat.GetDescription(), output);
                                    newFilePath = Path.Combine(taskSettings.CaptureFolder, fileName);
                                }

                                newFilePath = ImageHelpers.SaveImageFileDialog(output, newFilePath);
                            }

                            return newFilePath;
                        };

                        form.CopyImageRequested += output =>
                        {
                            Program.MainForm.InvokeSafe(() =>
                            {
                                using (output) { ClipboardHelpers.CopyImage(output); }
                            });
                        };

                        form.UploadImageRequested += output =>
                        {
                            Program.MainForm.InvokeSafe(() =>
                            {
                                UploadManager.UploadImage(output);
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
                                return (Image)form.Canvas.Clone();
                        }
                    }
                }
            }

            return null;
        }

        public static void OpenImageEffects(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = Program.DefaultTaskSettings;

            string filePath = ImageHelpers.OpenImageFileDialog();
            Image img = null;
            if (!string.IsNullOrEmpty(filePath))
            {
                img = ImageHelpers.LoadImage(filePath);
            }

            if (img != null)
            {
                if (img.PixelFormat.HasFlag(PixelFormat.Indexed))
                {
                    MessageBox.Show("Unsupported pixel format: " + img.PixelFormat, "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    using (ImageEffectsForm imageEffectsForm = new ImageEffectsForm(img, taskSettings.ImageSettings.ImageEffectPresets,
                        taskSettings.ImageSettings.SelectedImageEffectPreset))
                    {
                        imageEffectsForm.EnableToolMode(x => UploadManager.RunImageTask(x, taskSettings));
                        imageEffectsForm.ShowDialog();
                        //taskSettings.ImageSettings.SelectedImageEffectPreset = imageEffectsForm.SelectedPresetIndex;
                    }
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
#if WindowsStore
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

        public static void RunShareXAsAdmin(string arguments)
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

        public static void OpenRuler(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            RegionCaptureTasks.ShowScreenRuler(taskSettings.CaptureSettings.SurfaceOptions);
        }

        public static void SearchImage(string url)
        {
            new GoogleImageSearchSharingService().CreateSharer(null, null).ShareURL(url);
        }

        public static async Task OCRImage(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            using (Image img = RegionCaptureTasks.GetRegionImage(taskSettings.CaptureSettings.SurfaceOptions))
            {
                await OCRImage(img, taskSettings);
            }
        }

        public static async Task OCRImage(Image img, TaskSettings taskSettings = null)
        {
            if (img != null)
            {
                using (Stream stream = SaveImageAsStream(img, EImageFormat.PNG))
                {
                    await OCRImage(stream, "ShareX.png", null, taskSettings);
                }
            }
        }

        public static async Task OCRImage(string filePath, TaskSettings taskSettings = null)
        {
            if (File.Exists(filePath))
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

                OCROptions ocrOptions = taskSettings.CaptureSettings.OCROptions;

                if (!ocrOptions.Permission)
                {
                    if (MessageBox.Show(Resources.PleaseNoteThatShareXIsUsingOCRSpaceSOnlineAPIToPerformOpticalCharacterRecognitionDoYouGivePermissionToShareXToUploadImagesToThisService,
                        Resources.ShareXOpticalCharacterRecognition, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        taskSettings.CaptureSettingsReference.OCROptions.Permission = true;
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

        public static async Task AsyncOCRImage(Stream stream, string fileName, string filePath, OCROptions ocrOptions)
        {
            ShowBalloonTip(Resources.OCRForm_AutoProcessing, ToolTipIcon.None, 3000);

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

                ShowBalloonTip(Resources.OCRForm_AutoComplete, ToolTipIcon.None, 3000);
            }
            else
            {
                ShowBalloonTip(Resources.OCRForm_AutoCompleteFail, ToolTipIcon.Warning, 3000);
            }
        }

        public static void TweetMessage()
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
                                ShowBalloonTip(Resources.TaskHelpers_TweetMessage_Tweet_successfully_sent_, ToolTipIcon.Info, 3000);
                            }
                        }
                    });

                    return;
                }
            }

            MessageBox.Show(Resources.TaskHelpers_TweetMessage_Unable_to_find_valid_Twitter_account_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            bool hotkeysDisabled = !Program.Settings.DisableHotkeys;

            Program.Settings.DisableHotkeys = hotkeysDisabled;
            Program.HotkeyManager.ToggleHotkeys(hotkeysDisabled);
            Program.MainForm.UpdateToggleHotkeyButton();

            string balloonTipText = hotkeysDisabled ? Resources.TaskHelpers_ToggleHotkeys_Hotkeys_disabled_ : Resources.TaskHelpers_ToggleHotkeys_Hotkeys_enabled_;
            ShowBalloonTip(balloonTipText, ToolTipIcon.Info, 3000);

            return hotkeysDisabled;
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
                if (MessageBox.Show(string.Format(Resources.ScreenRecordForm_StartRecording_does_not_exist, ffmpegPath),
                    "ShareX - " + Resources.ScreenRecordForm_StartRecording_Missing + " ffmpeg.exe", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DialogResult downloadDialogResult = FFmpegDownloader.DownloadFFmpeg(false, DownloaderForm_InstallRequested);

                    if (downloadDialogResult == DialogResult.OK)
                    {
                        Program.DefaultTaskSettings.CaptureSettings.FFmpegOptions.CLIPath = taskSettings.TaskSettingsReference.CaptureSettings.FFmpegOptions.CLIPath =
                            taskSettings.CaptureSettings.FFmpegOptions.CLIPath = Program.DefaultFFmpegFilePath;

#if STEAM || WindowsStore
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
            bool result = FFmpegDownloader.ExtractFFmpeg(filePath, Program.ToolsFolder);

            if (result)
            {
                MessageBox.Show(Resources.ScreenRecordForm_DownloaderForm_InstallRequested_FFmpeg_successfully_downloaded_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(Resources.ScreenRecordForm_DownloaderForm_InstallRequested_Download_of_FFmpeg_failed_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void PlayCaptureSound(TaskSettings taskSettings)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (!string.IsNullOrEmpty(taskSettings.AdvancedSettings.SpeechCapture))
            {
                TextToSpeechAsync(taskSettings.AdvancedSettings.SpeechCapture);
            }
            else if (taskSettings.AdvancedSettings.UseCustomCaptureSound && !string.IsNullOrEmpty(taskSettings.AdvancedSettings.CustomCaptureSoundPath))
            {
                Helpers.PlaySoundAsync(taskSettings.AdvancedSettings.CustomCaptureSoundPath);
            }
            else
            {
                Helpers.PlaySoundAsync(Resources.CaptureSound);
            }
        }

        public static void PlayTaskCompleteSound(TaskSettings taskSettings)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (!string.IsNullOrEmpty(taskSettings.AdvancedSettings.SpeechTaskCompleted))
            {
                TextToSpeechAsync(taskSettings.AdvancedSettings.SpeechTaskCompleted);
            }
            else if (taskSettings.AdvancedSettings.UseCustomTaskCompletedSound && !string.IsNullOrEmpty(taskSettings.AdvancedSettings.CustomTaskCompletedSoundPath))
            {
                Helpers.PlaySoundAsync(taskSettings.AdvancedSettings.CustomTaskCompletedSoundPath);
            }
            else
            {
                Helpers.PlaySoundAsync(Resources.TaskCompletedSound);
            }
        }

        public static void PlayErrorSound(TaskSettings taskSettings)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (taskSettings.AdvancedSettings.UseCustomErrorSound && !string.IsNullOrEmpty(taskSettings.AdvancedSettings.CustomErrorSoundPath))
            {
                Helpers.PlaySoundAsync(taskSettings.AdvancedSettings.CustomErrorSoundPath);
            }
            else
            {
                Helpers.PlaySoundAsync(Resources.ErrorSound);
            }
        }

        public static void TextToSpeechAsync(string text)
        {
            Task.Run(() =>
            {
                using (SpeechSynthesizer speaker = new SpeechSynthesizer())
                {
                    speaker.Speak(text);
                }
            });
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

        public static Image FindMenuIcon<T>(int index)
        {
            T e = Helpers.GetEnumFromIndex<T>(index);

            if (e is AfterCaptureTasks)
            {
                switch ((AfterCaptureTasks)(object)e)
                {
                    case AfterCaptureTasks.ShowQuickTaskMenu:
                        return Resources.ui_menu_blue;
                    case AfterCaptureTasks.ShowAfterCaptureWindow:
                        return Resources.application_text_image;
                    case AfterCaptureTasks.AddImageEffects:
                        return Resources.image_saturation;
                    case AfterCaptureTasks.AnnotateImage:
                        return Resources.image_pencil;
                    case AfterCaptureTasks.CopyImageToClipboard:
                        return Resources.clipboard_paste_image;
                    case AfterCaptureTasks.SendImageToPrinter:
                        return Resources.printer;
                    case AfterCaptureTasks.SaveImageToFile:
                        return Resources.disk;
                    case AfterCaptureTasks.SaveImageToFileWithDialog:
                        return Resources.disk_rename;
                    case AfterCaptureTasks.SaveThumbnailImageToFile:
                        return Resources.disk_small;
                    case AfterCaptureTasks.PerformActions:
                        return Resources.application_terminal;
                    case AfterCaptureTasks.CopyFileToClipboard:
                        return Resources.clipboard_block;
                    case AfterCaptureTasks.CopyFilePathToClipboard:
                        return Resources.clipboard_list;
                    case AfterCaptureTasks.ShowInExplorer:
                        return Resources.folder_stand;
                    case AfterCaptureTasks.ScanQRCode:
                        return Resources.barcode_2d;
                    case AfterCaptureTasks.DoOCR:
                        return Resources.edit_drop_cap;
                    case AfterCaptureTasks.ShowBeforeUploadWindow:
                        return Resources.application__arrow;
                    case AfterCaptureTasks.UploadImageToHost:
                        return Resources.upload_cloud;
                    case AfterCaptureTasks.DeleteFile:
                        return Resources.bin;
                }
            }
            else if (e is AfterUploadTasks)
            {
                switch ((AfterUploadTasks)(object)e)
                {
                    case AfterUploadTasks.ShowAfterUploadWindow:
                        return Resources.application_browser;
                    case AfterUploadTasks.UseURLShortener:
                        return Resources.edit_scale;
                    case AfterUploadTasks.ShareURL:
                        return Resources.globe_share;
                    case AfterUploadTasks.CopyURLToClipboard:
                        return Resources.clipboard_paste_document_text;
                    case AfterUploadTasks.OpenURL:
                        return Resources.globe__arrow;
                    case AfterUploadTasks.ShowQRCode:
                        return Resources.barcode_2d;
                }
            }

            return null;
        }

        public static Image GetHotkeyTypeIcon(HotkeyType hotkeyType)
        {
            switch (hotkeyType)
            {
                default: throw new Exception("Icon missing for hotkey type.");
                case HotkeyType.None: return null;
                // Upload
                case HotkeyType.FileUpload: return Resources.folder_open_document;
                case HotkeyType.FolderUpload: return Resources.folder;
                case HotkeyType.ClipboardUpload: return Resources.clipboard;
                case HotkeyType.ClipboardUploadWithContentViewer: return Resources.clipboard_task;
                case HotkeyType.UploadText: return Resources.notebook;
                case HotkeyType.UploadURL: return Resources.drive;
                case HotkeyType.DragDropUpload: return Resources.inbox;
                case HotkeyType.ShortenURL: return Resources.edit_scale;
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
                case HotkeyType.TextCapture: return Resources.edit_drop_cap;
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
                case HotkeyType.AbortScreenRecording: return Resources.camcorder__exclamation;
                // Tools
                case HotkeyType.ColorPicker: return Resources.color;
                case HotkeyType.ScreenColorPicker: return Resources.pipette;
                case HotkeyType.ImageEditor: return Resources.image_pencil;
                case HotkeyType.ImageEffects: return Resources.image_saturation;
                case HotkeyType.HashCheck: return Resources.application_task;
                case HotkeyType.DNSChanger: return Resources.network_ip;
                case HotkeyType.QRCode: return Resources.barcode_2d;
                case HotkeyType.Ruler: return Resources.ruler_triangle;
                case HotkeyType.IndexFolder: return Resources.folder_tree;
                case HotkeyType.ImageCombiner: return Resources.document_break;
                case HotkeyType.VideoThumbnailer: return Resources.images_stack;
                case HotkeyType.TweetMessage: return Resources.Twitter;
                case HotkeyType.MonitorTest: return Resources.monitor;
                // Other
                case HotkeyType.DisableHotkeys: return Resources.keyboard__minus;
                case HotkeyType.OpenMainWindow: return Resources.application_home;
                case HotkeyType.OpenScreenshotsFolder: return Resources.folder_open_image;
                case HotkeyType.OpenHistory: return Resources.application_blog;
                case HotkeyType.OpenImageHistory: return Resources.application_icon_large;
                case HotkeyType.ToggleActionsToolbar: return Resources.ui_toolbar__arrow;
                case HotkeyType.ExitShareX: return Resources.cross;
            }
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

        public static void AddCustomUploader(string filePath)
        {
            if (Program.UploadersConfig != null)
            {
                try
                {
                    CustomUploaderItem cui = JsonHelpers.DeserializeFromFilePath<CustomUploaderItem>(filePath);

                    if (cui != null)
                    {
                        bool activate = false;

                        if (cui.DestinationType == CustomUploaderDestinationType.None)
                        {
                            DialogResult result = MessageBox.Show($"Would you like to add \"{cui.ToString()}\" custom uploader?",
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
                                (cui.DestinationType.HasFlag(CustomUploaderDestinationType.URLSharingService))) destinations.Add("urls");

                            string destinationsText = string.Join("/", destinations);

                            DialogResult result = MessageBox.Show($"Would you like to set \"{cui.ToString()}\" as the active custom uploader for {destinationsText}?",
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

        public static void DownloadAppVeyorBuild()
        {
            AppVeyorUpdateChecker updateChecker = new AppVeyorUpdateChecker()
            {
                IsBeta = Program.Dev,
                IsPortable = Program.Portable,
                Proxy = HelpersOptions.CurrentProxy.GetWebProxy()
            };
            updateChecker.CheckUpdate();
            UpdateMessageBox.Start(updateChecker);
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
                    TryInverted = true,
                    Options = new DecodingOptions
                    {
                        TryHarder = true
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
    }
}