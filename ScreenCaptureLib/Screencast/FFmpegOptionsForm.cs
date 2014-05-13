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
using ScreenCaptureLib;
using SevenZip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ScreenCaptureLib
{
    public partial class FFmpegOptionsForm : Form
    {
        public ScreencastOptions Options = new ScreencastOptions();
        public string DefaultToolsPath;

        public FFmpegOptionsForm(FFmpegOptions ffMpegOptions)
        {
            Options.FFmpeg = ffMpegOptions;

            InitializeComponent();

            Icon = ShareXResources.Icon;
            Text = string.Format("{0} - FFmpeg Options", Application.ProductName);
        }

        public FFmpegOptionsForm(ScreencastOptions options)
            : this(options.FFmpeg)
        {
            Options = options;

            if (options != null)
            {
                SettingsLoad();
                UpdatePreview();
                UpdateExtensions();
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
            cbExtension.Text = Options.FFmpeg.Extension;

            string cli = "ffmpeg.exe";
            if (string.IsNullOrEmpty(Options.FFmpeg.CLIPath) && File.Exists(cli))
            {
                Options.FFmpeg.CLIPath = cli;
            }

            tbFFmpegPath.Text = Options.FFmpeg.CLIPath;

            tbUserArgs.Text = Options.FFmpeg.UserArgs;

            // x264
            nudx264CRF.Value = Options.FFmpeg.x264CRF.Between((int)nudx264CRF.Minimum, (int)nudx264CRF.Maximum);
            cbPreset.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegPreset>());
            cbPreset.SelectedIndex = (int)Options.FFmpeg.Preset;

            // VPx
            nudVPxCRF.Value = Options.FFmpeg.VPxCRF.Between((int)nudVPxCRF.Minimum, (int)nudVPxCRF.Maximum);

            // XviD
            nudQscale.Value = Options.FFmpeg.qscale.Between((int)nudQscale.Minimum, (int)nudQscale.Maximum);
        }

        private void RefreshSourcesAsync()
        {
            btnRefreshSources.Enabled = false;
            DirectShowDevices devices = null;
            Helpers.AsyncJob(() =>
            {
                using (FFmpegHelper ffmpeg = new FFmpegHelper(Options))
                {
                    devices = ffmpeg.GetDirectShowDevices();
                }
            },
            () =>
            {
                cboVideoSource.Items.Clear();
                cboVideoSource.Items.Add("GDI grab");
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

        public void UpdatePreview()
        {
            tbCommandLinePreview.Text = "ffmpeg " + Options.GetFFmpegArgs();
        }

        public void UpdateExtensions()
        {
            cbExtension.Items.Clear();

            if (Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libx264 || Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libxvid)
            {
                cbExtension.Items.Add("mp4");
            }
            else if (Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libvpx)
            {
                cbExtension.Items.Add("webm");
            }

            cbExtension.Items.Add("avi");
            cbExtension.SelectedIndex = 0;

            tcFFmpegVideoCodecs.SelectedIndex = (int)Options.FFmpeg.VideoCodec;
        }

        private void btnRefreshSources_Click(object sender, EventArgs e)
        {
            RefreshSourcesAsync();
        }

        private void cboVideoSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.VideoSource = cboVideoSource.Text;
            UpdatePreview();
        }

        private void cboAudioSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.AudioSource = cboAudioSource.Text;
            UpdatePreview();
        }

        private void cboVideoCodec_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.VideoCodec = (FFmpegVideoCodec)cboVideoCodec.SelectedIndex;
            UpdateExtensions();
            UpdatePreview();
        }

        private void cboAudioCodec_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.AudioCodec = (FFmpegAudioCodec)cboAudioCodec.SelectedIndex;
            UpdatePreview();
        }

        private void cbExtension_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.Extension = cbExtension.Text;
            UpdatePreview();
        }

        private void nudx264CRF_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.x264CRF = (int)nudx264CRF.Value;
            UpdatePreview();
        }

        private void cbPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.Preset = (FFmpegPreset)cbPreset.SelectedIndex;
            UpdatePreview();
        }

        private void nudVPxCRF_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.VPxCRF = (int)nudVPxCRF.Value;
            UpdatePreview();
        }

        private void nudQscale_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.qscale = (int)nudQscale.Value;
            UpdatePreview();
        }

        private void tbFFmpegPath_TextChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.CLIPath = tbFFmpegPath.Text;
            UpdatePreview();
        }

        private void buttonFFmpegBrowse_Click(object sender, EventArgs e)
        {
            if (Helpers.BrowseFile("Browse for ffmpeg.exe", tbFFmpegPath, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)))
            {
                RefreshSourcesAsync();
                UpdatePreview();
            }
        }

        private void tbUserArgs_TextChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.UserArgs = tbUserArgs.Text;
            UpdatePreview();
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
                    tbFFmpegPath.Text = extractPath;
                    RefreshSourcesAsync();
                    UpdatePreview();
                });
                MessageBox.Show("FFmpeg successfully downloaded.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        psi.Arguments = "/k ffmpeg " + Options.GetFFmpegArgs();
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
            ClipboardHelpers.CopyText(tbCommandLinePreview.Text);
        }
    }
}