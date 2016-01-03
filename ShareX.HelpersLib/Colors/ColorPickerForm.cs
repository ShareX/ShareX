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

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class ColorPickerForm : BaseForm
    {
        public MyColor NewColor { get; protected set; }
        public MyColor OldColor { get; private set; }

        private bool oldColorExist;
        private bool controlChangingColor;

        public ColorPickerForm()
            : this(Color.Empty)
        {
        }

        public ColorPickerForm(Color currentColor)
        {
            NewColor = Color.Red;
            Initialize(currentColor);
        }

        public static Color GetColor(Color currentColor)
        {
            using (ColorPickerForm dialog = new ColorPickerForm(currentColor))
            {
                dialog.rbSaturation.Checked = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.NewColor;
                }
            }

            return currentColor;
        }

        private void Initialize(Color currentColor)
        {
            InitializeComponent();

            if (currentColor.IsEmpty)
            {
                colorPicker.DrawCrosshair = lblOld.Visible = oldColorExist;
                DrawPreviewColors();
            }
            else
            {
                SetCurrentColor(currentColor);
            }
        }

        public void SetCurrentColor(Color currentColor)
        {
            oldColorExist = true;
            colorPicker.DrawCrosshair = lblOld.Visible = oldColorExist;
            NewColor = OldColor = currentColor;
            colorPicker.ChangeColor(currentColor);
            nudAlpha.Value = currentColor.A;
            DrawPreviewColors();
        }

        private void UpdateControls(MyColor color, ColorType type)
        {
            DrawPreviewColors();
            controlChangingColor = true;

            if (type != ColorType.HSB)
            {
                nudHue.Value = (decimal)Math.Round(color.HSB.Hue360);
                nudSaturation.Value = (decimal)Math.Round(color.HSB.Saturation100);
                nudBrightness.Value = (decimal)Math.Round(color.HSB.Brightness100);
            }

            if (type != ColorType.RGBA)
            {
                nudRed.Value = color.RGBA.Red;
                nudGreen.Value = color.RGBA.Green;
                nudBlue.Value = color.RGBA.Blue;
                nudAlpha.Value = color.RGBA.Alpha;
            }

            if (type != ColorType.CMYK)
            {
                nudCyan.Value = (decimal)color.CMYK.Cyan100;
                nudMagenta.Value = (decimal)color.CMYK.Magenta100;
                nudYellow.Value = (decimal)color.CMYK.Yellow100;
                nudKey.Value = (decimal)color.CMYK.Key100;
            }

            if (type != ColorType.Hex)
            {
                txtHex.Text = ColorHelpers.ColorToHex(color);
            }

            if (type != ColorType.Decimal)
            {
                txtDecimal.Text = ColorHelpers.ColorToDecimal(color).ToString();
            }

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

        #region Events

        private void colorPicker_ColorChanged(object sender, ColorEventArgs e)
        {
            NewColor = e.Color;
            UpdateControls(NewColor, e.ColorType);
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
                if (!controlChangingColor)
                {
                    colorPicker.ChangeColor(ColorHelpers.DecimalToColor(Convert.ToInt32(txtDecimal.Text)), ColorType.Decimal);
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

        #endregion Events
    }
}