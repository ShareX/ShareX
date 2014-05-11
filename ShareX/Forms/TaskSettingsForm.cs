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
using ImageEffectsLib;
using ScreenCaptureLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UploadersLib;

namespace ShareX
{
    public partial class TaskSettingsForm : Form
    {
        public TaskSettings TaskSettings { get; private set; }
        public bool IsDefault { get; private set; }

        private ToolStripDropDownItem tsmiImageFileUploaders, tsmiTextFileUploaders;
        private bool loaded;

        private readonly string ConfigureEncoder = "Configure video encoders --->";

        public TaskSettingsForm(TaskSettings hotkeySetting, bool isDefault = false)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            TaskSettings = hotkeySetting;
            IsDefault = isDefault;

            if (IsDefault)
            {
                Text = Application.ProductName + " - Task settings";
                tcHotkeySettings.TabPages.Remove(tpTask);
                chkUseDefaultGeneralSettings.Visible = chkUseDefaultImageSettings.Visible = chkUseDefaultCaptureSettings.Visible = chkUseDefaultActions.Visible =
                    chkUseDefaultUploadSettings.Visible = chkUseDefaultIndexerSettings.Visible = chkUseDefaultAdvancedSettings.Visible = false;
                panelGeneral.BorderStyle = BorderStyle.None;
            }
            else
            {
                Text = Application.ProductName + " - Task settings for " + TaskSettings;
                tbDescription.Text = TaskSettings.Description;
                cbUseDefaultAfterCaptureSettings.Checked = TaskSettings.UseDefaultAfterCaptureJob;
                cbUseDefaultAfterUploadSettings.Checked = TaskSettings.UseDefaultAfterUploadJob;
                cbUseDefaultDestinationSettings.Checked = TaskSettings.UseDefaultDestinations;
                chkUseDefaultGeneralSettings.Checked = TaskSettings.UseDefaultGeneralSettings;
                chkUseDefaultImageSettings.Checked = TaskSettings.UseDefaultImageSettings;
                chkUseDefaultCaptureSettings.Checked = TaskSettings.UseDefaultCaptureSettings;
                chkUseDefaultActions.Checked = TaskSettings.UseDefaultActions;
                chkUseDefaultUploadSettings.Checked = TaskSettings.UseDefaultUploadSettings;
                chkUseDefaultIndexerSettings.Checked = TaskSettings.UseDefaultIndexerSettings;
                chkUseDefaultAdvancedSettings.Checked = TaskSettings.UseDefaultAdvancedSettings;
            }

            AddEnumItemsContextMenu<HotkeyType>(x => TaskSettings.Job = x, cmsTask);
            AddMultiEnumItemsContextMenu<AfterCaptureTasks>(x => TaskSettings.AfterCaptureJob = TaskSettings.AfterCaptureJob.Swap(x), cmsAfterCapture);
            AddMultiEnumItemsContextMenu<AfterUploadTasks>(x => TaskSettings.AfterUploadJob = TaskSettings.AfterUploadJob.Swap(x), cmsAfterUpload);
            AddEnumItems<ImageDestination>(x => TaskSettings.ImageDestination = x, tsmiImageUploaders);
            tsmiImageFileUploaders = (ToolStripDropDownItem)tsmiImageUploaders.DropDownItems[tsmiImageUploaders.DropDownItems.Count - 1];
            AddEnumItems<FileDestination>(x => TaskSettings.ImageFileDestination = x, tsmiImageFileUploaders);
            AddEnumItems<TextDestination>(x => TaskSettings.TextDestination = x, tsmiTextUploaders);
            tsmiTextFileUploaders = (ToolStripDropDownItem)tsmiTextUploaders.DropDownItems[tsmiTextUploaders.DropDownItems.Count - 1];
            AddEnumItems<FileDestination>(x => TaskSettings.TextFileDestination = x, tsmiTextFileUploaders);
            AddEnumItems<FileDestination>(x => TaskSettings.FileDestination = x, tsmiFileUploaders);
            AddEnumItems<UrlShortenerType>(x => TaskSettings.URLShortenerDestination = x, tsmiURLShorteners);
            AddEnumItems<SocialNetworkingService>(x => TaskSettings.SocialNetworkingServiceDestination = x, tsmiSocialServices);

