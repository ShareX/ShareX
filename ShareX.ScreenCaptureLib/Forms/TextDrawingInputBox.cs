#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    internal partial class TextDrawingInputBox : Form
    {
        public string InputText { get; private set; }
        public string TextFont { get; private set; }
        public Color TextColor { get; private set; }
        public int TextSize { get; private set; }
        public bool TextBold { get; private set; }
        public bool TextItalic { get; private set; }
        public bool TextUnderline { get; private set; }
        public StringAlignment AlignmentHorizontal { get; private set; }
        public StringAlignment AlignmentVertical { get; private set; }

        public TextDrawingInputBox(string inputText, string textFont, Color textColor, int textSize)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            InputText = inputText;
            TextFont = textFont;
            TextColor = textColor;
            TextSize = textSize;

            if (InputText != null)
            {
                txtInput.Text = InputText;
            }

            UpdateInputBox();

            cbFonts.Items.AddRange(FontFamily.Families.Select(x => x.Name).ToArray());

            if (cbFonts.Items.Contains(TextFont))
            {
                cbFonts.SelectedItem = TextFont;
            }
            else
            {
                cbFonts.SelectedItem = "Arial";
            }

            nudTextSize.Value = TextSize;
            btnTextColor.Color = TextColor;

            UpdateHorizontalAlignmentImage();
            UpdateVerticalAlignmentImage();
        }

        private void cbFonts_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextFont = cbFonts.SelectedItem as string;
            UpdateInputBox();
        }

        private void nudTextSize_ValueChanged(object sender, EventArgs e)
        {
            TextSize = (int)nudTextSize.Value;
            UpdateInputBox();
        }

        private void btnTextColor_ColorChanged(Color color)
        {
            TextColor = btnTextColor.Color;
            UpdateInputBox();
        }

        private void cbBold_CheckedChanged(object sender, EventArgs e)
        {
            TextBold = cbBold.Checked;
            UpdateInputBox();
        }

        private void cbItalic_CheckedChanged(object sender, EventArgs e)
        {
            TextItalic = cbItalic.Checked;
            UpdateInputBox();
        }

        private void cbUnderline_CheckedChanged(object sender, EventArgs e)
        {
            TextUnderline = cbUnderline.Checked;
            UpdateInputBox();
        }

        private void btnAlignmentHorizontal_Click(object sender, EventArgs e)
        {
            cmsAlignmentHorizontal.Show(btnAlignmentHorizontal, 0, btnAlignmentHorizontal.Height + 1);
        }

        private void tsmiAlignmentLeft_Click(object sender, EventArgs e)
        {
            AlignmentHorizontal = StringAlignment.Near;
            UpdateHorizontalAlignmentImage();
            UpdateInputBox();
        }

        private void tsmiAlignmentCenter_Click(object sender, EventArgs e)
        {
            AlignmentHorizontal = StringAlignment.Center;
            UpdateHorizontalAlignmentImage();
            UpdateInputBox();
        }

        private void tsmiAlignmentRight_Click(object sender, EventArgs e)
        {
            AlignmentHorizontal = StringAlignment.Far;
            UpdateHorizontalAlignmentImage();
            UpdateInputBox();
        }

        private void btnAlignmentVertical_Click(object sender, EventArgs e)
        {
            cmsAlignmentVertical.Show(btnAlignmentVertical, 0, btnAlignmentVertical.Height + 1);
        }

        private void tsmiAlignmentTop_Click(object sender, EventArgs e)
        {
            AlignmentVertical = StringAlignment.Near;
            UpdateVerticalAlignmentImage();
        }

        private void tsmiAlignmentMiddle_Click(object sender, EventArgs e)
        {
            AlignmentVertical = StringAlignment.Center;
            UpdateVerticalAlignmentImage();
        }

        private void tsmiAlignmentBottom_Click(object sender, EventArgs e)
        {
            AlignmentVertical = StringAlignment.Far;
            UpdateVerticalAlignmentImage();
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            InputText = txtInput.Text;
        }

        private void UpdateInputBox()
        {
            FontStyle fontStyle = FontStyle.Regular;

            if (TextBold)
            {
                fontStyle |= FontStyle.Bold;
            }

            if (TextItalic)
            {
                fontStyle |= FontStyle.Italic;
            }

            if (TextUnderline)
            {
                fontStyle |= FontStyle.Underline;
            }

            txtInput.Font = new Font(TextFont, TextSize, fontStyle);
            txtInput.ForeColor = TextColor;

            HorizontalAlignment horizontalAlignment;

            switch (AlignmentHorizontal)
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
            switch (AlignmentHorizontal)
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
            switch (AlignmentVertical)
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}