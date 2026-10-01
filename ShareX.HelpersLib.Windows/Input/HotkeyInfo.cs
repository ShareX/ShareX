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

using Newtonsoft.Json;
using System.Text;

namespace ShareX.HelpersLib
{
    public class HotkeyInfo
    {
        public InputKey Hotkey { get; set; }

        [JsonIgnore]
        public ushort ID { get; set; }

        [JsonIgnore]
        public HotkeyStatus Status { get; set; }

        public InputKey KeyCode => Hotkey & InputKey.KeyCode;

        public InputKey ModifiersKeys => Hotkey & InputKey.Modifiers;

        public bool Control => Hotkey.HasFlag(InputKey.Control);

        public bool Shift => Hotkey.HasFlag(InputKey.Shift);

        public bool Alt => Hotkey.HasFlag(InputKey.Alt);

        public bool Win { get; set; }

        public Modifiers ModifiersEnum
        {
            get
            {
                Modifiers modifiers = Modifiers.None;

                if (Alt) modifiers |= Modifiers.Alt;
                if (Control) modifiers |= Modifiers.Control;
                if (Shift) modifiers |= Modifiers.Shift;
                if (Win) modifiers |= Modifiers.Win;

                return modifiers;
            }
        }

        public bool IsOnlyModifiers => KeyCode == InputKey.ControlKey || KeyCode == InputKey.ShiftKey || KeyCode == InputKey.Menu || (KeyCode == InputKey.None && Win);

        public bool IsValidHotkey => KeyCode != InputKey.None && !IsOnlyModifiers;

        public HotkeyInfo()
        {
            Status = HotkeyStatus.NotConfigured;
        }

        public HotkeyInfo(InputKey hotkey) : this()
        {
            Hotkey = hotkey;
        }

        public HotkeyInfo(InputKey hotkey, ushort id) : this(hotkey)
        {
            ID = id;
        }

        public override string ToString()
        {
            string text = "";

            if (Control)
            {
                text += Localization.Strings.HotkeyInfo_Ctrl + " + ";
            }

            if (Shift)
            {
                text += Localization.Strings.HotkeyInfo_Shift + " + ";
            }

            if (Alt)
            {
                text += Localization.Strings.HotkeyInfo_Alt + " + ";
            }

            if (Win)
            {
                text += Localization.Strings.HotkeyInfo_Win + " + ";
            }

            if (IsOnlyModifiers)
            {
                text += "...";
            }
            else if (KeyCode == InputKey.Back)
            {
                text += Localization.Strings.HotkeyInfo_Backspace;
            }
            else if (KeyCode == InputKey.Return)
            {
                text += Localization.Strings.HotkeyInfo_Enter;
            }
            else if (KeyCode == InputKey.Capital)
            {
                text += Localization.Strings.HotkeyInfo_Caps_lock;
            }
            else if (KeyCode == InputKey.Next)
            {
                text += Localization.Strings.HotkeyInfo_Page_down;
            }
            else if (KeyCode == InputKey.Scroll)
            {
                text += Localization.Strings.HotkeyInfo_Scroll_lock;
            }
            else if (KeyCode >= InputKey.D0 && KeyCode <= InputKey.D9)
            {
                text += (KeyCode - InputKey.D0).ToString();
            }
            else if (KeyCode >= InputKey.NumPad0 && KeyCode <= InputKey.NumPad9)
            {
                text += Localization.Strings.HotkeyInfo_Numpad + " " + (KeyCode - InputKey.NumPad0).ToString();
            }
            else
            {
                text += ToStringWithSpaces(KeyCode);
            }

            return text;
        }

        private string ToStringWithSpaces(InputKey key)
        {
            string name = key.ToString();

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                if (i > 0 && char.IsUpper(name[i]))
                {
                    result.Append(" " + name[i]);
                }
                else
                {
                    result.Append(name[i]);
                }
            }

            return result.ToString();
        }
    }
}