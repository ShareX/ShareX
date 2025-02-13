#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using ShareX.MediaLib.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.MediaLib
{
    public partial class VideoThumbnailerForm : Form
    {
        public event Action<List<VideoThumbnailInfo>> ThumbnailsTaken;

        public string FFmpegPath { get; set; }
        public VideoThumbnailOptions Options { get; set; }

        public VideoThumbnailerForm(string ffmpegPath, VideoThumbnailOptions options)
        {
            FFmpegPath = ffmpegPath;
            Options = options;

            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            txtMediaPath.Text = Options.LastVideoPath ?? "";
            pgOptions.SelectedObject = Options;
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            string mediaPath = txtMediaPath.Text;

            if (File.Exists(mediaPath) && File.Exists(FFmpegPath))
            {
                Options.LastVideoPath = mediaPath;

                pbProgress.Value = 0;
                pbProgress.Maximum = Options.ThumbnailCount;
                pbProgress.Visible = true;
                btnStart.Visible = false;

                List<VideoThumbnailInfo> thumbnails = null;

                await Task.Run(() =>
                {
                    try
                    {
                        VideoThumbnailer thumbnailer = new VideoThumbnailer(FFmpegPath, Options);
                        thumbnailer.ProgressChanged += Thumbnailer_ProgressChanged;
                        thumbnails = thumbnailer.TakeThumbnails(mediaPath);
                    }
                    catch (Exception ex)
                    {
                        ex.ShowError();
                    }
                });

                if (thumbnails != null)
                {
                    OnThumbnailsTaken(thumbnails);
                }

                btnStart.Visible = true;
                pbProgress.Visible = false;
            }
        }

        private void Thumbnailer_ProgressChanged(int current, int length)
        {
            this.InvokeSafe(() => pbProgress.Value = current);
        }

        protected void OnThumbnailsTaken(List<VideoThumbnailInfo> thumbnails)
        {
            ThumbnailsTaken?.Invoke(thumbnails);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFile(Resources.VideoThumbnailerForm_btnBrowse_Click_Browse_for_media_file, txtMediaPath);
        }
    }
}