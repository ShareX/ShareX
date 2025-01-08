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
using System;
using System.Linq;
using System.Windows.Forms;

namespace ShareX
{
    public partial class QuickTaskInfoEditForm : Form
    {
        public QuickTaskInfo TaskInfo { get; private set; }

        public QuickTaskInfoEditForm(QuickTaskInfo taskInfo)
        {
            TaskInfo = taskInfo;

            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            txtName.Text = TaskInfo.Name;
            AddMultiEnumItemsContextMenu<AfterCaptureTasks>(x => TaskInfo.AfterCaptureTasks = TaskInfo.AfterCaptureTasks.Swap(x), cmsAfterCapture);
            AddMultiEnumItemsContextMenu<AfterUploadTasks>(x => TaskInfo.AfterUploadTasks = TaskInfo.AfterUploadTasks.Swap(x), cmsAfterUpload);
            SetMultiEnumCheckedContextMenu(TaskInfo.AfterCaptureTasks, cmsAfterCapture);
            SetMultiEnumCheckedContextMenu(TaskInfo.AfterUploadTasks, cmsAfterUpload);
            UpdateUploaderMenuNames();
        }

        private void AddMultiEnumItemsContextMenu<T>(Action<T> selectedEnum, params ToolStripDropDown[] parents) where T : Enum
        {
            string[] enums = Helpers.GetLocalizedEnumDescriptions<T>().Skip(1).Select(x => x.Replace("&", "&&")).ToArray();

            foreach (ToolStripDropDown parent in parents)
            {
                for (int i = 0; i < enums.Length; i++)
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(enums[i]);
                    tsmi.Image = TaskHelpers.FindMenuIcon<T>(i + 1);

                    int index = i;

                    tsmi.Click += (sender, e) =>
                    {
                        foreach (ToolStripDropDown parent2 in parents)
                        {
                            ToolStripMenuItem tsmi2 = (ToolStripMenuItem)parent2.Items[index];
                            tsmi2.Checked = !tsmi2.Checked;
                        }

                        selectedEnum((T)Enum.ToObject(typeof(T), 1 << index));

                        UpdateUploaderMenuNames();
                    };

                    parent.Items.Add(tsmi);
                }
            }
        }

        private void SetMultiEnumCheckedContextMenu(Enum value, params ToolStripDropDown[] parents)
        {
            for (int i = 0; i < parents[0].Items.Count; i++)
            {
                foreach (ToolStripDropDown parent in parents)
                {
                    ToolStripMenuItem tsmi = (ToolStripMenuItem)parent.Items[i];
                    tsmi.Checked = value.HasFlag(1 << i);
                }
            }
        }

        private void UpdateUploaderMenuNames()
        {
            txtName.SetWatermark(TaskInfo.ToString(), true);
            mbAfterCaptureTasks.Text = string.Join(", ", TaskInfo.AfterCaptureTasks.GetFlags().Select(x => x.GetLocalizedDescription()));
            mbAfterUploadTasks.Text = string.Join(", ", TaskInfo.AfterUploadTasks.GetFlags().Select(x => x.GetLocalizedDescription()));
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            TaskInfo.Name = txtName.Text;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}