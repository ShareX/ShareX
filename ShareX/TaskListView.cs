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
using System.Windows.Forms;

namespace ShareX
{
    public class TaskListView
    {
        public MyListView ListViewControl { get; private set; }

        public TaskListView(MyListView listViewControl)
        {
            ListViewControl = listViewControl;
        }

        public ListViewItem AddItem(WorkerTask task)
        {
            TaskInfo info = task.Info;

            if (task.Status != TaskStatus.History)
            {
                DebugHelper.WriteLine("Task in queue. Job: {0}, Type: {1}, Host: {2}", info.Job, info.UploadDestination, info.UploaderHost);
            }

            ListViewItem lvi = new ListViewItem();
            lvi.Tag = task;
            lvi.Text = info.FileName;

            if (task.Status == TaskStatus.History)
            {
                lvi.SubItems.Add(Resources.TaskManager_CreateListViewItem_History);
                lvi.SubItems.Add(task.Info.TaskEndTime.ToString());
            }
            else
            {
                lvi.SubItems.Add(Resources.TaskManager_CreateListViewItem_In_queue);
                lvi.SubItems.Add("");
            }

            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");

            if (task.Status == TaskStatus.History)
            {
                lvi.SubItems.Add(task.Info.ToString());
                lvi.ImageIndex = 4;
            }
            else
            {
                lvi.SubItems.Add("");
                lvi.ImageIndex = 3;
            }

            if (Program.Settings.ShowMostRecentTaskFirst)
            {
                ListViewControl.Items.Insert(0, lvi);
            }
            else
            {
                ListViewControl.Items.Add(lvi);
            }

            lvi.EnsureVisible();
            ListViewControl.FillLastColumn();

            return lvi;
        }

        public void RemoveItem(WorkerTask task)
        {
            ListViewItem lvi = FindItem(task);

            if (lvi != null)
            {
                ListViewControl.Items.Remove(lvi);
            }
        }

        public ListViewItem FindItem(WorkerTask task)
        {
            foreach (ListViewItem lvi in ListViewControl.Items)
            {
                if (lvi.Tag is WorkerTask tag && tag == task)
                {
                    return lvi;
                }
            }

            return null;
        }
    }
}