#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
using ScreenCaptureLib;
using ShareX.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using UploadersLib;

namespace ShareX
{
    public partial class MainForm : HotkeyForm
    {
        public bool IsReady { get; private set; }

        private bool forceClose;
        private UploadInfoManager uim;
        private ToolStripDropDownItem tsmiImageFileUploaders, tsmiTrayImageFileUploaders, tsmiTextFileUploaders, tsmiTrayTextFileUploaders;

        public MainForm()
        {
            InitControls();
            HandleCreated += MainForm_HandleCreated;
        }

        private void MainForm_HandleCreated(object sender, EventArgs e)
        {
            LoadSettings();
            InitHotkeys();

#if !DEBUG
            AutoCheckUpdate();
#endif

            IsReady = true;

            DebugHelper.WriteLine("Startup time: {0} ms", Program.StartTimer.ElapsedMilliseconds);

            UseCommandLineArgs(Environment.GetCommandLineArgs());
        }

        private void AfterShownJobs()
        {
            this.ShowActivate();
        }

        private void InitControls()
        {
            InitializeComponent();

            Text = Program.Title;
            Icon = ShareXResources.Icon;

            ((ToolStripDropDownMenu)tsddbWorkflows.DropDown).ShowImageMargin = ((ToolStripDropDownMenu)tsmiTrayWorkflows.DropDown).ShowImageMargin =
                ((ToolStripDropDownMenu)tsmiMonitor.DropDown).ShowImageMargin = ((ToolStripDropDownMenu)tsmiTrayMonitor.DropDown).ShowImageMargin =
                ((ToolStripDropDownMenu)tsmiShortenSelectedURL.DropDown).ShowImageMargin = ((ToolStripDropDownMenu)tsmiShareSelectedURL.DropDown).ShowImageMargin = false;

            AddMultiEnumItems<AfterCaptureTasks>(x => Program.DefaultTaskSettings.AfterCaptureJob = Program.DefaultTaskSettings.AfterCaptureJob.Swap(x),
                tsddbAfterCaptureTasks, tsmiTrayAfterCaptureTasks);
            AddMultiEnumItems<AfterUploadTasks>(x => Program.DefaultTaskSettings.AfterUploadJob = Program.DefaultTaskSettings.AfterUploadJob.Swap(x),
                tsddbAfterUploadTasks, tsmiTrayAfterUploadTasks);
            AddEnumItems<ImageDestination>(x => Program.DefaultTaskSettings.ImageDestination = x, tsmiImageUploaders, tsmiTrayImageUploaders);
            tsmiImageFileUploaders = (ToolStripDropDownItem)tsmiImageUploaders.DropDownItems[tsmiImageUploaders.DropDownItems.Count - 1];
            tsmiTrayImageFileUploaders = (ToolStripDropDownItem)tsmiTrayImageUploaders.DropDownItems[tsmiTrayImageUploaders.DropDownItems.Count - 1];
            AddEnumItems<FileDestination>(x =>
            {
                Program.DefaultTaskSettings.ImageFileDestination = x;
                tsmiImageFileUploaders.PerformClick();
                tsmiTrayImageFileUploaders.PerformClick();
            }, tsmiImageFileUploaders, tsmiTrayImageFileUploaders);
            AddEnumItems<TextDestination>(x => Program.DefaultTaskSettings.TextDestination = x, tsmiTextUploaders, tsmiTrayTextUploaders);
            tsmiTextFileUploaders = (ToolStripDropDownItem)tsmiTextUploaders.DropDownItems[tsmiTextUploaders.DropDownItems.Count - 1];
            tsmiTrayTextFileUploaders = (ToolStripDropDownItem)tsmiTrayTextUploaders.DropDownItems[tsmiTrayTextUploaders.DropDownItems.Count - 1];
            AddEnumItems<FileDestination>(x =>
            {
                Program.DefaultTaskSettings.TextFileDestination = x;
                tsmiTextFileUploaders.PerformClick();
                tsmiTrayTextFileUploaders.PerformClick();
            }, tsmiTextFileUploaders, tsmiTrayTextFileUploaders);
            AddEnumItems<FileDestination>(x => Program.DefaultTaskSettings.FileDestination = x, tsmiFileUploaders, tsmiTrayFileUploaders);
            AddEnumItems<UrlShortenerType>(x => Program.DefaultTaskSettings.URLShortenerDestination = x, tsmiURLShorteners, tsmiTrayURLShorteners);
            AddEnumItems<URLSharingServices>(x => Program.DefaultTaskSettings.SocialNetworkingServiceDestination = x, tsmiURLSharingServices, tsmiTrayURLSharingServices);

            foreach (UrlShortenerType urlShortener in Helpers.GetEnums<UrlShortenerType>())
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(urlShortener.GetDescription());
                tsmi.Click += (sender, e) => uim.ShortenURL(urlShortener);
                tsmiShortenSelectedURL.DropDownItems.Add(tsmi);
            }

            foreach (URLSharingServices socialNetworkingService in Helpers.GetEnums<URLSharingServices>())
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(socialNetworkingService.GetDescription());
                tsmi.Click += (sender, e) => uim.ShareURL(socialNetworkingService);
                tsmiShareSelectedURL.DropDownItems.Add(tsmi);
            }

            ImageList il = new ImageList();
            il.ColorDepth = ColorDepth.Depth32Bit;
            il.Images.Add(Resources.navigation_090_button);
            il.Images.Add(Resources.cross_button);
            il.Images.Add(Resources.tick_button);
            il.Images.Add(Resources.navigation_000_button);
            lvUploads.SmallImageList = il;

            TaskManager.ListViewControl = lvUploads;
            uim = new UploadInfoManager(lvUploads);

