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

using ShareX.AvaloniaUI.Integration;
using ShareX.Platform;
using System;
using System.Drawing;
using System.Linq;

namespace ShareX.HelpersLib;

/// <summary>A display snapshot in screenshot coordinates.</summary>
/// <remarks>
/// The platform's screen list comes first because captures use its coordinates: physical pixels on Windows and X11, layout
/// coordinates on Hyprland and sway, where Avalonia's own list uses different units. GNOME and KDE on Wayland do not reveal the
/// layout to applications, so Avalonia's list is used there.
/// </remarks>
public sealed class DesktopScreen
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(1);
    private static readonly object CacheLock = new();
    private static DesktopScreen[] cachedPlatformScreens;
    private static DateTime cachedAt;

    public Rectangle Bounds { get; }
    public Rectangle WorkingArea { get; }
    public bool Primary { get; }
    /// <summary>The platform's name for the monitor: \\.\DISPLAY2 on Windows, the output such as DP-1 elsewhere; null when unknown.</summary>
    public string DeviceName { get; }
    /// <summary>Monitor model name, when known; several monitors can share it.</summary>
    public string DisplayName { get; }

    private DesktopScreen(Avalonia.Platform.Screen screen)
    {
        Bounds = new Rectangle(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);
        WorkingArea = new Rectangle(screen.WorkingArea.X, screen.WorkingArea.Y, screen.WorkingArea.Width, screen.WorkingArea.Height);
        Primary = screen.IsPrimary;
        DeviceName = GetDeviceName(screen);
        DisplayName = screen.DisplayName;
    }

    private DesktopScreen(ScreenInfo screen)
    {
        Bounds = new Rectangle(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);
        WorkingArea = screen.WorkingArea.IsEmpty ? Bounds :
            new Rectangle(screen.WorkingArea.X, screen.WorkingArea.Y, screen.WorkingArea.Width, screen.WorkingArea.Height);
        Primary = screen.IsPrimary;
        DeviceName = screen.Id;
        DisplayName = screen.Name == screen.Id ? null : screen.Name;
    }

    /// <summary>The platform's name for an Avalonia screen (\\.\DISPLAY2 on Windows, the output such as DP-1 elsewhere), or null.</summary>
    /// <remarks>Matched by bounds, which both lists give in physical pixels on Windows and X11, then by name.</remarks>
    public static string GetDeviceName(Avalonia.Platform.Screen screen)
    {
        DesktopScreen[] screens = GetPlatformScreens();
        if (screens == null) return null;

        Rectangle bounds = new Rectangle(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);
        return (screens.FirstOrDefault(x => x.Bounds == bounds) ??
            screens.FirstOrDefault(x => !string.IsNullOrEmpty(screen.DisplayName) && x.DeviceName == screen.DisplayName))?.DeviceName;
    }

    /// <summary>
    /// Monitor model names by <see cref="DeviceName"/>, from Avalonia, for the monitors the platform's list does not name
    /// (Windows reports only \\.\DISPLAY2 there).
    /// </summary>
    public static System.Collections.Generic.IReadOnlyDictionary<string, string> GetModelNames() => DesktopServices.Run(() =>
    {
        System.Collections.Generic.Dictionary<string, string> names = new(StringComparer.OrdinalIgnoreCase);

        foreach (Avalonia.Platform.Screen screen in DesktopServices.GetWindow().Screens.All)
        {
            string deviceName = GetDeviceName(screen);

            if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(screen.DisplayName))
            {
                names[deviceName] = screen.DisplayName;
            }
        }

        return names;
    });

    public static DesktopScreen[] AllScreens => GetPlatformScreens() ?? DesktopServices.Run(() =>
        DesktopServices.GetWindow().Screens.All.Select(x => new DesktopScreen(x)).ToArray());

    public static DesktopScreen PrimaryScreen => AllScreens.FirstOrDefault(x => x.Primary) ?? AllScreens.FirstOrDefault();

    public static DesktopScreen FromPoint(Point point)
    {
        DesktopScreen[] screens = GetPlatformScreens();

        if (screens != null)
        {
            return screens.FirstOrDefault(x => x.Bounds.Contains(point)) ?? Nearest(screens, new Rectangle(point, new Size(1, 1)));
        }

        return DesktopServices.Run(() =>
        {
            var avaloniaScreens = DesktopServices.GetWindow().Screens;
            var screen = avaloniaScreens.ScreenFromPoint(new Avalonia.PixelPoint(point.X, point.Y)) ?? avaloniaScreens.Primary;
            return screen == null ? null : new DesktopScreen(screen);
        });
    }

    public static DesktopScreen FromRectangle(Rectangle bounds)
    {
        DesktopScreen[] screens = GetPlatformScreens();

        if (screens != null)
        {
            return Nearest(screens, bounds);
        }

        return DesktopServices.Run(() =>
        {
            var avaloniaScreens = DesktopServices.GetWindow().Screens;
            var screen = avaloniaScreens.ScreenFromBounds(new Avalonia.PixelRect(bounds.X, bounds.Y, bounds.Width, bounds.Height)) ?? avaloniaScreens.Primary;
            return screen == null ? null : new DesktopScreen(screen);
        });
    }

    /// <summary>The screen sharing the largest area with <paramref name="bounds"/>, or the primary screen.</summary>
    private static DesktopScreen Nearest(DesktopScreen[] screens, Rectangle bounds)
    {
        DesktopScreen best = null;
        long bestArea = 0;

        foreach (DesktopScreen screen in screens)
        {
            Rectangle overlap = Rectangle.Intersect(screen.Bounds, bounds);
            long area = (long)overlap.Width * overlap.Height;

            if (area > bestArea)
            {
                best = screen;
                bestArea = area;
            }
        }

        return best ?? screens.FirstOrDefault(x => x.Primary) ?? screens[0];
    }

    /// <summary>The platform's screens, cached briefly because some platforms ask the compositor; null when it has none.</summary>
    private static DesktopScreen[] GetPlatformScreens()
    {
        if (!PlatformServices.IsInitialized)
        {
            return null;
        }

        lock (CacheLock)
        {
            if (cachedPlatformScreens == null || DateTime.UtcNow - cachedAt > CacheDuration)
            {
                cachedPlatformScreens = PlatformServices.Current.ScreenCapture.GetScreens().Select(x => new DesktopScreen(x)).ToArray();
                cachedAt = DateTime.UtcNow;
            }

            return cachedPlatformScreens.Length > 0 ? cachedPlatformScreens : null;
        }
    }

    /// <summary>The screen holding most of the window, or the primary screen when the platform cannot locate the window.</summary>
    public static DesktopScreen FromHandle(IntPtr handle)
    {
        PlatformWindow window = PlatformServices.IsInitialized
            ? PlatformServices.Current.Windows.GetWindows().FirstOrDefault(x => x.Handle == handle.ToInt64())
            : null;

        return window != null
            ? FromRectangle(new Rectangle(window.Bounds.X, window.Bounds.Y, window.Bounds.Width, window.Bounds.Height))
            : PrimaryScreen;
    }
}
