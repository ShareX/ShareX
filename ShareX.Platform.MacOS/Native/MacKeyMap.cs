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

namespace ShareX.Platform.MacOS.Native;

/// <summary>Maps portable virtual key codes to macOS virtual key codes (kVK_* from HIToolbox/Events.h, ANSI layout positions).</summary>
internal static class MacKeyMap
{
    private static readonly Dictionary<int, uint> Keys = new Dictionary<int, uint>
    {
        ['A'] = 0x00, ['S'] = 0x01, ['D'] = 0x02, ['F'] = 0x03, ['H'] = 0x04, ['G'] = 0x05, ['Z'] = 0x06, ['X'] = 0x07,
        ['C'] = 0x08, ['V'] = 0x09, ['B'] = 0x0B, ['Q'] = 0x0C, ['W'] = 0x0D, ['E'] = 0x0E, ['R'] = 0x0F, ['Y'] = 0x10,
        ['T'] = 0x11, ['1'] = 0x12, ['2'] = 0x13, ['3'] = 0x14, ['4'] = 0x15, ['6'] = 0x16, ['5'] = 0x17, ['9'] = 0x19,
        ['7'] = 0x1A, ['8'] = 0x1C, ['0'] = 0x1D, ['O'] = 0x1F, ['U'] = 0x20, ['I'] = 0x22, ['P'] = 0x23, ['L'] = 0x25,
        ['J'] = 0x26, ['K'] = 0x28, ['N'] = 0x2D, ['M'] = 0x2E,
        [VirtualKeys.OemPlus] = 0x18,
        [VirtualKeys.OemMinus] = 0x1B,
        [VirtualKeys.OemCloseBrackets] = 0x1E,
        [VirtualKeys.OemOpenBrackets] = 0x21,
        [VirtualKeys.OemQuotes] = 0x27,
        [VirtualKeys.OemSemicolon] = 0x29,
        [VirtualKeys.OemPipe] = 0x2A,
        [VirtualKeys.OemComma] = 0x2B,
        [VirtualKeys.OemQuestion] = 0x2C,
        [VirtualKeys.OemPeriod] = 0x2F,
        [VirtualKeys.OemTilde] = 0x32,
        [VirtualKeys.Return] = 0x24,
        [VirtualKeys.Tab] = 0x30,
        [VirtualKeys.Space] = 0x31,
        [VirtualKeys.Back] = 0x33,
        [VirtualKeys.Escape] = 0x35,
        [VirtualKeys.CapsLock] = 0x39,
        [VirtualKeys.Decimal] = 0x41,
        [VirtualKeys.Multiply] = 0x43,
        [VirtualKeys.Add] = 0x45,
        [VirtualKeys.Divide] = 0x4B,
        [VirtualKeys.Subtract] = 0x4E,
        [VirtualKeys.NumPad0] = 0x52, [VirtualKeys.NumPad0 + 1] = 0x53, [VirtualKeys.NumPad0 + 2] = 0x54, [VirtualKeys.NumPad0 + 3] = 0x55,
        [VirtualKeys.NumPad0 + 4] = 0x56, [VirtualKeys.NumPad0 + 5] = 0x57, [VirtualKeys.NumPad0 + 6] = 0x58, [VirtualKeys.NumPad0 + 7] = 0x59,
        [VirtualKeys.NumPad0 + 8] = 0x5B, [VirtualKeys.NumPad0 + 9] = 0x5C,
        [VirtualKeys.F1] = 0x7A, [VirtualKeys.F1 + 1] = 0x78, [VirtualKeys.F1 + 2] = 0x63, [VirtualKeys.F1 + 3] = 0x76,
        [VirtualKeys.F1 + 4] = 0x60, [VirtualKeys.F1 + 5] = 0x61, [VirtualKeys.F1 + 6] = 0x62, [VirtualKeys.F1 + 7] = 0x64,
        [VirtualKeys.F1 + 8] = 0x65, [VirtualKeys.F1 + 9] = 0x6D, [VirtualKeys.F1 + 10] = 0x67, [VirtualKeys.F1 + 11] = 0x6F,
        [VirtualKeys.F1 + 12] = 0x69, [VirtualKeys.F1 + 13] = 0x6B, [VirtualKeys.F1 + 14] = 0x71, [VirtualKeys.F1 + 15] = 0x6A,
        [VirtualKeys.F1 + 16] = 0x40, [VirtualKeys.F1 + 17] = 0x4F, [VirtualKeys.F1 + 18] = 0x50, [VirtualKeys.F1 + 19] = 0x5A,
        // Mac keyboards have no Print Screen key. F13 sits in its place on extended keyboards.
        [VirtualKeys.PrintScreen] = 0x69,
        [VirtualKeys.Insert] = 0x72, // Help
        [VirtualKeys.Home] = 0x73,
        [VirtualKeys.PageUp] = 0x74,
        [VirtualKeys.Delete] = 0x75, // Forward delete
        [VirtualKeys.End] = 0x77,
        [VirtualKeys.PageDown] = 0x79,
        [VirtualKeys.Left] = 0x7B,
        [VirtualKeys.Right] = 0x7C,
        [VirtualKeys.Down] = 0x7D,
        [VirtualKeys.Up] = 0x7E
    };

    public static bool TryGetKeyCode(int virtualKey, out uint keyCode) => Keys.TryGetValue(virtualKey, out keyCode);

    public static uint ToCarbonModifiers(HotkeyModifiers modifiers)
    {
        uint result = 0;
        if (modifiers.HasFlag(HotkeyModifiers.Control)) result |= Carbon.controlKey;
        if (modifiers.HasFlag(HotkeyModifiers.Alt)) result |= Carbon.optionKey;
        if (modifiers.HasFlag(HotkeyModifiers.Shift)) result |= Carbon.shiftKey;
        if (modifiers.HasFlag(HotkeyModifiers.Super)) result |= Carbon.cmdKey;
        return result;
    }
}
