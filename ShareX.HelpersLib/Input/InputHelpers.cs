#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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

using ShareX.HelpersLib.Native;

using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib.Input;

public static class InputHelpers
{
    public static bool SendKeyDown(VirtualKeyCode keyCode)
    {
        InputManager inputManager = new();
        inputManager.AddKeyDown(keyCode);
        return inputManager.SendInputs();
    }

    public static bool SendKeyUp(VirtualKeyCode keyCode)
    {
        InputManager inputManager = new();
        inputManager.AddKeyUp(keyCode);
        return inputManager.SendInputs();
    }

    public static bool SendKeyPress(VirtualKeyCode keyCode)
    {
        InputManager inputManager = new();
        inputManager.AddKeyPress(keyCode);
        return inputManager.SendInputs();
    }

    public static bool SendKeyPressModifiers(VirtualKeyCode keyCode, params VirtualKeyCode[] modifiers)
    {
        InputManager inputManager = new();
        inputManager.AddKeyPressModifiers(keyCode, modifiers);
        return inputManager.SendInputs();
    }

    public static bool SendKeyPressText(string text)
    {
        InputManager inputManager = new();
        inputManager.AddKeyPressText(text);
        return inputManager.SendInputs();
    }

    public static bool SendMouseDown(MouseButtons button = MouseButtons.Left)
    {
        InputManager inputManager = new();
        inputManager.AddMouseDown(button);
        return inputManager.SendInputs();
    }

    public static bool SendMouseUp(MouseButtons button = MouseButtons.Left)
    {
        InputManager inputManager = new();
        inputManager.AddMouseUp(button);
        return inputManager.SendInputs();
    }

    public static bool SendMouseClick(MouseButtons button = MouseButtons.Left)
    {
        InputManager inputManager = new();
        inputManager.AddMouseClick(button);
        return inputManager.SendInputs();
    }

    public static bool SendMouseClick(int x, int y, MouseButtons button = MouseButtons.Left)
    {
        InputManager inputManager = new();
        inputManager.AddMouseClick(x, y, button);
        return inputManager.SendInputs();
    }

    public static bool SendMouseClick(Point position, MouseButtons button = MouseButtons.Left)
    {
        return SendMouseClick(position.X, position.Y, button);
    }

    public static bool SendMouseMove(int x, int y)
    {
        InputManager inputManager = new();
        inputManager.AddMouseMove(x, y);
        return inputManager.SendInputs();
    }

    public static bool SendMouseMove(Point position)
    {
        return SendMouseMove(position.X, position.Y);
    }

    public static bool SendMouseWheel(int delta)
    {
        InputManager inputManager = new();
        inputManager.AddMouseWheel(delta);
        return inputManager.SendInputs();
    }
}