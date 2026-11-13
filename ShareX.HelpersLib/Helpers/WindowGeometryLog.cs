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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib;

/// <summary>
/// Remembers where the last few capture overlays actually opened, so display diagnostics can compare the window Windows
/// created with the one ShareX asked for. Mixed scaling, app bars and odd monitor arrangements show up as a difference.
/// </summary>
public static class WindowGeometryLog
{
    private const int MaximumEntries = 10;
    private static readonly List<WindowGeometryEntry> entries = new();
    private static readonly object entriesLock = new();

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
    /// Records the window once layout has settled. An empty expected size means only the position is checked.
    /// </summary>
    public static void Record(Window window, string source, Rectangle expectedBounds, Size imageSize = default)
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        DateTime openedTime = DateTime.Now;

        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                IntPtr handle = window.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
                if (handle == IntPtr.Zero)
                {
                    return;
                }

                WindowGeometryEntry entry = new()
                {
                    Time = openedTime,
                    Source = source,
                    ExpectedBounds = expectedBounds,
                    ImageSize = imageSize,
                    ActualBounds = GetWindowRect(handle, out NativeRect windowRect)
                        ? Rectangle.FromLTRB(windowRect.Left, windowRect.Top, windowRect.Right, windowRect.Bottom)
                        : Rectangle.Empty,
                    WindowDpi = GetDpiForWindowSafe(handle),
                    RenderScaling = window.RenderScaling,
                    ClientSize = new SizeF((float)window.ClientSize.Width, (float)window.ClientSize.Height),
                    MonitorDeviceName = NativeMethods.GetMonitorDeviceName(MonitorFromWindow(handle, MonitorDefaultToNearest)) ?? string.Empty
                };

                NativePoint clientOrigin = default;
                if (ClientToScreen(handle, ref clientOrigin))
                {
                    entry.ClientOrigin = new Point(clientOrigin.X, clientOrigin.Y);
                }

                lock (entriesLock)
                {
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

    private static uint GetDpiForWindowSafe(IntPtr handle)
    {
        try
        {
            return GetDpiForWindow(handle);
        }
        catch (EntryPointNotFoundException)
        {
            return 0;
        }
    }

    private const uint MonitorDefaultToNearest = 2;

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr hWnd, out NativeRect rect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ClientToScreen(IntPtr hWnd, ref NativePoint point);

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hWnd, uint flags);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        public int X;
        public int Y;
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
}
