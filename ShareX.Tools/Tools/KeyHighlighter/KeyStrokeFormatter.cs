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

using System.Text;

namespace ShareX.Tools;

/// <summary>One line shown in a recording, such as "Ctrl + Shift + S" or typed text.</summary>
public sealed record KeyStrokeItem(string Text, int Count, double Opacity);

/// <summary>
/// Turns key presses into the lines shown in a recording. It holds every rule (combinations, key repeat, what counts as
/// a shortcut, typed text, password fields, fading) and has no Windows or drawing code, so it can be tested on its own.
/// </summary>
public sealed class KeyStrokeFormatter
{
    private const int MaxItems = 3;
    private const int MaxTypedLength = 32;
    private const double FadeMilliseconds = 300;

    private sealed class Item
    {
        public required string Text;
        public int Count = 1;
        public bool IsTyping;
        public long Updated;
    }

    private readonly bool showAllKeys;
    private readonly double displayMilliseconds;
    private readonly HashSet<int> down = [];
    private readonly List<Item> items = [];

    public KeyStrokeFormatter(bool showAllKeys, double displaySeconds)
    {
        this.showAllKeys = showAllKeys;
        displayMilliseconds = Math.Clamp(displaySeconds, 0.3, 30) * 1000;
    }

    /// <param name="virtualKey">Windows virtual-key code.</param>
    /// <param name="text">The character the key types with the current layout and Shift state, or null.</param>
    /// <param name="inPasswordField">True when the focused control hides what is typed.</param>
    /// <param name="time">Milliseconds on any steady clock.</param>
    public void KeyDown(int virtualKey, string? text, bool inPasswordField, long time)
    {
        bool repeat = !down.Add(virtualKey);
        if (IsModifier(virtualKey)) return;

        bool altGrTyping = down.Contains(VK_RMENU) && IsDown(VK_CONTROL) && !IsDown(VK_LWIN) && IsTextKey(virtualKey) &&
            !string.IsNullOrEmpty(text);
        bool command = !altGrTyping && (IsDown(VK_CONTROL) || IsDown(VK_MENU) || IsDown(VK_LWIN));
        string? name = GetKeyName(virtualKey, text);
        if (name == null) return;

        if (!command && IsTextKey(virtualKey))
        {
            if (!showAllKeys || inPasswordField || string.IsNullOrEmpty(text)) return;
            Item? last = items.Count > 0 ? items[^1] : null;
            if (last is { IsTyping: true } && time - last.Updated < displayMilliseconds)
            {
                last.Text = Tail(last.Text + text);
                last.Updated = time;
            }
            else
            {
                Add(new Item { Text = Tail(text), IsTyping = true, Updated = time });
            }
            return;
        }

        StringBuilder label = new();
        if (IsDown(VK_LWIN)) label.Append("Win + ");
        if (IsDown(VK_CONTROL)) label.Append("Ctrl + ");
        if (IsDown(VK_MENU)) label.Append("Alt + ");
        if (IsDown(VK_SHIFT)) label.Append("Shift + ");
        label.Append(name);
        string combination = label.ToString();

        Item? previous = items.Count > 0 ? items[^1] : null;
        if (previous is { IsTyping: false } && previous.Text == combination && (repeat || time - previous.Updated < displayMilliseconds))
        {
            previous.Count++;
            previous.Updated = time;
        }
        else
        {
            Add(new Item { Text = combination, Updated = time });
        }
    }

    public void KeyUp(int virtualKey, long time) => down.Remove(virtualKey);

    /// <summary>The lines to show at <paramref name="time"/>, oldest first. Expired lines are removed.</summary>
    public IReadOnlyList<KeyStrokeItem> GetItems(long time)
    {
        items.RemoveAll(item => time - item.Updated >= displayMilliseconds);
        return items.Select(item =>
        {
            double left = displayMilliseconds - (time - item.Updated);
            return new KeyStrokeItem(item.Text, item.Count, Math.Clamp(left / FadeMilliseconds, 0, 1));
        }).ToList();
    }

    private void Add(Item item)
    {
        items.Add(item);
        if (items.Count > MaxItems) items.RemoveAt(0);
    }

    private bool IsDown(int modifier) => modifier switch
    {
        VK_CONTROL => down.Contains(0x11) || down.Contains(0xA2) || down.Contains(0xA3),
        VK_SHIFT => down.Contains(0x10) || down.Contains(0xA0) || down.Contains(0xA1),
        VK_MENU => down.Contains(0x12) || down.Contains(0xA4) || down.Contains(0xA5),
        _ => down.Contains(0x5B) || down.Contains(0x5C)
    };

    private static string Tail(string text) => text.Length <= MaxTypedLength ? text : text[^MaxTypedLength..];

    private const int VK_SHIFT = 0x10, VK_CONTROL = 0x11, VK_MENU = 0x12, VK_LWIN = 0x5B, VK_RMENU = 0xA5;

    private static bool IsModifier(int key) => key is >= 0x10 and <= 0x12 or >= 0xA0 and <= 0xA5 or 0x5B or 0x5C or 0x14 or 0x90 or 0x91;

    /// <summary>Keys that type a character. Alone they are shown only with "show all keys".</summary>
    internal static bool IsTextKey(int key) => key is 0x20 or >= 0x30 and <= 0x39 or >= 0x41 and <= 0x5A or >= 0x60 and <= 0x6F
        or >= 0xBA and <= 0xC0 or >= 0xDB and <= 0xDF or 0xE2;

    internal static string? GetKeyName(int key, string? text) => key switch
    {
        0x08 => "Backspace",
        0x09 => "Tab",
        0x0D => "Enter",
        0x13 => "Pause",
        0x1B => "Esc",
        0x20 => "Space",
        0x21 => "Page Up",
        0x22 => "Page Down",
        0x23 => "End",
        0x24 => "Home",
        0x25 => "←",
        0x26 => "↑",
        0x27 => "→",
        0x28 => "↓",
        0x2C => "Print Screen",
        0x2D => "Insert",
        0x2E => "Delete",
        >= 0x30 and <= 0x39 => ((char)key).ToString(),
        >= 0x41 and <= 0x5A => ((char)key).ToString(),
        >= 0x60 and <= 0x69 => "Num " + (key - 0x60),
        0x6A => "Num *",
        0x6B => "Num +",
        0x6D => "Num -",
        0x6E => "Num .",
        0x6F => "Num /",
        >= 0x70 and <= 0x87 => "F" + (key - 0x6F),
        0x5D => "Menu",
        _ when IsTextKey(key) && !string.IsNullOrWhiteSpace(text) => text.ToUpperInvariant(),
        _ => null
    };
}
