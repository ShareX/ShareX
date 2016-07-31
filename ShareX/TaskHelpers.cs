#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Windows.Forms;

namespace ShareX
{
    public static class TaskHelpers
    {
        public static ImageData PrepareImage(Image img, TaskSettings taskSettings)
        {
            ImageData imageData = new ImageData();
            imageData.ImageStream = SaveImage(img, taskSettings.ImageSettings.ImageFormat, taskSettings);
            imageData.ImageFormat = taskSettings.ImageSettings.ImageFormat;

            if (taskSettings.ImageSettings.ImageAutoUseJPEG && taskSettings.ImageSettings.ImageFormat != EImageFormat.JPEG &&
                imageData.ImageStream.Length > taskSettings.ImageSettings.ImageAutoUseJPEGSize * 1000)
            {
                imageData.ImageStream = SaveImage(img, EImageFormat.JPEG, taskSettings);
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
                string thumbnailFilePath = CheckFilePath(folder, thumbnailFileName, taskSettings);

                if (!string.IsNullOrEmpty(thumbnailFilePath))
                {
                    Image thumbImage = null;

                    try
                    {
                        thumbImage = (Image)img.Clone();
                        thumbImage = new Resize
                        {
                            Width = taskSettings.ImageSettings.ThumbnailWidth,
                            Height = taskSettings.ImageSettings.ThumbnailHeight
                        }.Apply(thumbImage);
                        thumbImage = ImageHelpers.FillBackground(thumbImage, Color.White);
                        thumbImage.SaveJPG(thumbnailFilePath, 90);
                        return thumbnailFilePath;
                    }
                    catch (Exception e)
                    {
                        DebugHelper.WriteException(e);
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

        public static MemoryStream SaveImage(Image img, EImageFormat imageFormat, TaskSettings taskSettings)
        {
            return SaveImage(img, imageFormat, taskSettings.ImageSettings.ImageJPEGQuality, taskSettings.ImageSettings.ImageGIFQuality);
        }

        public static MemoryStream SaveImage(Image img, EImageFormat imageFormat, int jpegQuality = 90, GIFQuality gifQuality = GIFQuality.Default)
        {
            MemoryStream stream = new MemoryStream();

            switch (imageFormat)
            {
                case EImageFormat.PNG:
                    img.Save(stream, ImageFormat.Png);
                    break;
                case EImageFormat.JPEG:
                    img = ImageHelpers.FillBackground(img, Color.White);
                    img.SaveJPG(stream, jpegQuality);
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

        public static string GetFilename(TaskSettings taskSettings, string extension = null, Image image = null)
        {
            string filename;

            NameParser nameParser = new NameParser(NameParserType.FileName)
            {
                AutoIncrementNumber = Program.Settings.NameParserAutoIncrementNumber,
                MaxNameLength = taskSettings.AdvancedSettings.NamePatternMaxLength,
                MaxTitleLength = taskSettings.AdvancedSettings.NamePatternMaxTitleLength,
                CustomTimeZone = taskSettings.UploadSettings.UseCustomTimeZone ? taskSettings.UploadSettings.CustomTimeZone : null
            };

            if (image != null)
            {
                nameParser.ImageWidth = image.Width;
                nameParser.ImageHeight = image.Height;

                ImageTag imageTag = image.Tag as ImageTag;

                if (imageTag != null)
                {
                    nameParser.WindowText = imageTag.WindowTitle;
                    nameParser.ProcessName = imageTag.ProcessName;
                }
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

        public static bool ShowAfterCaptureForm(TaskSettings taskSettings, out string fileName, Image img = null)
        {
            fileName = null;

            if (taskSettings.AfterCaptureJob.HasFlag(AfterCaptureTasks.ShowAfterCaptureWindow))
            {
                using (AfterCaptureForm afterCaptureForm = new AfterCaptureForm(img, taskSettings))
                {
                    if (afterCaptureForm.ShowDialog() == DialogResult.Cancel)
                    {
                        if (img != null) img.Dispose();
                        return false;
                    }

                    fileName = afterCaptureForm.FileName;
                }
            }

            return true;
        }

        public static void AnnotateImage(string filePath)
        {
            AnnotateImage(null, filePath);
        }

        public static Image AnnotateImage(Image img, string imgPath)
        {
            return ImageHelpers.AnnotateImage(img, imgPath, !Program.Sandbox, Program.PersonalFolder,
                x => Program.MainForm.InvokeSafe(() => ClipboardHelpers.CopyImage(x)),
                x => Program.MainForm.InvokeSafe(() => UploadManager.UploadImage(x)),
                (x, filePath) => Program.MainForm.InvokeSafe(() => ImageHelpers.SaveImage(x, filePath)),
                (x, filePath) =>
                {
                    string newFilePath = null;
                    Program.MainForm.InvokeSafe(() => newFilePath = ImageHelpers.SaveImageFileDialog(x, filePath));
                    return newFilePath;
                },
                x => Program.MainForm.InvokeSafe(() => PrintImage(x)));
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

        public static Image AddImageEffects(Image img, TaskSettings taskSettings)
        {
            if (taskSettings.ImageSettings.ShowImageEffectsWindowAfterCapture)
            {
                using (ImageEffectsForm imageEffectsForm = new ImageEffectsForm(img, taskSettings.ImageSettings.ImageEffects))
                {
                    if (imageEffectsForm.ShowDialog() == DialogResult.OK)
                    {
                        taskSettings.ImageSettings.ImageEffects = imageEffectsForm.Effects;
                    }
                }
            }

            using (img)
            {
                return ImageEffectManager.ApplyEffects(img, taskSettings.ImageSettings.ImageEffects);
            }
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
            using (Bitmap bmp = new Bitmap(16, 16))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Black);

                int width = (int)(16 * (percentage / 100f));

                if (width > 0)
                {
                    using (Brush brush = new LinearGradientBrush(new Rectangle(0, 0, width, 16), Color.DarkBlue, Color.DodgerBlue, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(brush, 0, 0, width, 16);
                    }
                }

                using (Font font = new Font("Arial", 11, GraphicsUnit.Pixel))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(percentage.ToString(), font, Brushes.White, 8, 8, sf);
                }

                g.DrawRectangleProper(Pens.WhiteSmoke, 0, 0, 16, 16);

                return Icon.FromHandle(bmp.GetHicon());
            }
        }

        public static UpdateChecker CheckUpdate()
        {
            GitHubUpdateChecker updateChecker = new GitHubUpdateChecker("ShareX", "ShareX");
            updateChecker.IsBeta = Program.Beta;
            updateChecker.IsPortable = Program.Portable;
            updateChecker.IncludePreRelease = Program.Settings.CheckPreReleaseUpdates;
            updateChecker.Proxy = HelpersOptions.CurrentProxy.GetWebProxy();
            updateChecker.CheckUpdate();

            /*
            // Fallback if GitHub API fails
            if (updateChecker.Status == UpdateStatus.None || updateChecker.Status == UpdateStatus.UpdateCheckFailed)
            {
                updateChecker = new XMLUpdateChecker(Links.URL_UPDATE, "ShareX");
                updateChecker.IsBeta = Program.IsBeta;
                updateChecker.Proxy = HelpersOptions.CurrentProxy.GetWebProxy();
                updateChecker.CheckUpdate();
            }
            */

            return updateChecker;
        }

        public static string CheckFilePath(string folder, string filename, TaskSettings taskSettings)
        {
            string filepath = Path.Combine(folder, filename);

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

            ScrollingCaptureForm scrollingCaptureForm = new ScrollingCaptureForm(taskSettings.CaptureSettingsReference.ScrollingCaptureOptions, forceSelection);
            scrollingCaptureForm.ImageProcessRequested += img => UploadManager.RunImageTask(img, taskSettings);
            scrollingCaptureForm.Show();
        }

        public static void OpenAutoCapture()
        {
            AutoCaptureForm.Instance.ForceActivate();
        }

        public static void OpenWebpageCapture(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            WebpageCaptureForm webpageCaptureForm = new WebpageCaptureForm(Program.Settings.WebpageCaptureOptions);
            webpageCaptureForm.ImageUploadRequested += img => UploadManager.RunImageTask(img, taskSettings);
            webpageCaptureForm.ImageCopyRequested += img =>
            {
                using (img)
                {
                    ClipboardHelpers.CopyImage(img);
                }
            };
            webpageCaptureForm.Show();
        }

        public static void StartAutoCapture()
        {
            if (!AutoCaptureForm.IsRunning)
            {
                AutoCaptureForm form = AutoCaptureForm.Instance;
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
            HistoryForm historyForm = new HistoryForm(Program.HistoryFilePath, Program.Settings.HistoryMaxItemCount, Program.Settings.HistorySplitterDistance);
            historyForm.SplitterDistanceChanged += splitterDistance => Program.Settings.HistorySplitterDistance = splitterDistance;
            Program.Settings.HistoryWindowState.AutoHandleFormState(historyForm);
            historyForm.Show();
        }

        public static void OpenImageHistory()
        {
            ImageHistoryForm imageHistoryForm = new ImageHistoryForm(Program.HistoryFilePath, Program.Settings.ImageHistoryViewMode,
                Program.Settings.ImageHistoryThumbnailSize, Program.Settings.ImageHistoryMaxItemCount);
            Program.Settings.ImageHistoryWindowState.AutoHandleFormState(imageHistoryForm);
            imageHistoryForm.FormClosed += imageHistoryForm_FormClosed;
            imageHistoryForm.Show();
        }

        private static void imageHistoryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ImageHistoryForm imageHistoryForm = sender as ImageHistoryForm;
            Program.Settings.ImageHistoryViewMode = imageHistoryForm.ViewMode;
            Program.Settings.ImageHistoryThumbnailSize = imageHistoryForm.ThumbnailSize;
            Program.Settings.ImageHistoryMaxItemCount = imageHistoryForm.MaxItemCount;
        }

        public static void OpenColorPicker()
        {
            new ScreenColorPicker().Show();
        }

        public static void OpenScreenColorPicker(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            PointInfo pointInfo = RegionCaptureHelpers.GetPointInfo();

            if (pointInfo != null)
            {
                string text = CodeMenuEntryPixelInfo.Parse(taskSettings.ToolsSettings.ScreenColorPickerFormat, pointInfo.Color, pointInfo.Position);

                ClipboardHelpers.CopyText(text);

                if (Program.MainForm.niTray.Visible)
                {
                    Program.MainForm.niTray.Tag = null;
                    Program.MainForm.niTray.ShowBalloonTip(3000, "ShareX", string.Format(Resources.TaskHelpers_OpenQuickScreenColorPicker_Copied_to_clipboard___0_, text), ToolTipIcon.Info);
                }
            }
        }

        public static void OpenAutomate()
        {
            AutomateForm form = AutomateForm.GetInstance(Program.Settings.AutomateScripts);
            form.ForceActivate();
        }

        public static void StartAutomate()
        {
            AutomateForm form = AutomateForm.GetInstance(Program.Settings.AutomateScripts);

            if (form.Visible)
            {
                if (AutomateForm.IsRunning)
                {
                    form.Stop();
                }
                else
                {
                    form.Start();
                }
            }
            else
            {
                form.ForceActivate();
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

        public static void OpenImageCombiner(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            ImageCombinerForm imageCombinerForm = new ImageCombinerForm(taskSettings.ToolsSettingsReference.ImageCombinerOptions);
            imageCombinerForm.ProcessRequested += img => UploadManager.RunImageTask(img, taskSettings);
            imageCombinerForm.Show();
        }

        public static void OpenVideoThumbnailer(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (!CheckFFmpeg(taskSettings))
            {
                return;
            }

            taskSettings.ToolsSettings.VideoThumbnailOptions.DefaultOutputDirectory = taskSettings.CaptureFolder;
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

        public static void OpenImageEditor(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                if (Clipboard.ContainsImage() &&
                    MessageBox.Show(Resources.TaskHelpers_OpenImageEditor_Your_clipboard_contains_image,
                    Resources.TaskHelpers_OpenImageEditor_Image_editor___How_to_load_image_, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (Image img = ClipboardHelpers.GetImage())
                    {
                        if (img != null)
                        {
                            AnnotateImage(img, null);
                            return;
                        }
                    }
                }

                filePath = ImageHelpers.OpenImageFileDialog();
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                AnnotateImage(filePath);
            }
        }

        public static void OpenImageEffects()
        {
            string filePath = ImageHelpers.OpenImageFileDialog();
            Image img = null;
            if (!string.IsNullOrEmpty(filePath))
            {
                img = ImageHelpers.LoadImage(filePath);
            }
            ImageEffectsForm form = new ImageEffectsForm(img);
            form.EditorMode();
            form.Show();
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
            if (Helpers.IsAdministrator())
            {
                new DNSChangerForm().Show();
            }
            else
            {
                RunShareXAsAdmin("-dnschanger");
            }
        }

        public static void RunShareXAsAdmin(string arguments)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(Application.ExecutablePath);
                psi.Arguments = arguments;
                psi.Verb = "runas";
                Process.Start(psi);
            }
            catch { }
        }

        public static void OpenQRCode()
        {
            new QRCodeForm().Show();
        }

        public static void OpenOCR()
        {
            using (Image img = RegionCaptureHelpers.GetRegionImage())
            {
                if (img != null)
                {
                    using (Stream stream = SaveImage(img, EImageFormat.PNG))
                    {
                        if (stream != null)
                        {
                            using (OCRSpaceForm form = new OCRSpaceForm(stream, "ShareX.png"))
                            {
                                form.Language = Program.Settings.OCRLanguage;
                                form.ShowDialog();
                                Program.Settings.OCRLanguage = form.Language;
                            }
                        }
                    }
                }
            }
        }

        public static void OpenFTPClient()
        {
            if (Program.UploadersConfig != null && Program.UploadersConfig.FTPAccountList != null)
            {
                FTPAccount account = Program.UploadersConfig.FTPAccountList.ReturnIfValidIndex(Program.UploadersConfig.FTPSelectedImage);

                if (account != null)
                {
                    if (account.Protocol == FTPProtocol.FTP || account.Protocol == FTPProtocol.FTPS)
                    {
                        new FTPClientForm(account).Show();
                    }
                    else
                    {
                        MessageBox.Show(Resources.TaskHelpers_OpenFTPClient_FTP_client_only_supports_FTP_or_FTPS_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    return;
                }
            }

            MessageBox.Show(Resources.TaskHelpers_OpenFTPClient_Unable_to_find_valid_FTP_account_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void TweetMessage()
        {
            if (Program.UploadersConfig != null && Program.UploadersConfig.TwitterOAuthInfoList != null)
            {
                OAuthInfo twitterOAuth = Program.UploadersConfig.TwitterOAuthInfoList.ReturnIfValidIndex(Program.UploadersConfig.TwitterSelectedAccount);

                if (twitterOAuth != null && OAuthInfo.CheckOAuth(twitterOAuth))
                {
                    TaskEx.Run(() =>
                    {
                        using (TwitterTweetForm twitter = new TwitterTweetForm(twitterOAuth))
                        {
                            if (twitter.ShowDialog() == DialogResult.OK && twitter.IsTweetSent)
                            {
                                if (Program.MainForm.niTray.Visible)
                                {
                                    Program.MainForm.niTray.Tag = null;
                                    Program.MainForm.niTray.ShowBalloonTip(5000, "ShareX - Twitter", Resources.TaskHelpers_TweetMessage_Tweet_successfully_sent_, ToolTipIcon.Info);
                                }
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
            string ext = Helpers.GetFilenameExtension(filePath);

            if (!string.IsNullOrEmpty(ext))
            {
                if (taskSettings.AdvancedSettings.ImageExtensions.Any(x => ext.Equals(x, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return EDataType.Image;
                }

                if (taskSettings.AdvancedSettings.TextExtensions.Any(x => ext.Equals(x, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return EDataType.Text;
                }
            }

            return EDataType.File;
        }

        public static bool ToggleHotkeys()
        {
            bool hotkeysDisabled = !Program.Settings.DisableHotkeys;

            Program.Settings.DisableHotkeys = hotkeysDisabled;
            Program.HotkeyManager.ToggleHotkeys(hotkeysDisabled);
            Program.MainForm.UpdateToggleHotkeyButton();

            if (Program.MainForm.niTray.Visible)
            {
                Program.MainForm.niTray.Tag = null;
                string balloonTipText = hotkeysDisabled ? Resources.TaskHelpers_ToggleHotkeys_Hotkeys_disabled_ : Resources.TaskHelpers_ToggleHotkeys_Hotkeys_enabled_;
                Program.MainForm.niTray.ShowBalloonTip(3000, "ShareX", balloonTipText, ToolTipIcon.Info);
            }

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
                    if (FFmpegDownloader.DownloadFFmpeg(false, DownloaderForm_InstallRequested) == DialogResult.OK)
                    {
                        Program.DefaultTaskSettings.CaptureSettings.FFmpegOptions.CLIPath = taskSettings.TaskSettingsReference.CaptureSettings.FFmpegOptions.CLIPath =
                            taskSettings.CaptureSettings.FFmpegOptions.CLIPath = Program.DefaultFFmpegFilePath;

#if STEAM
                        Program.DefaultTaskSettings.CaptureSettings.FFmpegOptions.OverrideCLIPath = taskSettings.TaskSettingsReference.CaptureSettings.FFmpegOptions.OverrideCLIPath =
                          taskSettings.CaptureSettings.FFmpegOptions.OverrideCLIPath = true;
#endif
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
            bool result = FFmpegDownloader.ExtractFFmpeg(filePath, Program.DefaultFFmpegFilePath);

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
            TaskEx.Run(() =>
            {
                using (SpeechSynthesizer speaker = new SpeechSynthesizer())
                {
                    speaker.Speak(text);
                }
            });
        }

        public static void OpenUploadersConfigWindow(IUploaderService uploaderService = null)
        {
            if (Program.UploadersConfig == null)
            {
                Program.UploaderSettingsResetEvent.WaitOne();
            }

            bool firstInstance;
            UploadersConfigForm form = UploadersConfigForm.GetFormInstance(Program.UploadersConfig, out firstInstance);

            if (firstInstance)
            {
                form.FormClosed += (sender, e) => Program.UploadersConfigSaveAsync();

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
    }
}