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
using System.Windows.Forms;

namespace ShareX
{
    public partial class QuickTaskMenuEditorForm : Form
    {
        public QuickTaskMenuEditorForm()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            if (Program.Settings.QuickTaskPresets == null)
            {
                Program.Settings.QuickTaskPresets = new List<QuickTaskInfo>();
            }

            UpdateItems();
        }

        private void UpdateItem(ListViewItem lvi, QuickTaskInfo taskInfo)
        {
            lvi.Tag = taskInfo;
            lvi.Text = taskInfo.ToString();
        }

        private void UpdateItems()
        {
            lvPresets.Items.Clear();

            foreach (QuickTaskInfo taskInfo in Program.Settings.QuickTaskPresets)
            {
                ListViewItem lvi = new ListViewItem();
                UpdateItem(lvi, taskInfo);
                lvPresets.Items.Add(lvi);
            }
        }

        private void Edit(ListViewItem lvi, QuickTaskInfo taskInfo)
        {
            new QuickTaskInfoEditForm(taskInfo).ShowDialog();

            UpdateItem(lvi, taskInfo);
        }

        private void EditSelectedItem()
        {
            if (lvPresets.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvPresets.SelectedItems[0];
                QuickTaskInfo taskInfo = lvi.Tag as QuickTaskInfo;
                Edit(lvi, taskInfo);
            }
        }

        private void lvPresets_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                EditSelectedItem();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            QuickTaskInfo taskInfo = new QuickTaskInfo();
            ListViewItem lvi = new ListViewItem();
            Program.Settings.QuickTaskPresets.Add(taskInfo);
            lvPresets.Items.Add(lvi);
            Edit(lvi, taskInfo);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditSelectedItem();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvPresets.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvPresets.SelectedItems[0];
                QuickTaskInfo taskInfo = lvi.Tag as QuickTaskInfo;
                Program.Settings.QuickTaskPresets.Remove(taskInfo);
                lvPresets.Items.Remove(lvi);
            }
        }

        private void lvPresets_ItemMoved(object sender, int oldIndex, int newIndex)
        {
            Program.Settings.QuickTaskPresets.Move(oldIndex, newIndex);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.QuickTaskMenuEditorForm_Reset_all_quick_tasks_to_defaults_Confirmation, "ShareX", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Program.Settings.QuickTaskPresets = QuickTaskInfo.DefaultPresets;
                UpdateItems();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}