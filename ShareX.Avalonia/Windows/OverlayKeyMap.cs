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

#nullable enable

using Avalonia.Input;
using System.Collections.Generic;

namespace ShareX.AvaloniaUI.Windows;

/// <summary>
/// Translates Linux evdev keycodes and XKB keysyms from <see cref="ShareX.Platform.OverlayKeyEvent"/> into Avalonia
/// <see cref="Key"/> and <see cref="PhysicalKey"/> values.
/// </summary>
internal static class OverlayKeyMap
{
    private static readonly Dictionary<uint, Key> KeyFromEvdev = new(128)
    {
        { 1, Key.Escape },
        { 2, Key.D1 },
        { 3, Key.D2 },
        { 4, Key.D3 },
        { 5, Key.D4 },
        { 6, Key.D5 },
        { 7, Key.D6 },
        { 8, Key.D7 },
        { 9, Key.D8 },
        { 10, Key.D9 },
        { 11, Key.D0 },
        { 12, Key.OemMinus },
        { 13, Key.OemPlus },
        { 14, Key.Back },
        { 15, Key.Tab },
        { 16, Key.Q },
        { 17, Key.W },
        { 18, Key.E },
        { 19, Key.R },
        { 20, Key.T },
        { 21, Key.Y },
        { 22, Key.U },
        { 23, Key.I },
        { 24, Key.O },
        { 25, Key.P },
        { 26, Key.OemOpenBrackets },
        { 27, Key.OemCloseBrackets },
        { 28, Key.Return },
        { 29, Key.LeftCtrl },
        { 30, Key.A },
        { 31, Key.S },
        { 32, Key.D },
        { 33, Key.F },
        { 34, Key.G },
        { 35, Key.H },
        { 36, Key.J },
        { 37, Key.K },
        { 38, Key.L },
        { 39, Key.OemSemicolon },
        { 40, Key.OemQuotes },
        { 41, Key.OemTilde },
        { 42, Key.LeftShift },
        { 43, Key.OemPipe },
        { 44, Key.Z },
        { 45, Key.X },
        { 46, Key.C },
        { 47, Key.V },
        { 48, Key.B },
        { 49, Key.N },
        { 50, Key.M },
        { 51, Key.OemComma },
        { 52, Key.OemPeriod },
        { 53, Key.Oem2 },
        { 54, Key.RightShift },
        { 55, Key.Multiply },
        { 56, Key.LeftAlt },
        { 57, Key.Space },
        { 58, Key.CapsLock },
        { 59, Key.F1 },
        { 60, Key.F2 },
        { 61, Key.F3 },
        { 62, Key.F4 },
        { 63, Key.F5 },
        { 64, Key.F6 },
        { 65, Key.F7 },
        { 66, Key.F8 },
        { 67, Key.F9 },
        { 68, Key.F10 },
        { 69, Key.NumLock },
        { 70, Key.Scroll },
        { 71, Key.NumPad7 },
        { 72, Key.NumPad8 },
        { 73, Key.NumPad9 },
        { 74, Key.Subtract },
        { 75, Key.NumPad4 },
        { 76, Key.NumPad5 },
        { 77, Key.NumPad6 },
        { 78, Key.Add },
        { 79, Key.NumPad1 },
        { 80, Key.NumPad2 },
        { 81, Key.NumPad3 },
        { 82, Key.NumPad0 },
        { 83, Key.Decimal },
        { 87, Key.F11 },
        { 88, Key.F12 },
        { 96, Key.Return },
        { 97, Key.RightCtrl },
        { 98, Key.Divide },
        { 99, Key.Print },
        { 100, Key.RightAlt },
        { 102, Key.Home },
        { 103, Key.Up },
        { 104, Key.Prior },
        { 105, Key.Left },
        { 106, Key.Right },
        { 107, Key.End },
        { 108, Key.Down },
        { 109, Key.PageDown },
        { 110, Key.Insert },
        { 111, Key.Delete },
        { 119, Key.Pause },
        { 125, Key.LWin },
        { 126, Key.RWin },
        { 127, Key.Apps },
        { 183, Key.F13 },
        { 184, Key.F14 },
        { 185, Key.F15 },
        { 186, Key.F16 },
        { 187, Key.F17 },
        { 188, Key.F18 },
        { 189, Key.F19 },
        { 190, Key.F20 },
        { 191, Key.F21 },
        { 192, Key.F22 },
        { 193, Key.F23 },
        { 194, Key.F24 }
    };

