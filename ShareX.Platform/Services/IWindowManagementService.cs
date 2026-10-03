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

using System;
using System.Collections.Generic;

namespace ShareX.Platform;

/// <summary>What the Inspect Window tool shows about a window. Null members are not known on this platform.</summary>
/// <param name="ClassName">The Win32 window class, the X11 WM_CLASS or the Wayland app id.</param>
/// <param name="Styles">Window style flags (Win32 WS_*) or window manager states (EWMH, Hyprland), by name.</param>
/// <param name="ExtendedStyles">Win32 extended styles (WS_EX_*). Empty elsewhere.</param>
/// <param name="IsTopMost">Whether the window stays above others; null when it cannot be read or changed.</param>
/// <param name="Opacity">0 to 255; null when it cannot be read or changed.</param>
public sealed record WindowDetails(
    long Handle,
    string Title,
    string? ClassName,
    string? ProcessName,
    string? ProcessPath,
    int? ProcessId,
    PlatformRectangle Bounds,
    PlatformRectangle? ClientBounds,
    IReadOnlyList<string> Styles,
    IReadOnlyList<string> ExtendedStyles,
    bool? IsTopMost,
    byte? Opacity);

/// <summary>Inspecting and changing other applications' windows, for the Inspect Window and Borderless Window tools.</summary>
public interface IWindowManagementService
{
    /// <summary>Whether windows can be inspected at all.</summary>
    FeatureSupport Support { get; }

    /// <summary>The window under <paramref name="point"/>: its top level window, or with <paramref name="topLevel"/> false the control. 0 when unknown.</summary>
    long GetWindowAt(PlatformPoint point, bool topLevel);

    WindowDetails? GetDetails(long windowHandle);

    /// <summary>The window's icon as PNG, or null.</summary>
    byte[]? GetIcon(long windowHandle);

    bool SetTopMost(long windowHandle, bool topMost);

    bool SetOpacity(long windowHandle, byte opacity);

    /// <summary>Whether <see cref="ToggleBorderless"/> works.</summary>
    FeatureSupport BorderlessSupport { get; }

    /// <summary>
    /// Removes the window's frame and fits it to its screen (or the screen's working area), or, called again, restores it.
    /// Returns false when the window could not be changed.
    /// </summary>
    bool ToggleBorderless(long windowHandle, bool useWorkingArea);
}

public sealed class UnsupportedWindowManagementService(string reason) : IWindowManagementService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public long GetWindowAt(PlatformPoint point, bool topLevel) => 0;

    public WindowDetails? GetDetails(long windowHandle) => null;

    public byte[]? GetIcon(long windowHandle) => null;

    public bool SetTopMost(long windowHandle, bool topMost) => false;

    public bool SetOpacity(long windowHandle, byte opacity) => false;

    public FeatureSupport BorderlessSupport => Support;

    public bool ToggleBorderless(long windowHandle, bool useWorkingArea) => false;
}
