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

/// <summary>Hotkey modifiers. Values match the Win32 MOD_* flags used by RegisterHotKey.</summary>
[Flags]
public enum HotkeyModifiers
{
    None = 0,
    Alt = 0x1,
    Control = 0x2,
    Shift = 0x4,
    /// <summary>Windows key, Command on macOS, Super on Linux.</summary>
    Super = 0x8
}

/// <summary>A global hotkey.</summary>
/// <param name="KeyCode">
/// The key as a Windows virtual key code (the same values as System.Windows.Forms.Keys without modifiers).
/// Virtual key codes are used as the portable key identity and each platform maps them to its own key codes.
/// </param>
public readonly record struct PlatformHotkey(int KeyCode, HotkeyModifiers Modifiers)
{
    public bool IsValid => KeyCode > 0;

    public override string ToString()
    {
        List<string> parts = new List<string>(5);
        if (Modifiers.HasFlag(HotkeyModifiers.Control)) parts.Add("Ctrl");
        if (Modifiers.HasFlag(HotkeyModifiers.Alt)) parts.Add("Alt");
        if (Modifiers.HasFlag(HotkeyModifiers.Shift)) parts.Add("Shift");
        if (Modifiers.HasFlag(HotkeyModifiers.Super)) parts.Add("Super");
        parts.Add(VirtualKeys.GetName(KeyCode));
        return string.Join("+", parts);
    }
}

public enum HotkeyRegistrationStatus
{
    Registered,
    /// <summary>Another application already owns the key combination.</summary>
    InUse,
    /// <summary>The key cannot be mapped on this platform.</summary>
    UnsupportedKey,
    /// <summary>Global hotkeys are not available in this session.</summary>
    NotSupported,
    Failed
}

public sealed class HotkeyPressedEventArgs : EventArgs
{
    public HotkeyPressedEventArgs(int id, PlatformHotkey hotkey)
    {
        Id = id;
        Hotkey = hotkey;
    }

    public int Id { get; }

    public PlatformHotkey Hotkey { get; }
}
