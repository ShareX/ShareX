#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using ShareX.MediaLib;
using ShareX.ScreenCaptureLib.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class FFmpegOptionsForm : Form
    {
        public ScreencastOptions Options { get; private set; }
        public string DefaultToolsFolder { get; set; }

        private bool settingsLoaded;

        public FFmpegOptionsForm(ScreencastOptions options)
        {
            Options = options;

            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            eiFFmpeg.ObjectType = typeof(FFmpegOptions);
            cboVideoCodec.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegVideoCodec>());
            cboAudioCodec.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegAudioCodec>());
            cbx264Preset.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegPreset>());
            cbGIFStatsMode.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegPaletteGenStatsMode>());
            cbNVENCPreset.Items.AddRange(Helpers.GetEnums<FFmpegNVENCPreset>().Select(x => $"{x} ({x.GetDescription()})").ToArray());
            cbGIFDither.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegPaletteUseDither>());
            cbAMFUsage.Items.AddRange(Helpers.GetEnums<FFmpegAMFUsage>().Select(x => $"{x} ({x.GetDescription()})").ToArray());
            cbAMFQuality.Items.AddRange(Helpers.GetEnums<FFmpegAMFQuality>().Select(x => $"{x} ({x.GetDescription()})").ToArray());
            cbQSVPreset.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegQSVPreset>());
        }

        private async Task SettingsLoad()
        {
            settingsLoaded = false;

            // General

#if STEAM || WindowsStore
            cbOverrideFFmpegPath.Checked = Options.FFmpeg.OverrideCLIPath;
            gbFFmpegExe.Enabled = Options.FFmpeg.OverrideCLIPath;
#else
            cbOverrideFFmpegPath.Visible = false;
#endif

            txtFFmpegPath.Text = Options.FFmpeg.CLIPath;
            txtFFmpegPath.SelectionStart = txtFFmpegPath.TextLength;

            await RefreshSourcesAsync();

#if WindowsStore
            btnInstallHelperDevices.Visible = false;
            btnHelperDevicesHelp.Visible = false;
            lblHelperDevices.Visible = false;
#endif

            cboVideoCodec.SelectedIndex = (int)Options.FFmpeg.VideoCodec;
            cboAudioCodec.SelectedIndex = (int)Options.FFmpeg.AudioCodec;

            tbUserArgs.Text = Options.FFmpeg.UserArgs;

            // x264
            nudx264CRF.SetValue(Options.FFmpeg.x264_CRF);
            cbx264Preset.SelectedIndex = (int)Options.FFmpeg.x264_Preset;

            // VPx
            nudVP8Bitrate.SetValue(Options.FFmpeg.VPx_bitrate);

            // Xvid
            nudXvidQscale.SetValue(Options.FFmpeg.XviD_qscale);

            // NVENC
            nudNVENCBitrate.SetValue(Options.FFmpeg.NVENC_bitrate);
            cbNVENCPreset.SelectedIndex = (int)Options.FFmpeg.NVENC_preset;

            // GIF
            cbGIFStatsMode.SelectedIndex = (int)Options.FFmpeg.GIFStatsMode;
            cbGIFDither.SelectedIndex = (int)Options.FFmpeg.GIFDither;
            nudGIFBayerScale.SetValue(Options.FFmpeg.GIFBayerScale);

            // AMF
            cbAMFUsage.SelectedIndex = (int)Options.FFmpeg.AMF_usage;
            cbAMFQuality.SelectedIndex = (int)Options.FFmpeg.AMF_quality;

            // QuickSync
            nudQSVBitrate.SetValue(Options.FFmpeg.QSV_bitrate);
            cbQSVPreset.SelectedIndex = (int)Options.FFmpeg.QSV_preset;

            // AAC
            tbAACBitrate.Value = Options.FFmpeg.AAC_bitrate / 32;

            // Vorbis
            tbVorbis_qscale.Value = Options.FFmpeg.Vorbis_qscale;

            // MP3
            tbMP3_qscale.Value = FFmpegCLIManager.mp3_max - Options.FFmpeg.MP3_qscale;

#if WindowsStore
            btnTest.Visible = false;
#endif

            cbCustomCommands.Checked = Options.FFmpeg.UseCustomCommands;

            if (Options.FFmpeg.UseCustomCommands)
            {
                txtCommandLinePreview.Text = Options.FFmpeg.CustomCommands;
            }

            settingsLoaded = true;

            UpdateUI();
        }

        private async Task RefreshSourcesAsync(bool selectDevices = false)
        {
            btnRefreshSources.Enabled = false;
            DirectShowDevices devices = null;

            await Task.Run(() =>
            {
                if (File.Exists(Options.FFmpeg.FFmpegPath))
                {
                    using (FFmpegCLIManager ffmpeg = new FFmpegCLIManager(Options.FFmpeg.FFmpegPath))
                    {
                        devices = ffmpeg.GetDirectShowDevices();
                    }
                }
            });

            if (!IsDisposed)
            {
                cboVideoSource.Items.Clear();
                cboVideoSource.Items.Add(FFmpegCLIManager.SourceNone);
                cboVideoSource.Items.Add(FFmpegCLIManager.SourceGDIGrab);
                cboAudioSource.Items.Clear();
                cboAudioSource.Items.Add(FFmpegCLIManager.SourceNone);

                if (devices != null)
                {
                    cboVideoSource.Items.AddRange(devices.VideoDevices.ToArray());
                    cboAudioSource.Items.AddRange(devices.AudioDevices.ToArray());
                }

                if (selectDevices && cboVideoSource.Items.Contains(FFmpegCLIManager.SourceVideoDevice))
                {
                    Options.FFmpeg.VideoSource = FFmpegCLIManager.SourceVideoDevice;
                }

                cboVideoSource.Text = Options.FFmpeg.VideoSource;

                if (selectDevices && cboAudioSource.Items.Contains(FFmpegCLIManager.SourceAudioDevice))
                {
                    Options.FFmpeg.AudioSource = FFmpegCLIManager.SourceAudioDevice;
                }

                cboAudioSource.Text = Options.FFmpeg.AudioSource;

                btnRefreshSources.Enabled = true;
            }
        }

        private void UpdateUI()
        {
            if (settingsLoaded)
            {
                lblAACQuality.Text = string.Format(Resources.FFmpegOptionsForm_UpdateUI_Bitrate___0_k, Options.FFmpeg.AAC_bitrate);
                lblVorbisQuality.Text = Resources.FFmpegOptionsForm_UpdateUI_Quality_ + " " + Options.FFmpeg.Vorbis_qscale;
                lblMP3Quality.Text = Resources.FFmpegOptionsForm_UpdateUI_Quality_ + " " + Options.FFmpeg.MP3_qscale;
                lblOpusQuality.Text = string.Format(Resources.FFmpegOptionsForm_UpdateUI_Bitrate___0_k, Options.FFmpeg.Opus_bitrate);

                bool isValidAudioCodec = true;
                FFmpegVideoCodec videoCodec = (FFmpegVideoCodec)cboVideoCodec.SelectedIndex;

                if (videoCodec == FFmpegVideoCodec.libvpx)
                {
                    FFmpegAudioCodec audioCodec = (FFmpegAudioCodec)cboAudioCodec.SelectedIndex;

                    if (audioCodec != FFmpegAudioCodec.libvorbis && audioCodec != FFmpegAudioCodec.libopus)
                    {
                        isValidAudioCodec = false;
                    }
                }

                pbAudioCodecWarning.Visible = !isValidAudioCodec;
                pbx264PresetWarning.Visible = (FFmpegPreset)cbx264Preset.SelectedIndex > FFmpegPreset.fast;

                if (!Options.FFmpeg.UseCustomCommands)
                {
                    txtCommandLinePreview.Text = Options.GetFFmpegArgs();
                }

                nudGIFBayerScale.Visible = (Options.FFmpeg.GIFDither == FFmpegPaletteUseDither.bayer);
                UpdateFFmpegPathUI();
            }
        }

        private void UpdateFFmpegPathUI()
        {
#if !STEAM
            Color backColor = Color.FromArgb(255, 200, 200);

            try
            {
                if (File.Exists(Options.FFmpeg.FFmpegPath))
                {
                    backColor = Color.FromArgb(200, 255, 200);
                }
            }
            catch
            {
            }

            txtFFmpegPath.BackColor = backColor;
            txtFFmpegPath.ForeColor = SystemColors.ControlText;
#endif
        }

        private async void FFmpegOptionsForm_Load(object sender, EventArgs e)
        {
            await SettingsLoad();
        }

        private void cbOverrideFFmpegPath_CheckedChanged(object sender, EventArgs e)
        {
#if STEAM || WindowsStore
            Options.FFmpeg.OverrideCLIPath = cbOverrideFFmpegPath.Checked;
            gbFFmpegExe.Enabled = Options.FFmpeg.OverrideCLIPath;
#endif
        }

        private void txtFFmpegPath_TextChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.CLIPath = txtFFmpegPath.Text;
            UpdateFFmpegPathUI();
        }

        private async void buttonFFmpegBrowse_Click(object sender, EventArgs e)
        {
            if (Helpers.BrowseFile(Resources.FFmpegOptionsForm_buttonFFmpegBrowse_Click_Browse_for_ffmpeg_exe, txtFFmpegPath, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), true))
            {
                await RefreshSourcesAsync();
            }
        }

        private async void btnRefreshSources_Click(object sender, EventArgs e)
        {
            await RefreshSourcesAsync();
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

        private async void btnInstallHelperDevices_Click(object sender, EventArgs e)
        {
            string filePath = Helpers.GetAbsolutePath("Recorder-devices-setup.exe");

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                bool result = false;

                await Task.Run(() =>
                {
                    try
                    {
                        using (Process process = new Process())
                        {
                            ProcessStartInfo psi = new ProcessStartInfo()
                            {
                                FileName = filePath
                            };

                            process.StartInfo = psi;
                            process.Start();
                            result = process.WaitForExit(1000 * 60 * 5) && process.ExitCode == 0;
                        }
                    }
                    catch { }
                });

                if (result)
                {
                    await RefreshSourcesAsync(true);
                }
            }
            else
            {
                MessageBox.Show("File not exists: \"" + filePath + "\"", "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    case FFmpegVideoCodec.libvpx_vp9:
                        tcFFmpegVideoCodecs.SelectedIndex = 1;
                        break;
                    case FFmpegVideoCodec.libxvid:
                        tcFFmpegVideoCodecs.SelectedIndex = 2;
                        break;
                    case FFmpegVideoCodec.h264_nvenc:
                    case FFmpegVideoCodec.hevc_nvenc:
                        tcFFmpegVideoCodecs.SelectedIndex = 3;
                        break;
                    case FFmpegVideoCodec.gif:
                        tcFFmpegVideoCodecs.SelectedIndex = 4;
                        break;
                    case FFmpegVideoCodec.h264_amf:
                    case FFmpegVideoCodec.hevc_amf:
                        tcFFmpegVideoCodecs.SelectedIndex = 5;
                        break;
                    case FFmpegVideoCodec.h264_qsv:
                    case FFmpegVideoCodec.hevc_qsv:
                        tcFFmpegVideoCodecs.SelectedIndex = 6;
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
                    case FFmpegAudioCodec.libopus:
                        tcFFmpegAudioCodecs.SelectedIndex = 1;
                        break;
                    case FFmpegAudioCodec.libvorbis:
                        tcFFmpegAudioCodecs.SelectedIndex = 2;
                        break;
                    case FFmpegAudioCodec.libmp3lame:
                        tcFFmpegAudioCodecs.SelectedIndex = 3;
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

        private void cbNVENCPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.NVENC_preset = (FFmpegNVENCPreset)cbNVENCPreset.SelectedIndex;
            UpdateUI();
        }

        private void nudNVENCBitrate_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.NVENC_bitrate = (int)nudNVENCBitrate.Value;
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

        private void nudGIFBayerScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.GIFBayerScale = (int)nudGIFBayerScale.Value;
            UpdateUI();
        }

        private void cbAMFUsage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.AMF_usage = (FFmpegAMFUsage)cbAMFUsage.SelectedIndex;
            UpdateUI();
        }

        private void cbAMFQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.AMF_quality = (FFmpegAMFQuality)cbAMFQuality.SelectedIndex;
            UpdateUI();
        }

        private void cbQSVPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.QSV_preset = (FFmpegQSVPreset)cbQSVPreset.SelectedIndex;
            UpdateUI();
        }

        private void nudQSVBitrate_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.QSV_bitrate = (int)nudQSVBitrate.Value;
            UpdateUI();
        }

        private void tbAACBitrate_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.AAC_bitrate = tbAACBitrate.Value * 32;
            UpdateUI();
        }

        private void tbOpusBirate_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.Opus_bitrate = tbOpusBitrate.Value * 32;
            UpdateUI();
        }

        private void tbVorbis_qscale_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.Vorbis_qscale = tbVorbis_qscale.Value;
            UpdateUI();
        }

        private void tbMP3_qscale_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.MP3_qscale = FFmpegCLIManager.mp3_max - tbMP3_qscale.Value;
            UpdateUI();
        }

        private void tbUserArgs_TextChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.UserArgs = tbUserArgs.Text;
            UpdateUI();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            FFmpegGitHubDownloader.DownloadFFmpeg(true, DownloaderForm_InstallRequested);
        }

        private void DownloaderForm_InstallRequested(string filePath)
        {
            bool result = FFmpegGitHubDownloader.ExtractFFmpeg(filePath, DefaultToolsFolder);

            if (result)
            {
                this.InvokeSafe(async () =>
                {
                    txtFFmpegPath.Text = Helpers.GetVariableFolderPath(Path.Combine(DefaultToolsFolder, "ffmpeg.exe"));
                    await RefreshSourcesAsync();
                    if (!IsDisposed) UpdateUI();
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
                        ProcessStartInfo psi = new ProcessStartInfo()
                        {
                            FileName = "cmd.exe",
                            WorkingDirectory = Path.GetDirectoryName(Options.FFmpeg.FFmpegPath),
                            Arguments = $"/k {Path.GetFileName(Options.FFmpeg.FFmpegPath)} {Options.GetFFmpegCommands()}",
                            UseShellExecute = true
                        };

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

        private object eiFFmpeg_ExportRequested()
        {
            return Options.FFmpeg;
        }

        private async void eiFFmpeg_ImportRequested(object obj)
        {
            FFmpegOptions ffmpegOptions = obj as FFmpegOptions;

            if (ffmpegOptions != null)
            {
                string tempFFmpegPath = Options.FFmpeg.CLIPath;
                Options.FFmpeg = ffmpegOptions;
                Options.FFmpeg.CLIPath = tempFFmpegPath;
                await SettingsLoad();
            }
        }
    }
}