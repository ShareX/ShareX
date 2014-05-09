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
            Options.CRF = (int)nudCRF.Value;
        }

        private void comboBoxPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.Preset = (FFmpegPreset)comboBoxPreset.SelectedIndex;
        }

        public void UpdateUI()
        {
            groupBoxH263.Enabled = Options.VideoCodec == FFmpegVideoCodec.libxvid || Options.VideoCodec == FFmpegVideoCodec.mpeg4;
            groupBoxH264.Enabled = Options.VideoCodec == FFmpegVideoCodec.libx264 || Options.VideoCodec == FFmpegVideoCodec.libvpx;
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

        private void nudQscale_ValueChanged(object sender, EventArgs e)
        {
            Options.qscale = (int)nudQscale.Value;
        }
    }
}