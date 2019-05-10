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

        public TaskView()
        {
            InitializeComponent();

            TaskPanels = new List<TaskPanel>();
            AddTestTaskPanels();
        }

        public void AddTestTaskPanels()
        {
            for (int i = 0; i < 3; i++)
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
            TaskPanels.Add(panel);
            flpMain.Controls.Add(panel);
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

        public void HideProgress(WorkerTask task)
        {
            TaskPanel panel = FindPanel(task);

            if (panel != null)
            {
                panel.ProgressVisible = false;
            }
        }
    }
}