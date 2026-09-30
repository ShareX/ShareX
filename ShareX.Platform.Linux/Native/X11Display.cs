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
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace ShareX.Platform.Linux.Native;

/// <summary>A short lived connection to the X server with helpers for properties, monitors and pixel capture.</summary>
/// <remarks>Each instance owns its connection and must only be used from one thread at a time.</remarks>
internal sealed unsafe class X11Display : IDisposable
{
    private readonly DisplayHandle handle;
    private readonly Dictionary<string, nuint> atoms = new Dictionary<string, nuint>(StringComparer.Ordinal);

    private X11Display(DisplayHandle handle)
    {
        this.handle = handle;
        Root = X11.XDefaultRootWindow(handle.Display);
    }

    public IntPtr Display => handle.Display;

    public nuint Root { get; }

    /// <summary>Opens the display named by $DISPLAY, or returns null when there is no X server (including libX11 missing).</summary>
    public static X11Display? TryOpen()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")))
        {
            return null;
        }

        try
        {
            DisplayHandle handle = X11.OpenDisplay();

            if (handle.IsInvalid)
            {
                handle.Dispose();
                return null;
            }

            return new X11Display(handle);
        }
        catch (DllNotFoundException)
        {
            return null;
        }
    }

    public PlatformPoint? GetPointerPosition() =>
        X11.XQueryPointer(Display, Root, out _, out _, out int x, out int y, out _, out _, out _) != 0 ? new PlatformPoint(x, y) : null;

    public PlatformRectangle GetRootBounds()
    {
        int screen = X11.XDefaultScreen(Display);
        return new PlatformRectangle(0, 0, X11.XDisplayWidth(Display, screen), X11.XDisplayHeight(Display, screen));
    }

    public nuint GetAtom(string name)
    {
        if (!atoms.TryGetValue(name, out nuint atom))
        {
            atom = X11.XInternAtom(Display, name, false);
            atoms[name] = atom;
        }

        return atom;
    }

    public string? GetAtomName(nuint atom)
    {
        IntPtr name = X11.XGetAtomName(Display, atom);

        try
        {
            return X11.PtrToString(name);
        }
        finally
        {
            if (name != IntPtr.Zero) X11.XFree(name);
        }
    }

    /// <summary>Reads a 32 bit format property (CARDINAL, WINDOW, ATOM). Xlib returns those as C longs.</summary>
    public nuint[] GetLongProperty(nuint window, string property, int maxItems = 4096)
    {
        if (X11.XGetWindowProperty(Display, window, GetAtom(property), 0, maxItems, false, X11.AnyPropertyType,
            out _, out int format, out nuint count, out _, out IntPtr data) != X11.Success || data == IntPtr.Zero)
        {
            return Array.Empty<nuint>();
        }

        try
        {
            if (format != 32)
            {
                return Array.Empty<nuint>();
            }

            nuint[] values = new nuint[(int)count];
            new ReadOnlySpan<nuint>((void*)data, (int)count).CopyTo(values);
            return values;
        }
        finally
        {
            X11.XFree(data);
        }
    }

    public string? GetStringProperty(nuint window, string property)
    {
        if (X11.XGetWindowProperty(Display, window, GetAtom(property), 0, 1024, false, X11.AnyPropertyType,
            out _, out int format, out nuint count, out _, out IntPtr data) != X11.Success || data == IntPtr.Zero)
        {
            return null;
        }

        try
        {
            return format == 8 && count > 0 ? Encoding.UTF8.GetString((byte*)data, (int)count) : null;
        }
        finally
        {
            X11.XFree(data);
        }
    }

    public IReadOnlyList<ScreenInfo> GetMonitors()
    {
        List<ScreenInfo> screens = new List<ScreenInfo>();

        try
        {
            X11.XRRMonitorInfo* monitors = X11.XRRGetMonitors(Display, Root, true, out int count);

            if (monitors != null)
            {
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        X11.XRRMonitorInfo monitor = monitors[i];
                        string name = GetAtomName(monitor.name) ?? $"Monitor {i + 1}";
                        PlatformRectangle bounds = new PlatformRectangle(monitor.x, monitor.y, monitor.width, monitor.height);
                        screens.Add(new ScreenInfo(name, name, bounds, bounds, monitor.primary != 0, 1));
                    }
                }
                finally
                {
                    X11.XRRFreeMonitors(monitors);
                }
            }
        }
        catch (DllNotFoundException)
        {
            // libXrandr is missing. Fall back to the root window below.
        }
        catch (EntryPointNotFoundException)
        {
            // XRandR older than 1.5.
        }

        if (screens.Count == 0)
        {
            PlatformRectangle bounds = GetRootBounds();
            screens.Add(new ScreenInfo("default", "Screen", bounds, bounds, true, 1));
        }

        // _NET_WORKAREA covers the whole desktop, so it is only meaningful with a single monitor.
        if (screens.Count == 1)
        {
            nuint[] workArea = GetLongProperty(Root, "_NET_WORKAREA", 4);

            if (workArea.Length == 4)
            {
                screens[0] = screens[0] with { WorkingArea = new PlatformRectangle((int)workArea[0], (int)workArea[1], (int)workArea[2], (int)workArea[3]) };
            }
        }

        return screens;
    }

    /// <summary>Copies an area of the root window, which on X11 contains every visible window.</summary>
    public PixelBuffer CaptureRoot(PlatformRectangle area, bool includeCursor)
    {
        area = area.Intersect(GetRootBounds());

        if (area.IsEmpty)
        {
            throw new ArgumentException("The capture area is outside the screen.", nameof(area));
        }

        X11.XImage* image = X11.XGetImage(Display, Root, area.X, area.Y, (uint)area.Width, (uint)area.Height, X11.AllPlanes, X11.ZPixmap);

        if (image == null)
        {
            throw new InvalidOperationException("XGetImage failed.");
        }

        try
        {
            PixelBuffer buffer = ConvertImage(image);

            if (includeCursor)
            {
                DrawCursor(buffer, area);
            }

            return buffer;
        }
        finally
        {
            X11.DestroyImage(image);
        }
    }

    private static PixelBuffer ConvertImage(X11.XImage* image)
    {
        if (image->bits_per_pixel != 32)
        {
            throw new NotSupportedException($"{image->bits_per_pixel} bit X11 visuals are not supported.");
        }

        // The common case, 24 or 32 bit TrueColor in BGRA byte order, is copied row by row.
        if (image->red_mask == 0xFF0000 && image->green_mask == 0xFF00 && image->blue_mask == 0xFF && image->byte_order == 0)
        {
            return PixelBuffer.FromBgra(image->data, image->width, image->height, image->bytes_per_line, image->depth <= 24);
        }

        PixelBuffer buffer = new PixelBuffer(image->width, image->height);
        int redShift = BitOperations.TrailingZeroCount((ulong)image->red_mask);
        int greenShift = BitOperations.TrailingZeroCount((ulong)image->green_mask);
        int blueShift = BitOperations.TrailingZeroCount((ulong)image->blue_mask);

        for (int y = 0; y < image->height; y++)
        {
            uint* row = (uint*)((byte*)image->data + (long)y * image->bytes_per_line);

            for (int x = 0; x < image->width; x++)
            {
                uint pixel = image->byte_order == 0 ? row[x] : BitConverter.IsLittleEndian ? System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(row[x]) : row[x];
                int offset = y * buffer.Stride + x * 4;
                buffer.Pixels[offset] = (byte)((pixel & (uint)image->blue_mask) >> blueShift);
                buffer.Pixels[offset + 1] = (byte)((pixel & (uint)image->green_mask) >> greenShift);
                buffer.Pixels[offset + 2] = (byte)((pixel & (uint)image->red_mask) >> redShift);
                buffer.Pixels[offset + 3] = 255;
            }
        }

        return buffer;
    }

    private void DrawCursor(PixelBuffer buffer, PlatformRectangle area)
    {
        X11.XFixesCursorImage* cursor;

        try
        {
            cursor = X11.XFixesGetCursorImage(Display);
        }
        catch (DllNotFoundException)
        {
            return;
        }

        if (cursor == null)
        {
            return;
        }

        try
        {
            PixelBuffer cursorImage = new PixelBuffer(cursor->width, cursor->height);

            for (int i = 0; i < cursor->width * cursor->height; i++)
            {
                // Each pixel is a premultiplied ARGB value stored in an unsigned long.
                uint argb = (uint)cursor->pixels[i];
                byte a = (byte)(argb >> 24);
                int o = i * 4;

                if (a == 0) continue;

                cursorImage.Pixels[o] = (byte)Math.Min(255, (argb & 0xFF) * 255 / a);
                cursorImage.Pixels[o + 1] = (byte)Math.Min(255, ((argb >> 8) & 0xFF) * 255 / a);
                cursorImage.Pixels[o + 2] = (byte)Math.Min(255, ((argb >> 16) & 0xFF) * 255 / a);
                cursorImage.Pixels[o + 3] = a;
            }

            buffer.BlendFrom(cursorImage, cursor->x - cursor->xhot - area.X, cursor->y - cursor->yhot - area.Y);
        }
        finally
        {
            X11.XFree((IntPtr)cursor);
        }
    }

    public PlatformRectangle? GetWindowBounds(nuint window, bool includeFrame)
    {
        if (X11.XGetGeometry(Display, window, out _, out _, out _, out uint width, out uint height, out _, out _) == 0)
        {
            X11.TakeLastError();
            return null;
        }

        if (!X11.XTranslateCoordinates(Display, window, Root, 0, 0, out int x, out int y, out _))
        {
            X11.TakeLastError();
            return null;
        }

        PlatformRectangle bounds = new PlatformRectangle(x, y, (int)width, (int)height);

        if (includeFrame)
        {
            // left, right, top, bottom
            nuint[] extents = GetLongProperty(window, "_NET_FRAME_EXTENTS", 4);

            if (extents.Length == 4)
            {
                bounds = PlatformRectangle.FromLTRB(bounds.Left - (int)extents[0], bounds.Top - (int)extents[2],
                    bounds.Right + (int)extents[1], bounds.Bottom + (int)extents[3]);
            }
        }

        return bounds;
    }

    public void Dispose() => handle.Dispose();
}
