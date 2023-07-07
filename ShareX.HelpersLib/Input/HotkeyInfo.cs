#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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
                text += Resources.HotkeyInfo_Key_Combo_Name_Control_Plus_X;
            }

            if (Shift)
            {
                text += Resources.HotkeyInfo_Key_Combo_Name_Shift_Plus_X;
            }

            if (Alt)
            {
                text += Resources.HotkeyInfo_Key_Combo_Name_Alternate_Plus_X;
            }

            if (Win)
            {
                text += Resources.HotkeyInfo_Key_Combo_Name_Windows_Plus_X;
            }

            if (IsOnlyModifiers)
            {
                text += Resources.HotkeyInfo_Key_Combo_Name_Plus_Empty;
            }
            else if (KeyCode == Keys.Back)
            {
                text += Resources.HotkeyInfo_Key_Name_Backspace;
            }
            else if (KeyCode == Keys.Return)
            {
                text += Resources.HotkeyInfo_Key_Name_Enter;
            }
            else if (KeyCode == Keys.Capital)
            {
                text += Resources.HotkeyInfo_Key_Name_Caps_Lock;
            }
            else if (KeyCode == Keys.Next)
            {
                text += Resources.HotkeyInfo_Key_Name_Page_Down;
            }
            else if (KeyCode == Keys.Prior)
            {
                text += Resources.HotkeyInfo_Key_Name_Page_Up;
            }
            else if (KeyCode == Keys.Scroll)
            {
                text += Resources.HotkeyInfo_Key_Name_Scroll_Lock;
            }
            else if (KeyCode == Keys.Space)
            {
                text += Resources.HotkeyInfo_Key_Name_Spacebar;
            }
            else if (KeyCode == Keys.Left)
            {
                text += (Resources.HotkeyInfo_Key_Name_Left_Arrow;
            }
            else if (KeyCode == Keys.Up)
            {
                text += Resources.HotkeyInfo_Key_Name_Up_Arrow;
            }
            else if (KeyCode == Keys.Right)
            {
                text += Resources.HotkeyInfo_Key_Name_Right_Arrow;
            }
            else if (KeyCode == Keys.Down)
            {
                text += Resources.HotkeyInfo_Key_Name_Down_Arrow;
            }
            else if (KeyCode == Keys.Insert)
            {
                text += Resources.HotkeyInfo_Key_Name_Insert;
            }
            else if (KeyCode == Keys.Delete)
            {
                text += Resources.HotkeyInfo_Key_Name_Delete;
            }
            else if (KeyCode == Keys.None)
            {
                text += Resources.HotkeyInfo_Key_Name_Nothing_Bound;
            }
            else if (KeyCode >= Keys.D0 && KeyCode <= Keys.D9)
            {
                text += (KeyCode - Keys.D0).ToString();
            }
            else if (KeyCode >= Keys.NumPad0 && KeyCode <= Keys.NumPad9)
            {
                text += Resources.HotkeyInfo_Key_Name_Numpad_ + (KeyCode - Keys.NumPad0).ToString();
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