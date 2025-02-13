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
using ShareX.HistoryLib;
using ShareX.Properties;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX
{
    public static class TaskManager
    {
        public static List<WorkerTask> Tasks { get; } = new List<WorkerTask>();
        public static TaskListView TaskListView { get; set; }
        public static TaskThumbnailView TaskThumbnailView { get; set; }
        public static RecentTaskManager RecentManager { get; } = new RecentTaskManager();
        public static bool IsBusy => Tasks.Count > 0 && Tasks.Any(task => task.IsBusy);

        private static int lastIconStatus = -1;

        public static void Start(WorkerTask task)
        {
            if (task != null)
            {
                Tasks.Add(task);
                UpdateMainFormTip();

                if (task.Status != TaskStatus.History)
                {
                    task.StatusChanged += Task_StatusChanged;
                    task.ImageReady += Task_ImageReady;
                    task.UploadStarted += Task_UploadStarted;
                    task.UploadProgressChanged += Task_UploadProgressChanged;
                    task.UploadCompleted += Task_UploadCompleted;
                    task.TaskCompleted += Task_TaskCompleted;
                    task.UploadersConfigWindowRequested += Task_UploadersConfigWindowRequested;
                }

                TaskListView.AddItem(task);

                TaskThumbnailPanel panel = TaskThumbnailView.AddPanel(task);

                if (Program.Settings.TaskViewMode == TaskViewMode.ThumbnailView)
                {
                    panel.UpdateThumbnail();
                }

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

                TaskListView.RemoveItem(task);

                TaskThumbnailView.RemovePanel(task);

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
                    len = (Program.Settings.UploadLimit - workingTasksCount).Clamp(0, inQueueTasks.Length);
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
            Program.MainForm.pHotkeys.Visible = Program.Settings.ShowMainWindowTip && Tasks.Count == 0;
        }

        private static void Task_StatusChanged(WorkerTask task)
        {
            DebugHelper.WriteLine("Task status: " + task.Status);

            ListViewItem lvi = TaskListView.FindItem(task);

            if (lvi != null)
            {
                lvi.SubItems[1].Text = task.Info.Status;
            }

            UpdateProgressUI();
        }

        private static void Task_ImageReady(WorkerTask task, Bitmap image)
        {
            TaskThumbnailPanel panel = TaskThumbnailView.FindPanel(task);

            if (panel != null)
            {
                panel.UpdateTitle();

                if (Program.Settings.TaskViewMode == TaskViewMode.ThumbnailView)
                {
                    panel.UpdateThumbnail(image);
                }
            }
        }

        private static void Task_UploadStarted(WorkerTask task)
        {
            TaskInfo info = task.Info;

            string status = string.Format("Upload started. File name: {0}", info.FileName);
            if (!string.IsNullOrEmpty(info.FilePath)) status += ", File path: " + info.FilePath;
            DebugHelper.WriteLine(status);

            ListViewItem lvi = TaskListView.FindItem(task);

            if (lvi != null)
            {
                lvi.Text = info.FileName;
                lvi.SubItems[1].Text = info.Status;
                lvi.ImageIndex = 0;
            }

            TaskThumbnailPanel panel = TaskThumbnailView.FindPanel(task);

            if (panel != null)
            {
                panel.UpdateStatus();
                panel.ProgressVisible = true;
            }
        }

        private static void Task_UploadProgressChanged(WorkerTask task)
        {
            if (task.Status == TaskStatus.Working)
            {
                TaskInfo info = task.Info;

                ListViewItem lvi = TaskListView.FindItem(task);

                if (lvi != null)
                {
                    lvi.SubItems[1].Text = string.Format("{0:0.0}%", info.Progress.Percentage);
                    lvi.SubItems[2].Text = string.Format("{0} / {1}", info.Progress.Position.ToSizeString(Program.Settings.BinaryUnits),
                        info.Progress.Length.ToSizeString(Program.Settings.BinaryUnits));

                    if (info.Progress.Speed > 0)
                    {
                        lvi.SubItems[3].Text = ((long)info.Progress.Speed).ToSizeString(Program.Settings.BinaryUnits) + "/s";
                    }

                    lvi.SubItems[4].Text = Helpers.ProperTimeSpan(info.Progress.Elapsed);
                    lvi.SubItems[5].Text = Helpers.ProperTimeSpan(info.Progress.Remaining);
                }

                TaskThumbnailPanel panel = TaskThumbnailView.FindPanel(task);

                if (panel != null)
                {
                    panel.UpdateProgress();
                }

                UpdateProgressUI();
            }
        }

        private static void Task_UploadCompleted(WorkerTask task)
        {
            TaskInfo info = task.Info;

            if (info != null && info.Result != null && !info.Result.IsError)
            {
                string url = info.Result.ToString();

                if (!string.IsNullOrEmpty(url))
                {
                    string text = $"Upload completed. URL: {url}";

                    if (info.UploadDuration != null)
                    {
                        text += $", Duration: {info.UploadDuration.ElapsedMilliseconds} ms";
                    }

                    DebugHelper.WriteLine(text);
                }
            }

            TaskThumbnailPanel panel = TaskThumbnailView.FindPanel(task);

            if (panel != null)
            {
                panel.ProgressVisible = false;
            }
        }

        private static void Task_TaskCompleted(WorkerTask task)
        {
            try
            {
                if (task != null)
                {
                    task.KeepImage = false;

                    if (task.RequestSettingUpdate)
                    {
                        Program.MainForm.UpdateCheckStates();
                    }

                    TaskInfo info = task.Info;

                    if (info != null && info.Result != null)
                    {
                        string result = info.ToString();

                        if (!string.IsNullOrEmpty(result))
                        {
                            if (Program.Settings.HistorySaveTasks && (!Program.Settings.HistoryCheckURL ||
                                !string.IsNullOrEmpty(info.Result.URL) || !string.IsNullOrEmpty(info.Result.ShortenedURL)))
                            {
                                HistoryItem historyItem = info.GetHistoryItem();
                                AppendHistoryItemAsync(historyItem);
                            }

                            RecentManager.Add(task);
                        }

                        TaskThumbnailPanel panel = TaskThumbnailView.FindPanel(task);

                        if (panel != null)
                        {
                            panel.UpdateStatus();
                            panel.ProgressVisible = false;
                        }

                        ListViewItem lvi = TaskListView.FindItem(task);

                        if (task.Status == TaskStatus.Stopped)
                        {
                            DebugHelper.WriteLine($"Task stopped. File name: {info.FileName}");

                            if (lvi != null)
                            {
                                lvi.Text = info.FileName;
                                lvi.SubItems[1].Text = info.Status;
                                lvi.ImageIndex = 2;
                            }
                        }
                        else if (task.Status == TaskStatus.Failed)
                        {
                            string errors = info.Result.Errors.ToString();

                            DebugHelper.WriteLine($"Task failed. File name: {info.FileName}, Errors:\r\n{errors}");

                            if (lvi != null)
                            {
                                lvi.SubItems[1].Text = info.Status;
                                lvi.SubItems[6].Text = "";
                                lvi.ImageIndex = 1;
                            }

                            TaskHelpers.PlayNotificationSoundAsync(NotificationSound.Error, info.TaskSettings);

                            if (info.Result.Errors.Count > 0)
                            {
                                UploaderErrorInfo error = info.Result.Errors.Errors[0];

                                string title = error.Title;

                                if (string.IsNullOrEmpty(title))
                                {
                                    title = Resources.TaskManager_task_UploadCompleted_Error;
                                }

                                if (info.TaskSettings.GeneralSettings.ShowToastNotificationAfterTaskCompleted && !string.IsNullOrEmpty(error.Text) &&
                                    (!info.TaskSettings.GeneralSettings.DisableNotificationsOnFullscreen || !CaptureHelpers.IsActiveWindowFullscreen()))
                                {
                                    TaskHelpers.ShowNotificationTip(error.Text, "ShareX - " + title, 5000);
                                }
                            }
                        }
                        else
                        {
                            DebugHelper.WriteLine($"Task completed. File name: {info.FileName}, Duration: {(long)info.TaskDuration.TotalMilliseconds} ms");

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

                            if (!task.StopRequested && info.Job != TaskJob.ShareURL && !string.IsNullOrEmpty(result))
                            {
                                TaskHelpers.PlayNotificationSoundAsync(NotificationSound.TaskCompleted, info.TaskSettings);

                                if (!string.IsNullOrEmpty(info.TaskSettings.AdvancedSettings.BalloonTipContentFormat))
                                {
                                    result = new UploadInfoParser().Parse(info, info.TaskSettings.AdvancedSettings.BalloonTipContentFormat);
                                }

                                if (info.TaskSettings.GeneralSettings.ShowToastNotificationAfterTaskCompleted && !string.IsNullOrEmpty(result) &&
                                    (!info.TaskSettings.GeneralSettings.DisableNotificationsOnFullscreen || !CaptureHelpers.IsActiveWindowFullscreen()))
                                {
                                    task.KeepImage = true;

                                    NotificationFormConfig toastConfig = new NotificationFormConfig()
                                    {
                                        Duration = (int)(info.TaskSettings.GeneralSettings.ToastWindowDuration * 1000),
                                        FadeDuration = (int)(info.TaskSettings.GeneralSettings.ToastWindowFadeDuration * 1000),
                                        Placement = info.TaskSettings.GeneralSettings.ToastWindowPlacement,
                                        Size = info.TaskSettings.GeneralSettings.ToastWindowSize,
                                        LeftClickAction = info.TaskSettings.GeneralSettings.ToastWindowLeftClickAction,
                                        RightClickAction = info.TaskSettings.GeneralSettings.ToastWindowRightClickAction,
                                        MiddleClickAction = info.TaskSettings.GeneralSettings.ToastWindowMiddleClickAction,
                                        FilePath = info.FilePath,
                                        Image = task.Image,
                                        Title = "ShareX - " + Resources.TaskManager_task_UploadCompleted_ShareX___Task_completed,
                                        Text = result,
                                        URL = result
                                    };

                                    NotificationForm.Show(toastConfig);

                                    if (info.TaskSettings.AfterUploadJob.HasFlag(AfterUploadTasks.ShowAfterUploadWindow) && info.IsUploadJob)
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

                            if (Program.Settings.AutoSelectLastCompletedTask)
                            {
                                TaskListView.ListViewControl.SelectSingle(lvi);
                            }
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

                    if (Program.Settings.SaveSettingsAfterTaskCompleted && !IsBusy)
                    {
                        SettingManager.SaveAllSettingsAsync();
                    }
                }
            }
        }

        private static void Task_UploadersConfigWindowRequested(IUploaderService uploaderService)
        {
            TaskHelpers.OpenUploadersConfigWindow(uploaderService);
        }

        public static void UpdateProgressUI()
        {
            bool isTasksWorking = false;
            double averageProgress = 0;

            IEnumerable<WorkerTask> workingTasks = Tasks.Where(x => x != null && x.Status == TaskStatus.Working && x.Info != null);

            if (workingTasks.Count() > 0)
            {
                isTasksWorking = true;

                workingTasks = workingTasks.Where(x => x.Info.Progress != null);

                if (workingTasks.Count() > 0)
                {
                    averageProgress = workingTasks.Average(x => x.Info.Progress.Percentage);
                }
            }

            if (isTasksWorking)
            {
                Program.MainForm.Text = string.Format("{0} - {1:0.0}%", Program.Title, averageProgress);
                UpdateTrayIcon((int)averageProgress);
                TaskbarManager.SetProgressValue(Program.MainForm, (int)averageProgress);
            }
            else
            {
                Program.MainForm.Text = Program.Title;
                UpdateTrayIcon();
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
                        icon = Helpers.GetProgressIcon(progress);
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

        public static void AddTestTasks(int count)
        {
            for (int i = 0; i < count; i++)
            {
                WorkerTask task = WorkerTask.CreateHistoryTask(new RecentTask()
                {
                    FilePath = @"..\..\..\ShareX.HelpersLib\Resources\ShareX_Logo.png"
                });

                Start(task);
            }
        }

        public static async Task TestTrayIcon()
        {
            for (int i = 0; i <= 100; i++)
            {
                UpdateTrayIcon(i);

                await Task.Delay(50);
            }
        }

        private static void AppendHistoryItemAsync(HistoryItem historyItem)
        {
            Task.Run(() =>
            {
                HistoryManager history = new HistoryManagerJSON(Program.HistoryFilePath)
                {
                    BackupFolder = SettingManager.BackupFolder,
                    CreateBackup = false,
                    CreateWeeklyBackup = true
                };

                history.AppendHistoryItem(historyItem);
            });
        }

        public static void AddRecentTasksToMainWindow()
        {
            if (TaskListView.ListViewControl.Items.Count == 0)
            {
                foreach (RecentTask recentTask in RecentManager.Tasks)
                {
                    WorkerTask task = WorkerTask.CreateHistoryTask(recentTask);
                    Start(task);
                }
            }
        }
    }
}