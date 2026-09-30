using System.Collections.Generic;

namespace ShareX.Platform.Linux.Native;

/// <summary>Maps portable virtual key codes to X keysyms and their names, as used by XGrabKey and the XDG shortcuts specification.</summary>
internal static class XKeyMap
{
    private static readonly Dictionary<int, (uint Keysym, string Name)> SpecialKeys = new Dictionary<int, (uint, string)>
    {
        [VirtualKeys.Back] = (0xFF08, "BackSpace"),
        [VirtualKeys.Tab] = (0xFF09, "Tab"),
        [VirtualKeys.Return] = (0xFF0D, "Return"),
        [VirtualKeys.Pause] = (0xFF13, "Pause"),
        [VirtualKeys.CapsLock] = (0xFFE5, "Caps_Lock"),
        [VirtualKeys.Escape] = (0xFF1B, "Escape"),
        [VirtualKeys.Space] = (0x0020, "space"),
        [VirtualKeys.PageUp] = (0xFF55, "Page_Up"),
        [VirtualKeys.PageDown] = (0xFF56, "Page_Down"),
        [VirtualKeys.End] = (0xFF57, "End"),
        [VirtualKeys.Home] = (0xFF50, "Home"),
        [VirtualKeys.Left] = (0xFF51, "Left"),
        [VirtualKeys.Up] = (0xFF52, "Up"),
        [VirtualKeys.Right] = (0xFF53, "Right"),
        [VirtualKeys.Down] = (0xFF54, "Down"),
        [VirtualKeys.PrintScreen] = (0xFF61, "Print"),
        [VirtualKeys.Insert] = (0xFF63, "Insert"),
        [VirtualKeys.Delete] = (0xFFFF, "Delete"),
        [VirtualKeys.Multiply] = (0xFFAA, "KP_Multiply"),
        [VirtualKeys.Add] = (0xFFAB, "KP_Add"),
        [VirtualKeys.Subtract] = (0xFFAD, "KP_Subtract"),
        [VirtualKeys.Decimal] = (0xFFAE, "KP_Decimal"),
        [VirtualKeys.Divide] = (0xFFAF, "KP_Divide"),
        [VirtualKeys.NumLock] = (0xFF7F, "Num_Lock"),
        [VirtualKeys.ScrollLock] = (0xFF14, "Scroll_Lock"),
        [VirtualKeys.OemSemicolon] = (0x003B, "semicolon"),
        [VirtualKeys.OemPlus] = (0x003D, "equal"),
        [VirtualKeys.OemComma] = (0x002C, "comma"),
        [VirtualKeys.OemMinus] = (0x002D, "minus"),
        [VirtualKeys.OemPeriod] = (0x002E, "period"),
        [VirtualKeys.OemQuestion] = (0x002F, "slash"),
        [VirtualKeys.OemTilde] = (0x0060, "grave"),
        [VirtualKeys.OemOpenBrackets] = (0x005B, "bracketleft"),
        [VirtualKeys.OemPipe] = (0x005C, "backslash"),
        [VirtualKeys.OemCloseBrackets] = (0x005D, "bracketright"),
        [VirtualKeys.OemQuotes] = (0x0027, "apostrophe")
    };

    public static bool TryGetKeysym(int keyCode, out uint keysym, out string name)
    {
        if (VirtualKeys.IsLetter(keyCode))
        {
            // Lower case keysyms, Shift is a modifier.
            keysym = (uint)char.ToLowerInvariant((char)keyCode);
            name = ((char)keysym).ToString();
            return true;
        }

        if (VirtualKeys.IsDigit(keyCode))
        {
            keysym = (uint)keyCode;
            name = ((char)keyCode).ToString();
            return true;
        }

        if (VirtualKeys.IsFunctionKey(keyCode))
        {
            int number = keyCode - VirtualKeys.F1 + 1;
            keysym = (uint)(0xFFBE + number - 1);
            name = "F" + number;
            return true;
        }

        if (VirtualKeys.IsNumPadDigit(keyCode))
        {
            int number = keyCode - VirtualKeys.NumPad0;
            keysym = (uint)(0xFFB0 + number);
            name = "KP_" + number;
            return true;
        }

        if (SpecialKeys.TryGetValue(keyCode, out (uint Keysym, string Name) special))
        {
            keysym = special.Keysym;
            name = special.Name;
            return true;
        }

        keysym = 0;
        name = "";
        return false;
    }

    public static uint ToX11Modifiers(HotkeyModifiers modifiers)
    {
        uint mask = 0;
        if (modifiers.HasFlag(HotkeyModifiers.Shift)) mask |= X11.ShiftMask;
        if (modifiers.HasFlag(HotkeyModifiers.Control)) mask |= X11.ControlMask;
        if (modifiers.HasFlag(HotkeyModifiers.Alt)) mask |= X11.Mod1Mask;
        if (modifiers.HasFlag(HotkeyModifiers.Super)) mask |= X11.Mod4Mask;
        return mask;
    }

    /// <summary>Formats a trigger for the XDG shortcuts specification, for example "CTRL+SHIFT+Print".</summary>
    public static string? ToShortcutTrigger(PlatformHotkey hotkey)
    {
        if (!TryGetKeysym(hotkey.KeyCode, out _, out string name))
        {
            return null;
        }

        List<string> parts = new List<string>(5);
        if (hotkey.Modifiers.HasFlag(HotkeyModifiers.Control)) parts.Add("CTRL");
        if (hotkey.Modifiers.HasFlag(HotkeyModifiers.Alt)) parts.Add("ALT");
        if (hotkey.Modifiers.HasFlag(HotkeyModifiers.Shift)) parts.Add("SHIFT");
        if (hotkey.Modifiers.HasFlag(HotkeyModifiers.Super)) parts.Add("LOGO");
        parts.Add(name);
        return string.Join("+", parts);
    }
}