            SetEnumCheckedContextMenu(TaskSettings.Job, cmsTask);
            SetMultiEnumCheckedContextMenu(TaskSettings.AfterCaptureJob, cmsAfterCapture);
            SetMultiEnumCheckedContextMenu(TaskSettings.AfterUploadJob, cmsAfterUpload);
            SetEnumChecked(TaskSettings.ImageDestination, tsmiImageUploaders);
            SetEnumChecked(TaskSettings.ImageFileDestination, tsmiImageFileUploaders);
            SetEnumChecked(TaskSettings.TextDestination, tsmiTextUploaders);
            SetEnumChecked(TaskSettings.TextFileDestination, tsmiTextFileUploaders);
            SetEnumChecked(TaskSettings.FileDestination, tsmiFileUploaders);
            SetEnumChecked(TaskSettings.URLShortenerDestination, tsmiURLShorteners);
            SetEnumChecked(TaskSettings.SocialNetworkingServiceDestination, tsmiSocialServices);

            // FTP
            if (Program.UploadersConfig != null && Program.UploadersConfig.FTPAccountList.Count > 1)
            {
                chkOverrideFTP.Checked = TaskSettings.OverrideFTP;
                cboFTPaccounts.Items.Clear();
                cboFTPaccounts.Items.AddRange(Program.UploadersConfig.FTPAccountList.ToArray());
                cboFTPaccounts.SelectedIndex = TaskSettings.FTPIndex.BetweenOrDefault(0, Program.UploadersConfig.FTPAccountList.Count - 1);
            }

            UpdateDestinationStates();
            UpdateUploaderMenuNames();

            // General
            cbPlaySoundAfterCapture.Checked = TaskSettings.GeneralSettings.PlaySoundAfterCapture;
            cbShowAfterCaptureTasksForm.Checked = TaskSettings.GeneralSettings.ShowAfterCaptureTasksForm;
            cbPlaySoundAfterUpload.Checked = TaskSettings.GeneralSettings.PlaySoundAfterUpload;
            chkShowAfterUploadForm.Checked = TaskSettings.GeneralSettings.ShowAfterUploadForm;
            cboPopUpNotification.Items.Clear();
            cboPopUpNotification.Items.AddRange(Helpers.GetEnumDescriptions<PopUpNotificationType>());
            cboPopUpNotification.SelectedIndex = (int)TaskSettings.GeneralSettings.PopUpNotification;
            cbHistorySave.Checked = TaskSettings.GeneralSettings.SaveHistory;

            // Image - General
            cbImageFormat.SelectedIndex = (int)TaskSettings.ImageSettings.ImageFormat;
            nudImageJPEGQuality.Value = TaskSettings.ImageSettings.ImageJPEGQuality;
            cbImageGIFQuality.SelectedIndex = (int)TaskSettings.ImageSettings.ImageGIFQuality;
            nudUseImageFormat2After.Value = TaskSettings.ImageSettings.ImageSizeLimit;
            cbImageFormat2.SelectedIndex = (int)TaskSettings.ImageSettings.ImageFormat2;
            cbImageFileExist.Items.Clear();
            cbImageFileExist.Items.AddRange(Helpers.GetEnumDescriptions<FileExistAction>());
            cbImageFileExist.SelectedIndex = (int)TaskSettings.ImageSettings.FileExistAction;

            // Image - Effects
            chkShowImageEffectsWindowAfterCapture.Checked = TaskSettings.ImageSettings.ShowImageEffectsWindowAfterCapture;
            cbImageEffectOnlyRegionCapture.Checked = TaskSettings.ImageSettings.ImageEffectOnlyRegionCapture;

            // Image - Thumbnail
            nudThumbnailWidth.Value = TaskSettings.ImageSettings.ThumbnailWidth;
            nudThumbnailHeight.Value = TaskSettings.ImageSettings.ThumbnailHeight;
            txtThumbnailName.Text = TaskSettings.ImageSettings.ThumbnailName;
            lblThumbnailNamePreview.Text = "ImageName" + TaskSettings.ImageSettings.ThumbnailName + ".jpg";
            cbThumbnailIfSmaller.Checked = TaskSettings.ImageSettings.ThumbnailCheckSize;

