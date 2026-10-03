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

namespace ShareX.Platform.Linux.Desktop;

/// <summary>What one kind of Linux desktop can tell and do about application windows and the pointer.</summary>
internal interface IDesktopWindowBackend
{
    LinuxWindowService.Backend Kind { get; }

    FeatureSupport Support { get; }

    IReadOnlyList<PlatformWindow> GetWindows();

    /// <summary>The windows region capture may snap to: on screen, topmost first.</summary>
    IReadOnlyList<PlatformWindow> GetSnapWindows();

    PlatformWindow? GetActiveWindow();

    PlatformPoint? GetCursorPosition();

    bool SetCursorPosition(PlatformPoint position);

    bool ActivateWindow(long windowHandle);

    PlatformRectangle? GetWindowBounds(long windowHandle);

    PlatformRectangle? GetClientBounds(long windowHandle);

    /// <summary>See <see cref="IWindowService.GetOwnWindowPixelScale"/>.</summary>
    double GetOwnWindowPixelScale(PlatformPoint point);

    /// <summary>
    /// True where the backend's window ids differ from the X11 ids of ShareX's own (XWayland) windows, so ShareX's windows are
    /// left out of snapping by process id instead.
    /// </summary>
    bool IdentifiesOwnWindowsByProcess { get; }
}