            ExportImportControl.UploadRequested += json => UploadManager.UploadText(json);
        }

        private void UpdateWorkflowsMenu()
        {
            tsddbWorkflows.DropDownItems.Clear();
            tsmiTrayWorkflows.DropDownItems.Clear();

            foreach (HotkeySettings hotkeySetting in Program.HotkeysConfig.Hotkeys)
            {
                if (hotkeySetting.TaskSettings.Job != HotkeyType.None && (!Program.Settings.WorkflowsOnlyShowEdited || !hotkeySetting.TaskSettings.IsUsingDefaultSettings))
                {
                    tsddbWorkflows.DropDownItems.Add(WorkflowMenuItem(hotkeySetting));
                    tsmiTrayWorkflows.DropDownItems.Add(WorkflowMenuItem(hotkeySetting));
                }
            }

            if (tsddbWorkflows.DropDownItems.Count > 0)
            {
                ToolStripSeparator tss = new ToolStripSeparator();
                tsddbWorkflows.DropDownItems.Add(tss);
            }

            ToolStripMenuItem tsmi = new ToolStripMenuItem("You can add workflows from hotkey settings...");
            tsmi.Click += tsbHotkeySettings_Click;
            tsddbWorkflows.DropDownItems.Add(tsmi);

            tsmiTrayWorkflows.Visible = tsmiTrayWorkflows.DropDownItems.Count > 0;
        }

        private ToolStripMenuItem WorkflowMenuItem(HotkeySettings hotkeySetting)
        {
            ToolStripMenuItem tsmi = new ToolStripMenuItem(hotkeySetting.TaskSettings.Description);
            if (hotkeySetting.HotkeyInfo.IsValidHotkey)
            {
                tsmi.ShortcutKeyDisplayString = "  " + hotkeySetting.HotkeyInfo.ToString();
            }
            if (!hotkeySetting.TaskSettings.IsUsingDefaultSettings)
            {
                tsmi.Font = new Font(tsmi.Font, FontStyle.Bold);
            }
            tsmi.Click += (sender, e) => HandleTask(hotkeySetting.TaskSettings);
            return tsmi;
        }

        private void UpdateDestinationStates()
        {
            if (Program.UploadersConfig != null)
            {
                EnableDisableToolStripMenuItems<ImageDestination>(tsmiImageUploaders, tsmiTrayImageUploaders);
                EnableDisableToolStripMenuItems<FileDestination>(tsmiImageFileUploaders, tsmiTrayImageFileUploaders);
                EnableDisableToolStripMenuItems<TextDestination>(tsmiTextUploaders, tsmiTrayTextUploaders);
                EnableDisableToolStripMenuItems<FileDestination>(tsmiTextFileUploaders, tsmiTrayTextFileUploaders);
                EnableDisableToolStripMenuItems<FileDestination>(tsmiFileUploaders, tsmiTrayFileUploaders);
                EnableDisableToolStripMenuItems<UrlShortenerType>(tsmiURLShorteners, tsmiTrayURLShorteners);
                EnableDisableToolStripMenuItems<URLSharingServices>(tsmiURLSharingServices, tsmiTrayURLSharingServices);
            }
        }

        private void AddEnumItems<T>(Action<T> selectedEnum, params ToolStripDropDownItem[] parents)
        {
            string[] enums = Helpers.GetEnumDescriptions<T>();

            foreach (ToolStripDropDownItem parent in parents)
            {
                for (int i = 0; i < enums.Length; i++)
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(enums[i]);

                    int index = i;

                    tsmi.Click += (sender, e) =>
                    {
                        foreach (ToolStripDropDownItem parent2 in parents)
                        {
                            for (int i2 = 0; i2 < enums.Length; i2++)
                            {
                                ToolStripMenuItem tsmi2 = (ToolStripMenuItem)parent2.DropDownItems[i2];
                                tsmi2.Checked = index == i2;
                            }
                        }

                        selectedEnum((T)Enum.ToObject(typeof(T), index));

                        UpdateUploaderMenuNames();
                    };

                    parent.DropDownItems.Add(tsmi);
                }
            }
        }

        private void SetEnumChecked(Enum value, params ToolStripDropDownItem[] parents)
        {
            int index = value.GetIndex();

            foreach (ToolStripDropDownItem parent in parents)
            {
                ((ToolStripMenuItem)parent.DropDownItems[index]).Checked = true;
            }
        }

        private void AddMultiEnumItems<T>(Action<T> selectedEnum, params ToolStripDropDownItem[] parents)
        {
            string[] enums = Enum.GetValues(typeof(T)).Cast<Enum>().Skip(1).Select(x => x.GetDescription()).ToArray();

            foreach (ToolStripDropDownItem parent in parents)
            {
                for (int i = 0; i < enums.Length; i++)
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(enums[i]);

                    int index = i;

                    tsmi.Click += (sender, e) =>
                    {
                        foreach (ToolStripDropDownItem parent2 in parents)
                        {
                            ToolStripMenuItem tsmi2 = (ToolStripMenuItem)parent2.DropDownItems[index];
                            tsmi2.Checked = !tsmi2.Checked;
                        }

                        selectedEnum((T)Enum.ToObject(typeof(T), 1 << index));

                        UpdateUploaderMenuNames();
                    };

                    parent.DropDownItems.Add(tsmi);
                }
            }
        }

        private void SetMultiEnumChecked(Enum value, params ToolStripDropDownItem[] parents)
        {
            for (int i = 0; i < parents[0].DropDownItems.Count; i++)
            {
                foreach (ToolStripDropDownItem parent in parents)
                {
                    ToolStripMenuItem tsmi = (ToolStripMenuItem)parent.DropDownItems[i];
                    tsmi.Checked = value.HasFlag(1 << i);
                }
            }
        }

        private void EnableDisableToolStripMenuItems<T>(params ToolStripDropDownItem[] parents)
        {
            foreach (ToolStripDropDownItem parent in parents)
            {
                for (int i = 0; i < parent.DropDownItems.Count; i++)
                {
                    parent.DropDownItems[i].Enabled = Program.UploadersConfig.IsActive<T>(i);
                }
            }
        }

        private void UpdateControls()
        {
            cmsUploadInfo.SuspendLayout();

            tsmiStopUpload.Visible = tsmiOpen.Visible = tsmiCopy.Visible = tsmiShowErrors.Visible = tsmiShowResponse.Visible = tsmiShowQRCode.Visible =
                tsmiUploadSelectedFile.Visible = tsmiShortenSelectedURL.Visible = tsmiShareSelectedURL.Visible = tsmiClearList.Visible = tssUploadInfo1.Visible = false;
            pbPreview.Reset();
            uim.RefreshSelectedItems();

            switch (Program.Settings.ImagePreview)
            {
                case ImagePreviewVisibility.Show:
                    scMain.Panel2Collapsed = false;
                    break;
                case ImagePreviewVisibility.Hide:
                    scMain.Panel2Collapsed = true;
                    break;
                case ImagePreviewVisibility.Automatic:
                    scMain.Panel2Collapsed = !uim.IsItemSelected || (!uim.SelectedItem.IsImageFile && !uim.SelectedItem.IsImageURL);
                    break;
            }

            if (uim.IsItemSelected)
            {
                if (GetCurrentTasks().Any(x => x.IsWorking))
                {
                    tsmiStopUpload.Visible = true;
                }
                else
                {
                    tsmiShowErrors.Visible = uim.SelectedItem.Info.Result.IsError;

                    // Open
                    tsmiOpen.Visible = true;

                    tsmiOpenURL.Enabled = uim.SelectedItem.IsURLExist;
                    tsmiOpenShortenedURL.Enabled = uim.SelectedItem.IsShortenedURLExist;
                    tsmiOpenThumbnailURL.Enabled = uim.SelectedItem.IsThumbnailURLExist;
                    tsmiOpenDeletionURL.Enabled = uim.SelectedItem.IsDeletionURLExist;

                    tsmiOpenFile.Enabled = uim.SelectedItem.IsFileExist;
                    tsmiOpenFolder.Enabled = uim.SelectedItem.IsFileExist;
                    tsmiOpenThumbnailFile.Enabled = uim.SelectedItem.IsThumbnailFileExist;

                    // Copy
                    tsmiCopy.Visible = true;

                    tsmiCopyURL.Enabled = uim.SelectedItems.Any(x => x.IsURLExist);
                    tsmiCopyShortenedURL.Enabled = uim.SelectedItems.Any(x => x.IsShortenedURLExist);
                    tsmiCopyThumbnailURL.Enabled = uim.SelectedItems.Any(x => x.IsThumbnailURLExist);
                    tsmiCopyDeletionURL.Enabled = uim.SelectedItems.Any(x => x.IsDeletionURLExist);

                    tsmiCopyFile.Enabled = uim.SelectedItem.IsFileExist;
                    tsmiCopyImage.Enabled = uim.SelectedItem.IsImageFile;
                    tsmiCopyText.Enabled = uim.SelectedItem.IsTextFile;
                    tsmiCopyThumbnailFile.Enabled = uim.SelectedItem.IsThumbnailFileExist;
                    tsmiCopyThumbnailImage.Enabled = uim.SelectedItem.IsThumbnailFileExist;

                    tsmiCopyHTMLLink.Enabled = uim.SelectedItems.Any(x => x.IsURLExist);
                    tsmiCopyHTMLImage.Enabled = uim.SelectedItems.Any(x => x.IsImageURL);
                    tsmiCopyHTMLLinkedImage.Enabled = uim.SelectedItems.Any(x => x.IsImageURL && x.IsThumbnailURLExist);

                    tsmiCopyForumLink.Enabled = uim.SelectedItems.Any(x => x.IsURLExist);
                    tsmiCopyForumImage.Enabled = uim.SelectedItems.Any(x => x.IsImageURL && x.IsURLExist);
                    tsmiCopyForumLinkedImage.Enabled = uim.SelectedItems.Any(x => x.IsImageURL && x.IsThumbnailURLExist);

                    tsmiCopyFilePath.Enabled = uim.SelectedItems.Any(x => x.IsFilePathValid);
                    tsmiCopyFileName.Enabled = uim.SelectedItems.Any(x => x.IsFilePathValid);
                    tsmiCopyFileNameWithExtension.Enabled = uim.SelectedItems.Any(x => x.IsFilePathValid);
                    tsmiCopyFolder.Enabled = uim.SelectedItems.Any(x => x.IsFilePathValid);

                    CleanCustomClipboardFormats();

                    if (Program.Settings.ClipboardContentFormats != null && Program.Settings.ClipboardContentFormats.Count > 0)
                    {
                        tssCopy5.Visible = true;

                        foreach (ClipboardFormat cf in Program.Settings.ClipboardContentFormats)
                        {
                            ToolStripMenuItem tsmiClipboardFormat = new ToolStripMenuItem(cf.Description);
                            tsmiClipboardFormat.Tag = cf;
                            tsmiClipboardFormat.Click += tsmiClipboardFormat_Click;
                            tsmiCopy.DropDownItems.Add(tsmiClipboardFormat);
                        }
                    }

                    tsmiUploadSelectedFile.Visible = uim.SelectedItem.IsFileExist;
                    tsmiShortenSelectedURL.Visible = uim.SelectedItem.IsURLExist;
                    tsmiShareSelectedURL.Visible = uim.SelectedItem.IsURLExist;
                    tsmiShowQRCode.Visible = uim.SelectedItem.IsURLExist;
                    tsmiShowResponse.Visible = !string.IsNullOrEmpty(uim.SelectedItem.Info.Result.Response);
                }

                if (!scMain.Panel2Collapsed)
                {
                    if (uim.SelectedItem.IsImageFile)
                    {
                        pbPreview.LoadImageFromFileAsync(uim.SelectedItem.Info.FilePath);
                    }
                    else if (uim.SelectedItem.IsImageURL)
                    {
                        pbPreview.LoadImageFromURLAsync(uim.SelectedItem.Info.Result.URL);
                    }
                }
            }

            tsmiClearList.Visible = tssUploadInfo1.Visible = lvUploads.Items.Count > 0;

            cmsUploadInfo.ResumeLayout();
            Refresh();
        }

        private void CleanCustomClipboardFormats()
        {
            tssCopy5.Visible = false;

            int tssCopy5Index = tsmiCopy.DropDownItems.IndexOf(tssCopy5);

            while (tssCopy5Index < tsmiCopy.DropDownItems.Count - 1)
            {
                using (ToolStripItem tsi = tsmiCopy.DropDownItems[tsmiCopy.DropDownItems.Count - 1])
                {
                    tsmiCopy.DropDownItems.Remove(tsi);
                }
            }
        }

        private void tsmiClipboardFormat_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmiClipboardFormat = sender as ToolStripMenuItem;
            ClipboardFormat cf = tsmiClipboardFormat.Tag as ClipboardFormat;
            uim.CopyCustomFormat(cf.Format);
        }

        private void LoadSettings()
        {
            niTray.Icon = ShareXResources.Icon;
            niTray.Visible = Program.Settings.ShowTray;

            if (Program.Settings.RememberMainFormSize && !Program.Settings.MainFormSize.IsEmpty)
            {
                StartPosition = FormStartPosition.Manual;
                Size = Program.Settings.MainFormSize;
                Screen currentScreen = Screen.FromPoint(Cursor.Position);
                Location = new Point(currentScreen.Bounds.Width / 2 - Size.Width / 2, currentScreen.Bounds.Height / 2 - Size.Height / 2);
            }

            switch (Program.Settings.ImagePreview)
            {
                case ImagePreviewVisibility.Show:
                    tsmiImagePreviewShow.Check();
                    break;
                case ImagePreviewVisibility.Hide:
                    tsmiImagePreviewHide.Check();
                    break;
                case ImagePreviewVisibility.Automatic:
                    tsmiImagePreviewAutomatic.Check();
                    break;
            }

            UpdateMainFormSettings();
            UpdateMenu();
            UpdateUploaderMenuNames();
            RegisterMenuClosing();

            AfterSettingsJobs();

            if (Program.Settings.PreviewSplitterDistance > 0)
            {
                scMain.SplitterDistance = Program.Settings.PreviewSplitterDistance;
            }

            UpdateControls();

            TaskbarManager.Enabled = Program.Settings.TaskbarProgressEnabled;
        }

        private void RegisterMenuClosing()
        {
            foreach (ToolStripDropDownItem dropDownItem in new ToolStripDropDownItem[]
            {
                tsddbAfterCaptureTasks, tsddbAfterUploadTasks, tsmiImageUploaders, tsmiImageFileUploaders, tsmiTextUploaders, tsmiTextFileUploaders, tsmiFileUploaders,
                tsmiURLShorteners, tsmiURLSharingServices, tsmiTrayAfterCaptureTasks, tsmiTrayAfterUploadTasks, tsmiTrayImageUploaders, tsmiTrayImageFileUploaders,
                tsmiTrayTextUploaders, tsmiTrayTextFileUploaders, tsmiTrayFileUploaders, tsmiTrayURLShorteners, tsmiTrayURLSharingServices
            })
            {
                dropDownItem.DropDown.Closing += (sender, e) => e.Cancel = (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked);
            }
        }

        private void AfterSettingsJobs()
        {
            ProxyInfo.Current = Program.Settings.ProxySettings;
            ClipboardHelpers.UseAlternativeCopyImage = Program.Settings.UseAlternativeClipboardCopyImage;
        }

        public void UpdateMainFormSettings()
        {
            SetMultiEnumChecked(Program.DefaultTaskSettings.AfterCaptureJob, tsddbAfterCaptureTasks, tsmiTrayAfterCaptureTasks);
            SetMultiEnumChecked(Program.DefaultTaskSettings.AfterUploadJob, tsddbAfterUploadTasks, tsmiTrayAfterUploadTasks);
            SetEnumChecked(Program.DefaultTaskSettings.ImageDestination, tsmiImageUploaders, tsmiTrayImageUploaders);
            SetEnumChecked(Program.DefaultTaskSettings.ImageFileDestination, tsmiImageFileUploaders, tsmiTrayImageFileUploaders);
            SetEnumChecked(Program.DefaultTaskSettings.TextDestination, tsmiTextUploaders, tsmiTrayTextUploaders);
            SetEnumChecked(Program.DefaultTaskSettings.TextFileDestination, tsmiTextFileUploaders, tsmiTrayTextFileUploaders);
            SetEnumChecked(Program.DefaultTaskSettings.FileDestination, tsmiFileUploaders, tsmiTrayFileUploaders);
            SetEnumChecked(Program.DefaultTaskSettings.URLShortenerDestination, tsmiURLShorteners, tsmiTrayURLShorteners);
            SetEnumChecked(Program.DefaultTaskSettings.SocialNetworkingServiceDestination, tsmiURLSharingServices, tsmiTrayURLSharingServices);
        }

        private void UpdateUploaderMenuNames()
        {
            string imageUploader = Program.DefaultTaskSettings.ImageDestination == ImageDestination.FileUploader ?
                Program.DefaultTaskSettings.ImageFileDestination.GetDescription() : Program.DefaultTaskSettings.ImageDestination.GetDescription();
            tsmiImageUploaders.Text = tsmiTrayImageUploaders.Text = "Image uploader: " + imageUploader;

            string textUploader = Program.DefaultTaskSettings.TextDestination == TextDestination.FileUploader ?
                Program.DefaultTaskSettings.TextFileDestination.GetDescription() : Program.DefaultTaskSettings.TextDestination.GetDescription();
            tsmiTextUploaders.Text = tsmiTrayTextUploaders.Text = "Text uploader: " + textUploader;

            tsmiFileUploaders.Text = tsmiTrayFileUploaders.Text = "File uploader: " + Program.DefaultTaskSettings.FileDestination.GetDescription();

            tsmiURLShorteners.Text = tsmiTrayURLShorteners.Text = "URL shortener: " + Program.DefaultTaskSettings.URLShortenerDestination.GetDescription();

            tsmiURLSharingServices.Text = tsmiTrayURLSharingServices.Text = "Share URL via: " + Program.DefaultTaskSettings.SocialNetworkingServiceDestination.GetDescription();
        }

        private void AutoCheckUpdate()
        {
            if (Program.Settings.AutoCheckUpdate)
            {
                Thread updateThread = new Thread(CheckUpdate);
                updateThread.IsBackground = true;
                updateThread.Start();
            }
        }

        private void CheckUpdate()
        {
            UpdateChecker updateChecker = TaskHelpers.CheckUpdate();

            if (updateChecker.UpdateInfo != null && updateChecker.UpdateInfo.Status == UpdateStatus.UpdateAvailable &&
                MessageBox.Show("A newer version of ShareX is available.\r\nWould you like to download and install it?", string.Format("{0} {1} is available", Application.ProductName, updateChecker.UpdateInfo.LatestVersion.ToString()),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                using (DownloaderForm updaterForm = new DownloaderForm(updateChecker))
                {
                    updaterForm.ShowDialog();

                    if (updaterForm.Status == DownloaderFormStatus.InstallStarted)
                    {
                        Application.Exit();
                    }
                }
            }
        }

        private void ForceClose()
        {
            forceClose = true;
            Close();
        }

        public void UseCommandLineArgs(string[] args)
        {
            if (args != null && args.Length > 1)
            {
                for (int i = 1; i < args.Length; i++)
                {
                    if (args[i].Equals("-clipboardupload", StringComparison.InvariantCultureIgnoreCase))
                    {
                        UploadManager.ClipboardUpload();
                    }
                    else if (args[i].Equals("-autocapture", StringComparison.InvariantCultureIgnoreCase))
                    {
                        TaskHelpers.StartAutoCapture();
                    }
                    else if (args[i][0] != '-')
                    {
                        UploadManager.UploadFile(args[i]);
                    }
                }
            }
        }

        private UploadTask[] GetCurrentTasks()
        {
            if (lvUploads.SelectedItems.Count > 0)
            {
                return lvUploads.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as UploadTask).Where(x => x != null).ToArray();
            }

            return null;
        }

        private TaskInfo GetCurrentUploadInfo()
        {
            TaskInfo info = null;
            UploadTask[] tasks = GetCurrentTasks();

            if (tasks != null && tasks.Length > 0)
            {
                info = tasks[0].Info;
            }

            return info;
        }

        private void RemoveSelectedItems()
        {
            lvUploads.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as UploadTask).Where(x => x != null && !x.IsWorking).ForEach(TaskManager.Remove);
        }

        private void RemoveAllItems()
        {
            lvUploads.Items.Cast<ListViewItem>().Select(x => x.Tag as UploadTask).Where(x => x != null && !x.IsWorking).ForEach(TaskManager.Remove);
        }

        private void UpdateMenu()
        {
            if (Program.Settings.ShowMenu)
            {
                tsmiHideMenu.Text = "Hide menu";
            }
            else
            {
                tsmiHideMenu.Text = "Show menu";
            }

            tsMain.Visible = lblSplitter.Visible = Program.Settings.ShowMenu;
            Refresh();
        }

        #region Form events

        protected override void SetVisibleCore(bool value)
        {
            if (value && !IsHandleCreated && (Program.IsSilentRun || Program.Settings.SilentRun) && Program.Settings.ShowTray)
            {
                CreateHandle();
                value = false;
            }

            base.SetVisibleCore(value);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            AfterShownJobs();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            Refresh();

            if (WindowState == FormWindowState.Normal)
            {
                Program.Settings.MainFormSize = Size;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && Program.Settings.ShowTray && !forceClose)
            {
                e.Cancel = true;
                Hide();
                Program.Settings.SaveAsync(Program.ApplicationConfigFilePath);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            TaskManager.StopAllTasks();
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) ||
                e.Data.GetDataPresent(DataFormats.Bitmap, false) ||
                e.Data.GetDataPresent(DataFormats.Text, false))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            UploadManager.DragDropUpload(e.Data);
        }

        private void tsbFileUpload_Click(object sender, EventArgs e)
        {
            UploadManager.UploadFile();
        }

        private void tsbClipboardUpload_Click(object sender, EventArgs e)
        {
            UploadManager.ClipboardUploadMainWindow();
        }

        private void tsmiUploadURL_Click(object sender, EventArgs e)
        {
            UploadManager.UploadURL();
        }

        private void tsbDragDropUpload_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenDropWindow();
        }

        private void tsddbDestinations_DropDownOpened(object sender, EventArgs e)
        {
            UpdateDestinationStates();
        }

        private void tsmiShowDebugLog_Click(object sender, EventArgs e)
        {
            new DebugForm(DebugHelper.Logger).Show();
        }

        private void tsmiTestImageUpload_Click(object sender, EventArgs e)
        {
            UploadManager.RunImageTask(Resources.Test);
        }

        private void tsmiTestTextUpload_Click(object sender, EventArgs e)
        {
            UploadManager.UploadText("Text upload test");
        }

        private void tsmiTestFileUpload_Click(object sender, EventArgs e)
        {
            UploadManager.UploadImage(Resources.Test, ImageDestination.FileUploader);
        }

        private void tsmiTestURLShortener_Click(object sender, EventArgs e)
        {
            UploadManager.ShortenURL(Links.URL_WEBSITE);
        }

        private void tsmiTestUploaders_Click(object sender, EventArgs e)
        {
            using (UploadTestForm form = new UploadTestForm())
            {
                form.ShowDialog();
            }
        }

        private void tsmiScreenRecorder_Click(object sender, EventArgs e)
        {
            TaskHelpers.DoScreenRecorder();
        }

        private void tsmiAutoCapture_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenAutoCapture();
        }

        private void tsbApplicationSettings_Click(object sender, EventArgs e)
        {
            using (ApplicationSettingsForm settingsForm = new ApplicationSettingsForm())
            {
                settingsForm.ShowDialog();
            }

            AfterSettingsJobs();
            UpdateWorkflowsMenu();
            Program.Settings.SaveAsync(Program.ApplicationConfigFilePath);

            Program.ConfigureUploadersConfigWatcher();
        }

        private void tsbTaskSettings_Click(object sender, EventArgs e)
        {
            using (TaskSettingsForm taskSettingsForm = new TaskSettingsForm(Program.DefaultTaskSettings, true))
            {
                taskSettingsForm.ShowDialog();
            }

            Program.Settings.SaveAsync(Program.ApplicationConfigFilePath);
        }

        private void tsbHotkeySettings_Click(object sender, EventArgs e)
        {
            if (Program.HotkeysConfig == null)
            {
                Program.HotkeySettingsResetEvent.WaitOne();
            }

            using (HotkeySettingsForm hotkeySettingsForm = new HotkeySettingsForm())
            {
                hotkeySettingsForm.ShowDialog();
            }

            UpdateWorkflowsMenu();
            Program.HotkeysConfig.SaveAsync(Program.HotkeysConfigFilePath);
        }

        private void tsbDestinationSettings_Click(object sender, EventArgs e)
        {
            if (Program.UploadersConfig == null)
            {
                Program.UploaderSettingsResetEvent.WaitOne();
            }

            using (UploadersConfigForm uploadersConfigForm = new UploadersConfigForm(Program.UploadersConfig))
            {
                uploadersConfigForm.ShowDialog();
            }

            Program.UploadersConfigSaveAsync();
        }

        private void tsmiCursorHelper_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenScreenColorPicker();
        }

        private void tsmiRuler_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenRuler();
        }

        private void tsmiFTPClient_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenFTPClient();
        }

        private void tsmiHashCheck_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenHashCheck();
        }

        private void tsmiIndexFolder_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenIndexFolder();
        }

        private void tsmiImageEditor_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenImageEditor();
        }

        private void tsmiImageEffects_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenImageEffects();
        }

        private void tsmiMonitorTest_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenMonitorTest();
        }

        private void tsmiDNSChanger_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenDNSChanger();
        }

        private void tsmiQRCode_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenQRCode();
        }

        private void tsmiTweetMessage_Click(object sender, EventArgs e)
        {
            TaskHelpers.TweetMessage();
        }

        private void tsbScreenshotsFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Program.ScreenshotsFolder))
            {
                Helpers.OpenFolder(Program.ScreenshotsFolder);
            }
            else
            {
                Helpers.OpenFolder(Program.ScreenshotsParentFolder);
            }
        }

        private void tsbHistory_Click(object sender, EventArgs e)
        {
            HistoryForm historyForm = new HistoryForm(Program.HistoryFilePath);
            Program.Settings.HistoryWindowState.AutoHandleFormState(historyForm);
            historyForm.Text = "ShareX - History: " + Program.HistoryFilePath;
            historyForm.Show();
        }

        private void tsbImageHistory_Click(object sender, EventArgs e)
        {
            ImageHistoryForm imageHistoryForm = new ImageHistoryForm(Program.HistoryFilePath, Program.Settings.ImageHistoryViewMode,
                Program.Settings.ImageHistoryThumbnailSize, Program.Settings.ImageHistoryMaxItemCount);
            Program.Settings.ImageHistoryWindowState.AutoHandleFormState(imageHistoryForm);
            imageHistoryForm.Text = "ShareX - Image history: " + Program.HistoryFilePath;
            imageHistoryForm.FormClosed += imageHistoryForm_FormClosed;
            imageHistoryForm.Show();
        }

        private void imageHistoryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ImageHistoryForm imageHistoryForm = sender as ImageHistoryForm;
            Program.Settings.ImageHistoryViewMode = imageHistoryForm.ViewMode;
            Program.Settings.ImageHistoryThumbnailSize = imageHistoryForm.ThumbnailSize;
            Program.Settings.ImageHistoryMaxItemCount = imageHistoryForm.MaxItemCount;
        }

        private void tsbAbout_Click(object sender, EventArgs e)
        {
            using (AboutForm aboutForm = new AboutForm())
            {
                aboutForm.ShowDialog();
            }
        }

        private void tsbDonate_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.URL_DONATE);
        }

        private void lblDragAndDropTip_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UpdateControls();
                cmsUploadInfo.Show(lblDragAndDropTip, e.X + 1, e.Y + 1);
            }
        }

        private void lvUploads_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void lvUploads_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UpdateControls();
                cmsUploadInfo.Show(lvUploads, e.X + 1, e.Y + 1);
            }
        }

        private void lvUploads_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                uim.TryOpen();
            }
        }

        private void scMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Program.Settings.PreviewSplitterDistance = scMain.SplitterDistance;
        }

        private void lvUploads_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                default:
                    return;
                case Keys.Enter:
                    uim.TryOpen();
                    break;
                case Keys.Control | Keys.Enter:
                    uim.OpenFile();
                    break;
                case Keys.Control | Keys.X:
                    uim.CopyURL();
                    RemoveSelectedItems();
                    break;
                case Keys.Control | Keys.C:
                    uim.CopyURL();
                    break;
                case Keys.Control | Keys.V:
                    UploadManager.ClipboardUploadMainWindow();
                    break;
                case Keys.Delete:
                    RemoveSelectedItems();
                    break;
            }

            e.Handled = true;
        }

        #region Tray events

        private void niTray_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Middle:
                    CaptureScreenshot(CaptureType.Rectangle, null, false);
                    break;
            }
        }

        private void niTray_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.ShowActivate();
            }
        }

        private void niTray_BalloonTipClicked(object sender, EventArgs e)
        {
            string url = niTray.Tag as string;

            if (!string.IsNullOrEmpty(url))
            {
                URLHelpers.OpenURL(url);
            }
        }

        private void tsmiTrayShow_Click(object sender, EventArgs e)
        {
            this.ShowActivate();
        }

        private void tsmiTrayExit_Click(object sender, EventArgs e)
        {
            ForceClose();
        }

        #endregion Tray events

        #region UploadInfoMenu events

        private void tsmiStopUpload_Click(object sender, EventArgs e)
        {
            if (lvUploads.SelectedItems.Count > 0)
            {
                foreach (UploadTask task in GetCurrentTasks())
                {
                    task.Stop();
                }
            }
        }

        private void tsmiOpenURL_Click(object sender, EventArgs e)
        {
            uim.OpenURL();
        }

        private void tsmiOpenShortenedURL_Click(object sender, EventArgs e)
        {
            uim.OpenShortenedURL();
        }

        private void tsmiOpenThumbnailURL_Click(object sender, EventArgs e)
        {
            uim.OpenThumbnailURL();
        }

        private void tsmiOpenDeletionURL_Click(object sender, EventArgs e)
        {
            uim.OpenDeletionURL();
        }

        private void tsmiOpenFile_Click(object sender, EventArgs e)
        {
            uim.OpenFile();
        }

        private void tsmiOpenThumbnailFile_Click(object sender, EventArgs e)
        {
            uim.OpenThumbnailFile();
        }

        private void tsmiOpenFolder_Click(object sender, EventArgs e)
        {
            uim.OpenFolder();
        }

        private void tsmiCopyURL_Click(object sender, EventArgs e)
        {
            uim.CopyURL();
        }

        private void tsmiCopyShortenedURL_Click(object sender, EventArgs e)
        {
            uim.CopyShortenedURL();
        }

        private void tsmiCopyThumbnailURL_Click(object sender, EventArgs e)
        {
            uim.CopyThumbnailURL();
        }

        private void tsmiCopyDeletionURL_Click(object sender, EventArgs e)
        {
            uim.CopyDeletionURL();
        }

        private void tsmiCopyFile_Click(object sender, EventArgs e)
        {
            uim.CopyFile();
        }

        private void tsmiCopyImage_Click(object sender, EventArgs e)
        {
            uim.CopyImage();
        }

        private void tsmiCopyText_Click(object sender, EventArgs e)
        {
            uim.CopyText();
        }

        private void tsmiCopyThumbnailFile_Click(object sender, EventArgs e)
        {
            uim.CopyThumbnailFile();
        }

        private void tsmiCopyThumbnailImage_Click(object sender, EventArgs e)
        {
            uim.CopyThumbnailImage();
        }

        private void tsmiCopyHTMLLink_Click(object sender, EventArgs e)
        {
            uim.CopyHTMLLink();
        }

        private void tsmiCopyHTMLImage_Click(object sender, EventArgs e)
        {
            uim.CopyHTMLImage();
        }

        private void tsmiCopyHTMLLinkedImage_Click(object sender, EventArgs e)
        {
            uim.CopyHTMLLinkedImage();
        }

        private void tsmiCopyForumLink_Click(object sender, EventArgs e)
        {
            uim.CopyForumLink();
        }

        private void tsmiCopyForumImage_Click(object sender, EventArgs e)
        {
            uim.CopyForumImage();
        }

        private void tsmiCopyForumLinkedImage_Click(object sender, EventArgs e)
        {
            uim.CopyForumLinkedImage();
        }

        private void tsmiCopyFilePath_Click(object sender, EventArgs e)
        {
            uim.CopyFilePath();
        }

        private void tsmiCopyFileName_Click(object sender, EventArgs e)
        {
            uim.CopyFileName();
        }

        private void tsmiCopyFileNameWithExtension_Click(object sender, EventArgs e)
        {
            uim.CopyFileNameWithExtension();
        }

        private void tsmiCopyFolder_Click(object sender, EventArgs e)
        {
            uim.CopyFolder();
        }

        private void tsmiShowErrors_Click(object sender, EventArgs e)
        {
            uim.ShowErrors();
        }

        private void tsmiShowResponse_Click(object sender, EventArgs e)
        {
            uim.ShowResponse();
        }

        private void tsmiShowQRCode_Click(object sender, EventArgs e)
        {
            uim.ShowQRCode();
        }

        private void tsmiUploadSelectedFile_Click(object sender, EventArgs e)
        {
            uim.Upload();
        }

        private void tsmiClearList_Click(object sender, EventArgs e)
        {
            RemoveAllItems();
        }

        private void tsmiHideMenu_Click(object sender, EventArgs e)
        {
            Program.Settings.ShowMenu = !Program.Settings.ShowMenu;
            UpdateMenu();
        }

        private void tsmiImagePreviewShow_Click(object sender, EventArgs e)
        {
            Program.Settings.ImagePreview = ImagePreviewVisibility.Show;
            tsmiImagePreviewShow.Check();
            UpdateControls();
        }

        private void tsmiImagePreviewHide_Click(object sender, EventArgs e)
        {
            Program.Settings.ImagePreview = ImagePreviewVisibility.Hide;
            tsmiImagePreviewHide.Check();
            UpdateControls();
        }

        private void tsmiImagePreviewAutomatic_Click(object sender, EventArgs e)
        {
            Program.Settings.ImagePreview = ImagePreviewVisibility.Automatic;
            tsmiImagePreviewAutomatic.Check();
            UpdateControls();
        }

        #endregion UploadInfoMenu events

        #endregion Form events

        #region Hotkey/Capture codes and form events

        private delegate Image ScreenCaptureDelegate();

        private enum LastRegionCaptureType { Surface, Light, Annotate }

        private LastRegionCaptureType lastRegionCaptureType = LastRegionCaptureType.Surface;

        private void InitHotkeys()
        {
            TaskEx.Run(() =>
            {
                if (Program.HotkeysConfig == null)
                {
                    Program.HotkeySettingsResetEvent.WaitOne();
                }
            },
            () =>
            {
                Program.HotkeyManager = new HotkeyManager(this, Program.HotkeysConfig.Hotkeys);
                Program.HotkeyManager.HotkeyTrigger += HandleHotkeys;
                DebugHelper.WriteLine("HotkeyManager started");

                Program.WatchFolderManager = new WatchFolderManager();
                DebugHelper.WriteLine("WatchFolderManager started");

                UpdateWorkflowsMenu();
            });
        }

        private void HandleHotkeys(HotkeySettings hotkeySetting)
        {
            DebugHelper.WriteLine("Hotkey triggered: " + hotkeySetting);

            if (hotkeySetting.TaskSettings.Job == HotkeyType.None) return;

            HandleTask(hotkeySetting.TaskSettings);
        }

        private void HandleTask(TaskSettings taskSettings)
        {
            TaskSettings safeTaskSettings = TaskSettings.GetSafeTaskSettings(taskSettings);

            switch (safeTaskSettings.Job)
            {
                case HotkeyType.FileUpload:
                    UploadManager.UploadFile(safeTaskSettings);
                    break;
                case HotkeyType.ClipboardUpload:
                    UploadManager.ClipboardUpload(safeTaskSettings);
                    break;
                case HotkeyType.ClipboardUploadWithContentViewer:
                    UploadManager.ClipboardUploadWithContentViewer(safeTaskSettings);
                    break;
                case HotkeyType.UploadURL:
                    UploadManager.UploadURL(safeTaskSettings);
                    break;
                case HotkeyType.DragDropUpload:
                    TaskHelpers.OpenDropWindow();
                    break;
                case HotkeyType.StopUploads:
                    TaskManager.StopAllTasks();
                    break;
                case HotkeyType.PrintScreen:
                    CaptureScreenshot(CaptureType.Screen, safeTaskSettings, false);
                    break;
                case HotkeyType.ActiveWindow:
                    CaptureScreenshot(CaptureType.ActiveWindow, safeTaskSettings, false);
                    break;
                case HotkeyType.ActiveMonitor:
                    CaptureScreenshot(CaptureType.ActiveMonitor, safeTaskSettings, false);
                    break;
                case HotkeyType.RectangleRegion:
                    CaptureScreenshot(CaptureType.Rectangle, safeTaskSettings, false);
                    break;
                case HotkeyType.WindowRectangle:
                    CaptureScreenshot(CaptureType.RectangleWindow, safeTaskSettings, false);
                    break;
                case HotkeyType.RectangleAnnotate:
                    CaptureRectangleAnnotate(safeTaskSettings, false);
                    break;
                case HotkeyType.RectangleLight:
                    CaptureRectangleLight(safeTaskSettings, false);
                    break;
                case HotkeyType.RoundedRectangleRegion:
                    CaptureScreenshot(CaptureType.RoundedRectangle, safeTaskSettings, false);
                    break;
                case HotkeyType.EllipseRegion:
                    CaptureScreenshot(CaptureType.Ellipse, safeTaskSettings, false);
                    break;
                case HotkeyType.TriangleRegion:
                    CaptureScreenshot(CaptureType.Triangle, safeTaskSettings, false);
                    break;
                case HotkeyType.DiamondRegion:
                    CaptureScreenshot(CaptureType.Diamond, safeTaskSettings, false);
                    break;
                case HotkeyType.PolygonRegion:
                    CaptureScreenshot(CaptureType.Polygon, safeTaskSettings, false);
                    break;
                case HotkeyType.FreeHandRegion:
                    CaptureScreenshot(CaptureType.Freehand, safeTaskSettings, false);
                    break;
                case HotkeyType.LastRegion:
                    CaptureScreenshot(CaptureType.LastRegion, safeTaskSettings, false);
                    break;
                case HotkeyType.ScreenRecorder:
                    TaskHelpers.DoScreenRecorder(safeTaskSettings);
                    break;
                case HotkeyType.AutoCapture:
                    TaskHelpers.OpenAutoCapture();
                    break;
                case HotkeyType.ScreenColorPicker:
                    TaskHelpers.OpenScreenColorPicker(safeTaskSettings);
                    break;
                case HotkeyType.Ruler:
                    TaskHelpers.OpenRuler();
                    break;
                case HotkeyType.FTPClient:
                    TaskHelpers.OpenFTPClient();
                    break;
                case HotkeyType.HashCheck:
                    TaskHelpers.OpenHashCheck();
                    break;
                case HotkeyType.IndexFolder:
                    TaskHelpers.OpenIndexFolder();
                    break;
                case HotkeyType.ImageEffects:
                    TaskHelpers.OpenImageEffects();
                    break;
                case HotkeyType.QRCode:
                    TaskHelpers.OpenQRCode();
                    break;
                case HotkeyType.TweetMessage:
                    TaskHelpers.TweetMessage();
                    break;
            }
        }

        public void CaptureScreenshot(CaptureType captureType, TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            switch (captureType)
            {
                case CaptureType.Screen:
                    DoCapture(Screenshot.CaptureFullscreen, CaptureType.Screen, taskSettings, autoHideForm);
                    break;
                case CaptureType.ActiveWindow:
                    CaptureActiveWindow(taskSettings, autoHideForm);
                    break;
                case CaptureType.ActiveMonitor:
                    DoCapture(Screenshot.CaptureActiveMonitor, CaptureType.ActiveMonitor, taskSettings, autoHideForm);
                    break;
                case CaptureType.Rectangle:
                case CaptureType.RectangleWindow:
                case CaptureType.RoundedRectangle:
                case CaptureType.Ellipse:
                case CaptureType.Triangle:
                case CaptureType.Diamond:
                case CaptureType.Polygon:
                case CaptureType.Freehand:
                    CaptureRegion(captureType, taskSettings, autoHideForm);
                    break;
                case CaptureType.LastRegion:
                    CaptureLastRegion(taskSettings, autoHideForm);
                    break;
            }
        }

        private void DoCapture(ScreenCaptureDelegate capture, CaptureType captureType, TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            if (taskSettings.CaptureSettings.IsDelayScreenshot && taskSettings.CaptureSettings.DelayScreenshot > 0)
            {
                TaskEx.Run(() =>
                {
                    int sleep = (int)(taskSettings.CaptureSettings.DelayScreenshot * 1000);
                    Thread.Sleep(sleep);
                },
                () =>
                {
                    DoCaptureWork(capture, captureType, taskSettings, autoHideForm);
                });
            }
            else
            {
                DoCaptureWork(capture, captureType, taskSettings, autoHideForm);
            }
        }

        private void DoCaptureWork(ScreenCaptureDelegate capture, CaptureType captureType, TaskSettings taskSettings, bool autoHideForm = true)
        {
            if (autoHideForm)
            {
                Hide();
                Thread.Sleep(250);
            }

            Image img = null;

            try
            {
                Screenshot.CaptureCursor = taskSettings.CaptureSettings.ShowCursor;
                Screenshot.CaptureShadow = taskSettings.CaptureSettings.CaptureShadow;
                Screenshot.ShadowOffset = taskSettings.CaptureSettings.CaptureShadowOffset;
                Screenshot.CaptureClientArea = taskSettings.CaptureSettings.CaptureClientArea;
                Screenshot.AutoHideTaskbar = taskSettings.CaptureSettings.CaptureAutoHideTaskbar;

                img = capture();
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }
            finally
            {
                if (autoHideForm)
                {
                    this.ShowActivate();
                }

                AfterCapture(img, captureType, taskSettings);
            }
        }

        private void AfterCapture(Image img, CaptureType captureType, TaskSettings taskSettings)
        {
            if (img != null)
            {
                if (taskSettings.GeneralSettings.PlaySoundAfterCapture)
                {
                    Helpers.PlaySoundAsync(Resources.CameraSound);
                }

                if (taskSettings.ImageSettings.ImageEffectOnlyRegionCapture && !IsRegionCapture(captureType))
                {
                    taskSettings.AfterCaptureJob = taskSettings.AfterCaptureJob.Remove(AfterCaptureTasks.AddImageEffects);
                }

                if (taskSettings.GeneralSettings.ShowAfterCaptureTasksForm)
                {
                    using (AfterCaptureForm afterCaptureForm = new AfterCaptureForm(img, taskSettings))
                    {
                        afterCaptureForm.ShowDialog();

                        switch (afterCaptureForm.Result)
                        {
                            case AfterCaptureFormResult.Continue:
                                taskSettings.AfterCaptureJob = afterCaptureForm.AfterCaptureTasks;
                                break;
                            case AfterCaptureFormResult.Copy:
                                taskSettings.AfterCaptureJob = AfterCaptureTasks.CopyImageToClipboard;
                                break;
                            case AfterCaptureFormResult.Cancel:
                                if (img != null) img.Dispose();
                                return;
                        }
                    }
                }

                UploadManager.RunImageTask(img, taskSettings);
            }
        }

        private bool IsRegionCapture(CaptureType captureType)
        {
            return captureType.HasFlagAny(CaptureType.RectangleWindow, CaptureType.Rectangle, CaptureType.RoundedRectangle, CaptureType.Ellipse,
                CaptureType.Triangle, CaptureType.Diamond, CaptureType.Polygon, CaptureType.Freehand, CaptureType.LastRegion);
        }

        private void CaptureActiveWindow(TaskSettings taskSettings, bool autoHideForm = true)
        {
            DoCapture(() =>
            {
                Image img;
                string activeWindowTitle = NativeMethods.GetForegroundWindowText();
                string activeProcessName = null;

                using (Process process = NativeMethods.GetForegroundWindowProcess())
                {
                    if (process != null)
                    {
                        activeProcessName = process.ProcessName;
                    }
                }

                if (taskSettings.CaptureSettings.CaptureTransparent && !taskSettings.CaptureSettings.CaptureClientArea)
                {
                    img = Screenshot.CaptureActiveWindowTransparent();
                }
                else
                {
                    img = Screenshot.CaptureActiveWindow();
                }

                img.Tag = new ImageTag
                {
                    ActiveWindowTitle = activeWindowTitle,
                    ActiveProcessName = activeProcessName
                };

                return img;
            }, CaptureType.ActiveWindow, taskSettings, autoHideForm);
        }

        private void CaptureWindow(IntPtr handle, TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            autoHideForm = autoHideForm && handle != Handle;

            DoCapture(() =>
            {
                if (NativeMethods.IsIconic(handle))
                {
                    NativeMethods.RestoreWindow(handle);
                }

                NativeMethods.SetForegroundWindow(handle);
                Thread.Sleep(250);

                if (taskSettings.CaptureSettings.CaptureTransparent && !taskSettings.CaptureSettings.CaptureClientArea)
                {
                    return Screenshot.CaptureWindowTransparent(handle);
                }

                return Screenshot.CaptureWindow(handle);
            }, CaptureType.Window, taskSettings, autoHideForm);
        }

        private void CaptureRegion(CaptureType captureType, TaskSettings taskSettings, bool autoHideForm = true)
        {
            Surface surface;

            switch (captureType)
            {
                default:
                case CaptureType.Rectangle:
                    surface = new RectangleRegion();
                    break;
                case CaptureType.RectangleWindow:
                    RectangleRegion rectangleRegion = new RectangleRegion();
                    rectangleRegion.AreaManager.WindowCaptureMode = true;
                    rectangleRegion.AreaManager.IncludeControls = true;
                    surface = rectangleRegion;
                    break;
                case CaptureType.RoundedRectangle:
                    surface = new RoundedRectangleRegion();
                    break;
                case CaptureType.Ellipse:
                    surface = new EllipseRegion();
                    break;
                case CaptureType.Triangle:
                    surface = new TriangleRegion();
                    break;
                case CaptureType.Diamond:
                    surface = new DiamondRegion();
                    break;
                case CaptureType.Polygon:
                    surface = new PolygonRegion();
                    break;
                case CaptureType.Freehand:
                    surface = new FreeHandRegion();
                    break;
            }

            DoCapture(() =>
            {
                Image img = null;
                Image screenshot = Screenshot.CaptureFullscreen();

                try
                {
                    surface.Config = taskSettings.CaptureSettings.SurfaceOptions;
                    surface.SurfaceImage = screenshot;
                    surface.Prepare();
                    surface.ShowDialog();

                    if (surface.Result == SurfaceResult.Region)
                    {
                        img = surface.GetRegionImage();
                        screenshot.Dispose();
                    }
                    else if (surface.Result == SurfaceResult.Fullscreen)
                    {
                        img = screenshot;
                    }

                    if (img != null)
                    {
                        lastRegionCaptureType = LastRegionCaptureType.Surface;
                    }
                }
                finally
                {
                    surface.Dispose();
                }

                return img;
            }, captureType, taskSettings, autoHideForm);
        }

        private void CaptureRectangleAnnotate(TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            DoCapture(() =>
            {
                Image img = null;

                using (RectangleAnnotate rectangleAnnotate = new RectangleAnnotate(taskSettings.TaskSettingsCaptureReference.RectangleAnnotateOptions))
                {
                    if (rectangleAnnotate.ShowDialog() == DialogResult.OK)
                    {
                        img = rectangleAnnotate.GetAreaImage();

                        if (img != null)
                        {
                            lastRegionCaptureType = LastRegionCaptureType.Annotate;
                        }
                    }
                }

                return img;
            }, CaptureType.Rectangle, taskSettings, autoHideForm);
        }

        private void CaptureRectangleLight(TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            DoCapture(() =>
            {
                Image img = null;

                using (RectangleLight rectangleLight = new RectangleLight())
                {
                    if (rectangleLight.ShowDialog() == DialogResult.OK)
                    {
                        img = rectangleLight.GetAreaImage();

                        if (img != null)
                        {
                            lastRegionCaptureType = LastRegionCaptureType.Light;
                        }
                    }
                }

                return img;
            }, CaptureType.Rectangle, taskSettings, autoHideForm);
        }

        private void CaptureLastRegion(TaskSettings taskSettings, bool autoHideForm = true)
        {
            switch (lastRegionCaptureType)
            {
                case LastRegionCaptureType.Surface:
                    if (Surface.LastRegionFillPath != null)
                    {
                        DoCapture(() =>
                        {
                            using (Image screenshot = Screenshot.CaptureFullscreen())
                            {
                                return ShapeCaptureHelpers.GetRegionImage(screenshot, Surface.LastRegionFillPath, Surface.LastRegionDrawPath, taskSettings.CaptureSettings.SurfaceOptions);
                            }
                        }, CaptureType.LastRegion, taskSettings, autoHideForm);
                    }
                    else
                    {
                        CaptureRegion(CaptureType.Rectangle, taskSettings, autoHideForm);
                    }
                    break;
                case LastRegionCaptureType.Light:
                    if (!RectangleLight.LastSelectionRectangle0Based.IsEmpty)
                    {
                        DoCapture(() =>
                        {
                            using (Image screenshot = Screenshot.CaptureFullscreen())
                            {
                                return ImageHelpers.CropImage(screenshot, RectangleLight.LastSelectionRectangle0Based);
                            }
                        }, CaptureType.LastRegion, taskSettings, autoHideForm);
                    }
                    else
                    {
                        CaptureRectangleLight(taskSettings, autoHideForm);
                    }
                    break;
                case LastRegionCaptureType.Annotate:
                    if (!RectangleAnnotate.LastSelectionRectangle0Based.IsEmpty)
                    {
                        DoCapture(() =>
                        {
                            using (Image screenshot = Screenshot.CaptureFullscreen())
                            {
                                return ImageHelpers.CropImage(screenshot, RectangleAnnotate.LastSelectionRectangle0Based);
                            }
                        }, CaptureType.LastRegion, taskSettings, autoHideForm);
                    }
                    else
                    {
                        CaptureRectangleAnnotate(taskSettings, autoHideForm);
                    }
                    break;
            }
        }

        private void PrepareCaptureMenuAsync(ToolStripMenuItem tsmiWindow, EventHandler handlerWindow, ToolStripMenuItem tsmiMonitor, EventHandler handlerMonitor)
        {
            tsmiWindow.DropDownItems.Clear();

            WindowsList windowsList = new WindowsList();
            List<WindowInfo> windows = null;

            TaskEx.Run(() =>
            {
                windows = windowsList.GetVisibleWindowsList();
            },
            () =>
            {
                if (windows != null)
                {
                    foreach (WindowInfo window in windows)
                    {
                        try
                        {
                            string title = window.Text.Truncate(50);
                            ToolStripItem tsi = tsmiWindow.DropDownItems.Add(title);
                            tsi.Tag = window;
                            tsi.Click += handlerWindow;

                            using (Icon icon = window.Icon)
                            {
                                if (icon != null && icon.Width > 0 && icon.Height > 0)
                                {
                                    tsi.Image = icon.ToBitmap();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            DebugHelper.WriteException(e);
                        }
                    }
                }

                tsmiMonitor.DropDownItems.Clear();

                Screen[] screens = Screen.AllScreens;

                for (int i = 0; i < screens.Length; i++)
                {
                    Screen screen = screens[i];
                    string text = string.Format("{0}. {1}x{2}", i + 1, screen.Bounds.Width, screen.Bounds.Height);
                    ToolStripItem tsi = tsmiMonitor.DropDownItems.Add(text);
                    tsi.Tag = screen.Bounds;
                    tsi.Click += handlerMonitor;
                }

                tsmiWindow.Invalidate();
                tsmiMonitor.Invalidate();
            });
        }

        #region Menu events

        private void tsmiFullscreen_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Screen);
        }

        private void tsddbCapture_DropDownOpening(object sender, EventArgs e)
        {
            PrepareCaptureMenuAsync(tsmiWindow, tsmiWindowItems_Click, tsmiMonitor, tsmiMonitorItems_Click);
        }

        private void tsmiWindowItems_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            WindowInfo wi = tsi.Tag as WindowInfo;
            if (wi != null)
            {
                CaptureWindow(wi.Handle);
            }
        }

        private void tsmiMonitorItems_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            Rectangle rectangle = (Rectangle)tsi.Tag;
            if (!rectangle.IsEmpty)
            {
                DoCapture(() => Screenshot.CaptureRectangle(rectangle), CaptureType.Monitor);
            }
        }

        private void tsmiWindowRectangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.RectangleWindow);
        }

        private void tsmiRectangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Rectangle);
        }

        private void tsmiRoundedRectangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.RoundedRectangle);
        }

        private void tsmiRectangleAnnotate_Click(object sender, EventArgs e)
        {
            CaptureRectangleAnnotate();
        }

        private void tsmiRectangleLight_Click(object sender, EventArgs e)
        {
            CaptureRectangleLight();
        }

        private void tsmiEllipse_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Ellipse);
        }

        private void tsmiTriangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Triangle);
        }

        private void tsmiDiamond_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Diamond);
        }

        private void tsmiPolygon_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Polygon);
        }

        private void tsmiFreeHand_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Freehand);
        }

        private void tsmiLastRegion_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.LastRegion);
        }

        #endregion Menu events

        #region Tray events

        private void tsmiTrayFullscreen_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Screen, null, false);
        }

        private void tsmiCapture_DropDownOpening(object sender, EventArgs e)
        {
            PrepareCaptureMenuAsync(tsmiTrayWindow, tsmiTrayWindowItems_Click, tsmiTrayMonitor, tsmiTrayMonitorItems_Click);
        }

        private void tsmiTrayWindowItems_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            WindowInfo wi = tsi.Tag as WindowInfo;
            if (wi != null)
            {
                CaptureWindow(wi.Handle, null, false);
            }
        }

        private void tsmiTrayMonitorItems_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            Rectangle rectangle = (Rectangle)tsi.Tag;
            if (!rectangle.IsEmpty)
            {
                DoCapture(() => Screenshot.CaptureRectangle(rectangle), CaptureType.Monitor, null, false);
            }
        }

        private void tsmiTrayRectangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Rectangle, null, false);
        }

        private void tsmiTrayWindowRectangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.RectangleWindow, null, false);
        }

        private void tsmiTrayRectangleAnnotate_Click(object sender, EventArgs e)
        {
            CaptureRectangleAnnotate(null, false);
        }

        private void tsmiTrayRectangleLight_Click(object sender, EventArgs e)
        {
            CaptureRectangleLight(null, false);
        }

        private void tsmiTrayRoundedRectangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.RoundedRectangle, null, false);
        }

        private void tsmiTrayEllipse_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Ellipse, null, false);
        }

        private void tsmiTrayTriangle_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Triangle, null, false);
        }

        private void tsmiTrayDiamond_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Diamond, null, false);
        }

        private void tsmiTrayPolygon_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Polygon, null, false);
        }

        private void tsmiTrayFreeHand_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Freehand, null, false);
        }

        private void tsmiTrayLastRegion_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.LastRegion, null, false);
        }

        #endregion Tray events

        #endregion Hotkey/Capture codes and form events
    }
}