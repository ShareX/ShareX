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

using ShareX.Platform.Windows.Native;
using System;

namespace ShareX.Platform.Windows;

/// <summary>SendInput for keys and the mouse wheel, WM_VSCROLL for scroll bars.</summary>
public sealed unsafe class WindowsInputService : IInputService
{
    public FeatureSupport KeyboardSupport => FeatureSupport.Supported;

    public FeatureSupport MouseWheelSupport => FeatureSupport.Supported;

    public FeatureSupport WindowScrollSupport => FeatureSupport.Supported;

    public bool SendKeyPress(int virtualKey)
    {
        Win32.INPUT* inputs = stackalloc Win32.INPUT[2];
        inputs[0] = new Win32.INPUT { type = Win32.INPUT_KEYBOARD, ki = new Win32.KEYBDINPUT { wVk = (ushort)virtualKey } };
        inputs[1] = new Win32.INPUT { type = Win32.INPUT_KEYBOARD, ki = new Win32.KEYBDINPUT { wVk = (ushort)virtualKey, dwFlags = Win32.KEYEVENTF_KEYUP } };
        return Win32.SendInput(2, inputs, sizeof(Win32.INPUT)) == 2;
    }

    public bool SendMouseWheel(int detents)
    {
        Win32.INPUT input = new Win32.INPUT
        {
            type = Win32.INPUT_MOUSE,
            mi = new Win32.MOUSEINPUT { dwFlags = Win32.MOUSEEVENTF_WHEEL, mouseData = unchecked((uint)(detents * Win32.WHEEL_DELTA)) }
        };

        return Win32.SendInput(1, &input, sizeof(Win32.INPUT)) == 1;
    }

    public FeatureSupport MouseHookSupport => FeatureSupport.Supported;

    public IDisposable HookMouse(IGlobalMouseListener listener) => new WindowsMouseHook(listener);

    public bool ScrollWindow(long windowHandle, WindowScrollCommand command)
    {
        int code = command == WindowScrollCommand.Top ? Win32.SB_TOP : Win32.SB_LINEDOWN;
        Win32.SendMessage((IntPtr)windowHandle, Win32.WM_VSCROLL, code, 0);
        return true;
    }
}