    private static readonly Dictionary<uint, PhysicalKey> PhysicalKeyFromEvdev = new(128)
    {
        { 1, PhysicalKey.Escape },
        { 2, PhysicalKey.Digit1 },
        { 3, PhysicalKey.Digit2 },
        { 4, PhysicalKey.Digit3 },
        { 5, PhysicalKey.Digit4 },
        { 6, PhysicalKey.Digit5 },
        { 7, PhysicalKey.Digit6 },
        { 8, PhysicalKey.Digit7 },
        { 9, PhysicalKey.Digit8 },
        { 10, PhysicalKey.Digit9 },
        { 11, PhysicalKey.Digit0 },
        { 12, PhysicalKey.Minus },
        { 13, PhysicalKey.Equal },
        { 14, PhysicalKey.Backspace },
        { 15, PhysicalKey.Tab },
        { 16, PhysicalKey.Q },
        { 17, PhysicalKey.W },
        { 18, PhysicalKey.E },
        { 19, PhysicalKey.R },
        { 20, PhysicalKey.T },
        { 21, PhysicalKey.Y },
        { 22, PhysicalKey.U },
        { 23, PhysicalKey.I },
        { 24, PhysicalKey.O },
        { 25, PhysicalKey.P },
        { 26, PhysicalKey.BracketLeft },
        { 27, PhysicalKey.BracketRight },
        { 28, PhysicalKey.Enter },
        { 29, PhysicalKey.ControlLeft },
        { 30, PhysicalKey.A },
        { 31, PhysicalKey.S },
        { 32, PhysicalKey.D },
        { 33, PhysicalKey.F },
        { 34, PhysicalKey.G },
        { 35, PhysicalKey.H },
        { 36, PhysicalKey.J },
        { 37, PhysicalKey.K },
        { 38, PhysicalKey.L },
        { 39, PhysicalKey.Semicolon },
        { 40, PhysicalKey.Quote },
        { 41, PhysicalKey.Backquote },
        { 42, PhysicalKey.ShiftLeft },
        { 43, PhysicalKey.Backslash },
        { 44, PhysicalKey.Z },
        { 45, PhysicalKey.X },
        { 46, PhysicalKey.C },
        { 47, PhysicalKey.V },
        { 48, PhysicalKey.B },
        { 49, PhysicalKey.N },
        { 50, PhysicalKey.M },
        { 51, PhysicalKey.Comma },
        { 52, PhysicalKey.Period },
        { 53, PhysicalKey.Slash },
        { 54, PhysicalKey.ShiftRight },
        { 55, PhysicalKey.NumPadMultiply },
        { 56, PhysicalKey.AltLeft },
        { 57, PhysicalKey.Space },
        { 58, PhysicalKey.CapsLock },
        { 59, PhysicalKey.F1 },
        { 60, PhysicalKey.F2 },
        { 61, PhysicalKey.F3 },
        { 62, PhysicalKey.F4 },
        { 63, PhysicalKey.F5 },
        { 64, PhysicalKey.F6 },
        { 65, PhysicalKey.F7 },
        { 66, PhysicalKey.F8 },
        { 67, PhysicalKey.F9 },
        { 68, PhysicalKey.F10 },
        { 69, PhysicalKey.NumLock },
        { 70, PhysicalKey.ScrollLock },
        { 71, PhysicalKey.NumPad7 },
        { 72, PhysicalKey.NumPad8 },
        { 73, PhysicalKey.NumPad9 },
        { 74, PhysicalKey.NumPadSubtract },
        { 75, PhysicalKey.NumPad4 },
        { 76, PhysicalKey.NumPad5 },
        { 77, PhysicalKey.NumPad6 },
        { 78, PhysicalKey.NumPadAdd },
        { 79, PhysicalKey.NumPad1 },
        { 80, PhysicalKey.NumPad2 },
        { 81, PhysicalKey.NumPad3 },
        { 82, PhysicalKey.NumPad0 },
        { 83, PhysicalKey.NumPadDecimal },
        { 86, PhysicalKey.IntlBackslash },
        { 87, PhysicalKey.F11 },
        { 88, PhysicalKey.F12 },
        { 96, PhysicalKey.NumPadEnter },
        { 97, PhysicalKey.ControlRight },
        { 98, PhysicalKey.NumPadDivide },
        { 99, PhysicalKey.PrintScreen },
        { 100, PhysicalKey.AltRight },
        { 102, PhysicalKey.Home },
        { 103, PhysicalKey.ArrowUp },
        { 104, PhysicalKey.PageUp },
        { 105, PhysicalKey.ArrowLeft },
        { 106, PhysicalKey.ArrowRight },
        { 107, PhysicalKey.End },
        { 108, PhysicalKey.ArrowDown },
        { 109, PhysicalKey.PageDown },
        { 110, PhysicalKey.Insert },
        { 111, PhysicalKey.Delete },
        { 119, PhysicalKey.Pause },
        { 125, PhysicalKey.MetaLeft },
        { 126, PhysicalKey.MetaRight },
        { 127, PhysicalKey.ContextMenu },
        { 183, PhysicalKey.F13 },
        { 184, PhysicalKey.F14 },
        { 185, PhysicalKey.F15 },
        { 186, PhysicalKey.F16 },
        { 187, PhysicalKey.F17 },
        { 188, PhysicalKey.F18 },
        { 189, PhysicalKey.F19 },
        { 190, PhysicalKey.F20 },
        { 191, PhysicalKey.F21 },
        { 192, PhysicalKey.F22 },
        { 193, PhysicalKey.F23 },
        { 194, PhysicalKey.F24 }
    };

