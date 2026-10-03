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

using ShareX.Platform.Imaging;
using ShareX.Platform;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Linq;

namespace ShareX.HelpersLib
{
    public static class CaptureHelpers
    {
        public static Rectangle GetScreenBounds()
        {
            return DesktopScreen.AllScreens.Select(x => x.Bounds).Combine();
        }

        public static Rectangle GetScreenWorkingArea()
        {
            return DesktopScreen.AllScreens.Select(x => x.WorkingArea).Combine();
        }

        public static Rectangle GetScreenBounds(IntPtr handle, bool workingArea)
        {
            DesktopScreen screen = DesktopScreen.FromHandle(handle);
            return workingArea ? screen.WorkingArea : screen.Bounds;
        }

        public static Rectangle GetActiveScreenBounds()
        {
            return DesktopScreen.FromPoint(GetCursorPosition()).Bounds;
        }

        public static Rectangle GetActiveScreenWorkingArea()
        {
            return DesktopScreen.FromPoint(GetCursorPosition()).WorkingArea;
        }

        public static Rectangle GetPrimaryScreenBounds()
        {
            return DesktopScreen.PrimaryScreen.Bounds;
        }

        public static Point ScreenToClient(Point p)
        {
            Rectangle bounds = GetScreenBounds();
            int screenX = bounds.X;
            int screenY = bounds.Y;
            return new Point(p.X - screenX, p.Y - screenY);
        }

        public static Rectangle ScreenToClient(Rectangle r)
        {
            return new Rectangle(ScreenToClient(r.Location), r.Size);
        }

        public static Point ClientToScreen(Point p)
        {
            Rectangle bounds = GetScreenBounds();
            int screenX = bounds.X;
            int screenY = bounds.Y;
            return new Point(p.X + screenX, p.Y + screenY);
        }

        public static Rectangle ClientToScreen(Rectangle r)
        {
            return new Rectangle(ClientToScreen(r.Location), r.Size);
        }

        /// <summary>The pointer position, or Point.Empty where the platform does not reveal it (most Wayland compositors).</summary>
        /// <summary>
        /// Screen coordinates to the pixels ShareX's own windows are placed in. The same on Windows, macOS and X11; scaled on
        /// Hyprland with XWayland zero scaling (see <see cref="IWindowService.GetOwnWindowPixelScale"/>).
        /// </summary>
        public static Rectangle ScreenToOwnWindowPixels(Rectangle rectangle)
        {
            double scale = GetOwnWindowPixelScale(rectangle.Location);
            return scale == 1 ? rectangle : new Rectangle((int)Math.Round(rectangle.X * scale), (int)Math.Round(rectangle.Y * scale),
                (int)Math.Round(rectangle.Width * scale), (int)Math.Round(rectangle.Height * scale));
        }

        public static Rectangle OwnWindowPixelsToScreen(Rectangle rectangle)
        {
            double scale = GetOwnWindowPixelScale(rectangle.Location);
            return scale == 1 ? rectangle : new Rectangle((int)Math.Round(rectangle.X / scale), (int)Math.Round(rectangle.Y / scale),
                (int)Math.Round(rectangle.Width / scale), (int)Math.Round(rectangle.Height / scale));
        }

        private static double GetOwnWindowPixelScale(Point point)
        {
            double scale = PlatformServices.IsInitialized ? PlatformServices.Current.Windows.GetOwnWindowPixelScale(new PlatformPoint(point.X, point.Y)) : 1;
            return double.IsFinite(scale) && scale > 0 ? scale : 1;
        }

        public static Point GetCursorPosition()
        {
            PlatformPoint? point = PlatformServices.IsInitialized ? PlatformServices.Current.Windows.GetCursorPosition() : null;
            return point is PlatformPoint p ? new Point(p.X, p.Y) : Point.Empty;
        }

        /// <summary>Moves the pointer where the platform allows it; most Wayland compositors do not.</summary>
        public static void SetCursorPosition(int x, int y)
        {
            if (PlatformServices.IsInitialized)
            {
                PlatformServices.Current.Windows.SetCursorPosition(new PlatformPoint(x, y));
            }
        }

