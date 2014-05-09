using HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenCaptureLib
{
    public partial class FFmpegCLIOptionsForm : Form
    {
        private FFmpegOptions Options = null;

        public FFmpegCLIOptionsForm(FFmpegOptions options)
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
            comboBoxCodec.SelectedIndex = (int)Options.VideoCodec;

            comboBoxExtension.Text = Options.Extension;

            nudCRF.Value = Options.CRF.Between((int)nudCRF.Minimum, (int)nudCRF.Maximum);

            comboBoxPreset.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegPreset>());
            comboBoxPreset.SelectedIndex = (int)Options.Preset;

            nudQscale.Value = Options.qscale.Between((int)nudQscale.Minimum, (int)nudQscale.Maximum);

            textBoxFFmpegPath.Text = Options.CLIPath;
        }

        private void SaveSettings()
        {
            Options.VideoCodec = (FFmpegVideoCodec)comboBoxCodec.SelectedIndex;
            Options.Extension = comboBoxExtension.Text;

            Options.CRF = (int)nudCRF.Value;
            Options.Preset = (FFmpegPreset)comboBoxPreset.SelectedIndex;

            Options.qscale = (int)nudQscale.Value;
        }

        private void comboBoxCodec_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            groupBoxH263.Enabled = Options.VideoCodec == FFmpegVideoCodec.libxvid || Options.VideoCodec == FFmpegVideoCodec.mpeg4;
            groupBoxH264.Enabled = Options.VideoCodec == FFmpegVideoCodec.libx264 || Options.VideoCodec == FFmpegVideoCodec.libvpx;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            SaveSettings();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonFFmpegBrowse_Click(object sender, EventArgs e)
        {
            Helpers.BrowseFile("Browse for ffmpeg.exe", textBoxFFmpegPath, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            Options.CLIPath = textBoxFFmpegPath.Text;
        }
    }
}