            // Capture
            cbShowCursor.Checked = TaskSettings.CaptureSettings.ShowCursor;
            cbCaptureTransparent.Checked = TaskSettings.CaptureSettings.CaptureTransparent;
            cbCaptureShadow.Enabled = TaskSettings.CaptureSettings.CaptureTransparent;
            cbCaptureShadow.Checked = TaskSettings.CaptureSettings.CaptureShadow;
            nudCaptureShadowOffset.Value = TaskSettings.CaptureSettings.CaptureShadowOffset;
            cbCaptureClientArea.Checked = TaskSettings.CaptureSettings.CaptureClientArea;
            cbScreenshotDelay.Checked = TaskSettings.CaptureSettings.IsDelayScreenshot;
            nudScreenshotDelay.Value = TaskSettings.CaptureSettings.DelayScreenshot;
            cbCaptureAutoHideTaskbar.Checked = TaskSettings.CaptureSettings.CaptureAutoHideTaskbar;

            if (TaskSettings.CaptureSettings.SurfaceOptions == null) TaskSettings.CaptureSettings.SurfaceOptions = new SurfaceOptions();
            pgShapesCapture.SelectedObject = TaskSettings.CaptureSettings.SurfaceOptions;

            // Capture / Screen recorder
            cbScreenRecorderOutput.Items.AddRange(Helpers.GetEnumDescriptions<ScreenRecordOutput>());
            cbScreenRecorderOutput.SelectedIndex = (int)TaskSettings.CaptureSettings.ScreenRecordOutput;
            chkRunScreencastCLI.Checked = TaskSettings.CaptureSettings.RunScreencastCLI;
            UpdateVideoEncoders();

            nudScreenRecorderFPS.Value = TaskSettings.CaptureSettings.ScreenRecordFPS;
            cbScreenRecorderFixedDuration.Checked = TaskSettings.CaptureSettings.ScreenRecordFixedDuration;
            nudScreenRecorderDuration.Enabled = TaskSettings.CaptureSettings.ScreenRecordFixedDuration;
            nudScreenRecorderDuration.Value = (decimal)TaskSettings.CaptureSettings.ScreenRecordDuration;
            nudScreenRecorderStartDelay.Value = (decimal)TaskSettings.CaptureSettings.ScreenRecordStartDelay;

            // Actions
            TaskHelpers.AddDefaultExternalPrograms(TaskSettings);
            TaskSettings.ExternalPrograms.ForEach(x => AddFileAction(x));

            // Watch folders
            cbWatchFolderEnabled.Checked = TaskSettings.WatchFolderEnabled;

            if (TaskSettings.WatchFolderList == null)
            {
                TaskSettings.WatchFolderList = new List<WatchFolderSettings>();
            }
            else
            {
                foreach (WatchFolderSettings watchFolder in TaskSettings.WatchFolderList)
                {
                    AddWatchFolder(watchFolder);
                }
            }

            // Upload / Name pattern
            txtNameFormatPattern.Text = TaskSettings.UploadSettings.NameFormatPattern;
            txtNameFormatPatternActiveWindow.Text = TaskSettings.UploadSettings.NameFormatPatternActiveWindow;
            NameParser.CreateCodesMenu(txtNameFormatPattern, ReplacementVariables.n);
            NameParser.CreateCodesMenu(txtNameFormatPatternActiveWindow, ReplacementVariables.n);
            cbFileUploadUseNamePattern.Checked = TaskSettings.UploadSettings.FileUploadUseNamePattern;

            // Upload / Clipboard upload
            chkClipboardUploadContents.Checked = TaskSettings.UploadSettings.ClipboardUploadURLContents;
            cbClipboardUploadAutoDetectURL.Checked = TaskSettings.UploadSettings.ClipboardUploadShortenURL;
            cbClipboardUploadAutoIndexFolder.Checked = TaskSettings.UploadSettings.ClipboardUploadAutoIndexFolder;

            // Indexer
            pgIndexerConfig.SelectedObject = TaskSettings.IndexerSettings;

            // Advanced
            pgTaskSettings.SelectedObject = TaskSettings.AdvancedSettings;

            UpdateDefaultSettingVisibility();
            loaded = true;
        }

        private void UpdateVideoEncoders()
        {
            cboEncoder.Items.Clear();
            if (Program.Settings.VideoEncoders.Count > 0)
            {
                Program.Settings.VideoEncoders.ForEach(x => cboEncoder.Items.Add(x));
                cboEncoder.SelectedIndex = TaskSettings.CaptureSettings.VideoEncoderSelected.BetweenOrDefault(0, Program.Settings.VideoEncoders.Count - 1);
            }
            else if (!cboEncoder.Items.Contains(ConfigureEncoder))
            {
                cboEncoder.Items.Add(ConfigureEncoder);
            }
        }

