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

using ShareX.HelpersLib;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class Screenshot
    {
        public Bitmap CaptureWindowTransparent(IntPtr handle)
        {
            if (handle.ToInt32() > 0)
            {
                Rectangle rect = CaptureHelpers.GetWindowRectangle(handle);

                if (CaptureShadow && !NativeMethods.IsZoomed(handle) && NativeMethods.IsDWMEnabled())
                {
                    rect.Inflate(ShadowOffset, ShadowOffset);
                    Rectangle intersectBounds = Screen.AllScreens.Select(x => x.Bounds).Where(x => x.IntersectsWith(rect)).Combine();
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

                    if (CaptureCursor)
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

                    using (Form form = new Form())
                    {
                        form.BackColor = Color.White;
                        form.FormBorderStyle = FormBorderStyle.None;
                        form.ShowInTaskbar = false;
                        form.StartPosition = FormStartPosition.Manual;
                        form.Location = new Point(rect.X, rect.Y);
                        form.Size = new Size(rect.Width, rect.Height);

                        NativeMethods.ShowWindow(form.Handle, (int)WindowShowStyle.ShowNoActivate);

                        if (!NativeMethods.SetWindowPos(form.Handle, handle, 0, 0, 0, 0,
                            SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE))
                        {
                            form.Close();
                            DebugHelper.WriteLine("Transparent capture failed. Reason: SetWindowPos fail.");
                            return CaptureWindow(handle);
                        }

                        Application.DoEvents();
                        Thread.Sleep(10);

                        whiteBackground = CaptureRectangleNative(rect);

                        form.BackColor = Color.Black;
                        Application.DoEvents();
                        Thread.Sleep(10);

                        blackBackground = CaptureRectangleNative(rect);

                        form.BackColor = Color.White;
                        Application.DoEvents();
                        Thread.Sleep(10);

                        whiteBackground2 = CaptureRectangleNative(rect);

                        form.Close();
                    }

                    Bitmap transparentImage;

                    if (ImageHelpers.CompareImages(whiteBackground, whiteBackground2))
                    {
                        transparentImage = CreateTransparentImage(whiteBackground, blackBackground);
                        isTransparent = true;
                    }
                    else
                    {
                        DebugHelper.WriteLine("Transparent capture failed. Reason: Images not equal.");
                        transparentImage = whiteBackground2;
                    }

                    if (cursorData != null)
                    {
                        cursorData.DrawCursor(transparentImage, rect.Location);
                    }

                    if (isTransparent)
                    {
                        transparentImage = ImageHelpers.AutoCropImage(transparentImage);

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

        public Bitmap CaptureActiveWindowTransparent()
        {
            IntPtr handle = NativeMethods.GetForegroundWindow();

            return CaptureWindowTransparent(handle);
        }

        private Bitmap CreateTransparentImage(Bitmap whiteBackground, Bitmap blackBackground)
        {
            if (whiteBackground != null && blackBackground != null && whiteBackground.Size == blackBackground.Size)
            {
                Bitmap result = new Bitmap(whiteBackground.Width, whiteBackground.Height, PixelFormat.Format32bppArgb);

                using (UnsafeBitmap whiteBitmap = new UnsafeBitmap(whiteBackground, true, ImageLockMode.ReadOnly))
                using (UnsafeBitmap blackBitmap = new UnsafeBitmap(blackBackground, true, ImageLockMode.ReadOnly))
                using (UnsafeBitmap resultBitmap = new UnsafeBitmap(result, true, ImageLockMode.WriteOnly))
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
            int cornerSize = 10;
            int alphaOffset = 200;

            using (UnsafeBitmap unsafeBitmap = new UnsafeBitmap(bitmap, true))
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

        #region Not in use

        private byte[,] windows7Corner = new byte[,]
        {
            { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 },
            { 0, 1 }, { 1, 1 }, { 2, 1 },
            { 0, 2 }, { 1, 2 },
            { 0, 3 },
            { 0, 4 }
        };

        private byte[,] windowsVistaCorner = new byte[,]
        {
            { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 },
            { 0, 1 }, { 1, 1 },
            { 0, 2 },
            { 0, 3 }
        };

        private Bitmap RemoveCorners(Image img)
        {
            byte[,] corner;

            if (Helpers.IsWindows7())
            {
                corner = windows7Corner;
            }
            else if (Helpers.IsWindowsVista())
            {
                corner = windowsVistaCorner;
            }
            else
            {
                return null;
            }

            return RemoveCorners(img, corner);
        }

        private Bitmap RemoveCorners(Image img, byte[,] cornerData)
        {
            Bitmap bmp = new Bitmap(img);

            for (int i = 0; i < cornerData.GetLength(0); i++)
            {
                // Left top corner
                bmp.SetPixel(cornerData[i, 0], cornerData[i, 1], Color.Transparent);

                // Right top corner
                bmp.SetPixel(bmp.Width - cornerData[i, 0] - 1, cornerData[i, 1], Color.Transparent);

                // Left bottom corner
                bmp.SetPixel(cornerData[i, 0], bmp.Height - cornerData[i, 1] - 1, Color.Transparent);

                // Right bottom corner
                bmp.SetPixel(bmp.Width - cornerData[i, 0] - 1, bmp.Height - cornerData[i, 1] - 1, Color.Transparent);
            }

            return bmp;
        }

        #endregion Not in use
    }
}