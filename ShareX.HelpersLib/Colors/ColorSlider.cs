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

using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShareX.HelpersLib
{
    public class ColorSlider : ColorUserControl
    {
        public ColorSlider()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            Name = "ColorSlider";
            ClientSize = new Size(30, 256);
            base.Initialize();
        }

        protected override void DrawCrosshair(Graphics g)
        {
            DrawCrosshair(g, Pens.Black, 3, 11);
            DrawCrosshair(g, Pens.White, 4, 9);
        }

        private void DrawCrosshair(Graphics g, Pen pen, int offset, int height)
        {
            g.DrawRectangleProper(pen, new Rectangle(offset, lastPos.Y - (height / 2), clientWidth - (offset * 2), height));
        }

        // Y = Hue 360 -> 0
        protected override void DrawHue()
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                HSB color = new HSB(0.0, 1.0, 1.0, SelectedColor.RGBA.Alpha);

                for (int y = 0; y < clientHeight; y++)
                {
                    color.Hue = 1.0 - ((double)y / (clientHeight - 1));

                    using (Pen pen = new Pen(color))
                    {
                        g.DrawLine(pen, 0, y, clientWidth, y);
                    }
                }
            }
        }

        // Y = Saturation 100 -> 0
        protected override void DrawSaturation()
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                HSB start = new HSB(SelectedColor.HSB.Hue, 1.0, SelectedColor.HSB.Brightness, SelectedColor.RGBA.Alpha);
                HSB end = new HSB(SelectedColor.HSB.Hue, 0.0, SelectedColor.HSB.Brightness, SelectedColor.RGBA.Alpha);

                using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, clientWidth, clientHeight), start, end, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, new Rectangle(0, 0, clientWidth, clientHeight));
                }
            }
        }

        // Y = Brightness 100 -> 0
        protected override void DrawBrightness()
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                HSB start = new HSB(SelectedColor.HSB.Hue, SelectedColor.HSB.Saturation, 1.0, SelectedColor.RGBA.Alpha);
                HSB end = new HSB(SelectedColor.HSB.Hue, SelectedColor.HSB.Saturation, 0.0, SelectedColor.RGBA.Alpha);

                using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, clientWidth, clientHeight), start, end, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, new Rectangle(0, 0, clientWidth, clientHeight));
                }
            }
        }

        // Y = Red 255 -> 0
        protected override void DrawRed()
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                RGBA start = new RGBA(255, SelectedColor.RGBA.Green, SelectedColor.RGBA.Blue, SelectedColor.RGBA.Alpha);
                RGBA end = new RGBA(0, SelectedColor.RGBA.Green, SelectedColor.RGBA.Blue, SelectedColor.RGBA.Alpha);

                using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, clientWidth, clientHeight), start, end, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, new Rectangle(0, 0, clientWidth, clientHeight));
                }
            }
        }

        // Y = Green 255 -> 0
        protected override void DrawGreen()
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                RGBA start = new RGBA(SelectedColor.RGBA.Red, 255, SelectedColor.RGBA.Blue, SelectedColor.RGBA.Alpha);
                RGBA end = new RGBA(SelectedColor.RGBA.Red, 0, SelectedColor.RGBA.Blue, SelectedColor.RGBA.Alpha);

                using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, clientWidth, clientHeight), start, end, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, new Rectangle(0, 0, clientWidth, clientHeight));
                }
            }
        }

        // Y = Blue 255 -> 0
        protected override void DrawBlue()
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                RGBA start = new RGBA(SelectedColor.RGBA.Red, SelectedColor.RGBA.Green, 255, SelectedColor.RGBA.Alpha);
                RGBA end = new RGBA(SelectedColor.RGBA.Red, SelectedColor.RGBA.Green, 0, SelectedColor.RGBA.Alpha);

                using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, clientWidth, clientHeight), start, end, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, new Rectangle(0, 0, clientWidth, clientHeight));
                }
            }
        }
    }
}