#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

using HelpersLib;
using ImageEffectsLib;
using ScreenCaptureLib;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Media;
using System.Windows.Forms;
using UploadersLib;

namespace ShareX
{
    public static class TaskHelpers
    {
        private const int PropertyTagSoftwareUsed = 0x0131;

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
                string thumbnailFilePath = TaskHelpers.CheckFilePath(folder, thumbnailFileName, taskSettings);

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
                MaxTitleLength = taskSettings.AdvancedSettings.NamePatternMaxTitleLength
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
                MaxTitleLength = taskSettings.AdvancedSettings.NamePatternMaxTitleLength
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

        public static void ShowResultNotifications(string notificationText, TaskSettings taskSettings, string filePath)
        {
            if (!taskSettings.AdvancedSettings.DisableNotifications)
            {
                if (!string.IsNullOrEmpty(notificationText))
                {
                    switch (taskSettings.GeneralSettings.PopUpNotification)
                    {
                        case PopUpNotificationType.BalloonTip:
                            if (Program.MainForm.niTray.Visible)
                            {
                                Program.MainForm.niTray.Tag = notificationText;
                                Program.MainForm.niTray.ShowBalloonTip(5000, "ShareX - Task completed", notificationText, ToolTipIcon.Info);
                            }
                            break;
                        case PopUpNotificationType.ToastNotification:
                            NotificationFormConfig toastConfig = new NotificationFormConfig()
                            {
                                Action = taskSettings.AdvancedSettings.ToastWindowClickAction,
                                FilePath = filePath,
                                Text = "ShareX - Task completed\r\n" + notificationText,
                                URL = notificationText
                            };
                            NotificationForm.Show((int)(taskSettings.AdvancedSettings.ToastWindowDuration * 1000), taskSettings.AdvancedSettings.ToastWindowPlacement,
                   taskSettings.AdvancedSettings.ToastWindowSize, toastConfig);
                            break;
                    }
                }

                if (taskSettings.GeneralSettings.PlaySoundAfterUpload)
                {
                    SystemSounds.Exclamation.Play();
                }
            }
        }

        public static Image AnnotateImage(Image img)
        {
            return ImageHelpers.AnnotateImage(img, !Program.IsSandbox, Program.PersonalPath,
                x => Program.MainForm.InvokeSafe(() => ClipboardHelpers.CopyImage(x)),
                x => Program.MainForm.InvokeSafe(() => UploadManager.RunImageTask(x)));
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

        public static bool SelectRegion(out Rectangle rect)
        {
            using (RectangleRegion surface = new RectangleRegion())
            {
                surface.AreaManager.WindowCaptureMode = true;
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
            }

            rect = Rectangle.Empty;
            return false;
        }

        public static PointInfo SelectPointColor(SurfaceOptions surfaceOptions = null)
        {
            using (Image fullscreen = Screenshot.CaptureFullscreen())
            using (RectangleRegion surface = new RectangleRegion(fullscreen))
            {
                if (surfaceOptions != null)
                {
                    surface.Config = new SurfaceOptions
                    {
                        MagnifierPixelCount = surfaceOptions.MagnifierPixelCount,
                        MagnifierPixelSize = surfaceOptions.MagnifierPixelSize
                    };
                }

                surface.OneClickMode = true;
                surface.Prepare();
                surface.ShowDialog();

                if (surface.Result == SurfaceResult.Region)
                {
                    PointInfo pointInfo = new PointInfo();
                    pointInfo.Position = CaptureHelpers.ClientToScreen(surface.OneClickPosition);
                    pointInfo.Color = ((Bitmap)fullscreen).GetPixel(surface.OneClickPosition.X, surface.OneClickPosition.Y);
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
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                g.Clear(Color.Black);

                int width = (int)(16 * (percentage / 100f));

                if (width > 0)
                {
                    using (Brush brush = new LinearGradientBrush(new Rectangle(0, 0, width, 16), Color.DarkBlue, Color.DodgerBlue, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(brush, 0, 0, width, 16);
                    }
                }

                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(percentage.ToString(), new XmlFont("Arial", 7, FontStyle.Bold), Brushes.White, 8, 8, sf);
                }

                g.DrawRectangleProper(Pens.WhiteSmoke, 0, 0, 16, 16);

                return Icon.FromHandle(bmp.GetHicon());
            }
        }

        public static UpdateChecker CheckUpdate()
        {
            UpdateChecker updateChecker = new GitHubUpdateChecker("ShareX", "ShareX");
            updateChecker.CurrentVersion = Program.AssemblyVersion;
            updateChecker.Proxy = ProxyInfo.Current.GetWebProxy();
            updateChecker.CheckUpdate();

            // Fallback if GitHub API fails
            if (updateChecker.UpdateInfo == null || updateChecker.UpdateInfo.Status == UpdateStatus.UpdateCheckFailed)
            {
                updateChecker = new XMLUpdateChecker("http://getsharex.com/Update.xml", "ShareX");
                updateChecker.CurrentVersion = Program.AssemblyVersion;
                updateChecker.Proxy = ProxyInfo.Current.GetWebProxy();
                updateChecker.CheckUpdate();
            }

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
                        filepath = string.Empty;
                        break;
                }
            }

            return filepath;
        }
    }

    public class PointInfo
    {
        public Point Position { get; set; }
        public Color Color { get; set; }
    }
}