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

using Avalonia;
using Avalonia.Controls;
using ShareX.Platform;
using System;
using System.Collections.Generic;

namespace ShareX.ScreenCaptureLib;

/// <summary>Shapes ShareX's capture frame windows so only the frame (and its tool bar) takes clicks, through IWindowService.</summary>
internal static class FrameWindowShape
{
    internal static double NormalizeScaling(double scaling) => double.IsFinite(scaling) && scaling > 0 ? scaling : 1;

    internal static Size GetLogicalSize(int physicalWidth, int physicalHeight, double renderScaling)
    {
        double scaling = NormalizeScaling(renderScaling);
        return new Size(physicalWidth / scaling, physicalHeight / scaling);
    }

    internal static RecordingFrameLayout CreateRecordingLayout(PlatformRectangle recordingRegion, double renderScaling,
        double toolbarWidth, double toolbarHeight, int border, int toolbarGap)
    {
        double scaling = NormalizeScaling(renderScaling);
        int frameWidth = recordingRegion.Width + border * 2;
        int frameHeight = recordingRegion.Height + border * 2;
        int toolbarPixels = (int)Math.Ceiling(toolbarWidth * scaling);
        int contentWidth = Math.Max(frameWidth, toolbarPixels);
        int frameLeft = (contentWidth - frameWidth) / 2;
        int toolbarLeft = (contentWidth - toolbarPixels) / 2;
        return new RecordingFrameLayout(
            new PixelPoint(recordingRegion.X - frameLeft - border, recordingRegion.Y - border),
            contentWidth / scaling, (frameHeight + toolbarGap) / scaling + toolbarHeight, scaling,
            new PlatformRectangle(frameLeft, 0, frameWidth, frameHeight),
            new PlatformRectangle(toolbarLeft, frameHeight + toolbarGap, toolbarPixels, (int)Math.Ceiling(toolbarHeight * scaling)));
    }

    /// <summary>The four border strips of a frame, window relative, plus any extra areas such as a tool bar.</summary>
    public static List<PlatformRectangle> Create(PlatformRectangle frame, int border, params PlatformRectangle[] extras)
    {
        List<PlatformRectangle> areas =
        [
            new PlatformRectangle(frame.X, frame.Y, frame.Width, border),
            new PlatformRectangle(frame.X, frame.Bottom - border, frame.Width, border),
            new PlatformRectangle(frame.X, frame.Y + border, border, frame.Height - (2 * border)),
            new PlatformRectangle(frame.Right - border, frame.Y + border, border, frame.Height - (2 * border))
        ];

        areas.AddRange(extras);
        return areas;
    }

    public static void Apply(Window window, IReadOnlyList<PlatformRectangle> areas)
    {
        long handle = GetHandle(window);

        if (handle != 0)
        {
            PlatformServices.Current.Windows.SetWindowShape(handle, areas);
        }
    }

    public static void SetOverlayStyle(Window window, bool clickThrough)
    {
        long handle = GetHandle(window);

        if (handle != 0)
        {
            PlatformServices.Current.Windows.SetOverlayStyle(handle, clickThrough);
        }
    }

    private static long GetHandle(Window window) => window.TryGetPlatformHandle()?.Handle.ToInt64() ?? 0;
}

internal readonly record struct RecordingFrameLayout(PixelPoint Position, double Width, double Height, double Scaling,
    PlatformRectangle Frame, PlatformRectangle Toolbar);
