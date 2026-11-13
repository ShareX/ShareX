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
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Bitmap = SkiaSharp.SKBitmap;

namespace ShareX.ScreenCaptureLib
{
    public partial class Screenshot
    {
        public Bitmap CaptureWindowTransparent(IntPtr handle)
        {
            return CaptureWindowTransparentCore(handle, null, Rectangle.Empty);
        }

        /// <summary>
        /// Captures a window's rounded corners and, with <see cref="CaptureShadow"/>, its shadow from the live screen,
        /// but keeps the opaque window pixels of <paramref name="windowImage"/>, an earlier capture of
        /// <paramref name="windowRectangle"/> such as an annotated region capture. Returns null when the window has moved
        /// since or its transparency can't be captured, so the caller can keep its own image.
        /// </summary>
        public Bitmap CaptureWindowTransparent(IntPtr handle, Bitmap windowImage, Rectangle windowRectangle)
        {
            ArgumentNullException.ThrowIfNull(windowImage);

            return CaptureWindowTransparentCore(handle, windowImage, windowRectangle);
        }

        private Bitmap CaptureWindowTransparentCore(IntPtr handle, Bitmap windowImage, Rectangle windowRectangle)
        {
            if (handle != IntPtr.Zero)
            {
                Rectangle rect = CaptureHelpers.GetWindowRectangle(handle);

                if (windowImage != null && (rect != windowRectangle || windowImage.Width != rect.Width || windowImage.Height != rect.Height))
                {
                    DebugHelper.WriteLine("Transparent capture skipped. Reason: Window rectangle changed.");
                    return null;
                }

                Rectangle windowBounds = rect;

                if (CaptureShadow && !NativeMethods.IsZoomed(handle) && NativeMethods.IsDWMEnabled())
                {
                    rect.Inflate(ShadowOffset, ShadowOffset);
                    Rectangle intersectBounds = DesktopScreen.AllScreens.Select(x => x.Bounds).Where(x => x.IntersectsWith(rect)).Combine();
                    rect.Intersect(intersectBounds);
                }

                Bitmap whiteBackground = null, blackBackground = null, whiteBackground2 = null;
                CursorData cursorData = null;
                bool isTransparent = false, isTaskbarHide = false;

                try
                {
                    if (AutoHideTaskbar)
                    {
                        isTaskbarHide = NativeMethods.SetTaskbarVisibilityIfIntersect(false, rect);
                    }

                    // A window image already shows the cursor if the region capture drew it.
                    if (CaptureCursor && windowImage == null)
                    {
                        try
                        {
                            cursorData = new CursorData();
                        }
                        catch (Exception e)
                        {
                            DebugHelper.WriteException(e, "Cursor capture failed.");
                        }
                    }

                    using (WindowsNativeWindow window = new("ShareX - Transparent capture background", rect,
                        WindowStyles.WS_EX_LAYERED | WindowStyles.WS_EX_TOOLWINDOW | WindowStyles.WS_EX_NOACTIVATE))
                    {
                        window.SetBackground(Color.White, rect);
                        NativeMethods.ShowWindow(window.Handle, (int)WindowShowStyle.ShowNoActivate);
                        if (!NativeMethods.SetWindowPos(window.Handle, handle, 0, 0, 0, 0,
                            SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE))
                        {
                            DebugHelper.WriteLine("Transparent capture failed. Reason: SetWindowPos fail.");
                            return windowImage == null ? CaptureWindow(handle) : null;
                        }
                        FlushCaptureBackground();
                        whiteBackground = CaptureRectangleNative(rect);
                        window.SetBackground(Color.Black, rect);
                        FlushCaptureBackground();
                        blackBackground = CaptureRectangleNative(rect);
                        window.SetBackground(Color.White, rect);
                        FlushCaptureBackground();
                        whiteBackground2 = CaptureRectangleNative(rect);
                    }

                    Bitmap transparentImage;

                    if (SkiaImageHelpers.CompareImages(whiteBackground, whiteBackground2))
                    {
                        transparentImage = CreateTransparentImage(whiteBackground, blackBackground);
                        isTransparent = true;
                    }
                    else if (windowImage != null)
                    {
                        DebugHelper.WriteLine("Transparent capture skipped. Reason: Images not equal.");
                        whiteBackground2.Dispose();
                        return null;
                    }
                    else
                    {
                        DebugHelper.WriteLine("Transparent capture failed. Reason: Images not equal.");
                        transparentImage = whiteBackground2;
                    }

                    if (windowImage != null)
                    {
                        CopyOpaquePixels(windowImage, transparentImage, new Point(windowBounds.X - rect.X, windowBounds.Y - rect.Y));
                    }

                    if (cursorData != null)
                    {
                        cursorData.DrawCursor(transparentImage, rect.Location);
                    }

                    if (isTransparent)
                    {
                        transparentImage = SkiaImageHelpers.AutoCropImage(transparentImage);

                        if (!CaptureShadow)
                        {
                            TrimShadow(transparentImage);
                        }
                    }

                    return transparentImage;
                }
                finally
                {
                    if (isTaskbarHide)
                    {
                        NativeMethods.SetTaskbarVisibility(true);
                    }

                    if (whiteBackground != null) whiteBackground.Dispose();
                    if (blackBackground != null) blackBackground.Dispose();
                    if (isTransparent && whiteBackground2 != null) whiteBackground2.Dispose();
                }
            }

            return null;
        }

