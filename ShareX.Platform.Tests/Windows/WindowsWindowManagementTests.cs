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

using ShareX.Platform.Windows;
using ShareX.Platform.Windows.Native;
using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using Xunit;

namespace ShareX.Platform.Tests;

[Collection("Windows platform services")]
public sealed class WindowsWindowManagementTests
{
    [Fact]
    public void BorderlessScreenSelectionUsesTheFullRectangleAcrossUnequalAndGappedDisplays()
    {
        ScreenInfo primary = Screen("primary", new(0, 0, 1000, 1000), true);
        ScreenInfo narrow = Screen("narrow", new(1000, 450, 1000, 100));
        PlatformRectangle straddling = new(700, 0, 800, 1000);
        Assert.True(narrow.Bounds.Contains(new PlatformPoint(1100, 500))); // Its centre is on the smaller intersection.
        Assert.Same(primary, WindowsWindowManagementService.SelectBorderlessScreen(straddling, [narrow, primary]));

        ScreenInfo displaced = Screen("displaced", new(1000, 800, 1000, 200));
        PlatformRectangle gap = new(800, 0, 500, 1000);
        PlatformPoint centre = new(1050, 500);
        Assert.False(primary.Bounds.Contains(centre));
        Assert.False(displaced.Bounds.Contains(centre));
        Assert.Same(primary, WindowsWindowManagementService.SelectBorderlessScreen(gap, [displaced, primary]));

        ScreenInfo negative = Screen("negative", new(-1920, -1080, 1920, 1080));
        Assert.Same(negative, WindowsWindowManagementService.SelectBorderlessScreen(new(-700, -600, 500, 500), [primary, negative]));
    }

    [Fact]
    public void BorderlessScreenSelectionFallsBackToPrimaryThenFirstWhenNoDisplayOverlaps()
    {
        ScreenInfo other = Screen("other", new(-1000, 0, 1000, 1000));
        ScreenInfo primary = Screen("primary", new(0, 0, 1000, 1000), true);
        PlatformRectangle offscreen = new(-32000, -32000, 320, 220);
        Assert.Same(primary, WindowsWindowManagementService.SelectBorderlessScreen(offscreen, [other, primary]));
        Assert.Same(other, WindowsWindowManagementService.SelectBorderlessScreen(offscreen, [other]));
        Assert.Null(WindowsWindowManagementService.SelectBorderlessScreen(offscreen, []));
    }

    private static ScreenInfo Screen(string id, PlatformRectangle bounds, bool primary = false) =>
        new(id, id, bounds, bounds with { Height = bounds.Height - 40 }, primary, 1);

    [WindowsFact]
    public void BorderlessOffscreenWindowUsesPrimaryBoundsAndRestoresItsOriginalFrame()
    {
        if (!OperatingSystem.IsWindows()) return;
        Exception? failure = null;
        Thread thread = new(() =>
        {
            try { VerifyBorderlessWindow(); }
            catch (Exception exception) { failure = exception; }
        }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(15)), "Native borderless verification did not finish.");
        if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
    }

    private static void VerifyBorderlessWindow()
    {
        WindowsScreenCaptureService screens = new();
        ScreenInfo primary = screens.GetScreens().First(screen => screen.IsPrimary);
        WindowsWindowManagementService service = new(screens);
        const uint frame = 0x00cf0000; // WS_OVERLAPPEDWINDOW; deliberately omit WS_VISIBLE.
        const uint edges = 0x00020201; // CLIENTEDGE, STATICEDGE and DLGMODALFRAME.
        IntPtr window = Win32.CreateWindowEx(0x08000080 | edges, "STATIC", "ShareX hidden borderless fixture", frame,
            -32000, -32000, 320, 220, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        Assert.NotEqual(IntPtr.Zero, window);
        try
        {
            Assert.True(Win32.GetWindowRect(window, out Win32.RECT originalRect));
            PlatformRectangle originalBounds = originalRect.ToRectangle();
            Assert.All(screens.GetScreens(), screen => Assert.True(screen.Bounds.Intersect(originalBounds).IsEmpty));
            nint originalStyle = Win32.GetWindowLongPtr(window, Win32.GWL_STYLE);
            nint originalExtendedStyle = Win32.GetWindowLongPtr(window, Win32.GWL_EXSTYLE);
            IntPtr foreground = Win32.GetForegroundWindow();

            foreach (bool workingArea in new[] { false, true })
            {
                Assert.True(service.ToggleBorderless(window.ToInt64(), workingArea));
                Assert.True(Win32.GetWindowRect(window, out Win32.RECT borderlessRect));
                Assert.Equal(workingArea ? primary.WorkingArea : primary.Bounds, borderlessRect.ToRectangle());
                Assert.Equal(originalStyle & ~(nint)(WindowStyles.WS_CAPTION | WindowStyles.WS_MAXIMIZEBOX |
                    WindowStyles.WS_SYSMENU | WindowStyles.WS_THICKFRAME), Win32.GetWindowLongPtr(window, Win32.GWL_STYLE));
                // Windows also updates WINDOWEDGE when the standard frame disappears.
                nint extendedStyle = Win32.GetWindowLongPtr(window, Win32.GWL_EXSTYLE);
                Assert.Equal(originalExtendedStyle & ~(nint)(WindowStyles.WS_EX_CLIENTEDGE | WindowStyles.WS_EX_DLGMODALFRAME |
                    WindowStyles.WS_EX_STATICEDGE | WindowStyles.WS_EX_WINDOWEDGE),
                    extendedStyle & ~(nint)WindowStyles.WS_EX_WINDOWEDGE);
                Assert.False(Win32.IsWindowVisible(window));
                Assert.Equal(foreground, Win32.GetForegroundWindow());

                Assert.True(service.ToggleBorderless(window.ToInt64(), workingArea));
                Assert.True(Win32.GetWindowRect(window, out Win32.RECT restoredRect));
                Assert.Equal(originalBounds, restoredRect.ToRectangle());
                Assert.Equal(originalStyle, Win32.GetWindowLongPtr(window, Win32.GWL_STYLE));
                Assert.Equal(originalExtendedStyle, Win32.GetWindowLongPtr(window, Win32.GWL_EXSTYLE));
                Assert.False(Win32.IsWindowVisible(window));
                Assert.Equal(foreground, Win32.GetForegroundWindow());
            }
        }
        finally
        {
            Assert.True(Win32.DestroyWindow(window));
        }
    }
}
