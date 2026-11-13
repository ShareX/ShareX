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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace ShareX.ScreenCaptureLib
{
    public class WindowsRectangleList
    {
        public List<IntPtr> IgnoreHandleList { get; set; } = new List<IntPtr>();
        public List<string> IgnoreClassNameList { get; set; } = new List<string>()
        {
            "CEF-OSC-WIDGET" // NVIDIA GeForce Overlay DT
        };
        public bool IncludeChildWindows { get; set; }
        public int Timeout { get; set; }

        private List<SimpleWindowInfo> windows;
        private HashSet<IntPtr> parentHandles;
        private CancellationTokenSource cts;

        public List<SimpleWindowInfo> GetWindowInfoList()
        {
            windows = new List<SimpleWindowInfo>();
            parentHandles = new HashSet<IntPtr>();

            try
            {
                if (Timeout > 0)
                {
                    cts = new CancellationTokenSource();
                    cts.CancelAfter(Timeout);
                }

                bool EvalWindow(IntPtr hWnd, IntPtr _)
                {
                    return CheckHandle(hWnd, null);
                }

                NativeMethods.EnumWindows(EvalWindow, IntPtr.Zero);
            }
            catch
            {
            }
            finally
            {
                cts?.Dispose();
            }

            List<SimpleWindowInfo> result = new List<SimpleWindowInfo>();

            foreach (SimpleWindowInfo window in windows)
            {
                bool rectVisible = true;

                if (!window.IsWindow)
                {
                    foreach (SimpleWindowInfo window2 in result)
                    {
                        if (window2.Rectangle.Contains(window.Rectangle))
                        {
                            rectVisible = false;
                            break;
                        }
                    }
                }

                if (rectVisible)
                {
                    result.Add(window);
                }
            }

            return result;
        }

        private bool CheckHandle(IntPtr handle, Rectangle? clipRect)
        {
            if (cts != null && cts.IsCancellationRequested)
            {
                return false;
            }

            if (IgnoreHandleList.Contains(handle))
            {
                return true;
            }

            WindowInfo windowInfo = new WindowInfo(handle);

            if (!windowInfo.IsVisible)
            {
                return true;
            }

            bool isWindow = clipRect == null;

            if (isWindow)
            {
                if (windowInfo.IsCloaked)
                {
                    return true;
                }

                string className = windowInfo.ClassName;

                if (!string.IsNullOrEmpty(className) &&
                    IgnoreClassNameList.Any(ignore => className.Equals(ignore, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }

                WindowStyles exStyle = windowInfo.ExStyle;

                // Skip non-activatable tool windows (tiling manager overlays, system
                // auxiliaries, etc.). These are never the "real" application the user
                // intends to capture, and including them causes screenshot metadata
                // (filename, window title) to reflect the overlay instead of the app
                // underneath.
                if (exStyle.HasFlag(WindowStyles.WS_EX_TOOLWINDOW, WindowStyles.WS_EX_NOACTIVATE))
                {
                    return true;
                }
            }

            SimpleWindowInfo simpleWindowInfo = new SimpleWindowInfo(handle);

            if (isWindow)
            {
                simpleWindowInfo.IsWindow = true;
                simpleWindowInfo.Rectangle = CaptureHelpers.GetWindowRectangle(handle);
            }
            else
            {
                Rectangle rect = NativeMethods.GetWindowRect(handle);
                simpleWindowInfo.Rectangle = Rectangle.Intersect(rect, clipRect.Value);
            }

            if (!simpleWindowInfo.Rectangle.IsValid())
            {
                return true;
            }

            if (IncludeChildWindows && !parentHandles.Contains(handle))
            {
                parentHandles.Add(handle);

                bool EvalControl(IntPtr hWnd, IntPtr _)
                {
                    return CheckHandle(hWnd, simpleWindowInfo.Rectangle);
                }

                NativeMethods.EnumChildWindows(handle, EvalControl, IntPtr.Zero);
            }

            if (isWindow)
            {
                Rectangle clientRect = NativeMethods.GetClientRect(handle);

                if (clientRect.IsValid() && !IsSameAsWindowRectangle(clientRect, simpleWindowInfo.Rectangle))
                {
                    windows.Add(new SimpleWindowInfo(handle, clientRect));
                }
            }

            windows.Add(simpleWindowInfo);

            return true;
        }

        /// <summary>
        /// Whether a client area covers its window except for a thin border, as in windows that draw their own title bar.
        /// Such a client area isn't offered separately, otherwise it would always be picked instead of the whole window.
        /// </summary>
        private static bool IsSameAsWindowRectangle(Rectangle clientRect, Rectangle windowRect)
        {
            // Covers the 1 px window border up to 400% scaling, but stays well below the height of a real title bar.
            const int tolerance = 8;

            Rectangle visibleClientRect = Rectangle.Intersect(clientRect, windowRect);

            return !visibleClientRect.IsEmpty &&
                visibleClientRect.Left - windowRect.Left <= tolerance &&
                visibleClientRect.Top - windowRect.Top <= tolerance &&
                windowRect.Right - visibleClientRect.Right <= tolerance &&
                windowRect.Bottom - visibleClientRect.Bottom <= tolerance;
        }
    }
}