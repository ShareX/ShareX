using HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenCaptureLib
{
    public partial class FFmpegCLIOptionsForm : Form
    {
        private ScreencastOptions Options = null;

        public FFmpegCLIOptionsForm(ScreencastOptions options)
        {
            Options = options;

            InitializeComponent();
            this.Text = string.Format("{0} - FFmpeg CLI Options", Application.ProductName);
            this.Icon = ShareXResources.Icon;

            LoadSettings();
            UpdateUI();
        }

        private void LoadSettings()
        {
            comboBoxCodec.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegVideoCodec>());
            comboBoxCodec.SelectedIndex = (int)Options.FFmpeg.VideoCodec;
            comboBoxCodec.SelectedIndexChanged += (sender, e) => UpdateUI();

            comboBoxExtension.Text = Options.FFmpeg.Extension;
            comboBoxExtension.SelectedIndexChanged += (sender, e) => UpdateUI();

            nudCRF.Value = Options.FFmpeg.CRF.Between((int)nudCRF.Minimum, (int)nudCRF.Maximum);
            nudCRF.ValueChanged += (sender, e) => UpdateUI();

            comboBoxPreset.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegPreset>());
            comboBoxPreset.SelectedIndex = (int)Options.FFmpeg.Preset;
            comboBoxPreset.SelectedIndexChanged += (sender, e) => UpdateUI();

            nudQscale.Value = Options.FFmpeg.qscale.Between((int)nudQscale.Minimum, (int)nudQscale.Maximum);
            nudQscale.ValueChanged += (sender, e) => UpdateUI();

            textBoxUserArgs.Text = Options.FFmpeg.UserArgs;
            textBoxUserArgs.TextChanged += (sender, e) => UpdateUI();

            string cli = Path.Combine(Application.StartupPath, "ffmpeg.exe");
            if (string.IsNullOrEmpty(Options.FFmpeg.CLIPath) && File.Exists(cli))
                Options.FFmpeg.CLIPath = cli;
            textBoxFFmpegPath.Text = Options.FFmpeg.CLIPath;
            textBoxFFmpegPath.TextChanged += (sender, e) => UpdateUI();
        }

        public void SaveSettings()
        {
            Options.FFmpeg.VideoCodec = (FFmpegVideoCodec)comboBoxCodec.SelectedIndex;
            Options.FFmpeg.Extension = comboBoxExtension.Text;

            Options.FFmpeg.CRF = (int)nudCRF.Value;
            Options.FFmpeg.Preset = (FFmpegPreset)comboBoxPreset.SelectedIndex;

            Options.FFmpeg.qscale = (int)nudQscale.Value;

            Options.FFmpeg.UserArgs = textBoxUserArgs.Text;

            Options.FFmpeg.CLIPath = textBoxFFmpegPath.Text;
        }

        public void UpdateUI()
        {
            SaveSettings();

            groupBoxH263.Enabled = Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libxvid;
            groupBoxH264.Enabled = Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libx264 || Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libvpx;

            if ((Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libx264 || Options.FFmpeg.VideoCodec == FFmpegVideoCodec.libxvid) && comboBoxExtension.Text == "webm")
                comboBoxExtension.Text = "mp4";
            else if ((FFmpegVideoCodec)comboBoxCodec.SelectedIndex == FFmpegVideoCodec.libvpx && comboBoxExtension.Text != "webm")
                comboBoxExtension.Text = "webm";

            textBoxCommandLinePreview.Text = Options.GetFFmpegArgs();
        }

        private void buttonFFmpegBrowse_Click(object sender, EventArgs e)
        {
            Helpers.BrowseFile("Browse for ffmpeg.exe", textBoxFFmpegPath, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            UpdateUI();
        }

        private void buttonFFmpegHelp_Click(object sender, EventArgs e)
        {
            Helpers.OpenURL("https://www.ffmpeg.org/ffmpeg.html");
        }
    }
}