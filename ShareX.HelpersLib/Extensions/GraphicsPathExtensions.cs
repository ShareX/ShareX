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
using System.Reflection;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib
{
    public static class GraphicsPathExtensions
    {
        public static void AddRectangleProper(this GraphicsPath graphicsPath, RectangleF rect, float penWidth = 1)
        {
            if (penWidth == 1)
            {
                rect = new RectangleF(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }

            if (rect.Width > 0 && rect.Height > 0)
            {
                graphicsPath.AddRectangle(rect);
            }
        }

        public static void AddRoundedRectangleProper(this GraphicsPath graphicsPath, RectangleF rect, float radius, float penWidth = 1)
        {
            if (penWidth == 1)
            {
                rect = new RectangleF(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }

            if (rect.Width > 0 && rect.Height > 0)
            {
                graphicsPath.AddRoundedRectangle(rect, radius);
            }
        }

        public static void AddRoundedRectangle(this GraphicsPath gp, RectangleF rect, float radius)
        {
            if (radius <= 0f)
            {
                gp.AddRectangle(rect);
            }
            else
            {
                // If the corner radius is greater than or equal to
                // half the width, or height (whichever is shorter)
                // then return a capsule instead of a lozenge
                if (radius >= (Math.Min(rect.Width, rect.Height) / 2.0f))
                {
                    gp.AddCapsule(rect);
                }
                else
                {
                    // Create the arc for the rectangle sides and declare
                    // a graphics path object for the drawing
                    float diameter = radius * 2.0f;
                    SizeF size = new SizeF(diameter, diameter);
                    RectangleF arc = new RectangleF(rect.Location, size);

                    // Top left arc
                    gp.AddArc(arc, 180, 90);

                    // Top right arc
                    arc.X = rect.Right - diameter;
                    gp.AddArc(arc, 270, 90);

                    // Bottom right arc
                    arc.Y = rect.Bottom - diameter;
                    gp.AddArc(arc, 0, 90);

                    // Bottom left arc
                    arc.X = rect.Left;
                    gp.AddArc(arc, 90, 90);

                    gp.CloseFigure();
                }
            }
        }

        public static void AddCapsule(this GraphicsPath gp, RectangleF rect)
        {
            float diameter;
            RectangleF arc;

            try
            {
                if (rect.Width > rect.Height)
                {
                    // Horizontal capsule
                    diameter = rect.Height;
                    SizeF sizeF = new SizeF(diameter, diameter);
                    arc = new RectangleF(rect.Location, sizeF);
                    gp.AddArc(arc, 90, 180);
                    arc.X = rect.Right - diameter;
                    gp.AddArc(arc, 270, 180);
                }
                else if (rect.Width < rect.Height)
                {
                    // Vertical capsule
                    diameter = rect.Width;
                    SizeF sizeF = new SizeF(diameter, diameter);
                    arc = new RectangleF(rect.Location, sizeF);
                    gp.AddArc(arc, 180, 180);
                    arc.Y = rect.Bottom - diameter;
                    gp.AddArc(arc, 0, 180);
                }
                else
                {
                    // Circle
                    gp.AddEllipse(rect);
                }
            }
            catch
            {
                gp.AddEllipse(rect);
            }

            gp.CloseFigure();
        }

        public static void AddDiamond(this GraphicsPath graphicsPath, RectangleF rect)
        {
            PointF p1 = new PointF(rect.X + (rect.Width / 2.0f), rect.Y);
            PointF p2 = new PointF(rect.X + rect.Width, rect.Y + (rect.Height / 2.0f));
            PointF p3 = new PointF(rect.X + (rect.Width / 2.0f), rect.Y + rect.Height);
            PointF p4 = new PointF(rect.X, rect.Y + (rect.Height / 2.0f));

            graphicsPath.AddPolygon(new PointF[] { p1, p2, p3, p4 });
        }

        public static void AddPolygon(this GraphicsPath graphicsPath, RectangleF rect, int sideCount)
        {
            PointF[] points = new PointF[sideCount];

            float a = 0;

            for (int i = 0; i < sideCount; i++)
            {
                points[i] = new PointF(rect.X + ((rect.Width / 2.0f) * (float)Math.Cos(a)) + (rect.Width / 2.0f),
                    rect.Y + ((rect.Height / 2.0f) * (float)Math.Sin(a)) + (rect.Height / 2.0f));

                a += (float)Math.PI * 2.0f / sideCount;
            }

            graphicsPath.AddPolygon(points);
        }

        public static void WindingModeOutline(this GraphicsPath graphicsPath)
        {
            IntPtr handle = (IntPtr)graphicsPath.GetType().GetField("nativePath", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(graphicsPath);
            HandleRef path = new HandleRef(graphicsPath, handle);
            NativeMethods.GdipWindingModeOutline(path, IntPtr.Zero, 0.25F);
        }
    }
}