        private void UpdateDefaultSettingVisibility()
        {
            if (!IsDefault)
            {
                panelGeneral.Enabled = !TaskSettings.UseDefaultGeneralSettings;
                tcImage.Enabled = !TaskSettings.UseDefaultImageSettings;
                tcCapture.Enabled = !TaskSettings.UseDefaultCaptureSettings;
                pActions.Enabled = !TaskSettings.UseDefaultActions;
                tcUpload.Enabled = !TaskSettings.UseDefaultUploadSettings;
                pgIndexerConfig.Enabled = !TaskSettings.UseDefaultIndexerSettings;
                pgTaskSettings.Enabled = !TaskSettings.UseDefaultAdvancedSettings;
            }
        }

        private void TaskSettingsForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        #region Task

        private void UpdateDestinationStates()
        {
            if (Program.UploadersConfig != null)
            {
                EnableDisableToolStripMenuItems<ImageDestination>(tsmiImageUploaders);
                EnableDisableToolStripMenuItems<FileDestination>(tsmiImageFileUploaders);
                EnableDisableToolStripMenuItems<TextDestination>(tsmiTextUploaders);
                EnableDisableToolStripMenuItems<FileDestination>(tsmiTextFileUploaders);
                EnableDisableToolStripMenuItems<FileDestination>(tsmiFileUploaders);
                EnableDisableToolStripMenuItems<UrlShortenerType>(tsmiURLShorteners);
                EnableDisableToolStripMenuItems<SocialNetworkingService>(tsmiSocialServices);
                chkOverrideFTP.Visible = cboFTPaccounts.Visible = Program.UploadersConfig.FTPAccountList.Count > 1;
            }
        }

