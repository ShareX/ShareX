#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace ShareX.HelpersLib
{
    public class GradientInfo
    {
        [DefaultValue(LinearGradientMode.Vertical)]
        public LinearGradientMode Type { get; set; }

        public List<GradientStop> Colors { get; set; }

        public bool IsValid
        {
            get
            {
                return Colors != null && Colors.Count > 0;
            }
        }

        public GradientInfo()
        {
            Type = LinearGradientMode.Vertical;
            Colors = new List<GradientStop>();
        }

        public void Draw(Graphics g, Rectangle rect)
        {
            if (IsValid)
            {
                try
                {
                    using (LinearGradientBrush brush = GetGradientBrush(new Rectangle(0, 0, rect.Width, rect.Height)))
                    {
                        g.FillRectangle(brush, rect);
                    }
                }
                catch
                {
                }
            }
        }

        public ColorBlend GetColorBlend()
        {
            List<GradientStop> colors = new List<GradientStop>(Colors.OrderBy(x => x.Location));

            if (!colors.Any(x => x.Location == 0))
            {
                colors.Insert(0, new GradientStop(colors[0].Color, 0f));
            }

            if (!colors.Any(x => x.Location == 100))
            {
                colors.Add(new GradientStop(colors[colors.Count - 1].Color, 100f));
            }

            ColorBlend colorBlend = new ColorBlend();
            colorBlend.Colors = colors.Select(x => x.Color).ToArray();
            colorBlend.Positions = colors.Select(x => x.Location / 100).ToArray();
            return colorBlend;
        }

        public LinearGradientBrush GetGradientBrush(Rectangle rect)
        {
            LinearGradientBrush brush = new LinearGradientBrush(rect, Color.Transparent, Color.Transparent, Type);
            brush.InterpolationColors = GetColorBlend();
            return brush;
        }

        public override string ToString()
        {
            return "Gradient";
        }
    }
}