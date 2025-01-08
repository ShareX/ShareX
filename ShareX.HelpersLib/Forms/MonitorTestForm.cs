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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class MonitorTestForm : Form
    {
        public MonitorTestForm()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            Rectangle screenBounds = CaptureHelpers.GetScreenBounds();
            Location = screenBounds.Location;
            Size = screenBounds.Size;

            rbBlackWhite.Checked = true;
            tbBlackWhite.Value = 255;
            tbRed.Value = 255;
            cbGradient.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<LinearGradientMode>());
            cbGradient.SelectedIndex = 1;
            btnGradientColor1.Color = Color.White;
            btnGradientColor2.Color = Color.Black;
            cbShapes.SelectedIndex = 0;
            tbShapeSize.Value = 5;
        }

        private void SetBackColor()
        {
            SetBackColor(Color.White);
        }

        private void SetBackColor(Color color)
        {
            if (BackgroundImage != null)
            {
                Image temp = BackgroundImage;
                BackgroundImage = null;
                temp.Dispose();
            }

            BackColor = color;
        }

        private void DrawBlackWhite(int value)
        {
            Color color = Color.FromArgb(value, value, value);
            SetBackColor(color);
        }

        private void DrawRedGreenBlue(int red, int green, int blue)
        {
            Color color = Color.FromArgb(red, green, blue);
            SetBackColor(color);
        }

        private Bitmap DrawGradient(Color fromColor, Color toColor, LinearGradientMode gradientMode)
        {
            Bitmap bmp = new Bitmap(Width, Height);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, fromColor, toColor, gradientMode))
            {
                g.FillRectangle(brush, rect);
            }

            return bmp;
        }

        private Bitmap DrawHorizontalLine(int size, Color color)
        {
            Bitmap bmp = new Bitmap(1, size * 2);

            for (int i = 0; i < size; i++)
            {
                bmp.SetPixel(0, i, color);
            }

            return bmp;
        }

        private Bitmap DrawVerticalLine(int size, Color color)
        {
            Bitmap bmp = new Bitmap(size * 2, 1);

            for (int i = 0; i < size; i++)
            {
                bmp.SetPixel(i, 0, color);
            }

            return bmp;
        }

        private Bitmap DrawChecker(int size, Color color)
        {
            Bitmap bmp = new Bitmap(size * 2, size * 2);

            using (Graphics g = Graphics.FromImage(bmp))
            using (Brush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, new Rectangle(0, 0, size, size));
                g.FillRectangle(brush, new Rectangle(size, size, size, size));
            }

            return bmp;
        }

        private void DrawBlackWhite()
        {
            if (rbBlackWhite.Checked)
            {
                DrawBlackWhite(tbBlackWhite.Value);
            }
        }

        private void DrawRedGreenBlue()
        {
            if (rbRedGreenBlue.Checked)
            {
                DrawRedGreenBlue(tbRed.Value, tbGreen.Value, tbBlue.Value);
            }
        }

        private void DrawGradient()
        {
            if (rbGradient.Checked)
            {
                SetBackColor();

                Color color1 = btnGradientColor1.Color;
                Color color2 = btnGradientColor2.Color;
                LinearGradientMode gradientMode = (LinearGradientMode)cbGradient.SelectedIndex;
                BackgroundImage = DrawGradient(color1, color2, gradientMode);
            }
        }

        private void DrawSelectedShape()
        {
            if (rbShapes.Checked)
            {
                SetBackColor();

                int shapeSize = Math.Max(tbShapeSize.Value, 1);

                switch (cbShapes.SelectedIndex)
                {
                    case 0:
                        BackgroundImage = DrawHorizontalLine(shapeSize, Color.Black);
                        break;
                    case 1:
                        BackgroundImage = DrawVerticalLine(shapeSize, Color.Black);
                        break;
                    case 2:
                        BackgroundImage = DrawChecker(shapeSize, Color.Black);
                        break;
                }
            }
        }

        #region Form events

        private void MonitorTestForm_MouseDown(object sender, MouseEventArgs e)
        {
            bool visible = !pSettings.Visible;
            if (visible) pSettings.Location = e.Location;
            pSettings.Visible = visible;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void rbBlackWhite_CheckedChanged(object sender, EventArgs e)
        {
            DrawBlackWhite();
        }

        private void tbBlackWhite_ValueChanged(object sender, EventArgs e)
        {
            lblBlackWhiteValue.Text = tbBlackWhite.Value.ToString();
            DrawBlackWhite();
        }

        private void rbRedGreenBlue_CheckedChanged(object sender, EventArgs e)
        {
            DrawRedGreenBlue();
        }

        private void btnColorDialog_Click(object sender, EventArgs e)
        {
            Color currentColor = Color.FromArgb(tbRed.Value, tbGreen.Value, tbBlue.Value);

            if (ColorPickerForm.PickColor(currentColor, out Color newColor, this))
            {
                tbRed.Value = newColor.R;
                tbGreen.Value = newColor.G;
                tbBlue.Value = newColor.B;
                DrawRedGreenBlue();
            }
        }

        private void tbRedGreenBlue_ValueChanged(object sender, EventArgs e)
        {
            lblRedValue.Text = tbRed.Value.ToString();
            lblGreenValue.Text = tbGreen.Value.ToString();
            lblBlueValue.Text = tbBlue.Value.ToString();
            DrawRedGreenBlue();
        }

        private void rbGradient_CheckedChanged(object sender, EventArgs e)
        {
            DrawGradient();
        }

        private void cbGradient_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawGradient();
        }

        private void btnGradientColor1_ColorChanged(Color color)
        {
            DrawGradient();
        }

        private void btnGradientColor2_ColorChanged(Color color)
        {
            DrawGradient();
        }

        private void rbShapes_CheckedChanged(object sender, EventArgs e)
        {
            DrawSelectedShape();
        }

        private void cbShapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawSelectedShape();
        }

        private void tbShapeSize_ValueChanged(object sender, EventArgs e)
        {
            lblShapeSizeValue.Text = tbShapeSize.Value.ToString();
            DrawSelectedShape();
        }

        private void btnScreenTearingTest_Click(object sender, EventArgs e)
        {
            using (ScreenTearingTestForm screenTearingTestForm = new ScreenTearingTestForm())
            {
                screenTearingTestForm.ShowDialog();
            }
        }

        #endregion Form events
    }
}