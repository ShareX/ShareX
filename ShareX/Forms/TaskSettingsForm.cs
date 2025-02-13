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
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX
{
    public partial class TaskSettingsForm : Form
    {
        public TaskSettings TaskSettings { get; private set; }
        public bool IsDefault { get; private set; }

        private ToolStripDropDownItem tsmiImageFileUploaders, tsmiTextFileUploaders;
        private bool loaded;

        public TaskSettingsForm(TaskSettings hotkeySetting, bool isDefault = false)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            tsmiURLShorteners.Image = ShareXResources.IsDarkTheme ? Resources.edit_scale_white : Resources.edit_scale;

            TaskSettings = hotkeySetting;
            IsDefault = isDefault;

            UpdateWindowTitle();

            if (IsDefault)
            {
                tcTaskSettings.TabPages.Remove(tpTask);
                cbOverrideGeneralSettings.Visible = cbOverrideImageSettings.Visible = cbOverrideCaptureSettings.Visible = cbOverrideActions.Visible =
                    cbOverrideUploadSettings.Visible = cbOverrideToolsSettings.Visible = cbOverrideAdvancedSettings.Visible = false;
            }
            else
            {
                #region Task

                AddEnumItemsContextMenu<HotkeyType>(x =>
                {
                    TaskSettings.Job = x;
                    UpdateWindowTitle();
                }, cmsTask);
                SetEnumCheckedContextMenu(TaskSettings.Job, cmsTask);

                tbDescription.Text = TaskSettings.Description;

                cbOverrideAfterCaptureSettings.Checked = !TaskSettings.UseDefaultAfterCaptureJob;
                btnAfterCapture.Enabled = !TaskSettings.UseDefaultAfterCaptureJob;
                AddMultiEnumItemsContextMenu<AfterCaptureTasks>(x => TaskSettings.AfterCaptureJob = TaskSettings.AfterCaptureJob.Swap(x), cmsAfterCapture);
                SetMultiEnumCheckedContextMenu(TaskSettings.AfterCaptureJob, cmsAfterCapture);

                cbOverrideAfterUploadSettings.Checked = !TaskSettings.UseDefaultAfterUploadJob;
                btnAfterUpload.Enabled = !TaskSettings.UseDefaultAfterUploadJob;
                AddMultiEnumItemsContextMenu<AfterUploadTasks>(x => TaskSettings.AfterUploadJob = TaskSettings.AfterUploadJob.Swap(x), cmsAfterUpload);
                SetMultiEnumCheckedContextMenu(TaskSettings.AfterUploadJob, cmsAfterUpload);

                cbOverrideDestinationSettings.Checked = !TaskSettings.UseDefaultDestinations;
                btnDestinations.Enabled = !TaskSettings.UseDefaultDestinations;
                AddEnumItems<ImageDestination>(x =>
                {
                    TaskSettings.ImageDestination = x;

                    if (x == ImageDestination.FileUploader)
                    {
                        SetEnumChecked(TaskSettings.ImageFileDestination, tsmiImageFileUploaders);
                    }
                    else
                    {
                        MainForm.Uncheck(tsmiImageFileUploaders);
                    }
                }, tsmiImageUploaders);
                tsmiImageFileUploaders = (ToolStripDropDownItem)tsmiImageUploaders.DropDownItems[tsmiImageUploaders.DropDownItems.Count - 1];
                AddEnumItems<FileDestination>(x =>
                {
                    TaskSettings.ImageFileDestination = x;
                    tsmiImageFileUploaders.PerformClick();
                }, tsmiImageFileUploaders);
                SetEnumChecked(TaskSettings.ImageDestination, tsmiImageUploaders);
                MainForm.SetImageFileDestinationChecked(TaskSettings.ImageDestination, TaskSettings.ImageFileDestination, tsmiImageFileUploaders);
                AddEnumItems<TextDestination>(x =>
                {
                    TaskSettings.TextDestination = x;

                    if (x == TextDestination.FileUploader)
                    {
                        SetEnumChecked(TaskSettings.TextFileDestination, tsmiTextFileUploaders);
                    }
                    else
                    {
                        MainForm.Uncheck(tsmiTextFileUploaders);
                    }
                }, tsmiTextUploaders);
                tsmiTextFileUploaders = (ToolStripDropDownItem)tsmiTextUploaders.DropDownItems[tsmiTextUploaders.DropDownItems.Count - 1];
                AddEnumItems<FileDestination>(x =>
                {
                    TaskSettings.TextFileDestination = x;
                    tsmiTextFileUploaders.PerformClick();
                }, tsmiTextFileUploaders);
                SetEnumChecked(TaskSettings.TextDestination, tsmiTextUploaders);
                MainForm.SetTextFileDestinationChecked(TaskSettings.TextDestination, TaskSettings.TextFileDestination, tsmiTextFileUploaders);
                AddEnumItems<FileDestination>(x => TaskSettings.FileDestination = x, tsmiFileUploaders);
                SetEnumChecked(TaskSettings.FileDestination, tsmiFileUploaders);
                AddEnumItems<UrlShortenerType>(x => TaskSettings.URLShortenerDestination = x, tsmiURLShorteners);
                SetEnumChecked(TaskSettings.URLShortenerDestination, tsmiURLShorteners);
                AddEnumItems<URLSharingServices>(x => TaskSettings.URLSharingServiceDestination = x, tsmiURLSharingServices);
                SetEnumChecked(TaskSettings.URLSharingServiceDestination, tsmiURLSharingServices);
                UpdateDestinationStates();

                if (Program.UploadersConfig != null)
                {
                    cbOverrideFTPAccount.Enabled = cbFTPAccounts.Enabled = Program.UploadersConfig.FTPAccountList.Count > 0;

                    if (Program.UploadersConfig.FTPAccountList.Count > 0)
                    {
                        cbOverrideFTPAccount.Checked = TaskSettings.OverrideFTP;
                        cbFTPAccounts.Enabled = TaskSettings.OverrideFTP;
                        cbFTPAccounts.Items.Clear();
                        cbFTPAccounts.Items.AddRange(Program.UploadersConfig.FTPAccountList.ToArray());
                        cbFTPAccounts.SelectedIndex = TaskSettings.FTPIndex.BetweenOrDefault(0, Program.UploadersConfig.FTPAccountList.Count - 1);
                    }

                    cbOverrideCustomUploader.Enabled = cbCustomUploaders.Enabled = Program.UploadersConfig.CustomUploadersList.Count > 0;

                    if (Program.UploadersConfig.CustomUploadersList.Count > 0)
                    {
                        cbOverrideCustomUploader.Checked = TaskSettings.OverrideCustomUploader;
                        cbCustomUploaders.Enabled = TaskSettings.OverrideCustomUploader;
                        cbCustomUploaders.Items.Clear();
                        cbCustomUploaders.Items.AddRange(Program.UploadersConfig.CustomUploadersList.ToArray());
                        cbCustomUploaders.SelectedIndex = TaskSettings.CustomUploaderIndex.BetweenOrDefault(0, Program.UploadersConfig.CustomUploadersList.Count - 1);
                    }
                }

                cbOverrideScreenshotsFolder.Checked = TaskSettings.OverrideScreenshotsFolder;
                CodeMenu screenshotsFolderMenu = CodeMenu.Create<CodeMenuEntryFilename>(txtScreenshotsFolder, CodeMenuEntryFilename.t, CodeMenuEntryFilename.pn,
                    CodeMenuEntryFilename.i, CodeMenuEntryFilename.width, CodeMenuEntryFilename.height, CodeMenuEntryFilename.n);
                screenshotsFolderMenu.MenuLocationBottom = true;
                txtScreenshotsFolder.Text = TaskSettings.ScreenshotsFolder;
                txtScreenshotsFolder.Enabled = btnScreenshotsFolderBrowse.Enabled = TaskSettings.OverrideScreenshotsFolder;

                UpdateTaskTabMenuNames();

                #endregion Task

                cbOverrideGeneralSettings.Checked = !TaskSettings.UseDefaultGeneralSettings;
                cbOverrideImageSettings.Checked = !TaskSettings.UseDefaultImageSettings;
                cbOverrideCaptureSettings.Checked = !TaskSettings.UseDefaultCaptureSettings;
                cbOverrideActions.Checked = !TaskSettings.UseDefaultActions;
                cbOverrideUploadSettings.Checked = !TaskSettings.UseDefaultUploadSettings;
                cbOverrideToolsSettings.Checked = !TaskSettings.UseDefaultToolsSettings;
                cbOverrideAdvancedSettings.Checked = !TaskSettings.UseDefaultAdvancedSettings;
            }

            UpdateDefaultSettingVisibility();

            tttvMain.MainTabControl = tcTaskSettings;

            #region General

            #region Notifications

            cbPlaySoundAfterCapture.Checked = TaskSettings.GeneralSettings.PlaySoundAfterCapture;
            cbPlaySoundAfterUpload.Checked = TaskSettings.GeneralSettings.PlaySoundAfterUpload;
            cbPlaySoundAfterAction.Checked = TaskSettings.GeneralSettings.PlaySoundAfterAction;
            cbShowToastNotificationAfterTaskCompleted.Checked = TaskSettings.GeneralSettings.ShowToastNotificationAfterTaskCompleted;
            gbToastWindow.Enabled = TaskSettings.GeneralSettings.ShowToastNotificationAfterTaskCompleted;
            nudToastWindowDuration.SetValue((decimal)TaskSettings.GeneralSettings.ToastWindowDuration);
            nudToastWindowFadeDuration.SetValue((decimal)TaskSettings.GeneralSettings.ToastWindowFadeDuration);
            cbToastWindowPlacement.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<ContentAlignment>());
            cbToastWindowPlacement.SelectedIndex = TaskSettings.GeneralSettings.ToastWindowPlacement.GetIndex();
            nudToastWindowSizeWidth.SetValue(TaskSettings.GeneralSettings.ToastWindowSize.Width);
            nudToastWindowSizeHeight.SetValue(TaskSettings.GeneralSettings.ToastWindowSize.Height);
            cbToastWindowLeftClickAction.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<ToastClickAction>());
            cbToastWindowLeftClickAction.SelectedIndex = (int)TaskSettings.GeneralSettings.ToastWindowLeftClickAction;
            cbToastWindowRightClickAction.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<ToastClickAction>());
            cbToastWindowRightClickAction.SelectedIndex = (int)TaskSettings.GeneralSettings.ToastWindowRightClickAction;
            cbToastWindowMiddleClickAction.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<ToastClickAction>());
            cbToastWindowMiddleClickAction.SelectedIndex = (int)TaskSettings.GeneralSettings.ToastWindowMiddleClickAction;
            cbToastWindowAutoHide.Checked = TaskSettings.GeneralSettings.ToastWindowAutoHide;
            cbDisableNotificationsOnFullscreen.Checked = TaskSettings.GeneralSettings.DisableNotificationsOnFullscreen;
            cbUseCustomCaptureSound.Checked = TaskSettings.GeneralSettings.UseCustomCaptureSound;
            txtCustomCaptureSoundPath.Enabled = btnCustomCaptureSoundPath.Enabled = TaskSettings.GeneralSettings.UseCustomCaptureSound;
            txtCustomCaptureSoundPath.Text = TaskSettings.GeneralSettings.CustomCaptureSoundPath;
            cbUseCustomTaskCompletedSound.Checked = TaskSettings.GeneralSettings.UseCustomTaskCompletedSound;
            txtCustomTaskCompletedSoundPath.Enabled = btnCustomTaskCompletedSoundPath.Enabled = TaskSettings.GeneralSettings.UseCustomTaskCompletedSound;
            txtCustomTaskCompletedSoundPath.Text = TaskSettings.GeneralSettings.CustomTaskCompletedSoundPath;
            cbUseCustomActionCompletedSound.Checked = TaskSettings.GeneralSettings.UseCustomActionCompletedSound;
            txtCustomActionCompletedSoundPath.Enabled = btnCustomActionCompletedSoundPath.Enabled = TaskSettings.GeneralSettings.UseCustomActionCompletedSound;
            txtCustomActionCompletedSoundPath.Text = TaskSettings.GeneralSettings.CustomActionCompletedSoundPath;
            cbUseCustomErrorSound.Checked = TaskSettings.GeneralSettings.UseCustomErrorSound;
            txtCustomErrorSoundPath.Enabled = btnCustomErrorSoundPath.Enabled = TaskSettings.GeneralSettings.UseCustomErrorSound;
            txtCustomErrorSoundPath.Text = TaskSettings.GeneralSettings.CustomErrorSoundPath;

            #endregion

            #endregion General

            #region Image

            #region General

            cbImageFormat.Items.AddRange(Enum.GetNames(typeof(EImageFormat)));
            cbImageFormat.SelectedIndex = (int)TaskSettings.ImageSettings.ImageFormat;
            cbImagePNGBitDepth.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<PNGBitDepth>());
            cbImagePNGBitDepth.SelectedIndex = (int)TaskSettings.ImageSettings.ImagePNGBitDepth;
            nudImageJPEGQuality.SetValue(TaskSettings.ImageSettings.ImageJPEGQuality);
            cbImageGIFQuality.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<GIFQuality>());
            cbImageGIFQuality.SelectedIndex = (int)TaskSettings.ImageSettings.ImageGIFQuality;
            cbImageAutoUseJPEG.Checked = TaskSettings.ImageSettings.ImageAutoUseJPEG;
            nudImageAutoUseJPEGSize.Enabled = TaskSettings.ImageSettings.ImageAutoUseJPEG;
            cbImageAutoJPEGQuality.Enabled = TaskSettings.ImageSettings.ImageAutoUseJPEG;
            nudImageAutoUseJPEGSize.SetValue(TaskSettings.ImageSettings.ImageAutoUseJPEGSize);
            cbImageAutoJPEGQuality.Checked = TaskSettings.ImageSettings.ImageAutoJPEGQuality;
            cbImageFileExist.Items.Clear();
            cbImageFileExist.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<FileExistAction>());
            cbImageFileExist.SelectedIndex = (int)TaskSettings.ImageSettings.FileExistAction;

            #endregion General

            #region Effects

            cbShowImageEffectsWindowAfterCapture.Checked = TaskSettings.ImageSettings.ShowImageEffectsWindowAfterCapture;
            cbImageEffectOnlyRegionCapture.Checked = TaskSettings.ImageSettings.ImageEffectOnlyRegionCapture;
            cbUseRandomImageEffect.Checked = TaskSettings.ImageSettings.UseRandomImageEffect;

            #endregion Effects

            #region Thumbnail

            nudThumbnailWidth.SetValue(TaskSettings.ImageSettings.ThumbnailWidth);
            nudThumbnailHeight.SetValue(TaskSettings.ImageSettings.ThumbnailHeight);
            txtThumbnailName.Text = TaskSettings.ImageSettings.ThumbnailName;
            lblThumbnailNamePreview.Text = "ImageName" + TaskSettings.ImageSettings.ThumbnailName + ".jpg";
            cbThumbnailIfSmaller.Checked = TaskSettings.ImageSettings.ThumbnailCheckSize;

            #endregion Thumbnail

            #endregion Image

            #region Capture

            #region General

            cbShowCursor.Checked = TaskSettings.CaptureSettings.ShowCursor;
            nudScreenshotDelay.SetValue(TaskSettings.CaptureSettings.ScreenshotDelay);
            cbCaptureTransparent.Checked = TaskSettings.CaptureSettings.CaptureTransparent;
            cbCaptureShadow.Enabled = TaskSettings.CaptureSettings.CaptureTransparent;
            cbCaptureShadow.Checked = TaskSettings.CaptureSettings.CaptureShadow;
            nudCaptureShadowOffset.SetValue(TaskSettings.CaptureSettings.CaptureShadowOffset);
            cbCaptureClientArea.Checked = TaskSettings.CaptureSettings.CaptureClientArea;
            cbCaptureAutoHideTaskbar.Checked = TaskSettings.CaptureSettings.CaptureAutoHideTaskbar;
            nudCaptureCustomRegionX.SetValue(TaskSettings.CaptureSettings.CaptureCustomRegion.X);
            nudCaptureCustomRegionY.SetValue(TaskSettings.CaptureSettings.CaptureCustomRegion.Y);
            nudCaptureCustomRegionWidth.SetValue(TaskSettings.CaptureSettings.CaptureCustomRegion.Width);
            nudCaptureCustomRegionHeight.SetValue(TaskSettings.CaptureSettings.CaptureCustomRegion.Height);
            txtCaptureCustomWindow.Text = TaskSettings.CaptureSettings.CaptureCustomWindow;

            #endregion General

            #region Region capture

            cbRegionCaptureMultiRegionMode.Checked = !TaskSettings.CaptureSettings.SurfaceOptions.QuickCrop;
            cbRegionCaptureMouseRightClickAction.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<RegionCaptureAction>());
            cbRegionCaptureMouseRightClickAction.SelectedIndex = (int)TaskSettings.CaptureSettings.SurfaceOptions.RegionCaptureActionRightClick;
            cbRegionCaptureMouseMiddleClickAction.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<RegionCaptureAction>());
            cbRegionCaptureMouseMiddleClickAction.SelectedIndex = (int)TaskSettings.CaptureSettings.SurfaceOptions.RegionCaptureActionMiddleClick;
            cbRegionCaptureMouse4ClickAction.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<RegionCaptureAction>());
            cbRegionCaptureMouse4ClickAction.SelectedIndex = (int)TaskSettings.CaptureSettings.SurfaceOptions.RegionCaptureActionX1Click;
            cbRegionCaptureMouse5ClickAction.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<RegionCaptureAction>());
            cbRegionCaptureMouse5ClickAction.SelectedIndex = (int)TaskSettings.CaptureSettings.SurfaceOptions.RegionCaptureActionX2Click;
            cbRegionCaptureDetectWindows.Checked = TaskSettings.CaptureSettings.SurfaceOptions.DetectWindows;
            cbRegionCaptureDetectControls.Enabled = TaskSettings.CaptureSettings.SurfaceOptions.DetectWindows;
            cbRegionCaptureDetectControls.Checked = TaskSettings.CaptureSettings.SurfaceOptions.DetectControls;
            nudRegionCaptureBackgroundDimStrength.SetValue(TaskSettings.CaptureSettings.SurfaceOptions.BackgroundDimStrength);
            cbRegionCaptureUseCustomInfoText.Checked = TaskSettings.CaptureSettings.SurfaceOptions.UseCustomInfoText;
            txtRegionCaptureCustomInfoText.Enabled = TaskSettings.CaptureSettings.SurfaceOptions.UseCustomInfoText;
            TaskSettings.CaptureSettings.SurfaceOptions.CustomInfoText = TaskSettings.CaptureSettings.SurfaceOptions.CustomInfoText.Replace("\r\n", "$n").Replace("\n", "$n");
            CodeMenu.Create<CodeMenuEntryPixelInfo>(txtRegionCaptureCustomInfoText);
            txtRegionCaptureCustomInfoText.Text = TaskSettings.CaptureSettings.SurfaceOptions.CustomInfoText;
            cbRegionCaptureSnapSizes.Items.AddRange(TaskSettings.CaptureSettings.SurfaceOptions.SnapSizes.ToArray());
            cbRegionCaptureShowInfo.Checked = TaskSettings.CaptureSettings.SurfaceOptions.ShowInfo;
            cbRegionCaptureShowMagnifier.Checked = TaskSettings.CaptureSettings.SurfaceOptions.ShowMagnifier;
            cbRegionCaptureUseSquareMagnifier.Enabled = nudRegionCaptureMagnifierPixelCount.Enabled = nudRegionCaptureMagnifierPixelSize.Enabled = TaskSettings.CaptureSettings.SurfaceOptions.ShowMagnifier;
            cbRegionCaptureUseSquareMagnifier.Checked = TaskSettings.CaptureSettings.SurfaceOptions.UseSquareMagnifier;
            nudRegionCaptureMagnifierPixelCount.Minimum = RegionCaptureOptions.MagnifierPixelCountMinimum;
            nudRegionCaptureMagnifierPixelCount.Maximum = RegionCaptureOptions.MagnifierPixelCountMaximum;
            nudRegionCaptureMagnifierPixelCount.SetValue(TaskSettings.CaptureSettings.SurfaceOptions.MagnifierPixelCount);
            nudRegionCaptureMagnifierPixelSize.Minimum = RegionCaptureOptions.MagnifierPixelSizeMinimum;
            nudRegionCaptureMagnifierPixelSize.Maximum = RegionCaptureOptions.MagnifierPixelSizeMaximum;
            nudRegionCaptureMagnifierPixelSize.SetValue(TaskSettings.CaptureSettings.SurfaceOptions.MagnifierPixelSize);
            cbRegionCaptureShowCrosshair.Checked = TaskSettings.CaptureSettings.SurfaceOptions.ShowCrosshair;
            cbRegionCaptureIsFixedSize.Checked = TaskSettings.CaptureSettings.SurfaceOptions.IsFixedSize;
            nudRegionCaptureFixedSizeWidth.Enabled = nudRegionCaptureFixedSizeHeight.Enabled = TaskSettings.CaptureSettings.SurfaceOptions.IsFixedSize;
            nudRegionCaptureFixedSizeWidth.SetValue(TaskSettings.CaptureSettings.SurfaceOptions.FixedSize.Width);
            nudRegionCaptureFixedSizeHeight.SetValue(TaskSettings.CaptureSettings.SurfaceOptions.FixedSize.Height);
            cbRegionCaptureShowFPS.Checked = TaskSettings.CaptureSettings.SurfaceOptions.ShowFPS;
            nudRegionCaptureFPSLimit.SetValue(TaskSettings.CaptureSettings.SurfaceOptions.FPSLimit);
            cbRegionCaptureActiveMonitorMode.Checked = TaskSettings.CaptureSettings.SurfaceOptions.ActiveMonitorMode;

            #endregion Region capture

            #region Screen recorder

            if (HelpersOptions.DevMode)
            {
                nudScreenRecordFPS.Maximum = 300;
                nudGIFFPS.Maximum = 60;
            }

            nudScreenRecordFPS.SetValue(TaskSettings.CaptureSettings.ScreenRecordFPS);
            nudGIFFPS.SetValue(TaskSettings.CaptureSettings.GIFFPS);
            cbScreenRecorderFixedDuration.Checked = nudScreenRecorderDuration.Enabled = TaskSettings.CaptureSettings.ScreenRecordFixedDuration;
            nudScreenRecorderDuration.SetValue((decimal)TaskSettings.CaptureSettings.ScreenRecordDuration);
            cbScreenRecordAutoStart.Checked = nudScreenRecorderStartDelay.Enabled = TaskSettings.CaptureSettings.ScreenRecordAutoStart;
            nudScreenRecorderStartDelay.SetValue((decimal)TaskSettings.CaptureSettings.ScreenRecordStartDelay);
            cbScreenRecorderShowCursor.Checked = TaskSettings.CaptureSettings.ScreenRecordShowCursor;
            cbScreenRecordTwoPassEncoding.Checked = TaskSettings.CaptureSettings.ScreenRecordTwoPassEncoding;
            cbScreenRecordTransparentRegion.Checked = TaskSettings.CaptureSettings.ScreenRecordTransparentRegion;
            cbScreenRecordConfirmAbort.Checked = TaskSettings.CaptureSettings.ScreenRecordAskConfirmationOnAbort;

            #endregion Screen recorder

            #region OCR

            OCROptions ocrOptions = TaskSettings.CaptureSettings.OCROptions;

            try
            {
                OCRLanguage[] languages = OCRHelper.AvailableLanguages.OrderBy(x => x.DisplayName).ToArray();

                if (languages.Length > 0)
                {
                    cbCaptureOCRDefaultLanguage.Items.AddRange(languages);

                    if (ocrOptions.Language == null)
                    {
                        cbCaptureOCRDefaultLanguage.SelectedIndex = 0;
                        ocrOptions.Language = languages[0].LanguageTag;
                    }
                    else
                    {
                        int index = Array.FindIndex(languages, x => x.LanguageTag.Equals(ocrOptions.Language, StringComparison.OrdinalIgnoreCase));

                        if (index >= 0)
                        {
                            cbCaptureOCRDefaultLanguage.SelectedIndex = index;
                        }
                        else
                        {
                            cbCaptureOCRDefaultLanguage.SelectedIndex = 0;
                            ocrOptions.Language = languages[0].LanguageTag;
                        }
                    }
                }
            }
            catch
            {
                cbCaptureOCRDefaultLanguage.Enabled = false;
            }

            cbCaptureOCRSilent.Checked = ocrOptions.Silent;
            cbCaptureOCRAutoCopy.Enabled = !ocrOptions.Silent;
            cbCaptureOCRAutoCopy.Checked = ocrOptions.AutoCopy;
            cbCloseWindowAfterOpenServiceLink.Checked = ocrOptions.CloseWindowAfterOpeningServiceLink;

            #endregion OCR

            #endregion Capture

            #region Upload

            #region File naming

            txtNameFormatPattern.Text = TaskSettings.UploadSettings.NameFormatPattern;
            txtNameFormatPatternActiveWindow.Text = TaskSettings.UploadSettings.NameFormatPatternActiveWindow;
            CodeMenu.Create<CodeMenuEntryFilename>(txtNameFormatPattern, CodeMenuEntryFilename.n, CodeMenuEntryFilename.t, CodeMenuEntryFilename.pn);
            CodeMenu.Create<CodeMenuEntryFilename>(txtNameFormatPatternActiveWindow, CodeMenuEntryFilename.n);
            cbFileUploadUseNamePattern.Checked = TaskSettings.UploadSettings.FileUploadUseNamePattern;
            nudAutoIncrementNumber.SetValue(Program.Settings.NameParserAutoIncrementNumber);
            UpdateNameFormatPreviews();
            cbNameFormatCustomTimeZone.Checked = cbNameFormatTimeZone.Enabled = TaskSettings.UploadSettings.UseCustomTimeZone;
            cbNameFormatTimeZone.Items.AddRange(TimeZoneInfo.GetSystemTimeZones().ToArray());
            for (int i = 0; i < cbNameFormatTimeZone.Items.Count; i++)
            {
                if (cbNameFormatTimeZone.Items[i].Equals(TaskSettings.UploadSettings.CustomTimeZone))
                {
                    cbNameFormatTimeZone.SelectedIndex = i;
                    break;
                }
            }
            cbFileUploadReplaceProblematicCharacters.Checked = TaskSettings.UploadSettings.FileUploadReplaceProblematicCharacters;
            cbURLRegexReplace.Checked = TaskSettings.UploadSettings.URLRegexReplace;
            lblURLRegexReplacePattern.Enabled = txtURLRegexReplacePattern.Enabled =
                lblURLRegexReplaceReplacement.Enabled = txtURLRegexReplaceReplacement.Enabled = TaskSettings.UploadSettings.URLRegexReplace;
            txtURLRegexReplacePattern.Text = TaskSettings.UploadSettings.URLRegexReplacePattern;
            txtURLRegexReplaceReplacement.Text = TaskSettings.UploadSettings.URLRegexReplaceReplacement;

            #endregion File naming

            #region Clipboard upload

            cbClipboardUploadURLContents.Checked = TaskSettings.UploadSettings.ClipboardUploadURLContents;
            cbClipboardUploadShortenURL.Checked = TaskSettings.UploadSettings.ClipboardUploadShortenURL;
            cbClipboardUploadShareURL.Checked = TaskSettings.UploadSettings.ClipboardUploadShareURL;
            cbClipboardUploadAutoIndexFolder.Checked = TaskSettings.UploadSettings.ClipboardUploadAutoIndexFolder;

            #endregion Clipboard upload

            #region Uploader filters

            cbUploaderFiltersDestination.Items.AddRange(UploaderFactory.AllGenericUploaderServices.OrderBy(x => x.ServiceName).ToArray());

            if (TaskSettings.UploadSettings.UploaderFilters == null) TaskSettings.UploadSettings.UploaderFilters = new List<UploaderFilter>();

            foreach (UploaderFilter filter in TaskSettings.UploadSettings.UploaderFilters)
            {
                AddUploaderFilterToList(filter);
            }

            #endregion Uploader filters

            #endregion Upload

            #region Actions

            TaskHelpers.AddDefaultExternalPrograms(TaskSettings);
            TaskSettings.ExternalPrograms.ForEach(AddFileAction);

            #endregion Actions

            #region Watch folders

            cbWatchFolderEnabled.Checked = TaskSettings.WatchFolderEnabled;

            if (TaskSettings.WatchFolderList == null)
            {
                TaskSettings.WatchFolderList = new List<WatchFolderSettings>();
            }
            else
            {
                foreach (WatchFolderSettings watchFolder in TaskSettings.WatchFolderList)
                {
                    WatchFolderAdd(watchFolder);
                }
            }

            #endregion Watch folders

            #region Tools

            CodeMenu.Create<CodeMenuEntryPixelInfo>(txtToolsScreenColorPickerFormat);
            txtToolsScreenColorPickerFormat.Text = TaskSettings.ToolsSettings.ScreenColorPickerFormat;

            CodeMenu.Create<CodeMenuEntryPixelInfo>(txtToolsScreenColorPickerFormatCtrl);
            txtToolsScreenColorPickerFormatCtrl.Text = TaskSettings.ToolsSettings.ScreenColorPickerFormatCtrl;

            CodeMenu.Create<CodeMenuEntryPixelInfo>(txtToolsScreenColorPickerInfoText);
            txtToolsScreenColorPickerInfoText.Text = TaskSettings.ToolsSettings.ScreenColorPickerInfoText;

            #endregion Tools

            #region Advanced

            pgTaskSettings.SelectedObject = TaskSettings.AdvancedSettings;

            #endregion Advanced

            loaded = true;
        }

        private void TaskSettingsForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void tttvMain_TabChanged(TabPage tabPage)
        {
            if (IsDefault && (tabPage == tpGeneralMain || tabPage == tpUploadMain))
            {
                tttvMain.SelectChildNode();
            }
        }

        private void UpdateWindowTitle()
        {
            if (IsDefault)
            {
                Text = "ShareX - " + Resources.TaskSettingsForm_UpdateWindowTitle_Task_settings;
            }
            else
            {
                Text = "ShareX - " + string.Format(Resources.TaskSettingsForm_UpdateWindowTitle_Task_settings_for__0_, TaskSettings);
            }
        }

        private void UpdateDefaultSettingVisibility()
        {
            if (!IsDefault)
            {
                tpNotifications.Enabled = !TaskSettings.UseDefaultGeneralSettings;
                pImage.Enabled = tpEffects.Enabled = tpThumbnail.Enabled = !TaskSettings.UseDefaultImageSettings;
                pCapture.Enabled = tpRegionCapture.Enabled = tpScreenRecorder.Enabled = tpOCR.Enabled = !TaskSettings.UseDefaultCaptureSettings;
                pActions.Enabled = !TaskSettings.UseDefaultActions;
                tpFileNaming.Enabled = tpUploadClipboard.Enabled = tpUploaderFilters.Enabled = !TaskSettings.UseDefaultUploadSettings;
                pTools.Enabled = !TaskSettings.UseDefaultToolsSettings;
                pgTaskSettings.Enabled = !TaskSettings.UseDefaultAdvancedSettings;
            }
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
                EnableDisableToolStripMenuItems<URLSharingServices>(tsmiURLSharingServices);
            }
        }

        private void AddEnumItemsContextMenu<T>(Action<T> selectedEnum, params ToolStripDropDown[] parents) where T : Enum
        {
            EnumInfo[] enums = Helpers.GetEnums<T>().OfType<Enum>().Select(x => new EnumInfo(x)).ToArray();

            foreach (ToolStripDropDown parent in parents)
            {
                foreach (EnumInfo enumInfo in enums)
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(enumInfo.Description.Replace("&", "&&"));
                    tsmi.Image = TaskHelpers.FindMenuIcon(enumInfo.Value);
                    tsmi.Tag = enumInfo;

                    tsmi.Click += (sender, e) =>
                    {
                        SetEnumCheckedContextMenu(enumInfo, parents);

                        selectedEnum((T)enumInfo.Value);

                        UpdateTaskTabMenuNames();
                    };

                    if (!string.IsNullOrEmpty(enumInfo.Category))
                    {
                        ToolStripMenuItem tsmiParent = parent.Items.OfType<ToolStripMenuItem>().FirstOrDefault(x => x.Text == enumInfo.Category);

                        if (tsmiParent == null)
                        {
                            tsmiParent = new ToolStripMenuItem(enumInfo.Category);
                            parent.Items.Add(tsmiParent);
                        }

                        tsmiParent.DropDownItems.Add(tsmi);
                    }
                    else
                    {
                        parent.Items.Add(tsmi);
                    }
                }
            }
        }

        private void SetEnumCheckedContextMenu(Enum value, params ToolStripDropDown[] parents)
        {
            SetEnumCheckedContextMenu(new EnumInfo(value), parents);
        }

        private void SetEnumCheckedContextMenu(EnumInfo enumInfo, params ToolStripDropDown[] parents)
        {
            foreach (ToolStripDropDown parent in parents)
            {
                foreach (ToolStripMenuItem tsmiParent in parent.Items)
                {
                    EnumInfo currentEnumInfo;

                    if (tsmiParent.DropDownItems.Count > 0)
                    {
                        foreach (ToolStripMenuItem tsmiCategoryParent in tsmiParent.DropDownItems)
                        {
                            currentEnumInfo = (EnumInfo)tsmiCategoryParent.Tag;
                            tsmiCategoryParent.Checked = currentEnumInfo.Value.Equals(enumInfo.Value);
                        }
                    }
                    else
                    {
                        currentEnumInfo = (EnumInfo)tsmiParent.Tag;
                        tsmiParent.Checked = currentEnumInfo.Value.Equals(enumInfo.Value);
                    }
                }
            }
        }

        private void AddMultiEnumItemsContextMenu<T>(Action<T> selectedEnum, params ToolStripDropDown[] parents) where T : Enum
        {
            string[] enums = Helpers.GetLocalizedEnumDescriptions<T>().Skip(1).Select(x => x.Replace("&", "&&")).ToArray();

            foreach (ToolStripDropDown parent in parents)
            {
                for (int i = 0; i < enums.Length; i++)
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(enums[i]);
                    tsmi.Image = TaskHelpers.FindMenuIcon<T>(i + 1);

                    int index = i;

                    tsmi.Click += (sender, e) =>
                    {
                        foreach (ToolStripDropDown parent2 in parents)
                        {
                            ToolStripMenuItem tsmi2 = (ToolStripMenuItem)parent2.Items[index];
                            tsmi2.Checked = !tsmi2.Checked;
                        }

                        selectedEnum((T)Enum.ToObject(typeof(T), 1 << index));

                        UpdateTaskTabMenuNames();
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
            string[] enums = Helpers.GetLocalizedEnumDescriptions<T>();

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

                        UpdateTaskTabMenuNames();
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
                    parent.DropDownItems[i].Enabled = UploadersConfigValidator.Validate<T>(i, Program.UploadersConfig);
                }
            }
        }

        private void UpdateTaskTabMenuNames()
        {
            btnTask.Text = TaskSettings.Job.GetLocalizedDescription();
            btnTask.Image = TaskHelpers.FindMenuIcon(TaskSettings.Job);

            btnAfterCapture.Text = string.Format(Resources.TaskSettingsForm_UpdateUploaderMenuNames_After_capture___0_,
                string.Join(", ", TaskSettings.AfterCaptureJob.GetFlags().Select(x => x.GetLocalizedDescription())));

            btnAfterUpload.Text = string.Format(Resources.TaskSettingsForm_UpdateUploaderMenuNames_After_upload___0_,
                string.Join(", ", TaskSettings.AfterUploadJob.GetFlags().Select(x => x.GetLocalizedDescription())));

            string imageUploader = TaskSettings.ImageDestination == ImageDestination.FileUploader ?
                TaskSettings.ImageFileDestination.GetLocalizedDescription() : TaskSettings.ImageDestination.GetLocalizedDescription();
            tsmiImageUploaders.Text = string.Format(Resources.TaskSettingsForm_UpdateUploaderMenuNames_Image_uploader___0_, imageUploader);

            string textUploader = TaskSettings.TextDestination == TextDestination.FileUploader ?
                TaskSettings.TextFileDestination.GetLocalizedDescription() : TaskSettings.TextDestination.GetLocalizedDescription();
            tsmiTextUploaders.Text = string.Format(Resources.TaskSettingsForm_UpdateUploaderMenuNames_Text_uploader___0_, textUploader);

            tsmiFileUploaders.Text = string.Format(Resources.TaskSettingsForm_UpdateUploaderMenuNames_File_uploader___0_, TaskSettings.FileDestination.GetLocalizedDescription());

            tsmiURLShorteners.Text = string.Format(Resources.TaskSettingsForm_UpdateUploaderMenuNames_URL_shortener___0_, TaskSettings.URLShortenerDestination.GetLocalizedDescription());

            tsmiURLSharingServices.Text = string.Format(Resources.TaskSettingsForm_UpdateUploaderMenuNames_URL_sharing_service___0_, TaskSettings.URLSharingServiceDestination.GetLocalizedDescription());
        }

        private void tbDescription_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.Description = tbDescription.Text;
            UpdateWindowTitle();
        }

        private void cbUseDefaultAfterCaptureSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultAfterCaptureJob = !cbOverrideAfterCaptureSettings.Checked;
            btnAfterCapture.Enabled = !TaskSettings.UseDefaultAfterCaptureJob;
        }

        private void cbUseDefaultAfterUploadSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultAfterUploadJob = !cbOverrideAfterUploadSettings.Checked;
            btnAfterUpload.Enabled = !TaskSettings.UseDefaultAfterUploadJob;
        }

        private void cbUseDefaultDestinationSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultDestinations = !cbOverrideDestinationSettings.Checked;
            btnDestinations.Enabled = !TaskSettings.UseDefaultDestinations;
        }

        private void cbOverrideFTPAccount_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.OverrideFTP = cbOverrideFTPAccount.Checked;
            cbFTPAccounts.Enabled = TaskSettings.OverrideFTP;
        }

        private void cbFTPAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.FTPIndex = cbFTPAccounts.SelectedIndex;
        }

        private void cbOverrideCustomUploader_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.OverrideCustomUploader = cbOverrideCustomUploader.Checked;
            cbCustomUploaders.Enabled = TaskSettings.OverrideCustomUploader;
        }

        private void cbCustomUploaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.CustomUploaderIndex = cbCustomUploaders.SelectedIndex;
        }

        private void cbOverrideScreenshotsFolder_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.OverrideScreenshotsFolder = cbOverrideScreenshotsFolder.Checked;
            txtScreenshotsFolder.Enabled = btnScreenshotsFolderBrowse.Enabled = TaskSettings.OverrideScreenshotsFolder;
        }

        private void txtScreenshotsFolder_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.ScreenshotsFolder = txtScreenshotsFolder.Text;
        }

        private void btnScreenshotsFolderBrowse_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFolder(Resources.ApplicationSettingsForm_btnBrowseCustomScreenshotsPath_Click_Choose_screenshots_folder_path,
                txtScreenshotsFolder, TaskSettings.ScreenshotsFolder, true);
        }

        #endregion Task

        #region General

        private void cbUseDefaultGeneralSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultGeneralSettings = !cbOverrideGeneralSettings.Checked;
            UpdateDefaultSettingVisibility();
        }

        private void cbPlaySoundAfterCapture_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.PlaySoundAfterCapture = cbPlaySoundAfterCapture.Checked;
        }

        private void cbPlaySoundAfterUpload_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.PlaySoundAfterUpload = cbPlaySoundAfterUpload.Checked;
        }

        private void cbPlaySoundAfterAction_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.PlaySoundAfterAction = cbPlaySoundAfterAction.Checked;
        }

        private void cbShowToastNotificationAfterTaskCompleted_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.ShowToastNotificationAfterTaskCompleted = cbShowToastNotificationAfterTaskCompleted.Checked;
            gbToastWindow.Enabled = TaskSettings.GeneralSettings.ShowToastNotificationAfterTaskCompleted;
        }

        private void nudToastWindowDuration_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.ToastWindowDuration = (float)nudToastWindowDuration.Value;
        }

        private void nudToastWindowFadeDuration_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.ToastWindowFadeDuration = (float)nudToastWindowFadeDuration.Value;
        }

        private void cbToastWindowPlacement_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.ToastWindowPlacement = Helpers.GetEnumFromIndex<ContentAlignment>(cbToastWindowPlacement.SelectedIndex);
        }

        private void nudToastWindowSizeWidth_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.ToastWindowSize = new Size((int)nudToastWindowSizeWidth.Value, TaskSettings.GeneralSettings.ToastWindowSize.Height);
        }

        private void nudToastWindowSizeHeight_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.ToastWindowSize = new Size(TaskSettings.GeneralSettings.ToastWindowSize.Width, (int)nudToastWindowSizeHeight.Value);
        }

        private void cbToastWindowLeftClickAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.ToastWindowLeftClickAction = (ToastClickAction)cbToastWindowLeftClickAction.SelectedIndex;
        }

        private void cbToastWindowRightClickAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.ToastWindowRightClickAction = (ToastClickAction)cbToastWindowRightClickAction.SelectedIndex;
        }

        private void cbToastWindowMiddleClickAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.ToastWindowMiddleClickAction = (ToastClickAction)cbToastWindowMiddleClickAction.SelectedIndex;
        }

        private void cbToastWindowAutoHide_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.ToastWindowAutoHide = cbToastWindowAutoHide.Checked;
        }

        private void cbDisableNotificationsOnFullscreen_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.DisableNotificationsOnFullscreen = cbDisableNotificationsOnFullscreen.Checked;
        }

        private void cbUseCustomCaptureSound_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.UseCustomCaptureSound = cbUseCustomCaptureSound.Checked;
            txtCustomCaptureSoundPath.Enabled = btnCustomCaptureSoundPath.Enabled = TaskSettings.GeneralSettings.UseCustomCaptureSound;
        }

        private void txtCustomCaptureSoundPath_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.CustomCaptureSoundPath = txtCustomCaptureSoundPath.Text;
        }

        private void btnCustomCaptureSoundPath_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFile(txtCustomCaptureSoundPath, filter: "Audio file (*.wav)|*.wav");
        }

        private void cbUseCustomTaskCompletedSound_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.UseCustomTaskCompletedSound = cbUseCustomTaskCompletedSound.Checked;
            txtCustomTaskCompletedSoundPath.Enabled = btnCustomTaskCompletedSoundPath.Enabled = TaskSettings.GeneralSettings.UseCustomTaskCompletedSound;
        }

        private void txtCustomTaskCompletedSoundPath_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.CustomTaskCompletedSoundPath = txtCustomTaskCompletedSoundPath.Text;
        }

        private void btnCustomTaskCompletedSoundPath_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFile(txtCustomTaskCompletedSoundPath, filter: "Audio file (*.wav)|*.wav");
        }

        private void cbUseCustomActionCompletedSound_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.UseCustomActionCompletedSound = cbUseCustomActionCompletedSound.Checked;
            txtCustomActionCompletedSoundPath.Enabled = btnCustomActionCompletedSoundPath.Enabled = TaskSettings.GeneralSettings.UseCustomActionCompletedSound;
        }

        private void txtCustomActionCompletedSoundPath_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.CustomActionCompletedSoundPath = txtCustomActionCompletedSoundPath.Text;
        }

        private void btnCustomActionCompletedSoundPath_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFile(txtCustomActionCompletedSoundPath, filter: "Audio file (*.wav)|*.wav");
        }

        private void cbUseCustomErrorSound_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.UseCustomErrorSound = cbUseCustomErrorSound.Checked;
            txtCustomErrorSoundPath.Enabled = btnCustomErrorSoundPath.Enabled = TaskSettings.GeneralSettings.UseCustomErrorSound;
        }

        private void txtCustomErrorSoundPath_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.GeneralSettings.CustomErrorSoundPath = txtCustomErrorSoundPath.Text;
        }

        private void btnCustomErrorSoundPath_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFile(txtCustomErrorSoundPath, filter: "Audio file (*.wav)|*.wav");
        }

        #endregion General

        #region Image

        private void cbUseDefaultImageSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultImageSettings = !cbOverrideImageSettings.Checked;
            UpdateDefaultSettingVisibility();
        }

        private void cbImageFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ImageFormat = (EImageFormat)cbImageFormat.SelectedIndex;
        }

        private void cbImagePNGBitDepth_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ImagePNGBitDepth = (PNGBitDepth)cbImagePNGBitDepth.SelectedIndex;
        }

        private void nudImageJPEGQuality_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ImageJPEGQuality = (int)nudImageJPEGQuality.Value;
        }

        private void cbImageGIFQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ImageGIFQuality = (GIFQuality)cbImageGIFQuality.SelectedIndex;
        }

        private void cbImageAutoUseJPEG_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ImageAutoUseJPEG = cbImageAutoUseJPEG.Checked;
            nudImageAutoUseJPEGSize.Enabled = TaskSettings.ImageSettings.ImageAutoUseJPEG;
            cbImageAutoJPEGQuality.Enabled = TaskSettings.ImageSettings.ImageAutoUseJPEG;
        }

        private void nudImageAutoUseJPEGSize_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ImageAutoUseJPEGSize = (int)nudImageAutoUseJPEGSize.Value;
        }

        private void cbImageAutoJPEGQuality_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ImageAutoJPEGQuality = cbImageAutoJPEGQuality.Checked;
        }

        private void cbImageFileExist_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.FileExistAction = (FileExistAction)cbImageFileExist.SelectedIndex;
        }

        private void cbShowImageEffectsWindowAfterCapture_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ShowImageEffectsWindowAfterCapture = cbShowImageEffectsWindowAfterCapture.Checked;
        }

        private void cbImageEffectOnlyRegionCapture_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.ImageEffectOnlyRegionCapture = cbImageEffectOnlyRegionCapture.Checked;
        }

        private void cbUseRandomImageEffect_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.ImageSettings.UseRandomImageEffect = cbUseRandomImageEffect.Checked;
        }

        private void btnImageEffects_Click(object sender, EventArgs e)
        {
            TaskHelpers.OpenImageEffectsSingleton(TaskSettings);
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

        #region General

        private void cbUseDefaultCaptureSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultCaptureSettings = !cbOverrideCaptureSettings.Checked;
            UpdateDefaultSettingVisibility();
        }

        private void cbShowCursor_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ShowCursor = cbShowCursor.Checked;
        }

        private void nudScreenshotDelay_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenshotDelay = nudScreenshotDelay.Value;
        }

        private void cbCaptureTransparent_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureTransparent = cbCaptureTransparent.Checked;
            cbCaptureShadow.Enabled = TaskSettings.CaptureSettings.CaptureTransparent;
        }

        private void cbCaptureShadow_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureShadow = cbCaptureShadow.Checked;
        }

        private void nudCaptureShadowOffset_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureShadowOffset = (int)nudCaptureShadowOffset.Value;
        }

        private void cbCaptureClientArea_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureClientArea = cbCaptureClientArea.Checked;
        }

        private void cbCaptureAutoHideTaskbar_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureAutoHideTaskbar = cbCaptureAutoHideTaskbar.Checked;
        }

        private void nudScreenRegionX_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureCustomRegion.X = (int)nudCaptureCustomRegionX.Value;
        }

        private void nudScreenRegionY_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureCustomRegion.Y = (int)nudCaptureCustomRegionY.Value;
        }

        private void nudScreenRegionWidth_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureCustomRegion.Width = (int)nudCaptureCustomRegionWidth.Value;
        }

        private void nudScreenRegionHeight_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureCustomRegion.Height = (int)nudCaptureCustomRegionHeight.Value;
        }

        private void btnCaptureCustomRegionSelectRectangle_Click(object sender, EventArgs e)
        {
            if (RegionCaptureTasks.GetRectangleRegion(out Rectangle rect, TaskSettings.CaptureSettings.SurfaceOptions))
            {
                nudCaptureCustomRegionX.SetValue(rect.X);
                nudCaptureCustomRegionY.SetValue(rect.Y);
                nudCaptureCustomRegionWidth.SetValue(rect.Width);
                nudCaptureCustomRegionHeight.SetValue(rect.Height);
            }
        }

        private void txtCaptureCustomWindow_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.CaptureCustomWindow = txtCaptureCustomWindow.Text;
        }

        #endregion General

        #region Region capture

        private void cbRegionCaptureMultiRegionMode_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.QuickCrop = !cbRegionCaptureMultiRegionMode.Checked;
        }

        private void cbRegionCaptureMouseRightClickAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.RegionCaptureActionRightClick = (RegionCaptureAction)cbRegionCaptureMouseRightClickAction.SelectedIndex;
        }

        private void cbRegionCaptureMouseMiddleClickAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.RegionCaptureActionMiddleClick = (RegionCaptureAction)cbRegionCaptureMouseMiddleClickAction.SelectedIndex;
        }

        private void cbRegionCaptureMouse4ClickAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.RegionCaptureActionX1Click = (RegionCaptureAction)cbRegionCaptureMouse4ClickAction.SelectedIndex;
        }

        private void cbRegionCaptureMouse5ClickAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.RegionCaptureActionX2Click = (RegionCaptureAction)cbRegionCaptureMouse5ClickAction.SelectedIndex;
        }

        private void cbRegionCaptureDetectWindows_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.DetectWindows = cbRegionCaptureDetectWindows.Checked;
            cbRegionCaptureDetectControls.Enabled = TaskSettings.CaptureSettings.SurfaceOptions.DetectWindows;
        }

        private void cbRegionCaptureDetectControls_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.DetectControls = cbRegionCaptureDetectControls.Checked;
        }

        private void nudRegionCaptureBackgroundDimStrength_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.BackgroundDimStrength = (int)nudRegionCaptureBackgroundDimStrength.Value;
        }

        private void cbRegionCaptureUseCustomInfoText_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.UseCustomInfoText = cbRegionCaptureUseCustomInfoText.Checked;
            txtRegionCaptureCustomInfoText.Enabled = TaskSettings.CaptureSettings.SurfaceOptions.UseCustomInfoText;
        }

        private void txtRegionCaptureCustomInfoText_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.CustomInfoText = txtRegionCaptureCustomInfoText.Text;
        }

        private void btnRegionCaptureSnapSizesAdd_Click(object sender, EventArgs e)
        {
            pRegionCaptureSnapSizes.Visible = true;
        }

        private void btnRegionCaptureSnapSizesRemove_Click(object sender, EventArgs e)
        {
            int index = cbRegionCaptureSnapSizes.SelectedIndex;

            if (index > -1)
            {
                TaskSettings.CaptureSettings.SurfaceOptions.SnapSizes.RemoveAt(index);
                cbRegionCaptureSnapSizes.Items.RemoveAt(index);
                cbRegionCaptureSnapSizes.SelectedIndex = cbRegionCaptureSnapSizes.Items.Count - 1;
            }
        }

        private void btnRegionCaptureSnapSizesDialogAdd_Click(object sender, EventArgs e)
        {
            pRegionCaptureSnapSizes.Visible = false;
            SnapSize size = new SnapSize((int)nudRegionCaptureSnapSizesWidth.Value, (int)nudRegionCaptureSnapSizesHeight.Value);
            TaskSettings.CaptureSettings.SurfaceOptions.SnapSizes.Add(size);
            cbRegionCaptureSnapSizes.Items.Add(size);
            cbRegionCaptureSnapSizes.SelectedIndex = cbRegionCaptureSnapSizes.Items.Count - 1;
        }

        private void btnRegionCaptureSnapSizesDialogCancel_Click(object sender, EventArgs e)
        {
            pRegionCaptureSnapSizes.Visible = false;
        }

        private void cbRegionCaptureShowInfo_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.ShowInfo = cbRegionCaptureShowInfo.Checked;
        }

        private void cbRegionCaptureShowMagnifier_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.ShowMagnifier = cbRegionCaptureShowMagnifier.Checked;
            cbRegionCaptureUseSquareMagnifier.Enabled = nudRegionCaptureMagnifierPixelCount.Enabled = nudRegionCaptureMagnifierPixelSize.Enabled = TaskSettings.CaptureSettings.SurfaceOptions.ShowMagnifier;
        }

        private void cbRegionCaptureUseSquareMagnifier_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.UseSquareMagnifier = cbRegionCaptureUseSquareMagnifier.Checked;
        }

        private void nudRegionCaptureMagnifierPixelCount_ValueChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                TaskSettings.CaptureSettings.SurfaceOptions.MagnifierPixelCount = (int)nudRegionCaptureMagnifierPixelCount.Value;
            }
        }

        private void nudRegionCaptureMagnifierPixelSize_ValueChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                TaskSettings.CaptureSettings.SurfaceOptions.MagnifierPixelSize = (int)nudRegionCaptureMagnifierPixelSize.Value;
            }
        }

        private void cbRegionCaptureShowCrosshair_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.ShowCrosshair = cbRegionCaptureShowCrosshair.Checked;
        }

        private void cbRegionCaptureIsFixedSize_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.IsFixedSize = cbRegionCaptureIsFixedSize.Checked;
            nudRegionCaptureFixedSizeWidth.Enabled = nudRegionCaptureFixedSizeHeight.Enabled = TaskSettings.CaptureSettings.SurfaceOptions.IsFixedSize;
        }

        private void nudRegionCaptureFixedSizeWidth_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.FixedSize = new Size((int)nudRegionCaptureFixedSizeWidth.Value, TaskSettings.CaptureSettings.SurfaceOptions.FixedSize.Height);
        }

        private void nudRegionCaptureFixedSizeHeight_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.FixedSize = new Size(TaskSettings.CaptureSettings.SurfaceOptions.FixedSize.Width, (int)nudRegionCaptureFixedSizeHeight.Value);
        }

        private void cbRegionCaptureShowFPS_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.ShowFPS = cbRegionCaptureShowFPS.Checked;
        }

        private void nudRegionCaptureFPSLimit_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.FPSLimit = (int)nudRegionCaptureFPSLimit.Value;
        }

        private void cbRegionCaptureActiveMonitorMode_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.SurfaceOptions.ActiveMonitorMode = cbRegionCaptureActiveMonitorMode.Checked;
        }

        #endregion Region capture

        #region Screen recorder

        private void btnScreenRecorderFFmpegOptions_Click(object sender, EventArgs e)
        {
            ScreenRecordingOptions options = new ScreenRecordingOptions
            {
                IsRecording = true,
                FFmpeg = TaskSettings.CaptureSettings.FFmpegOptions,
                FPS = TaskSettings.CaptureSettings.ScreenRecordFPS,
                Duration = TaskSettings.CaptureSettings.ScreenRecordFixedDuration ? TaskSettings.CaptureSettings.ScreenRecordDuration : 0,
                OutputPath = "output.mp4",
                CaptureArea = Screen.PrimaryScreen.Bounds,
                DrawCursor = TaskSettings.CaptureSettings.ScreenRecordShowCursor
            };

            using (FFmpegOptionsForm form = new FFmpegOptionsForm(options))
            {
                form.ShowDialog();

                TaskSettings.CaptureSettings.FFmpegOptions = form.Options.FFmpeg;
            }
        }

        private void nudScreenRecordFPS_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenRecordFPS = (int)nudScreenRecordFPS.Value;
        }

        private void nudGIFFPS_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.GIFFPS = (int)nudGIFFPS.Value;
        }

        private void cbScreenRecorderFixedDuration_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenRecordFixedDuration = cbScreenRecorderFixedDuration.Checked;
            nudScreenRecorderDuration.Enabled = TaskSettings.CaptureSettings.ScreenRecordFixedDuration;
        }

        private void nudScreenRecorderDuration_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenRecordDuration = (float)nudScreenRecorderDuration.Value;
        }

        private void cbScreenRecordAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenRecordAutoStart = cbScreenRecordAutoStart.Checked;
            nudScreenRecorderStartDelay.Enabled = cbScreenRecordAutoStart.Checked;
        }

        private void nudScreenRecorderStartDelay_ValueChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenRecordStartDelay = (float)nudScreenRecorderStartDelay.Value;
        }

        private void cbScreenRecorderShowCursor_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenRecordShowCursor = cbScreenRecorderShowCursor.Checked;
        }

        private void cbScreenRecordTwoPassEncoding_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenRecordTwoPassEncoding = cbScreenRecordTwoPassEncoding.Checked;
        }

        private void cbScreenRecordTransparentRegion_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenRecordTransparentRegion = cbScreenRecordTransparentRegion.Checked;
        }

        private void cbScreenRecordConfirmAbort_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.ScreenRecordAskConfirmationOnAbort = cbScreenRecordConfirmAbort.Checked;
        }

        #endregion Screen recorder

        #region OCR

        private void cbCaptureOCRDefaultLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                TaskSettings.CaptureSettings.OCROptions.Language = ((OCRLanguage)cbCaptureOCRDefaultLanguage.SelectedItem).LanguageTag;
            }
        }

        private void btnCaptureOCRHelp_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.DocsOCR);
        }

        private void cbCaptureOCRSilent_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.OCROptions.Silent = cbCaptureOCRSilent.Checked;
            cbCaptureOCRAutoCopy.Enabled = !TaskSettings.CaptureSettings.OCROptions.Silent;
        }

        private void cbCaptureOCRAutoCopy_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.OCROptions.AutoCopy = cbCaptureOCRAutoCopy.Checked;
        }

        private void cbCloseWindowAfterOpenServiceLink_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.CaptureSettings.OCROptions.CloseWindowAfterOpeningServiceLink = cbCloseWindowAfterOpenServiceLink.Checked;
        }

        #endregion OCR

        #endregion Capture

        #region Upload

        private void UpdateNameFormatPreviews()
        {
            NameParser nameParser = new NameParser(NameParserType.FileName)
            {
                AutoIncrementNumber = Program.Settings.NameParserAutoIncrementNumber,
                ImageWidth = 1920,
                ImageHeight = 1080,
                MaxNameLength = TaskSettings.AdvancedSettings.NamePatternMaxLength,
                MaxTitleLength = TaskSettings.AdvancedSettings.NamePatternMaxTitleLength,
                CustomTimeZone = TaskSettings.UploadSettings.UseCustomTimeZone ? TaskSettings.UploadSettings.CustomTimeZone : null,
                IsPreviewMode = true
            };

            lblNameFormatPatternPreview.Text = Resources.TaskSettingsForm_txtNameFormatPatternActiveWindow_TextChanged_Preview_ + " " +
                nameParser.Parse(TaskSettings.UploadSettings.NameFormatPattern);

            nameParser.WindowText = Text;
            nameParser.ProcessName = "ShareX";

            lblNameFormatPatternPreviewActiveWindow.Text = Resources.TaskSettingsForm_txtNameFormatPatternActiveWindow_TextChanged_Preview_ + " " +
                nameParser.Parse(TaskSettings.UploadSettings.NameFormatPatternActiveWindow);
        }

        private void cbUseDefaultUploadSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultUploadSettings = !cbOverrideUploadSettings.Checked;
            UpdateDefaultSettingVisibility();
        }

        private void txtNameFormatPattern_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.NameFormatPattern = txtNameFormatPattern.Text;
            UpdateNameFormatPreviews();
        }

        private void txtNameFormatPatternActiveWindow_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.NameFormatPatternActiveWindow = txtNameFormatPatternActiveWindow.Text;
            UpdateNameFormatPreviews();
        }

        private void cbFileUploadUseNamePattern_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.FileUploadUseNamePattern = cbFileUploadUseNamePattern.Checked;
        }

        private void btnAutoIncrementNumber_Click(object sender, EventArgs e)
        {
            Program.Settings.NameParserAutoIncrementNumber = (int)nudAutoIncrementNumber.Value;
            UpdateNameFormatPreviews();
        }

        private void cbNameFormatCustomTimeZone_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.UseCustomTimeZone = cbNameFormatCustomTimeZone.Checked;
            cbNameFormatTimeZone.Enabled = TaskSettings.UploadSettings.UseCustomTimeZone;
            UpdateNameFormatPreviews();
        }

        private void cbNameFormatTimeZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbNameFormatTimeZone.SelectedItem is TimeZoneInfo timeZoneInfo)
            {
                TaskSettings.UploadSettings.CustomTimeZone = timeZoneInfo;
            }

            UpdateNameFormatPreviews();
        }

        private void cbFileUploadReplaceProblematicCharacters_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.FileUploadReplaceProblematicCharacters = cbFileUploadReplaceProblematicCharacters.Checked;
        }

        private void cbURLRegexReplace_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.URLRegexReplace = cbURLRegexReplace.Checked;
            lblURLRegexReplacePattern.Enabled = txtURLRegexReplacePattern.Enabled =
                lblURLRegexReplaceReplacement.Enabled = txtURLRegexReplaceReplacement.Enabled = TaskSettings.UploadSettings.URLRegexReplace;
        }

        private void txtURLRegexReplacePattern_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.URLRegexReplacePattern = txtURLRegexReplacePattern.Text;
        }

        private void txtURLRegexReplaceReplacement_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.URLRegexReplaceReplacement = txtURLRegexReplaceReplacement.Text;
        }

        private void cbClipboardUploadContents_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.ClipboardUploadURLContents = cbClipboardUploadURLContents.Checked;
        }

        private void cbClipboardUploadAutoDetectURL_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.ClipboardUploadShortenURL = cbClipboardUploadShortenURL.Checked;
        }

        private void cbClipboardUploadShareURL_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.ClipboardUploadShareURL = cbClipboardUploadShareURL.Checked;
        }

        private void cbClipboardUploadAutoIndexFolder_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UploadSettings.ClipboardUploadAutoIndexFolder = cbClipboardUploadAutoIndexFolder.Checked;
        }

        private UploaderFilter GetUploaderFilterFromFields()
        {
            if (cbUploaderFiltersDestination.SelectedItem is IGenericUploaderService service)
            {
                UploaderFilter filter = new UploaderFilter();
                filter.Uploader = service.ServiceIdentifier;
                filter.SetExtensions(txtUploaderFiltersExtensions.Text);
                return filter;
            }

            return null;
        }

        private void AddUploaderFilterToList(UploaderFilter filter)
        {
            if (filter != null)
            {
                ListViewItem lvi = new ListViewItem(filter.Uploader);
                lvi.SubItems.Add(filter.GetExtensions());
                lvi.Tag = filter;

                lvUploaderFiltersList.Items.Add(lvi);
            }
        }

        private void UpdateUploaderFilterFields(UploaderFilter filter)
        {
            if (filter == null)
            {
                filter = new UploaderFilter();
            }

            for (int i = 0; i < cbUploaderFiltersDestination.Items.Count; i++)
            {
                if (cbUploaderFiltersDestination.Items[i] is IGenericUploaderService service &&
                    service.ServiceIdentifier.Equals(filter.Uploader, StringComparison.OrdinalIgnoreCase))
                {
                    cbUploaderFiltersDestination.SelectedIndex = i;
                    break;
                }
            }

            txtUploaderFiltersExtensions.Text = filter.GetExtensions();
        }

        private void btnUploaderFiltersAdd_Click(object sender, EventArgs e)
        {
            UploaderFilter filter = GetUploaderFilterFromFields();

            if (filter != null)
            {
                TaskSettings.UploadSettings.UploaderFilters.Add(filter);

                AddUploaderFilterToList(filter);

                lvUploaderFiltersList.SelectedIndex = lvUploaderFiltersList.Items.Count - 1;
            }
        }

        private void btnUploaderFiltersUpdate_Click(object sender, EventArgs e)
        {
            int index = lvUploaderFiltersList.SelectedIndex;

            if (index > -1)
            {
                UploaderFilter filter = GetUploaderFilterFromFields();

                if (filter != null)
                {
                    TaskSettings.UploadSettings.UploaderFilters[index] = filter;

                    ListViewItem lvi = lvUploaderFiltersList.Items[index];
                    lvi.Text = filter.Uploader;
                    lvi.SubItems[1].Text = filter.GetExtensions();
                    lvi.Tag = filter;
                }
            }
        }

        private void btnUploaderFiltersRemove_Click(object sender, EventArgs e)
        {
            int index = lvUploaderFiltersList.SelectedIndex;

            if (index > -1)
            {
                TaskSettings.UploadSettings.UploaderFilters.RemoveAt(index);

                lvUploaderFiltersList.Items.RemoveAt(index);
            }
        }

        private void lvUploaderFiltersList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UploaderFilter filter = null;

            if (lvUploaderFiltersList.SelectedItems.Count > 0)
            {
                filter = lvUploaderFiltersList.SelectedItems[0].Tag as UploaderFilter;
            }

            UpdateUploaderFilterFields(filter);
        }

        #endregion Upload

        #region Actions

        private void cbUseDefaultActions_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultActions = !cbOverrideActions.Checked;
            UpdateDefaultSettingVisibility();
        }

        private void btnActionsAdd_Click(object sender, EventArgs e)
        {
            using (ActionsForm form = new ActionsForm())
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
            lvi.SubItems.Add(fileAction.Extensions ?? "");
            lvActions.Items.Add(lvi);
        }

        private void btnActionsEdit_Click(object sender, EventArgs e)
        {
            if (lvActions.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvActions.SelectedItems[0];
                ExternalProgram fileAction = lvi.Tag as ExternalProgram;

                using (ActionsForm form = new ActionsForm(fileAction))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        lvi.Text = fileAction.Name ?? "";
                        lvi.SubItems[1].Text = fileAction.Path ?? "";
                        lvi.SubItems[2].Text = fileAction.Args ?? "";
                        lvi.SubItems[3].Text = fileAction.Extensions ?? "";
                    }
                }
            }
        }

        private void btnActionsDuplicate_Click(object sender, EventArgs e)
        {
            foreach (ExternalProgram fileAction in lvActions.SelectedItems.Cast<ListViewItem>().Select(x => ((ExternalProgram)x.Tag).Copy()))
            {
                TaskSettings.ExternalPrograms.Add(fileAction);
                AddFileAction(fileAction);
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

        private void btnActions_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.Actions);
        }

        private void lvActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnActionsEdit.Enabled = btnActionsDuplicate.Enabled = btnActionsRemove.Enabled = lvActions.SelectedItems.Count > 0;
        }

        private void lvActions_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ExternalProgram fileAction = e.Item.Tag as ExternalProgram;
            fileAction.IsActive = e.Item.Checked;
        }

        private void lvActions_ItemMoved(object sender, int oldIndex, int newIndex)
        {
            TaskSettings.ExternalPrograms.Move(oldIndex, newIndex);
        }

        #endregion Actions

        #region Watch folders

        private void WatchFolderAdd(WatchFolderSettings watchFolderSetting)
        {
            if (Program.WatchFolderManager != null && watchFolderSetting != null)
            {
                Program.WatchFolderManager.AddWatchFolder(watchFolderSetting, TaskSettings);

                ListViewItem lvi = new ListViewItem(watchFolderSetting.FolderPath ?? "");
                lvi.Tag = watchFolderSetting;
                lvi.SubItems.Add(watchFolderSetting.Filter ?? "");
                lvi.SubItems.Add(watchFolderSetting.IncludeSubdirectories.ToString());
                lvWatchFolderList.Items.Add(lvi);
            }
        }

        private void WatchFolderEditSelected()
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
                        lvi.SubItems[2].Text = watchFolder.IncludeSubdirectories.ToString();

                        Program.WatchFolderManager.UpdateWatchFolderState(watchFolder);
                    }
                }
            }
        }

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
                    WatchFolderAdd(form.WatchFolder);
                }
            }
        }

        private void btnWatchFolderEdit_Click(object sender, EventArgs e)
        {
            WatchFolderEditSelected();
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

        private void lvWatchFolderList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WatchFolderEditSelected();
        }

        #endregion Watch folders

        #region Tools

        private void cbUseDefaultToolsSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultToolsSettings = !cbOverrideToolsSettings.Checked;
            UpdateDefaultSettingVisibility();
        }

        private void txtToolsScreenColorPickerFormat_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.ToolsSettings.ScreenColorPickerFormat = txtToolsScreenColorPickerFormat.Text;
        }

        private void txtToolsScreenColorPickerFormatCtrl_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.ToolsSettings.ScreenColorPickerFormatCtrl = txtToolsScreenColorPickerFormatCtrl.Text;
        }

        private void txtToolsScreenColorPickerInfoText_TextChanged(object sender, EventArgs e)
        {
            TaskSettings.ToolsSettings.ScreenColorPickerInfoText = txtToolsScreenColorPickerInfoText.Text;
        }

        #endregion Tools

        #region Advanced

        private void cbUseDefaultAdvancedSettings_CheckedChanged(object sender, EventArgs e)
        {
            TaskSettings.UseDefaultAdvancedSettings = !cbOverrideAdvancedSettings.Checked;
            UpdateDefaultSettingVisibility();
        }

        #endregion Advanced
    }
}