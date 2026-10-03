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

/// <summary>A global shortcut registered with the portal: its id, the keys ShareX wants for it and what it does.</summary>
internal readonly record struct GlobalShortcut(string Id, PlatformHotkey Hotkey, string Description);

/// <summary>
/// Assigns keys to the portal's global shortcuts on desktops whose portal registers shortcuts without binding keys (Hyprland).
/// GNOME and KDE ask the user in their own dialog, so they need no binder.
/// </summary>
internal interface IShortcutKeyBinder : System.IDisposable
{
    /// <summary>True when the key combination is already bound by the user's configuration or another application.</summary>
    bool IsInUse(PlatformHotkey hotkey);

    /// <summary>Binds each shortcut's keys to it, replacing the keys ShareX bound before.</summary>
    void Apply(IReadOnlyList<GlobalShortcut> shortcuts);

    /// <summary>Removes the keys ShareX bound.</summary>
    void Clear();
}
