#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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

using ShareX.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public partial class HotkeySelectionControl : UserControl
    {
        public event EventHandler HotkeyChanged;
        public event EventHandler SelectedChanged;
        public event EventHandler EditRequested;

        public HotkeySettings Setting { get; set; }

        private bool selected;

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;

                if (selected)
                {
                    lblHotkeyDescription.BackColor = Color.FromArgb(200, 255, 200);
                }
                else
                {
                    lblHotkeyDescription.BackColor = Color.White;
                }
            }
        }

        public bool EditingHotkey { get; private set; }

        public HotkeySelectionControl(HotkeySettings setting)
        {
            InitializeComponent();
            Setting = setting;
            UpdateDescription();
            UpdateHotkeyText();
            UpdateHotkeyStatus();
        }

        public void UpdateDescription()
        {
            if (Setting.TaskSettings.IsUsingDefaultSettings)
            {
                lblHotkeyDescription.Image = null;
            }
            else
            {
                lblHotkeyDescription.Image = Resources.pencil;
            }

            lblHotkeyDescription.Text = Setting.TaskSettings.ToString();
        }

        private void UpdateHotkeyText()
        {
            btnHotkey.Text = Setting.HotkeyInfo.ToString();
        }

        private void UpdateHotkeyStatus()
        {
            switch (Setting.HotkeyInfo.Status)
            {
                default:
                case HotkeyStatus.NotConfigured:
                    lblHotkeyStatus.BackColor = Color.LightGoldenrodYellow;
                    break;
                case HotkeyStatus.Failed:
                    lblHotkeyStatus.BackColor = Color.IndianRed;
                    break;
                case HotkeyStatus.Registered:
                    lblHotkeyStatus.BackColor = Color.PaleGreen;
                    break;
            }
        }

        private void btnHotkey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (EditingHotkey)
            {
                // For handle Tab key etc.
                e.IsInputKey = true;
            }
        }

        private void btnHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;

            if (EditingHotkey)
            {
                if (e.KeyData == Keys.Escape)
                {
                    Setting.HotkeyInfo.Hotkey = Keys.None;
                    StopEditing();
                }
                else if (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
                {
                    Setting.HotkeyInfo.Win = !Setting.HotkeyInfo.Win;
                    UpdateHotkeyText();
                }
                else if (new HotkeyInfo(e.KeyData).IsValidHotkey)
                {
                    Setting.HotkeyInfo.Hotkey = e.KeyData;
                    StopEditing();
                }
                else
                {
                    Setting.HotkeyInfo.Hotkey = e.KeyData;
                    UpdateHotkeyText();
                }
            }
        }

        private void btnHotkey_KeyUp(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;

            if (EditingHotkey)
            {
                // PrintScreen not trigger KeyDown event
                if (e.KeyCode == Keys.PrintScreen)
                {
                    Setting.HotkeyInfo.Hotkey = e.KeyData;
                    StopEditing();
                }
            }
        }

        private void btnHotkey_MouseClick(object sender, MouseEventArgs e)
        {
            if (EditingHotkey)
            {
                StopEditing();
            }
            else
            {
                StartEditing();
            }
        }

        private void btnHotkey_Leave(object sender, EventArgs e)
        {
            if (EditingHotkey)
            {
                StopEditing();
            }
        }

        private void StartEditing()
        {
            EditingHotkey = true;

            Program.HotkeyManager.IgnoreHotkeys = true;

            btnHotkey.BackColor = Color.FromArgb(225, 255, 225);
            btnHotkey.Text = Resources.HotkeySelectionControl_StartEditing_Select_a_hotkey___;

            Setting.HotkeyInfo.Hotkey = Keys.None;
            Setting.HotkeyInfo.Win = false;
            OnHotkeyChanged();
            UpdateHotkeyStatus();
        }

        private void StopEditing()
        {
            EditingHotkey = false;

            Program.HotkeyManager.IgnoreHotkeys = false;

            if (Setting.HotkeyInfo.IsOnlyModifiers)
            {
                Setting.HotkeyInfo.Hotkey = Keys.None;
            }

            btnHotkey.BackColor = SystemColors.Control;
            btnHotkey.UseVisualStyleBackColor = true;

            OnHotkeyChanged();
            UpdateHotkeyStatus();
            UpdateHotkeyText();
        }

        protected void OnHotkeyChanged()
        {
            if (HotkeyChanged != null)
            {
                HotkeyChanged(this, EventArgs.Empty);
            }
        }

        protected void OnSelectedChanged()
        {
            if (SelectedChanged != null)
            {
                SelectedChanged(this, EventArgs.Empty);
            }
        }

        protected void OnEditRequested()
        {
            if (EditRequested != null)
            {
                EditRequested(this, EventArgs.Empty);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            OnEditRequested();
        }

        private void lblHotkeyDescription_MouseEnter(object sender, EventArgs e)
        {
            if (!Selected)
            {
                lblHotkeyDescription.BackColor = Color.FromArgb(220, 240, 255);
            }
        }

        private void lblHotkeyDescription_MouseLeave(object sender, EventArgs e)
        {
            if (!Selected)
            {
                lblHotkeyDescription.BackColor = Color.White;
            }
        }

        private void lblHotkeyDescription_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Selected = true;
                OnSelectedChanged();
                Focus();
            }
        }

        private void lblHotkeyDescription_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OnEditRequested();
            }
        }
    }
}