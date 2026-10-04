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
using ShareX.Platform.Linux;
using ShareX.Platform.Linux.Native;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Xunit;

namespace ShareX.Platform.Tests;

/// <summary>
/// Runs against a real X server named by SHAREX_X11_TEST_DISPLAY, for example a rootful "Xwayland :5". DISPLAY must name the same
/// server when the test process starts: Xlib reads the native environment, so setting it from .NET would leave the tests on the
/// user's own display, where they move the pointer and click.
/// </summary>
public sealed class X11ServerFactAttribute : FactAttribute
{
    public X11ServerFactAttribute()
    {
        string? test = Environment.GetEnvironmentVariable("SHAREX_X11_TEST_DISPLAY");

        if (!OperatingSystem.IsLinux() || string.IsNullOrEmpty(test) || Environment.GetEnvironmentVariable("DISPLAY") != test)
        {
            Skip = "Run with DISPLAY and SHAREX_X11_TEST_DISPLAY both set to a test X server, such as a rootful Xwayland :5.";
        }
    }
}

[Collection("X11 server")]
public class X11ServerTests
{
    private sealed class Listener : IGlobalMouseListener
    {
        public ConcurrentQueue<PlatformPoint> Moves { get; } = new();
        public ConcurrentQueue<GlobalMouseButtonEvent> Buttons { get; } = new();
        public void OnMove(PlatformPoint position) => Moves.Enqueue(position);
        public void OnButton(GlobalMouseButtonEvent buttonEvent) => Buttons.Enqueue(buttonEvent);
    }

    [X11ServerFact]
    public void Overlay_DrawsItsBufferAtTheRequestedPlace()
    {
        using X11Display display = X11Display.TryOpen()!;
        using X11ScreenOverlay overlay = new X11ScreenOverlay(display.GetRootBounds());

        OverlayBuffer buffer = overlay.GetBuffer(100, 100);
        // Opaque red, premultiplied BGRA.
        byte[] row = Enumerable.Range(0, 100).SelectMany(_ => new byte[] { 0, 0, 255, 255 }).ToArray();
        for (int y = 0; y < 100; y++)
        {
            Marshal.Copy(row, 0, buffer.Pixels + y * buffer.Stride, row.Length);
        }

        overlay.Present(new PlatformRectangle(200, 150, 100, 100));
        Thread.Sleep(300);
        PixelBuffer inside = display.CaptureRoot(new PlatformRectangle(250, 200, 1, 1), false);
        overlay.Hide();
        Thread.Sleep(300);
        PixelBuffer after = display.CaptureRoot(new PlatformRectangle(250, 200, 1, 1), false);

        Assert.Equal(new byte[] { 0, 0, 255 }, inside.Pixels[..3]);
        Assert.NotEqual(new byte[] { 0, 0, 255 }, after.Pixels[..3]);
    }

    [X11ServerFact]
    public void MouseHook_ReportsPointerMovesAndButtons()
    {
        using X11Display display = X11Display.TryOpen()!;
        Listener listener = new Listener();

        using (new X11MouseHook(listener))
        {
            Thread.Sleep(100);
            X11.XWarpPointer(display.Display, 0, display.Root, 0, 0, 0, 0, 321, 123);
            X11.XFlush(display.Display);
            Thread.Sleep(200);
            X11.XTestFakeButtonEvent(display.Display, 3, true, 0);
            X11.XFlush(display.Display);
            Thread.Sleep(200);
            X11.XTestFakeButtonEvent(display.Display, 3, false, 0);
            X11.XFlush(display.Display);
            Thread.Sleep(200);
        }

        Assert.Contains(new PlatformPoint(321, 123), listener.Moves);
        Assert.Equal([true, false], listener.Buttons.Where(b => b.Button == GlobalMouseButton.Secondary).Select(b => b.Pressed));
    }
}
