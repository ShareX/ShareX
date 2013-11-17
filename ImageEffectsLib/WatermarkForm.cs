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
        private WatermarkConfig config;
        private bool IsGuiReady;
        private ContextMenuStrip codesMenu;

        public WatermarkForm(WatermarkConfig watermarkConfig)
        {
            InitializeComponent();
            config = watermarkConfig;
            codesMenu = NameParser.CreateCodesMenu(txtWatermarkText, ReplacementVariables.t);
        }

        private void WatermarkUI_Load(object sender, EventArgs e)
        {
            if (cboWatermarkType.Items.Count == 0)
            {
                cboWatermarkType.Items.AddRange(Enum.GetNames(typeof(WatermarkType)));
            }

            cboWatermarkType.SelectedIndex = (int)config.Type;

            if (chkWatermarkPosition.Items.Count == 0)
            {
                chkWatermarkPosition.Items.AddRange(Enum.GetNames(typeof(ContentAlignment)));
            }

            chkWatermarkPosition.SelectedIndex = config.Text.Placement.GetIndex();
            nudWatermarkOffset.Value = config.Text.Offset.X;
            cbWatermarkAutoHide.Checked = config.Text.AutoHide;

            txtWatermarkText.Text = config.Text.Text;
            pbWatermarkFontColor.BackColor = config.Text.TextColor;
            lblWatermarkFont.Text = new FontConverter().ConvertToInvariantString(config.Text.TextFont);

            cbWatermarkDrawBackground.Checked = config.Text.DrawBackground;
            nudWatermarkCornerRadius.Value = config.Text.CornerRadius;
            pbWatermarkGradient1.BackColor = config.Text.BackgroundColor;
            cbWatermarkBackColor2.Checked = config.Text.UseGradient;
            pbWatermarkGradient2.BackColor = config.Text.BackgroundColor2;
            pbWatermarkBorderColor.BackColor = config.Text.BorderColor;

            if (cbWatermarkGradientType.Items.Count == 0)
            {
                cbWatermarkGradientType.Items.AddRange(Enum.GetNames(typeof(LinearGradientMode)));
            }

            cbWatermarkGradientType.SelectedIndex = (int)config.Text.GradientType;

            txtWatermarkImageLocation.Text = config.Image.ImageLocation;

            IsGuiReady = true;
            UpdatePreview();
        }

        private void WatermarkUI_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private Color SelectColor(Control pb)
        {
            using (DialogColor dColor = new DialogColor(pb.BackColor))
            {
                if (dColor.ShowDialog() == DialogResult.OK)
                {
                    pb.BackColor = dColor.NewColor;
                    return (Color)dColor.NewColor;
                }

                return pb.BackColor;
            }
        }

        private void UpdatePreview()
        {
            if (IsGuiReady)
            {
                using (Bitmap bmp = new Bitmap(pbPreview.ClientSize.Width, pbPreview.ClientSize.Height))
                {
                    config.Apply(bmp);
                    pbPreview.LoadImage(bmp);
                }
            }
        }

        private void cboWatermarkType_SelectedIndexChanged(object sender, EventArgs e)
        {
            config.Type = (WatermarkType)cboWatermarkType.SelectedIndex;
            UpdatePreview();
        }

        private void cbWatermarkPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            config.Text.Placement = Helpers.GetEnumFromIndex<ContentAlignment>(chkWatermarkPosition.SelectedIndex);
            UpdatePreview();
        }

        private void nudWatermarkOffset_ValueChanged(object sender, EventArgs e)
        {
            config.Text.Offset = new Point((int)nudWatermarkOffset.Value, (int)nudWatermarkOffset.Value);
            UpdatePreview();
        }

        private void cbWatermarkAutoHide_CheckedChanged(object sender, EventArgs e)
        {
            config.Text.AutoHide = cbWatermarkAutoHide.Checked;
            UpdatePreview();
        }

        private void txtWatermarkText_TextChanged(object sender, EventArgs e)
        {
            config.Text.Text = txtWatermarkText.Text;
            UpdatePreview();
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
                        fontDialog.Font = config.Text.TextFont;
                        fontDialog.Color = config.Text.TextColor;
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteException(ex, "Error while initializing font.");
                    }

                    if (fontDialog.ShowDialog() == DialogResult.OK)
                    {
                        config.Text.TextFont = fontDialog.Font;
                        config.Text.TextColor = fontDialog.Color;

                        pbWatermarkFontColor.BackColor = config.Text.TextColor;
                        lblWatermarkFont.Text = new FontConverter().ConvertToInvariantString(config.Text.TextFont);
                        UpdatePreview();
                    }
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex, "Error while setting watermark font.");
            }
        }

        private void pbWatermarkFontColor_Click(object sender, EventArgs e)
        {
            config.Text.TextColor = SelectColor((PictureBox)sender);
            UpdatePreview();
        }

        private void cbWatermarkDrawBackground_CheckedChanged(object sender, EventArgs e)
        {
            config.Text.DrawBackground = cbWatermarkDrawBackground.Checked;
            UpdatePreview();
        }

        private void nudWatermarkCornerRadius_ValueChanged(object sender, EventArgs e)
        {
            config.Text.CornerRadius = (int)nudWatermarkCornerRadius.Value;
            UpdatePreview();
        }

        private void pbWatermarkBorderColor_Click(object sender, EventArgs e)
        {
            config.Text.BorderColor = SelectColor((PictureBox)sender);
            UpdatePreview();
        }

        private void pbWatermarkGradient1_Click(object sender, EventArgs e)
        {
            config.Text.BackgroundColor = SelectColor((PictureBox)sender);
            UpdatePreview();
        }

        private void cbWatermarkBackColor2_CheckedChanged(object sender, EventArgs e)
        {
            config.Text.UseGradient = cbWatermarkBackColor2.Checked;
            pbWatermarkGradient2.Enabled = config.Text.UseGradient;
            cbWatermarkGradientType.Enabled = config.Text.UseGradient;
            UpdatePreview();
        }

        private void pbWatermarkGradient2_Click(object sender, EventArgs e)
        {
            config.Text.BackgroundColor2 = SelectColor((PictureBox)sender);
            UpdatePreview();
        }

        private void cbWatermarkGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            config.Text.GradientType = (LinearGradientMode)cbWatermarkGradientType.SelectedIndex;
            UpdatePreview();
        }

        private void txtWatermarkImageLocation_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(txtWatermarkImageLocation.Text))
            {
                config.Image.ImageLocation = txtWatermarkImageLocation.Text;
                UpdatePreview();
            }
        }

        private void btwWatermarkBrowseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtWatermarkImageLocation.Text = ofd.FileName;
                }
            }
        }
    }
}