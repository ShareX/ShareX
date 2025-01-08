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
using System.Linq;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static class CaptureHelpers
    {
        public static Rectangle GetScreenBounds()
        {
            return SystemInformation.VirtualScreen;
        }

        public static Rectangle GetScreenWorkingArea()
        {
            return Screen.AllScreens.Select(x => x.WorkingArea).Combine();
        }

        private static Rectangle GetScreenBounds2()
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

        private static Rectangle GetScreenBounds3()
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

        private static Rectangle GetScreenBounds4()
        {
            return Screen.AllScreens.Select(x => x.Bounds).Combine();
        }

        public static Rectangle GetActiveScreenBounds()
        {
            return Screen.FromPoint(GetCursorPosition()).Bounds;
        }

        public static Rectangle GetActiveScreenWorkingArea()
        {
            return Screen.FromPoint(GetCursorPosition()).WorkingArea;
        }

        public static Rectangle GetPrimaryScreenBounds()
        {
            return Screen.PrimaryScreen.Bounds;
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
            if (NativeMethods.GetCursorPos(out POINT point))
            {
                return (Point)point;
            }

            return Point.Empty;
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

            return targetColor.R.IsBetween((byte)(color.R - variation), (byte)(color.R + variation)) &&
                targetColor.G.IsBetween((byte)(color.G - variation), (byte)(color.G + variation)) &&
                targetColor.B.IsBetween((byte)(color.B - variation), (byte)(color.B + variation));
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

        public static RectangleF CreateRectangle(float x, float y, float x2, float y2)
        {
            float width, height;

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

            return new RectangleF(x, y, width, height);
        }

        public static RectangleF CreateRectangle(PointF pos, PointF pos2)
        {
            return CreateRectangle(pos.X, pos.Y, pos2.X, pos2.Y);
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

        public static PointF SnapPositionToDegree(PointF pos, PointF pos2, float degree, float startDegree)
        {
            float angle = MathHelpers.LookAtRadian(pos, pos2);
            float startAngle = MathHelpers.DegreeToRadian(startDegree);
            float snapAngle = MathHelpers.DegreeToRadian(degree);
            float newAngle = ((float)Math.Round((angle + startAngle) / snapAngle) * snapAngle) - startAngle;
            float distance = MathHelpers.Distance(pos, pos2);
            return pos.Add((PointF)MathHelpers.RadianToVector2(newAngle, distance));
        }

        public static PointF CalculateNewPosition(PointF posOnClick, PointF posCurrent, Size size)
        {
            if (posCurrent.X > posOnClick.X)
            {
                if (posCurrent.Y > posOnClick.Y)
                {
                    return new PointF(posOnClick.X + size.Width - 1, posOnClick.Y + size.Height - 1);
                }
                else
                {
                    return new PointF(posOnClick.X + size.Width - 1, posOnClick.Y - size.Height + 1);
                }
            }
            else
            {
                if (posCurrent.Y > posOnClick.Y)
                {
                    return new PointF(posOnClick.X - size.Width + 1, posOnClick.Y + size.Height - 1);
                }
                else
                {
                    return new PointF(posOnClick.X - size.Width + 1, posOnClick.Y - size.Height + 1);
                }
            }
        }

        public static RectangleF CalculateNewRectangle(PointF posOnClick, PointF posCurrent, Size size)
        {
            PointF newPosition = CalculateNewPosition(posOnClick, posCurrent, size);
            return CreateRectangle(posOnClick, newPosition);
        }

        public static Rectangle GetWindowRectangle(IntPtr handle)
        {
            Rectangle rect = Rectangle.Empty;

            if (NativeMethods.IsDWMEnabled() && NativeMethods.GetExtendedFrameBounds(handle, out Rectangle tempRect))
            {
                rect = tempRect;
            }

            if (rect.IsEmpty)
            {
                rect = NativeMethods.GetWindowRect(handle);
            }

            if (!Helpers.IsWindows10OrGreater() && NativeMethods.IsZoomed(handle))
            {
                rect = NativeMethods.MaximizedWindowFix(handle, rect);
            }

            return rect;
        }

        public static Rectangle GetActiveWindowRectangle()
        {
            IntPtr handle = NativeMethods.GetForegroundWindow();
            return GetWindowRectangle(handle);
        }

        public static Rectangle GetActiveWindowClientRectangle()
        {
            IntPtr handle = NativeMethods.GetForegroundWindow();
            return NativeMethods.GetClientRect(handle);
        }

        public static bool IsActiveWindowFullscreen()
        {
            IntPtr handle = NativeMethods.GetForegroundWindow();

            if (handle.ToInt32() > 0)
            {
                WindowInfo windowInfo = new WindowInfo(handle);
                string className = windowInfo.ClassName;
                string[] ignoreList = new string[] { "Progman", "WorkerW" };

                if (ignoreList.All(ignore => !className.Equals(ignore, StringComparison.OrdinalIgnoreCase)))
                {
                    Rectangle windowRectangle = windowInfo.Rectangle;
                    Rectangle monitorRectangle = Screen.FromRectangle(windowRectangle).Bounds;
                    return windowRectangle.Contains(monitorRectangle);
                }
            }

            return false;
        }

        public static Rectangle EvenRectangleSize(Rectangle rect)
        {
            rect.Width -= rect.Width & 1;
            rect.Height -= rect.Height & 1;
            return rect;
        }
    }
}