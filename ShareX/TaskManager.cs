#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using ShareX.HistoryLib;
using ShareX.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
                return Tasks.Count > 0 && Tasks.Any(task => task.IsBusy);
            }
        }

        private static readonly List<WorkerTask> Tasks = new List<WorkerTask>();

        public static readonly RecentTaskManager RecentManager = new RecentTaskManager();

        private static int lastIconStatus = -1;

        public static void Start(WorkerTask task)
        {
            if (task != null)
            {
                Tasks.Add(task);
                UpdateMainFormTip();

                if (task.Status != TaskStatus.History)
                {
                    task.StatusChanged += task_StatusChanged;
                    task.UploadStarted += task_UploadStarted;
                    task.UploadProgressChanged += task_UploadProgressChanged;
                    task.UploadCompleted += task_UploadCompleted;
                }

                CreateListViewItem(task);

                if (task.Status != TaskStatus.History)
                {
                    StartTasks();
                }
            }
        }

        public static void Remove(WorkerTask task)
        {
            if (task != null)
            {
                task.Stop();
                Tasks.Remove(task);
                UpdateMainFormTip();

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
            WorkerTask[] inQueueTasks = Tasks.Where(x => x.Status == TaskStatus.InQueue).ToArray();

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
            foreach (WorkerTask task in Tasks)
            {
                if (task != null) task.Stop();
            }
        }

        public static void UpdateMainFormTip()
        {
            Program.MainForm.lblMainFormTip.Visible = Program.Settings.ShowMainWindowTip && Tasks.Count == 0;
        }

        private static ListViewItem FindListViewItem(WorkerTask task)
        {
            if (ListViewControl != null)
            {
                foreach (ListViewItem lvi in ListViewControl.Items)
                {
                    WorkerTask tag = lvi.Tag as WorkerTask;

                    if (tag != null && tag == task)
                    {
                        return lvi;
                    }
                }
            }

            return null;
        }

        private static void ChangeListViewItemStatus(WorkerTask task)
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

        private static void CreateListViewItem(WorkerTask task)
        {
            if (ListViewControl != null)
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
                    // TODO: Translate
                    lvi.SubItems.Add("History");
                    lvi.SubItems.Add(task.Info.UploadTime.ToString());
                }
                else
                {
                    lvi.SubItems.Add(Resources.TaskManager_CreateListViewItem_In_queue);
                    lvi.SubItems.Add(string.Empty);
                }

                lvi.SubItems.Add(string.Empty);
                lvi.SubItems.Add(string.Empty);
                lvi.SubItems.Add(string.Empty);

                if (task.Status == TaskStatus.History)
                {
                    lvi.SubItems.Add(task.Info.ToString());
                    lvi.ImageIndex = 4;
                }
                else
                {
                    lvi.SubItems.Add(string.Empty);
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
            }
        }

        private static void task_StatusChanged(WorkerTask task)
        {
            DebugHelper.WriteLine("Task status: " + task.Status);
            ChangeListViewItemStatus(task);
            UpdateProgressUI();
        }

        private static void task_UploadStarted(WorkerTask task)
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

        private static void task_UploadProgressChanged(WorkerTask task)
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

        private static void task_UploadCompleted(WorkerTask task)
        {
            try
            {
                if (ListViewControl != null && task != null)
                {
                    if (task.RequestSettingUpdate)
                    {
                        Program.MainForm.UpdateCheckStates();
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
                                lvi.SubItems[1].Text = Resources.TaskManager_task_UploadCompleted_Error;
                                lvi.SubItems[6].Text = string.Empty;
                                lvi.ImageIndex = 1;
                            }

                            if (!info.TaskSettings.AdvancedSettings.DisableNotifications)
                            {
                                if (info.TaskSettings.GeneralSettings.PlaySoundAfterUpload)
                                {
                                    TaskHelpers.PlayErrorSound(info.TaskSettings);
                                }

                                if (info.TaskSettings.GeneralSettings.PopUpNotification != PopUpNotificationType.None && Program.MainForm.niTray.Visible && !string.IsNullOrEmpty(errors))
                                {
                                    Program.MainForm.niTray.Tag = null;
                                    Program.MainForm.niTray.ShowBalloonTip(5000, "ShareX - " + Resources.TaskManager_task_UploadCompleted_Error, errors, ToolTipIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            DebugHelper.WriteLine("Task completed. Filename: {0}, URL: {1}, Duration: {2} ms", info.FileName, info.Result.ToString(), (int)info.UploadDuration.TotalMilliseconds);

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
                                    lvi.SubItems[6].Text = result;
                                }
                            }

                            if (!task.StopRequested && !string.IsNullOrEmpty(result))
                            {
                                if (info.TaskSettings.GeneralSettings.SaveHistory && (!info.TaskSettings.AdvancedSettings.HistorySaveOnlyURL ||
                                   (!string.IsNullOrEmpty(info.Result.URL) || !string.IsNullOrEmpty(info.Result.ShortenedURL))))
                                {
                                    HistoryManager.AddHistoryItemAsync(Program.HistoryFilePath, info.GetHistoryItem());
                                }

                                RecentManager.Add(task);

                                if (Program.Settings.RecentLinksRemember)
                                {
                                    Program.Settings.RecentTasks = RecentManager.Tasks.ToArray();
                                }
                                else
                                {
                                    Program.Settings.RecentTasks = null;
                                }

                                if (!info.TaskSettings.AdvancedSettings.DisableNotifications && info.Job != TaskJob.ShareURL)
                                {
                                    if (info.TaskSettings.GeneralSettings.PlaySoundAfterUpload)
                                    {
                                        TaskHelpers.PlayTaskCompleteSound(info.TaskSettings);
                                    }

                                    if (!string.IsNullOrEmpty(info.TaskSettings.AdvancedSettings.BalloonTipContentFormat))
                                    {
                                        result = new UploadInfoParser().Parse(info, info.TaskSettings.AdvancedSettings.BalloonTipContentFormat);
                                    }

                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        switch (info.TaskSettings.GeneralSettings.PopUpNotification)
                                        {
                                            case PopUpNotificationType.BalloonTip:
                                                if (Program.MainForm.niTray.Visible)
                                                {
                                                    Program.MainForm.niTray.Tag = result;
                                                    Program.MainForm.niTray.ShowBalloonTip(5000, "ShareX - " + Resources.TaskManager_task_UploadCompleted_ShareX___Task_completed,
                                                        result, ToolTipIcon.Info);
                                                }
                                                break;
                                            case PopUpNotificationType.ToastNotification:
                                                NotificationFormConfig toastConfig = new NotificationFormConfig()
                                                {
                                                    Action = info.TaskSettings.AdvancedSettings.ToastWindowClickAction,
                                                    FilePath = info.FilePath,
                                                    Text = "ShareX - " + Resources.TaskManager_task_UploadCompleted_ShareX___Task_completed + "\r\n" + result,
                                                    URL = result
                                                };
                                                NotificationForm.Show((int)(info.TaskSettings.AdvancedSettings.ToastWindowDuration * 1000),
                                                    info.TaskSettings.AdvancedSettings.ToastWindowPlacement,
                                                    info.TaskSettings.AdvancedSettings.ToastWindowSize, toastConfig);
                                                break;
                                        }
                                    }

                                    if (info.TaskSettings.AfterUploadJob.HasFlag(AfterUploadTasks.ShowAfterUploadWindow))
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
                if (!IsBusy && Program.CLI.IsCommandExist("AutoClose"))
                {
                    Application.Exit();
                }
                else
                {
                    StartTasks();
                    UpdateProgressUI();
                }
            }
        }

        public static void UpdateProgressUI()
        {
            bool isWorkingTasks = false;
            double averageProgress = 0;

            IEnumerable<WorkerTask> workingTasks = Tasks.Where(x => x != null && x.Status == TaskStatus.Working && x.Info != null);

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
                    try
                    {
                        icon = TaskHelpers.GetProgressIcon(progress);
                    }
                    catch (Exception e)
                    {
                        DebugHelper.WriteException(e);
                        progress = -1;
                        if (lastIconStatus == progress) return;
                        icon = ShareXResources.Icon;
                    }
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

        public static void AddRecentTasksToMainWindow()
        {
            foreach (RecentTask recentTask in RecentManager.Tasks)
            {
                WorkerTask task = WorkerTask.CreateHistoryTask(recentTask);
                Start(task);
            }
        }
    }
}