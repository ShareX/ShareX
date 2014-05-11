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
using ShareX.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using UploadersLib;

namespace ShareX
{
    public partial class UploadTestForm : Form
    {
        public enum UploadStatus
        {
            Uploading,
            Uploaded
        }

        private bool isTesting;

        public bool Testing
        {
            get
            {
                return isTesting;
            }
            set
            {
                isTesting = value;
                btnTestAll.Enabled = !value;
                btnTestSelected.Enabled = !value;
                testSelectedUploadersToolStripMenuItem.Enabled = !value;
            }
        }

        public Image TestImage { get; set; }
        public string TestText { get; set; }
        public string TestURL { get; set; }

        public UploadTestForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            if (TestImage == null)
            {
                TestImage = Resources.Test;
            }

            if (string.IsNullOrEmpty(TestText))
            {
                TestText = Program.ApplicationName + " text upload test";
            }

            if (string.IsNullOrEmpty(TestURL))
            {
                TestURL = Links.URL_WEBSITE;
            }

            ListViewItem lvi;

            ListViewGroup imageUploadersGroup = new ListViewGroup("Image Uploaders", HorizontalAlignment.Left);
            ListViewGroup textUploadersGroup = new ListViewGroup("Text Uploaders", HorizontalAlignment.Left);
            ListViewGroup fileUploadersGroup = new ListViewGroup("File Uploaders", HorizontalAlignment.Left);
            ListViewGroup urlShortenersGroup = new ListViewGroup("URL Shorteners", HorizontalAlignment.Left);
            lvUploaders.Groups.AddRange(new[] { imageUploadersGroup, textUploadersGroup, fileUploadersGroup, urlShortenersGroup });

            foreach (ImageDestination uploader in Enum.GetValues(typeof(ImageDestination)))
            {
                switch (uploader)
                {
                    case ImageDestination.Twitsnaps: // Not possible to upload without post Twitter
                    case ImageDestination.FileUploader: // We are going to test this in File Uploader tests
                        continue;
                }

                lvi = new ListViewItem(uploader.GetDescription());

                TaskSettings taskSettings = TaskSettings.GetDefaultTaskSettings();
                taskSettings.UseDefaultImageSettings = false;
                taskSettings.SafeImageSettings.FileExistAction = FileExistAction.Cancel;
                taskSettings.UseDefaultDestinations = false;
                taskSettings.SafeDestinations.ImageDestination = uploader;
                UploadTask task = UploadTask.CreateImageUploaderTask((Image)TestImage.Clone(), taskSettings);

                lvi.Tag = task;
                lvi.Group = imageUploadersGroup;
                lvUploaders.Items.Add(lvi);
            }

            foreach (TextDestination uploader in Enum.GetValues(typeof(TextDestination)))
            {
                switch (uploader)
                {
                    case TextDestination.FileUploader:
                        continue;
                }

                lvi = new ListViewItem(uploader.GetDescription());

                TaskSettings taskSettings = TaskSettings.GetDefaultTaskSettings();
                taskSettings.UseDefaultImageSettings = false;
                taskSettings.SafeImageSettings.FileExistAction = FileExistAction.Cancel;
                taskSettings.UseDefaultDestinations = false;
                taskSettings.SafeDestinations.TextDestination = uploader;
                UploadTask task = UploadTask.CreateTextUploaderTask(TestText, taskSettings);

                lvi.Tag = task;
                lvi.Group = textUploadersGroup;
                lvUploaders.Items.Add(lvi);
            }

            foreach (FileDestination uploader in Enum.GetValues(typeof(FileDestination)))
            {
                switch (uploader)
                {
                    case FileDestination.CustomFileUploader:
                    case FileDestination.SharedFolder:
                    case FileDestination.Email:
                        continue;
                }

                lvi = new ListViewItem(uploader.GetDescription());

                TaskSettings taskSettings = TaskSettings.GetDefaultTaskSettings();
                taskSettings.UseDefaultImageSettings = false;
                taskSettings.SafeImageSettings.FileExistAction = FileExistAction.Cancel;
                taskSettings.UseDefaultDestinations = false;
                taskSettings.SafeDestinations.ImageDestination = ImageDestination.FileUploader;
                taskSettings.SafeDestinations.ImageFileDestination = uploader;
                UploadTask task = UploadTask.CreateImageUploaderTask((Image)TestImage.Clone(), taskSettings);

                lvi.Tag = task;
                lvi.Group = fileUploadersGroup;
                lvUploaders.Items.Add(lvi);
            }

            foreach (UrlShortenerType uploader in Enum.GetValues(typeof(UrlShortenerType)))
            {
                lvi = new ListViewItem(uploader.GetDescription());

                TaskSettings taskSettings = TaskSettings.GetDefaultTaskSettings();
                taskSettings.UseDefaultImageSettings = false;
                taskSettings.SafeImageSettings.FileExistAction = FileExistAction.Cancel;
                taskSettings.UseDefaultDestinations = false;
                taskSettings.SafeDestinations.URLShortenerDestination = uploader;
                UploadTask task = UploadTask.CreateURLShortenerTask(TestURL, taskSettings);

                lvi.Tag = task;
                lvi.Group = urlShortenersGroup;
                lvUploaders.Items.Add(lvi);
            }

