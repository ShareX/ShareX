#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class FFmpegOptionsForm : Form
    {
        public ScreenRecordingOptions Options { get; private set; }

        private bool settingsLoaded;

        public FFmpegOptionsForm(ScreenRecordingOptions options)
        {
            Options = options;

            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            eiFFmpeg.ObjectType = typeof(FFmpegOptions);
            cbVideoCodec.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegVideoCodec>());
            cbAudioCodec.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegAudioCodec>());
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

            cbUseCustomFFmpegPath.Checked = Options.FFmpeg.OverrideCLIPath;
            txtFFmpegPath.Enabled = btnFFmpegBrowse.Enabled = Options.FFmpeg.OverrideCLIPath;
            txtFFmpegPath.Text = Options.FFmpeg.CLIPath;
            txtFFmpegPath.SelectionStart = txtFFmpegPath.TextLength;

            await RefreshSourcesAsync();

#if MicrosoftStore
            btnInstallHelperDevices.Visible = false;
            btnHelperDevicesHelp.Visible = false;
            lblHelperDevices.Visible = false;
#endif

            cbVideoCodec.SelectedIndex = (int)Options.FFmpeg.VideoCodec;
            cbAudioCodec.SelectedIndex = (int)Options.FFmpeg.AudioCodec;

            txtUserArgs.Text = Options.FFmpeg.UserArgs;

            // x264
            nudx264CRF.SetValue(Options.FFmpeg.x264_CRF);
            nudx264Bitrate.SetValue(Options.FFmpeg.x264_Bitrate);
            cbx264UseBitrate.Checked = Options.FFmpeg.x264_Use_Bitrate;
            cbx264Preset.SelectedIndex = (int)Options.FFmpeg.x264_Preset;

            // VPx
            nudVP8Bitrate.SetValue(Options.FFmpeg.VPx_Bitrate);

            // Xvid
            nudXvidQscale.SetValue(Options.FFmpeg.XviD_QScale);

            // NVENC
            nudNVENCBitrate.SetValue(Options.FFmpeg.NVENC_Bitrate);
            cbNVENCPreset.SelectedIndex = (int)Options.FFmpeg.NVENC_Preset;

            // GIF
            cbGIFStatsMode.SelectedIndex = (int)Options.FFmpeg.GIFStatsMode;
            cbGIFDither.SelectedIndex = (int)Options.FFmpeg.GIFDither;
            nudGIFBayerScale.SetValue(Options.FFmpeg.GIFBayerScale);

            // AMF
            cbAMFUsage.SelectedIndex = (int)Options.FFmpeg.AMF_Usage;
            cbAMFQuality.SelectedIndex = (int)Options.FFmpeg.AMF_Quality;

            // QuickSync
            nudQSVBitrate.SetValue(Options.FFmpeg.QSV_Bitrate);
            cbQSVPreset.SelectedIndex = (int)Options.FFmpeg.QSV_Preset;

            // AAC
            tbAACBitrate.Value = Options.FFmpeg.AAC_Bitrate / 32;

            // Vorbis
            tbVorbis_qscale.Value = Options.FFmpeg.Vorbis_QScale;

            // MP3
            tbMP3_qscale.Value = FFmpegCLIManager.mp3_max - Options.FFmpeg.MP3_QScale;

