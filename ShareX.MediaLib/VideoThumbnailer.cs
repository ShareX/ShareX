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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ShareX.MediaLib
{
    public class VideoThumbnailer
    {
        public delegate void ProgressChangedEventHandler(int current, int length);
        public event ProgressChangedEventHandler ProgressChanged;

        public string MediaPath { get; private set; }
        public string FFmpegPath { get; private set; }
        public VideoThumbnailOptions Options { get; private set; }
        public VideoInfo VideoInfo { get; private set; }

        public VideoThumbnailer(string mediaPath, string ffmpegPath, VideoThumbnailOptions options)
        {
            MediaPath = mediaPath;
            FFmpegPath = ffmpegPath;
            Options = options;

            using (FFmpegCLIManager ffmpegCLI = new FFmpegCLIManager(FFmpegPath))
            {
                VideoInfo = ffmpegCLI.GetVideoInfo(MediaPath);
            }
        }

        public List<VideoThumbnailInfo> TakeScreenshots()
        {
            List<VideoThumbnailInfo> tempScreenshots = new List<VideoThumbnailInfo>();

            for (int i = 0; i < Options.ScreenshotCount; i++)
            {
                string mediaFileName = Path.GetFileNameWithoutExtension(MediaPath);

                int timeSliceElapsed;

                if (Options.RandomFrame)
                {
                    timeSliceElapsed = GetRandomTimeSlice(i);
                }
                else
                {
                    timeSliceElapsed = GetTimeSlice(Options.ScreenshotCount) * (i + 1);
                }

                string filename = string.Format("{0}-{1}.{2}", mediaFileName, timeSliceElapsed, Options.ImageFormat.GetDescription());
                string tempScreenshotPath = Path.Combine(GetOutputDirectory(), filename);

                using (Process p = new Process())
                {
                    ProcessStartInfo psi = new ProcessStartInfo(FFmpegPath);
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    psi.Arguments = string.Format("-ss {0} -i \"{1}\" -f image2 -vframes 1 -y \"{2}\"", timeSliceElapsed, MediaPath, tempScreenshotPath);
                    p.StartInfo = psi;
                    p.Start();
                    p.WaitForExit(1000 * 30);
                }

                if (File.Exists(tempScreenshotPath))
                {
                    VideoThumbnailInfo screenshotInfo = new VideoThumbnailInfo(tempScreenshotPath)
                    {
                        Timestamp = TimeSpan.FromSeconds(timeSliceElapsed)
                    };

                    tempScreenshots.Add(screenshotInfo);
                }

                OnProgressChanged(i + 1, Options.ScreenshotCount);
            }

            return Finish(tempScreenshots);
        }

        private List<VideoThumbnailInfo> Finish(List<VideoThumbnailInfo> tempScreenshots)
        {
            List<VideoThumbnailInfo> screenshots = new List<VideoThumbnailInfo>();

            if (tempScreenshots != null && tempScreenshots.Count > 0)
            {
                if (Options.CombineScreenshots)
                {
                    using (Image img = CombineScreenshots(tempScreenshots))
                    {
                        string tempFilepath = Path.Combine(GetOutputDirectory(), Path.GetFileNameWithoutExtension(MediaPath) + "_Thumbnail." + Options.ImageFormat.GetDescription());
                        ImageHelpers.SaveImage(img, tempFilepath);
                        screenshots.Add(new VideoThumbnailInfo(tempFilepath));
                    }

                    if (!Options.KeepScreenshots)
                    {
                        tempScreenshots.ForEach(x => File.Delete(x.Filepath));
                    }
                }
                else
                {
                    screenshots.AddRange(tempScreenshots);
                }

                if (Options.OpenDirectory && screenshots.Count > 0)
                {
                    Helpers.OpenFolderWithFile(screenshots[0].Filepath);
                }
            }

            return screenshots;
        }

        protected void OnProgressChanged(int current, int length)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(current, length);
            }
        }

        private string GetOutputDirectory()
        {
            switch (Options.OutputLocation)
            {
                default:
                case ThumbnailLocationType.ParentFolder:
                    return Path.GetDirectoryName(MediaPath);
                case ThumbnailLocationType.CustomFolder:
                    return Options.CustomOutputDirectory;
                case ThumbnailLocationType.DefaultFolder: // TODO
                    return "";
            }
        }

        private int GetTimeSlice(int count)
        {
            return (int)(VideoInfo.Duration.TotalSeconds / count);
        }

        private int GetRandomTimeSlice(int start)
        {
            List<int> mediaSeekTimes = new List<int>();

            for (int i = 1; i < Options.ScreenshotCount + 2; i++)
            {
                mediaSeekTimes.Add(GetTimeSlice(Options.ScreenshotCount + 2) * i);
            }

            Random random = new Random();
            return (int)(random.NextDouble() * (mediaSeekTimes[start + 1] - mediaSeekTimes[start]) + mediaSeekTimes[start]);
        }

        private Image CombineScreenshots(List<VideoThumbnailInfo> screenshots)
        {
            List<Image> images = new List<Image>();
            Image finalImage = null;

            try
            {
                string infoString = "";
                int infoStringHeight = 0;

                if (Options.AddVideoInfo)
                {
                    infoString = VideoInfo.ToString();

                    using (Font font = new Font("Arial", 14))
                    {
                        infoStringHeight = Helpers.MeasureText(infoString, font).Height;
                    }
                }

                foreach (VideoThumbnailInfo screenshot in screenshots)
                {
                    Image img = Image.FromFile(screenshot.Filepath);

                    if (Options.MaxThumbnailWidth > 0 && img.Width > Options.MaxThumbnailWidth)
                    {
                        int maxThumbnailHeight = (int)((float)Options.MaxThumbnailWidth / img.Width * img.Height);
                        img = ImageHelpers.ResizeImage(img, Options.MaxThumbnailWidth, maxThumbnailHeight);
                    }

                    images.Add(img);
                }

                int columnCount = Options.ColumnCount;

                int thumbWidth = images[0].Width;

                int width = Options.Padding * 2 +
                            thumbWidth * columnCount +
                            (columnCount - 1) * Options.Spacing;

                int rowCount = (int)Math.Ceiling(images.Count / (float)columnCount);

                int thumbHeight = images[0].Height;

                int height = Options.Padding * 3 +
                             infoStringHeight +
                             thumbHeight * rowCount +
                             (rowCount - 1) * Options.Spacing;

                finalImage = new Bitmap(width, height);

                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    g.Clear(Color.WhiteSmoke);

                    if (!string.IsNullOrEmpty(infoString))
                    {
                        using (Font font = new Font("Arial", 14))
                        {
                            g.DrawString(infoString, font, Brushes.Black, Options.Padding, Options.Padding);
                        }
                    }

                    int i = 0;
                    int offsetY = Options.Padding * 2 + infoStringHeight;

                    for (int y = 0; y < rowCount; y++)
                    {
                        int offsetX = Options.Padding;

                        for (int x = 0; x < columnCount; x++)
                        {
                            if (Options.DrawShadow)
                            {
                                int shadowOffset = 3;

                                using (Brush shadowBrush = new SolidBrush(Color.FromArgb(75, Color.Black)))
                                {
                                    g.FillRectangle(shadowBrush, offsetX + shadowOffset, offsetY + shadowOffset, thumbWidth, thumbHeight);
                                }
                            }

                            g.DrawImage(images[i], offsetX, offsetY, thumbWidth, thumbHeight);

                            if (Options.AddTimestamp)
                            {
                                int timestampOffset = 10;

                                using (Font font = new Font("Arial", 12))
                                {
                                    ImageHelpers.DrawTextWithShadow(g, screenshots[i].Timestamp.ToString(),
                                        new Point(offsetX + timestampOffset, offsetY + timestampOffset), font, Color.White, Color.Black);
                                }
                            }

                            i++;

                            if (i >= images.Count)
                            {
                                return finalImage;
                            }

                            offsetX += thumbWidth + Options.Spacing;
                        }

                        offsetY += thumbHeight + Options.Spacing;
                    }
                }

                return finalImage;
            }
            catch
            {
                if (finalImage != null)
                {
                    finalImage.Dispose();
                }

                throw;
            }
            finally
            {
                foreach (Image image in images)
                {
                    if (image != null)
                    {
                        image.Dispose();
                    }
                }
            }
        }
    }
}