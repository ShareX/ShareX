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

using ShareX.HelpersLib;
using ShareX.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX
{
    public partial class HotkeySettingsForm : Form
    {
        public HotkeySelectionControl Selected { get; private set; }

        private HotkeyManager manager;

        public HotkeySettingsForm(HotkeyManager hotkeyManager)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            btnHotkeysDisabled.Visible = Program.Settings.DisableHotkeys;

            PrepareHotkeys(hotkeyManager);
        }

        private void HotkeySettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (manager != null)
            {
                manager.IgnoreHotkeys = false;
                manager.HotkeysToggledTrigger -= HandleHotkeysToggle;
            }
        }

        public void PrepareHotkeys(HotkeyManager hotkeyManager)
        {
            if (manager == null)
            {
                manager = hotkeyManager;
                manager.HotkeysToggledTrigger += HandleHotkeysToggle;
                manager.RegisterFailedHotkeys();

                AddControls();
            }
        }

        private void AddControls()
        {
            flpHotkeys.Controls.Clear();

            foreach (HotkeySettings hotkeySettings in manager.Hotkeys)
            {
                AddHotkeySelectionControl(hotkeySettings);
            }
        }

        private void UpdateButtons()
        {
            btnEdit.Enabled = btnRemove.Enabled = btnDuplicate.Enabled = Selected != null;
            btnMoveUp.Enabled = btnMoveDown.Enabled = Selected != null && flpHotkeys.Controls.Count > 1;
        }

        private HotkeySelectionControl FindSelectionControl(HotkeySettings hotkeySetting)
        {
            return flpHotkeys.Controls.Cast<HotkeySelectionControl>().FirstOrDefault(hsc => hsc.HotkeySettings == hotkeySetting);
        }

        private void control_SelectedChanged(object sender, EventArgs e)
        {
            Selected = (HotkeySelectionControl)sender;
            UpdateButtons();
            UpdateCheckStates();
        }

        private void UpdateCheckStates()
        {
            foreach (Control control in flpHotkeys.Controls)
            {
                ((HotkeySelectionControl)control).Selected = Selected == control;
            }
        }

        private void UpdateHotkeyStatus()
        {
            foreach (Control control in flpHotkeys.Controls)
            {
                ((HotkeySelectionControl)control).UpdateHotkeyStatus();
            }
        }

        private void control_HotkeyChanged(object sender, EventArgs e)
        {
            HotkeySelectionControl control = (HotkeySelectionControl)sender;
            manager.RegisterHotkey(control.HotkeySettings);
            manager.RegisterFailedHotkeys();
            UpdateHotkeyStatus();
        }

        private HotkeySelectionControl AddHotkeySelectionControl(HotkeySettings hotkeySettings)
        {
            HotkeySelectionControl control = new HotkeySelectionControl(hotkeySettings);
            control.Margin = new Padding(0, 0, 0, 4);
            control.SelectedChanged += control_SelectedChanged;
            control.HotkeyChanged += control_HotkeyChanged;
            control.EditRequested += control_EditRequested;
            flpHotkeys.Controls.Add(control);
            return control;
        }

        private void Edit(HotkeySelectionControl selectionControl)
        {
            TaskSettings taskSettings = selectionControl.HotkeySettings.TaskSettings;
            taskSettings.SetDefaultSettings();

            using (TaskSettingsForm taskSettingsForm = new TaskSettingsForm(taskSettings))
            {
                taskSettingsForm.ShowDialog();
                selectionControl.UpdateControls();
            }
        }

        private void control_EditRequested(object sender, EventArgs e)
        {
            Edit((HotkeySelectionControl)sender);
        }

        private void EditSelected()
        {
            if (Selected != null)
            {
                Edit(Selected);
            }
        }

        private void HandleHotkeysToggle(bool hotkeysEnabled)
        {
            UpdateHotkeyStatus();
        }

        private void flpHotkeys_Layout(object sender, LayoutEventArgs e)
        {
            foreach (Control control in flpHotkeys.Controls)
            {
                control.ClientSize = new Size(flpHotkeys.ClientSize.Width, control.ClientSize.Height);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            HotkeySettings hotkeySettings = new HotkeySettings();
            hotkeySettings.TaskSettings = TaskSettings.GetDefaultTaskSettings();
            manager.Hotkeys.Add(hotkeySettings);
            HotkeySelectionControl control = AddHotkeySelectionControl(hotkeySettings);
            control.Selected = true;
            Selected = control;
            UpdateButtons();
            UpdateCheckStates();
            control.Focus();
            Update();
            control.OpenTaskMenu();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (Selected != null)
            {
                manager.UnregisterHotkey(Selected.HotkeySettings);
                HotkeySelectionControl hsc = FindSelectionControl(Selected.HotkeySettings);
                if (hsc != null) flpHotkeys.Controls.Remove(hsc);
                Selected = null;
                UpdateButtons();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditSelected();
        }

        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            if (Selected != null)
            {
                HotkeySettings hotkeySettings = new HotkeySettings();
                hotkeySettings.TaskSettings = Selected.HotkeySettings.TaskSettings.Copy();
                hotkeySettings.TaskSettings.WatchFolderEnabled = false;
                hotkeySettings.TaskSettings.WatchFolderList = new List<WatchFolderSettings>();
                manager.Hotkeys.Add(hotkeySettings);
                HotkeySelectionControl control = AddHotkeySelectionControl(hotkeySettings);
                control.Selected = true;
                Selected = control;
                UpdateCheckStates();
                control.Focus();
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (Selected != null && flpHotkeys.Controls.Count > 1)
            {
                int index = flpHotkeys.Controls.GetChildIndex(Selected);

                int newIndex;

                if (index == 0)
                {
                    newIndex = flpHotkeys.Controls.Count - 1;
                }
                else
                {
                    newIndex = index - 1;
                }

                flpHotkeys.Controls.SetChildIndex(Selected, newIndex);
                manager.Hotkeys.Move(index, newIndex);
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (Selected != null && flpHotkeys.Controls.Count > 1)
            {
                int index = flpHotkeys.Controls.GetChildIndex(Selected);

                int newIndex;

                if (index == flpHotkeys.Controls.Count - 1)
                {
                    newIndex = 0;
                }
                else
                {
                    newIndex = index + 1;
                }

                flpHotkeys.Controls.SetChildIndex(Selected, newIndex);
                manager.Hotkeys.Move(index, newIndex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.HotkeySettingsForm_Reset_all_hotkeys_to_defaults_Confirmation, "ShareX", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                manager.ResetHotkeys();
                AddControls();
                Selected = null;
                UpdateButtons();
            }
        }

        private void btnHotkeysDisabled_Click(object sender, EventArgs e)
        {
            TaskHelpers.ToggleHotkeys(false);
            btnHotkeysDisabled.Visible = false;
        }
    }
}