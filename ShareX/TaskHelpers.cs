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
using ShareX.ImageEffectsLib;
using ShareX.MediaLib;
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
using ShareX.UploadersLib.HelperClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ShareX
{
    public static class TaskHelpers
    {
        public static ImageData PrepareImage(Image img, TaskSettings taskSettings)
        {
            ImageData imageData = new ImageData();
            imageData.ImageFormat = taskSettings.ImageSettings.ImageFormat;

            if (taskSettings.ImageSettings.ImageFormat == EImageFormat.JPEG)
            {
                img = ImageHelpers.FillBackground(img, Color.White);
            }

            imageData.ImageStream = SaveImage(img, taskSettings.ImageSettings.ImageFormat, taskSettings);

            int sizeLimit = taskSettings.ImageSettings.ImageSizeLimit * 1000;

            if (taskSettings.ImageSettings.ImageFormat != taskSettings.ImageSettings.ImageFormat2 && sizeLimit > 0 && imageData.ImageStream.Length > sizeLimit)
            {
                if (taskSettings.ImageSettings.ImageFormat2 == EImageFormat.JPEG)
                {
                    img = ImageHelpers.FillBackground(img, Color.White);
                }

                imageData.ImageStream = SaveImage(img, taskSettings.ImageSettings.ImageFormat2, taskSettings);
                imageData.ImageFormat = taskSettings.ImageSettings.ImageFormat2;
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
            MemoryStream stream = new MemoryStream();

            switch (imageFormat)
            {
                case EImageFormat.PNG:
                    img.Save(stream, ImageFormat.Png);
                    break;
                case EImageFormat.JPEG:
                    img.SaveJPG(stream, taskSettings.ImageSettings.ImageJPEGQuality);
                    break;
                case EImageFormat.GIF:
                    img.SaveGIF(stream, taskSettings.ImageSettings.ImageGIFQuality);
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

        public static string GetFilename(TaskSettings taskSettings, string extension = "")
        {
            NameParser nameParser = new NameParser(NameParserType.FileName)
            {
                AutoIncrementNumber = Program.Settings.NameParserAutoIncrementNumber,
                MaxNameLength = taskSettings.AdvancedSettings.NamePatternMaxLength,
                MaxTitleLength = taskSettings.AdvancedSettings.NamePatternMaxTitleLength,
                CustomTimeZone = taskSettings.UploadSettings.UseCustomTimeZone ? taskSettings.UploadSettings.CustomTimeZone : null
            };

            string filename = nameParser.Parse(taskSettings.UploadSettings.NameFormatPattern);

            if (!string.IsNullOrEmpty(extension))
            {
                filename += "." + extension.TrimStart('.');
            }

            Program.Settings.NameParserAutoIncrementNumber = nameParser.AutoIncrementNumber;

            return filename;
        }

        public static string GetImageFilename(TaskSettings taskSettings, Image image)
        {
            string filename;

            NameParser nameParser = new NameParser(NameParserType.FileName)
            {
                Picture = image,
                AutoIncrementNumber = Program.Settings.NameParserAutoIncrementNumber,
                MaxNameLength = taskSettings.AdvancedSettings.NamePatternMaxLength,
                MaxTitleLength = taskSettings.AdvancedSettings.NamePatternMaxTitleLength,
                CustomTimeZone = taskSettings.UploadSettings.UseCustomTimeZone ? taskSettings.UploadSettings.CustomTimeZone : null
            };

            ImageTag imageTag = image.Tag as ImageTag;

            if (imageTag != null)
            {
                nameParser.WindowText = imageTag.ActiveWindowTitle;
                nameParser.ProcessName = imageTag.ActiveProcessName;
            }

            if (string.IsNullOrEmpty(nameParser.WindowText))
            {
                filename = nameParser.Parse(taskSettings.UploadSettings.NameFormatPattern) + ".bmp";
            }
            else
            {
                filename = nameParser.Parse(taskSettings.UploadSettings.NameFormatPatternActiveWindow) + ".bmp";
            }

            Program.Settings.NameParserAutoIncrementNumber = nameParser.AutoIncrementNumber;

            return filename;
        }

        public static bool ShowAfterCaptureForm(TaskSettings taskSettings, Image img = null)
        {
            if (taskSettings.GeneralSettings.ShowAfterCaptureTasksForm)
            {
                using (AfterCaptureForm afterCaptureForm = new AfterCaptureForm(img, taskSettings))
                {
                    afterCaptureForm.ShowDialog();

                    switch (afterCaptureForm.Result)
                    {
                        case AfterCaptureFormResult.Continue:
                            taskSettings.AfterCaptureJob = afterCaptureForm.AfterCaptureTasks;
                            break;
                        case AfterCaptureFormResult.Copy:
                            taskSettings.AfterCaptureJob = AfterCaptureTasks.CopyImageToClipboard;
                            break;
                        case AfterCaptureFormResult.Cancel:
                            if (img != null) img.Dispose();
                            return false;
                    }
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
            return ImageHelpers.AnnotateImage(img, imgPath, !Program.IsSandbox, Program.PersonalPath,
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

        public static bool SelectRegion(out Rectangle rect, TaskSettings taskSettings)
        {
            using (RectangleRegion surface = new RectangleRegion())
            {
                surface.Config = taskSettings.CaptureSettings.SurfaceOptions;
                surface.Config.ShowTips = false;
                surface.Config.QuickCrop = true;
                surface.Config.ForceWindowCapture = true;
                surface.Prepare();
                surface.ShowDialog();

                if (surface.Result == SurfaceResult.Region)
                {
                    if (surface.AreaManager.IsCurrentAreaValid)
                    {
                        rect = CaptureHelpers.ClientToScreen(surface.AreaManager.CurrentArea);
                        return true;
                    }
                }
                else if (surface.Result == SurfaceResult.Fullscreen)
                {
                    rect = CaptureHelpers.GetScreenBounds();
                    return true;
                }
                else if (surface.Result == SurfaceResult.Monitor)
                {
                    Screen[] screens = Screen.AllScreens;

                    if (surface.MonitorIndex < screens.Length)
                    {
                        Screen screen = screens[surface.MonitorIndex];
                        rect = screen.Bounds;
                        return true;
                    }
                }
                else if (surface.Result == SurfaceResult.ActiveMonitor)
                {
                    rect = CaptureHelpers.GetActiveScreenBounds();
                    return true;
                }
            }

            rect = Rectangle.Empty;
            return false;
        }

        public static PointInfo SelectPointColor()
        {
            using (RectangleRegion surface = new RectangleRegion())
            {
                surface.ScreenColorPickerMode = true;
                surface.Config.UseDimming = false;
                surface.Config.ShowInfo = true;
                surface.Config.ShowMagnifier = true;
                surface.Config.ShowTips = false;
                surface.Prepare();
                surface.ShowDialog();

                if (surface.Result == SurfaceResult.Region)
                {
                    PointInfo pointInfo = new PointInfo();
                    pointInfo.Position = surface.CurrentPosition;
                    pointInfo.Color = surface.CurrentColor;
                    return pointInfo;
                }
            }

            return null;
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
            UpdateChecker updateChecker = new GitHubUpdateChecker("ShareX", "ShareX");
            updateChecker.IsBeta = Program.IsBeta;
            updateChecker.Proxy = HelpersOptions.CurrentProxy.GetWebProxy();
            updateChecker.CheckUpdate();

            // Fallback if GitHub API fails
            if (updateChecker.Status == UpdateStatus.None || updateChecker.Status == UpdateStatus.UpdateCheckFailed)
            {
                updateChecker = new XMLUpdateChecker(Links.URL_UPDATE, "ShareX");
                updateChecker.IsBeta = Program.IsBeta;
                updateChecker.Proxy = HelpersOptions.CurrentProxy.GetWebProxy();
                updateChecker.CheckUpdate();
            }

            return updateChecker;
        }

        public static void CheckDownloadCounts()
        {
            GitHubUpdateChecker updateChecker = new GitHubUpdateChecker("ShareX", "ShareX");
            updateChecker.Proxy = HelpersOptions.CurrentProxy.GetWebProxy();
            string output = updateChecker.GetDownloadCounts();
            Debug.WriteLine(output);
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
                        filepath = string.Empty;
                        break;
                }
            }

            return filepath;
        }

        public static void OpenDropWindow(TaskSettings taskSettings = null)
        {
            DropForm.GetInstance(Program.Settings.DropSize, Program.Settings.DropOffset, Program.Settings.DropAlignment, Program.Settings.DropOpacity,
                Program.Settings.DropHoverOpacity, taskSettings).ShowActivate();
        }

        public static void StartScreenRecording(ScreenRecordOutput outputType, ScreenRecordStartMethod startMethod, TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            ScreenRecordForm form = ScreenRecordForm.Instance;

            if (form.IsRecording)
            {
                form.StartStopRecording();
            }
            else
            {
                form.StartRecording(outputType, taskSettings, startMethod);
            }
        }

        public static void OpenAutoCapture()
        {
            AutoCaptureForm.Instance.ShowActivate();
        }

        public static void OpenWebpageCapture(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            WebpageCaptureForm webpageCaptureForm = new WebpageCaptureForm();
            webpageCaptureForm.OnImageUploadRequested += img => UploadManager.RunImageTask(img, taskSettings);
            webpageCaptureForm.OnImageCopyRequested += img =>
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

        public static void OpenColorPicker()
        {
            new ScreenColorPicker().Show();
        }

        public static void OpenScreenColorPicker(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            PointInfo pointInfo = SelectPointColor();

            if (pointInfo != null)
            {
                string text = taskSettings.AdvancedSettings.ScreenColorPickerFormat;

                text = text.Replace("$r", pointInfo.Color.R.ToString(), StringComparison.InvariantCultureIgnoreCase).
                    Replace("$g", pointInfo.Color.G.ToString(), StringComparison.InvariantCultureIgnoreCase).
                    Replace("$b", pointInfo.Color.B.ToString(), StringComparison.InvariantCultureIgnoreCase).
                    Replace("$hex", ColorHelpers.ColorToHex(pointInfo.Color), StringComparison.InvariantCultureIgnoreCase).
                    Replace("$x", pointInfo.Position.X.ToString(), StringComparison.InvariantCultureIgnoreCase).
                    Replace("$y", pointInfo.Position.Y.ToString(), StringComparison.InvariantCultureIgnoreCase);

                ClipboardHelpers.CopyText(text);

                if (Program.MainForm.niTray.Visible)
                {
                    Program.MainForm.niTray.Tag = null;
                    Program.MainForm.niTray.ShowBalloonTip(3000, "ShareX", string.Format(Resources.TaskHelpers_OpenQuickScreenColorPicker_Copied_to_clipboard___0_, text), ToolTipIcon.Info);
                }
            }
        }

        public static void OpenRuler()
        {
            using (RectangleRegion surface = new RectangleRegion())
            {
                surface.RulerMode = true;
                surface.Config.ShowTips = false;
                surface.Config.QuickCrop = false;
                surface.Config.ShowInfo = true;
                surface.AreaManager.MinimumSize = 3;
                surface.Prepare();
                surface.ShowDialog();
            }
        }

        public static void OpenAutomate()
        {
            AutomateForm form = AutomateForm.GetInstance(Program.Settings.AutomateScripts);
            form.ShowActivate();
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
                form.ShowActivate();
            }
        }

        public static void OpenHashCheck()
        {
            new HashCheckForm().Show();
        }

        public static void OpenIndexFolder()
        {
            UploadManager.IndexFolder();
        }

        public static void OpenVideoThumbnailer(TaskSettings taskSettings = null)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            Program.Settings.VideoThumbnailOptions.DefaultOutputDirectory = taskSettings.CaptureFolder;
            VideoThumbnailerForm thumbnailerForm = new VideoThumbnailerForm(taskSettings.CaptureSettings.FFmpegOptions.CLIPath, Program.Settings.VideoThumbnailOptions);
            thumbnailerForm.UploadRequested += filepath => UploadManager.UploadFile(filepath, taskSettings);
            thumbnailerForm.Show();
        }

        public static void OpenImageEditor(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
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
            bool result = !Program.Settings.DisableHotkeys;
            Program.Settings.DisableHotkeys = result;

            Program.MainForm.UpdateToggleHotkeyButton();

            if (Program.MainForm.niTray.Visible)
            {
                Program.MainForm.niTray.Tag = null;
                // TODO: Translate
                string balloonTipText = result ? "Hotkeys disabled." : "Hotkeys enabled.";
                Program.MainForm.niTray.ShowBalloonTip(3000, "ShareX", balloonTipText, ToolTipIcon.Info);
            }

            return result;
        }
    }
}