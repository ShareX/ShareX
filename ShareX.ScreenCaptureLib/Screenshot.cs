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

#nullable enable

using ShareX.HelpersLib;
using ShareX.Platform;
using ShareX.Platform.Imaging;
using SkiaSharp;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.ScreenCaptureLib
{
    /// <summary>
    /// Screen, monitor and window capture with ShareX's capture settings, on top of <see cref="IScreenCaptureService"/>. Options a
    /// platform does not implement (see <see cref="IScreenCaptureService.Features"/>) are ignored there.
    /// </summary>
    public class Screenshot
    {
        public bool CaptureCursor { get; set; } = false;
        public bool CaptureClientArea { get; set; } = false;
        public bool RemoveOutsideScreenArea { get; set; } = true;
        public bool CaptureShadow { get; set; } = false;
        public int ShadowOffset { get; set; } = 20;
        public bool AutoHideTaskbar { get; set; } = false;
        public bool HDRScreenshotColorCorrection { get; set; } = false;

        private static IScreenCaptureService Service => PlatformServices.Current.ScreenCapture;

        public Task<SKBitmap?> CaptureRectangleAsync(Rectangle rect, CancellationToken cancellationToken = default)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return Task.FromResult<SKBitmap?>(null);
            }

            return CaptureAsync(CreateRequest() with
            {
                Mode = ScreenCaptureMode.Region,
                Region = new PlatformRectangle(rect.X, rect.Y, rect.Width, rect.Height)
            }, cancellationToken);
        }

        public Task<SKBitmap?> CaptureFullscreenAsync(CancellationToken cancellationToken = default)
        {
            Rectangle bounds = CaptureHelpers.GetScreenBounds();

            // GNOME and KDE on Wayland do not reveal the screen layout; let the platform capture every screen.
            return bounds.IsEmpty
                ? CaptureAsync(CreateRequest() with { Mode = ScreenCaptureMode.FullScreen }, cancellationToken)
                : CaptureRectangleAsync(bounds, cancellationToken);
        }

        public Task<SKBitmap?> CaptureActiveMonitorAsync(CancellationToken cancellationToken = default)
        {
            Rectangle bounds = CaptureHelpers.GetActiveScreenBounds();
            return bounds.IsEmpty ? CaptureFullscreenAsync(cancellationToken) : CaptureRectangleAsync(bounds, cancellationToken);
        }

        public Task<SKBitmap?> CaptureWindowAsync(IntPtr handle, CancellationToken cancellationToken = default) =>
            CaptureWindowAsync(handle, transparent: false, cancellationToken);

        public Task<SKBitmap?> CaptureActiveWindowAsync(CancellationToken cancellationToken = default) =>
            CaptureWindowAsync(GetActiveWindowHandle(), transparent: false, cancellationToken);

        /// <summary>The window with its transparent corners and, with <see cref="CaptureShadow"/>, its shadow, where the platform supports it.</summary>
        public Task<SKBitmap?> CaptureWindowTransparentAsync(IntPtr handle, CancellationToken cancellationToken = default) =>
            CaptureWindowAsync(handle, transparent: true, cancellationToken);

        public Task<SKBitmap?> CaptureActiveWindowTransparentAsync(CancellationToken cancellationToken = default) =>
            CaptureWindowAsync(GetActiveWindowHandle(), transparent: true, cancellationToken);

        // Synchronous forms for callers that cannot await. Windows captures complete synchronously; elsewhere these block the caller
        // while the platform captures, which may launch a helper program, so prefer the async methods.
        public SKBitmap? CaptureRectangle(Rectangle rect) => CaptureRectangleAsync(rect).GetAwaiter().GetResult();

        public SKBitmap? CaptureFullscreen() => CaptureFullscreenAsync().GetAwaiter().GetResult();

        public SKBitmap? CaptureActiveMonitor() => CaptureActiveMonitorAsync().GetAwaiter().GetResult();

        public SKBitmap? CaptureWindow(IntPtr handle) => CaptureWindowAsync(handle).GetAwaiter().GetResult();

        public SKBitmap? CaptureActiveWindow() => CaptureActiveWindowAsync().GetAwaiter().GetResult();

        public SKBitmap? CaptureWindowTransparent(IntPtr handle) => CaptureWindowTransparentAsync(handle).GetAwaiter().GetResult();

        /// <summary>
        /// Recaptures <paramref name="handle"/> with its rounded corners and shadow, then keeps the opaque pixels of
        /// <paramref name="windowImage"/> (the region capture of <paramref name="windowRectangle"/>, annotations included).
        /// Null when the window moved or this system cannot do it.
        /// </summary>
        public SKBitmap? CaptureWindowTransparent(IntPtr handle, SKBitmap windowImage, Rectangle windowRectangle)
        {
            PixelBuffer? interior = PlatformImageConverter.ToPixelBuffer(windowImage);

            if (interior == null || !PlatformServices.IsInitialized)
            {
                return null;
            }

            ScreenCaptureResult? result = Service.CaptureWindowPreservingInterior(
                handle.ToInt64(),
                interior,
                new PlatformRectangle(windowRectangle.X, windowRectangle.Y, windowRectangle.Width, windowRectangle.Height),
                CaptureShadow,
                ShadowOffset,
                AutoHideTaskbar);

            return result == null ? null : PlatformImageConverter.ToSKBitmap(result);
        }

        public SKBitmap? CaptureActiveWindowTransparent() => CaptureActiveWindowTransparentAsync().GetAwaiter().GetResult();

        private Task<SKBitmap?> CaptureWindowAsync(IntPtr handle, bool transparent, CancellationToken cancellationToken)
        {
            if (handle == IntPtr.Zero)
            {
                return Task.FromResult<SKBitmap?>(null);
            }

            return CaptureAsync(CreateRequest() with
            {
                Mode = ScreenCaptureMode.Window,
                WindowHandle = handle.ToInt64(),
                Window = new WindowCaptureOptions
                {
                    ClientAreaOnly = CaptureClientArea && !transparent,
                    Transparent = transparent,
                    IncludeShadow = CaptureShadow,
                    ShadowOffset = ShadowOffset,
                    HideTaskbar = AutoHideTaskbar
                }
            }, cancellationToken);
        }

        private static IntPtr GetActiveWindowHandle()
        {
            PlatformWindow? window = PlatformServices.Current.Windows.GetActiveWindow();
            return window != null ? (IntPtr)window.Handle : IntPtr.Zero;
        }

        private ScreenCaptureRequest CreateRequest() => new ScreenCaptureRequest
        {
            IncludeCursor = CaptureCursor,
            ClipToScreens = RemoveOutsideScreenArea,
            HdrToneMapping = HDRScreenshotColorCorrection
        };

        private static async Task<SKBitmap?> CaptureAsync(ScreenCaptureRequest request, CancellationToken cancellationToken)
        {
            ScreenCaptureResult result;

            try
            {
                result = await Service.CaptureAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (ArgumentException e)
            {
                // A window that closed or an area outside every screen: nothing to capture, as before.
                DebugHelper.WriteException(e);
                return null;
            }

            SKBitmap? bitmap = PlatformImageConverter.ToSKBitmap(result);

            if (bitmap != null && request.Mode != ScreenCaptureMode.Window)
            {
                MakeOpaque(bitmap);
            }

            return bitmap;
        }

        // A screen has no transparency, but not every platform says so: X11 leaves the alpha byte of 24-bit visuals at 0, and
        // portal and screencapture PNGs may carry an alpha channel. Region capture draws screen captures directly as opaque.
        private static unsafe void MakeOpaque(SKBitmap bitmap)
        {
            if (bitmap.ColorType is not (SKColorType.Bgra8888 or SKColorType.Rgba8888) || bitmap.AlphaType == SKAlphaType.Opaque)
            {
                return;
            }

            int width = bitmap.Width, height = bitmap.Height, rowBytes = bitmap.RowBytes;
            byte* pixels = (byte*)bitmap.GetPixels();

            for (int y = 0; y < height; y++)
            {
                PixelBuffer.MakeOpaque(new Span<byte>(pixels + (nint)y * rowBytes, width * 4));
            }

            bitmap.NotifyPixelsChanged();
        }
    }
}
