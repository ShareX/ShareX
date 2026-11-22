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
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

internal static class WaylandLayerOverlayHelper
{
    public static bool TryConfigure(Window window, DrawingRectangle screenBounds, bool activeMonitorMode, string overlayNamespace)
    {
        if (!OperatingSystem.IsLinux() ||
            !WaylandProperties.IsWaylandBackend(window) ||
            !WaylandProperties.IsLayerShellSupported(window))
        {
            return false;
        }

        IReadOnlyList<Screen>? screens = window.Screens?.All;
        string? targetOutputName = null;

        if (screens == null || screens.Count <= 1)
        {
            targetOutputName = screens is { Count: 1 } ? screens[0].DisplayName : null;
        }
        else
        {
            Screen? singleMatch = null;
            int intersectingCount = 0;

            for (int i = 0; i < screens.Count; i++)
            {
                Screen screen = screens[i];
                if (Intersects(screen.Bounds, screenBounds))
                {
                    singleMatch = screen;
                    intersectingCount++;
                }
            }

            if (intersectingCount == 1)
            {
                targetOutputName = singleMatch?.DisplayName;
            }
            else if (intersectingCount == 0 && activeMonitorMode)
            {
                Screen closest = FindClosestScreen(screens, screenBounds);
                targetOutputName = closest.DisplayName;
            }
            else
            {
                // Multi-monitor spanning uses PlatformOverlayWindowHost + WaylandLayerOverlayWindow
                // so pointer drags and frames stay unified across output seams.
                return false;
            }
        }

        WaylandProperties.SetLayer(window, WaylandLayer.Overlay);
        WaylandProperties.SetLayerAnchor(window, WaylandLayerAnchor.All);
        WaylandProperties.SetLayerExclusiveZone(window, -1);
        WaylandProperties.SetLayerKeyboardInteractivity(window, WaylandLayerKeyboardInteractivity.Exclusive);
        WaylandProperties.SetLayerNamespace(window, overlayNamespace);

        if (!string.IsNullOrWhiteSpace(targetOutputName))
        {
            WaylandProperties.SetLayerOutputName(window, targetOutputName);
        }

        return true;
    }

    private static bool Intersects(PixelRect a, DrawingRectangle b)
    {
        if (b.Width <= 0 || b.Height <= 0)
        {
            return true;
        }

        int left = Math.Max(a.X, b.X);
        int top = Math.Max(a.Y, b.Y);
        int right = Math.Min(a.X + a.Width, b.Right);
        int bottom = Math.Min(a.Y + a.Height, b.Bottom);
        return right > left && bottom > top;
    }

    private static Screen FindClosestScreen(IReadOnlyList<Screen> screens, DrawingRectangle bounds)
    {
        double targetX = bounds.X + bounds.Width / 2.0;
        double targetY = bounds.Y + bounds.Height / 2.0;

        Screen best = screens[0];
        double bestDistSq = double.MaxValue;

        for (int i = 0; i < screens.Count; i++)
        {
            Screen screen = screens[i];
            PixelRect sb = screen.Bounds;
            double clampedX = Math.Clamp(targetX, sb.X, sb.X + sb.Width);
            double clampedY = Math.Clamp(targetY, sb.Y, sb.Y + sb.Height);
            double dx = targetX - clampedX;
            double dy = targetY - clampedY;
            double distSq = dx * dx + dy * dy;

            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                best = screen;
            }
        }

        return best;
    }
}
