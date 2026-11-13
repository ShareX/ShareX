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
using ShareX.Platform.Windows.Native;
using System;
using System.Diagnostics;
using System.Linq;

namespace ShareX.Platform.Windows;

/// <summary>
/// Captures a window with its real transparency: photographs it over a white and a black backdrop placed right behind it and
/// derives each pixel's alpha from the difference. Ported from ShareX's Screenshot.CaptureWindowTransparent.
/// </summary>
internal static class TransparentWindowCapture
{
    /// <returns>Null when the backdrop could not be placed behind the window; the caller then captures it normally.</returns>
    public static ScreenCaptureResult? Capture(IntPtr hwnd, ScreenCaptureRequest request) =>
        Capture(hwnd, request, null, default);

    /// <summary>
    /// Same capture, then the opaque pixels of <paramref name="interior"/> replace the window body so annotations survive.
    /// Returns null when the window moved or the two backdrop photos disagree.
    /// </summary>
    public static ScreenCaptureResult? Capture(IntPtr hwnd, ScreenCaptureRequest request, PixelBuffer? interior, PlatformRectangle interiorBounds)
    {
        WindowCaptureOptions options = request.Window;
        PlatformRectangle windowBounds = WindowsWindowService.GetBounds(hwnd);

        if (interior != null && (windowBounds != interiorBounds || interior.Width != windowBounds.Width || interior.Height != windowBounds.Height))
        {
            Trace.WriteLine("Transparent capture skipped. Reason: Window rectangle changed.");
            return null;
        }

        PlatformRectangle rect = windowBounds;

        // DWM is always on from Windows 8, so the shadow is always there to keep.
        if (options.IncludeShadow && !Win32.IsZoomed(hwnd))
        {
            rect = rect.Inflate(options.ShadowOffset, options.ShadowOffset);
            PlatformRectangle screens = new WindowsScreenCaptureService().GetScreens()
                .Select(screen => screen.Bounds)
                .Where(bounds => bounds.IntersectsWith(rect))
                .Aggregate(PlatformRectangle.Empty, (union, bounds) => union.Union(bounds));
            rect = rect.Intersect(screens);
        }

        if (rect.IsEmpty)
        {
            return null;
        }

        (PixelBuffer Image, PlatformPoint Position)? cursor = null;

        // A region capture already shows the cursor when the user asked for one.
        if (request.IncludeCursor && interior == null)
        {
            try
            {
                cursor = WindowsScreenCaptureService.CaptureCursorImage();
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Cursor capture failed: {e}");
            }
        }

        PixelBuffer whiteBackground, blackBackground, whiteBackground2;

        using (options.HideTaskbar ? TaskbarHider.HideIfIntersecting(rect) : null)
        using (Win32BackdropWindow backdrop = new Win32BackdropWindow(rect, 0xFFFFFF))
        {
            if (!backdrop.PlaceBehind(hwnd))
            {
                Trace.WriteLine("Transparent capture failed. Reason: SetWindowPos fail.");
                return null;
            }

            backdrop.PumpMessages();
            whiteBackground = WindowsScreenCaptureService.Capture(rect, false, request.HdrToneMapping);

            backdrop.SetColor(0x000000);
            blackBackground = WindowsScreenCaptureService.Capture(rect, false, request.HdrToneMapping);

            backdrop.SetColor(0xFFFFFF);
            whiteBackground2 = WindowsScreenCaptureService.Capture(rect, false, request.HdrToneMapping);
        }

        PixelBuffer image;
        bool isTransparent = whiteBackground.Pixels.AsSpan().SequenceEqual(whiteBackground2.Pixels);

        if (isTransparent)
        {
            image = CombineBackgrounds(whiteBackground, blackBackground);
        }
        else if (interior != null)
        {
            Trace.WriteLine("Transparent capture skipped. Reason: Images not equal.");
            return null;
        }
        else
        {
            // The window changed between the captures (an animation, a blinking caret), so the alpha would be wrong.
            Trace.WriteLine("Transparent capture failed. Reason: Images not equal.");
            image = whiteBackground2;
        }

        if (interior != null)
        {
            CopyOpaquePixels(interior, image, windowBounds.X - rect.X, windowBounds.Y - rect.Y);
        }

        if (cursor is { } c)
        {
            image.BlendFrom(c.Image, c.Position.X - rect.X, c.Position.Y - rect.Y);
        }

        PlatformRectangle bounds = rect;

        if (isTransparent && interior == null)
        {
            PlatformRectangle crop = FindAutoCropRectangle(image);

            if (crop != new PlatformRectangle(0, 0, image.Width, image.Height))
            {
                image = image.Crop(crop);
                bounds = crop.Offset(rect.X, rect.Y);
            }

            if (!options.IncludeShadow)
            {
                TrimShadow(image);
            }
        }

        return new ScreenCaptureResult(image, bounds, isTransparent ? "GDI transparent" : "GDI");
    }

