#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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
using ShareX.ScreenCaptureLib.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class FFmpegOptionsForm : BaseForm
    {
        public ScreencastOptions Options { get; private set; }
        public string DefaultToolsPath { get; set; }

        private bool settingsLoaded;

        public FFmpegOptionsForm(ScreencastOptions options)
        {
            InitializeComponent();
            Options = options;

            eiFFmpeg.ObjectType = typeof(FFmpegOptions);
            cboVideoCodec.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegVideoCodec>());
            cboAudioCodec.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegAudioCodec>());
            cbx264Preset.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegPreset>());
            cbGIFStatsMode.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegPaletteGenStatsMode>());
            cbGIFDither.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegPaletteUseDither>());

            SettingsLoad();
        }

        private void SettingsLoad()
        {
            settingsLoaded = false;

            // General

#if STEAM
            cbOverrideFFmpegPath.Checked = Options.FFmpeg.OverrideCLIPath;
            gbFFmpegExe.Enabled = Options.FFmpeg.OverrideCLIPath;
#else
            cbOverrideFFmpegPath.Visible = false;
#endif

            txtFFmpegPath.Text = Options.FFmpeg.CLIPath;
            txtFFmpegPath.SelectionStart = txtFFmpegPath.TextLength;

            RefreshSourcesAsync();

            cboVideoCodec.SelectedIndex = (int)Options.FFmpeg.VideoCodec;
            cboAudioCodec.SelectedIndex = (int)Options.FFmpeg.AudioCodec;

            tbUserArgs.Text = Options.FFmpeg.UserArgs;

            // x264
            nudx264CRF.Value = Options.FFmpeg.x264_CRF.Between((int)nudx264CRF.Minimum, (int)nudx264CRF.Maximum);
            cbx264Preset.SelectedIndex = (int)Options.FFmpeg.x264_Preset;

            // VPx
            nudVP8Bitrate.Value = Options.FFmpeg.VPx_bitrate.Between((int)nudVP8Bitrate.Minimum, (int)nudVP8Bitrate.Maximum);

            // Xvid
            nudXvidQscale.Value = Options.FFmpeg.XviD_qscale.Between((int)nudXvidQscale.Minimum, (int)nudXvidQscale.Maximum);

            // GIF
            cbGIFStatsMode.SelectedIndex = (int)Options.FFmpeg.GIFStatsMode;
            cbGIFDither.SelectedIndex = (int)Options.FFmpeg.GIFDither;

            // AAC
            tbAACBitrate.Value = Options.FFmpeg.AAC_bitrate / 32;

            // Vorbis
            tbVorbis_qscale.Value = Options.FFmpeg.Vorbis_qscale;

            // MP3
            tbMP3_qscale.Value = FFmpegHelper.libmp3lame_qscale_end - Options.FFmpeg.MP3_qscale;

            cbCustomCommands.Checked = Options.FFmpeg.UseCustomCommands;

            if (Options.FFmpeg.UseCustomCommands)
            {
                txtCommandLinePreview.Text = Options.FFmpeg.CustomCommands;
            }

            settingsLoaded = true;

            UpdateUI();
        }

        private void RefreshSourcesAsync(bool selectDevices = false)
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
                cboVideoSource.Items.Add(FFmpegHelper.SourceNone);
                cboVideoSource.Items.Add(FFmpegHelper.SourceGDIGrab);
                cboAudioSource.Items.Clear();
                cboAudioSource.Items.Add(FFmpegHelper.SourceNone);

                if (devices != null)
                {
                    cboVideoSource.Items.AddRange(devices.VideoDevices.ToArray());
                    cboAudioSource.Items.AddRange(devices.AudioDevices.ToArray());
                }

                if (selectDevices && cboVideoSource.Items.Contains(FFmpegHelper.SourceVideoDevice))
                {
                    Options.FFmpeg.VideoSource = FFmpegHelper.SourceVideoDevice;
                }

                cboVideoSource.Text = Options.FFmpeg.VideoSource;

                if (selectDevices && cboAudioSource.Items.Contains(FFmpegHelper.SourceAudioDevice))
                {
                    Options.FFmpeg.AudioSource = FFmpegHelper.SourceAudioDevice;
                }

                cboAudioSource.Text = Options.FFmpeg.AudioSource;

                btnRefreshSources.Enabled = true;
            });
        }

        private void UpdateUI()
        {
            if (settingsLoaded)
            {
                lblAACQuality.Text = string.Format(Resources.FFmpegOptionsForm_UpdateUI_Bitrate___0_k, Options.FFmpeg.AAC_bitrate);
                lblVorbisQuality.Text = Resources.FFmpegOptionsForm_UpdateUI_Quality_ + " " + Options.FFmpeg.Vorbis_qscale;
                lblMP3Quality.Text = Resources.FFmpegOptionsForm_UpdateUI_Quality_ + " " + Options.FFmpeg.MP3_qscale;

                if (!Options.FFmpeg.UseCustomCommands)
                {
                    txtCommandLinePreview.Text = Options.GetFFmpegArgs();
                }
            }
        }

        private void cbOverrideFFmpegPath_CheckedChanged(object sender, EventArgs e)
        {
#if STEAM
            Options.FFmpeg.OverrideCLIPath = cbOverrideFFmpegPath.Checked;
            gbFFmpegExe.Enabled = Options.FFmpeg.OverrideCLIPath;
#endif
        }

        private void txtFFmpegPath_TextChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.CLIPath = txtFFmpegPath.Text;
        }

        private void buttonFFmpegBrowse_Click(object sender, EventArgs e)
        {
            if (Helpers.BrowseFile(Resources.FFmpegOptionsForm_buttonFFmpegBrowse_Click_Browse_for_ffmpeg_exe, txtFFmpegPath, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)))
            {
                RefreshSourcesAsync();
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

        private void btnInstallHelperDevices_Click(object sender, EventArgs e)
        {
            string filepath = Helpers.GetAbsolutePath(FFmpegHelper.DeviceSetupPath);

            if (!string.IsNullOrEmpty(filepath) && File.Exists(filepath))
            {
                TaskEx.Run(() =>
                {
                    try
                    {
                        Process process = Process.Start(filepath);

                        if (process.WaitForExit(1000 * 60 * 5) && process.ExitCode == 0)
                        {
                            this.InvokeSafe(() =>
                            {
                                RefreshSourcesAsync(true);
                            });
                        }
                    }
                    catch { }
                });
            }
            else
            {
                MessageBox.Show("File not exists: \"" + filepath + "\"", "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHelperDevicesHelp_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://github.com/rdp/screen-capture-recorder-to-video-windows-free");
        }

        private void cboVideoCodec_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.VideoCodec = (FFmpegVideoCodec)cboVideoCodec.SelectedIndex;

            if (cboVideoCodec.SelectedIndex >= 0)
            {
                switch (Options.FFmpeg.VideoCodec)
                {
                    default:
                    case FFmpegVideoCodec.libx264:
                    case FFmpegVideoCodec.libx265:
                        tcFFmpegVideoCodecs.SelectedIndex = 0;
                        break;
                    case FFmpegVideoCodec.libvpx:
                        tcFFmpegVideoCodecs.SelectedIndex = 1;
                        break;
                    case FFmpegVideoCodec.libxvid:
                        tcFFmpegVideoCodecs.SelectedIndex = 2;
                        break;
                    case FFmpegVideoCodec.gif:
                        tcFFmpegVideoCodecs.SelectedIndex = 3;
                        break;
                }
            }

            UpdateUI();
        }

        private void cboAudioCodec_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.AudioCodec = (FFmpegAudioCodec)cboAudioCodec.SelectedIndex;

            if (cboAudioCodec.SelectedIndex >= 0)
            {
                switch (Options.FFmpeg.AudioCodec)
                {
                    default:
                    case FFmpegAudioCodec.libvoaacenc:
                        tcFFmpegAudioCodecs.SelectedIndex = 0;
                        break;
                    case FFmpegAudioCodec.libvorbis:
                        tcFFmpegAudioCodecs.SelectedIndex = 1;
                        break;
                    case FFmpegAudioCodec.libmp3lame:
                        tcFFmpegAudioCodecs.SelectedIndex = 2;
                        break;
                }
            }

            UpdateUI();
        }

        private void nudx264CRF_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.x264_CRF = (int)nudx264CRF.Value;
            UpdateUI();
        }

        private void cbPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.x264_Preset = (FFmpegPreset)cbx264Preset.SelectedIndex;
            UpdateUI();
        }

        private void nudVP8Bitrate_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.VPx_bitrate = (int)nudVP8Bitrate.Value;
            UpdateUI();
        }

        private void nudQscale_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.XviD_qscale = (int)nudXvidQscale.Value;
            UpdateUI();
        }

        private void cbGIFStatsMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.GIFStatsMode = (FFmpegPaletteGenStatsMode)cbGIFStatsMode.SelectedIndex;
            UpdateUI();
        }

        private void cbGIFDither_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.GIFDither = (FFmpegPaletteUseDither)cbGIFDither.SelectedIndex;
            UpdateUI();
        }

        private void tbAACBitrate_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.AAC_bitrate = tbAACBitrate.Value * 32;
            UpdateUI();
        }

        private void tbVorbis_qscale_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.Vorbis_qscale = tbVorbis_qscale.Value;
            UpdateUI();
        }

        private void tbMP3_qscale_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.MP3_qscale = FFmpegHelper.libmp3lame_qscale_end - tbMP3_qscale.Value;
            UpdateUI();
        }

        private void tbUserArgs_TextChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.UserArgs = tbUserArgs.Text;
            UpdateUI();
        }

        private void buttonFFmpegHelp_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://github.com/ShareX/ShareX/wiki/FFmpeg-options#additional-commands");
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            FFmpegDownloader.DownloadFFmpeg(true, DownloaderForm_InstallRequested);
        }

        private void DownloaderForm_InstallRequested(string filePath)
        {
            string extractPath = DefaultToolsPath ?? "ffmpeg.exe";
            bool result = FFmpegDownloader.ExtractFFmpeg(filePath, extractPath);

            if (result)
            {
                this.InvokeSafe(() =>
                {
                    txtFFmpegPath.Text = extractPath;
                    RefreshSourcesAsync();
                    UpdateUI();
                });

                MessageBox.Show(Resources.FFmpegOptionsForm_DownloaderForm_InstallRequested_Successfully_downloaded_FFmpeg_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(Resources.FFmpegOptionsForm_DownloaderForm_InstallRequested_Download_of_FFmpeg_failed_, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (File.Exists(Options.FFmpeg.FFmpegPath))
            {
                try
                {
                    using (Process process = new Process())
                    {
                        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
                        psi.Arguments = $"/k {Path.GetFileName(Options.FFmpeg.FFmpegPath)} {Options.GetFFmpegCommands()}";
                        psi.WorkingDirectory = Path.GetDirectoryName(Options.FFmpeg.FFmpegPath);

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
            ClipboardHelpers.CopyText($"{Path.GetFileName(Options.FFmpeg.FFmpegPath)} {Options.GetFFmpegCommands()}");
        }

        private void cbCustomCommands_CheckedChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.UseCustomCommands = cbCustomCommands.Checked;
            txtCommandLinePreview.ReadOnly = !Options.FFmpeg.UseCustomCommands;

            if (settingsLoaded)
            {
                if (Options.FFmpeg.UseCustomCommands)
                {
                    txtCommandLinePreview.Text = Options.GetFFmpegArgs(true);
                }
                else
                {
                    txtCommandLinePreview.Text = Options.GetFFmpegArgs();
                }
            }
        }

        private void txtCommandLinePreview_TextChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.CustomCommands = txtCommandLinePreview.Text;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://github.com/ShareX/ShareX/wiki/FFmpeg-options");
        }

        private object eiFFmpeg_ExportRequested()
        {
            return Options.FFmpeg;
        }

        private void eiFFmpeg_ImportRequested(object obj)
        {
            FFmpegOptions ffmpegOptions = obj as FFmpegOptions;

            if (ffmpegOptions != null)
            {
                string tempFFmpegPath = Options.FFmpeg.CLIPath;
                Options.FFmpeg = ffmpegOptions;
                Options.FFmpeg.CLIPath = tempFFmpegPath;
                SettingsLoad();
            }
        }
    }
}