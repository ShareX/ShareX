#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScreenCaptureLib
{
    public partial class FFmpegOptionsForm : Form
    {
        public ScreencastOptions Options = new ScreencastOptions();
        public string DefaultToolsPath;

        public FFmpegOptionsForm(ScreencastOptions options)
        {
            InitializeComponent();

            Icon = ShareXResources.Icon;
            Text = string.Format("{0} - FFmpeg Options", Application.ProductName);

            Options = options;

            if (Options != null)
            {
                SettingsLoad();
                UpdateUI();
            }
        }

        private void SettingsLoad()
        {
            // General
            RefreshSourcesAsync();
            cboVideoCodec.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegVideoCodec>());
            cboVideoCodec.SelectedIndex = (int)Options.FFmpeg.VideoCodec;
            cboAudioCodec.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegAudioCodec>());
            cboAudioCodec.SelectedIndex = (int)Options.FFmpeg.AudioCodec;
            cbShowError.Checked = Options.FFmpeg.ShowError;

            string cli = "ffmpeg.exe";
            if (string.IsNullOrEmpty(Options.FFmpeg.CLIPath) && File.Exists(cli))
            {
                Options.FFmpeg.CLIPath = cli;
            }

            txtFFmpegPath.Text = Options.FFmpeg.CLIPath;

            tbUserArgs.Text = Options.FFmpeg.UserArgs;

            // x264
            nudx264CRF.Value = Options.FFmpeg.x264_CRF.Between((int)nudx264CRF.Minimum, (int)nudx264CRF.Maximum);
            cbPreset.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegPreset>());
            cbPreset.SelectedIndex = (int)Options.FFmpeg.Preset;

            // VPx
            nudVPxCRF.Value = Options.FFmpeg.VPx_CRF.Between((int)nudVPxCRF.Minimum, (int)nudVPxCRF.Maximum);

            // Xvid
            nudQscale.Value = Options.FFmpeg.XviD_qscale.Between((int)nudQscale.Minimum, (int)nudQscale.Maximum);

            // AAC
            tbAACBitrate.Value = Options.FFmpeg.AAC_bitrate / 32;

            // Vorbis
            tbVorbis_qscale.Value = Options.FFmpeg.Vorbis_qscale;

            // MP3
            tbMP3_qscale.Value = FFmpegHelper.libmp3lame_qscale_end - Options.FFmpeg.MP3_qscale; // 0-9 where a lower value is a higher quality

            cbCustomCommands.Checked = Options.FFmpeg.UseCustomCommands;

            txtCommandLinePreview.Text = Options.GetFFmpegCommands();
        }

        private void RefreshSourcesAsync()
        {
            btnRefreshSources.Enabled = false;
            DirectShowDevices devices = null;

            TaskEx.Run(() =>
            {
                using (FFmpegHelper ffmpeg = new FFmpegHelper(Options))
                {
                    devices = ffmpeg.GetDirectShowDevices();
                }
            },
            () =>
            {
                cboVideoSource.Items.Clear();
                cboVideoSource.Items.Add(FFmpegHelper.GDIgrab);
                cboAudioSource.Items.Clear();
                cboAudioSource.Items.Add("None");
                if (devices != null)
                {
                    cboVideoSource.Items.AddRange(devices.VideoDevices.ToArray());
                    cboAudioSource.Items.AddRange(devices.AudioDevices.ToArray());
                }
                cboVideoSource.Text = Options.FFmpeg.VideoSource;
                cboAudioSource.Text = Options.FFmpeg.AudioSource;
                btnRefreshSources.Enabled = true;
            });
        }

        public void UpdateUI()
        {
            lblVorbisQuality.Text = "Quality: " + Options.FFmpeg.Vorbis_qscale;
            lblMP3Quality.Text = "Quality: " + Options.FFmpeg.MP3_qscale;
            lblAACQuality.Text = string.Format("Bitrate: {0}k", Options.FFmpeg.AAC_bitrate);

            if (!Options.FFmpeg.UseCustomCommands)
            {
                txtCommandLinePreview.Text = Options.GetFFmpegCommands();
            }
        }

        public void UpdateExtensions()
        {
            cboExtension.Items.Clear();

            if (Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libx264 || Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libxvid)
            {
                cboExtension.Items.Add("mp4");
            }
            else if (Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libvpx)
            {
                cboExtension.Items.Add("webm");
            }

            cboExtension.Items.Add("mkv");
            cboExtension.Items.Add("avi");

            if (cboExtension.Items.Contains(Options.FFmpeg.Extension))
            {
                cboExtension.Text = Options.FFmpeg.Extension;
            }
            else
            {
                cboExtension.SelectedIndex = 0;
            }
        }

        private void btnRefreshSources_Click(object sender, EventArgs e)
        {
            RefreshSourcesAsync();
        }

        private void cboVideoSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.VideoSource = cboVideoSource.Text;
            UpdateUI();
        }

        private void cboAudioSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.AudioSource = cboAudioSource.Text;
            UpdateUI();
        }

        private void cboVideoCodec_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.VideoCodec = (FFmpegVideoCodec)cboVideoCodec.SelectedIndex;
            UpdateExtensions();
            UpdateUI();
        }

        private void cboAudioCodec_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.AudioCodec = (FFmpegAudioCodec)cboAudioCodec.SelectedIndex;
            UpdateUI();
        }

        private void cbExtension_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.Extension = cboExtension.Text;
            UpdateUI();
        }

        private void nudx264CRF_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.x264_CRF = (int)nudx264CRF.Value;
            UpdateUI();
        }

        private void cbPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.Preset = (FFmpegPreset)cbPreset.SelectedIndex;
            UpdateUI();
        }

        private void nudVPxCRF_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.VPx_CRF = (int)nudVPxCRF.Value;
            UpdateUI();
        }

        private void nudQscale_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.XviD_qscale = (int)nudQscale.Value;
            UpdateUI();
        }

        private void tbAACBitrate_Scroll(object sender, EventArgs e)
        {
            Options.FFmpeg.AAC_bitrate = tbAACBitrate.Value * 32;
            UpdateUI();
        }

        private void tbVorbis_qscale_Scroll(object sender, EventArgs e)
        {
            Options.FFmpeg.Vorbis_qscale = tbVorbis_qscale.Value;
            UpdateUI();
        }

        private void tbMP3_qscale_Scroll(object sender, EventArgs e)
        {
            Options.FFmpeg.MP3_qscale = FFmpegHelper.libmp3lame_qscale_end - tbMP3_qscale.Value; // 0-9 where a lower value is a higher quality
            UpdateUI();
        }

        private void cbShowError_CheckedChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.ShowError = cbShowError.Checked;
        }

        private void tbFFmpegPath_TextChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.CLIPath = txtFFmpegPath.Text;
            txtFFmpegPath.BackColor = File.Exists(txtFFmpegPath.Text) ? Color.FromArgb(200, 255, 200) : Color.FromArgb(255, 200, 200);
        }

        private void buttonFFmpegBrowse_Click(object sender, EventArgs e)
        {
            if (Helpers.BrowseFile("Browse for ffmpeg.exe", txtFFmpegPath, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)))
            {
                RefreshSourcesAsync();
            }
        }

        private void tbUserArgs_TextChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.UserArgs = tbUserArgs.Text;
            UpdateUI();
        }

        private void buttonFFmpegHelp_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL("https://www.ffmpeg.org/ffmpeg.html");
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            FFmpegHelper.DownloadFFmpeg(true, DownloaderForm_InstallRequested);
        }

        private void DownloaderForm_InstallRequested(string filePath)
        {
            string extractPath = DefaultToolsPath ?? "ffmpeg.exe";
            bool result = FFmpegHelper.ExtractFFmpeg(filePath, extractPath);

            if (result)
            {
                this.InvokeSafe(() =>
                {
                    txtFFmpegPath.Text = extractPath;
                    RefreshSourcesAsync();
                    UpdateUI();
                });
                MessageBox.Show("Successfully downloaded FFmpeg.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Download of FFmpeg failed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (File.Exists(Options.FFmpeg.CLIPath))
            {
                try
                {
                    using (Process process = new Process())
                    {
                        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
                        psi.Arguments = "/k ffmpeg " + Options.GetFFmpegCommands();
                        psi.WorkingDirectory = Path.GetDirectoryName(Options.FFmpeg.CLIPath);

                        process.StartInfo = psi;
                        process.Start();
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                }
            }
        }

        private void btnCopyPreview_Click(object sender, EventArgs e)
        {
            ClipboardHelpers.CopyText("ffmpeg " + Options.GetFFmpegCommands());
        }

        private void FFmpegOptionsForm_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Helpers.OpenURL("https://docs.google.com/document/d/1aKLSqsouoeC5Tjf-Z3P880V3rpzvQU0A2Clayn9ElZo/edit?usp=sharing");
        }

        private void cbCustomCommands_CheckedChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.UseCustomCommands = cbCustomCommands.Checked;
            txtCommandLinePreview.ReadOnly = !Options.FFmpeg.UseCustomCommands;

            txtCommandLinePreview.Text = Options.FFmpeg.UseCustomCommands ? Options.FFmpeg.CustomCommands : Options.GetFFmpegCommands();
        }

        private void txtCommandLinePreview_TextChanged(object sender, EventArgs e)
        {
            if (cbCustomCommands.Checked)
            {
                Options.FFmpeg.CustomCommands = txtCommandLinePreview.Text;
            }
        }
    }
}