        /// <summary>
        /// Copies <paramref name="source"/> onto <paramref name="destination"/> at <paramref name="offset"/> as opaque
        /// pixels, except where a pixel in the source's corner squares or outer edge is not opaque in the destination, so
        /// the rounded corners and the translucent Windows 11 border keep their transparency instead of the desktop
        /// behind them. Translucent window content such as acrylic is taken from the source, because annotations may
        /// cover it.
        /// </summary>
        internal static void CopyOpaquePixels(Bitmap source, Bitmap destination, Point offset)
        {
            // Covers the 8 px Windows 11 corner radius up to 400% scaling.
            const int maxCornerSize = 32;
            const int edgeSize = 2;
            int cornerSize = Math.Min(maxCornerSize, Math.Min(source.Width, source.Height) / 2);

            Rectangle area = Rectangle.Intersect(new Rectangle(offset, new Size(source.Width, source.Height)),
                new Rectangle(0, 0, destination.Width, destination.Height));

            if (area.IsEmpty)
            {
                return;
            }

            using (SkiaPixelBuffer sourceBuffer = new SkiaPixelBuffer(source, true, PixelAccess.ReadOnly))
            using (SkiaPixelBuffer destinationBuffer = new SkiaPixelBuffer(destination, true))
            {
                for (int y = area.Top; y < area.Bottom; y++)
                {
                    int sourceY = y - offset.Y;
                    bool cornerRow = sourceY < cornerSize || sourceY >= source.Height - cornerSize;
                    bool edgeRow = sourceY < edgeSize || sourceY >= source.Height - edgeSize;

                    for (int x = area.Left; x < area.Right; x++)
                    {
                        int sourceX = x - offset.X;
                        bool keepTransparency = edgeRow || sourceX < edgeSize || sourceX >= source.Width - edgeSize ||
                            (cornerRow && (sourceX < cornerSize || sourceX >= source.Width - cornerSize));

                        if (!keepTransparency || destinationBuffer.GetPixel(x, y).Alpha == 255)
                        {
                            ColorBgra color = sourceBuffer.GetPixel(sourceX, sourceY);
                            color.Alpha = 255;
                            destinationBuffer.SetPixel(x, y, color);
                        }
                    }
                }
            }
        }

        private static void FlushCaptureBackground()
        {
            if (NativeMethods.IsDWMEnabled()) DwmFlush();
            Thread.Sleep(10);
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmFlush();

        public Bitmap CaptureActiveWindowTransparent()
        {
            IntPtr handle = NativeMethods.GetForegroundWindow();

            return CaptureWindowTransparent(handle);
        }

        private Bitmap CreateTransparentImage(Bitmap whiteBackground, Bitmap blackBackground)
        {
            if (whiteBackground != null && blackBackground != null && whiteBackground.GetSize() == blackBackground.GetSize())
            {
                Bitmap result = SkiaImageHelpers.CreateBitmap(whiteBackground.Width, whiteBackground.Height);

                using (SkiaPixelBuffer whiteBitmap = new SkiaPixelBuffer(whiteBackground, true, PixelAccess.ReadOnly))
                using (SkiaPixelBuffer blackBitmap = new SkiaPixelBuffer(blackBackground, true, PixelAccess.ReadOnly))
                using (SkiaPixelBuffer resultBitmap = new SkiaPixelBuffer(result, true, PixelAccess.WriteOnly))
                {
                    int pixelCount = blackBitmap.PixelCount;

                    for (int i = 0; i < pixelCount; i++)
                    {
                        ColorBgra white = whiteBitmap.GetPixel(i);
                        ColorBgra black = blackBitmap.GetPixel(i);

                        double alpha = (black.Red - white.Red + 255) / 255.0;

                        if (alpha == 1)
                        {
                            resultBitmap.SetPixel(i, white);
                        }
                        else if (alpha > 0)
                        {
                            white.Blue = (byte)(black.Blue / alpha);
                            white.Green = (byte)(black.Green / alpha);
                            white.Red = (byte)(black.Red / alpha);
                            white.Alpha = (byte)(255 * alpha);

                            resultBitmap.SetPixel(i, white);
                        }
                    }
                }

                return result;
            }

            return whiteBackground;
        }

        private void TrimShadow(Bitmap bitmap)
        {
            // The auto-cropped image has no minimum size, and SkiaPixelBuffer does not check bounds, so the corners must fit in it.
            int cornerSize = Math.Min(10, Math.Min(bitmap.Width, bitmap.Height));
            int alphaOffset = 200;

            if (cornerSize <= 0)
            {
                return;
            }

            using (SkiaPixelBuffer unsafeBitmap = new SkiaPixelBuffer(bitmap, true))
            {
                for (int i = 0; i < cornerSize; i++)
                {
                    int y = i;
                    int width = bitmap.Width;

                    if (Helpers.IsWindows11OrGreater())
                    {
                        alphaOffset = 75;
                    }

                    // Left top
                    for (int x = 0; x < cornerSize; x++)
                    {
                        if (unsafeBitmap.GetPixel(x, y).Alpha < alphaOffset)
                        {
                            unsafeBitmap.ClearPixel(x, y);
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Right top
                    for (int x = width - 1; x > width - cornerSize - 1; x--)
                    {
                        if (unsafeBitmap.GetPixel(x, y).Alpha < alphaOffset)
                        {
                            unsafeBitmap.ClearPixel(x, y);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (Helpers.IsWindows11OrGreater())
                    {
                        alphaOffset = 123;
                    }

                    y = bitmap.Height - i - 1;

                    // Left bottom
                    for (int x = 0; x < cornerSize; x++)
                    {
                        if (unsafeBitmap.GetPixel(x, y).Alpha < alphaOffset)
                        {
                            unsafeBitmap.ClearPixel(x, y);
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Right bottom
                    for (int x = width - 1; x > width - cornerSize - 1; x--)
                    {
                        if (unsafeBitmap.GetPixel(x, y).Alpha < alphaOffset)
                        {
                            unsafeBitmap.ClearPixel(x, y);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

    }
}
