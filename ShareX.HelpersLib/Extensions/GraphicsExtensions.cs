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
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static class GraphicsExtensions
    {
        public static void DrawRectangleProper(this Graphics g, Pen pen, RectangleF rect)
        {
            if (pen.Width == 1)
            {
                rect = rect.SizeOffset(-1);
            }

            if (rect.Width > 0 && rect.Height > 0)
            {
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
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

                if (currentAlpha > 0)
                {
                    using (Pen pen = new Pen(Color.FromArgb(currentAlpha, shadowColor)))
                    {
                        Rectangle shadowRect = new Rectangle(rect.X + (-shadowDirection.Left * i), rect.Y + (-shadowDirection.Top * i),
                            rect.Width + ((shadowDirection.Left + shadowDirection.Right) * i), rect.Height + ((shadowDirection.Top + shadowDirection.Bottom) * i));

                        g.DrawRectangleProper(pen, shadowRect);
                    }
                }
            }
        }

        public static void DrawRoundedRectangle(this Graphics g, Pen pen, RectangleF rect, float radius)
        {
            g.DrawRoundedRectangle(null, pen, rect, radius);
        }

        public static void DrawRoundedRectangle(this Graphics g, Brush brush, RectangleF rect, float radius)
        {
            g.DrawRoundedRectangle(brush, null, rect, radius);
        }

        public static void DrawRoundedRectangle(this Graphics g, Brush brush, Pen pen, int x, int y, int width, int height, float radius)
        {
            DrawRoundedRectangle(g, brush, pen, new Rectangle(x, y, width, height), radius);
        }

        public static void DrawRoundedRectangle(this Graphics g, Brush brush, Pen pen, RectangleF rect, float radius)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddRoundedRectangleProper(rect, radius);
                if (brush != null) g.FillPath(brush, gp);
                if (pen != null) g.DrawPath(pen, gp);
            }
        }

        public static void FillRoundedRectangle(this Graphics g, Brush brush, int x, int y, int width, int height, float radius)
        {
            FillRoundedRectangle(g, brush, new Rectangle(x, y, width, height), radius);
        }

        public static void FillRoundedRectangle(this Graphics g, Brush brush, RectangleF rect, float radius)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddRoundedRectangle(rect, radius);
                    g.FillPath(brush, gp);
                }
            }
        }

        public static void DrawCapsule(this Graphics g, Brush brush, RectangleF rect)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddCapsule(rect);
                g.FillPath(brush, gp);
            }
        }

        public static void DrawDiamond(this Graphics g, Pen pen, Rectangle rect)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddDiamond(rect);
                g.DrawPath(pen, gp);
            }
        }

        public static void DrawCross(this Graphics g, Pen pen, PointF center, int crossSize)
        {
            if (crossSize > 0)
            {
                // Horizontal
                g.DrawLine(pen, center.X - crossSize, center.Y, center.X + crossSize, center.Y);

                // Vertical
                g.DrawLine(pen, center.X, center.Y - crossSize, center.X, center.Y + crossSize);
            }
        }

        public static void DrawCrossRectangle(this Graphics g, Pen pen, RectangleF rect, int crossSize)
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

        public static void DrawCornerLines(this Graphics g, RectangleF rect, Pen pen, int lineSize)
        {
            if (rect.Width <= lineSize * 2)
            {
                g.DrawLine(pen, rect.X, rect.Y, rect.Right - 1, rect.Y);
                g.DrawLine(pen, rect.X, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
            }
            else
            {
                // Top left
                g.DrawLine(pen, rect.X, rect.Y, rect.X + lineSize, rect.Y);

                // Top right
                g.DrawLine(pen, rect.Right - 1, rect.Y, rect.Right - 1 - lineSize, rect.Y);

                // Bottom left
                g.DrawLine(pen, rect.X, rect.Bottom - 1, rect.X + lineSize, rect.Bottom - 1);

                // Bottom right
                g.DrawLine(pen, rect.Right - 1, rect.Bottom - 1, rect.Right - 1 - lineSize, rect.Bottom - 1);
            }

            if (rect.Height <= lineSize * 2)
            {
                g.DrawLine(pen, rect.X, rect.Y, rect.X, rect.Bottom - 1);
                g.DrawLine(pen, rect.Right - 1, rect.Y, rect.Right - 1, rect.Bottom - 1);
            }
            else
            {
                // Top left
                g.DrawLine(pen, rect.X, rect.Y, rect.X, rect.Y + lineSize);

                // Top right
                g.DrawLine(pen, rect.Right - 1, rect.Y, rect.Right - 1, rect.Y + lineSize);

                // Bottom left
                g.DrawLine(pen, rect.X, rect.Bottom - 1, rect.X, rect.Bottom - 1 - lineSize);

                // Bottom right
                g.DrawLine(pen, rect.Right - 1, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1 - lineSize);
            }
        }

        public static void SetHighQuality(this Graphics g)
        {
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
        }

        public static void DrawTextWithOutline(this Graphics g, string text, PointF position, Font font, Color textColor, Color borderColor, int borderSize = 2)
        {
            SmoothingMode tempMode = g.SmoothingMode;

            g.SmoothingMode = SmoothingMode.HighQuality;

            using (GraphicsPath gp = new GraphicsPath())
            {
                using (StringFormat sf = new StringFormat())
                {
                    float emSize = g.DpiY * font.SizeInPoints / 72;
                    gp.AddString(text, font.FontFamily, (int)font.Style, emSize, position, sf);
                }

                if (borderSize > 0)
                {
                    using (Pen borderPen = new Pen(borderColor, borderSize) { LineJoin = LineJoin.Round })
                    {
                        g.DrawPath(borderPen, gp);
                    }
                }

                using (Brush textBrush = new SolidBrush(textColor))
                {
                    g.FillPath(textBrush, gp);
                }
            }

            g.SmoothingMode = tempMode;
        }

        public static void DrawTextWithShadow(this Graphics g, string text, PointF position, Font font, Brush textBrush, Brush shadowBrush)
        {
            DrawTextWithShadow(g, text, position, font, textBrush, shadowBrush, new Point(1, 1));
        }

        public static void DrawTextWithShadow(this Graphics g, string text, PointF position, Font font, Brush textBrush, Brush shadowBrush, Point shadowOffset)
        {
            g.DrawString(text, font, shadowBrush, position.X + shadowOffset.X, position.Y + shadowOffset.Y);
            g.DrawString(text, font, textBrush, position.X, position.Y);
        }
    }
}