        private void AddEnumItemsContextMenu<T>(Action<T> selectedEnum, params ToolStripDropDown[] parents)
        {
            string[] enums = Helpers.GetEnumDescriptions<T>().Select(x => x.Replace("&", "&&")).ToArray();

            foreach (ToolStripDropDown parent in parents)
            {
                for (int i = 0; i < enums.Length; i++)
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(enums[i]);

                    int index = i;

                    tsmi.Click += (sender, e) =>
                    {
                        foreach (ToolStripDropDown parent2 in parents)
                        {
                            for (int i2 = 0; i2 < enums.Length; i2++)
                            {
                                ToolStripMenuItem tsmi2 = (ToolStripMenuItem)parent2.Items[i2];
                                tsmi2.Checked = index == i2;
                            }
                        }

                        selectedEnum((T)Enum.ToObject(typeof(T), index));

                        UpdateUploaderMenuNames();
                    };

                    parent.Items.Add(tsmi);
                }
            }
        }

        private void SetEnumCheckedContextMenu(Enum value, params ToolStripDropDown[] parents)
        {
            int index = value.GetIndex();

            foreach (ToolStripDropDown parent in parents)
            {
                ((ToolStripMenuItem)parent.Items[index]).Checked = true;
            }
        }

        private void AddMultiEnumItemsContextMenu<T>(Action<T> selectedEnum, params ToolStripDropDown[] parents)
        {
            string[] enums = Enum.GetValues(typeof(T)).Cast<Enum>().Skip(1).Select(x => x.GetDescription().Replace("&", "&&")).ToArray();

            foreach (ToolStripDropDown parent in parents)
            {
                for (int i = 0; i < enums.Length; i++)
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(enums[i]);

                    int index = i;

                    tsmi.Click += (sender, e) =>
                    {
                        foreach (ToolStripDropDown parent2 in parents)
                        {
                            ToolStripMenuItem tsmi2 = (ToolStripMenuItem)parent2.Items[index];
                            tsmi2.Checked = !tsmi2.Checked;
                        }

                        selectedEnum((T)Enum.ToObject(typeof(T), 1 << index));

                        UpdateUploaderMenuNames();
                    };

                    parent.Items.Add(tsmi);
                }
            }
        }

        private void SetMultiEnumCheckedContextMenu(Enum value, params ToolStripDropDown[] parents)
        {
            for (int i = 0; i < parents[0].Items.Count; i++)
            {
                foreach (ToolStripDropDown parent in parents)
                {
                    ToolStripMenuItem tsmi = (ToolStripMenuItem)parent.Items[i];
                    tsmi.Checked = value.HasFlag(1 << i);
                }
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

        private void UpdateUploaderMenuNames()
        {
            btnTask.Text = "Task: " + TaskSettings.Job.GetDescription();

            btnAfterCapture.Text = "After capture: " + string.Join(", ", TaskSettings.AfterCaptureJob.GetFlags<AfterCaptureTasks>().
                Select(x => x.GetDescription()).ToArray());

            btnAfterUpload.Text = "After upload: " + string.Join(", ", TaskSettings.AfterUploadJob.GetFlags<AfterUploadTasks>().
                Select(x => x.GetDescription()).ToArray());

            string imageUploader = TaskSettings.ImageDestination == ImageDestination.FileUploader ?
                TaskSettings.ImageFileDestination.GetDescription() : TaskSettings.ImageDestination.GetDescription();
            tsmiImageUploaders.Text = "Image uploader: " + imageUploader;

            string textUploader = TaskSettings.TextDestination == TextDestination.FileUploader ?
                TaskSettings.TextFileDestination.GetDescription() : TaskSettings.TextDestination.GetDescription();
            tsmiTextUploaders.Text = "Text uploader: " + textUploader;

            tsmiFileUploaders.Text = "File uploader: " + TaskSettings.FileDestination.GetDescription();

            tsmiURLShorteners.Text = "URL shortener: " + TaskSettings.URLShortenerDestination.GetDescription();

            tsmiSocialServices.Text = "Social networking service: " + TaskSettings.SocialNetworkingServiceDestination.GetDescription();
        }

        private void tbDescription_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.Description = tbDescription.Text;
        }

        private void cbUseDefaultAfterCaptureSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultAfterCaptureJob = cbUseDefaultAfterCaptureSettings.Checked;
            btnAfterCapture.Enabled = !TaskSettings.UseDefaultAfterCaptureJob;
        }

        private void cbUseDefaultAfterUploadSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultAfterUploadJob = cbUseDefaultAfterUploadSettings.Checked;
            btnAfterUpload.Enabled = !TaskSettings.UseDefaultAfterUploadJob;
        }

        private void cbUseDefaultDestinationSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultDestinations = cbUseDefaultDestinationSettings.Checked;
            btnDestinations.Enabled = !TaskSettings.UseDefaultDestinations;
        }

        private void chkOverrideFTP_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.OverrideFTP = chkOverrideFTP.Checked;
            cboFTPaccounts.Enabled = TaskSettings.OverrideFTP;
        }

        private void cboFTPaccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.FTPIndex = cboFTPaccounts.SelectedIndex;
        }

        #endregion Task

        #region General

        private void chkUseDefaultGeneralSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultGeneralSettings = chkUseDefaultGeneralSettings.Checked;
            UpdateDefaultSettingVisibility();
        }

        private void cbPlaySoundAfterCapture_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.PlaySoundAfterCapture = cbPlaySoundAfterCapture.Checked;
        }

        private void cbShowAfterCaptureTasksForm_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.ShowAfterCaptureTasksForm = cbShowAfterCaptureTasksForm.Checked;
        }

        private void cbPlaySoundAfterUpload_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.PlaySoundAfterUpload = cbPlaySoundAfterUpload.Checked;
        }

        private void chkShowAfterUploadForm_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.ShowAfterUploadForm = chkShowAfterUploadForm.Checked;
        }

        private void cboPopUpNotification_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.PopUpNotification = (PopUpNotificationType)cboPopUpNotification.SelectedIndex;
        }

        private void cbHistorySave_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.SaveHistory = cbHistorySave.Checked;
        }

        #endregion General

        #region Image

        private void chkUseDefaultImageSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultImageSettings = chkUseDefaultImageSettings.Checked;
            UpdateDefaultSettingVisibility();
        }

        private void cbImageFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ImageFormat = (EImageFormat)cbImageFormat.SelectedIndex;
        }

        private void cbImageGIFQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ImageGIFQuality = (GIFQuality)cbImageGIFQuality.SelectedIndex;
        }

        private void cbImageFormat2_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ImageFormat2 = (EImageFormat)cbImageFormat2.SelectedIndex;
        }

        private void nudImageJPEGQuality_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ImageJPEGQuality = (int)nudImageJPEGQuality.Value;
        }

        private void nudUseImageFormat2After_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ImageSizeLimit = (int)nudUseImageFormat2After.Value;
        }

        private void cbImageFileExist_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.FileExistAction = (FileExistAction)cbImageFileExist.SelectedIndex;
        }

        private void btnWatermarkSettings_Click(object sender, EventArgs e)
        {
            using (WatermarkForm watermarkForm = new WatermarkForm(TaskSettings.ImageSettings.WatermarkConfig) { Icon = Icon })
            {
                watermarkForm.ShowDialog();
            }
        }

        private void cbImageEffectOnlyRegionCapture_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ImageEffectOnlyRegionCapture = cbImageEffectOnlyRegionCapture.Checked;
        }

        private void chkShowImageEffectsWindowAfterCapture_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ShowImageEffectsWindowAfterCapture = chkShowImageEffectsWindowAfterCapture.Checked;
        }

        private void btnImageEffects_Click(object sender, EventArgs e)
        {
            using (ImageEffectsForm imageEffectsForm = new ImageEffectsForm(ShareXResources.Logo, TaskSettings.ImageSettings.ImageEffects))
            {
                if (imageEffectsForm.ShowDialog() == DialogResult.OK)
                {
                    TaskSettings.ImageSettings.ImageEffects = imageEffectsForm.Effects;
                }
            }
        }

        private void nudThumbnailWidth_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ThumbnailWidth = (int)nudThumbnailWidth.Value;
        }

        private void nudThumbnailHeight_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ThumbnailHeight = (int)nudThumbnailHeight.Value;
        }

        private void txtThumbnailName_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ThumbnailName = txtThumbnailName.Text;
            lblThumbnailNamePreview.Text = "ImageName" + TaskSettings.ImageSettings.ThumbnailName + ".jpg";
        }

        private void cbThumbnailIfSmaller_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ThumbnailCheckSize = cbThumbnailIfSmaller.Checked;
        }

        #endregion Image

        #region Capture

        private void chkUseDefaultCaptureSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultCaptureSettings = chkUseDefaultCaptureSettings.Checked;
            UpdateDefaultSettingVisibility();
        }

        private void cbCaptureAutoHideTaskbar_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureAutoHideTaskbar = cbCaptureAutoHideTaskbar.Checked;
        }

        private void nudScreenshotDelay_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.DelayScreenshot = nudScreenshotDelay.Value;
        }

        private void cbScreenshotDelay_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.IsDelayScreenshot = cbScreenshotDelay.Checked;
        }

        private void nudCaptureShadowOffset_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureShadowOffset = (int)nudCaptureShadowOffset.Value;
        }

        private void cbCaptureClientArea_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureClientArea = cbCaptureClientArea.Checked;
        }

        private void cbCaptureShadow_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureShadow = cbCaptureShadow.Checked;
        }

        private void cbShowCursor_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ShowCursor = cbShowCursor.Checked;
        }

        private void cbCaptureTransparent_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureTransparent = cbCaptureTransparent.Checked;
            cbCaptureShadow.Enabled = TaskSettings.CaptureSettings.CaptureTransparent;
        }

        #endregion Capture

        #region Screen recorder

        private void cbScreenRecorderOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenRecordOutput = (ScreenRecordOutput)cbScreenRecorderOutput.SelectedIndex;
            btnScreenRecorderOptions.Enabled = TaskSettings.CaptureSettings.ScreenRecordOutput == ScreenRecordOutput.AVI ||
                TaskSettings.CaptureSettings.ScreenRecordOutput == ScreenRecordOutput.FFmpeg;
            btnEncoderConfig.Enabled = cboEncoder.Enabled = chkRunScreencastCLI.Enabled && chkRunScreencastCLI.Checked;
        }

        private void btnScreenRecorderOptions_Click(object sender, EventArgs e)
        {
            ScreencastOptions options = new ScreencastOptions
            {
                AVI = TaskSettings.CaptureSettings.AVIOptions,
                FFmpeg = TaskSettings.CaptureSettings.FFmpegOptions,
                ShowAVIOptionsDialog = true,
                FPS = TaskSettings.CaptureSettings.ScreenRecordFPS,
                OutputPath = Path.Combine(TaskSettings.CaptureFolder, TaskHelpers.GetFilename(TaskSettings, "avi")),
                ParentWindow = this.Handle,
                CaptureArea = new Rectangle(0, 0, 100, 100)
            };

            switch (TaskSettings.CaptureSettings.ScreenRecordOutput)
            {
                case ScreenRecordOutput.AVI:

                    try
                    {
                        // Ugly workaround for show AVI compression dialog
                        using (AVICache aviCache = new AVICache(options))
                        {
                            TaskSettings.CaptureSettings.AVIOptions.CompressOptions = options.AVI.CompressOptions;
                        }
                    }
                    catch (Exception ex)
                    {
                        TaskSettings.CaptureSettings.AVIOptions.CompressOptions = new AVICOMPRESSOPTIONS();
                        MessageBox.Show(ex.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                case ScreenRecordOutput.FFmpeg:
                    using (FFmpegOptionsForm form = new FFmpegOptionsForm(options))
                    {
                        form.DefaultToolsPath = Path.Combine(Program.ToolsFolder, "ffmpeg.exe");
                        form.ShowDialog();
                    }
                    break;
            }
        }

        private void cboEncoder_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.VideoEncoderSelected = cboEncoder.SelectedIndex;
        }

        private void btnEncoderConfig_Click(object sender, EventArgs e)
        {
            using (ApplicationSettingsForm form = new ApplicationSettingsForm())
            {
                form.SelectProfilesTab();
                form.ShowDialog();
                UpdateVideoEncoders();
            }
        }

        private void nudScreenRecorderFPS_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenRecordFPS = (int)nudScreenRecorderFPS.Value;
        }

        private void cbScreenRecorderFixedDuration_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenRecordFixedDuration = cbScreenRecorderFixedDuration.Checked;
            nudScreenRecorderDuration.Enabled = TaskSettings.CaptureSettings.ScreenRecordFixedDuration;
        }

        private void chkRunScreencastCLI_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.RunScreencastCLI = btnEncoderConfig.Enabled = cboEncoder.Enabled = chkRunScreencastCLI.Checked;
        }

        private void nudScreenRecorderDuration_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenRecordDuration = (float)nudScreenRecorderDuration.Value;
        }

        private void nudScreenRecorderStartDelay_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenRecordStartDelay = (float)nudScreenRecorderStartDelay.Value;
        }

        #endregion Screen recorder

        #region Actions

        private void chkUseDefaultActions_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultActions = chkUseDefaultActions.Checked;
            UpdateDefaultSettingVisibility();
        }

        private void btnActionsAdd_Click(object sender, EventArgs e)
        {
            using (ExternalProgramForm form = new ExternalProgramForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ExternalProgram fileAction = form.FileAction;
                    fileAction.IsActive = true;
                    TaskSettings.ExternalPrograms.Add(fileAction);
                    AddFileAction(fileAction);
                }
            }
        }

        private void AddFileAction(ExternalProgram fileAction)
        {
            ListViewItem lvi = new ListViewItem(fileAction.Name ?? "");
            lvi.Tag = fileAction;
            lvi.Checked = fileAction.IsActive;
            lvi.SubItems.Add(fileAction.Path ?? "");
            lvi.SubItems.Add(fileAction.Args ?? "");
            lvActions.Items.Add(lvi);
        }

        private void btnActionsEdit_Click(object sender, EventArgs e)
        {
            if (lvActions.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvActions.SelectedItems[0];
                ExternalProgram fileAction = lvi.Tag as ExternalProgram;

                using (ExternalProgramForm form = new ExternalProgramForm(fileAction))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        lvi.Text = fileAction.Name ?? "";
                        lvi.SubItems[1].Text = fileAction.Path ?? "";
                        lvi.SubItems[2].Text = fileAction.Args ?? "";
                    }
                }
            }
        }

        private void btnActionsRemove_Click(object sender, EventArgs e)
        {
            if (lvActions.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvActions.SelectedItems[0];
                ExternalProgram fileAction = lvi.Tag as ExternalProgram;

                TaskSettings.ExternalPrograms.Remove(fileAction);
                lvActions.Items.Remove(lvi);
            }
        }

        private void lvActions_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ExternalProgram fileAction = e.Item.Tag as ExternalProgram;
            fileAction.IsActive = e.Item.Checked;
        }

        #endregion Actions

        #region Watch folders

        private void cbWatchFolderEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                TaskSettings.WatchFolderEnabled = cbWatchFolderEnabled.Checked;

                foreach (WatchFolderSettings watchFolderSetting in TaskSettings.WatchFolderList)
                {
                    Program.WatchFolderManager.UpdateWatchFolderState(watchFolderSetting);
                }
            }
        }

        private void btnWatchFolderAdd_Click(object sender, EventArgs e)
        {
            using (WatchFolderForm form = new WatchFolderForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    AddWatchFolder(form.WatchFolder);
                }
            }
        }

        private void lvWatchFolderList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lvWatchFolderList.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvWatchFolderList.SelectedItems[0];
                WatchFolderSettings watchFolder = lvi.Tag as WatchFolderSettings;

                using (WatchFolderForm form = new WatchFolderForm(watchFolder))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        lvi.Text = watchFolder.FolderPath ?? "";
                        lvi.SubItems[1].Text = watchFolder.Filter ?? "";
                        lvi.SubItems[2].Text = watchFolder.IncludeSubdirectories.ToString() ?? "";
                    }
                }
            }
        }

        private void AddWatchFolder(WatchFolderSettings watchFolderSetting)
        {
            if (watchFolderSetting != null)
            {
                Program.WatchFolderManager.AddWatchFolder(watchFolderSetting, TaskSettings);

                ListViewItem lvi = new ListViewItem(watchFolderSetting.FolderPath ?? "");
                lvi.Tag = watchFolderSetting;
                lvi.SubItems.Add(watchFolderSetting.Filter ?? "");
                lvi.SubItems.Add(watchFolderSetting.IncludeSubdirectories.ToString());
                lvWatchFolderList.Items.Add(lvi);
            }
        }

        private void btnWatchFolderRemove_Click(object sender, EventArgs e)
        {
            if (lvWatchFolderList.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvWatchFolderList.SelectedItems[0];
                WatchFolderSettings watchFolderSetting = lvi.Tag as WatchFolderSettings;
                Program.WatchFolderManager.RemoveWatchFolder(watchFolderSetting);
                lvWatchFolderList.Items.Remove(lvi);
            }
        }

        #endregion Watch folders

        #region Upload

        private void chkUseDefaultUploadSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultUploadSettings = chkUseDefaultUploadSettings.Checked;
            UpdateDefaultSettingVisibility();
        }

        private void cbFileUploadUseNamePattern_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.FileUploadUseNamePattern = cbFileUploadUseNamePattern.Checked;
        }

        private void txtNameFormatPatternActiveWindow_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.NameFormatPatternActiveWindow = txtNameFormatPatternActiveWindow.Text;
            NameParser nameParser = new NameParser(NameParserType.FileName)
            {
                AutoIncrementNumber = Program.Settings.NameParserAutoIncrementNumber,
                WindowText = Text,
                ProcessName = "ShareX",
                MaxNameLength = TaskSettings.AdvancedSettings.NamePatternMaxLength,
                MaxTitleLength = TaskSettings.AdvancedSettings.NamePatternMaxTitleLength
            };
            lblNameFormatPatternPreviewActiveWindow.Text = "Preview: " + nameParser.Parse(TaskSettings.UploadSettings.NameFormatPatternActiveWindow);
        }

        private void btnResetAutoIncrementNumber_Click(object sender, EventArgs e)
        {
            Program.Settings.NameParserAutoIncrementNumber = 0;
        }

        private void txtNameFormatPattern_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.NameFormatPattern = txtNameFormatPattern.Text;
            NameParser nameParser = new NameParser(NameParserType.FileName)
            {
                AutoIncrementNumber = Program.Settings.NameParserAutoIncrementNumber,
                MaxNameLength = TaskSettings.AdvancedSettings.NamePatternMaxLength,
                MaxTitleLength = TaskSettings.AdvancedSettings.NamePatternMaxTitleLength
            };
            lblNameFormatPatternPreview.Text = "Preview: " + nameParser.Parse(TaskSettings.UploadSettings.NameFormatPattern);
        }

        private void chkClipboardUploadContents_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.ClipboardUploadURLContents = chkClipboardUploadContents.Checked;
        }

        private void cbClipboardUploadAutoDetectURL_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.ClipboardUploadShortenURL = cbClipboardUploadAutoDetectURL.Checked;
        }

        private void cbClipboardUploadAutoIndexFolder_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.ClipboardUploadAutoIndexFolder = cbClipboardUploadAutoIndexFolder.Checked;
        }

        #endregion Upload

        #region Indexer

        private void chkUseDefaultIndexerSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultIndexerSettings = chkUseDefaultIndexerSettings.Checked;
            UpdateDefaultSettingVisibility();
        }

        #endregion Indexer

        #region Advanced

        private void chkUseDefaultAdvancedSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultAdvancedSettings = chkUseDefaultAdvancedSettings.Checked;
            UpdateDefaultSettingVisibility();
        }

        #endregion Advanced
    }
}