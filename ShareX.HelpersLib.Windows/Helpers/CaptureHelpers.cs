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

using ShareX.Platform;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System;

namespace ShareX.HelpersLib
{
    // Windows-only members of CaptureHelpers, kept in the same namespace so existing call sites keep compiling.
    public static class CaptureHelpersWindows
    {
        extension(CaptureHelpers)
        {
            [SupportedOSPlatform("windows")]
            public static Rectangle GetScreenBounds(IntPtr handle, bool workingArea)
            {
                Rectangle window = GetWindowRectangle(handle);
                ScreenInfo? screen = CaptureHelpers.GetScreenAt(new Point(window.X + window.Width / 2, window.Y + window.Height / 2));
                return screen == null ? Rectangle.Empty : CaptureHelpers.ToRectangle(workingArea ? screen.WorkingArea : screen.Bounds);
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
                return GetPixelColor(CaptureHelpers.GetCursorPosition());
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
        }
    }
}
