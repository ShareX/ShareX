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
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.MediaLib
{
    public partial class VideoThumbnailerForm : Form
    {
        public event Action<string> UploadRequested;

        public string FFmpegPath { get; set; }
        public VideoThumbnailOptions Options { get; set; }

        public VideoThumbnailerForm(string ffmpegPath, VideoThumbnailOptions options)
        {
            FFmpegPath = ffmpegPath;
            Options = options;
            InitializeComponent();
            Icon = ShareXResources.Icon;
            txtMediaPath.Text = Options.LastVideoPath ?? string.Empty;
            pgOptions.SelectedObject = Options;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string mediaPath = txtMediaPath.Text;

            if (File.Exists(mediaPath) && File.Exists(FFmpegPath))
            {
                Options.LastVideoPath = mediaPath;

                VideoThumbnailer thumbnailer = new VideoThumbnailer(mediaPath, FFmpegPath, Options);
                thumbnailer.ProgressChanged += Thumbnailer_ProgressChanged;
                pbProgress.Value = 0;
                pbProgress.Maximum = Options.ScreenshotCount;
                pbProgress.Visible = true;
                btnStart.Visible = false;

                new Thread(() =>
                {
                    List<VideoThumbnailInfo> screenshots = thumbnailer.TakeScreenshots();

                    if (screenshots != null && screenshots.Count > 0)
                    {
                        this.InvokeSafe(() =>
                        {
                            btnStart.Visible = true;
                            pbProgress.Visible = false;

                            if (Options.UploadScreenshots)
                            {
                                screenshots.ForEach(x => OnUploadRequested(x.Filepath));
                            }
                        });
                    }
                }).Start();
            }
        }

        private void Thumbnailer_ProgressChanged(int current, int length)
        {
            this.InvokeSafe(() => pbProgress.Value = current);
        }

        protected void OnUploadRequested(string filepath)
        {
            if (UploadRequested != null)
            {
                UploadRequested(filepath);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            // TODO: Translate
            Helpers.BrowseFile("Browse for media file", txtMediaPath);
        }
    }
}