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

using Avalonia.Controls;
using Avalonia.Threading;
using ShareX.Platform;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ShareX.HelpersLib;

/// <summary>
/// Remembers where the last few capture overlays actually opened, so display diagnostics can compare the window Windows
/// created with the one ShareX asked for. Mixed scaling, app bars and odd monitor arrangements show up as a difference.
/// </summary>
public static class WindowGeometryLog
{
    private const int MaximumEntries = 10;
    private static readonly List<WindowGeometryEntry> entries = new();
    private static readonly Dictionary<long, WindowDpiChanges> dpiChanges = new();
    private static readonly object entriesLock = new();

    static WindowGeometryLog()
    {
        OverlayDpiHub.DpiChanged += (handle, dpi, suppressed) => NoteDpiChange(handle, dpi, suppressed);
        OverlayDpiHub.WindowDestroyed += NoteWindowDestroyed;
    }

    public static IReadOnlyList<WindowGeometryEntry> Entries
    {
        get
        {
            lock (entriesLock)
            {
                return entries.ToArray();
            }
        }
    }

    /// <summary>
    /// Notes a WM_DPICHANGED the window received. Windows changes the window's DPI even when the message is dropped, so a
    /// suppressed change leaves Avalonia and Windows using different scales for the same window.
    /// </summary>
    public static void NoteDpiChange(long handle, uint newDpi, bool suppressed)
    {
        lock (entriesLock)
        {
            if (!dpiChanges.TryGetValue(handle, out WindowDpiChanges? changes))
            {
                changes = new WindowDpiChanges();
                dpiChanges[handle] = changes;
            }

            changes.Add(newDpi, suppressed);
        }
    }

    /// <summary>Stops tracking DPI changes for a destroyed window, so a reused handle starts clean.</summary>
    public static void NoteWindowDestroyed(long handle)
    {
        lock (entriesLock)
        {
            dpiChanges.Remove(handle);
        }
    }

    /// <summary>
    /// Records the window once layout has settled. An empty expected size means only the position is checked. The optional
    /// checks compare where parts of the window should be on screen with where they are, in physical pixels.
    /// </summary>
    public static void Record(Window window, string source, Rectangle expectedBounds, Size imageSize = default,
        Func<IEnumerable<WindowGeometryCheck>>? checks = null, bool trackDpiChanges = false)
    {
        if (!PlatformServices.IsInitialized)
        {
            return;
        }

        DateTime openedTime = DateTime.Now;

        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                IntPtr handle = window.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
                NativeWindowGeometry? native = handle == IntPtr.Zero
                    ? null
                    : PlatformServices.Current.Windows.ReadNativeWindowGeometry(handle.ToInt64());

                if (native == null)
                {
                    return;
                }

                WindowGeometryEntry entry = new()
                {
                    Time = openedTime,
                    Source = source,
                    ExpectedBounds = expectedBounds,
                    ImageSize = imageSize,
                    ActualBounds = Rectangle.FromLTRB(native.Value.Bounds.Left, native.Value.Bounds.Top, native.Value.Bounds.Right, native.Value.Bounds.Bottom),
                    WindowDpi = native.Value.Dpi,
                    RenderScaling = window.RenderScaling,
                    ClientSize = new SizeF((float)window.ClientSize.Width, (float)window.ClientSize.Height),
                    MonitorDeviceName = native.Value.MonitorDeviceName
                };

                if (native.Value.HasClientOrigin)
                {
                    entry.ClientOrigin = new Point(native.Value.ClientOrigin.X, native.Value.ClientOrigin.Y);
                }

                // Where Avalonia thinks the cursor is inside the window, against where the system says it is. A difference
                // means pointer input and the screenshot are mapped with different scales.
                if (native.Value.HasCursor)
                {
                    entry.CursorScreen = new Point(native.Value.Cursor.X, native.Value.Cursor.Y);
                    Avalonia.Point client = Avalonia.VisualExtensions.PointToClient(window, new Avalonia.PixelPoint(native.Value.Cursor.X, native.Value.Cursor.Y));
                    entry.CursorAvaloniaClientPixels = new PointF(
                        (float)(client.X * window.RenderScaling), (float)(client.Y * window.RenderScaling));
                }

                if (checks != null)
                {
                    entry.Checks = new List<WindowGeometryCheck>(checks());
                }

                lock (entriesLock)
                {
                    // Share the window's tracker with the entry, so DPI changes after this point still reach the report.
                    if (!dpiChanges.TryGetValue(handle.ToInt64(), out WindowDpiChanges? changes) && trackDpiChanges)
                    {
                        changes = new WindowDpiChanges();
                        dpiChanges[handle.ToInt64()] = changes;
                    }

                    entry.DpiChanges = changes;
                    entries.Add(entry);
                    if (entries.Count > MaximumEntries)
                    {
                        entries.RemoveAt(0);
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "Failed to record the window geometry.");
            }
        }, DispatcherPriority.Background);
    }

    /// <summary>The scale the system uses for the window; null where it is not available.</summary>
    public static double? GetWindowScaling(IntPtr handle)
    {
        if (!PlatformServices.IsInitialized || handle == IntPtr.Zero)
        {
            return null;
        }

        uint dpi = PlatformServices.Current.Windows.ReadNativeWindowGeometry(handle.ToInt64())?.Dpi ?? 0;
        return dpi > 0 ? dpi / 96.0 : null;
    }
}

