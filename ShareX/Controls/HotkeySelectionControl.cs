#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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

using ShareX.HelpersLib;
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

                UpdateTheme();
            }
        }

        public bool EditingHotkey { get; private set; }

        private bool descriptionHover;

        public HotkeySelectionControl(HotkeySettings setting)
        {
            Setting = setting;

            InitializeComponent();
            UpdateDescription();
            UpdateHotkeyText();
            if (ShareXResources.UseCustomTheme)
            {
                ShareXResources.ApplyCustomThemeToControl(this);
            }
            UpdateHotkeyStatus();
            UpdateTheme();
        }

        public void UpdateTheme()
        {
            if (ShareXResources.UseCustomTheme)
            {
                if (Selected)
                {
                    lblHotkeyDescription.ForeColor = SystemColors.ControlText;
                    lblHotkeyDescription.BackColor = Color.FromArgb(200, 255, 200);
                }
                else if (descriptionHover)
                {
                    lblHotkeyDescription.ForeColor = SystemColors.ControlText;
                    lblHotkeyDescription.BackColor = Color.FromArgb(220, 240, 255);
                }
                else
                {
                    lblHotkeyDescription.ForeColor = ShareXResources.Theme.TextColor;
                    lblHotkeyDescription.BackColor = ShareXResources.Theme.LightBackgroundColor;
                }

                btnHotkey.BorderColor = ShareXResources.Theme.BorderColor;

                if (EditingHotkey)
                {
                    btnHotkey.ForeColor = SystemColors.ControlText;
                    btnHotkey.BackColor = Color.FromArgb(225, 255, 225);
                }
                else
                {
                    btnHotkey.ForeColor = ShareXResources.Theme.TextColor;
                    btnHotkey.BackColor = ShareXResources.Theme.LightBackgroundColor;
                }
            }
            else
            {
                lblHotkeyDescription.ForeColor = SystemColors.ControlText;

                if (Selected)
                {
                    lblHotkeyDescription.BackColor = Color.FromArgb(200, 255, 200);
                }
                else if (descriptionHover)
                {
                    lblHotkeyDescription.BackColor = Color.FromArgb(220, 240, 255);
                }
                else
                {
                    lblHotkeyDescription.BackColor = SystemColors.Window;
                }

                btnHotkey.ForeColor = SystemColors.ControlText;

                if (EditingHotkey)
                {
                    btnHotkey.BackColor = Color.FromArgb(225, 255, 225);
                }
                else
                {
                    btnHotkey.BackColor = SystemColors.Control;
                    btnHotkey.UseVisualStyleBackColor = true;
                }
            }
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

        public void UpdateHotkeyStatus()
        {
            switch (Setting.HotkeyInfo.Status)
            {
                default:
                case HotkeyStatus.NotConfigured:
                    btnHotkey.Color = Color.LightGoldenrodYellow;
                    break;
                case HotkeyStatus.Failed:
                    btnHotkey.Color = Color.IndianRed;
                    break;
                case HotkeyStatus.Registered:
                    btnHotkey.Color = Color.PaleGreen;
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

            btnHotkey.Text = Resources.HotkeySelectionControl_StartEditing_Select_a_hotkey___;
            UpdateTheme();

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

            UpdateTheme();
            OnHotkeyChanged();
            UpdateHotkeyStatus();
            UpdateHotkeyText();
        }

        protected void OnHotkeyChanged()
        {
            HotkeyChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void OnSelectedChanged()
        {
            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void OnEditRequested()
        {
            EditRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            OnEditRequested();
        }

        private void lblHotkeyDescription_MouseEnter(object sender, EventArgs e)
        {
            if (!Selected)
            {
                descriptionHover = true;
                UpdateTheme();
            }
        }

        private void lblHotkeyDescription_MouseLeave(object sender, EventArgs e)
        {
            descriptionHover = false;
            UpdateTheme();
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