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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    [DefaultEvent("ColorChanged")]
    public class ColorButton : Button
    {
        public delegate void ColorChangedEventHandler(Color color);
        public event ColorChangedEventHandler ColorChanged;

        private Color color;

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;

                OnColorChanged(color);

                Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "DarkGray")]
        public Color BorderColor { get; set; } = Color.DarkGray;

        [DefaultValue(3)]
        public int Offset { get; set; } = 3;

        [DefaultValue(false)]
        public bool HoverEffect { get; set; } = false;

        [DefaultValue(false)]
        public bool ManualButtonClick { get; set; }

        public ColorPickerOptions ColorPickerOptions { get; set; }

        private bool isMouseHover;

        protected void OnColorChanged(Color color)
        {
            ColorChanged?.Invoke(color);
        }

        protected override void OnMouseClick(MouseEventArgs mevent)
        {
            base.OnMouseClick(mevent);

            if (!ManualButtonClick)
            {
                ShowColorDialog();
            }
        }

        public void ShowColorDialog()
        {
            if (ColorPickerForm.PickColor(Color, out Color newColor, FindForm(), null, ColorPickerOptions))
            {
                Color = newColor;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isMouseHover = true;

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isMouseHover = false;

            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            int boxSize = ClientRectangle.Height - (Offset * 2);
            Rectangle boxRectangle = new Rectangle(ClientRectangle.Width - Offset - boxSize, Offset, boxSize, boxSize);

            Graphics g = pevent.Graphics;

            if (Color.IsTransparent())
            {
                using (Image checker = ImageHelpers.CreateCheckerPattern(boxSize, boxSize))
                {
                    g.DrawImage(checker, boxRectangle);
                }
            }

            if (Color.A > 0)
            {
                using (Brush brush = new SolidBrush(Color))
                {
                    g.FillRectangle(brush, boxRectangle);
                }
            }

            if (HoverEffect && isMouseHover)
            {
                using (Brush hoverBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 255)))
                {
                    g.FillRectangle(hoverBrush, boxRectangle);
                }
            }

            using (Pen borderPen = new Pen(BorderColor))
            {
                g.DrawRectangleProper(borderPen, boxRectangle);
            }
        }
    }
}