public sealed class WindowGeometryEntry
{
    public DateTime Time { get; init; }
    public string Source { get; init; } = string.Empty;
    /// <summary>The window rectangle ShareX asked for, in physical pixels.</summary>
    public Rectangle ExpectedBounds { get; init; }
    /// <summary>GetWindowRect after layout settled, in physical pixels.</summary>
    public Rectangle ActualBounds { get; init; }
    public Point ClientOrigin { get; set; }
    /// <summary>Size of the screenshot shown in the window, when it shows one.</summary>
    public Size ImageSize { get; init; }
    public uint WindowDpi { get; init; }
    public double RenderScaling { get; init; }
    /// <summary>Avalonia client size in device-independent pixels.</summary>
    public SizeF ClientSize { get; init; }
    public string MonitorDeviceName { get; init; } = string.Empty;
    public Point? CursorScreen { get; set; }
    /// <summary>The cursor inside the window as Avalonia maps it, converted to physical pixels with the render scaling.</summary>
    public PointF? CursorAvaloniaClientPixels { get; set; }
    public IReadOnlyList<WindowGeometryCheck> Checks { get; set; } = Array.Empty<WindowGeometryCheck>();
    /// <summary>WM_DPICHANGED messages the window received; updated while the window stays open.</summary>
    public WindowDpiChanges? DpiChanges { get; set; }
}

/// <summary>Where a part of a window should be and where it is, in physical screen pixels.</summary>
public sealed record WindowGeometryCheck(string Name, Point Expected, Point Actual, bool CompareX = true, bool CompareY = true)
{
    public bool Matches(int tolerance = 2) =>
        (!CompareX || Math.Abs(Expected.X - Actual.X) <= tolerance) && (!CompareY || Math.Abs(Expected.Y - Actual.Y) <= tolerance);
}

public sealed class WindowDpiChanges
{
    private readonly object changesLock = new();
    private readonly List<string> history = new();

    public int Received { get; private set; }
    public int Suppressed { get; private set; }

    /// <summary>The DPI values in the order they arrived, marked when ShareX dropped them.</summary>
    public string History
    {
        get
        {
            lock (changesLock)
            {
                return string.Join(", ", history);
            }
        }
    }

    internal void Add(uint dpi, bool suppressed)
    {
        lock (changesLock)
        {
            Received++;
            if (suppressed)
            {
                Suppressed++;
            }

            if (history.Count < 20)
            {
                history.Add(suppressed ? $"{dpi} (dropped)" : dpi.ToString());
            }
        }
    }
}
