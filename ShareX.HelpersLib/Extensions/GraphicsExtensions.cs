#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static class GraphicsExtensions
    {
        public static void DrawRectangleProper(this Graphics g, Pen pen, Rectangle rect)
        {
            if (pen.Width == 1)
            {
                rect = rect.SizeOffset(-1);
            }

            if (rect.Width > 0 && rect.Height > 0)
            {
                g.DrawRectangle(pen, rect);
            }
        }

        public static void DrawRectangleProper(this Graphics g, Pen pen, int x, int y, int width, int height)
        {
            DrawRectangleProper(g, pen, new Rectangle(x, y, width, height));
        }

        public static void DrawRectangleShadow(this Graphics g, Rectangle rect, Color shadowColor, int shadowDepth, int shadowMaxAlpha, int shadowMinAlpha, Padding shadowDirection)
        {
            for (int i = 0; i < shadowDepth; i++)
            {
                int currentAlpha = (int)MathHelpers.Lerp(shadowMaxAlpha, shadowMinAlpha, (float)i / (shadowDepth - 1));

                using (Pen pen = new Pen(Color.FromArgb(currentAlpha, shadowColor)))
                {
                    Rectangle shadowRect = new Rectangle(rect.X + -shadowDirection.Left * i, rect.Y + -shadowDirection.Top * i,
                        rect.Width + (shadowDirection.Left + shadowDirection.Right) * i, rect.Height + (shadowDirection.Top + shadowDirection.Bottom) * i);

                    g.DrawRectangleProper(pen, shadowRect);
                }
            }
        }

        public static void DrawRoundedRectangle(this Graphics g, Brush brush, Pen pen, Rectangle rect, float radius)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddRoundedRectangle(rect, radius);
                if (brush != null) g.FillPath(brush, gp);
                if (pen != null) g.DrawPath(pen, gp);
            }
        }

        public static void DrawRoundedRectangle(this Graphics g, Brush brush, Pen pen, int x, int y, int width, int height, float radius)
        {
            DrawRoundedRectangle(g, brush, pen, new Rectangle(x, y, width, height), radius);
        }

        public static void DrawDiamond(this Graphics g, Pen pen, Rectangle rect)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddDiamond(rect);
                g.DrawPath(pen, gp);
            }
        }

        public static void DrawCrossRectangle(this Graphics g, Pen pen, Rectangle rect, int crossSize)
        {
            rect = rect.SizeOffset(-1);

            if (rect.Width > 0 && rect.Height > 0)
            {
                // Top
                g.DrawLine(pen, rect.X - crossSize, rect.Y, rect.Right + crossSize, rect.Y);

                // Right
                g.DrawLine(pen, rect.Right, rect.Y - crossSize, rect.Right, rect.Bottom + crossSize);

                // Bottom
                g.DrawLine(pen, rect.X - crossSize, rect.Bottom, rect.Right + crossSize, rect.Bottom);

                // Left
                g.DrawLine(pen, rect.X, rect.Y - crossSize, rect.X, rect.Bottom + crossSize);
            }
        }

        public static void SetHighQuality(this Graphics g)
        {
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
        }
    }
}