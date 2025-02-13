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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX
{
    public partial class HotkeySelectionControl : UserControl
    {
        public event EventHandler HotkeyChanged;
        public event EventHandler SelectedChanged;
        public event EventHandler EditRequested;

        public HotkeySettings HotkeySettings { get; private set; }

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

                UpdateControls();
            }
        }

        public bool EditingHotkey { get; private set; }

        public HotkeySelectionControl(HotkeySettings hotkeySettings)
        {
            HotkeySettings = hotkeySettings;

            InitializeComponent();
            UpdateControls();

            AddEnumItemsContextMenu<HotkeyType>(x =>
            {
                HotkeySettings.TaskSettings.Job = x;
                UpdateControls();
            }, cmsTask);
            SetEnumCheckedContextMenu(HotkeySettings.TaskSettings.Job, cmsTask);

            if (ShareXResources.UseCustomTheme)
            {
                ShareXResources.ApplyCustomThemeToControl(this);
            }

            UpdateHotkeyStatus();
        }

        private void AddEnumItemsContextMenu<T>(Action<T> selectedEnum, params ToolStripDropDown[] parents) where T : Enum
        {
            EnumInfo[] enums = Helpers.GetEnums<T>().OfType<Enum>().Select(x => new EnumInfo(x)).ToArray();

            foreach (ToolStripDropDown parent in parents)
            {
                foreach (EnumInfo enumInfo in enums)
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(enumInfo.Description.Replace("&", "&&"));
                    tsmi.Image = TaskHelpers.FindMenuIcon(enumInfo.Value);
                    tsmi.Tag = enumInfo;

                    tsmi.Click += (sender, e) =>
                    {
                        SetEnumCheckedContextMenu(enumInfo, parents);

                        selectedEnum((T)enumInfo.Value);

                        UpdateControls();
                    };

                    if (!string.IsNullOrEmpty(enumInfo.Category))
                    {
                        ToolStripMenuItem tsmiParent = parent.Items.OfType<ToolStripMenuItem>().FirstOrDefault(x => x.Text == enumInfo.Category);

                        if (tsmiParent == null)
                        {
                            tsmiParent = new ToolStripMenuItem(enumInfo.Category);
                            parent.Items.Add(tsmiParent);
                        }

                        tsmiParent.DropDownItems.Add(tsmi);
                    }
                    else
                    {
                        parent.Items.Add(tsmi);
                    }
                }
            }
        }

        private void SetEnumCheckedContextMenu(Enum value, params ToolStripDropDown[] parents)
        {
            SetEnumCheckedContextMenu(new EnumInfo(value), parents);
        }

        private void SetEnumCheckedContextMenu(EnumInfo enumInfo, params ToolStripDropDown[] parents)
        {
            foreach (ToolStripDropDown parent in parents)
            {
                foreach (ToolStripMenuItem tsmiParent in parent.Items)
                {
                    EnumInfo currentEnumInfo;

                    if (tsmiParent.DropDownItems.Count > 0)
                    {
                        foreach (ToolStripMenuItem tsmiCategoryParent in tsmiParent.DropDownItems)
                        {
                            currentEnumInfo = (EnumInfo)tsmiCategoryParent.Tag;
                            tsmiCategoryParent.Checked = currentEnumInfo.Value.Equals(enumInfo.Value);
                        }
                    }
                    else
                    {
                        currentEnumInfo = (EnumInfo)tsmiParent.Tag;
                        tsmiParent.Checked = currentEnumInfo.Value.Equals(enumInfo.Value);
                    }
                }
            }
        }

        public void UpdateControls()
        {
            if (Selected)
            {
                btnTask.ChangeFontStyle(FontStyle.Bold);
            }
            else
            {
                btnTask.ChangeFontStyle(FontStyle.Regular);
            }

            btnTask.Image = TaskHelpers.FindMenuIcon(HotkeySettings.TaskSettings.Job);

            string taskText = " " + HotkeySettings.TaskSettings.ToString();
            if (!HotkeySettings.TaskSettings.IsUsingDefaultSettings)
            {
                taskText += "*";
            }
            btnTask.Text = taskText;
        }

        public void UpdateHotkeyStatus()
        {
            btnHotkey.Text = HotkeySettings.HotkeyInfo.ToString();

            switch (HotkeySettings.HotkeyInfo.Status)
            {
                default:
                case HotkeyStatus.NotConfigured:
                    btnHotkey.Image = Resources.status_away;
                    break;
                case HotkeyStatus.Failed:
                    btnHotkey.Image = Resources.status_busy;
                    break;
                case HotkeyStatus.Registered:
                    btnHotkey.Image = Resources.status;
                    break;
            }
        }

        private void StartEditing()
        {
            EditingHotkey = true;

            Program.HotkeyManager.IgnoreHotkeys = true;

            HotkeySettings.HotkeyInfo.Hotkey = Keys.None;
            HotkeySettings.HotkeyInfo.Win = false;
            OnHotkeyChanged();
            UpdateHotkeyStatus();
            btnHotkey.Text = Resources.HotkeySelectionControl_StartEditing_Select_a_hotkey___;
        }

        private void StopEditing()
        {
            EditingHotkey = false;

            Program.HotkeyManager.IgnoreHotkeys = false;

            if (HotkeySettings.HotkeyInfo.IsOnlyModifiers)
            {
                HotkeySettings.HotkeyInfo.Hotkey = Keys.None;
            }

            OnHotkeyChanged();
            UpdateHotkeyStatus();
        }

        public void OpenTaskMenu()
        {
            btnTask.OpenMenu();
        }

        private void SelectControl()
        {
            Selected = true;
            OnSelectedChanged();
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

        private void btnTask_Click(object sender, EventArgs e)
        {
            SelectControl();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            SelectControl();
            OnEditRequested();
        }

        private void btnHotkey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (EditingHotkey)
            {
                // To handle "Tab" key etc.
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
                    HotkeySettings.HotkeyInfo.Hotkey = Keys.None;
                    StopEditing();
                }
                else if (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
                {
                    HotkeySettings.HotkeyInfo.Win = !HotkeySettings.HotkeyInfo.Win;
                    UpdateHotkeyStatus();
                }
                else if (new HotkeyInfo(e.KeyData).IsValidHotkey)
                {
                    HotkeySettings.HotkeyInfo.Hotkey = e.KeyData;
                    StopEditing();
                }
                else
                {
                    HotkeySettings.HotkeyInfo.Hotkey = e.KeyData;
                    UpdateHotkeyStatus();
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
                    HotkeySettings.HotkeyInfo.Hotkey = e.KeyData;
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
                SelectControl();
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
    }
}