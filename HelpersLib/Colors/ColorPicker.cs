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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HelpersLib
{
    [DefaultEvent("ColorChanged")]
    public class ColorPicker : UserControl
    {
        public event ColorEventHandler ColorChanged;

        private MyColor selectedColor;

        public MyColor SelectedColor
        {
            get
            {
                return selectedColor;
            }
            private set
            {
                if (selectedColor != value)
                {
                    selectedColor = value;
                    colorBox.SelectedColor = selectedColor;
                    colorSlider.SelectedColor = selectedColor;
                }
            }
        }

        private DrawStyle drawStyle;

        public DrawStyle DrawStyle
        {
            get
            {
                return drawStyle;
            }
            set
            {
                if (drawStyle != value)
                {
                    drawStyle = value;
                    colorBox.DrawStyle = value;
                    colorSlider.DrawStyle = value;
                }
            }
        }

        public bool DrawCrosshair
        {
            set
            {
                colorBox.drawCrosshair = value;
                colorSlider.drawCrosshair = value;
            }
        }

        private ColorBox colorBox;
        private ColorSlider colorSlider;

        public ColorPicker()
        {
            InitializeComponent();
            DrawStyle = DrawStyle.Hue;
            colorBox.ColorChanged += colorBox_ColorChanged;
            colorSlider.ColorChanged += colorSlider_ColorChanged;
        }

        private void colorBox_ColorChanged(object sender, ColorEventArgs e)
        {
            selectedColor = e.Color;
            colorSlider.SelectedColor = SelectedColor;
            OnColorChanged();
        }

        private void colorSlider_ColorChanged(object sender, ColorEventArgs e)
        {
            selectedColor = e.Color;
            colorBox.SelectedColor = SelectedColor;
            OnColorChanged();
        }

        public void ChangeColor(Color color, ColorType colorType = ColorType.None)
        {
            SelectedColor = color;
            OnColorChanged(colorType);
        }

        private void OnColorChanged(ColorType colorType = ColorType.None)
        {
            if (ColorChanged != null)
            {
                ColorChanged(this, new ColorEventArgs(SelectedColor, colorType));
            }
        }

        #region Component Designer generated code

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.colorBox = new HelpersLib.ColorBox();
            this.colorSlider = new HelpersLib.ColorSlider();

            // colorBox
            this.colorBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.colorBox.DrawStyle = HelpersLib.DrawStyle.Hue;
            this.colorBox.Location = new System.Drawing.Point(0, 0);
            this.colorBox.Name = "colorBox";
            this.colorBox.TabIndex = 0;

            // colorSlider
            this.colorSlider.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.colorSlider.DrawStyle = HelpersLib.DrawStyle.Hue;
            this.colorSlider.Location = new System.Drawing.Point(colorBox.Width, 0);
            this.colorSlider.Name = "colorSlider";
            this.colorSlider.TabIndex = 1;

            // ColorPicker
            this.Controls.Add(this.colorBox);
            this.Controls.Add(this.colorSlider);
            this.AutoSize = true;
            this.Name = "ColorPicker";
            this.Size = new System.Drawing.Size(colorBox.Width + colorSlider.Width, colorBox.Height);

            this.ResumeLayout(false);
        }

        #endregion Component Designer generated code
    }
}