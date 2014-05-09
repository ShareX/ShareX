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

            nudCRF.Value = Options.Quantizer;

            comboBoxPreset.Items.AddRange(Helpers.GetEnumDescriptions<FFmpegPreset>());
            comboBoxPreset.SelectedIndex = (int)Options.Preset;
        }

        private void comboBoxCodec_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.VideoCodec = (FFmpegVideoCodec)comboBoxCodec.SelectedIndex;
            UpdateUI();
        }

        private void comboBoxExtension_SelectedValueChanged(object sender, EventArgs e)
        {
            Options.Extension = comboBoxExtension.Text;
        }

        private void nudCRF_ValueChanged(object sender, EventArgs e)
        {
            Options.Quantizer = (int)nudCRF.Value;
        }

        private void comboBoxPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.Preset = (FFmpegPreset)comboBoxPreset.SelectedIndex;
        }

        public void UpdateUI()
        {
            switch (Options.VideoCodec)
            {
                case FFmpegVideoCodec.mpeg4:
                case FFmpegVideoCodec.libxvid:
                    lblQuantizer.Text = "qscale";
                    nudCRF.Minimum = 1;
                    nudCRF.Maximum = 31;
                    break;
                case FFmpegVideoCodec.libx264:
                    lblQuantizer.Text = "crf";
                    nudCRF.Minimum = 0;
                    nudCRF.Maximum = 51;
                    break;
                case FFmpegVideoCodec.libvpx:
                    lblQuantizer.Text = "crf";
                    nudCRF.Minimum = 4;
                    nudCRF.Maximum = 63;
                    break;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}