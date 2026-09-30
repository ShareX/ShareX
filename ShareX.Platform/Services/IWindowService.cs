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

using System.Collections.Generic;

namespace ShareX.Platform;

/// <summary>A top level window as seen by the window manager.</summary>
/// <param name="Handle">Native handle: HWND on Windows, CGWindowID on macOS, X11 window id, or compositor address on Wayland.</param>
/// <param name="Bounds">Visible bounds on the virtual desktop in physical pixels, excluding invisible resize borders where the OS reports them.</param>
public sealed record PlatformWindow(long Handle, string Title, string? ProcessName, int? ProcessId, PlatformRectangle Bounds, bool IsMinimized);

/// <summary>Window enumeration for window and region capture.</summary>
public interface IWindowService
{
    FeatureSupport Support { get; }

    /// <summary>Visible top level windows in Z order, topmost first where the platform reports it.</summary>
    IReadOnlyList<PlatformWindow> GetWindows();

    PlatformWindow? GetActiveWindow();

    /// <summary>
    /// The mouse pointer on the virtual desktop, in the same coordinates as IScreenCaptureService.GetScreens, or null where the session does not reveal it
    /// (for example Wayland compositors other than Hyprland).
    /// </summary>
    PlatformPoint? GetCursorPosition();
}
