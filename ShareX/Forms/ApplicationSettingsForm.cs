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
using ShareX.UploadersLib;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX
{
    public partial class ApplicationSettingsForm : Form
    {
        private bool ready;
        private string lastPersonalPath;

        public ApplicationSettingsForm()
        {
            InitializeControls();
            ShareXResources.ApplyTheme(this, true);
        }

        private void SettingsForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void SettingsForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void tttvMain_TabChanged(TabPage tabPage)
        {
            if (tabPage == tpIntegration)
            {
                UpdateStartWithWindows();
            }
        }

        private void InitializeControls()
        {
            InitializeComponent();

            foreach (SupportedLanguage language in Helpers.GetEnums<SupportedLanguage>())
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(language.GetLocalizedDescription());
                tsmi.Image = LanguageHelper.GetLanguageIcon(language);
                tsmi.ImageScaling = ToolStripItemImageScaling.None;
                SupportedLanguage lang = language;
                tsmi.Click += (sender, e) => ChangeLanguage(lang);
                cmsLanguages.Items.Add(tsmi);
            }

            cbTrayLeftDoubleClickAction.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<HotkeyType>());
            cbTrayLeftClickAction.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<HotkeyType>());
            cbTrayMiddleClickAction.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<HotkeyType>());
            cbUpdateChannel.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<UpdateChannel>());
            cbMainWindowTaskViewMode.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<TaskViewMode>());
            cbThumbnailViewTitleLocation.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<ThumbnailTitleLocation>());
            cbThumbnailViewThumbnailClickAction.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<ThumbnailViewClickAction>());
            cbListViewImagePreviewVisibility.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<ImagePreviewVisibility>());
            cbListViewImagePreviewLocation.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<ImagePreviewLocation>());

            eiTheme.ObjectType = typeof(ShareXTheme);

            CodeMenu.Create<CodeMenuEntryFilename>(txtSaveImageSubFolderPattern, CodeMenuEntryFilename.t, CodeMenuEntryFilename.pn, CodeMenuEntryFilename.i, CodeMenuEntryFilename.width, CodeMenuEntryFilename.height, CodeMenuEntryFilename.n);
            CodeMenu.Create<CodeMenuEntryFilename>(txtSaveImageSubFolderPatternWindow, CodeMenuEntryFilename.i, CodeMenuEntryFilename.n);

            cbProxyMethod.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<ProxyMethod>());

            UpdateControls();
        }

        private void UpdateControls()
        {
            ready = false;

            // General
            ChangeLanguage(Program.Settings.Language);

            cbShowTray.Checked = Program.Settings.ShowTray;
            cbSilentRun.Enabled = Program.Settings.ShowTray;
            cbSilentRun.Checked = Program.Settings.SilentRun;
            cbTrayIconProgressEnabled.Checked = Program.Settings.TrayIconProgressEnabled;
            cbTaskbarProgressEnabled.Enabled = TaskbarManager.IsPlatformSupported;
            cbTaskbarProgressEnabled.Checked = Program.Settings.TaskbarProgressEnabled;
            cbUseCustomTheme.Checked = Program.Settings.UseCustomTheme;
            cbUseWhiteShareXIcon.Checked = Program.Settings.UseWhiteShareXIcon;
            cbRememberMainFormPosition.Checked = Program.Settings.RememberMainFormPosition;
            cbRememberMainFormSize.Checked = Program.Settings.RememberMainFormSize;

            cbTrayLeftDoubleClickAction.SelectedIndex = (int)Program.Settings.TrayLeftDoubleClickAction;
            cbTrayLeftClickAction.SelectedIndex = (int)Program.Settings.TrayLeftClickAction;
            cbTrayMiddleClickAction.SelectedIndex = (int)Program.Settings.TrayMiddleClickAction;

#if STEAM || MicrosoftStore
            cbAutoCheckUpdate.Visible = false;
            lblUpdateChannel.Visible = false;
            cbUpdateChannel.Visible = false;
            btnCheckDevBuild.Visible = false;
#else
            if (SystemOptions.DisableUpdateCheck)
            {
                cbAutoCheckUpdate.Visible = false;
                lblUpdateChannel.Visible = false;
                cbUpdateChannel.Visible = false;
                btnCheckDevBuild.Visible = false;
            }
            else
            {
                cbAutoCheckUpdate.Checked = Program.Settings.AutoCheckUpdate;
                cbUpdateChannel.Enabled = Program.Settings.AutoCheckUpdate;
                cbUpdateChannel.SelectedIndex = (int)Program.Settings.UpdateChannel;
            }
#endif

            // Theme
            cbThemes.Items.AddRange(Program.Settings.Themes.ToArray());
            cbThemes.SelectedIndex = Program.Settings.SelectedTheme;
            pgTheme.SelectedObject = Program.Settings.Themes[Program.Settings.SelectedTheme];
            UpdateThemeControls();

            // Integration
#if MicrosoftStore
            cbShellContextMenu.Visible = false;
            cbEditWithShareX.Visible = false;
            cbSendToMenu.Visible = false;
            gbChrome.Visible = false;
            gbFirefox.Visible = false;
#else
            cbShellContextMenu.Checked = IntegrationHelpers.CheckShellContextMenuButton();
            cbEditWithShareX.Checked = IntegrationHelpers.CheckEditShellContextMenuButton();
            cbSendToMenu.Checked = IntegrationHelpers.CheckSendToMenuButton();
            cbChromeExtensionSupport.Checked = IntegrationHelpers.CheckChromeExtensionSupport();
            btnChromeOpenExtensionPage.Enabled = cbChromeExtensionSupport.Checked;
            cbFirefoxAddonSupport.Checked = IntegrationHelpers.CheckFirefoxAddonSupport();
            btnFirefoxOpenAddonPage.Enabled = cbFirefoxAddonSupport.Checked;
#endif

#if STEAM
            cbSteamShowInApp.Checked = IntegrationHelpers.CheckSteamShowInApp();
#else
            gbSteam.Visible = false;
#endif

            // Paths
            lastPersonalPath = Program.ReadPersonalPathConfig();
            txtPersonalFolderPath.Text = lastPersonalPath;
            UpdatePersonalFolderPathPreview();
            cbUseCustomScreenshotsPath.Checked = Program.Settings.UseCustomScreenshotsPath;
            txtCustomScreenshotsPath.Text = Program.Settings.CustomScreenshotsPath;
            txtSaveImageSubFolderPattern.Text = Program.Settings.SaveImageSubFolderPattern;
            txtSaveImageSubFolderPatternWindow.Text = Program.Settings.SaveImageSubFolderPatternWindow;

            // Settings
            cbAutomaticallyCleanupBackupFiles.Checked = Program.Settings.AutoCleanupBackupFiles;
            cbAutomaticallyCleanupLogFiles.Checked = Program.Settings.AutoCleanupLogFiles;
            nudCleanupKeepFileCount.SetValue(Program.Settings.CleanupKeepFileCount);

            // Main window
            cbMainWindowShowMenu.Checked = Program.Settings.ShowMenu;
            cbMainWindowTaskViewMode.SelectedIndex = (int)Program.Settings.TaskViewMode;
            cbThumbnailViewShowTitle.Checked = Program.Settings.ShowThumbnailTitle;
            cbThumbnailViewTitleLocation.SelectedIndex = (int)Program.Settings.ThumbnailTitleLocation;
            nudThumbnailViewThumbnailSizeWidth.SetValue(Program.Settings.ThumbnailSize.Width);
            nudThumbnailViewThumbnailSizeHeight.SetValue(Program.Settings.ThumbnailSize.Height);
            cbThumbnailViewThumbnailClickAction.SelectedIndex = (int)Program.Settings.ThumbnailClickAction;
            cbListViewShowColumns.Checked = Program.Settings.ShowColumns;
            cbListViewImagePreviewVisibility.SelectedIndex = (int)Program.Settings.ImagePreview;
            cbListViewImagePreviewLocation.SelectedIndex = (int)Program.Settings.ImagePreviewLocation;

            // Clipboard formats
            lvClipboardFormats.Items.Clear();
            foreach (ClipboardFormat cf in Program.Settings.ClipboardContentFormats)
            {
                AddClipboardFormat(cf);
            }

            // Upload
            nudUploadLimit.SetValue(Program.Settings.UploadLimit);

            cbBufferSize.Items.Clear();
            int maxBufferSizePower = 14;
            for (int i = 0; i < maxBufferSizePower; i++)
            {
                string size = ((long)(Math.Pow(2, i) * 1024)).ToSizeString(Program.Settings.BinaryUnits, 0);
                cbBufferSize.Items.Add(size);
            }
            cbBufferSize.SelectedIndex = Program.Settings.BufferSizePower.Clamp(0, maxBufferSizePower);

            nudRetryUpload.SetValue(Program.Settings.MaxUploadFailRetry);
            cbUseSecondaryUploaders.Checked = Program.Settings.UseSecondaryUploaders;

            Program.Settings.SecondaryImageUploaders.AddRange(Helpers.GetEnums<ImageDestination>().Where(n => Program.Settings.SecondaryImageUploaders.All(e => e != n)));
            Program.Settings.SecondaryTextUploaders.AddRange(Helpers.GetEnums<TextDestination>().Where(n => Program.Settings.SecondaryTextUploaders.All(e => e != n)));
            Program.Settings.SecondaryFileUploaders.AddRange(Helpers.GetEnums<FileDestination>().Where(n => Program.Settings.SecondaryFileUploaders.All(e => e != n)));

            Program.Settings.SecondaryImageUploaders.Where(n => Helpers.GetEnums<ImageDestination>().All(e => e != n)).ForEach(x => Program.Settings.SecondaryImageUploaders.Remove(x));
            Program.Settings.SecondaryTextUploaders.Where(n => Helpers.GetEnums<TextDestination>().All(e => e != n)).ForEach(x => Program.Settings.SecondaryTextUploaders.Remove(x));
            Program.Settings.SecondaryFileUploaders.Where(n => Helpers.GetEnums<FileDestination>().All(e => e != n)).ForEach(x => Program.Settings.SecondaryFileUploaders.Remove(x));

            lvSecondaryImageUploaders.Items.Clear();
            Program.Settings.SecondaryImageUploaders.ForEach<ImageDestination>(x => lvSecondaryImageUploaders.Items.Add(new ListViewItem(x.GetLocalizedDescription()) { Tag = x }));
            lvSecondaryTextUploaders.Items.Clear();
            Program.Settings.SecondaryTextUploaders.ForEach<TextDestination>(x => lvSecondaryTextUploaders.Items.Add(new ListViewItem(x.GetLocalizedDescription()) { Tag = x }));
            lvSecondaryFileUploaders.Items.Clear();
            Program.Settings.SecondaryFileUploaders.ForEach<FileDestination>(x => lvSecondaryFileUploaders.Items.Add(new ListViewItem(x.GetLocalizedDescription()) { Tag = x }));

            // History
            cbHistorySaveTasks.Checked = Program.Settings.HistorySaveTasks;
            cbHistoryCheckURL.Checked = Program.Settings.HistoryCheckURL;

            cbRecentTasksSave.Checked = Program.Settings.RecentTasksSave;
            nudRecentTasksMaxCount.SetValue(Program.Settings.RecentTasksMaxCount);
            cbRecentTasksShowInMainWindow.Checked = Program.Settings.RecentTasksShowInMainWindow;
            cbRecentTasksShowInTrayMenu.Checked = Program.Settings.RecentTasksShowInTrayMenu;
            cbRecentTasksTrayMenuMostRecentFirst.Checked = Program.Settings.RecentTasksTrayMenuMostRecentFirst;

            // Print
            cbDontShowPrintSettingDialog.Checked = Program.Settings.DontShowPrintSettingsDialog;
            cbPrintDontShowWindowsDialog.Checked = !Program.Settings.PrintSettings.ShowPrintDialog;
            txtDefaultPrinterOverride.Text = Program.Settings.PrintSettings.DefaultPrinterOverride;
            lblDefaultPrinterOverride.Visible = txtDefaultPrinterOverride.Visible = !Program.Settings.PrintSettings.ShowPrintDialog;

            // Proxy
            cbProxyMethod.SelectedIndex = (int)Program.Settings.ProxySettings.ProxyMethod;
            txtProxyUsername.Text = Program.Settings.ProxySettings.Username;
            txtProxyPassword.Text = Program.Settings.ProxySettings.Password;
            txtProxyHost.Text = Program.Settings.ProxySettings.Host ?? "";
            nudProxyPort.SetValue(Program.Settings.ProxySettings.Port);
            UpdateProxyControls();

            // Advanced
            pgSettings.SelectedObject = Program.Settings;

            tttvMain.MainTabControl = tcSettings;

            ready = true;
        }

        private void ChangeLanguage(SupportedLanguage language)
        {
            btnLanguages.Text = language.GetLocalizedDescription();
            btnLanguages.Image = LanguageHelper.GetLanguageIcon(language);

            if (ready)
            {
                Program.Settings.Language = language;

                if (LanguageHelper.ChangeLanguage(Program.Settings.Language) &&
                    MessageBox.Show(Resources.ApplicationSettingsForm_cbLanguage_SelectedIndexChanged_Language_Restart,
                    Resources.ShareXConfirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Program.Restart();
                }
            }
        }

        private void UpdateStartWithWindows()
        {
            ready = false;

            cbStartWithWindows.Text = Resources.ApplicationSettingsForm_cbStartWithWindows_Text;
            cbStartWithWindows.Enabled = false;

            try
            {
                StartupState state = StartupManager.State;
                cbStartWithWindows.Checked = state == StartupState.Enabled || state == StartupState.EnabledByPolicy;

                if (state == StartupState.DisabledByUser)
                {
                    cbStartWithWindows.Text = Resources.ApplicationSettingsForm_cbStartWithWindows_DisabledByUser_Text;
                }
                else if (state == StartupState.DisabledByPolicy)
                {
                    cbStartWithWindows.Text = Resources.ApplicationSettingsForm_cbStartWithWindows_DisabledByPolicy_Text;
                }
                else if (state == StartupState.EnabledByPolicy)
                {
                    cbStartWithWindows.Text = Resources.ApplicationSettingsForm_cbStartWithWindows_EnabledByPolicy_Text;
                }
                else
                {
                    cbStartWithWindows.Enabled = true;
                }
            }
            catch (Exception e)
            {
                e.ShowError();
            }

            ready = true;
        }

        private void UpdateProxyControls()
        {
            switch (Program.Settings.ProxySettings.ProxyMethod)
            {
                case ProxyMethod.None:
                    txtProxyUsername.Enabled = txtProxyPassword.Enabled = txtProxyHost.Enabled = nudProxyPort.Enabled = false;
                    break;
                case ProxyMethod.Manual:
                    txtProxyUsername.Enabled = txtProxyPassword.Enabled = txtProxyHost.Enabled = nudProxyPort.Enabled = true;
                    break;
                case ProxyMethod.Automatic:
                    txtProxyUsername.Enabled = txtProxyPassword.Enabled = true;
                    txtProxyHost.Enabled = nudProxyPort.Enabled = false;
                    break;
            }
        }

        private void UpdatePersonalFolderPathPreview()
        {
            try
            {
                string personalPath = FileHelpers.SanitizePath(txtPersonalFolderPath.Text);

                if (string.IsNullOrEmpty(personalPath))
                {
                    if (Program.Portable)
                    {
                        personalPath = Program.PortablePersonalFolder;
                    }
                    else
                    {
                        personalPath = Program.DefaultPersonalFolder;
                    }
                }
                else
                {
                    personalPath = FileHelpers.GetAbsolutePath(personalPath);
                }

                lblPreviewPersonalFolderPath.Text = personalPath;
                btnPersonalFolderPathApply.Enabled = !personalPath.Equals(lastPersonalPath, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception e)
            {
                btnPersonalFolderPathApply.Enabled = false;
                lblPreviewPersonalFolderPath.Text = "Error: " + e.Message;
            }
        }

        private void UpdateScreenshotsFolderPathPreview()
        {
            try
            {
                lblSaveImageSubFolderPatternPreview.Text = TaskHelpers.GetScreenshotsFolder();
            }
            catch (Exception e)
            {
                lblSaveImageSubFolderPatternPreview.Text = "Error: " + e.Message;
            }
        }

        #region General

        private void cbShowTray_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.ShowTray = cbShowTray.Checked;

            if (ready)
            {
                Program.MainForm.niTray.Visible = Program.Settings.ShowTray;
            }

            cbSilentRun.Enabled = Program.Settings.ShowTray;
        }

        private void cbSilentRun_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.SilentRun = cbSilentRun.Checked;
        }

        private void cbTrayIconProgressEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.TrayIconProgressEnabled = cbTrayIconProgressEnabled.Checked;
        }

        private void cbTaskbarProgressEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.TaskbarProgressEnabled = cbTaskbarProgressEnabled.Checked;

            if (ready)
            {
                TaskbarManager.Enabled = Program.Settings.TaskbarProgressEnabled;
            }
        }

        private void CbUseWhiteShareXIcon_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.UseWhiteShareXIcon = cbUseWhiteShareXIcon.Checked;
        }

        private void cbRememberMainFormPosition_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.RememberMainFormPosition = cbRememberMainFormPosition.Checked;
        }

        private void cbRememberMainFormSize_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.RememberMainFormSize = cbRememberMainFormSize.Checked;
        }

        private void cbTrayLeftDoubleClickAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.TrayLeftDoubleClickAction = (HotkeyType)cbTrayLeftDoubleClickAction.SelectedIndex;
        }

        private void cbTrayLeftClickAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.TrayLeftClickAction = (HotkeyType)cbTrayLeftClickAction.SelectedIndex;
        }

        private void cbTrayMiddleClickAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.TrayMiddleClickAction = (HotkeyType)cbTrayMiddleClickAction.SelectedIndex;
        }

        private void btnEditQuickTaskMenu_Click(object sender, EventArgs e)
        {
            new QuickTaskMenuEditorForm().ShowDialog();
        }

        private void cbAutoCheckUpdate_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.AutoCheckUpdate = cbAutoCheckUpdate.Checked;
            cbUpdateChannel.Enabled = Program.Settings.AutoCheckUpdate;
        }

        private void cbUpdateChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.UpdateChannel = (UpdateChannel)cbUpdateChannel.SelectedIndex;
        }

        private async void btnCheckDevBuild_Click(object sender, EventArgs e)
        {
            btnCheckDevBuild.Enabled = false;

            await TaskHelpers.DownloadDevBuild();

            if (!IsDisposed)
            {
                btnCheckDevBuild.Enabled = true;
            }
        }

        #endregion General

        #region Theme

        private void UpdateThemeControls()
        {
            btnThemeAdd.Enabled = eiTheme.Enabled = btnThemeReset.Enabled = pgTheme.Enabled = Program.Settings.UseCustomTheme;
            cbThemes.Enabled = btnThemeRemove.Enabled = Program.Settings.UseCustomTheme && cbThemes.Items.Count > 0;
        }

        private void ApplySelectedTheme()
        {
            Program.MainForm.UpdateTheme();
            ShareXResources.ApplyTheme(this);
        }

        private void AddTheme(ShareXTheme theme)
        {
            if (theme != null)
            {
                Program.Settings.Themes.Add(theme);
                cbThemes.Items.Add(theme);
                int index = Program.Settings.Themes.Count - 1;
                Program.Settings.SelectedTheme = index;
                cbThemes.SelectedIndex = index;
                UpdateThemeControls();
            }
        }

        private void CbUseCustomTheme_CheckedChanged(object sender, EventArgs e)
        {
            if (ready)
            {
                Program.Settings.UseCustomTheme = cbUseCustomTheme.Checked;
                UpdateThemeControls();
                ApplySelectedTheme();
            }
        }

        private void CbThemes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ready)
            {
                Program.Settings.SelectedTheme = cbThemes.SelectedIndex;

                if (cbThemes.SelectedItem != null)
                {
                    pgTheme.SelectedObject = cbThemes.SelectedItem;
                }
                else
                {
                    pgTheme.SelectedObject = null;
                }

                UpdateThemeControls();
                ApplySelectedTheme();
            }
        }

        private void BtnThemeAdd_Click(object sender, EventArgs e)
        {
            AddTheme(ShareXTheme.DarkTheme);
        }

        private void BtnThemeRemove_Click(object sender, EventArgs e)
        {
            int index = cbThemes.SelectedIndex;
            if (index > -1)
            {
                Program.Settings.Themes.RemoveAt(index);
                cbThemes.Items.RemoveAt(index);
                if (Program.Settings.Themes.Count > 0)
                {
                    index = 0;
                }
                else
                {
                    index = -1;
                }
                Program.Settings.SelectedTheme = index;
                cbThemes.SelectedIndex = index;
                pgTheme.SelectedObject = cbThemes.SelectedItem;
                UpdateThemeControls();
            }
        }

        private object EiTheme_ExportRequested()
        {
            return pgTheme.SelectedObject as ShareXTheme;
        }

        private void EiTheme_ImportRequested(object obj)
        {
            AddTheme(obj as ShareXTheme);
        }

        private void BtnThemeReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.WouldYouLikeToResetThemes, "ShareX - " + Resources.Confirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Program.Settings.Themes = ShareXTheme.GetDefaultThemes();
                Program.Settings.SelectedTheme = 0;

                cbThemes.Items.Clear();
                cbThemes.Items.AddRange(Program.Settings.Themes.ToArray());
                cbThemes.SelectedIndex = Program.Settings.SelectedTheme;
                pgTheme.SelectedObject = Program.Settings.Themes[Program.Settings.SelectedTheme];
            }
        }

        private void pgTheme_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateThemeControls();
            ApplySelectedTheme();
        }

        #endregion

        #region Integration

        private void cbStartWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            if (ready)
            {
                try
                {
                    StartupManager.State = cbStartWithWindows.Checked ? StartupState.Enabled : StartupState.Disabled;
                    UpdateStartWithWindows();
                }
                catch (Exception ex)
                {
                    ex.ShowError();
                }
            }
        }

        private void cbShellContextMenu_CheckedChanged(object sender, EventArgs e)
        {
            if (ready)
            {
                IntegrationHelpers.CreateShellContextMenuButton(cbShellContextMenu.Checked);
            }
        }

        private void cbEditWithShareX_CheckedChanged(object sender, EventArgs e)
        {
            if (ready)
            {
                IntegrationHelpers.CreateEditShellContextMenuButton(cbEditWithShareX.Checked);
            }
        }

        private void cbSendToMenu_CheckedChanged(object sender, EventArgs e)
        {
            if (ready)
            {
                IntegrationHelpers.CreateSendToMenuButton(cbSendToMenu.Checked);
            }
        }

        private void cbChromeExtensionSupport_CheckedChanged(object sender, EventArgs e)
        {
            if (ready)
            {
                IntegrationHelpers.CreateChromeExtensionSupport(cbChromeExtensionSupport.Checked);
                btnChromeOpenExtensionPage.Enabled = cbChromeExtensionSupport.Checked;
            }
        }

        private void btnChromeOpenExtensionPage_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://chrome.google.com/webstore/detail/sharex/nlkoigbdolhchiicbonbihbphgamnaoc");
        }

        private void cbFirefoxAddonSupport_CheckedChanged(object sender, EventArgs e)
        {
            if (ready)
            {
                IntegrationHelpers.CreateFirefoxAddonSupport(cbFirefoxAddonSupport.Checked);
                btnFirefoxOpenAddonPage.Enabled = cbFirefoxAddonSupport.Checked;
            }
        }

        private void btnFirefoxOpenAddonPage_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://addons.mozilla.org/en-US/firefox/addon/sharex/");
        }

        private void cbSteamShowInApp_CheckedChanged(object sender, EventArgs e)
        {
            if (ready)
            {
                IntegrationHelpers.SteamShowInApp(cbSteamShowInApp.Checked);
            }
        }

        #endregion Integration

        #region Paths

        private void txtPersonalFolderPath_TextChanged(object sender, EventArgs e)
        {
            UpdatePersonalFolderPathPreview();
        }

        private void btnBrowsePersonalFolderPath_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFolder(Resources.ApplicationSettingsForm_btnBrowsePersonalFolderPath_Click_Choose_ShareX_personal_folder_path,
                txtPersonalFolderPath, Program.PersonalFolder, true);
        }

        private void btnPersonalFolderPathApply_Click(object sender, EventArgs e)
        {
            string currentPersonalPath = FileHelpers.SanitizePath(txtPersonalFolderPath.Text);

            if (!currentPersonalPath.Equals(lastPersonalPath, StringComparison.OrdinalIgnoreCase) && Program.WritePersonalPathConfig(currentPersonalPath))
            {
                lastPersonalPath = currentPersonalPath;
                btnPersonalFolderPathApply.Enabled = false;

                if (MessageBox.Show(Resources.ShareXNeedsToBeRestartedForThePersonalFolderChangesToApply, Resources.ShareXConfirmation,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Program.Restart();
                }
            }
        }

        private void btnOpenPersonalFolder_Click(object sender, EventArgs e)
        {
            FileHelpers.OpenFolder(lblPreviewPersonalFolderPath.Text);
        }

        private void cbUseCustomScreenshotsPath_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.UseCustomScreenshotsPath = cbUseCustomScreenshotsPath.Checked;
            UpdateScreenshotsFolderPathPreview();
        }

        private void txtCustomScreenshotsPath_TextChanged(object sender, EventArgs e)
        {
            Program.Settings.CustomScreenshotsPath = FileHelpers.SanitizePath(txtCustomScreenshotsPath.Text);
            UpdateScreenshotsFolderPathPreview();
        }

        private void btnBrowseCustomScreenshotsPath_Click(object sender, EventArgs e)
        {
            FileHelpers.BrowseFolder(Resources.ApplicationSettingsForm_btnBrowseCustomScreenshotsPath_Click_Choose_screenshots_folder_path,
                txtCustomScreenshotsPath, Program.PersonalFolder, true);
        }

        private void txtSaveImageSubFolderPattern_TextChanged(object sender, EventArgs e)
        {
            Program.Settings.SaveImageSubFolderPattern = FileHelpers.SanitizePath(txtSaveImageSubFolderPattern.Text);
            UpdateScreenshotsFolderPathPreview();
        }

        private void btnOpenScreenshotsFolder_Click(object sender, EventArgs e)
        {
            FileHelpers.OpenFolder(lblSaveImageSubFolderPatternPreview.Text);
        }

        private void txtSaveImageSubFolderPatternWindow_TextChanged(object sender, EventArgs e)
        {
            Program.Settings.SaveImageSubFolderPatternWindow = FileHelpers.SanitizePath(txtSaveImageSubFolderPatternWindow.Text);
        }

        #endregion Paths

        #region Settings

        private void cbExportSettings_CheckedChanged(object sender, EventArgs e)
        {
            btnExport.Enabled = cbExportSettings.Checked || cbExportHistory.Checked;
        }

        private void cbExportHistory_CheckedChanged(object sender, EventArgs e)
        {
            btnExport.Enabled = cbExportSettings.Checked || cbExportHistory.Checked;
        }

        private async void btnExport_Click(object sender, EventArgs e)
        {
            bool exportSettings = cbExportSettings.Checked;
            bool exportHistory = cbExportHistory.Checked;

            if (exportSettings || exportHistory)
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.DefaultExt = "sxb";
                    sfd.FileName = $"ShareX-{Helpers.GetApplicationVersion()}-backup.sxb";
                    sfd.Filter = "ShareX backup (*.sxb)|*.sxb|All files (*.*)|*.*";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        btnExport.Enabled = false;
                        btnImport.Enabled = false;
                        pbExportImport.Location = btnExport.Location;
                        pbExportImport.Visible = true;

                        string exportPath = sfd.FileName;

                        DebugHelper.WriteLine($"Export started: {exportPath}");

                        await Task.Run(() =>
                        {
                            SettingManager.SaveAllSettings();
                            SettingManager.Export(exportPath, exportSettings, exportHistory);
                        });

                        if (!IsDisposed)
                        {
                            pbExportImport.Visible = false;
                            btnExport.Enabled = true;
                            btnImport.Enabled = true;
                        }

                        DebugHelper.WriteLine($"Export completed: {exportPath}");
                    }
                }
            }
        }

        private async void btnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "ShareX backup (*.sxb)|*.sxb|All files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    btnExport.Enabled = false;
                    btnImport.Enabled = false;
                    pbExportImport.Location = btnImport.Location;
                    pbExportImport.Visible = true;

                    string importPath = ofd.FileName;

                    DebugHelper.WriteLine($"Import started: {importPath}");

                    await Task.Run(() =>
                    {
                        SettingManager.Import(importPath);
                        SettingManager.LoadAllSettings();
                    });

                    if (!IsDisposed)
                    {
                        UpdateControls();

                        pbExportImport.Visible = false;
                        btnExport.Enabled = true;
                        btnImport.Enabled = true;
                    }

                    LanguageHelper.ChangeLanguage(Program.Settings.Language);

                    await Program.MainForm.UpdateControls();

                    DebugHelper.WriteLine($"Import completed: {importPath}");
                }
            }
        }

        private async void btnResetSettings_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.ApplicationSettingsForm_btnResetSettings_Click_WouldYouLikeToResetShareXSettings, "ShareX - " + Resources.Confirmation,
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                SettingManager.ResetSettings();
                SettingManager.SaveAllSettings();

                UpdateControls();

                LanguageHelper.ChangeLanguage(Program.Settings.Language);

                await Program.MainForm.UpdateControls();

                DebugHelper.WriteLine("Settings reset.");
            }
        }

        private void cbAutomaticallyCleanupBackupFiles_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.AutoCleanupBackupFiles = cbAutomaticallyCleanupBackupFiles.Checked;
        }

        private void cbAutomaticallyCleanupLogFiles_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.AutoCleanupLogFiles = cbAutomaticallyCleanupLogFiles.Checked;
        }

        private void nudCleanupKeepFileCount_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.CleanupKeepFileCount = (int)nudCleanupKeepFileCount.Value;
        }

        #endregion Settings

        #region Main window

        private void cbMainWindowShowMenu_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.ShowMenu = cbMainWindowShowMenu.Checked;
        }

        private void cbMainWindowTaskViewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.TaskViewMode = (TaskViewMode)cbMainWindowTaskViewMode.SelectedIndex;
        }

        private void cbThumbnailViewShowTitle_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.ShowThumbnailTitle = cbThumbnailViewShowTitle.Checked;
        }

        private void cbThumbnailViewTitleLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.ThumbnailTitleLocation = (ThumbnailTitleLocation)cbThumbnailViewTitleLocation.SelectedIndex;
        }

        private void nudThumbnailViewThumbnailSizeWidth_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.ThumbnailSize = new Size((int)nudThumbnailViewThumbnailSizeWidth.Value, Program.Settings.ThumbnailSize.Height);
        }

        private void nudThumbnailViewThumbnailSizeHeight_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.ThumbnailSize = new Size(Program.Settings.ThumbnailSize.Width, (int)nudThumbnailViewThumbnailSizeHeight.Value);
        }

        private void btnThumbnailViewThumbnailSizeReset_Click(object sender, EventArgs e)
        {
            nudThumbnailViewThumbnailSizeWidth.SetValue(200);
            nudThumbnailViewThumbnailSizeHeight.SetValue(150);
        }

        private void cbThumbnailViewThumbnailClickAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.ThumbnailClickAction = (ThumbnailViewClickAction)cbThumbnailViewThumbnailClickAction.SelectedIndex;
        }

        private void cbListViewShowColumns_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.ShowColumns = cbListViewShowColumns.Checked;
        }

        private void cbListViewImagePreviewVisibility_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.ImagePreview = (ImagePreviewVisibility)cbListViewImagePreviewVisibility.SelectedIndex;
        }

        private void cbListViewImagePreviewLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.ImagePreviewLocation = (ImagePreviewLocation)cbListViewImagePreviewLocation.SelectedIndex;
        }

        #endregion

        #region Clipboard formats

        private void AddClipboardFormat(ClipboardFormat cf)
        {
            ListViewItem lvi = new ListViewItem(cf.Description ?? "");
            lvi.Tag = cf;
            lvi.SubItems.Add(cf.Format ?? "");
            lvClipboardFormats.Items.Add(lvi);
        }

        private void ClipboardFormatsEditSelected()
        {
            if (lvClipboardFormats.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvClipboardFormats.SelectedItems[0];
                ClipboardFormat cf = lvi.Tag as ClipboardFormat;
                using (ClipboardFormatForm form = new ClipboardFormatForm(cf))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        lvi.Text = form.ClipboardFormat.Description ?? "";
                        lvi.Tag = form.ClipboardFormat;
                        lvi.SubItems[1].Text = form.ClipboardFormat.Format ?? "";
                    }
                }
            }
        }

        private void lvClipboardFormats_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ClipboardFormatsEditSelected();
            }
        }

        private void btnAddClipboardFormat_Click(object sender, EventArgs e)
        {
            using (ClipboardFormatForm form = new ClipboardFormatForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ClipboardFormat cf = form.ClipboardFormat;
                    Program.Settings.ClipboardContentFormats.Add(cf);
                    AddClipboardFormat(cf);
                }
            }
        }

        private void btnClipboardFormatEdit_Click(object sender, EventArgs e)
        {
            ClipboardFormatsEditSelected();
        }

        private void btnClipboardFormatRemove_Click(object sender, EventArgs e)
        {
            if (lvClipboardFormats.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvClipboardFormats.SelectedItems[0];
                ClipboardFormat cf = lvi.Tag as ClipboardFormat;
                Program.Settings.ClipboardContentFormats.Remove(cf);
                lvClipboardFormats.Items.Remove(lvi);
            }
        }

        #endregion

        #region Upload

        private void nudUploadLimit_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.UploadLimit = (int)nudUploadLimit.Value;
        }

        private void cbBufferSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.BufferSizePower = cbBufferSize.SelectedIndex;
        }

        private void nudRetryUpload_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.MaxUploadFailRetry = (int)nudRetryUpload.Value;
        }

        private void cbUseSecondaryUploaders_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.UseSecondaryUploaders = cbUseSecondaryUploaders.Checked;
        }

        private void lvSecondaryUploaders_MouseUp(object sender, MouseEventArgs e)
        {
            Program.Settings.SecondaryImageUploaders = lvSecondaryImageUploaders.Items.Cast<ListViewItem>().Select(x => (ImageDestination)x.Tag).ToList();
            Program.Settings.SecondaryTextUploaders = lvSecondaryTextUploaders.Items.Cast<ListViewItem>().Select(x => (TextDestination)x.Tag).ToList();
            Program.Settings.SecondaryFileUploaders = lvSecondaryFileUploaders.Items.Cast<ListViewItem>().Select(x => (FileDestination)x.Tag).ToList();
        }

        #endregion Upload

        #region History

        private void cbHistorySaveTasks_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.HistorySaveTasks = cbHistorySaveTasks.Checked;
        }

        private void cbHistoryCheckURL_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.HistoryCheckURL = cbHistoryCheckURL.Checked;
        }

        private void cbRecentTasksSave_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.RecentTasksSave = cbRecentTasksSave.Checked;
        }

        private void nudRecentTasksMaxCount_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.RecentTasksMaxCount = (int)nudRecentTasksMaxCount.Value;
        }

        private void cbRecentTasksShowInMainWindow_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.RecentTasksShowInMainWindow = cbRecentTasksShowInMainWindow.Checked;
        }

        private void cbRecentTasksShowInTrayMenu_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.RecentTasksShowInTrayMenu = cbRecentTasksShowInTrayMenu.Checked;
        }

        private void cbRecentTasksTrayMenuMostRecentFirst_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.RecentTasksTrayMenuMostRecentFirst = cbRecentTasksTrayMenuMostRecentFirst.Checked;
        }

        #endregion History

        #region Print

        private void cbDontShowPrintSettingDialog_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.DontShowPrintSettingsDialog = cbDontShowPrintSettingDialog.Checked;
        }

        private void btnShowImagePrintSettings_Click(object sender, EventArgs e)
        {
            using (Image testImage = TaskHelpers.GetScreenshot().CaptureActiveMonitor())
            using (PrintForm printForm = new PrintForm(testImage, Program.Settings.PrintSettings, true))
            {
                printForm.ShowDialog();
            }
        }

        private void cbPrintDontShowWindowsDialog_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.PrintSettings.ShowPrintDialog = !cbPrintDontShowWindowsDialog.Checked;
            lblDefaultPrinterOverride.Visible = txtDefaultPrinterOverride.Visible = !Program.Settings.PrintSettings.ShowPrintDialog;
        }

        private void txtDefaultPrinterOverride_TextChanged(object sender, EventArgs e)
        {
            Program.Settings.PrintSettings.DefaultPrinterOverride = txtDefaultPrinterOverride.Text;
        }

        #endregion Print

        #region Proxy

        private void cbProxyMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.ProxySettings.ProxyMethod = (ProxyMethod)cbProxyMethod.SelectedIndex;

            if (Program.Settings.ProxySettings.ProxyMethod == ProxyMethod.Automatic)
            {
                Program.Settings.ProxySettings.IsValidProxy();
                txtProxyHost.Text = Program.Settings.ProxySettings.Host ?? "";
                nudProxyPort.SetValue(Program.Settings.ProxySettings.Port);
            }

            UpdateProxyControls();
        }

        private void txtProxyUsername_TextChanged(object sender, EventArgs e)
        {
            Program.Settings.ProxySettings.Username = txtProxyUsername.Text;
        }

        private void txtProxyPassword_TextChanged(object sender, EventArgs e)
        {
            Program.Settings.ProxySettings.Password = txtProxyPassword.Text;
        }

        private void txtProxyHost_TextChanged(object sender, EventArgs e)
        {
            Program.Settings.ProxySettings.Host = txtProxyHost.Text;
        }

        private void nudProxyPort_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.ProxySettings.Port = (int)nudProxyPort.Value;
        }

        #endregion Proxy
    }
}