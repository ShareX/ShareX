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

using System;
using System.Drawing;
using System.Linq;
using ShareX.Platform;
using System.Collections.Generic;
using System.Runtime.Versioning;

#nullable enable

namespace ShareX.HelpersLib
{
    public static class CaptureHelpers
    {
        // Screens and the pointer come from ShareX.Platform so these work on every OS: EnumDisplayMonitors and GetCursorPos on Windows,
        // XRandR or the compositor on Linux, CoreGraphics on macOS. They return empty values before PlatformServices is initialized
        // and on Wayland desktops that do not reveal the layout (GNOME, KDE); callers there use the UI toolkit's screen list.

        private static IReadOnlyList<ScreenInfo> GetScreens() =>
            PlatformServices.IsInitialized ? PlatformServices.Current.ScreenCapture.GetScreens() : Array.Empty<ScreenInfo>();

        private static Rectangle ToRectangle(PlatformRectangle r) => new Rectangle(r.X, r.Y, r.Width, r.Height);

        private static ScreenInfo? GetScreenAt(Point point)
        {
            IReadOnlyList<ScreenInfo> screens = GetScreens();
            return screens.FirstOrDefault(s => s.Bounds.Contains(new PlatformPoint(point.X, point.Y))) ?? screens.FirstOrDefault(s => s.IsPrimary) ?? screens.FirstOrDefault();
        }

        /// <summary>All screens combined, like SystemInformation.VirtualScreen.</summary>
        public static Rectangle GetScreenBounds()
        {
            return GetScreens().Select(x => ToRectangle(x.Bounds)).Combine();
        }

        public static Rectangle GetScreenWorkingArea()
        {
            return GetScreens().Select(x => ToRectangle(x.WorkingArea)).Combine();
        }

        [SupportedOSPlatform("windows")]
        public static Rectangle GetScreenBounds(IntPtr handle, bool workingArea)
        {
            Rectangle window = GetWindowRectangle(handle);
            ScreenInfo? screen = GetScreenAt(new Point(window.X + window.Width / 2, window.Y + window.Height / 2));
            return screen == null ? Rectangle.Empty : ToRectangle(workingArea ? screen.WorkingArea : screen.Bounds);
        }

        public static Rectangle GetActiveScreenBounds()
        {
            ScreenInfo? screen = GetScreenAt(GetCursorPosition());
            return screen == null ? Rectangle.Empty : ToRectangle(screen.Bounds);
        }

        public static Rectangle GetActiveScreenWorkingArea()
        {
            ScreenInfo? screen = GetScreenAt(GetCursorPosition());
            return screen == null ? Rectangle.Empty : ToRectangle(screen.WorkingArea);
        }

        public static Rectangle GetPrimaryScreenBounds()
        {
            IReadOnlyList<ScreenInfo> screens = GetScreens();
            ScreenInfo? screen = screens.FirstOrDefault(s => s.IsPrimary) ?? screens.FirstOrDefault();
            return screen == null ? Rectangle.Empty : ToRectangle(screen.Bounds);
        }

        [SupportedOSPlatform("windows")]
        public static Point ScreenToClient(Point p)
        {
            int screenX = NativeMethods.GetSystemMetrics(SystemMetric.SM_XVIRTUALSCREEN);
            int screenY = NativeMethods.GetSystemMetrics(SystemMetric.SM_YVIRTUALSCREEN);
            return new Point(p.X - screenX, p.Y - screenY);
        }

        [SupportedOSPlatform("windows")]
        public static Rectangle ScreenToClient(Rectangle r)
        {
            return new Rectangle(ScreenToClient(r.Location), r.Size);
        }

        [SupportedOSPlatform("windows")]
        public static Point ClientToScreen(Point p)
        {
            int screenX = NativeMethods.GetSystemMetrics(SystemMetric.SM_XVIRTUALSCREEN);
            int screenY = NativeMethods.GetSystemMetrics(SystemMetric.SM_YVIRTUALSCREEN);
            return new Point(p.X + screenX, p.Y + screenY);
        }

        [SupportedOSPlatform("windows")]
        public static Rectangle ClientToScreen(Rectangle r)
        {
            return new Rectangle(ClientToScreen(r.Location), r.Size);
        }

        /// <summary>The pointer position, or Point.Empty where the platform does not reveal it.</summary>
        public static Point GetCursorPosition()
        {
            PlatformPoint? point = PlatformServices.IsInitialized ? PlatformServices.Current.Windows.GetCursorPosition() : null;
            return point is PlatformPoint p ? new Point(p.X, p.Y) : Point.Empty;
        }

        [SupportedOSPlatform("windows")]
        public static void SetCursorPosition(int x, int y)
        {
            NativeMethods.SetCursorPos(x, y);
        }

        [SupportedOSPlatform("windows")]
        public static void SetCursorPosition(Point position)
        {
            SetCursorPosition(position.X, position.Y);
        }

        [SupportedOSPlatform("windows")]
        public static Color GetPixelColor()
        {
            return GetPixelColor(GetCursorPosition());
        }

        [SupportedOSPlatform("windows")]
        public static Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = NativeMethods.GetDC(IntPtr.Zero);
            uint pixel = NativeMethods.GetPixel(hdc, x, y);
            NativeMethods.ReleaseDC(IntPtr.Zero, hdc);
            return Color.FromArgb((int)(pixel & 0x000000FF), (int)(pixel & 0x0000FF00) >> 8, (int)(pixel & 0x00FF0000) >> 16);
        }

        [SupportedOSPlatform("windows")]
        public static Color GetPixelColor(Point position)
        {
            return GetPixelColor(position.X, position.Y);
        }

        [SupportedOSPlatform("windows")]
        public static bool CheckPixelColor(int x, int y, Color color)
        {
            Color targetColor = GetPixelColor(x, y);

            return targetColor.R == color.R && targetColor.G == color.G && targetColor.B == color.B;
        }

        [SupportedOSPlatform("windows")]
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

        [SupportedOSPlatform("windows")]
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

        [SupportedOSPlatform("windows")]
        public static Rectangle GetActiveWindowRectangle()
        {
            IntPtr handle = NativeMethods.GetForegroundWindow();
            return GetWindowRectangle(handle);
        }

        [SupportedOSPlatform("windows")]
        public static Rectangle GetActiveWindowClientRectangle()
        {
            IntPtr handle = NativeMethods.GetForegroundWindow();
            return NativeMethods.GetClientRect(handle);
        }

        [SupportedOSPlatform("windows")]
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
                    Rectangle monitorRectangle = GetScreenBounds(handle, workingArea: false);
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
