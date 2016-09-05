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
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace ShareX
{
    public partial class ScreenColorPicker : ColorPickerForm
    {
        private Timer colorTimer = new Timer { Interval = 10 };

        public ScreenColorPicker()
        {
            InitializeComponent();
            btnOK.Visible = false;
            btnCancel.Text = Resources.ScreenColorPicker_ScreenColorPicker_Close;
            colorPicker.DrawCrosshair = true;
            colorTimer.Tick += colorTimer_Tick;

            UpdateControls(false);

            foreach (Control control in Controls)
            {
                if (control is NumericUpDown || control is TextBox)
                {
                    control.DoubleClick += CopyToClipboard;
                }
            }
        }

        private void CopyToClipboard(object sender, EventArgs e)
        {
            string text = "";

            if (sender is NumericUpDown)
            {
                text = ((NumericUpDown)sender).Value.ToString();
            }
            else if (sender is TextBox)
            {
                text = ((TextBox)sender).Text;
            }

            if (!string.IsNullOrEmpty(text))
            {
                ClipboardHelpers.CopyText(text);
            }
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

        private void UpdateControls(bool colorTimerEnable)
        {
            colorTimer.Enabled = colorTimerEnable;

            if (colorTimerEnable)
            {
                btnColorPicker.Text = Resources.ScreenColorPicker_UpdateControls_Stop_screen_color_picker;
            }
            else
            {
                btnColorPicker.Text = Resources.ScreenColorPicker_UpdateControls_Start_screen_color_picker;
            }

            lblScreenColorPickerTip.Visible = colorTimerEnable;
            TopMost = colorTimerEnable;
        }

        private void btnColorPicker_Click(object sender, EventArgs e)
        {
            if (!colorTimer.Enabled) SetCurrentColor(NewColor, true);
            UpdateControls(!colorTimer.Enabled);
        }

        private void btnPipette_Click(object sender, EventArgs e)
        {
            try
            {
                SetCurrentColor(NewColor, true);
                UpdateControls(false);

                Hide();
                Thread.Sleep(250);

                TaskSettings taskSettings = TaskSettings.GetDefaultTaskSettings();
                PointInfo pointInfo = RegionCaptureTasks.GetPointInfo(taskSettings.CaptureSettings.SurfaceOptions);

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

        private void colorTimer_Tick(object sender, EventArgs e)
        {
            Point position = CaptureHelpers.GetCursorPosition();
            UpdateColor(position.X, position.Y);
        }

        private void ScreenColorPicker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey && !txtHex.Focused)
            {
                btnColorPicker.Focus();
                if (!colorTimer.Enabled) SetCurrentColor(NewColor, true);
                UpdateControls(!colorTimer.Enabled);
                e.SuppressKeyPress = true;
            }
        }

        private void tsmiCopyAll_Click(object sender, EventArgs e)
        {
            string colors = colorPicker.SelectedColor.ToString();
            colors += Environment.NewLine + string.Format(Resources.ScreenColorPicker_btnCopyAll_Click_Cursor_position, txtX.Text, txtY.Text);
            ClipboardHelpers.CopyText(colors);
        }

        private void tsmiCopyRGB_Click(object sender, EventArgs e)
        {
            RGBA rgba = colorPicker.SelectedColor.RGBA;
            ClipboardHelpers.CopyText($"{rgba.Red}, {rgba.Green}, {rgba.Blue}");
        }

        private void tsmiCopyHex_Click(object sender, EventArgs e)
        {
            string hex = ColorHelpers.ColorToHex(colorPicker.SelectedColor, ColorFormat.RGB);
            ClipboardHelpers.CopyText("#" + hex);
        }

        private void tsmiCopyHSB_Click(object sender, EventArgs e)
        {
            HSB hsb = colorPicker.SelectedColor.HSB;
            ClipboardHelpers.CopyText($"{hsb.Hue360:0.0}°, {hsb.Saturation100:0.0}%, {hsb.Brightness100:0.0}%");
        }

        private void tsmiCopyCMYK_Click(object sender, EventArgs e)
        {
            CMYK cmyk = colorPicker.SelectedColor.CMYK;
            ClipboardHelpers.CopyText($"{cmyk.Cyan100:0.0}%, {cmyk.Magenta100:0.0}%, {cmyk.Yellow100:0.0}%, {cmyk.Key100:0.0}%");
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
    }
}