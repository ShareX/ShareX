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
using ScreenCaptureLib;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace ShareX
{
    public partial class ScreenColorPicker : DialogColor
    {
        private Timer colorTimer = new Timer { Interval = 10 };
        private SurfaceOptions surfaceOptions;

        public ScreenColorPicker(TaskSettings taskSettings)
        {
            if (taskSettings != null)
            {
                surfaceOptions = taskSettings.CaptureSettings.SurfaceOptions;
            }
            else
            {
                surfaceOptions = new SurfaceOptions();
            }

            InitializeComponent();
            colorPicker.DrawCrosshair = true;
            colorTimer.Tick += colorTimer_Tick;

            UpdateControls(true);

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
            string text = string.Empty;

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
                btnColorPicker.Text = "Stop screen color picker";
            }
            else
            {
                btnColorPicker.Text = "Start screen color picker";
            }

            lblScreenColorPickerTip.Visible = colorTimerEnable;

            TopMost = colorTimerEnable;
        }

        private void btnColorPicker_Click(object sender, EventArgs e)
        {
            if (!colorTimer.Enabled) SetCurrentColor(NewColor);
            UpdateControls(!colorTimer.Enabled);
        }

        private void btnPipette_Click(object sender, EventArgs e)
        {
            try
            {
                SetCurrentColor(NewColor);
                UpdateControls(false);

                Hide();
                Thread.Sleep(100);

                PointInfo pointInfo = TaskHelpers.SelectPointColor(surfaceOptions);

                if (pointInfo != null)
                {
                    UpdateColor(pointInfo.Position.X, pointInfo.Position.Y, pointInfo.Color);
                }
            }
            finally
            {
                this.ShowActivate();
            }
        }

        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            string colors = colorPicker.SelectedColor.ToString();
            colors += string.Format("\r\nCursor position (X, Y) = {0}, {1}", txtX.Text, txtY.Text);
            ClipboardHelpers.CopyText(colors);
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
                if (!colorTimer.Enabled) SetCurrentColor(NewColor);
                UpdateControls(!colorTimer.Enabled);
                e.SuppressKeyPress = true;
            }
        }
    }
}