        public static void SetCursorPosition(Point position)
        {
            SetCursorPosition(position.X, position.Y);
        }

        public static Color GetPixelColor()
        {
            return GetPixelColor(GetCursorPosition());
        }

        /// <summary>The colour of one screen pixel, read with a one pixel screen capture.</summary>
        public static Color GetPixelColor(int x, int y)
        {
            try
            {
                ScreenCaptureResult result = PlatformServices.Current.ScreenCapture
                    .CaptureAsync(ScreenCaptureRequest.ForRegion(new PlatformRectangle(x, y, 1, 1)))
                    .GetAwaiter().GetResult();
                PixelBuffer pixels = result.Pixels ?? PngCodec.Decode(result.Png);
                return Color.FromArgb(pixels.Pixels[2], pixels.Pixels[1], pixels.Pixels[0]);
            }
            catch (Exception e) when (e is ArgumentException or InvalidOperationException or PlatformNotSupportedException)
            {
                // Outside every screen, or the platform cannot capture here.
                return Color.Empty;
            }
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

        /// <summary>The window's visible frame (on Windows without the invisible resize borders), or Rectangle.Empty.</summary>
        public static Rectangle GetWindowRectangle(IntPtr handle)
        {
            PlatformRectangle? bounds = PlatformServices.Current.Windows.GetWindowBounds(handle.ToInt64());
            return bounds is PlatformRectangle b ? new Rectangle(b.X, b.Y, b.Width, b.Height) : Rectangle.Empty;
        }

        /// <summary>A window by its title: an exact match first, then the first window whose title contains the text. IntPtr.Zero when none.</summary>
        public static IntPtr FindWindowByTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return IntPtr.Zero;
            }

            IReadOnlyList<PlatformWindow> windows = PlatformServices.Current.Windows.GetWindows();
            PlatformWindow window = windows.FirstOrDefault(x => x.Title == title) ??
                windows.FirstOrDefault(x => x.Title.Contains(title, StringComparison.InvariantCultureIgnoreCase));
            return window != null ? new IntPtr(window.Handle) : IntPtr.Zero;
        }

        /// <summary>Title, process and bounds of the window with keyboard focus, or null where the platform does not say.</summary>
        public static WindowDetails GetActiveWindowDetails()
        {
            if (!PlatformServices.IsInitialized)
            {
                return null;
            }

            long handle = PlatformServices.Current.Windows.GetActiveWindowHandle();
            return handle != 0 ? PlatformServices.Current.WindowManagement.GetDetails(handle) : null;
        }

        public static Rectangle GetActiveWindowRectangle()
        {
            return GetWindowRectangle(new IntPtr(PlatformServices.Current.Windows.GetActiveWindowHandle()));
        }

        public static Rectangle GetActiveWindowClientRectangle()
        {
            IWindowService windows = PlatformServices.Current.Windows;
            PlatformRectangle? bounds = windows.GetClientBounds(windows.GetActiveWindowHandle());
            return bounds is PlatformRectangle b ? new Rectangle(b.X, b.Y, b.Width, b.Height) : Rectangle.Empty;
        }

        /// <summary>
        /// Whether the active window covers its whole screen (a game or a full screen video). The desktop itself never counts:
        /// the platform does not report it as the active window.
        /// </summary>
        public static bool IsActiveWindowFullscreen()
        {
            PlatformWindow window = PlatformServices.IsInitialized ? PlatformServices.Current.Windows.GetActiveWindow() : null;

            if (window == null)
            {
                return false;
            }

            Rectangle windowRectangle = new Rectangle(window.Bounds.X, window.Bounds.Y, window.Bounds.Width, window.Bounds.Height);
            Rectangle monitorRectangle = DesktopScreen.FromRectangle(windowRectangle).Bounds;
            return windowRectangle.Contains(monitorRectangle);
        }

        public static Rectangle EvenRectangleSize(Rectangle rect)
        {
            rect.Width -= rect.Width & 1;
            rect.Height -= rect.Height & 1;
            return rect;
        }
    }
}
