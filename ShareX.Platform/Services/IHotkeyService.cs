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

namespace ShareX.Platform;

/// <summary>
/// System wide hotkeys: RegisterHotKey on Windows, Carbon hot keys on macOS, XGrabKey on X11 and the
/// GlobalShortcuts portal on Wayland.
/// </summary>
/// <remarks><see cref="HotkeyPressed"/> may be raised on a background thread. Marshal to the UI thread before touching UI.</remarks>
public interface IHotkeyService : IDisposable
{
    FeatureSupport Support { get; }

    event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed;

    /// <param name="id">Caller chosen identifier, unique per registration.</param>
    HotkeyRegistrationStatus Register(int id, PlatformHotkey hotkey);

    bool Unregister(int id);

    void UnregisterAll();
}
