#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    internal partial class TextDrawingInputBox : Form
    {
        public string InputText { get; private set; }
        public TextDrawingOptions Options { get; private set; }

        public TextDrawingInputBox(string text, TextDrawingOptions options)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            InputText = text;
            Options = options;

            if (InputText != null)
            {
                txtInput.Text = InputText;
            }

            UpdateInputBox();

            cbFonts.Items.AddRange(FontFamily.Families.Select(x => x.Name).ToArray());

            if (cbFonts.Items.Contains(Options.Font))
            {
                cbFonts.SelectedItem = Options.Font;
            }
            else
            {
                cbFonts.SelectedItem = AnnotationOptions.DefaultFont;
            }

            nudTextSize.SetValue(Options.Size);
            btnTextColor.Color = Options.Color;
            cbBold.Checked = Options.Bold;
            cbItalic.Checked = Options.Italic;
            cbUnderline.Checked = Options.Underline;

            UpdateHorizontalAlignmentImage();
            UpdateVerticalAlignmentImage();
        }

        private void Close(DialogResult result)
        {
            DialogResult = result;

            if (result == DialogResult.OK)
            {
                InputText = txtInput.Text;
            }

            Close();
        }

        private void TextDrawingInputBox_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void cbFonts_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.Font = cbFonts.SelectedItem as string;
            UpdateInputBox();
        }

        private void nudTextSize_ValueChanged(object sender, EventArgs e)
        {
            Options.Size = (int)nudTextSize.Value;
            UpdateInputBox();
        }

        private void btnTextColor_ColorChanged(Color color)
        {
            Options.Color = btnTextColor.Color;
            UpdateInputBox();
        }

        private void cbBold_CheckedChanged(object sender, EventArgs e)
        {
            Options.Bold = cbBold.Checked;
            UpdateInputBox();
        }

        private void cbItalic_CheckedChanged(object sender, EventArgs e)
        {
            Options.Italic = cbItalic.Checked;
            UpdateInputBox();
        }

        private void cbUnderline_CheckedChanged(object sender, EventArgs e)
        {
            Options.Underline = cbUnderline.Checked;
            UpdateInputBox();
        }

        private void btnAlignmentHorizontal_Click(object sender, EventArgs e)
        {
            cmsAlignmentHorizontal.Show(btnAlignmentHorizontal, 0, btnAlignmentHorizontal.Height + 1);
        }

        private void tsmiAlignmentLeft_Click(object sender, EventArgs e)
        {
            Options.AlignmentHorizontal = StringAlignment.Near;
            UpdateHorizontalAlignmentImage();
            UpdateInputBox();
        }

        private void tsmiAlignmentCenter_Click(object sender, EventArgs e)
        {
            Options.AlignmentHorizontal = StringAlignment.Center;
            UpdateHorizontalAlignmentImage();
            UpdateInputBox();
        }

        private void tsmiAlignmentRight_Click(object sender, EventArgs e)
        {
            Options.AlignmentHorizontal = StringAlignment.Far;
            UpdateHorizontalAlignmentImage();
            UpdateInputBox();
        }

        private void btnAlignmentVertical_Click(object sender, EventArgs e)
        {
            cmsAlignmentVertical.Show(btnAlignmentVertical, 0, btnAlignmentVertical.Height + 1);
        }

        private void tsmiAlignmentTop_Click(object sender, EventArgs e)
        {
            Options.AlignmentVertical = StringAlignment.Near;
            UpdateVerticalAlignmentImage();
        }

        private void tsmiAlignmentMiddle_Click(object sender, EventArgs e)
        {
            Options.AlignmentVertical = StringAlignment.Center;
            UpdateVerticalAlignmentImage();
        }

        private void tsmiAlignmentBottom_Click(object sender, EventArgs e)
        {
            Options.AlignmentVertical = StringAlignment.Far;
            UpdateVerticalAlignmentImage();
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Escape)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void txtInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                Close(DialogResult.OK);
            }
            else if (e.KeyData == Keys.Escape)
            {
                Close(DialogResult.Cancel);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close(DialogResult.OK);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close(DialogResult.Cancel);
        }

        private void UpdateInputBox()
        {
            Font font;

            try
            {
                font = new Font(Options.Font, Options.Size, Options.Style);
            }
            catch
            {
                Options.Font = AnnotationOptions.DefaultFont;
                font = new Font(Options.Font, Options.Size, Options.Style);
            }

            txtInput.Font = font;

            txtInput.ForeColor = Options.Color;
            txtInput.BackColor = ColorHelpers.VisibleColor(Options.Color, Color.White, Color.FromArgb(50, 50, 50));

            HorizontalAlignment horizontalAlignment;

            switch (Options.AlignmentHorizontal)
            {
                default:
                case StringAlignment.Near:
                    horizontalAlignment = HorizontalAlignment.Left;
                    break;
                case StringAlignment.Center:
                    horizontalAlignment = HorizontalAlignment.Center;
                    break;
                case StringAlignment.Far:
                    horizontalAlignment = HorizontalAlignment.Right;
                    break;
            }

            txtInput.TextAlign = horizontalAlignment;
        }

        private void UpdateHorizontalAlignmentImage()
        {
            switch (Options.AlignmentHorizontal)
            {
                default:
                case StringAlignment.Near:
                    btnAlignmentHorizontal.Image = Resources.edit_alignment;
                    break;
                case StringAlignment.Center:
                    btnAlignmentHorizontal.Image = Resources.edit_alignment_center;
                    break;
                case StringAlignment.Far:
                    btnAlignmentHorizontal.Image = Resources.edit_alignment_right;
                    break;
            }
        }

        private void UpdateVerticalAlignmentImage()
        {
            switch (Options.AlignmentVertical)
            {
                default:
                case StringAlignment.Near:
                    btnAlignmentVertical.Image = Resources.edit_vertical_alignment_top;
                    break;
                case StringAlignment.Center:
                    btnAlignmentVertical.Image = Resources.edit_vertical_alignment_middle;
                    break;
                case StringAlignment.Far:
                    btnAlignmentVertical.Image = Resources.edit_vertical_alignment;
                    break;
            }
        }
    }
}