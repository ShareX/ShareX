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

using System.Drawing;

namespace HelpersLib
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
            Size = new Size(30, 260);
            base.Initialize();
        }

        protected override void DrawCrosshair(Graphics g)
        {
            int rectOffset = 3;
            int rectSize = 4;

            g.DrawRectangle(Pens.Black, new Rectangle(rectOffset, lastPos.Y - rectSize, width - rectOffset * 2, rectSize * 2 + 1));
            g.DrawRectangle(Pens.White, new Rectangle(rectOffset + 1, lastPos.Y - rectSize + 1, width - rectOffset * 2 - 2, rectSize * 2 - 1));
        }

        // Y = Hue 360 -> 0
        protected override void DrawHue()
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                HSB color = new HSB(0.0, 1.0, 1.0, SelectedColor.RGBA.Alpha);

                for (int y = 0; y < height; y++)
                {
                    color.Hue = 1.0 - (double)y / (height - 1);

                    using (Pen pen = new Pen(color))
                    {
                        g.DrawLine(pen, 0, y, width, y);
                    }
                }
            }
        }

        // Y = Saturation 100 -> 0
        protected override void DrawSaturation()
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                HSB color = new HSB(SelectedColor.HSB.Hue, 0.0, SelectedColor.HSB.Brightness, SelectedColor.RGBA.Alpha);

                for (int y = 0; y < height; y++)
                {
                    color.Saturation = 1.0 - (double)y / (height - 1);

                    using (Pen pen = new Pen(color))
                    {
                        g.DrawLine(pen, 0, y, width, y);
                    }
                }
            }
        }

        // Y = Brightness 100 -> 0
        protected override void DrawBrightness()
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                HSB color = new HSB(SelectedColor.HSB.Hue, SelectedColor.HSB.Saturation, 0.0, SelectedColor.RGBA.Alpha);

                for (int y = 0; y < height; y++)
                {
                    color.Brightness = 1.0 - (double)y / (height - 1);

                    using (Pen pen = new Pen(color))
                    {
                        g.DrawLine(pen, 0, y, width, y);
                    }
                }
            }
        }

        // Y = Red 255 -> 0
        protected override void DrawRed()
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                RGBA color = new RGBA(0, SelectedColor.RGBA.Green, SelectedColor.RGBA.Blue, SelectedColor.RGBA.Alpha);

                for (int y = 0; y < height; y++)
                {
                    color.Red = 255 - Round(255 * (double)y / (height - 1));

                    using (Pen pen = new Pen(color))
                    {
                        g.DrawLine(pen, 0, y, width, y);
                    }
                }
            }
        }

        // Y = Green 255 -> 0
        protected override void DrawGreen()
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                RGBA color = new RGBA(SelectedColor.RGBA.Red, 0, SelectedColor.RGBA.Blue, SelectedColor.RGBA.Alpha);

                for (int y = 0; y < height; y++)
                {
                    color.Green = 255 - Round(255 * (double)y / (height - 1));

                    using (Pen pen = new Pen(color))
                    {
                        g.DrawLine(pen, 0, y, width, y);
                    }
                }
            }
        }

        // Y = Blue 255 -> 0
        protected override void DrawBlue()
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                RGBA color = new RGBA(SelectedColor.RGBA.Red, SelectedColor.RGBA.Green, 0, SelectedColor.RGBA.Alpha);

                for (int y = 0; y < height; y++)
                {
                    color.Blue = 255 - Round(255 * (double)y / (height - 1));

                    using (Pen pen = new Pen(color))
                    {
                        g.DrawLine(pen, 0, y, width, y);
                    }
                }
            }
        }
    }
}