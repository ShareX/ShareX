﻿#region License Information (GPL v3)

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
using ShareX.ImageEffectsLib;
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX
{
    public partial class MainForm : HotkeyForm
    {
        public bool IsReady { get; private set; }

        private bool forceClose, trayMenuSaveSettings = true;
        private int trayClickCount = 0;
        private UploadInfoManager uim;
        private ToolStripDropDownItem tsmiImageFileUploaders, tsmiTrayImageFileUploaders, tsmiTextFileUploaders, tsmiTrayTextFileUploaders;
        private ImageFilesCache actionsMenuIconCache = new ImageFilesCache();

        public MainForm()
        {
            InitializeControls();
        }

        private async void MainForm_HandleCreated(object sender, EventArgs e)
        {
            RunPuushTasks();

            NativeMethods.UseImmersiveDarkMode(Handle, ShareXResources.IsDarkTheme);

            await UpdateControls();

            DebugHelper.WriteLine("Startup time: {0} ms", Program.StartTimer.ElapsedMilliseconds);

            await Program.CLI.UseCommandLineArgs();

            if (Program.Settings.ActionsToolbarRunAtStartup)
            {
                TaskHelpers.OpenActionsToolbar();
            }
        }

        private void InitializeControls()
        {
            InitializeComponent();

            ShareXResources.UseWhiteIcon = Program.Settings.UseWhiteShareXIcon;

            Icon = ShareXResources.Icon;
            niTray.Icon = ShareXResources.Icon;
            Text = Program.Title;

            UpdateTheme();

            this.CloseOnEscape();
            cmsTray.IgnoreSeparatorClick();
            cmsTaskInfo.IgnoreSeparatorClick();
            tsmiMonitor.HideImageMargin();
            tsmiTrayMonitor.HideImageMargin();
            tsmiOpen.HideImageMargin();
            tsmiCopy.HideImageMargin();
            tsmiShortenSelectedURL.HideImageMargin();
            tsmiShareSelectedURL.HideImageMargin();
            tsmiTrayRecentItems.HideImageMargin();

            AfterCaptureTasks[] ignoreAfterCaptureTasks = null;

            if (SystemOptions.DisableUpload)
            {
                ignoreAfterCaptureTasks = new AfterCaptureTasks[] { AfterCaptureTasks.ShowBeforeUploadWindow, AfterCaptureTasks.UploadImageToHost };
            }

            AddMultiEnumItems<AfterCaptureTasks>(x => Program.DefaultTaskSettings.AfterCaptureJob = Program.DefaultTaskSettings.AfterCaptureJob.Swap(x),
                new ToolStripDropDownItem[] { tsddbAfterCaptureTasks, tsmiTrayAfterCaptureTasks }, ignoreAfterCaptureTasks);
            tsddbAfterCaptureTasks.DropDownOpening += TsddbAfterCaptureTasks_DropDownOpening;
            tsmiTrayAfterCaptureTasks.DropDownOpening += TsmiTrayAfterCaptureTasks_DropDownOpening;

            AddMultiEnumItems<AfterUploadTasks>(x => Program.DefaultTaskSettings.AfterUploadJob = Program.DefaultTaskSettings.AfterUploadJob.Swap(x),
                new ToolStripDropDownItem[] { tsddbAfterUploadTasks, tsmiTrayAfterUploadTasks });

            AddEnumItems<ImageDestination>(x =>
            {
                Program.DefaultTaskSettings.ImageDestination = x;

                if (x == ImageDestination.FileUploader)
                {
                    SetEnumChecked(Program.DefaultTaskSettings.ImageFileDestination, tsmiImageFileUploaders, tsmiTrayImageFileUploaders);
                }
                else
                {
                    Uncheck(tsmiImageFileUploaders, tsmiTrayImageFileUploaders);
                }
            }, tsmiImageUploaders, tsmiTrayImageUploaders);
            tsmiImageFileUploaders = (ToolStripDropDownItem)tsmiImageUploaders.DropDownItems[tsmiImageUploaders.DropDownItems.Count - 1];
            tsmiTrayImageFileUploaders = (ToolStripDropDownItem)tsmiTrayImageUploaders.DropDownItems[tsmiTrayImageUploaders.DropDownItems.Count - 1];
            AddEnumItems<FileDestination>(x =>
            {
                Program.DefaultTaskSettings.ImageFileDestination = x;
                tsmiImageFileUploaders.PerformClick();
                tsmiTrayImageFileUploaders.PerformClick();
            }, tsmiImageFileUploaders, tsmiTrayImageFileUploaders);

            AddEnumItems<TextDestination>(x =>
            {
                Program.DefaultTaskSettings.TextDestination = x;

                if (x == TextDestination.FileUploader)
                {
                    SetEnumChecked(Program.DefaultTaskSettings.TextFileDestination, tsmiTextFileUploaders, tsmiTrayTextFileUploaders);
                }
                else
                {
                    Uncheck(tsmiTextFileUploaders, tsmiTrayTextFileUploaders);
                }
            }, tsmiTextUploaders, tsmiTrayTextUploaders);
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

            AddEnumItems<URLSharingServices>(x => Program.DefaultTaskSettings.URLSharingServiceDestination = x, tsmiURLSharingServices, tsmiTrayURLSharingServices);

            foreach (UrlShortenerType urlShortener in Helpers.GetEnums<UrlShortenerType>())
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(urlShortener.GetLocalizedDescription());
                tsmi.Click += (sender, e) => uim.ShortenURL(urlShortener);
                tsmiShortenSelectedURL.DropDownItems.Add(tsmi);
            }

            foreach (URLSharingServices urlSharingService in Helpers.GetEnums<URLSharingServices>())
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(urlSharingService.GetLocalizedDescription());
                tsmi.Click += (sender, e) => uim.ShareURL(urlSharingService);
                tsmiShareSelectedURL.DropDownItems.Add(tsmi);
            }

            lvUploads.SupportCustomTheme();

            ImageList il = new ImageList();
            il.ColorDepth = ColorDepth.Depth32Bit;
            il.Images.Add(Resources.navigation_090_button);
            il.Images.Add(Resources.cross_button);
            il.Images.Add(Resources.tick_button);
            il.Images.Add(Resources.navigation_000_button);
            il.Images.Add(Resources.clock);
            lvUploads.SmallImageList = il;

            TaskManager.TaskListView = new TaskListView(lvUploads);
            TaskManager.TaskThumbnailView = ucTaskThumbnailView;
            uim = new UploadInfoManager();

            foreach (ToolStripDropDownItem dropDownItem in new ToolStripDropDownItem[]
            {
                tsddbAfterCaptureTasks, tsddbAfterUploadTasks, tsmiImageUploaders, tsmiImageFileUploaders, tsmiTextUploaders, tsmiTextFileUploaders, tsmiFileUploaders,
                tsmiURLShorteners, tsmiURLSharingServices, tsmiTrayAfterCaptureTasks, tsmiTrayAfterUploadTasks, tsmiTrayImageUploaders, tsmiTrayImageFileUploaders,
                tsmiTrayTextUploaders, tsmiTrayTextFileUploaders, tsmiTrayFileUploaders, tsmiTrayURLShorteners, tsmiTrayURLSharingServices, tsmiScreenshotDelay,
                tsmiTrayScreenshotDelay
            })
            {
                dropDownItem.DisableMenuCloseOnClick();
            }

            ExportImportControl.UploadRequested += json => UploadManager.UploadText(json);

            if (SystemOptions.DisableUpload)
            {
                tsddbUpload.Visible = false;
                tsddbAfterUploadTasks.Visible = false;
                tsddbDestinations.Visible = false;
                tsmiTestImageUpload.Visible = false;
                tsmiTestTextUpload.Visible = false;
                tsmiTestFileUpload.Visible = false;
                tsmiTestURLShortener.Visible = false;
                tsmiTestURLSharing.Visible = false;

                tsmiTrayUpload.Visible = false;
                tsmiTrayAfterUploadTasks.Visible = false;
                tsmiTrayDestinations.Visible = false;

                tsmiUploadSelectedFile.Visible = false;
                tsmiShortenSelectedURL.Visible = false;
                tsmiShareSelectedURL.Visible = false;
            }

            // TODO: Translate
#if STEAM
            tsbDonate.Text = "ShareX website...";
            tsbDonate.Image = Resources.globe;
#endif

            HandleCreated += MainForm_HandleCreated;
        }

        public async Task UpdateControls()
        {
            IsReady = false;

            niTray.Visible = Program.Settings.ShowTray;

            TaskManager.UpdateMainFormTip();
            TaskManager.RecentManager.InitItems();

            bool isPositionChanged = false;

            if (Program.Settings.RememberMainFormPosition && !Program.Settings.MainFormPosition.IsEmpty &&
                CaptureHelpers.GetScreenBounds().IntersectsWith(new Rectangle(Program.Settings.MainFormPosition, Program.Settings.MainFormSize)))
            {
                StartPosition = FormStartPosition.Manual;
                Location = Program.Settings.MainFormPosition;
                isPositionChanged = true;
            }

            tsMain.Width = tsMain.PreferredSize.Width;
            int height = Math.Max(Size.Height + tsMain.PreferredSize.Height - tsMain.Height, MinimumSize.Height);
            MinimumSize = new Size(MinimumSize.Width, height);

            if (Program.Settings.RememberMainFormSize && !Program.Settings.MainFormSize.IsEmpty)
            {
                Size = Program.Settings.MainFormSize;

                if (!isPositionChanged)
                {
                    StartPosition = FormStartPosition.Manual;
                    Rectangle activeScreen = CaptureHelpers.GetActiveScreenBounds();
                    Location = new Point((activeScreen.Width / 2) - (Size.Width / 2), (activeScreen.Height / 2) - (Size.Height / 2));
                }
            }
            else
            {
                Size = new Size(Size.Width, height);
            }

            if (Program.Settings.PreviewSplitterDistance > 0)
            {
                scMain.SplitterDistance = Program.Settings.PreviewSplitterDistance;
            }

            if (Program.Settings.TaskListViewColumnWidths != null)
            {
                int len = Math.Min(lvUploads.Columns.Count - 1, Program.Settings.TaskListViewColumnWidths.Count);

                for (int i = 0; i < len; i++)
                {
                    lvUploads.Columns[i].Width = Program.Settings.TaskListViewColumnWidths[i];
                }
            }

            TaskbarManager.Enabled = Program.Settings.TaskbarProgressEnabled;

            UpdateCheckStates();
            UpdateUploaderMenuNames();
            UpdateDestinationStates();
            UpdateToggleHotkeyButton();
            AfterTaskSettingsJobs();
            AfterApplicationSettingsJobs();

            await InitHotkeys();

            IsReady = true;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)WindowsMessages.QUERYENDSESSION)
            {
                // Calling ToInt64 because the int conversion operator (called when directly casting the IntPtr to the enum)
                // enforces checked semantics thus crashes any 64 bits build. ToInt64() and long -> enum conversion doesn't.
                EndSessionReasons reason = (EndSessionReasons)m.LParam.ToInt64();
                if (reason.HasFlag(EndSessionReasons.ENDSESSION_CLOSEAPP))
                {
                    // Register for restart. This allows our application to automatically restart when it is installing an update from the Store.
                    // Also allows it to restart if it gets terminated for other reasons (see description of ENDSESSION_CLOSEAPP).
                    // Add the silent switch to avoid ShareX popping up in front of the user when the application restarts.
                    NativeMethods.RegisterApplicationRestart("-silent", 0);
                }
                m.Result = new IntPtr(1); // "Applications should respect the user's intentions and return TRUE."
            }
            else if (m.Msg == (int)WindowsMessages.ENDSESSION)
            {
                if (m.WParam != IntPtr.Zero)
                {
                    // If wParam is not equal to false (0), the application can be terminated at any moment after processing this message
                    // thus should save its data while processing the message.
                    Program.CloseSequence();
                }
                m.Result = IntPtr.Zero; // "If an application processes this message, it should return zero."
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private void AfterShownJobs()
        {
            if (!Program.Settings.ShowMostRecentTaskFirst && lvUploads.Items.Count > 0)
            {
                lvUploads.Items[lvUploads.Items.Count - 1].EnsureVisible();
            }

            if (Program.SteamFirstTimeConfig)
            {
                using (FirstTimeConfigForm firstTimeConfigForm = new FirstTimeConfigForm())
                {
                    firstTimeConfigForm.ShowDialog();
                }
            }
            else
            {
                this.ForceActivate();
            }
        }

        private async Task InitHotkeys()
        {
            await Task.Run(() => SettingManager.WaitHotkeysConfig());

            if (Program.HotkeyManager == null)
            {
                Program.HotkeyManager = new HotkeyManager(this);
                Program.HotkeyManager.HotkeyTrigger += HandleHotkeys;
            }

            Program.HotkeyManager.UpdateHotkeys(Program.HotkeysConfig.Hotkeys, !Program.IgnoreHotkeyWarning);

            DebugHelper.WriteLine("HotkeyManager started.");

            if (Program.WatchFolderManager == null)
            {
                Program.WatchFolderManager = new WatchFolderManager();
            }

            Program.WatchFolderManager.UpdateWatchFolders();

            DebugHelper.WriteLine("WatchFolderManager started.");

            UpdateWorkflowsMenu();

            if (pHotkeys.Visible)
            {
                pHotkeys.Focus();
            }
        }

        private async void HandleHotkeys(HotkeySettings hotkeySetting)
        {
            DebugHelper.WriteLine("Hotkey triggered. " + hotkeySetting);

            await TaskHelpers.ExecuteJob(hotkeySetting.TaskSettings);
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

            ToolStripMenuItem tsmi = new ToolStripMenuItem(Resources.MainForm_UpdateWorkflowsMenu_You_can_add_workflows_from_hotkey_settings___);
            tsmi.Click += tsbHotkeySettings_Click;
            tsddbWorkflows.DropDownItems.Add(tsmi);

            tsmiTrayWorkflows.Visible = tsmiTrayWorkflows.DropDownItems.Count > 0;

            UpdateMainFormTip();
        }

        private void UpdateMainFormTip()
        {
            TaskManager.UpdateMainFormTip();

            dgvHotkeys.Rows.Clear();

            foreach (HotkeySettings hotkey in Program.HotkeysConfig.Hotkeys.Where(x => x.HotkeyInfo.IsValidHotkey))
            {
                int index = dgvHotkeys.Rows.Add();
                DataGridViewRow row = dgvHotkeys.Rows[index];
                row.Cells[0].Style.BackColor = row.Cells[0].Style.SelectionBackColor =
                    hotkey.HotkeyInfo.Status == HotkeyStatus.Registered ? Color.FromArgb(80, 160, 80) : Color.FromArgb(200, 80, 80);
                row.Cells[1].Value = hotkey.HotkeyInfo.ToString();
                row.Cells[2].Value = hotkey.TaskSettings.ToString();
            }
        }

        private ToolStripMenuItem WorkflowMenuItem(HotkeySettings hotkeySetting)
        {
            ToolStripMenuItem tsmi = new ToolStripMenuItem();

            tsmi.Text = hotkeySetting.TaskSettings.ToString().Replace("&", "&&");

            if (!hotkeySetting.TaskSettings.IsUsingDefaultSettings)
            {
                tsmi.Text += "*";
            }

            if (hotkeySetting.HotkeyInfo.IsValidHotkey)
            {
                tsmi.ShortcutKeyDisplayString = "  " + hotkeySetting.HotkeyInfo;
            }

            tsmi.Image = TaskHelpers.FindMenuIcon(hotkeySetting.TaskSettings.Job);

            tsmi.Click += async (sender, e) => await TaskHelpers.ExecuteJob(hotkeySetting.TaskSettings);

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

        private void AddEnumItems<T>(Action<T> selectedEnum, params ToolStripDropDownItem[] parents) where T : Enum
        {
            T[] enums = Helpers.GetEnums<T>();

            foreach (ToolStripDropDownItem parent in parents)
            {
                for (int i = 0; i < enums.Length; i++)
                {
                    T currentEnum = enums[i];
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(currentEnum.GetLocalizedDescription());

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

                        selectedEnum(currentEnum);

                        UpdateUploaderMenuNames();
                    };

                    parent.DropDownItems.Add(tsmi);
                }
            }
        }

        public static void Uncheck(params ToolStripDropDownItem[] lists)
        {
            foreach (ToolStripDropDownItem parent in lists)
            {
                foreach (ToolStripItem dropDownItem in parent.DropDownItems)
                {
                    ((ToolStripMenuItem)dropDownItem).Checked = false;
                }
            }
        }

        private static void SetEnumChecked(Enum value, params ToolStripDropDownItem[] parents)
        {
            if (value == null)
            {
                return;
            }

            int index = value.GetIndex();

            foreach (ToolStripDropDownItem parent in parents)
            {
                ((ToolStripMenuItem)parent.DropDownItems[index]).RadioCheck();
            }
        }

        private void AddMultiEnumItems<T>(Action<T> selectedEnum, ToolStripDropDownItem[] parents, T[] ignoreEnums = null) where T : Enum
        {
            if (ignoreEnums == null)
            {
                ignoreEnums = new T[0];
            }

            T[] enums = Helpers.GetEnums<T>().Skip(1).Except(ignoreEnums).ToArray();

            foreach (ToolStripDropDownItem parent in parents)
            {
                for (int i = 0; i < enums.Length; i++)
                {
                    T currentEnum = enums[i];
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(currentEnum.GetLocalizedDescription());
                    tsmi.Tag = currentEnum;
                    tsmi.Image = TaskHelpers.FindMenuIcon(currentEnum);

                    int index = i;

                    tsmi.Click += (sender, e) =>
                    {
                        foreach (ToolStripDropDownItem parent2 in parents)
                        {
                            ToolStripMenuItem tsmi2 = (ToolStripMenuItem)parent2.DropDownItems[index];
                            tsmi2.Checked = !tsmi2.Checked;
                        }

                        selectedEnum(currentEnum);
                    };

                    parent.DropDownItems.Add(tsmi);
                }
            }
        }

        private void UpdateImageEffectsMenu(ToolStripDropDownItem parent)
        {
            int indexAddImageEffects = AfterCaptureTasks.AddImageEffects.GetIndex() - 1;
            ToolStripMenuItem tsmiAddImageEffects = (ToolStripMenuItem)parent.DropDownItems[indexAddImageEffects];
            tsmiAddImageEffects.DisableMenuCloseOnClick();
            tsmiAddImageEffects.DropDownItems.Clear();

            if (Program.DefaultTaskSettings.ImageSettings.ImageEffectPresets == null)
            {
                Program.DefaultTaskSettings.ImageSettings.ImageEffectPresets = new List<ImageEffectPreset>();
            }

            int count = Program.DefaultTaskSettings.ImageSettings.ImageEffectPresets.Count;
            if (count > 0)
            {
                List<ToolStripItem> items = new List<ToolStripItem>();

                for (int i = 0; i < count; i++)
                {
                    ImageEffectPreset effectPreset = Program.DefaultTaskSettings.ImageSettings.ImageEffectPresets[i];
                    if (effectPreset != null)
                    {
                        ToolStripMenuItem tsmi = new ToolStripMenuItem(effectPreset.ToString());
                        tsmi.Checked = i == Program.DefaultTaskSettings.ImageSettings.SelectedImageEffectPreset;
                        int indexSelected = i;
                        tsmi.Click += (sender, e) =>
                        {
                            Program.DefaultTaskSettings.ImageSettings.SelectedImageEffectPreset = indexSelected;
                            ((ToolStripMenuItem)sender).RadioCheck();
                        };
                        items.Add(tsmi);
                    }
                }

                if (items.Count > 0)
                {
                    tsmiAddImageEffects.DropDownItems.AddRange(items.ToArray());
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
                    parent.DropDownItems[i].ForeColor = UploadersConfigValidator.Validate<T>(i, Program.UploadersConfig) ?
                        SystemColors.ControlText : Color.FromArgb(200, 0, 0);
                }
            }
        }

        private void UpdateInfoManager()
        {
            cmsTaskInfo.SuspendLayout();

            tsmiStopUpload.Visible = tsmiOpen.Visible = tsmiCopy.Visible = tsmiShowErrors.Visible = tsmiShowResponse.Visible =
                tsmiGoogleLens.Visible = tsmiBingVisualSearch.Visible = tsmiShowQRCode.Visible = tsmiOCRImage.Visible =
                tsmiCombineImages.Visible = tsmiUploadSelectedFile.Visible = tsmiDownloadSelectedURL.Visible = tsmiEditSelectedFile.Visible =
                tsmiBeautifyImage.Visible = tsmiAddImageEffects.Visible = tsmiPinSelectedFile.Visible = tsmiRunAction.Visible =
                tsmiDeleteSelectedItem.Visible = tsmiDeleteSelectedFile.Visible = tsmiShortenSelectedURL.Visible = tsmiShareSelectedURL.Visible = false;

            if (Program.Settings.TaskViewMode == TaskViewMode.ListView)
            {
                pbPreview.Reset();
                uim.UpdateSelectedItems(lvUploads.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as WorkerTask));

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

                switch (Program.Settings.ImagePreviewLocation)
                {
                    case ImagePreviewLocation.Side:
                        scMain.Orientation = Orientation.Vertical;
                        break;
                    case ImagePreviewLocation.Bottom:
                        scMain.Orientation = Orientation.Horizontal;
                        break;
                }
            }
            else if (Program.Settings.TaskViewMode == TaskViewMode.ThumbnailView)
            {
                uim.UpdateSelectedItems(ucTaskThumbnailView.SelectedPanels.Select(x => x.Task));
            }

            if (uim.IsItemSelected)
            {
                // Open
                tsmiOpen.Visible = true;

                tsmiOpenURL.Enabled = uim.SelectedItem.IsURLExist;
                tsmiOpenShortenedURL.Enabled = uim.SelectedItem.IsShortenedURLExist;
                tsmiOpenThumbnailURL.Enabled = uim.SelectedItem.IsThumbnailURLExist;
                tsmiOpenDeletionURL.Enabled = uim.SelectedItem.IsDeletionURLExist;

                tsmiOpenFile.Enabled = uim.SelectedItem.IsFileExist;
                tsmiOpenFolder.Enabled = uim.SelectedItem.IsFileExist;
                tsmiOpenThumbnailFile.Enabled = uim.SelectedItem.IsThumbnailFileExist;

                if (uim.SelectedItems != null && uim.SelectedItems.Any(x => x.Task.IsWorking))
                {
                    tsmiStopUpload.Visible = true;
                }
                else
                {
                    tsmiShowErrors.Visible = uim.SelectedItem.Info.Result.IsError;

                    // Copy
                    tsmiCopy.Visible = true;

                    tsmiCopyURL.Enabled = uim.SelectedItems.Any(x => x.IsURLExist);
                    tsmiCopyShortenedURL.Enabled = uim.SelectedItems.Any(x => x.IsShortenedURLExist);
                    tsmiCopyThumbnailURL.Enabled = uim.SelectedItems.Any(x => x.IsThumbnailURLExist);
                    tsmiCopyDeletionURL.Enabled = uim.SelectedItems.Any(x => x.IsDeletionURLExist);

                    tsmiCopyFile.Enabled = uim.SelectedItem.IsFileExist;
                    tsmiCopyImage.Enabled = uim.SelectedItem.IsImageFile;
                    tsmiCopyImageDimensions.Enabled = uim.SelectedItem.IsImageFile;
                    tsmiCopyText.Enabled = uim.SelectedItem.IsTextFile;
                    tsmiCopyThumbnailFile.Enabled = uim.SelectedItem.IsThumbnailFileExist;
                    tsmiCopyThumbnailImage.Enabled = uim.SelectedItem.IsThumbnailFileExist;

                    tsmiCopyHTMLLink.Enabled = uim.SelectedItems.Any(x => x.IsURLExist);
                    tsmiCopyHTMLImage.Enabled = uim.SelectedItems.Any(x => x.IsImageURL);
                    tsmiCopyHTMLLinkedImage.Enabled = uim.SelectedItems.Any(x => x.IsImageURL && x.IsThumbnailURLExist);

                    tsmiCopyForumLink.Enabled = uim.SelectedItems.Any(x => x.IsURLExist);
                    tsmiCopyForumImage.Enabled = uim.SelectedItems.Any(x => x.IsImageURL && x.IsURLExist);
                    tsmiCopyForumLinkedImage.Enabled = uim.SelectedItems.Any(x => x.IsImageURL && x.IsThumbnailURLExist);

                    tsmiCopyMarkdownLink.Enabled = uim.SelectedItems.Any(x => x.IsURLExist);
                    tsmiCopyMarkdownImage.Enabled = uim.SelectedItems.Any(x => x.IsImageURL);
                    tsmiCopyMarkdownLinkedImage.Enabled = uim.SelectedItems.Any(x => x.IsImageURL && x.IsThumbnailURLExist);

                    tsmiCopyFilePath.Enabled = uim.SelectedItems.Any(x => x.IsFilePathValid);
                    tsmiCopyFileName.Enabled = uim.SelectedItems.Any(x => x.IsFilePathValid);
                    tsmiCopyFileNameWithExtension.Enabled = uim.SelectedItems.Any(x => x.IsFilePathValid);
                    tsmiCopyFolder.Enabled = uim.SelectedItems.Any(x => x.IsFilePathValid);

                    CleanCustomClipboardFormats();

                    if (Program.Settings.ClipboardContentFormats != null && Program.Settings.ClipboardContentFormats.Count > 0)
                    {
                        tssCopy6.Visible = true;

                        foreach (ClipboardFormat cf in Program.Settings.ClipboardContentFormats)
                        {
                            ToolStripMenuItem tsmiClipboardFormat = new ToolStripMenuItem(cf.Description);
                            tsmiClipboardFormat.Tag = cf;
                            tsmiClipboardFormat.Click += tsmiClipboardFormat_Click;
                            tsmiCopy.DropDownItems.Add(tsmiClipboardFormat);
                        }
                    }

                    tsmiUploadSelectedFile.Visible = !SystemOptions.DisableUpload && uim.SelectedItem.IsFileExist;
                    tsmiDownloadSelectedURL.Visible = uim.SelectedItem.IsFileURL;
                    tsmiEditSelectedFile.Visible = uim.SelectedItem.IsImageFile;
                    tsmiBeautifyImage.Visible = uim.SelectedItem.IsImageFile;
                    tsmiAddImageEffects.Visible = uim.SelectedItem.IsImageFile;
                    tsmiPinSelectedFile.Visible = uim.SelectedItem.IsImageFile;
                    UpdateActionsMenu(uim.SelectedItem.Info.FilePath);
                    tsmiDeleteSelectedItem.Visible = true;
                    tsmiDeleteSelectedFile.Visible = uim.SelectedItem.IsFileExist;
                    tsmiShortenSelectedURL.Visible = !SystemOptions.DisableUpload && uim.SelectedItem.IsURLExist;
                    tsmiShareSelectedURL.Visible = !SystemOptions.DisableUpload && uim.SelectedItem.IsURLExist;
                    tsmiGoogleLens.Visible = uim.SelectedItem.IsURLExist;
                    tsmiBingVisualSearch.Visible = uim.SelectedItem.IsURLExist;
                    tsmiShowQRCode.Visible = uim.SelectedItem.IsURLExist;
                    tsmiOCRImage.Visible = uim.SelectedItem.IsImageFile;
                    tsmiCombineImages.Visible = uim.SelectedItems.Count(x => x.IsImageFile) > 1;
                    tsmiShowResponse.Visible = !string.IsNullOrEmpty(uim.SelectedItem.Info.Result.Response);
                }

                if (Program.Settings.TaskViewMode == TaskViewMode.ListView)
                {
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
            }

            tsmiClearList.Visible = tssUploadInfo1.Visible = lvUploads.Items.Count > 0;

            cmsTaskInfo.ResumeLayout();

            Refresh();
        }

        private void UpdateTaskViewMode()
        {
            if (Program.Settings.TaskViewMode == TaskViewMode.ListView)
            {
                tsmiSwitchTaskViewMode.Text = Resources.SwitchToThumbnailView;
                tsmiSwitchTaskViewMode.Image = Resources.application_icon_large;
                scMain.Visible = true;
                ucTaskThumbnailView.Visible = false;
                scMain.Focus();
            }
            else
            {
                tsmiSwitchTaskViewMode.Text = Resources.SwitchToListView;
                tsmiSwitchTaskViewMode.Image = Resources.application_list;
                ucTaskThumbnailView.Visible = true;
                scMain.Visible = false;
                ucTaskThumbnailView.Focus();
            }
        }

        public void UpdateTheme()
        {
            if (Program.Settings.Themes == null || Program.Settings.Themes.Count == 0)
            {
                Program.Settings.Themes = ShareXTheme.GetDefaultThemes();
                Program.Settings.SelectedTheme = 0;
            }

            if (!Program.Settings.Themes.IsValidIndex(Program.Settings.SelectedTheme))
            {
                Program.Settings.SelectedTheme = 0;
            }

            ShareXResources.Theme = Program.Settings.Themes[Program.Settings.SelectedTheme];

            if (IsHandleCreated)
            {
                NativeMethods.UseImmersiveDarkMode(Handle, ShareXResources.IsDarkTheme);
            }

#pragma warning disable WFO5001
            if (ShareXResources.IsDarkTheme)
            {
                Application.SetColorMode(SystemColorMode.Dark);
            }
            else
            {
                Application.SetColorMode(SystemColorMode.Classic);
            }
#pragma warning restore WFO5001

            BackColor = ShareXResources.Theme.BackgroundColor;
            tsMain.Font = ShareXResources.Theme.MenuFont;
            tsMain.Renderer = new ToolStripDarkRenderer();
            tsMain.DrawCustomBorder = false;
            ShareXResources.ApplyCustomThemeToContextMenuStrip(cmsTray);
            ShareXResources.ApplyCustomThemeToContextMenuStrip(cmsTaskInfo);
            ttMain.BackColor = ShareXResources.Theme.BackgroundColor;
            ttMain.ForeColor = ShareXResources.Theme.TextColor;
            lvUploads.BackColor = ShareXResources.Theme.BackgroundColor;
            lvUploads.ForeColor = ShareXResources.Theme.TextColor;
            scMain.SplitterColor = ShareXResources.Theme.BackgroundColor;
            scMain.SplitterLineColor = ShareXResources.Theme.BorderColor;
            ShareXResources.ApplyCustomThemeToControl(dgvHotkeys);
            dgvHotkeys.BackgroundColor = ShareXResources.Theme.BackgroundColor;

            tsbX.Image = ShareXResources.IsDarkTheme ? Resources.X_white : Resources.X_black;
            tsbDiscord.Image = ShareXResources.IsDarkTheme ? Resources.Discord_white : Resources.Discord_black;

            tsmiQRCode.Image = TaskHelpers.FindMenuIcon(HotkeyType.QRCode);
            tsmiTrayQRCode.Image = TaskHelpers.FindMenuIcon(HotkeyType.QRCode);
            tsmiShowQRCode.Image = TaskHelpers.FindMenuIcon(HotkeyType.QRCode);

            tsmiOCR.Image = TaskHelpers.FindMenuIcon(HotkeyType.OCR);
            tsmiTrayOCR.Image = TaskHelpers.FindMenuIcon(HotkeyType.OCR);
            tsmiOCRImage.Image = TaskHelpers.FindMenuIcon(HotkeyType.OCR);

            tsmiShortenURL.Image = TaskHelpers.FindMenuIcon(HotkeyType.ShortenURL);
            tsmiTrayShortenURL.Image = TaskHelpers.FindMenuIcon(HotkeyType.ShortenURL);
            tsmiURLShorteners.Image = TaskHelpers.FindMenuIcon(HotkeyType.ShortenURL);
            tsmiTrayURLShorteners.Image = TaskHelpers.FindMenuIcon(HotkeyType.ShortenURL);
            tsmiTestURLShortener.Image = TaskHelpers.FindMenuIcon(HotkeyType.ShortenURL);
            tsmiShortenSelectedURL.Image = TaskHelpers.FindMenuIcon(HotkeyType.ShortenURL);

            pbPreview.UpdateTheme();
            pbPreview.UpdateCheckers(true);
            ucTaskThumbnailView.UpdateTheme();
        }

        private void CleanCustomClipboardFormats()
        {
            tssCopy6.Visible = false;

            int tssCopy6Index = tsmiCopy.DropDownItems.IndexOf(tssCopy6);

            while (tssCopy6Index < tsmiCopy.DropDownItems.Count - 1)
            {
                using (ToolStripItem tsi = tsmiCopy.DropDownItems[tsmiCopy.DropDownItems.Count - 1])
                {
                    tsmiCopy.DropDownItems.Remove(tsi);
                }
            }
        }

        private void UpdateActionsMenu(string filePath)
        {
            tsmiRunAction.DropDownItems.Clear();

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                IEnumerable<ExternalProgram> actions = Program.DefaultTaskSettings.ExternalPrograms.
                    Where(x => !string.IsNullOrEmpty(x.Name) && x.CheckExtension(filePath));

                if (actions.Count() > 0)
                {
                    tsmiRunAction.Visible = true;

                    foreach (ExternalProgram action in actions)
                    {
                        string name = action.Name.Truncate(50, "...");
                        ToolStripMenuItem tsmi = new ToolStripMenuItem(name);

                        try
                        {
                            string actionFilePath = action.GetFullPath();
                            tsmi.Image = actionsMenuIconCache.GetFileIconAsImage(actionFilePath);
                        }
                        catch
                        {
                        }

                        tsmi.Click += async (sender, e) => await action.RunAsync(filePath);
                        tsmiRunAction.DropDownItems.Add(tsmi);
                    }
                }
            }
        }

        private void AfterApplicationSettingsJobs()
        {
            HotkeyRepeatLimit = Program.Settings.HotkeyRepeatLimit;

            HelpersOptions.CurrentProxy = Program.Settings.ProxySettings;
            HelpersOptions.URLEncodeIgnoreEmoji = Program.Settings.URLEncodeIgnoreEmoji;
            HelpersOptions.DefaultCopyImageFillBackground = Program.Settings.DefaultClipboardCopyImageFillBackground;
            HelpersOptions.UseAlternativeClipboardCopyImage = Program.Settings.UseAlternativeClipboardCopyImage;
            HelpersOptions.UseAlternativeClipboardGetImage = Program.Settings.UseAlternativeClipboardGetImage;
            HelpersOptions.RotateImageByExifOrientationData = Program.Settings.RotateImageByExifOrientationData;
            HelpersOptions.BrowserPath = Program.Settings.BrowserPath;
            HelpersOptions.RecentColors = Program.Settings.RecentColors;
            HelpersOptions.DevMode = Program.Settings.DevMode;
            Program.UpdateHelpersSpecialFolders();

            TaskManager.RecentManager.MaxCount = Program.Settings.RecentTasksMaxCount;

            UpdateTheme();
            Refresh();

            if (ShareXResources.UseWhiteIcon != Program.Settings.UseWhiteShareXIcon)
            {
                ShareXResources.UseWhiteIcon = Program.Settings.UseWhiteShareXIcon;

                Icon = ShareXResources.Icon;
                niTray.Icon = ShareXResources.Icon;
            }

            Text = Program.Title;
            niTray.Text = Program.TitleShort;

            tsmiRestartAsAdmin.Visible = HelpersOptions.DevMode && !Helpers.IsAdministrator();

#if RELEASE
            ConfigureAutoUpdate();
#else
            if (UpdateChecker.ForceUpdate)
            {
                ConfigureAutoUpdate();
            }
#endif

            UpdateTaskViewMode();
            UpdateMainWindowLayout();
            UpdateInfoManager();
        }

        private void ConfigureAutoUpdate()
        {
            Program.UpdateManager.AllowAutoUpdate = !SystemOptions.DisableUpdateCheck && Program.Settings.AutoCheckUpdate;
            Program.UpdateManager.UpdateChannel = Program.Settings.UpdateChannel;
            Program.UpdateManager.ConfigureAutoUpdate();
        }

        private void AfterTaskSettingsJobs()
        {
            tsmiShowCursor.Checked = tsmiTrayShowCursor.Checked = Program.DefaultTaskSettings.CaptureSettings.ShowCursor;
            SetScreenshotDelay(Program.DefaultTaskSettings.CaptureSettings.ScreenshotDelay);
        }

        public void UpdateCheckStates()
        {
            SetMultiEnumChecked(Program.DefaultTaskSettings.AfterCaptureJob, tsddbAfterCaptureTasks, tsmiTrayAfterCaptureTasks);
            SetMultiEnumChecked(Program.DefaultTaskSettings.AfterUploadJob, tsddbAfterUploadTasks, tsmiTrayAfterUploadTasks);
            SetEnumChecked(Program.DefaultTaskSettings.ImageDestination, tsmiImageUploaders, tsmiTrayImageUploaders);
            SetImageFileDestinationChecked(Program.DefaultTaskSettings.ImageDestination, Program.DefaultTaskSettings.ImageFileDestination, tsmiImageFileUploaders, tsmiTrayImageFileUploaders);
            SetEnumChecked(Program.DefaultTaskSettings.TextDestination, tsmiTextUploaders, tsmiTrayTextUploaders);
            SetTextFileDestinationChecked(Program.DefaultTaskSettings.TextDestination, Program.DefaultTaskSettings.TextFileDestination, tsmiTextFileUploaders, tsmiTrayTextFileUploaders);
            SetEnumChecked(Program.DefaultTaskSettings.FileDestination, tsmiFileUploaders, tsmiTrayFileUploaders);
            SetEnumChecked(Program.DefaultTaskSettings.URLShortenerDestination, tsmiURLShorteners, tsmiTrayURLShorteners);
            SetEnumChecked(Program.DefaultTaskSettings.URLSharingServiceDestination, tsmiURLSharingServices, tsmiTrayURLSharingServices);
        }

        public static void SetTextFileDestinationChecked(TextDestination textDestination, FileDestination textFileDestination, params ToolStripDropDownItem[] lists)
        {
            if (textDestination == TextDestination.FileUploader)
            {
                SetEnumChecked(textFileDestination, lists);
            }
            else
            {
                Uncheck(lists);
            }
        }

        public static void SetImageFileDestinationChecked(ImageDestination imageDestination, FileDestination imageFileDestination, params ToolStripDropDownItem[] lists)
        {
            if (imageDestination == ImageDestination.FileUploader)
            {
                SetEnumChecked(imageFileDestination, lists);
            }
            else
            {
                Uncheck(lists);
            }
        }

        public void UpdateUploaderMenuNames()
        {
            string imageUploader = Program.DefaultTaskSettings.ImageDestination == ImageDestination.FileUploader ?
                Program.DefaultTaskSettings.ImageFileDestination.GetLocalizedDescription() : Program.DefaultTaskSettings.ImageDestination.GetLocalizedDescription();
            tsmiImageUploaders.Text = tsmiTrayImageUploaders.Text = string.Format(Resources.TaskSettingsForm_UpdateUploaderMenuNames_Image_uploader___0_, imageUploader);

            string textUploader = Program.DefaultTaskSettings.TextDestination == TextDestination.FileUploader ?
                Program.DefaultTaskSettings.TextFileDestination.GetLocalizedDescription() : Program.DefaultTaskSettings.TextDestination.GetLocalizedDescription();
            tsmiTextUploaders.Text = tsmiTrayTextUploaders.Text = string.Format(Resources.TaskSettingsForm_UpdateUploaderMenuNames_Text_uploader___0_, textUploader);

            tsmiFileUploaders.Text = tsmiTrayFileUploaders.Text = string.Format(Resources.TaskSettingsForm_UpdateUploaderMenuNames_File_uploader___0_,
                Program.DefaultTaskSettings.FileDestination.GetLocalizedDescription());

            tsmiURLShorteners.Text = tsmiTrayURLShorteners.Text = string.Format(Resources.TaskSettingsForm_UpdateUploaderMenuNames_URL_shortener___0_,
                Program.DefaultTaskSettings.URLShortenerDestination.GetLocalizedDescription());

            tsmiURLSharingServices.Text = tsmiTrayURLSharingServices.Text = string.Format(Resources.TaskSettingsForm_UpdateUploaderMenuNames_URL_sharing_service___0_,
                Program.DefaultTaskSettings.URLSharingServiceDestination.GetLocalizedDescription());
        }

        private WorkerTask[] GetSelectedTasks()
        {
            if (lvUploads.SelectedItems.Count > 0)
            {
                return lvUploads.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as WorkerTask).Where(x => x != null).ToArray();
            }

            return null;
        }

        private void RemoveTasks(WorkerTask[] tasks)
        {
            if (tasks != null)
            {
                foreach (WorkerTask task in tasks.Where(x => x != null && !x.IsWorking))
                {
                    TaskManager.Remove(task);
                }

                UpdateInfoManager();
            }
        }

        private void RemoveSelectedItems()
        {
            IEnumerable<WorkerTask> tasks = null;

            if (Program.Settings.TaskViewMode == TaskViewMode.ListView)
            {
                tasks = lvUploads.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as WorkerTask);
            }
            else if (Program.Settings.TaskViewMode == TaskViewMode.ThumbnailView)
            {
                tasks = ucTaskThumbnailView.SelectedPanels.Select(x => x.Task);
            }

            RemoveTasks(tasks.ToArray());
        }

        private void RemoveAllItems()
        {
            RemoveTasks(lvUploads.Items.Cast<ListViewItem>().Select(x => x.Tag as WorkerTask).ToArray());
        }

        private void UpdateMainWindowLayout()
        {
            tsMain.Visible = Program.Settings.ShowMenu;

            ucTaskThumbnailView.TitleVisible = Program.Settings.ShowThumbnailTitle;
            ucTaskThumbnailView.TitleLocation = Program.Settings.ThumbnailTitleLocation;
            ucTaskThumbnailView.ThumbnailSize = Program.Settings.ThumbnailSize;
            ucTaskThumbnailView.ClickAction = Program.Settings.ThumbnailClickAction;

            lvUploads.HeaderStyle = Program.Settings.ShowColumns ? ColumnHeaderStyle.Nonclickable : ColumnHeaderStyle.None;

            Refresh();
        }

        public void UpdateToggleHotkeyButton()
        {
            if (Program.Settings.DisableHotkeys)
            {
                tsmiTrayToggleHotkeys.Text = Resources.MainForm_UpdateToggleHotkeyButton_Enable_hotkeys;
                tsmiTrayToggleHotkeys.Image = Resources.keyboard__plus;
            }
            else
            {
                tsmiTrayToggleHotkeys.Text = Resources.MainForm_UpdateToggleHotkeyButton_Disable_hotkeys;
                tsmiTrayToggleHotkeys.Image = Resources.keyboard__minus;
            }
        }

        private void RunPuushTasks()
        {
            if (Program.PuushMode && Program.Settings.IsFirstTimeRun)
            {
                using (PuushLoginForm puushLoginForm = new PuushLoginForm())
                {
                    if (puushLoginForm.ShowDialog() == DialogResult.OK)
                    {
                        Program.DefaultTaskSettings.ImageDestination = ImageDestination.FileUploader;
                        Program.DefaultTaskSettings.ImageFileDestination = FileDestination.Puush;
                        Program.DefaultTaskSettings.TextDestination = TextDestination.FileUploader;
                        Program.DefaultTaskSettings.TextFileDestination = FileDestination.Puush;
                        Program.DefaultTaskSettings.FileDestination = FileDestination.Puush;

                        SettingManager.WaitUploadersConfig();

                        if (Program.UploadersConfig != null)
                        {
                            Program.UploadersConfig.PuushAPIKey = puushLoginForm.APIKey;
                        }
                    }
                }
            }
        }

        private void SetScreenshotDelay(decimal delay)
        {
            Program.DefaultTaskSettings.CaptureSettings.ScreenshotDelay = delay;

            switch (delay)
            {
                default:
                    tsmiScreenshotDelay.UpdateCheckedAll(false);
                    tsmiTrayScreenshotDelay.UpdateCheckedAll(false);
                    break;
                case 0:
                    tsmiScreenshotDelay0.RadioCheck();
                    tsmiTrayScreenshotDelay0.RadioCheck();
                    break;
                case 1:
                    tsmiScreenshotDelay1.RadioCheck();
                    tsmiTrayScreenshotDelay1.RadioCheck();
                    break;
                case 2:
                    tsmiScreenshotDelay2.RadioCheck();
                    tsmiTrayScreenshotDelay2.RadioCheck();
                    break;
                case 3:
                    tsmiScreenshotDelay3.RadioCheck();
                    tsmiTrayScreenshotDelay3.RadioCheck();
                    break;
                case 4:
                    tsmiScreenshotDelay4.RadioCheck();
                    tsmiTrayScreenshotDelay4.RadioCheck();
                    break;
                case 5:
                    tsmiScreenshotDelay5.RadioCheck();
                    tsmiTrayScreenshotDelay5.RadioCheck();
                    break;
            }

            tsmiScreenshotDelay.Text = tsmiTrayScreenshotDelay.Text = string.Format(Resources.ScreenshotDelay0S, delay.ToString("0.#"));
            tsmiScreenshotDelay.Checked = tsmiTrayScreenshotDelay.Checked = delay > 0;
        }

        private async Task PrepareCaptureMenuAsync(ToolStripMenuItem tsmiWindow, EventHandler handlerWindow, ToolStripMenuItem tsmiMonitor, EventHandler handlerMonitor)
        {
            tsmiWindow.DropDownItems.Clear();
            WindowsList windowsList = new WindowsList();
            List<WindowInfo> windows = await Task.Run(() => windowsList.GetVisibleWindowsList());

            if (windows != null && windows.Count > 0)
            {
                List<ToolStripItem> items = new List<ToolStripItem>();

                foreach (WindowInfo window in windows)
                {
                    try
                    {
                        string title = window.Text.Truncate(50, "...");
                        ToolStripMenuItem tsmi = new ToolStripMenuItem(title);
                        tsmi.Tag = window;
                        tsmi.Click += handlerWindow;
                        items.Add(tsmi);

                        using (Icon icon = await Task.Run(() => window.Icon))
                        {
                            if (icon != null && icon.Width > 0 && icon.Height > 0)
                            {
                                tsmi.Image = icon.ToBitmap();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        DebugHelper.WriteException(e);
                    }
                }

                tsmiWindow.DropDownItems.AddRange(items.ToArray());
            }

            tsmiWindow.Invalidate();

            tsmiMonitor.DropDownItems.Clear();
            Screen[] screens = Screen.AllScreens;

            if (screens != null && screens.Length > 0)
            {
                ToolStripItem[] items = new ToolStripItem[screens.Length];

                for (int i = 0; i < items.Length; i++)
                {
                    Screen screen = screens[i];
                    string text = string.Format("{0}. {1}x{2}", i + 1, screen.Bounds.Width, screen.Bounds.Height);
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(text);
                    tsmi.Tag = screen.Bounds;
                    tsmi.Click += handlerMonitor;
                    items[i] = tsmi;
                }

                tsmiMonitor.DropDownItems.AddRange(items);
            }

            tsmiMonitor.Invalidate();
        }

        public void ForceClose()
        {
            if (ScreenRecordManager.IsRecording)
            {
                if (MessageBox.Show(Resources.ShareXCannotBeClosedWhileScreenRecordingIsActive, "ShareX",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    ScreenRecordManager.AbortRecording();
                }
            }
            else
            {
                forceClose = true;
                Close();
            }
        }

        #region Form events

        protected override void SetVisibleCore(bool value)
        {
            if (value && !IsHandleCreated && (Program.SilentRun || Program.Settings.SilentRun) && Program.Settings.ShowTray)
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
        }

        private void MainForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !CaptureHelpers.GetScreenBounds().IntersectsWith(Bounds))
            {
                Rectangle activeScreen = CaptureHelpers.GetActiveScreenBounds();
                Location = new Point((activeScreen.Width - Size.Width) / 2, (activeScreen.Height - Size.Height) / 2);
            }
        }

        private void MainForm_LocationChanged(object sender, EventArgs e)
        {
            if (IsReady && WindowState == FormWindowState.Normal)
            {
                Program.Settings.MainFormPosition = Location;
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (IsReady && WindowState == FormWindowState.Normal)
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
                SettingManager.SaveAllSettingsAsync();

                if (Program.Settings.FirstTimeMinimizeToTray)
                {
                    TaskHelpers.ShowNotificationTip(Resources.ShareXIsMinimizedToTheSystemTray, "ShareX", 8000);
                    Program.Settings.FirstTimeMinimizeToTray = false;
                }
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

        private void TtMain_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            e.DrawBorder();
            e.DrawText();
        }

        private void dgvHotkeys_MouseUp(object sender, MouseEventArgs e)
        {
            if (Program.Settings.TaskViewMode == TaskViewMode.ListView)
            {
                if (e.Button == MouseButtons.Left)
                {
                    lvUploads.Focus();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    UpdateInfoManager();
                    cmsTaskInfo.Show((Control)sender, e.X + 1, e.Y + 1);
                }
            }
            else if (Program.Settings.TaskViewMode == TaskViewMode.ThumbnailView)
            {
                if (e.Button == MouseButtons.Right)
                {
                    UcTaskView_ContextMenuRequested(sender, e);
                }
            }
        }

        private void dgvHotkeys_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                tsbHotkeySettings_Click(sender, e);
            }
        }

        private async void lvUploads_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvUploads.SelectedIndexChanged -= lvUploads_SelectedIndexChanged;
            await Task.Delay(1);
            lvUploads.SelectedIndexChanged += lvUploads_SelectedIndexChanged;
            UpdateInfoManager();
        }

        private void lvUploads_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UpdateInfoManager();
                cmsTaskInfo.Show(lvUploads, e.X + 1, e.Y + 1);
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
                case Keys.Shift | Keys.Enter:
                    uim.OpenFolder();
                    break;
                case Keys.Control | Keys.C:
                    uim.TryCopy();
                    break;
                case Keys.Shift | Keys.C:
                    uim.CopyFile();
                    break;
                case Keys.Alt | Keys.C:
                    uim.CopyImage();
                    break;
                case Keys.Control | Keys.Shift | Keys.C:
                    uim.CopyFilePath();
                    break;
                case Keys.Control | Keys.X:
                    uim.TryCopy();
                    RemoveSelectedItems();
                    break;
                case Keys.Control | Keys.V:
                    UploadManager.ClipboardUploadMainWindow();
                    break;
                case Keys.Control | Keys.U:
                    uim.Upload();
                    break;
                case Keys.Control | Keys.D:
                    uim.Download();
                    break;
                case Keys.Control | Keys.E:
                    uim.EditImage();
                    break;
                case Keys.Control | Keys.P:
                    uim.PinToScreen();
                    break;
                case Keys.Delete:
                    RemoveSelectedItems();
                    break;
                case Keys.Shift | Keys.Delete:
                    uim.DeleteFiles();
                    RemoveSelectedItems();
                    break;
                case Keys.Apps:
                    if (lvUploads.SelectedItems.Count > 0)
                    {
                        UpdateInfoManager();
                        Rectangle rect = lvUploads.GetItemRect(lvUploads.SelectedIndex);
                        cmsTaskInfo.Show(lvUploads, new Point(rect.X, rect.Bottom));
                    }
                    break;
            }

            e.SuppressKeyPress = true;
        }

        private void pbPreview_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && lvUploads.SelectedIndices.Count > 0)
            {
                string[] files = lvUploads.Items.Cast<ListViewItem>().Select(x => ((WorkerTask)x.Tag).Info?.FilePath).ToArray();
                int index = lvUploads.SelectedIndices[0];
                ImageViewer.ShowImage(files, index);
            }
        }

        private void ucTaskThumbnailView_SelectedPanelChanged(object sender, EventArgs e)
        {
            UpdateInfoManager();
        }

        private void UcTaskView_ContextMenuRequested(object sender, MouseEventArgs e)
        {
            cmsTaskInfo.Show(sender as Control, e.X + 1, e.Y + 1);
        }

        private void cmsTaskInfo_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.Keyboard)
            {
                e.Cancel = !(NativeMethods.GetKeyState((int)Keys.Apps) < 0 || NativeMethods.GetKeyState((int)Keys.F10) < 0 || NativeMethods.GetKeyState((int)Keys.Escape) < 0);
            }
        }

        private void cmsTaskInfo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Apps)
            {
                cmsTaskInfo.Close();
            }
        }

        private void lvUploads_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (IsReady)
            {
                Program.Settings.TaskListViewColumnWidths = new List<int>();

                for (int i = 0; i < lvUploads.Columns.Count; i++)
                {
                    Program.Settings.TaskListViewColumnWidths.Add(lvUploads.Columns[i].Width);
                }
            }
        }

        private void lvUploads_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TaskInfo[] taskInfos = GetSelectedTasks().Select(x => x.Info).Where(x => x != null).ToArray();

            if (taskInfos.Length > 0)
            {
                IDataObject dataObject = null;

                if (ModifierKeys.HasFlag(Keys.Control))
                {
                    string[] urls = taskInfos.Select(x => x.ToString()).Where(x => !string.IsNullOrEmpty(x)).ToArray();

                    if (urls.Length > 0)
                    {
                        dataObject = new DataObject(DataFormats.Text, string.Join(Environment.NewLine, urls));
                    }
                }
                else
                {
                    string[] files = taskInfos.Select(x => x.FilePath).Where(x => !string.IsNullOrEmpty(x) && File.Exists(x)).ToArray();

                    if (files.Length > 0)
                    {
                        dataObject = new DataObject(DataFormats.FileDrop, files);
                    }
                }

                if (dataObject != null)
                {
                    AllowDrop = false;
                    lvUploads.DoDragDrop(dataObject, DragDropEffects.Copy | DragDropEffects.Move);
                    AllowDrop = true;
                }
            }
        }

        #region Menu events

        private void tsmiFullscreen_Click(object sender, EventArgs e)
        {
            new CaptureFullscreen().Capture(true);
        }

        private async void tsddbCapture_DropDownOpening(object sender, EventArgs e)
        {
            await PrepareCaptureMenuAsync(tsmiWindow, tsmiWindowItems_Click, tsmiMonitor, tsmiMonitorItems_Click);
        }

        private void tsmiWindowItems_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            if (tsi.Tag is WindowInfo wi)
            {
                new CaptureWindow(wi.Handle).Capture(true);
            }
        }

        private void tsmiMonitorItems_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            Rectangle rect = (Rectangle)tsi.Tag;
            if (!rect.IsEmpty)
            {
                new CaptureMonitor(rect).Capture(true);
            }
        }

        private void tsmiRectangle_Click(object sender, EventArgs e)
        {
            new CaptureRegion().Capture(true);
        }

        private void tsmiRectangleLight_Click(object sender, EventArgs e)
        {
            new CaptureRegion(RegionCaptureType.Light).Capture(true);
        }

        private void tsmiRectangleTransparent_Click(object sender, EventArgs e)
        {
            new CaptureRegion(RegionCaptureType.Transparent).Capture(true);
        }

        private void tsmiLastRegion_Click(object sender, EventArgs e)
        {
            new CaptureLastRegion().Capture(true);
        }

        private void tsmiScreenRecordingFFmpeg_Click(object sender, EventArgs e)
        {
            TaskHelpers.StartScreenRecording(ScreenRecordOutput.FFmpeg, ScreenRecordStartMethod.Region);
        }

        private void tsmiScreenRecordingGIF_Click(object sender, EventArgs e)
        {
            TaskHelpers.StartScreenRecording(ScreenRecordOutput.GIF, ScreenRecordStartMethod.Region);
        }

        private async void tsmiScrollingCapture_Click(object sender, EventArgs e)
        {
            await TaskHelpers.OpenScrollingCapture();
        }

        private void tsmiAutoCapture_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenAutoCapture();
        }

        private void tsmiShowCursor_Click(object sender, EventArgs e)
        {
            Program.DefaultTaskSettings.CaptureSettings.ShowCursor = ((ToolStripMenuItem)sender).Checked;
            tsmiShowCursor.Checked = tsmiTrayShowCursor.Checked = Program.DefaultTaskSettings.CaptureSettings.ShowCursor;
        }

        private void tsmiScreenshotDelay0_Click(object sender, EventArgs e)
        {
            SetScreenshotDelay(0);
        }

        private void tsmiScreenshotDelay1_Click(object sender, EventArgs e)
        {
            SetScreenshotDelay(1);
        }

        private void tsmiScreenshotDelay2_Click(object sender, EventArgs e)
        {
            SetScreenshotDelay(2);
        }

        private void tsmiScreenshotDelay3_Click(object sender, EventArgs e)
        {
            SetScreenshotDelay(3);
        }

        private void tsmiScreenshotDelay4_Click(object sender, EventArgs e)
        {
            SetScreenshotDelay(4);
        }

        private void tsmiScreenshotDelay5_Click(object sender, EventArgs e)
        {
            SetScreenshotDelay(5);
        }

        private void tsbFileUpload_Click(object sender, EventArgs e)
        {
            UploadManager.UploadFile();
        }

        private void tsmiUploadFolder_Click(object sender, EventArgs e)
        {
            UploadManager.UploadFolder();
        }

        private void tsbClipboardUpload_Click(object sender, EventArgs e)
        {
            UploadManager.ClipboardUploadMainWindow();
        }

        private void tsmiUploadText_Click(object sender, EventArgs e)
        {
            UploadManager.ShowTextUploadDialog();
        }

        private void tsmiUploadURL_Click(object sender, EventArgs e)
        {
            UploadManager.UploadURL();
        }

        private void tsbDragDropUpload_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenDropWindow();
        }

        private void tsmiShortenURL_Click(object sender, EventArgs e)
        {
            UploadManager.ShowShortenURLDialog();
        }

        private void tsmiColorPicker_Click(object sender, EventArgs e)
        {
            TaskHelpers.ShowScreenColorPickerDialog();
        }

        private void tsmiScreenColorPicker_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenScreenColorPicker();
        }

        private void tsmiRuler_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenRuler();
        }

        private void tsmiPinToScreen_Click(object sender, EventArgs e)
        {
            TaskHelpers.PinToScreen();
        }

        private void tsmiImageEditor_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenImageEditor();
        }

        private void tsmiImageBeautifier_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenImageBeautifier();
        }

        private void tsmiImageEffects_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenImageEffects();
        }

        private void tsmiImageViewer_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenImageViewer();
        }

        private void tsmiImageCombiner_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenImageCombiner();
        }

        private void tsmiImageSplitter_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenImageSplitter();
        }

        private void tsmiImageThumbnailer_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenImageThumbnailer();
        }

        private void tsmiVideoConverter_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenVideoConverter();
        }

        private void tsmiVideoThumbnailer_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenVideoThumbnailer();
        }

        private async void tsmiOCR_Click(object sender, EventArgs e)
        {
            Hide();
            await Task.Delay(250);

            try
            {
                await TaskHelpers.OCRImage();
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }
            finally
            {
                this.ForceActivate();
            }
        }

        private void tsmiQRCode_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenQRCode();
        }

        private void tsmiHashChecker_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenHashCheck();
        }

        private void tsmiMetadata_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenMetadataWindow();
        }

        private void tsmiIndexFolder_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenDirectoryIndexer();
        }

        private void tsmiClipboardViewer_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenClipboardViewer();
        }

        private void tsmiBorderlessWindow_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenBorderlessWindow();
        }

        private void tsmiInspectWindow_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenInspectWindow();
        }

        private void tsmiMonitorTest_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenMonitorTest();
        }

        private void TsddbAfterCaptureTasks_DropDownOpening(object sender, EventArgs e)
        {
            UpdateImageEffectsMenu(tsddbAfterCaptureTasks);
        }

        private void TsmiTrayAfterCaptureTasks_DropDownOpening(object sender, EventArgs e)
        {
            UpdateImageEffectsMenu(tsmiTrayAfterCaptureTasks);
        }

        private void tsddbDestinations_DropDownOpened(object sender, EventArgs e)
        {
            UpdateDestinationStates();
        }

        private void tsbApplicationSettings_Click(object sender, EventArgs e)
        {
            using (ApplicationSettingsForm settingsForm = new ApplicationSettingsForm())
            {
                settingsForm.ShowDialog();
            }

            if (!IsDisposed)
            {
                AfterApplicationSettingsJobs();
                UpdateWorkflowsMenu();
                SettingManager.SaveApplicationConfigAsync();
            }
        }

        private void tsbTaskSettings_Click(object sender, EventArgs e)
        {
            using (TaskSettingsForm taskSettingsForm = new TaskSettingsForm(Program.DefaultTaskSettings, true))
            {
                taskSettingsForm.ShowDialog();
            }

            if (!IsDisposed)
            {
                AfterTaskSettingsJobs();
                SettingManager.SaveApplicationConfigAsync();
            }
        }

        private void tsbHotkeySettings_Click(object sender, EventArgs e)
        {
            if (Program.HotkeyManager != null)
            {
                using (HotkeySettingsForm hotkeySettingsForm = new HotkeySettingsForm(Program.HotkeyManager))
                {
                    hotkeySettingsForm.ShowDialog();
                }

                if (!IsDisposed)
                {
                    UpdateWorkflowsMenu();
                    SettingManager.SaveHotkeysConfigAsync();
                }
            }
        }

        private void tsbDestinationSettings_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenUploadersConfigWindow();
        }

        private void tsbCustomUploaderSettings_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenCustomUploaderSettingsWindow();
        }

        private void tsbScreenshotsFolder_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenScreenshotsFolder();
        }

        private void tsbHistory_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenHistory();
        }

        private void tsbImageHistory_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenImageHistory();
        }

        private void tsmiShowDebugLog_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenDebugLog();
        }

        private void tsmiTestImageUpload_Click(object sender, EventArgs e)
        {
            UploadManager.UploadImage(ShareXResources.Logo);
        }

        private void tsmiTestTextUpload_Click(object sender, EventArgs e)
        {
            UploadManager.UploadText(Resources.MainForm_tsmiTestTextUpload_Click_Text_upload_test);
        }

        private void tsmiTestFileUpload_Click(object sender, EventArgs e)
        {
            UploadManager.UploadImage(ShareXResources.Logo, ImageDestination.FileUploader, Program.DefaultTaskSettings.FileDestination);
        }

        private void tsmiTestURLShortener_Click(object sender, EventArgs e)
        {
            UploadManager.ShortenURL(Links.Website);
        }

        private void tsmiTestURLSharing_Click(object sender, EventArgs e)
        {
            UploadManager.ShareURL(Links.Website);
        }

        private void tsbDonate_Click(object sender, EventArgs e)
        {
#if STEAM
            URLHelpers.OpenURL(Links.Website);
#else
            URLHelpers.OpenURL(Links.Donate);
#endif
        }

        private void tsbX_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.XFollow);
        }

        private void tsbDiscord_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.Discord);
        }

        private void tsbAbout_Click(object sender, EventArgs e)
        {
            using (AboutForm aboutForm = new AboutForm())
            {
                aboutForm.ShowDialog();
            }
        }

        #endregion Menu events

        #region Tray events

        private async void niTray_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Program.Settings.TrayLeftDoubleClickAction == HotkeyType.None)
                {
                    await TaskHelpers.ExecuteJob(Program.Settings.TrayLeftClickAction);
                }
                else
                {
                    trayClickCount++;

                    if (trayClickCount == 1)
                    {
                        timerTraySingleClick.Interval = SystemInformation.DoubleClickTime;
                        timerTraySingleClick.Start();
                    }
                    else
                    {
                        trayClickCount = 0;
                        timerTraySingleClick.Stop();

                        await TaskHelpers.ExecuteJob(Program.Settings.TrayLeftDoubleClickAction);
                    }
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                await TaskHelpers.ExecuteJob(Program.Settings.TrayMiddleClickAction);
            }
        }

        private async void timerTraySingleClick_Tick(object sender, EventArgs e)
        {
            if (trayClickCount == 1)
            {
                trayClickCount = 0;
                timerTraySingleClick.Stop();

                await TaskHelpers.ExecuteJob(Program.Settings.TrayLeftClickAction);
            }
        }

        private void niTray_BalloonTipClicked(object sender, EventArgs e)
        {
            if (niTray.Tag is BalloonTipAction action)
            {
                switch (action.ClickAction)
                {
                    case BalloonTipClickAction.OpenURL:
                        URLHelpers.OpenURL(action.Text);
                        break;
                    case BalloonTipClickAction.OpenDebugLog:
                        TaskHelpers.OpenDebugLog();
                        break;
                }
            }
        }

        private void cmsTray_Opened(object sender, EventArgs e)
        {
            if (Program.Settings.TrayAutoExpandCaptureMenu)
            {
                tsmiTrayCapture.Select();
                tsmiTrayCapture.ShowDropDown();
            }
        }

        private void tsmiTrayFullscreen_Click(object sender, EventArgs e)
        {
            new CaptureFullscreen().Capture();
        }

        private async void tsmiCapture_DropDownOpening(object sender, EventArgs e)
        {
            await PrepareCaptureMenuAsync(tsmiTrayWindow, tsmiTrayWindowItems_Click, tsmiTrayMonitor, tsmiTrayMonitorItems_Click);
        }

        private void tsmiTrayWindowItems_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            if (tsi.Tag is WindowInfo wi)
            {
                new CaptureWindow(wi.Handle).Capture();
            }
        }

        private void tsmiTrayMonitorItems_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            Rectangle rect = (Rectangle)tsi.Tag;
            if (!rect.IsEmpty)
            {
                new CaptureMonitor(rect).Capture();
            }
        }

        private void tsmiTrayRectangle_Click(object sender, EventArgs e)
        {
            new CaptureRegion().Capture();
        }

        private void tsmiTrayRectangleLight_Click(object sender, EventArgs e)
        {
            new CaptureRegion(RegionCaptureType.Light).Capture();
        }

        private void tsmiTrayRectangleTransparent_Click(object sender, EventArgs e)
        {
            new CaptureRegion(RegionCaptureType.Transparent).Capture();
        }

        private void tsmiTrayLastRegion_Click(object sender, EventArgs e)
        {
            new CaptureLastRegion().Capture();
        }

        private async void tsmiTrayOCR_Click(object sender, EventArgs e)
        {
            try
            {
                await TaskHelpers.OCRImage();
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }
        }

        private void tsmiTrayToggleHotkeys_Click(object sender, EventArgs e)
        {
            TaskHelpers.ToggleHotkeys();
        }

        private void tsmiRestartAsAdmin_Click(object sender, EventArgs e)
        {
            Program.Restart(true);
        }

        private void tsmiOpenActionsToolbar_Click(object sender, EventArgs e)
        {
            TaskHelpers.ToggleActionsToolbar();
        }

        private void tsmiTrayShow_Click(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void tsmiTrayExit_MouseDown(object sender, MouseEventArgs e)
        {
            trayMenuSaveSettings = false;
        }

        private void cmsTray_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            if (trayMenuSaveSettings)
            {
                SettingManager.SaveAllSettingsAsync();
            }

            trayMenuSaveSettings = true;
        }

        private void tsmiTrayExit_Click(object sender, EventArgs e)
        {
            ForceClose();
        }

        #endregion Tray events

        #region UploadInfoMenu events

        private void tsmiShowErrors_Click(object sender, EventArgs e)
        {
            uim.ShowErrors();
        }

        private void tsmiStopUpload_Click(object sender, EventArgs e)
        {
            uim.StopUpload();
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

        private void tsmiCopyImageDimensions_Click(object sender, EventArgs e)
        {
            uim.CopyImageDimensions();
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

        private void tsmiCopyMarkdownLink_Click(object sender, EventArgs e)
        {
            uim.CopyMarkdownLink();
        }

        private void tsmiCopyMarkdownImage_Click(object sender, EventArgs e)
        {
            uim.CopyMarkdownImage();
        }

        private void tsmiCopyMarkdownLinkedImage_Click(object sender, EventArgs e)
        {
            uim.CopyMarkdownLinkedImage();
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

        private void tsmiClipboardFormat_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmiClipboardFormat = sender as ToolStripMenuItem;
            ClipboardFormat cf = tsmiClipboardFormat.Tag as ClipboardFormat;
            uim.CopyCustomFormat(cf.Format);
        }

        private void tsmiUploadSelectedFile_Click(object sender, EventArgs e)
        {
            uim.Upload();
        }

        private void tsmiDownloadSelectedURL_Click(object sender, EventArgs e)
        {
            uim.Download();
        }

        private void tsmiDeleteSelectedItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedItems();
        }

        private void tsmiDeleteSelectedFile_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.MainForm_tsmiDeleteSelectedFile_Click_Do_you_really_want_to_delete_this_file_,
                "ShareX - " + Resources.MainForm_tsmiDeleteSelectedFile_Click_File_delete_confirmation, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                uim.DeleteFiles();
                RemoveSelectedItems();
            }
        }

        private void tsmiEditSelectedFile_Click(object sender, EventArgs e)
        {
            uim.EditImage();
        }

        private void tsmiBeautifyImage_Click(object sender, EventArgs e)
        {
            uim.BeautifyImage();
        }

        private void tsmiAddImageEffects_Click(object sender, EventArgs e)
        {
            uim.AddImageEffects();
        }

        private void tsmiPinSelectedFile_Click(object sender, EventArgs e)
        {
            uim.PinToScreen();
        }

        private void tsmiGoogleLens_Click(object sender, EventArgs e)
        {
            uim.SearchImageUsingGoogleLens();
        }

        private void tsmiBingVisualSearch_Click(object sender, EventArgs e)
        {
            uim.SearchImageUsingBing();
        }

        private void tsmiShowQRCode_Click(object sender, EventArgs e)
        {
            uim.ShowQRCode();
        }

        private async void tsmiOCRImage_Click(object sender, EventArgs e)
        {
            await uim.OCRImage();
        }

        private void tsmiCombineImages_Click(object sender, EventArgs e)
        {
            uim.CombineImages();
        }

        private void tsmiCombineImagesHorizontally_Click(object sender, EventArgs e)
        {
            uim.CombineImages(Orientation.Horizontal);
        }

        private void tsmiCombineImagesVertically_Click(object sender, EventArgs e)
        {
            uim.CombineImages(Orientation.Vertical);
        }

        private void tsmiShowResponse_Click(object sender, EventArgs e)
        {
            uim.ShowResponse();
        }

        private void tsmiClearList_Click(object sender, EventArgs e)
        {
            RemoveAllItems();
            TaskManager.RecentManager.Clear();
        }

        private void TsmiSwitchTaskViewMode_Click(object sender, EventArgs e)
        {
            tsMain.SendToBack();

            if (Program.Settings.TaskViewMode == TaskViewMode.ListView)
            {
                Program.Settings.TaskViewMode = TaskViewMode.ThumbnailView;
                ucTaskThumbnailView.UpdateAllThumbnails();
            }
            else
            {
                Program.Settings.TaskViewMode = TaskViewMode.ListView;
            }

            UpdateTaskViewMode();
            UpdateMainWindowLayout();
            UpdateInfoManager();
        }

        #endregion UploadInfoMenu events

        #endregion Form events
    }
}