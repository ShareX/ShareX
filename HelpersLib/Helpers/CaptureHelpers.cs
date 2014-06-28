#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using System.Linq;
using System.Windows.Forms;

namespace HelpersLib
{
    public static class CaptureHelpers
    {
        public static Rectangle GetScreenBounds()
        {
            return SystemInformation.VirtualScreen;
        }

        public static Rectangle GetScreenWorkingArea()
        {
            return SystemInformation.WorkingArea;
        }

        public static Rectangle GetScreenBounds2()
        {
            Point topLeft = Point.Empty;
            Point bottomRight = Point.Empty;

            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Bounds.X < topLeft.X) topLeft.X = screen.Bounds.X;
                if (screen.Bounds.Y < topLeft.Y) topLeft.Y = screen.Bounds.Y;
                if ((screen.Bounds.X + screen.Bounds.Width) > bottomRight.X) bottomRight.X = screen.Bounds.X + screen.Bounds.Width;
                if ((screen.Bounds.Y + screen.Bounds.Height) > bottomRight.Y) bottomRight.Y = screen.Bounds.Y + screen.Bounds.Height;
            }

            return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X + Math.Abs(topLeft.X), bottomRight.Y + Math.Abs(topLeft.Y));
        }

        public static Rectangle GetScreenBounds3()
        {
            Point topLeft = Point.Empty;
            Point bottomRight = Point.Empty;

            foreach (Screen screen in Screen.AllScreens)
            {
                topLeft.X = Math.Min(topLeft.X, screen.Bounds.X);
                topLeft.Y = Math.Min(topLeft.Y, screen.Bounds.Y);
                bottomRight.X = Math.Max(bottomRight.X, screen.Bounds.Right);
                bottomRight.Y = Math.Max(bottomRight.Y, screen.Bounds.Bottom);
            }

            return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X + Math.Abs(topLeft.X), bottomRight.Y + Math.Abs(topLeft.Y));
        }

        public static Rectangle GetScreenBounds4()
        {
            return Screen.AllScreens.Aggregate(Rectangle.Empty, (current, screen) => Rectangle.Union(current, screen.Bounds));
        }

        public static Rectangle GetActiveScreenBounds()
        {
            return Screen.FromPoint(GetCursorPosition()).Bounds;
        }

        public static Rectangle GetScreenBounds0Based()
        {
            return ScreenToClient(GetScreenBounds());
        }

        public static Point ScreenToClient(Point p)
        {
            int screenX = NativeMethods.GetSystemMetrics(SystemMetric.SM_XVIRTUALSCREEN);
            int screenY = NativeMethods.GetSystemMetrics(SystemMetric.SM_YVIRTUALSCREEN);
            return new Point(p.X - screenX, p.Y - screenY);
        }

        public static Rectangle ScreenToClient(Rectangle r)
        {
            return new Rectangle(ScreenToClient(r.Location), r.Size);
        }

        public static Point ClientToScreen(Point p)
        {
            int screenX = NativeMethods.GetSystemMetrics(SystemMetric.SM_XVIRTUALSCREEN);
            int screenY = NativeMethods.GetSystemMetrics(SystemMetric.SM_YVIRTUALSCREEN);
            return new Point(p.X + screenX, p.Y + screenY);
        }

        public static Rectangle ClientToScreen(Rectangle r)
        {
            return new Rectangle(ClientToScreen(r.Location), r.Size);
        }

        public static Point GetCursorPosition()
        {
            POINT point;

            if (NativeMethods.GetCursorPos(out point))
            {
                return (Point)point;
            }

            return Point.Empty;
        }

        public static Point GetZeroBasedMousePosition()
        {
            return ScreenToClient(GetCursorPosition());
        }

        public static void SetCursorPosition(int x, int y)
        {
            NativeMethods.SetCursorPos(x, y);
        }

        public static void SetCursorPosition(Point position)
        {
            SetCursorPosition(position.X, position.Y);
        }

        public static Color GetPixelColor()
        {
            return GetPixelColor(GetCursorPosition());
        }

        public static Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = NativeMethods.GetDC(IntPtr.Zero);
            uint pixel = NativeMethods.GetPixel(hdc, x, y);
            NativeMethods.ReleaseDC(IntPtr.Zero, hdc);
            return Color.FromArgb((int)(pixel & 0x000000FF), (int)(pixel & 0x0000FF00) >> 8, (int)(pixel & 0x00FF0000) >> 16);
        }

        public static Color GetPixelColor(Point position)
        {
            return GetPixelColor(position.X, position.Y);
        }

        public static bool CheckPixelColor(int x, int y, Color color)
        {
            Color targetColor = GetPixelColor(x, y);

            return targetColor.R == color.R && targetColor.G == color.G && targetColor.B == color.B;
        }

        public static bool CheckPixelColor(int x, int y, Color color, byte variation)
        {
            Color targetColor = GetPixelColor(x, y);

            return targetColor.R.IsBetween(color.R - variation, color.R + variation) &&
                   targetColor.G.IsBetween(color.G - variation, color.G + variation) &&
                   targetColor.B.IsBetween(color.B - variation, color.B + variation);
        }

        public static Rectangle CreateRectangle(int x, int y, int x2, int y2)
        {
            int width, height;

            if (x <= x2)
            {
                width = x2 - x + 1;
            }
            else
            {
                width = x - x2 + 1;
                x = x2;
            }

            if (y <= y2)
            {
                height = y2 - y + 1;
            }
            else
            {
                height = y - y2 + 1;
                y = y2;
            }

            return new Rectangle(x, y, width, height);
        }

        public static Rectangle CreateRectangle(Point pos, Point pos2)
        {
            return CreateRectangle(pos.X, pos.Y, pos2.X, pos2.Y);
        }

        public static Rectangle FixRectangle(int x, int y, int width, int height)
        {
            if (width < 0)
            {
                x += width;
                width = -width;
            }

            if (height < 0)
            {
                y += height;
                height = -height;
            }

            return new Rectangle(x, y, width, height);
        }

        public static Rectangle FixRectangle(Rectangle rect)
        {
            return FixRectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Point ProportionalPosition(Point pos, Point pos2)
        {
            Point newPosition = Point.Empty;
            int min;

            if (pos.X < pos2.X)
            {
                if (pos.Y < pos2.Y)
                {
                    min = Math.Min(pos2.X - pos.X, pos2.Y - pos.Y);
                    newPosition.X = pos.X + min;
                    newPosition.Y = pos.Y + min;
                }
                else
                {
                    min = Math.Min(pos2.X - pos.X, pos.Y - pos2.Y);
                    newPosition.X = pos.X + min;
                    newPosition.Y = pos.Y - min;
                }
            }
            else
            {
                if (pos.Y > pos2.Y)
                {
                    min = Math.Min(pos.X - pos2.X, pos.Y - pos2.Y);
                    newPosition.X = pos.X - min;
                    newPosition.Y = pos.Y - min;
                }
                else
                {
                    min = Math.Min(pos.X - pos2.X, pos2.Y - pos.Y);
                    newPosition.X = pos.X - min;
                    newPosition.Y = pos.Y + min;
                }
            }

            return newPosition;
        }

        public static Rectangle GetWindowRectangle(IntPtr handle, bool maximizeFix = true)
        {
            Rectangle rect = Rectangle.Empty;

            if (NativeMethods.IsDWMEnabled())
            {
                Rectangle tempRect;

                if (NativeMethods.GetExtendedFrameBounds(handle, out tempRect))
                {
                    rect = tempRect;
                }
            }

            if (rect.IsEmpty)
            {
                rect = NativeMethods.GetWindowRect(handle);
            }

            if (maximizeFix && NativeMethods.IsZoomed(handle))
            {
                rect = NativeMethods.MaximizedWindowFix(handle, rect);
            }

            return rect;
        }

        public static Rectangle EvenRectangleSize(Rectangle rect)
        {
            rect.Width += rect.Width & 1;
            rect.Height += rect.Height & 1;
            return rect;
        }
    }
}