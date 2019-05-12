#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareX.HelpersLib;

namespace ShareX
{
    public partial class TaskView : UserControl
    {
        public List<TaskPanel> TaskPanels { get; private set; }
        public Size ThumbnailSize { get; set; } = new Size(200, 150);
        public WorkerTask SelectedTask { get; private set; }

        public delegate void TaskViewMouseEventHandler(object sender, MouseEventArgs e, WorkerTask task);
        public event TaskViewMouseEventHandler ContextMenuRequested;

        public TaskView()
        {
            InitializeComponent();

            TaskPanels = new List<TaskPanel>();
            //AddTestTaskPanels();
        }

        public void AddTestTaskPanels()
        {
            for (int i = 0; i < 7; i++)
            {
                WorkerTask task = WorkerTask.CreateHistoryTask(new RecentTask()
                {
                    FilePath = @"..\..\..\ShareX.HelpersLib\Resources\ShareX_Logo.png"
                });

                AddTaskPanel(task);
            }
        }

        public TaskPanel FindPanel(WorkerTask task)
        {
            return TaskPanels.FirstOrDefault(x => x.Task == task);
        }

        public void AddTaskPanel(WorkerTask task)
        {
            TaskPanel panel = new TaskPanel(task);
            panel.ChangeThumbnailSize(ThumbnailSize);
            panel.MouseDown += (sender, e) => SelectedTask = panel.Task;
            panel.MouseUp += Panel_MouseUp;
            TaskPanels.Add(panel);
            flpMain.Controls.Add(panel);
            flpMain.Controls.SetChildIndex(panel, 0);
        }

        protected void OnContextMenuRequested(object sender, MouseEventArgs e, WorkerTask task)
        {
            if (ContextMenuRequested != null)
            {
                ContextMenuRequested(sender, e, task);
            }
        }

        private void FlpMain_MouseDown(object sender, MouseEventArgs e)
        {
            SelectedTask = null;
        }

        private void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                OnContextMenuRequested(sender, e, SelectedTask);
            }
        }

        public void UpdateFilename(WorkerTask task)
        {
            FindPanel(task)?.UpdateFilename();
        }

        public void UpdateThumbnail(WorkerTask task)
        {
            FindPanel(task)?.UpdateThumbnail();
        }

        public void UpdateProgress(WorkerTask task)
        {
            FindPanel(task)?.UpdateProgress();
        }

        public void UpdateProgressVisible(WorkerTask task, bool visible)
        {
            TaskPanel panel = FindPanel(task);

            if (panel != null)
            {
                panel.ProgressVisible = visible;
            }
        }
    }
}