    private static readonly Dictionary<uint, Key> KeyFromKeysym = new(160)
    {
        { 0xff08, Key.Back },
        { 0xff09, Key.Tab },
        { 0xff0a, Key.LineFeed },
        { 0xff0b, Key.Clear },
        { 0xff0d, Key.Return },
        { 0xff8d, Key.Return },
        { 0xff13, Key.Pause },
        { 0xffe5, Key.CapsLock },
        { 0xff1b, Key.Escape },
        { 0x0020, Key.Space },
        { 0xff55, Key.Prior },
        { 0xff9a, Key.Prior },
        { 0xff56, Key.PageDown },
        { 0xff9b, Key.PageDown },
        { 0xff57, Key.End },
        { 0xff9c, Key.End },
        { 0xff50, Key.Home },
        { 0xff95, Key.Home },
        { 0xff51, Key.Left },
        { 0xff96, Key.Left },
        { 0xff52, Key.Up },
        { 0xff97, Key.Up },
        { 0xff53, Key.Right },
        { 0xff98, Key.Right },
        { 0xff54, Key.Down },
        { 0xff99, Key.Down },
        { 0xff60, Key.Select },
        { 0xff61, Key.Print },
        { 0xff62, Key.Execute },
        { 0xff63, Key.Insert },
        { 0xff9e, Key.Insert },
        { 0xffff, Key.Delete },
        { 0xff9f, Key.Delete },
        { 0xff6a, Key.Help },
        { 0xffeb, Key.LWin },
        { 0xffec, Key.RWin },
        { 0xff67, Key.Apps },
        { 0xff7f, Key.NumLock },
        { 0xff14, Key.Scroll },
        { 0xffe1, Key.LeftShift },
        { 0xffe2, Key.RightShift },
        { 0xffe3, Key.LeftCtrl },
        { 0xffe4, Key.RightCtrl },
        { 0xffe9, Key.LeftAlt },
        { 0xffea, Key.RightAlt },
        { 0xffb0, Key.NumPad0 },
        { 0xffb1, Key.NumPad1 },
        { 0xffb2, Key.NumPad2 },
        { 0xffb3, Key.NumPad3 },
        { 0xffb4, Key.NumPad4 },
        { 0xffb5, Key.NumPad5 },
        { 0xffb6, Key.NumPad6 },
        { 0xffb7, Key.NumPad7 },
        { 0xffb8, Key.NumPad8 },
        { 0xffb9, Key.NumPad9 },
        { 0x002a, Key.Multiply },
        { 0xffaa, Key.Multiply },
        { 0xffab, Key.Add },
        { 0xffad, Key.Subtract },
        { 0xffae, Key.Decimal },
        { 0xffaf, Key.Divide },
        { 0x002d, Key.OemMinus },
        { 0x005f, Key.OemMinus },
        { 0x002b, Key.OemPlus },
        { 0x003d, Key.OemPlus },
        { 0x005b, Key.OemOpenBrackets },
        { 0x007b, Key.OemOpenBrackets },
        { 0x005d, Key.OemCloseBrackets },
        { 0x007d, Key.OemCloseBrackets },
        { 0x005c, Key.OemPipe },
        { 0x007c, Key.OemPipe },
        { 0x003b, Key.OemSemicolon },
        { 0x003a, Key.OemSemicolon },
        { 0x0027, Key.OemQuotes },
        { 0x0022, Key.OemQuotes },
        { 0x002c, Key.OemComma },
        { 0x003c, Key.OemComma },
        { 0x002e, Key.OemPeriod },
        { 0x003e, Key.OemPeriod },
        { 0x002f, Key.Oem2 },
        { 0x003f, Key.Oem2 },
        { 0x0060, Key.OemTilde },
        { 0x007e, Key.OemTilde },
    };