#if MicrosoftStore
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
                cbVideoSource.Items.Clear();
                cbVideoSource.Items.Add(FFmpegCLIManager.SourceNone);
                cbVideoSource.Items.Add(FFmpegCLIManager.SourceGDIGrab);
                cbAudioSource.Items.Clear();
                cbAudioSource.Items.Add(FFmpegCLIManager.SourceNone);

                if (devices != null)
                {
                    cbVideoSource.Items.AddRange(devices.VideoDevices.ToArray());
                    cbAudioSource.Items.AddRange(devices.AudioDevices.ToArray());
                }

                if (selectDevices && cbVideoSource.Items.Contains(FFmpegCLIManager.SourceVideoDevice))
                {
                    Options.FFmpeg.VideoSource = FFmpegCLIManager.SourceVideoDevice;
                }
                else if (!cbVideoSource.Items.Contains(Options.FFmpeg.VideoSource))
                {
                    Options.FFmpeg.VideoSource = FFmpegCLIManager.SourceGDIGrab;
                }

                cbVideoSource.Text = Options.FFmpeg.VideoSource;

                if (selectDevices && cbAudioSource.Items.Contains(FFmpegCLIManager.SourceAudioDevice))
                {
                    Options.FFmpeg.AudioSource = FFmpegCLIManager.SourceAudioDevice;
                }
                else if (!cbAudioSource.Items.Contains(Options.FFmpeg.AudioSource))
                {
                    Options.FFmpeg.AudioSource = FFmpegCLIManager.SourceNone;
                }

                cbAudioSource.Text = Options.FFmpeg.AudioSource;

                btnRefreshSources.Enabled = true;
            }
        }

        private void UpdateUI()
        {
            if (settingsLoaded)
            {
                lblx264CRF.Text = Options.FFmpeg.x264_Use_Bitrate ? Resources.Bitrate : Resources.CRF;
                nudx264CRF.Visible = !Options.FFmpeg.x264_Use_Bitrate;
                nudx264Bitrate.Visible = lblx264BitrateK.Visible = Options.FFmpeg.x264_Use_Bitrate;

                lblAACQuality.Text = string.Format(Resources.FFmpegOptionsForm_UpdateUI_Bitrate___0_k, Options.FFmpeg.AAC_Bitrate);
                lblVorbisQuality.Text = Resources.FFmpegOptionsForm_UpdateUI_Quality_ + " " + Options.FFmpeg.Vorbis_QScale;
                lblMP3Quality.Text = Resources.FFmpegOptionsForm_UpdateUI_Quality_ + " " + Options.FFmpeg.MP3_QScale;
                lblOpusQuality.Text = string.Format(Resources.FFmpegOptionsForm_UpdateUI_Bitrate___0_k, Options.FFmpeg.Opus_Bitrate);

                bool isValidAudioCodec = true;
                FFmpegVideoCodec videoCodec = (FFmpegVideoCodec)cbVideoCodec.SelectedIndex;

                if (videoCodec == FFmpegVideoCodec.libvpx)
                {
                    FFmpegAudioCodec audioCodec = (FFmpegAudioCodec)cbAudioCodec.SelectedIndex;

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
            }
        }

        private async void FFmpegOptionsForm_Load(object sender, EventArgs e)
        {
            await SettingsLoad();
        }

        private void cbUseCustomFFmpegPath_CheckedChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.OverrideCLIPath = cbUseCustomFFmpegPath.Checked;
            txtFFmpegPath.Enabled = btnFFmpegBrowse.Enabled = Options.FFmpeg.OverrideCLIPath;
        }

        private void txtFFmpegPath_TextChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.CLIPath = txtFFmpegPath.Text;
        }

        private async void buttonFFmpegBrowse_Click(object sender, EventArgs e)
        {
            if (FileHelpers.BrowseFile(Resources.FFmpegOptionsForm_buttonFFmpegBrowse_Click_Browse_for_ffmpeg_exe, txtFFmpegPath, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), true))
            {
                await RefreshSourcesAsync();
            }
        }

        private async void btnRefreshSources_Click(object sender, EventArgs e)
        {
            await RefreshSourcesAsync();
        }

        private void cbVideoSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.VideoSource = cbVideoSource.Text;
            UpdateUI();
        }

        private void cbAudioSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.AudioSource = cbAudioSource.Text;
            UpdateUI();
        }

        private async void btnInstallHelperDevices_Click(object sender, EventArgs e)
        {
            string filePath = FileHelpers.GetAbsolutePath("Recorder-devices-setup.exe");

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

        private void cbVideoCodec_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.VideoCodec = (FFmpegVideoCodec)cbVideoCodec.SelectedIndex;

            if (cbVideoCodec.SelectedIndex >= 0)
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

        private void cbAudioCodec_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.AudioCodec = (FFmpegAudioCodec)cbAudioCodec.SelectedIndex;

            if (cbAudioCodec.SelectedIndex >= 0)
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

        private void nudx264Bitrate_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.x264_Bitrate = (int)nudx264Bitrate.Value;
            UpdateUI();
        }

        private void cbx264UseBitrate_CheckedChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.x264_Use_Bitrate = cbx264UseBitrate.Checked;
            UpdateUI();
        }

        private void cbPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.x264_Preset = (FFmpegPreset)cbx264Preset.SelectedIndex;
            UpdateUI();
        }

        private void nudVP8Bitrate_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.VPx_Bitrate = (int)nudVP8Bitrate.Value;
            UpdateUI();
        }

        private void nudQscale_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.XviD_QScale = (int)nudXvidQscale.Value;
            UpdateUI();
        }

        private void cbNVENCPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.NVENC_Preset = (FFmpegNVENCPreset)cbNVENCPreset.SelectedIndex;
            UpdateUI();
        }

        private void nudNVENCBitrate_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.NVENC_Bitrate = (int)nudNVENCBitrate.Value;
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
            Options.FFmpeg.AMF_Usage = (FFmpegAMFUsage)cbAMFUsage.SelectedIndex;
            UpdateUI();
        }

        private void cbAMFQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.AMF_Quality = (FFmpegAMFQuality)cbAMFQuality.SelectedIndex;
            UpdateUI();
        }

        private void cbQSVPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.QSV_Preset = (FFmpegQSVPreset)cbQSVPreset.SelectedIndex;
            UpdateUI();
        }

        private void nudQSVBitrate_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.QSV_Bitrate = (int)nudQSVBitrate.Value;
            UpdateUI();
        }

        private void tbAACBitrate_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.AAC_Bitrate = tbAACBitrate.Value * 32;
            UpdateUI();
        }

        private void tbOpusBirate_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.Opus_Bitrate = tbOpusBitrate.Value * 32;
            UpdateUI();
        }

        private void tbVorbis_qscale_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.Vorbis_QScale = tbVorbis_qscale.Value;
            UpdateUI();
        }

        private void tbMP3_qscale_ValueChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.MP3_QScale = FFmpegCLIManager.mp3_max - tbMP3_qscale.Value;
            UpdateUI();
        }

        private void txtUserArgs_TextChanged(object sender, EventArgs e)
        {
            Options.FFmpeg.UserArgs = txtUserArgs.Text;
            UpdateUI();
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
            if (obj is FFmpegOptions ffmpegOptions)
            {
                string tempFFmpegPath = Options.FFmpeg.CLIPath;
                Options.FFmpeg = ffmpegOptions;
                Options.FFmpeg.CLIPath = tempFFmpegPath;
                await SettingsLoad();
            }
        }
    }
}