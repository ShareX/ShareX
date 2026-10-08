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

public enum GlobalMouseButton
{
    /// <summary>The button the user clicks with, the left one unless the buttons are swapped.</summary>
    Primary,
    Secondary,
    Middle
}

/// <summary>A mouse button press or release anywhere on the desktop.</summary>
/// <param name="Timestamp">A <see cref="System.Diagnostics.Stopwatch"/> timestamp taken when the event arrived.</param>
public readonly record struct GlobalMouseButtonEvent(GlobalMouseButton Button, bool Pressed, PlatformPoint Position, long Timestamp);

/// <summary>Receives mouse input from every application. Called on a background thread; implementations must return quickly.</summary>
public interface IGlobalMouseListener
{
    void OnMove(PlatformPoint position);

    void OnButton(GlobalMouseButtonEvent buttonEvent);
}

/// <summary>Scroll bar commands sent straight to a window, without moving the mouse or the keyboard focus.</summary>
public enum WindowScrollCommand
{
    Top,
    LineDown
}

/// <summary>Synthetic keyboard and mouse input, used by scrolling capture to scroll the window being captured.</summary>
public interface IInputService
{
    /// <summary>
    /// Asks the system for permission to send input, before a scrolling capture starts, so its prompt is not answered in the middle
    /// of a capture. False when input is not allowed yet; the reason is then in <see cref="KeyboardSupport"/>.
    /// </summary>
    bool RequestPermission() => true;

    /// <summary>Whether <see cref="SendKeyPress"/> works in this session.</summary>
    FeatureSupport KeyboardSupport { get; }

    /// <summary>Whether <see cref="SendMouseWheel"/> works in this session.</summary>
    FeatureSupport MouseWheelSupport { get; }

    /// <summary>Whether <see cref="ScrollWindow"/> works. Only Windows lets one application drive another's scroll bars.</summary>
    FeatureSupport WindowScrollSupport { get; }

    /// <summary>Presses and releases a key (a <see cref="VirtualKeys"/> code) in the focused window.</summary>
    bool SendKeyPress(int virtualKey);

    /// <summary>Turns the mouse wheel under the pointer. Positive detents scroll up, negative scroll down.</summary>
    bool SendMouseWheel(int detents);

    bool ScrollWindow(long windowHandle, WindowScrollCommand command);

    /// <summary>Whether <see cref="HookMouse"/> works. Wayland, by design, does not show applications input meant for others.</summary>
    FeatureSupport MouseHookSupport { get; }

    /// <summary>Reports mouse movement and buttons across the whole desktop until the returned object is disposed.</summary>
    IDisposable HookMouse(IGlobalMouseListener listener);
}
