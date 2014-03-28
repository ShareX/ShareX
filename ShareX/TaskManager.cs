#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

using HelpersLib;
using HistoryLib;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace ShareX
{
    public static class TaskManager
    {
        public static MyListView ListViewControl { get; set; }

        public static bool IsBusy
        {
            get
            {
                return Tasks.Count > 0 && Tasks.Any(task => task.Status != TaskStatus.Completed);
            }
        }

        private static readonly List<UploadTask> Tasks = new List<UploadTask>();

        private static int lastIconStatus = -1;

        public static void Start(UploadTask task)
        {
            Tasks.Add(task);
            UpdateDragAndDropTip();
            task.StatusChanged += task_StatusChanged;
            task.UploadStarted += task_UploadStarted;
            task.UploadProgressChanged += task_UploadProgressChanged;
            task.UploadCompleted += task_UploadCompleted;
            CreateListViewItem(task);
            StartTasks();
        }

        public static void Remove(UploadTask task)
        {
            if (task != null)
            {
                task.Stop();
                Tasks.Remove(task);
                UpdateDragAndDropTip();

                ListViewItem lvi = FindListViewItem(task);

                if (lvi != null)
                {
                    ListViewControl.Items.Remove(lvi);
                }

                task.Dispose();
            }
        }

        private static void StartTasks()
        {
            int workingTasksCount = Tasks.Count(x => x.IsWorking);
            UploadTask[] inQueueTasks = Tasks.Where(x => x.Status == TaskStatus.InQueue).ToArray();

            if (inQueueTasks.Length > 0)
            {
                int len;

                if (Program.Settings.UploadLimit == 0)
                {
                    len = inQueueTasks.Length;
                }
                else
                {
                    len = (Program.Settings.UploadLimit - workingTasksCount).Between(0, inQueueTasks.Length);
                }

                for (int i = 0; i < len; i++)
                {
                    inQueueTasks[i].Start();
                }
            }
        }

        public static void StopAllTasks()
        {
            foreach (UploadTask task in Tasks)
            {
                if (task != null) task.Stop();
            }
        }

        private static void UpdateDragAndDropTip()
        {
            Program.MainForm.lblDragAndDropTip.Visible = Tasks.Count == 0;
        }

        private static ListViewItem FindListViewItem(UploadTask task)
        {
            if (ListViewControl != null)
            {
                foreach (ListViewItem lvi in ListViewControl.Items)
                {
                    UploadTask tag = lvi.Tag as UploadTask;

                    if (tag != null && tag == task)
                    {
                        return lvi;
                    }
                }
            }

            return null;
        }

        private static void ChangeListViewItemStatus(UploadTask task)
        {
            if (ListViewControl != null)
            {
                ListViewItem lvi = FindListViewItem(task);

                if (lvi != null)
                {
                    lvi.SubItems[1].Text = task.Info.Status;
                }
            }
        }

        private static void CreateListViewItem(UploadTask task)
        {
            if (ListViewControl != null)
            {
                TaskInfo info = task.Info;

                DebugHelper.WriteLine("Task in queue. Job: {0}, Type: {1}, Host: {2}", info.Job, info.UploadDestination, info.UploaderHost);

                ListViewItem lvi = new ListViewItem();
                lvi.Tag = task;
                lvi.Text = info.FileName;
                lvi.SubItems.Add("In queue");
                lvi.SubItems.Add(string.Empty);
                lvi.SubItems.Add(string.Empty);
                lvi.SubItems.Add(string.Empty);
                lvi.SubItems.Add(string.Empty);
                lvi.SubItems.Add(info.DataType.ToString());
                lvi.SubItems.Add(info.IsUploadJob ? info.UploaderHost : string.Empty);
                lvi.SubItems.Add(string.Empty);
                lvi.ImageIndex = 3;
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
            }
        }

        private static void task_StatusChanged(UploadTask task)
        {
            DebugHelper.WriteLine("Task status: " + task.Status);
            ChangeListViewItemStatus(task);
            UpdateProgressUI();
        }

        private static void task_UploadStarted(UploadTask task)
        {
            TaskInfo info = task.Info;

            string status = string.Format("Upload started. Filename: {0}", info.FileName);
            if (!string.IsNullOrEmpty(info.FilePath)) status += ", Filepath: " + info.FilePath;
            DebugHelper.WriteLine(status);

            ListViewItem lvi = FindListViewItem(task);

            if (lvi != null)
            {
                lvi.Text = info.FileName;
                lvi.SubItems[1].Text = info.Status;
                lvi.ImageIndex = 0;
            }
        }

        private static void task_UploadProgressChanged(UploadTask task)
        {
            if (task.Status == TaskStatus.Working && ListViewControl != null)
            {
                TaskInfo info = task.Info;

                ListViewItem lvi = FindListViewItem(task);

                if (lvi != null)
                {
                    lvi.SubItems[1].Text = string.Format("{0:0.0}%", info.Progress.Percentage);
                    lvi.SubItems[2].Text = string.Format("{0} / {1}", info.Progress.Position.ToSizeString(Program.Settings.BinaryUnits), info.Progress.Length.ToSizeString(Program.Settings.BinaryUnits));

                    if (info.Progress.Speed > 0)
                    {
                        lvi.SubItems[3].Text = ((long)info.Progress.Speed).ToSizeString(Program.Settings.BinaryUnits) + "/s";
                    }

                    lvi.SubItems[4].Text = Helpers.ProperTimeSpan(info.Progress.Elapsed);
                    lvi.SubItems[5].Text = Helpers.ProperTimeSpan(info.Progress.Remaining);
                }

                UpdateProgressUI();
            }
        }

        private static void task_UploadCompleted(UploadTask task)
        {
            try
            {
                if (ListViewControl != null && task != null)
                {
                    if (task.RequestSettingUpdate)
                    {
                        Program.MainForm.UpdateMainFormSettings();
                    }

                    TaskInfo info = task.Info;

                    if (info != null && info.Result != null)
                    {
                        ListViewItem lvi = FindListViewItem(task);

                        if (info.Result.IsError)
                        {
                            string errors = string.Join("\r\n\r\n", info.Result.Errors.ToArray());

                            DebugHelper.WriteLine("Task failed. Filename: {0}, Errors:\r\n{1}", info.FileName, errors);

                            if (lvi != null)
                            {
                                lvi.SubItems[1].Text = "Error";
                                lvi.SubItems[8].Text = string.Empty;
                                lvi.ImageIndex = 1;
                            }

                            if (task.Info.TaskSettings.GeneralSettings.PlaySoundAfterUpload)
                            {
                                SystemSounds.Asterisk.Play();
                            }
                        }
                        else
                        {
                            DebugHelper.WriteLine("Task completed. Filename: {0}, URL: {1}, Duration: {2} ms",
                                info.FileName, info.Result.ToString(), (int)info.UploadDuration.TotalMilliseconds);

                            string result = info.Result.ToString();

                            if (string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(info.FilePath))
                            {
                                result = info.FilePath;
                            }

                            if (lvi != null)
                            {
                                lvi.Text = info.FileName;
                                lvi.SubItems[1].Text = info.Status;
                                lvi.ImageIndex = 2;

                                if (!string.IsNullOrEmpty(result))
                                {
                                    lvi.SubItems[8].Text = result;
                                }
                            }

                            if (!task.IsStopped && !string.IsNullOrEmpty(result))
                            {
                                if (task.Info.TaskSettings.GeneralSettings.SaveHistory)
                                {
                                    HistoryManager.AddHistoryItemAsync(Program.HistoryFilePath, info.GetHistoryItem());
                                }

                                if (!info.TaskSettings.AdvancedSettings.DisableNotifications)
                                {
                                    if (task.Info.TaskSettings.GeneralSettings.PlaySoundAfterUpload)
                                    {
                                        SystemSounds.Exclamation.Play();
                                    }

                                    if (!string.IsNullOrEmpty(info.TaskSettings.AdvancedSettings.BalloonTipContentFormat))
                                    {
                                        result = new UploadInfoParser().Parse(info, info.TaskSettings.AdvancedSettings.BalloonTipContentFormat);
                                    }
                                    TaskHelpers.ShowResultNotifications(result, info.TaskSettings, info.FilePath);

                                    if (info.TaskSettings.GeneralSettings.ShowAfterUploadForm)
                                    {
                                        AfterUploadForm dlg = new AfterUploadForm(info);
                                        NativeMethods.ShowWindow(dlg.Handle, (int)WindowShowStyle.ShowNoActivate);
                                    }
                                }
                            }
                        }

                        if (lvi != null)
                        {
                            lvi.EnsureVisible();
                        }
                    }
                }
            }
            finally
            {
                StartTasks();
                UpdateProgressUI();
            }
        }

        public static void UpdateProgressUI()
        {
            bool isWorkingTasks = false;
            double averageProgress = 0;

            IEnumerable<UploadTask> workingTasks = Tasks.Where(x => x != null && x.Status == TaskStatus.Working && x.Info != null);

            if (workingTasks.Count() > 0)
            {
                isWorkingTasks = true;

                workingTasks = workingTasks.Where(x => x.Info.Progress != null);

                if (workingTasks.Count() > 0)
                {
                    averageProgress = workingTasks.Average(x => x.Info.Progress.Percentage);
                }
            }

            int progress = isWorkingTasks ? ((int)averageProgress).Between(0, 99) : -1;
            UpdateTrayIcon(progress);

            string title;

            if (isWorkingTasks)
            {
                title = string.Format("{0} - {1:0.0}%", Program.Title, averageProgress);
                TaskbarManager.SetProgressValue(Program.MainForm, (int)averageProgress);
            }
            else
            {
                title = Program.Title;
            }

            Program.MainForm.Text = title;

            if (!IsBusy)
            {
                TaskbarManager.SetProgressState(Program.MainForm, TaskbarProgressBarStatus.NoProgress);
            }
        }

        public static void UpdateTrayIcon(int progress = -1)
        {
            if (Program.Settings.TrayIconProgressEnabled && Program.MainForm.niTray.Visible && lastIconStatus != progress)
            {
                Icon icon;

                if (progress >= 0)
                {
                    icon = TaskHelpers.GetProgressIcon(progress);
                }
                else
                {
                    icon = ShareXResources.Icon;
                }

                using (Icon oldIcon = Program.MainForm.niTray.Icon)
                {
                    Program.MainForm.niTray.Icon = icon;
                    oldIcon.DisposeHandle();
                }

                lastIconStatus = progress;
            }
        }
    }
}