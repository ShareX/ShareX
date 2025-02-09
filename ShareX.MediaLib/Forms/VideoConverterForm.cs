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
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.MediaLib
{
    public partial class VideoConverterForm : Form
    {
        public string FFmpegFilePath { get; private set; }
        public VideoConverterOptions Options { get; private set; }

        private bool formClosing, formReady, encoding;
        private FFmpegCLIManager ffmpeg;

        public VideoConverterForm(string ffmpegFilePath, VideoConverterOptions options)
        {
            FFmpegFilePath = ffmpegFilePath;
            Options = options;

            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            UpdateOptions();

            txtInputFilePath.Text = Options.InputFilePath;
            txtOutputFolder.Text = Options.OutputFolderPath;
            txtOutputFileName.Text = Options.OutputFileName;
            cbVideoEncoder.Items.AddRange(Helpers.GetEnumDescriptions<ConverterVideoCodecs>());
            cbVideoEncoder.SelectedIndex = (int)Options.VideoCodec;
            cbVideoQualityUseBitrate.Checked = Options.VideoQualityUseBitrate;
            tbVideoQuality.SetValue(tbVideoQuality.Minimum + tbVideoQuality.Maximum - Options.VideoQuality);
            nudVideoQualityBitrate.SetValue(Options.VideoQualityBitrate);

            cbAutoOpenFolder.Checked = Options.AutoOpenFolder;

            cbUseCustomArguments.Checked = Options.UseCustomArguments;

            if (Options.UseCustomArguments)
            {
                txtArguments.Text = Options.CustomArguments;
            }

            formReady = true;
        }

        public VideoConverterForm(string inputFilePath, string ffmpegFilePath, VideoConverterOptions options) : this(ffmpegFilePath, options)
        {
            txtInputFilePath.Text = inputFilePath;
            txtOutputFolder.Text = Path.GetDirectoryName(inputFilePath);
            txtOutputFileName.Text = Path.GetFileNameWithoutExtension(inputFilePath) + "-output";
        }

        private void UpdateOptions()
        {
            if (formReady)
            {
                Options.InputFilePath = txtInputFilePath.Text;
                Options.OutputFolderPath = txtOutputFolder.Text;
                Options.OutputFileName = txtOutputFileName.Text;
                Options.VideoCodec = (ConverterVideoCodecs)cbVideoEncoder.SelectedIndex;
                Options.VideoQualityUseBitrate = cbVideoQualityUseBitrate.Checked;
                Options.VideoQualityBitrate = (int)nudVideoQualityBitrate.Value;
                Options.UseCustomArguments = cbUseCustomArguments.Checked;

                if (Options.UseCustomArguments)
                {
                    Options.CustomArguments = txtArguments.Text;
                }
            }

            switch (Options.VideoCodec)
            {
                case ConverterVideoCodecs.x264:
                case ConverterVideoCodecs.x265:
                case ConverterVideoCodecs.vp8:
                case ConverterVideoCodecs.vp9:
                case ConverterVideoCodecs.av1:
                case ConverterVideoCodecs.xvid:
                    cbVideoQualityUseBitrate.Visible = true;
                    tbVideoQuality.Visible = lblVideoQualityValue.Visible = lblVideoQualityLower.Visible = lblVideoQualityHigher.Visible = !Options.VideoQualityUseBitrate;
                    nudVideoQualityBitrate.Visible = lblVideoQualityBitrateHint.Visible = Options.VideoQualityUseBitrate;
                    break;
                case ConverterVideoCodecs.h264_nvenc:
                case ConverterVideoCodecs.hevc_nvenc:
                case ConverterVideoCodecs.h264_amf:
                case ConverterVideoCodecs.hevc_amf:
                case ConverterVideoCodecs.h264_qsv:
                case ConverterVideoCodecs.hevc_qsv:
                    cbVideoQualityUseBitrate.Visible = false;
                    tbVideoQuality.Visible = lblVideoQualityValue.Visible = lblVideoQualityLower.Visible = lblVideoQualityHigher.Visible = false;
                    nudVideoQualityBitrate.Visible = lblVideoQualityBitrateHint.Visible = true;
                    break;
                default:
                    cbVideoQualityUseBitrate.Visible = tbVideoQuality.Visible = lblVideoQualityValue.Visible = lblVideoQualityLower.Visible =
                        lblVideoQualityHigher.Visible = nudVideoQualityBitrate.Visible = lblVideoQualityBitrateHint.Visible = false;
                    break;
            }

            switch (Options.VideoCodec)
            {
                case ConverterVideoCodecs.x264:
                    tbVideoQuality.Minimum = FFmpegCLIManager.x264_min;
                    tbVideoQuality.Maximum = FFmpegCLIManager.x264_max;
                    break;
                case ConverterVideoCodecs.x265:
                    tbVideoQuality.Minimum = FFmpegCLIManager.x265_min;
                    tbVideoQuality.Maximum = FFmpegCLIManager.x265_max;
                    break;
                case ConverterVideoCodecs.vp8:
                    tbVideoQuality.Minimum = FFmpegCLIManager.vp8_min;
                    tbVideoQuality.Maximum = FFmpegCLIManager.vp8_max;
                    break;
                case ConverterVideoCodecs.vp9:
                    tbVideoQuality.Minimum = FFmpegCLIManager.vp9_min;
                    tbVideoQuality.Maximum = FFmpegCLIManager.vp9_max;
                    break;
                case ConverterVideoCodecs.av1:
                    tbVideoQuality.Minimum = FFmpegCLIManager.av1_min;
                    tbVideoQuality.Maximum = FFmpegCLIManager.av1_max;
                    break;
                case ConverterVideoCodecs.xvid:
                    tbVideoQuality.Minimum = FFmpegCLIManager.xvid_min;
                    tbVideoQuality.Maximum = FFmpegCLIManager.xvid_max;
                    break;
            }

            lblVideoQualityLower.Text = tbVideoQuality.Maximum + "   ← " + Resources.LowerQualitySize;
            lblVideoQualityHigher.Text = Resources.HigherQualitySize + " →   " + tbVideoQuality.Minimum;

            if (formReady)
            {
                Options.VideoQuality = tbVideoQuality.Minimum + tbVideoQuality.Maximum - tbVideoQuality.Value;
            }

            lblVideoQualityValue.Text = Options.VideoQuality.ToString();

            if (!Options.UseCustomArguments)
            {
                try
                {
                    txtArguments.Text = Options.GetFFmpegArgs();
                }
                catch
                {
                    txtArguments.Text = "";
                }
            }

            txtArguments.Enabled = Options.UseCustomArguments;

            btnEncode.Enabled = !string.IsNullOrEmpty(Options.InputFilePath) && !string.IsNullOrEmpty(Options.OutputFolderPath) &&
                !string.IsNullOrEmpty(Options.OutputFileName);
        }

        private void UpdateInputFilePathTextBox(string filePath)
        {
            txtInputFilePath.Text = filePath;

            if (string.IsNullOrEmpty(txtOutputFolder.Text))
            {
                txtOutputFolder.Text = Path.GetDirectoryName(filePath);
            }

            if (string.IsNullOrEmpty(txtOutputFileName.Text))
            {
                txtOutputFileName.Text = Path.GetFileNameWithoutExtension(filePath) + "-output";
            }
        }

        private bool StartEncoding()
        {
            bool result = false;

            if (File.Exists(FFmpegFilePath) && !string.IsNullOrEmpty(Options.InputFilePath) && File.Exists(Options.InputFilePath) &&
                !string.IsNullOrEmpty(Options.OutputFolderPath) && !string.IsNullOrEmpty(Options.OutputFileName))
            {
                using (ffmpeg = new FFmpegCLIManager(FFmpegFilePath))
                {
                    ffmpeg.ShowError = true;
                    ffmpeg.TrackEncodeProgress = true;
                    ffmpeg.EncodeProgressChanged += Manager_EncodeProgressChanged;

                    try
                    {
                        string outputFilePath = Options.OutputFilePath;
                        string args = Options.Arguments;
                        result = ffmpeg.Run(args);

                        if (Options.AutoOpenFolder && result && !ffmpeg.StopRequested)
                        {
                            FileHelpers.OpenFolderWithFile(outputFilePath);
                        }
                    }
                    catch (Exception e)
                    {
                        e.ShowError();
                    }
                }
            }

            return result;
        }

        private Task<bool> StartEncodingAsync()
        {
            return Task.Run(StartEncoding);
        }

        private void Manager_EncodeProgressChanged(float percentage)
        {
            if (!formClosing)
            {
                this.InvokeSafe(() => pbProgress.Value = (int)percentage);
            }
        }

        private void txtInputFilePath_TextChanged(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private void btnInputFilePathBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    string filePath = ofd.FileName;
                    UpdateInputFilePathTextBox(filePath);
                }
            }
        }

        private void txtOutputFolder_TextChanged(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private void btnOutputFolderBrowse_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFolder(txtOutputFolder);
        }

        private void txtOutputFileName_TextChanged(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private void cbVideoEncoder_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private void cbVideoQualityUseBitrate_CheckedChanged(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private void tbVideoQuality_ValueChanged(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private void nudVideoQualityBitrate_ValueChanged(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private void cbAutoOpenFolder_CheckedChanged(object sender, EventArgs e)
        {
            Options.AutoOpenFolder = cbAutoOpenFolder.Checked;
        }

        private void cbUseCustomArguments_CheckedChanged(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private void txtArguments_TextChanged(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private async void btnEncode_Click(object sender, EventArgs e)
        {
            if (!encoding)
            {
                encoding = true;

                btnEncode.Text = Resources.StopEncoding;

                UpdateOptions();
                pbProgress.Value = 0;

                bool result = await StartEncodingAsync();

                if (!formClosing)
                {
                    if (result && ffmpeg != null && !ffmpeg.StopRequested)
                    {
                        pbProgress.Value = 100;
                    }
                    else
                    {
                        pbProgress.Value = 0;
                    }

                    btnEncode.Text = Resources.StartEncoding;
                }

                encoding = false;
            }
            else if (ffmpeg != null)
            {
                ffmpeg.Close();
            }
        }

        private void VideoConverterForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void VideoConverterForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) && e.Data.GetData(DataFormats.FileDrop, false) is string[] files && files.Length > 0)
            {
                string filePath = files[0];
                UpdateInputFilePathTextBox(filePath);
            }
        }

        private void VideoConverterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            formClosing = true;

            if (ffmpeg != null)
            {
                ffmpeg.Close();
            }
        }
    }
}