            PrepareListItems();
        }

        private void PrepareListItems()
        {
            for (int i = 0; i < lvUploaders.Items.Count; i++)
            {
                ListViewItem lvi = lvUploaders.Items[i];

                while (lvi.SubItems.Count < 3)
                {
                    lvi.SubItems.Add(string.Empty);
                }

                lvi.SubItems[1].Text = "Waiting";
                lvi.BackColor = Color.LightYellow;
            }
        }

        private void btnTestAll_Click(object sender, EventArgs e)
        {
            UploadTask[] uploaders = lvUploaders.Items.Cast<ListViewItem>().Select(x => x.Tag as UploadTask).ToArray();
            StartTest(uploaders);
        }

        private void btnTestSelected_Click(object sender, EventArgs e)
        {
            UploadTask[] uploaders = lvUploaders.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as UploadTask).ToArray();
            StartTest(uploaders);
        }

        private void ConsoleWriteLine(string value)
        {
            this.InvokeSafe(() => txtConsole.AppendText(value + "\r\n"));
        }

        private ListViewItem FindListViewItem(UploadTask task)
        {
            foreach (ListViewItem lvi in lvUploaders.Items)
            {
                UploadTask x = lvi.Tag as UploadTask;
                if (x != null && x == task) return lvi;
            }

            return null;
        }

        public void StartTest(UploadTask[] uploaders)
        {
            Testing = true;

            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += (x, x2) =>
            {
                TaskManager.UpdateProgressUI();
                Testing = false;
            };
            bw.RunWorkerAsync(uploaders);
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = (BackgroundWorker)sender;
            UploadTask[] uploaders = (UploadTask[])e.Argument;

            foreach (UploadTask task in uploaders)
            {
                if (IsDisposed || !isTesting || task == null)
                {
                    break;
                }

                bw.ReportProgress((int)UploadStatus.Uploading, task);

                try
                {
                    task.StartSync();
                }
                catch (Exception ex)
                {
                    ConsoleWriteLine(ex.ToString());
                }
                finally
                {
                    bw.ReportProgress((int)UploadStatus.Uploaded, task);
                }
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!IsDisposed)
            {
                UploadTask task = e.UserState as UploadTask;

                if (task != null)
                {
                    ListViewItem lvi = FindListViewItem(task);

                    if (lvi != null)
                    {
                        switch ((UploadStatus)e.ProgressPercentage)
                        {
                            case UploadStatus.Uploading:
                                lvi.BackColor = Color.Gold;
                                lvi.SubItems[1].Text = "Uploading...";
                                lvi.SubItems[2].Text = string.Empty;
                                break;
                            case UploadStatus.Uploaded:
                                TaskInfo info = task.Info;

                                if (info != null && info.Result != null)
                                {
                                    if (!info.Result.IsError && !string.IsNullOrEmpty(info.Result.ToString()))
                                    {
                                        lvi.BackColor = Color.LightGreen;
                                        lvi.SubItems[1].Text = "Success: " + info.Result;
                                    }
                                    else
                                    {
                                        lvi.BackColor = Color.LightCoral;
                                        lvi.SubItems[1].Text = "Failed: " + info.Result.ErrorsToString();
                                        txtConsole.AppendText(info.Result.ErrorsToString());
                                    }
                                }

                                lvi.SubItems[2].Text = (int)info.UploadDuration.TotalMilliseconds + " ms";
                                break;
                        }
                    }
                }
            }
        }

        private void openURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvUploaders.SelectedItems.Count > 0)
            {
                UploadTask task = lvUploaders.SelectedItems[0].Tag as UploadTask;

                if (task != null && task.Info != null && task.Info.Result != null && !string.IsNullOrEmpty(task.Info.Result.ToString()))
                {
                    ThreadPool.QueueUserWorkItem(x => Process.Start(task.Info.Result.ToString()));
                }
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvUploaders.SelectedItems.Count > 0)
            {
                List<string> urls = new List<string>();

                foreach (ListViewItem lvi in lvUploaders.SelectedItems)
                {
                    UploadTask task = lvi.Tag as UploadTask;

                    if (task != null && task.Info != null && task.Info.Result != null && !string.IsNullOrEmpty(task.Info.Result.ToString()))
                    {
                        urls.Add(string.Format("{0}: {1}", lvi.Text, task.Info.Result));
                    }
                }

                if (urls.Count > 0)
                {
                    ClipboardHelpers.CopyText(string.Join("\r\n", urls.ToArray()));
                }
            }
        }

        private void TesterGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            isTesting = false;

            if (TestImage != null)
            {
                TestImage.Dispose();
            }
        }
    }
}