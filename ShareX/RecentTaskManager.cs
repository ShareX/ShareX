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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ShareX
{
    public class RecentTaskManager
    {
        private int maxCount = 10;

        public int MaxCount
        {
            get
            {
                return maxCount;
            }
            set
            {
                maxCount = value.Clamp(1, 100);

                lock (itemsLock)
                {
                    while (Tasks.Count > maxCount)
                    {
                        Tasks.Dequeue();
                    }

                    UpdateTrayMenu();
                }
            }
        }

        public Queue<RecentTask> Tasks { get; private set; }

        private static readonly object itemsLock = new object();

        public RecentTaskManager()
        {
            Tasks = new Queue<RecentTask>();
        }

        public void InitItems()
        {
            lock (itemsLock)
            {
                MaxCount = Program.Settings.RecentTasksMaxCount;

                if (Program.Settings.RecentTasks != null)
                {
                    Tasks = new Queue<RecentTask>(Program.Settings.RecentTasks.Take(MaxCount));
                }

                UpdateTrayMenu();
                UpdateMainWindowList();
            }
        }

        public void Add(WorkerTask task)
        {
            string info = task.Info.ToString();

            if (!string.IsNullOrEmpty(info))
            {
                RecentTask recentItem = new RecentTask()
                {
                    FilePath = task.Info.FilePath,
                    URL = task.Info.Result.URL,
                    ThumbnailURL = task.Info.Result.ThumbnailURL,
                    DeletionURL = task.Info.Result.DeletionURL,
                    ShortenedURL = task.Info.Result.ShortenedURL
                };

                Add(recentItem);
            }

            if (Program.Settings.RecentTasksSave)
            {
                Program.Settings.RecentTasks = Tasks.ToArray();
            }
            else
            {
                Program.Settings.RecentTasks = null;
            }
        }

        public void Add(RecentTask task)
        {
            lock (itemsLock)
            {
                while (Tasks.Count >= MaxCount)
                {
                    Tasks.Dequeue();
                }

                Tasks.Enqueue(task);

                UpdateTrayMenu();
            }
        }

        public void Clear()
        {
            lock (itemsLock)
            {
                Tasks.Clear();

                Program.Settings.RecentTasks = null;

                UpdateTrayMenu();
            }
        }

        private void UpdateTrayMenu()
        {
            ToolStripMenuItem tsmi = Program.MainForm.tsmiTrayRecentItems;

            if (Program.Settings.RecentTasksSave && Program.Settings.RecentTasksShowInTrayMenu && Tasks.Count > 0)
            {
                tsmi.Visible = true;

                tsmi.DropDownItems.Clear();
                ToolStripMenuItem tsmiTip = new ToolStripMenuItem(Resources.RecentManager_UpdateRecentMenu_Left_click_to_copy_URL_to_clipboard__Right_click_to_open_URL_);
                tsmiTip.Enabled = false;
                tsmi.DropDownItems.Add(tsmiTip);
                tsmi.DropDownItems.Add(new ToolStripSeparator());

                foreach (RecentTask task in Tasks)
                {
                    ToolStripMenuItem tsmiLink = new ToolStripMenuItem();
                    tsmiLink.Text = task.TrayMenuText;
                    string link = task.ToString();
                    tsmiLink.ToolTipText = link;
                    tsmiLink.MouseUp += (sender, e) =>
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            ClipboardHelpers.CopyText(link);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            URLHelpers.OpenURL(link);
                        }
                    };

                    if (Program.Settings.RecentTasksTrayMenuMostRecentFirst)
                    {
                        tsmi.DropDownItems.Insert(2, tsmiLink);
                    }
                    else
                    {
                        tsmi.DropDownItems.Add(tsmiLink);
                    }
                }
            }
            else
            {
                tsmi.Visible = false;
            }
        }

        private void UpdateMainWindowList()
        {
            if (Program.Settings.RecentTasksSave && Program.Settings.RecentTasksShowInMainWindow && Tasks.Count > 0)
            {
                TaskManager.AddRecentTasksToMainWindow();
            }
        }
    }
}