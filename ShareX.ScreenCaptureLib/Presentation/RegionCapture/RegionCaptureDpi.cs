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

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

/// <summary>
/// Places a region-capture overlay on the monitor the system will take its DPI from, then keeps that scale.
/// On Windows that monitor is the one the overlay overlaps most. Creating it at the desktop origin instead left
/// Avalonia and Windows on different scales, so the toolbar and the selection missed the screenshot.
/// </summary>
internal static class RegionCaptureDpi
{
    public static PlatformPoint Anchor(PlatformRectangle bounds) =>
        PlatformServices.IsInitialized
            ? PlatformServices.Current.Windows.GetOverlayDpiAnchor(bounds)
            : new PlatformPoint(bounds.X, bounds.Y);

    public static void PositionAtAnchor(Window window, PlatformRectangle bounds)
    {
        PlatformPoint anchor = Anchor(bounds);
        window.Position = new PixelPoint(anchor.X, anchor.Y);

        if (PlatformServices.IsInitialized && window.TryGetPlatformHandle()?.Handle is IntPtr handle && handle != IntPtr.Zero)
        {
            PlatformServices.Current.Windows.KeepScaleFixed(handle.ToInt64());
        }
    }
}
