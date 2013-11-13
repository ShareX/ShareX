#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace ImageEffectsLib
{
    public partial class WatermarkForm : Form
    {
        public WatermarkConfig Config { get; set; }
        private bool IsGuiReady;
        private ContextMenuStrip codesMenu;

        public WatermarkForm(WatermarkConfig config = null)
        {
            InitializeComponent();

            if (config == null)
            {
                config = new WatermarkConfig();
            }

            Config = config;

            codesMenu = NameParser.CreateCodesMenu(txtWatermarkText, ReplacementVariables.t);
        }

        private void WatermarkUI_Load(object sender, EventArgs e)
        {
            if (cboWatermarkType.Items.Count == 0)
            {
                cboWatermarkType.Items.AddRange(Helpers.GetEnumDescriptions<WatermarkType>());
            }

            cboWatermarkType.SelectedIndex = (int)Config.WatermarkMode;
            if (chkWatermarkPosition.Items.Count == 0)
            {
                chkWatermarkPosition.Items.AddRange(Helpers.GetEnumDescriptions<PositionType>());
            }

            chkWatermarkPosition.SelectedIndex = (int)Config.WatermarkPositionMode;
            nudWatermarkOffset.Value = Config.WatermarkOffset;
            cbWatermarkAutoHide.Checked = Config.WatermarkAutoHide;

            txtWatermarkText.Text = Config.WatermarkText;
            pbWatermarkFontColor.BackColor = Config.WatermarkFontArgb;
            lblWatermarkFont.Text = FontToString();
            nudWatermarkCornerRadius.Value = Config.WatermarkCornerRadius;
            pbWatermarkGradient1.BackColor = Config.WatermarkGradient1Argb;
            pbWatermarkGradient2.BackColor = Config.WatermarkGradient2Argb;
            pbWatermarkBorderColor.BackColor = Config.WatermarkBorderArgb;
            if (cbWatermarkGradientType.Items.Count == 0)
            {
                cbWatermarkGradientType.Items.AddRange(Enum.GetNames(typeof(LinearGradientMode)));
            }

            cbWatermarkGradientType.SelectedIndex = (int)Config.WatermarkGradientType;

            txtWatermarkImageLocation.Text = Config.WatermarkImageLocation;

            IsGuiReady = true;
            UpdatePreview();
        }

        private void WatermarkUI_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private string FontToString()
        {
            return FontToString(Config.WatermarkFont, Config.WatermarkFontArgb);
        }

        private string FontToString(Font font, Color color)
        {
            return string.Format("{0} - {1} - {2} - {3},{4},{5},{6}", font.Name, font.Size, font.Style, color.R, color.G, color.B, color.A);
        }

        private void SelectColor(Control pb, ref XmlColor color)
        {
            using (DialogColor dColor = new DialogColor(pb.BackColor))
            {
                if (dColor.ShowDialog() == DialogResult.OK)
                {
                    pb.BackColor = dColor.NewColor;
                    color = (Color)dColor.NewColor;
                }
            }
        }

        private void UpdatePreview()
        {
            if (IsGuiReady)
            {
                using (Bitmap bmp = new Bitmap(pbPreview.ClientSize.Width, pbPreview.ClientSize.Height))
                {
                    WatermarkManager.ApplyWatermark(bmp, Config);
                    pbPreview.LoadImage(bmp);
                }
            }
        }

        private void btnWatermarkFont_Click(object sender, EventArgs e)
        {
            try
            {
                using (FontDialog fontDialog = new FontDialog())
                {
                    fontDialog.ShowColor = true;

                    try
                    {
                        fontDialog.Color = Config.WatermarkFontArgb;
                        fontDialog.Font = Config.WatermarkFont;
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteException(ex, "Error while initializing font.");
                    }

                    if (fontDialog.ShowDialog() == DialogResult.OK)
                    {
                        Config.WatermarkFont = fontDialog.Font;
                        Config.WatermarkFontArgb = fontDialog.Color;

                        pbWatermarkFontColor.BackColor = Config.WatermarkFontArgb;
                        lblWatermarkFont.Text = FontToString();
                        UpdatePreview();
                    }
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex, "Error while setting watermark font.");
            }
        }

        private void btwWatermarkBrowseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog { InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) };

            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtWatermarkImageLocation.Text = fd.FileName;
            }
        }

        private void cboWatermarkType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.WatermarkMode = (WatermarkType)cboWatermarkType.SelectedIndex;
            UpdatePreview();
        }

        private void cbWatermarkAutoHide_CheckedChanged(object sender, EventArgs e)
        {
            Config.WatermarkAutoHide = cbWatermarkAutoHide.Checked;
            UpdatePreview();
        }

        private void cbWatermarkGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.WatermarkGradientType = (LinearGradientMode)cbWatermarkGradientType.SelectedIndex;
            UpdatePreview();
        }

        private void cbWatermarkPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.WatermarkPositionMode = (PositionType)chkWatermarkPosition.SelectedIndex;
            UpdatePreview();
        }

        private void nudWatermarkCornerRadius_ValueChanged(object sender, EventArgs e)
        {
            Config.WatermarkCornerRadius = (int)nudWatermarkCornerRadius.Value;
            UpdatePreview();
        }

        private void nudWatermarkOffset_ValueChanged(object sender, EventArgs e)
        {
            Config.WatermarkOffset = (int)nudWatermarkOffset.Value;
            UpdatePreview();
        }

        private void pbWatermarkBorderColor_Click(object sender, EventArgs e)
        {
            SelectColor((PictureBox)sender, ref Config.WatermarkBorderArgb);
            UpdatePreview();
        }

        private void pbWatermarkFontColor_Click(object sender, EventArgs e)
        {
            SelectColor((PictureBox)sender, ref Config.WatermarkFontArgb);
            lblWatermarkFont.Text = FontToString();
            UpdatePreview();
        }

        private void pbWatermarkGradient1_Click(object sender, EventArgs e)
        {
            SelectColor((PictureBox)sender, ref Config.WatermarkGradient1Argb);
            UpdatePreview();
        }

        private void pbWatermarkGradient2_Click(object sender, EventArgs e)
        {
            SelectColor((PictureBox)sender, ref Config.WatermarkGradient2Argb);
            UpdatePreview();
        }

        private void txtWatermarkImageLocation_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(txtWatermarkImageLocation.Text))
            {
                Config.WatermarkImageLocation = txtWatermarkImageLocation.Text;
                UpdatePreview();
            }
        }

        private void txtWatermarkText_TextChanged(object sender, EventArgs e)
        {
            Config.WatermarkText = txtWatermarkText.Text;
            UpdatePreview();
        }
    }
}