    public static (Key Key, PhysicalKey PhysicalKey) Resolve(uint evdevKeycode, uint keysym)
    {
        PhysicalKey physicalKey = PhysicalKeyFromEvdev.TryGetValue(evdevKeycode, out PhysicalKey pk)
            ? pk
            : PhysicalKey.None;

        if (physicalKey is >= PhysicalKey.Digit0 and <= PhysicalKey.Digit9)
        {
            return (physicalKey.ToQwertyKey(), physicalKey);
        }

        if (keysym != 0)
        {
            if (keysym is >= 0x0041 and <= 0x005a)
            {
                return ((Key)((int)Key.A + (int)(keysym - 0x0041)), physicalKey);
            }

            if (keysym is >= 0x0061 and <= 0x007a)
            {
                return ((Key)((int)Key.A + (int)(keysym - 0x0061)), physicalKey);
            }

            if (keysym is >= 0x0030 and <= 0x0039)
            {
                return ((Key)((int)Key.D0 + (int)(keysym - 0x0030)), physicalKey);
            }

            if (keysym is >= 0xffbe and <= 0xffd5)
            {
                return ((Key)((int)Key.F1 + (int)(keysym - 0xffbe)), physicalKey);
            }

            if (KeyFromKeysym.TryGetValue(keysym, out Key mapped))
            {
                return (mapped, physicalKey);
            }
        }

        if (KeyFromEvdev.TryGetValue(evdevKeycode, out Key evdevMapped))
        {
            return (evdevMapped, physicalKey);
        }

        return (physicalKey.ToQwertyKey(), physicalKey);
    }
}
