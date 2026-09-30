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
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class HotkeyInfo
    {
        public Keys Hotkey { get; set; }

        [JsonIgnore]
        public ushort ID { get; set; }

        [JsonIgnore]
        public HotkeyStatus Status { get; set; }

        public Keys KeyCode => Hotkey & Keys.KeyCode;

        public Keys ModifiersKeys => Hotkey & Keys.Modifiers;

        public bool Control => Hotkey.HasFlag(Keys.Control);

        public bool Shift => Hotkey.HasFlag(Keys.Shift);

        public bool Alt => Hotkey.HasFlag(Keys.Alt);

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

        public bool IsOnlyModifiers => KeyCode == Keys.ControlKey || KeyCode == Keys.ShiftKey || KeyCode == Keys.Menu || (KeyCode == Keys.None && Win);

        public bool IsValidHotkey => KeyCode != Keys.None && !IsOnlyModifiers;

        public HotkeyInfo()
        {
            Status = HotkeyStatus.NotConfigured;
        }

        public HotkeyInfo(Keys hotkey) : this()
        {
            Hotkey = hotkey;
        }

        public HotkeyInfo(Keys hotkey, ushort id) : this(hotkey)
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
            else if (KeyCode == Keys.Back)
            {
                text += Localization.Strings.HotkeyInfo_Backspace;
            }
            else if (KeyCode == Keys.Return)
            {
                text += Localization.Strings.HotkeyInfo_Enter;
            }
            else if (KeyCode == Keys.Capital)
            {
                text += Localization.Strings.HotkeyInfo_Caps_lock;
            }
            else if (KeyCode == Keys.Next)
            {
                text += Localization.Strings.HotkeyInfo_Page_down;
            }
            else if (KeyCode == Keys.Scroll)
            {
                text += Localization.Strings.HotkeyInfo_Scroll_lock;
            }
            else if (KeyCode >= Keys.D0 && KeyCode <= Keys.D9)
            {
                text += (KeyCode - Keys.D0).ToString();
            }
            else if (KeyCode >= Keys.NumPad0 && KeyCode <= Keys.NumPad9)
            {
                text += Localization.Strings.HotkeyInfo_Numpad + " " + (KeyCode - Keys.NumPad0).ToString();
            }
            else
            {
                text += ToStringWithSpaces(KeyCode);
            }

            return text;
        }

        private string ToStringWithSpaces(Keys key)
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