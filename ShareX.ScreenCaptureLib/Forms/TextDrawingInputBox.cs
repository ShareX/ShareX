﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    internal partial class TextDrawingInputBox : Form
    {
        public string InputText { get; private set; }
        public TextDrawingOptions Options { get; private set; }
        public ColorPickerOptions ColorPickerOptions { get; private set; }

        private int processKeyCount;

        public TextDrawingInputBox(string text, TextDrawingOptions options, bool supportGradient, ColorPickerOptions colorPickerOptions)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            InputText = text;
            Options = options;
            ColorPickerOptions = colorPickerOptions;

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

            btnTextColor.ColorPickerOptions = ColorPickerOptions;
            btnTextColor.Color = Options.Color;

            btnGradient.Visible = supportGradient;

            if (supportGradient)
            {
                tsmiEnableGradient.Checked = Options.Gradient;

                tsmiSecondColor.Image = ImageHelpers.CreateColorPickerIcon(Options.Color2, new Rectangle(0, 0, 16, 16));

                switch (Options.GradientMode)
                {
                    case LinearGradientMode.Horizontal:
                        tsrbmiGradientHorizontal.Checked = true;
                        break;
                    case LinearGradientMode.Vertical:
                        tsrbmiGradientVertical.Checked = true;
                        break;
                    case LinearGradientMode.ForwardDiagonal:
                        tsrbmiGradientForwardDiagonal.Checked = true;
                        break;
                    case LinearGradientMode.BackwardDiagonal:
                        tsrbmiGradientBackwardDiagonal.Checked = true;
                        break;
                }
            }

            cbBold.Checked = Options.Bold;
            cbItalic.Checked = Options.Italic;
            cbUnderline.Checked = Options.Underline;

            UpdateButtonImages();
            UpdateEnterTip();

            txtInput.SupportSelectAll();

            ApplyDpiSafeLayout();
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

        private void UpdateEnterTip()
        {
            if (Options.EnterKeyNewLine)
            {
                lblTip.Text = Resources.NewLineEnterOKCtrlEnter;
            }
            else
            {
                lblTip.Text = Resources.NewLineCtrlEnterOKEnter;
            }
        }

        private void TextDrawingInputBox_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
            ApplyDpiSafeLayout();
        }

        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {
            base.OnDpiChanged(e);
            ApplyDpiSafeLayout(e.DeviceDpiNew);
            UpdateInputBox();
        }

        private void ApplyDpiSafeLayout(int? deviceDpi = null)
        {
            int dpi = deviceDpi ?? DeviceDpi;
            float scale = Math.Max(1f, dpi / 96f);

            if (flpProperties != null)
            {
                flpProperties.AutoSize = true;
                flpProperties.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            }

            if (cbFonts != null)
            {
                cbFonts.IntegralHeight = false;
                cbFonts.AutoSize = false;
                cbFonts.Height = Math.Max(cbFonts.Height, cbFonts.PreferredHeight);
            }

            if (nudTextSize != null)
            {
                nudTextSize.AutoSize = false;
                nudTextSize.Height = Math.Max(nudTextSize.Height, nudTextSize.PreferredHeight);
            }

            if (txtInput != null)
            {
                txtInput.AutoSize = false;
                int preferred = txtInput.PreferredHeight;
                if (txtInput.Height < preferred)
                {
                    txtInput.Height = preferred;
                }

                int spacing = (int)Math.Round(6 * scale);

                if (btnOK != null) btnOK.Anchor = AnchorStyles.Bottom | (btnOK.Anchor & AnchorStyles.Right) | AnchorStyles.Left;
                if (btnCancel != null) btnCancel.Anchor = AnchorStyles.Bottom | (btnCancel.Anchor & AnchorStyles.Right) | AnchorStyles.Left;
                if (btnSwapEnterKey != null) btnSwapEnterKey.Anchor = AnchorStyles.Bottom | (btnSwapEnterKey.Anchor & AnchorStyles.Right) | AnchorStyles.Left;
                if (lblTip != null) lblTip.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

                int bottomRowHeight = 0;
                if (btnOK != null) bottomRowHeight = Math.Max(bottomRowHeight, btnOK.Height);
                if (btnCancel != null) bottomRowHeight = Math.Max(bottomRowHeight, btnCancel.Height);
                if (btnSwapEnterKey != null) bottomRowHeight = Math.Max(bottomRowHeight, btnSwapEnterKey.Height);
                if (lblTip != null) bottomRowHeight = Math.Max(bottomRowHeight, lblTip.Height);
                if (bottomRowHeight <= 0) bottomRowHeight = (int)Math.Round(24 * scale);

                int bottomRowTop = ClientSize.Height - bottomRowHeight - spacing;
                if (btnOK != null && btnOK.Top != bottomRowTop) btnOK.Top = bottomRowTop;
                if (btnCancel != null && btnCancel.Top != bottomRowTop) btnCancel.Top = bottomRowTop;
                if (btnSwapEnterKey != null && btnSwapEnterKey.Top != bottomRowTop) btnSwapEnterKey.Top = bottomRowTop;
                if (lblTip != null && lblTip.Top != bottomRowTop) lblTip.Top = bottomRowTop;

                int topBound = txtInput.Top;
                if (flpProperties != null)
                {
                    topBound = Math.Max(topBound, flpProperties.Bottom + spacing);
                }

                int left = txtInput.Left;
                int rightBound = ClientSize.Width - spacing;
                int bottomBound = bottomRowTop - spacing;

                if (bottomBound > topBound)
                {
                    if (txtInput.Top != topBound) txtInput.Top = topBound;
                    int desiredHeight = Math.Max(preferred, bottomBound - topBound);
                    if (desiredHeight > 0 && txtInput.Height != desiredHeight)
                    {
                        txtInput.Height = desiredHeight;
                    }
                    int desiredWidth = Math.Max(50, rightBound - left);
                    if (txtInput.Width != desiredWidth) txtInput.Width = desiredWidth;
                }
            }

            int buttonMin = (int)Math.Round(24 * scale);
            if (btnOK != null)
            {
                btnOK.AutoSize = true;
                btnOK.MinimumSize = new Size(0, buttonMin);
            }
            if (btnCancel != null)
            {
                btnCancel.AutoSize = true;
                btnCancel.MinimumSize = new Size(0, buttonMin);
            }
            if (btnSwapEnterKey != null)
            {
                btnSwapEnterKey.AutoSize = true;
                btnSwapEnterKey.MinimumSize = new Size(0, buttonMin);
            }
            if (btnTextColor != null)
            {
                btnTextColor.AutoSize = true;
                btnTextColor.MinimumSize = new Size(0, buttonMin);
            }
            if (btnGradient != null)
            {
                btnGradient.AutoSize = true;
                btnGradient.MinimumSize = new Size(0, buttonMin);
            }
            if (cbBold != null)
            {
                cbBold.AutoSize = true;
            }
            if (cbItalic != null)
            {
                cbItalic.AutoSize = true;
            }
            if (cbUnderline != null)
            {
                cbUnderline.AutoSize = true;
            }
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
            ApplyDpiSafeLayout();
        }

        private void btnTextColor_ColorChanged(Color color)
        {
            Options.Color = btnTextColor.Color;
            UpdateInputBox();
        }

        private void btnGradient_Click(object sender, EventArgs e)
        {
            cmsGradient.Show(btnGradient, 1, btnGradient.Height + 1);
        }

        private void tsmiEnableGradient_Click(object sender, EventArgs e)
        {
            Options.Gradient = tsmiEnableGradient.Checked;
        }

        private void tsmiSecondColor_Click(object sender, EventArgs e)
        {
            ColorPickerForm.PickColor(Options.Color2, out Color newColor, this, null, ColorPickerOptions);
            Options.Color2 = newColor;
            if (tsmiSecondColor.Image != null) tsmiSecondColor.Image.Dispose();
            tsmiSecondColor.Image = ImageHelpers.CreateColorPickerIcon(Options.Color2, new Rectangle(0, 0, 16, 16));
        }

        private void tsrbmiGradientHorizontal_Click(object sender, EventArgs e)
        {
            Options.GradientMode = LinearGradientMode.Horizontal;
        }

        private void tsrbmiGradientVertical_Click(object sender, EventArgs e)
        {
            Options.GradientMode = LinearGradientMode.Vertical;
        }

        private void tsrbmiGradientForwardDiagonal_Click(object sender, EventArgs e)
        {
            Options.GradientMode = LinearGradientMode.ForwardDiagonal;
        }

        private void tsrbmiGradientBackwardDiagonal_Click(object sender, EventArgs e)
        {
            Options.GradientMode = LinearGradientMode.BackwardDiagonal;
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
            cmsAlignmentHorizontal.Show(btnAlignmentHorizontal, 1, btnAlignmentHorizontal.Height + 1);
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
            cmsAlignmentVertical.Show(btnAlignmentVertical, 1, btnAlignmentVertical.Height + 1);
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
            Keys keyOK = Options.EnterKeyNewLine ? Keys.Control | Keys.Enter : Keys.Enter;

            // If we get VK_PROCESSKEY, the next KeyUp event will be fired by the IME
            // we should ignore these when checking if enter is pressed (GH-3621)
            if (e.KeyCode == Keys.ProcessKey)
            {
                processKeyCount += 1;
            }

            if (e.KeyData == keyOK || e.KeyData == Keys.Escape)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void txtInput_KeyUp(object sender, KeyEventArgs e)
        {
            // If processKeyCount != 0, then this KeyUp event was fired by the
            // IME suggestion box, not by the user intentionally pressing Enter
            if (processKeyCount == 0)
            {
                Keys keyOK = Options.EnterKeyNewLine ? Keys.Control | Keys.Enter : Keys.Enter;

                if (e.KeyData == keyOK)
                {
                    Close(DialogResult.OK);
                }
                else if (e.KeyData == Keys.Escape)
                {
                    Close(DialogResult.Cancel);
                }
            }

            processKeyCount = Math.Max(0, processKeyCount - 1);
        }

        private void btnSwapEnterKey_Click(object sender, EventArgs e)
        {
            Options.EnterKeyNewLine = !Options.EnterKeyNewLine;
            UpdateEnterTip();
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
                float displayPoints = Math.Max(1f, Options.Size * 72f / Math.Max(1, DeviceDpi));
                font = new Font(Options.Font, displayPoints, Options.Style, GraphicsUnit.Point);
            }
            catch
            {
                Options.Font = AnnotationOptions.DefaultFont;
                float displayPoints = Math.Max(1f, Options.Size * 72f / Math.Max(1, DeviceDpi));
                font = new Font(Options.Font, displayPoints, Options.Style, GraphicsUnit.Point);
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

        private void UpdateButtonImages()
        {
            cbBold.Image = ShareXResources.IsDarkTheme ? Resources.edit_bold_white : Resources.edit_bold;
            cbItalic.Image = ShareXResources.IsDarkTheme ? Resources.edit_italic_white : Resources.edit_italic;
            cbUnderline.Image = ShareXResources.IsDarkTheme ? Resources.edit_underline_white : Resources.edit_underline;
            UpdateHorizontalAlignmentImage();
            UpdateVerticalAlignmentImage();
            tsmiAlignmentLeft.Image = ShareXResources.IsDarkTheme ? Resources.edit_alignment_white : Resources.edit_alignment;
            tsmiAlignmentCenter.Image = ShareXResources.IsDarkTheme ? Resources.edit_alignment_center_white : Resources.edit_alignment_center;
            tsmiAlignmentRight.Image = ShareXResources.IsDarkTheme ? Resources.edit_alignment_right_white : Resources.edit_alignment_right;
            tsmiAlignmentTop.Image = ShareXResources.IsDarkTheme ? Resources.edit_vertical_alignment_top_white : Resources.edit_vertical_alignment_top;
            tsmiAlignmentMiddle.Image = ShareXResources.IsDarkTheme ? Resources.edit_vertical_alignment_middle_white : Resources.edit_vertical_alignment_middle;
            tsmiAlignmentBottom.Image = ShareXResources.IsDarkTheme ? Resources.edit_vertical_alignment_white : Resources.edit_vertical_alignment;
        }

        private void UpdateHorizontalAlignmentImage()
        {
            switch (Options.AlignmentHorizontal)
            {
                default:
                case StringAlignment.Near:
                    btnAlignmentHorizontal.Image = ShareXResources.IsDarkTheme ? Resources.edit_alignment_white : Resources.edit_alignment;
                    break;
                case StringAlignment.Center:
                    btnAlignmentHorizontal.Image = ShareXResources.IsDarkTheme ? Resources.edit_alignment_center_white : Resources.edit_alignment_center;
                    break;
                case StringAlignment.Far:
                    btnAlignmentHorizontal.Image = ShareXResources.IsDarkTheme ? Resources.edit_alignment_right_white : Resources.edit_alignment_right;
                    break;
            }
        }

        private void UpdateVerticalAlignmentImage()
        {
            switch (Options.AlignmentVertical)
            {
                default:
                case StringAlignment.Near:
                    btnAlignmentVertical.Image = ShareXResources.IsDarkTheme ? Resources.edit_vertical_alignment_top_white : Resources.edit_vertical_alignment_top;
                    break;
                case StringAlignment.Center:
                    btnAlignmentVertical.Image = ShareXResources.IsDarkTheme ? Resources.edit_vertical_alignment_middle_white : Resources.edit_vertical_alignment_middle;
                    break;
                case StringAlignment.Far:
                    btnAlignmentVertical.Image = ShareXResources.IsDarkTheme ? Resources.edit_vertical_alignment_white : Resources.edit_vertical_alignment;
                    break;
            }
        }
    }
}