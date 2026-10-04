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

using ShareX.Platform.Linux.Native;
using System;
using System.Runtime.InteropServices;

namespace ShareX.Platform.Linux;

/// <summary>
/// An X11 overlay: an override-redirect window with a 32 bit ARGB visual, so the compositing manager blends it over other
/// windows, and an empty input shape, so every click goes to the window below. Frames are premultiplied BGRA, which is the byte
/// order of a 32 bit ZPixmap on little endian machines. Each overlay owns its X connection and is used from the UI thread.
/// </summary>
internal sealed unsafe class X11ScreenOverlay : IScreenOverlay
{
    private readonly X11Display display;
    private readonly nuint window;
    private readonly nuint colormap;
    private readonly IntPtr gc;
    private readonly X11.XVisualInfo visual;
    private IntPtr pixels;
    private int bufferWidth;
    private int bufferHeight;
    private bool visible;

    public X11ScreenOverlay(PlatformRectangle screenBounds)
    {
        display = X11Display.TryOpen() ?? throw new PlatformNotSupportedException("No X server is available.");

        try
        {
            if (X11.XMatchVisualInfo(display.Display, X11.XDefaultScreen(display.Display), 32, X11.TrueColor, out visual) == 0)
            {
                throw new PlatformNotSupportedException("The X server has no 32 bit visual for transparent windows.");
            }

            colormap = X11.XCreateColormap(display.Display, display.Root, visual.visual, X11.AllocNone);
            X11.XSetWindowAttributes attributes = new X11.XSetWindowAttributes { override_redirect = 1, colormap = colormap };
            window = X11.XCreateWindow(display.Display, display.Root, screenBounds.X, screenBounds.Y, 1, 1, 0, 32, X11.InputOutput,
                visual.visual, X11.CWOverrideRedirect | X11.CWColormap | X11.CWBackPixel | X11.CWBorderPixel, &attributes);

            // An empty input region: the overlay never receives the mouse.
            X11.XShapeCombineRectangles(display.Display, window, X11.ShapeInput, 0, 0, null, 0, X11.ShapeSet, X11.Unsorted);
            gc = X11.XCreateGC(display.Display, window, 0, IntPtr.Zero);
            X11.XFlush(display.Display);
        }
        catch
        {
            display.Dispose();
            throw;
        }
    }

    /// <summary>Whether a compositing manager runs, without which ARGB windows are not blended and the overlay would hide the desktop.</summary>
    internal static bool HasCompositingManager(X11Display display) =>
        X11.XGetSelectionOwner(display.Display, display.GetAtom("_NET_WM_CM_S" + X11.XDefaultScreen(display.Display))) != 0;

    public OverlayBuffer GetBuffer(int width, int height)
    {
        if (pixels == IntPtr.Zero || width > bufferWidth || height > bufferHeight || width < bufferWidth / 4 || height < bufferHeight / 4)
        {
            NativeMemory.Free((void*)pixels);
            bufferWidth = (width + 63) / 64 * 64;
            bufferHeight = (height + 63) / 64 * 64;
            pixels = (IntPtr)NativeMemory.AllocZeroed((nuint)bufferWidth * (nuint)bufferHeight * 4);
        }

        return new OverlayBuffer(pixels, bufferWidth * 4, bufferHeight);
    }

    public void Present(PlatformRectangle area)
    {
        if (pixels == IntPtr.Zero || area.Width <= 0 || area.Height <= 0)
        {
            return;
        }

        int width = Math.Min(area.Width, bufferWidth);
        int height = Math.Min(area.Height, bufferHeight);
        X11.XMoveResizeWindow(display.Display, window, area.X, area.Y, (uint)width, (uint)height);

        if (!visible)
        {
            X11.XMapRaised(display.Display, window);
            visible = true;
        }
        else
        {
            X11.XRaiseWindow(display.Display, window);
        }

        X11.XImage image = new X11.XImage
        {
            width = bufferWidth,
            height = bufferHeight,
            format = X11.ZPixmap,
            data = pixels,
            byte_order = 0, // LSBFirst
            bitmap_unit = 32,
            bitmap_bit_order = 0,
            bitmap_pad = 32,
            depth = 32,
            bytes_per_line = bufferWidth * 4,
            bits_per_pixel = 32,
            red_mask = visual.red_mask,
            green_mask = visual.green_mask,
            blue_mask = visual.blue_mask
        };
        X11.XInitImage(&image);
        X11.XPutImage(display.Display, window, gc, &image, 0, 0, 0, 0, (uint)width, (uint)height);
        X11.XFlush(display.Display);
    }

    public void Hide()
    {
        if (visible)
        {
            X11.XUnmapWindow(display.Display, window);
            X11.XFlush(display.Display);
            visible = false;
        }
    }

    public void Dispose()
    {
        if (gc != IntPtr.Zero) X11.XFreeGC(display.Display, gc);
        if (window != 0) X11.XDestroyWindow(display.Display, window);
        if (colormap != 0) X11.XFreeColormap(display.Display, colormap);
        NativeMemory.Free((void*)pixels);
        pixels = IntPtr.Zero;
        display.Dispose();
    }
}
