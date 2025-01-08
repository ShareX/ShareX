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

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class ColorPickerForm : Form
    {
        public Func<PointInfo> OpenScreenColorPicker;

        public MyColor NewColor { get; private set; }
        public MyColor OldColor { get; private set; }
        public bool IsScreenColorPickerMode { get; private set; }
        public ColorPickerOptions Options { get; private set; }

        private bool oldColorExist;
        private bool controlChangingColor;
        private ControlHider clipboardStatusHider;

        public ColorPickerForm(Color currentColor, bool isScreenColorPickerMode = false, bool checkClipboard = true, ColorPickerOptions options = null)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);
            clipboardStatusHider = new ControlHider(btnClipboardStatus, 2000);

            IsScreenColorPickerMode = isScreenColorPickerMode;
            Options = options;

            if (Options == null)
            {
                Options = new ColorPickerOptions();
            }

            if (Options.RecentColorsSelected)
            {
                rbRecentColors.Checked = true;
            }
            else
            {
                rbStandardColors.Checked = true;
            }

            PrepareColorPalette();
            SetCurrentColor(currentColor, !IsScreenColorPickerMode);

            if (checkClipboard)
            {
                CheckClipboard();
            }

            btnOK.Visible = btnCancel.Visible = !IsScreenColorPickerMode;
            mbCopy.Visible = btnClose.Visible = pCursorPosition.Visible = IsScreenColorPickerMode;
        }

        public void EnableScreenColorPickerButton(Func<PointInfo> openScreenColorPicker)
        {
            OpenScreenColorPicker = openScreenColorPicker;
            btnScreenColorPicker.Visible = true;
        }

        public bool CheckClipboard()
        {
            string text = ClipboardHelpers.GetText(true);

            if (!string.IsNullOrEmpty(text))
            {
                text = text.Trim();

                if (ColorHelpers.ParseColor(text, out Color clipboardColor))
                {
                    colorPicker.ChangeColor(clipboardColor);
                    btnClipboardStatus.Text = "Clipboard: " + text;
                    btnClipboardStatus.Location = new Point(btnClipboardColorPicker.Left + (btnClipboardColorPicker.Width / 2) - (btnClipboardStatus.Width / 2),
                        btnClipboardColorPicker.Top - btnClipboardStatus.Height - 5);
                    clipboardStatusHider.Show();
                    return true;
                }
            }

            return false;
        }

        public static bool PickColor(Color currentColor, out Color newColor, Form owner = null, Func<PointInfo> openScreenColorPicker = null, ColorPickerOptions options = null)
        {
            using (ColorPickerForm dialog = new ColorPickerForm(currentColor, options: options))
            {
                if (openScreenColorPicker != null)
                {
                    dialog.EnableScreenColorPickerButton(openScreenColorPicker);
                }

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                {
                    newColor = dialog.NewColor;
                    return true;
                }
            }

            newColor = currentColor;
            return false;
        }

        private void PrepareColorPalette()
        {
            flpColorPalette.Controls.Clear();

            Color[] colors;

            if (Options.RecentColorsSelected)
            {
                colors = HelpersOptions.RecentColors.ToArray();
            }
            else
            {
                colors = ColorHelpers.StandardColors;
            }

            int length = Math.Min(colors.Length, HelpersOptions.RecentColorsMax);

            Color previousColor = Color.Empty;

            for (int i = 0; i < length; i++)
            {
                ColorButton colorButton = new ColorButton()
                {
                    Color = colors[i],
                    Size = new Size(16, 16),
                    Margin = new Padding(1),
                    BorderColor = Color.FromArgb(100, 100, 100),
                    Offset = 0,
                    HoverEffect = true,
                    ManualButtonClick = true
                };

                colorButton.MouseClick += (sender, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        SetCurrentColor(colorButton.Color, true);

                        if (!IsScreenColorPickerMode)
                        {
                            if (!previousColor.IsEmpty && previousColor == colorButton.Color)
                            {
                                CloseOK();
                            }
                            else
                            {
                                previousColor = colorButton.Color;
                            }
                        }
                    }
                };

                flpColorPalette.Controls.Add(colorButton);
                if ((i + 1) % 16 == 0) flpColorPalette.SetFlowBreak(colorButton, true);
            }
        }

        private void AddRecentColor(Color color)
        {
            HelpersOptions.RecentColors.Remove(color);

            if (HelpersOptions.RecentColors.Count >= HelpersOptions.RecentColorsMax)
            {
                HelpersOptions.RecentColors.RemoveRange(HelpersOptions.RecentColorsMax - 1, HelpersOptions.RecentColors.Count - HelpersOptions.RecentColorsMax + 1);
            }

            HelpersOptions.RecentColors.Insert(0, color);
        }

        public void SetCurrentColor(Color currentColor, bool keepPreviousColor)
        {
            oldColorExist = keepPreviousColor;
            lblOld.Visible = oldColorExist;
            NewColor = OldColor = currentColor;
            colorPicker.ChangeColor(currentColor);
            nudAlpha.SetValue(currentColor.A);
            DrawPreviewColors();
        }

        private void UpdateColor(int x, int y)
        {
            UpdateColor(x, y, CaptureHelpers.GetPixelColor(x, y));
        }

        private void UpdateColor(int x, int y, Color color)
        {
            txtX.Text = x.ToString();
            txtY.Text = y.ToString();
            colorPicker.ChangeColor(color);
        }

        private void UpdateControls(MyColor color, ColorType type)
        {
            DrawPreviewColors();
            controlChangingColor = true;

            if (type != ColorType.HSB)
            {
                nudHue.SetValue((decimal)Math.Round(color.HSB.Hue360));
                nudSaturation.SetValue((decimal)Math.Round(color.HSB.Saturation100));
                nudBrightness.SetValue((decimal)Math.Round(color.HSB.Brightness100));
            }

            if (type != ColorType.RGBA)
            {
                nudRed.SetValue(color.RGBA.Red);
                nudGreen.SetValue(color.RGBA.Green);
                nudBlue.SetValue(color.RGBA.Blue);
                nudAlpha.SetValue(color.RGBA.Alpha);
            }

            if (type != ColorType.CMYK)
            {
                nudCyan.SetValue((decimal)color.CMYK.Cyan100);
                nudMagenta.SetValue((decimal)color.CMYK.Magenta100);
                nudYellow.SetValue((decimal)color.CMYK.Yellow100);
                nudKey.SetValue((decimal)color.CMYK.Key100);
            }

            if (type != ColorType.Hex)
            {
                txtHex.Text = ColorHelpers.ColorToHex(color);
            }

            if (type != ColorType.Decimal)
            {
                txtDecimal.Text = ColorHelpers.ColorToDecimal(color).ToString();
            }

            lblNameValue.Text = ColorHelpers.GetColorName(color);

            controlChangingColor = false;
        }

        private void DrawPreviewColors()
        {
            Bitmap bmp = new Bitmap(pbColorPreview.ClientSize.Width, pbColorPreview.ClientSize.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                int bmpHeight = bmp.Height;

                if (oldColorExist)
                {
                    bmpHeight /= 2;

                    using (SolidBrush oldColorBrush = new SolidBrush(OldColor))
                    {
                        g.FillRectangle(oldColorBrush, new Rectangle(0, bmpHeight, bmp.Width, bmpHeight));
                    }
                }

                using (SolidBrush newColorBrush = new SolidBrush(NewColor))
                {
                    g.FillRectangle(newColorBrush, new Rectangle(0, 0, bmp.Width, bmpHeight));
                }
            }

            using (bmp)
            {
                pbColorPreview.LoadImage(bmp);
            }
        }

        private void CloseOK()
        {
            AddRecentColor(NewColor);
            DialogResult = DialogResult.OK;
            Close();
        }

        #region Events

        private void ColorPickerForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void colorPicker_ColorChanged(object sender, ColorEventArgs e)
        {
            NewColor = e.Color;
            UpdateControls(NewColor, e.ColorType);
        }

        private void rbRecentColors_CheckedChanged(object sender, EventArgs e)
        {
            Options.RecentColorsSelected = rbRecentColors.Checked;

            PrepareColorPalette();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            CloseOK();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void rbHue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbHue.Checked) colorPicker.DrawStyle = DrawStyle.Hue;
        }

        private void rbSaturation_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSaturation.Checked) colorPicker.DrawStyle = DrawStyle.Saturation;
        }

        private void rbBrightness_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBrightness.Checked) colorPicker.DrawStyle = DrawStyle.Brightness;
        }

        private void rbRed_CheckedChanged(object sender, EventArgs e)
        {
            if (rbRed.Checked) colorPicker.DrawStyle = DrawStyle.Red;
        }

        private void rbGreen_CheckedChanged(object sender, EventArgs e)
        {
            if (rbGreen.Checked) colorPicker.DrawStyle = DrawStyle.Green;
        }

        private void rbBlue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBlue.Checked) colorPicker.DrawStyle = DrawStyle.Blue;
        }

        private void RGB_ValueChanged(object sender, EventArgs e)
        {
            if (!controlChangingColor)
            {
                colorPicker.ChangeColor(Color.FromArgb((int)nudAlpha.Value, (int)nudRed.Value, (int)nudGreen.Value, (int)nudBlue.Value), ColorType.RGBA);
            }
        }

        private void cbTransparent_Click(object sender, EventArgs e)
        {
            if (nudAlpha.Value == 0)
            {
                nudAlpha.SetValue(255);
            }
            else
            {
                nudAlpha.SetValue(0);
            }
        }

        private void HSB_ValueChanged(object sender, EventArgs e)
        {
            if (!controlChangingColor)
            {
                colorPicker.ChangeColor(new HSB((int)nudHue.Value, (int)nudSaturation.Value, (int)nudBrightness.Value, (int)nudAlpha.Value).ToColor(), ColorType.HSB);
            }
        }

        private void CMYK_ValueChanged(object sender, EventArgs e)
        {
            if (!controlChangingColor)
            {
                colorPicker.ChangeColor(new CMYK((double)nudCyan.Value / 100, (double)nudMagenta.Value / 100, (double)nudYellow.Value / 100,
                    (double)nudKey.Value / 100, (int)nudAlpha.Value).ToColor(), ColorType.CMYK);
            }
        }

        private void txtHex_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!controlChangingColor)
                {
                    colorPicker.ChangeColor(ColorHelpers.HexToColor(txtHex.Text), ColorType.Hex);
                }
            }
            catch
            {
            }
        }

        private void txtDecimal_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!controlChangingColor && int.TryParse(txtDecimal.Text, out int dec))
                {
                    colorPicker.ChangeColor(ColorHelpers.DecimalToColor(dec), ColorType.Decimal);
                }
            }
            catch
            {
            }
        }

        private void pbColorPreview_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && oldColorExist)
            {
                colorPicker.ChangeColor(OldColor);
            }
        }

        private void tsmiCopyAll_Click(object sender, EventArgs e)
        {
            string colors = colorPicker.SelectedColor.ToString();
            colors += Environment.NewLine + string.Format("Cursor position (X, Y) = {0}, {1}", txtX.Text, txtY.Text);
            ClipboardHelpers.CopyText(colors);
        }

        private void tsmiCopyRGB_Click(object sender, EventArgs e)
        {
            RGBA rgba = colorPicker.SelectedColor.RGBA;
            ClipboardHelpers.CopyText($"{rgba.Red}, {rgba.Green}, {rgba.Blue}");
        }

        private void tsmiCopyHexadecimal_Click(object sender, EventArgs e)
        {
            string hex = ColorHelpers.ColorToHex(colorPicker.SelectedColor, ColorFormat.RGB);
            ClipboardHelpers.CopyText("#" + hex);
        }

        private void tsmiCopyCMYK_Click(object sender, EventArgs e)
        {
            CMYK cmyk = colorPicker.SelectedColor.CMYK;
            ClipboardHelpers.CopyText($"{cmyk.Cyan100:0.0}%, {cmyk.Magenta100:0.0}%, {cmyk.Yellow100:0.0}%, {cmyk.Key100:0.0}%");
        }

        private void tsmiCopyHSB_Click(object sender, EventArgs e)
        {
            HSB hsb = colorPicker.SelectedColor.HSB;
            ClipboardHelpers.CopyText($"{hsb.Hue360:0.0}°, {hsb.Saturation100:0.0}%, {hsb.Brightness100:0.0}%");
        }

        private void tsmiCopyDecimal_Click(object sender, EventArgs e)
        {
            int dec = ColorHelpers.ColorToDecimal(colorPicker.SelectedColor, ColorFormat.RGB);
            ClipboardHelpers.CopyText(dec.ToString());
        }

        private void tsmiCopyPosition_Click(object sender, EventArgs e)
        {
            ClipboardHelpers.CopyText($"{txtX.Text}, {txtY.Text}");
        }

        private void btnScreenColorPicker_Click(object sender, EventArgs e)
        {
            try
            {
                SetCurrentColor(NewColor, true);

                Hide();
                Thread.Sleep(250);

                PointInfo pointInfo = OpenScreenColorPicker();

                if (pointInfo != null)
                {
                    UpdateColor(pointInfo.Position.X, pointInfo.Position.Y, pointInfo.Color);
                }
            }
            finally
            {
                this.ForceActivate();
            }
        }

        private void btnClipboardColorPicker_Click(object sender, EventArgs e)
        {
            CheckClipboard();
        }

        #endregion Events
    }
}