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

namespace ShareX.Platform;

/// <summary>Scroll bar commands sent straight to a window, without moving the mouse or the keyboard focus.</summary>
public enum WindowScrollCommand
{
    Top,
    LineDown
}

/// <summary>Synthetic keyboard and mouse input, used by scrolling capture to scroll the window being captured.</summary>
public interface IInputService
{
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
}