    /// <summary>
    /// Copies <paramref name="source"/> onto <paramref name="destination"/> at <paramref name="offsetX"/>,
    /// <paramref name="offsetY"/> as opaque pixels, except where a pixel in the source's corner squares or outer edge is not
    /// opaque in the destination. Rounded corners and the translucent border keep their transparency. Acrylic and annotations
    /// come from the source.
    /// </summary>
    internal static void CopyOpaquePixels(PixelBuffer source, PixelBuffer destination, int offsetX, int offsetY)
    {
        // Covers the 8 px Windows 11 corner radius up to 400% scaling.
        const int maxCornerSize = 32;
        const int edgeSize = 2;
        int cornerSize = Math.Min(maxCornerSize, Math.Min(source.Width, source.Height) / 2);

        for (int y = 0; y < destination.Height; y++)
        {
            int sourceY = y - offsetY;

            if (sourceY < 0 || sourceY >= source.Height)
            {
                continue;
            }

            bool cornerRow = sourceY < cornerSize || sourceY >= source.Height - cornerSize;
            bool edgeRow = sourceY < edgeSize || sourceY >= source.Height - edgeSize;

            for (int x = 0; x < destination.Width; x++)
            {
                int sourceX = x - offsetX;

                if (sourceX < 0 || sourceX >= source.Width)
                {
                    continue;
                }

                bool keepTransparency = edgeRow || sourceX < edgeSize || sourceX >= source.Width - edgeSize ||
                    (cornerRow && (sourceX < cornerSize || sourceX >= source.Width - cornerSize));
                int destinationIndex = y * destination.Stride + x * 4;

                if (!keepTransparency || destination.Pixels[destinationIndex + 3] == 255)
                {
                    int sourceIndex = sourceY * source.Stride + sourceX * 4;
                    destination.Pixels[destinationIndex] = source.Pixels[sourceIndex];
                    destination.Pixels[destinationIndex + 1] = source.Pixels[sourceIndex + 1];
                    destination.Pixels[destinationIndex + 2] = source.Pixels[sourceIndex + 2];
                    destination.Pixels[destinationIndex + 3] = 255;
                }
            }
        }
    }

