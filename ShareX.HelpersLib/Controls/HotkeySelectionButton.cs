#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class HotkeySelectionButton : Button
    {
        public event EventHandler HotkeyChanged;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HotkeyInfo HotkeyInfo { get; set; } = new HotkeyInfo();

        [Browsable(false)]
        public bool EditingHotkey { get; private set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text { get; set; }

        public HotkeySelectionButton()
        {
            SetDefaultButtonText();
        }

        private void SetDefaultButtonText()
        {
            Text = "Select a hotkey...";
            Invalidate();
        }

        public void Reset()
        {
            EditingHotkey = false;
            HotkeyInfo = new HotkeyInfo();
            SetDefaultButtonText();
        }

        private void StartEditing()
        {
            EditingHotkey = true;

            BackColor = Color.FromArgb(225, 255, 225);
            SetDefaultButtonText();

            HotkeyInfo.Hotkey = Keys.None;
            HotkeyInfo.Win = false;

            OnHotkeyChanged();
        }

        private void StopEditing()
        {
            EditingHotkey = false;

            if (HotkeyInfo.IsOnlyModifiers)
            {
                HotkeyInfo.Hotkey = Keys.None;
            }

            BackColor = SystemColors.Control;
            UseVisualStyleBackColor = true;

            OnHotkeyChanged();
            UpdateHotkeyText();
        }

        public void UpdateHotkey(HotkeyInfo hotkeyInfo)
        {
            HotkeyInfo = hotkeyInfo;
            UpdateHotkeyText();
        }

        private void UpdateHotkeyText()
        {
            Text = HotkeyInfo.ToString();
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (EditingHotkey)
            {
                StopEditing();
            }
            else
            {
                StartEditing();
            }

            base.OnMouseClick(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            if (EditingHotkey)
            {
                StopEditing();
            }

            base.OnLeave(e);
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (EditingHotkey)
            {
                // For handle Tab key etc.
                e.IsInputKey = true;
            }

            base.OnPreviewKeyDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            kevent.SuppressKeyPress = true;

            if (EditingHotkey)
            {
                if (kevent.KeyData == Keys.Escape)
                {
                    HotkeyInfo.Hotkey = Keys.None;
                    StopEditing();
                }
                else if (kevent.KeyCode == Keys.LWin || kevent.KeyCode == Keys.RWin)
                {
                    HotkeyInfo.Win = !HotkeyInfo.Win;
                    UpdateHotkeyText();
                }
                else if (new HotkeyInfo(kevent.KeyData).IsValidHotkey)
                {
                    HotkeyInfo.Hotkey = kevent.KeyData;
                    StopEditing();
                }
                else
                {
                    HotkeyInfo.Hotkey = kevent.KeyData;
                    UpdateHotkeyText();
                }
            }

            base.OnKeyDown(kevent);
        }

        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            kevent.SuppressKeyPress = true;

            if (EditingHotkey)
            {
                // PrintScreen not trigger KeyDown event
                if (kevent.KeyCode == Keys.PrintScreen)
                {
                    HotkeyInfo.Hotkey = kevent.KeyData;
                    StopEditing();
                }
            }

            base.OnKeyUp(kevent);
        }

        protected void OnHotkeyChanged()
        {
            HotkeyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}