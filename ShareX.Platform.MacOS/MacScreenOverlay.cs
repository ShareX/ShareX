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

using ShareX.Platform.MacOS.Native;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ShareX.Platform.MacOS;

/// <summary>
/// A borderless, transparent NSWindow above everything (screen saver level, on every Space) that ignores the mouse, showing each
/// frame as a CGImage on its layer. Frames are premultiplied BGRA, which CoreGraphics reads as 32 bit little endian with alpha
/// first. AppKit windows belong to the main thread, which is Avalonia's UI thread on macOS; use the overlay from there.
/// </summary>
[SupportedOSPlatform("macos")]
internal sealed unsafe class MacScreenOverlay : IScreenOverlay
{
    private const nuint NSWindowStyleMaskBorderless = 0;
    private const nuint NSBackingStoreBuffered = 2;
    private const nint NSScreenSaverWindowLevel = 1000;
    // canJoinAllSpaces | stationary | ignoresCycle | fullScreenAuxiliary
    private const nuint CollectionBehavior = 1 | 16 | 64 | 256;

    private readonly IntPtr window;
    private IntPtr pixels;
    private int bufferWidth;
    private int bufferHeight;
    private bool visible;

    public MacScreenOverlay(PlatformRectangle screenBounds)
    {
        window = ObjC.WithAutoreleasePool(() =>
        {
            IntPtr created = ObjC.SendInitWithRect(ObjC.Send(ObjC.GetClass("NSWindow"), "alloc"), "initWithContentRect:styleMask:backing:defer:",
                ToCocoa(new PlatformRectangle(screenBounds.X, screenBounds.Y, 1, 1)), NSWindowStyleMaskBorderless, NSBackingStoreBuffered, false);
            ObjC.SendBoolArg(created, "setReleasedWhenClosed:", false);
            ObjC.SendBoolArg(created, "setOpaque:", false);
            ObjC.SendBoolArg(created, "setHasShadow:", false);
            ObjC.SendBoolArg(created, "setIgnoresMouseEvents:", true);
            ObjC.Send(created, "setBackgroundColor:", ObjC.Send(ObjC.GetClass("NSColor"), "clearColor"));
            ObjC.Send(created, "setLevel:", NSScreenSaverWindowLevel);
            ObjC.Send(created, "setCollectionBehavior:", (IntPtr)CollectionBehavior);
            IntPtr view = ObjC.Send(created, "contentView");
            ObjC.SendBoolArg(view, "setWantsLayer:", true);
            return created;
        });
    }

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

        ObjC.WithAutoreleasePool(() =>
        {
            IntPtr image = CreateImage(width, height);

            try
            {
                ObjC.SendRectBool(window, "setFrame:display:", ToCocoa(new PlatformRectangle(area.X, area.Y, width, height)), false);
                IntPtr layer = ObjC.Send(ObjC.Send(window, "contentView"), "layer");
                ObjC.Send(layer, "setContents:", image);

                if (!visible)
                {
                    ObjC.Send(window, "orderFrontRegardless");
                    visible = true;
                }
            }
            finally
            {
                // The layer retains the image it shows.
                CoreGraphics.CGImageRelease(image);
            }

            return true;
        });
    }

    public void Hide()
    {
        if (visible)
        {
            ObjC.Send(window, "orderOut:", IntPtr.Zero);
            visible = false;
        }
    }

    public void Dispose()
    {
        if (window != IntPtr.Zero)
        {
            ObjC.Send(window, "orderOut:", IntPtr.Zero);
            ObjC.Send(window, "close");
            ObjC.Send(window, "release");
        }

        NativeMemory.Free((void*)pixels);
        pixels = IntPtr.Zero;
    }

    private IntPtr CreateImage(int width, int height)
    {
        int stride = bufferWidth * 4;
        IntPtr data = CoreFoundation.CreateData(new ReadOnlySpan<byte>((void*)pixels, stride * height));
        IntPtr provider = CoreGraphics.CGDataProviderCreateWithCFData(data);
        IntPtr space = CoreGraphics.CGColorSpaceCreateDeviceRGB();

        try
        {
            return CoreGraphics.CGImageCreate((nuint)width, (nuint)height, 8, 32, (nuint)stride, space,
                CoreGraphics.kCGImageAlphaPremultipliedFirst | CoreGraphics.kCGBitmapByteOrder32Little, provider, IntPtr.Zero, false, 0);
        }
        finally
        {
            CoreGraphics.CGColorSpaceRelease(space);
            CoreGraphics.CGDataProviderRelease(provider);
            CoreFoundation.CFRelease(data);
        }
    }

    /// <summary>Global display coordinates (top left origin) to Cocoa screen coordinates (bottom left of the main display).</summary>
    internal static CoreGraphics.CGRect ToCocoa(PlatformRectangle area) => ToCocoa(area, CoreGraphics.CGDisplayBounds(CoreGraphics.CGMainDisplayID()).Height);

    internal static CoreGraphics.CGRect ToCocoa(PlatformRectangle area, double mainDisplayHeight) =>
        new CoreGraphics.CGRect { X = area.X, Y = mainDisplayHeight - area.Y - area.Height, Width = area.Width, Height = area.Height };
}