    /// <summary>Recovers colour and alpha from the same pixels over white and over black.</summary>
    internal static PixelBuffer CombineBackgrounds(PixelBuffer whiteBackground, PixelBuffer blackBackground)
    {
        if (whiteBackground.Width != blackBackground.Width || whiteBackground.Height != blackBackground.Height)
        {
            return whiteBackground;
        }

        PixelBuffer result = new PixelBuffer(whiteBackground.Width, whiteBackground.Height);
        byte[] white = whiteBackground.Pixels, black = blackBackground.Pixels, output = result.Pixels;

        for (int i = 0; i < output.Length; i += 4)
        {
            // Over black a pixel is colour * alpha, over white colour * alpha + (1 - alpha); the red channels give alpha.
            double alpha = (black[i + 2] - white[i + 2] + 255) / 255.0;

            if (alpha == 1)
            {
                output[i] = white[i];
                output[i + 1] = white[i + 1];
                output[i + 2] = white[i + 2];
                output[i + 3] = 255;
            }
            else if (alpha > 0)
            {
                output[i] = (byte)(black[i] / alpha);
                output[i + 1] = (byte)(black[i + 1] / alpha);
                output[i + 2] = (byte)(black[i + 2] / alpha);
                output[i + 3] = (byte)(255 * alpha);
            }
        }

        return result;
    }

    /// <summary>The bounds of everything that differs from the top left pixel (only its alpha, when that pixel is transparent).</summary>
    internal static PlatformRectangle FindAutoCropRectangle(PixelBuffer image)
    {
        uint[] pixels = new uint[image.Width * image.Height];
        Buffer.BlockCopy(image.Pixels, 0, pixels, 0, pixels.Length * 4);
        uint mask = (pixels[0] >> 24) == 0 ? 0xFF000000 : 0xFFFFFFFF;
        uint check = pixels[0] & mask;
        int left = image.Width, top = image.Height, right = -1, bottom = -1;

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                if ((pixels[(y * image.Width) + x] & mask) != check)
                {
                    left = Math.Min(left, x);
                    right = Math.Max(right, x);
                    top = Math.Min(top, y);
                    bottom = Math.Max(bottom, y);
                }
            }
        }

        return right < 0 ? new PlatformRectangle(0, 0, image.Width, image.Height) : PlatformRectangle.FromLTRB(left, top, right + 1, bottom + 1);
    }

    /// <summary>Clears the faint shadow pixels left in the rounded corners when the shadow is not wanted.</summary>
    internal static void TrimShadow(PixelBuffer image)
    {
        const int cornerSize = 10;
        bool windows11 = Environment.OSVersion.Version.Build >= 22000;
        int topAlpha = windows11 ? 75 : 200;
        int bottomAlpha = windows11 ? 123 : 200;
        int width = image.Width;

        for (int i = 0; i < cornerSize && i < image.Height; i++)
        {
            ClearRun(image, i, 0, 1, topAlpha);
            ClearRun(image, i, width - 1, -1, topAlpha);
            ClearRun(image, image.Height - i - 1, 0, 1, bottomAlpha);
            ClearRun(image, image.Height - i - 1, width - 1, -1, bottomAlpha);
        }

        static void ClearRun(PixelBuffer image, int y, int startX, int step, int alphaOffset)
        {
            for (int n = 0, x = startX; n < cornerSize && x >= 0 && x < image.Width; n++, x += step)
            {
                int offset = (y * image.Stride) + (x * 4);

                if (image.Pixels[offset + 3] >= alphaOffset)
                {
                    break;
                }

                image.Pixels.AsSpan(offset, 4).Clear();
            }
        }
    }
}

/// <summary>Hides the Windows task bar while a capture that overlaps it runs.</summary>
internal sealed class TaskbarHider : IDisposable
{
    private readonly IntPtr taskbar;

    private TaskbarHider(IntPtr taskbar)
    {
        this.taskbar = taskbar;
        Win32.ShowWindow(taskbar, Win32.SW_HIDE);
    }

    public static TaskbarHider? HideIfIntersecting(PlatformRectangle area)
    {
        IntPtr taskbar = Win32.FindWindow("Shell_TrayWnd", null);

        if (taskbar != IntPtr.Zero && Win32.GetWindowRect(taskbar, out Win32.RECT rect) && rect.ToRectangle().IntersectsWith(area))
        {
            return new TaskbarHider(taskbar);
        }

        return null;
    }

    public void Dispose() => Win32.ShowWindow(taskbar, Win32.SW_SHOW);
}
