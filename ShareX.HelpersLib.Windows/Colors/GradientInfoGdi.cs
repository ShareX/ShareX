#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace ShareX.HelpersLib
{
    /// <summary>GDI+ drawing for GradientInfo, used by the WinForms application until its image code moves to SkiaSharp.</summary>
    public static class GradientInfoGdi
    {
        public static ColorBlend GetColorBlend(this GradientInfo gradient)
        {
            List<GradientStop> colors = gradient.GetNormalizedStops();

            return new ColorBlend
            {
                Colors = colors.Select(x => x.Color).ToArray(),
                Positions = colors.Select(x => x.Location / 100).ToArray()
            };
        }

        public static LinearGradientBrush GetGradientBrush(this GradientInfo gradient, Rectangle rect)
        {
            LinearGradientBrush brush = new LinearGradientBrush(rect, Color.Transparent, Color.Transparent, (LinearGradientMode)gradient.Type);
            brush.InterpolationColors = gradient.GetColorBlend();
            return brush;
        }

        public static void Draw(this GradientInfo gradient, Graphics g, Rectangle rect)
        {
            if (gradient.IsValid)
            {
                try
                {
                    using (LinearGradientBrush brush = gradient.GetGradientBrush(new Rectangle(0, 0, rect.Width, rect.Height)))
                    {
                        g.FillRectangle(brush, rect);
                    }
                }
                catch
                {
                }
            }
        }

        public static void Draw(this GradientInfo gradient, Image img)
        {
            if (gradient.IsValid)
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    gradient.Draw(g, new Rectangle(0, 0, img.Width, img.Height));
                }
            }
        }

        public static Bitmap CreateGradientPreview(this GradientInfo gradient, int width, int height, bool border = false, bool checkers = false)
        {
            Bitmap bmp = new Bitmap(width, height);
            Rectangle rect = new Rectangle(0, 0, width, height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                if (checkers && gradient.IsTransparent)
                {
                    using (Image checker = ImageHelpers.CreateCheckerPattern())
                    using (Brush checkerBrush = new TextureBrush(checker, WrapMode.Tile))
                    {
                        g.FillRectangle(checkerBrush, rect);
                    }
                }

                gradient.Draw(g, rect);

                if (border)
                {
                    g.DrawRectangleProper(Pens.Black, rect);
                }
            }

            return bmp;
        }
    }
}
