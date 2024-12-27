#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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

namespace ShareX.HelpersLib.Colors;

[DefaultEvent("ColorChanged")]
public class ColorPicker : UserControl
{
    public event ColorEventHandler ColorChanged;

    private MyColor selectedColor;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MyColor SelectedColor
    {
        get => selectedColor;
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

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DrawStyle DrawStyle
    {
        get => drawStyle;
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

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool CrosshairVisible
    {
        set
        {
            colorBox.CrosshairVisible = value;
            colorSlider.CrosshairVisible = value;
        }
    }

    private ColorBox colorBox;
    private ColorSlider colorSlider;

    public ColorPicker()
    {
        InitializeComponent();
        DrawStyle = DrawStyle.Hue;
        colorBox.ColorChanged += ColorBox_ColorChanged;
        colorSlider.ColorChanged += ColorSlider_ColorChanged;
    }

    private void ColorBox_ColorChanged(object sender, ColorEventArgs e)
    {
        selectedColor = e.Color;
        colorSlider.SelectedColor = SelectedColor;
        OnColorChanged();
    }

    private void ColorSlider_ColorChanged(object sender, ColorEventArgs e)
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

    private void OnColorChanged(ColorType colorType = ColorType.None) => ColorChanged?.Invoke(this, new ColorEventArgs(SelectedColor, colorType));

    #region Component Designer generated code

    private IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        colorBox = new HelpersLib.Colors.ColorBox();
        colorSlider = new ColorSlider();
        SuspendLayout();
        //
        // colorBox
        //
        colorBox.BorderStyle = BorderStyle.FixedSingle;
        colorBox.DrawStyle = DrawStyle.Hue;
        colorBox.Location = new Point(0, 0);
        colorBox.Name = "colorBox";
        colorBox.Size = new Size(258, 258);
        colorBox.TabIndex = 0;
        //
        // colorSlider
        //
        colorSlider.BorderStyle = BorderStyle.FixedSingle;
        colorSlider.DrawStyle = DrawStyle.Hue;
        colorSlider.Location = new Point(257, 0);
        colorSlider.Name = "colorSlider";
        colorSlider.Size = new Size(32, 258);
        colorSlider.TabIndex = 1;
        //
        // ColorPicker
        //
        AutoSize = true;
        Controls.Add(colorBox);
        Controls.Add(colorSlider);
        Name = "ColorPicker";
        Size = new Size(292, 261);
        ResumeLayout(false);
    }

    #endregion Component Designer generated code
}