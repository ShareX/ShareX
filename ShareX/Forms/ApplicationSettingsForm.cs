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
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using ShareX.UploadersLib;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX
{
    public partial class ApplicationSettingsForm : Form
    {
        private const int MaxBufferSizePower = 14;

        private bool ready;

        public ApplicationSettingsForm()
        {
            InitializeControls();
            Icon = ShareXResources.Icon;
        }

        private void SettingsForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void SettingsForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.WritePersonalPathConfig(txtPersonalFolderPath.Text);
        }

        private void InitializeControls()
        {
            InitializeComponent();

            foreach (SupportedLanguage language in Helpers.GetEnums<SupportedLanguage>())
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(language.GetLocalizedDescription());
                tsmi.Image = GetLanguageIcon(language);
                tsmi.ImageScaling = ToolStripItemImageScaling.None;
                SupportedLanguage lang = language;
                tsmi.Click += (sender, e) => ChangeLanguage(lang);
                cmsLanguages.Items.Add(tsmi);
            }

            CodeMenu.Create(txtSaveImageSubFolderPattern, ReplCodeMenuEntry.t, ReplCodeMenuEntry.pn, ReplCodeMenuEntry.i, ReplCodeMenuEntry.width, ReplCodeMenuEntry.height, ReplCodeMenuEntry.n);

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
            cbRememberMainFormPosition.Checked = Program.Settings.RememberMainFormPosition;
            cbRememberMainFormSize.Checked = Program.Settings.RememberMainFormSize;

            // Integration
            cbStartWithWindows.Checked = IntegrationHelpers.CheckStartupShortcut();
            cbShellContextMenu.Checked = IntegrationHelpers.CheckShellContextMenuButton();
            cbSendToMenu.Checked = IntegrationHelpers.CheckSendToMenuButton();

#if STEAM
            cbSteamShowInApp.Checked = IntegrationHelpers.CheckSteamShowInApp();
#else
            gbSteam.Visible = false;
#endif

            // Paths
            txtPersonalFolderPath.Text = Program.ReadPersonalPathConfig();
            UpdatePersonalFolderPathPreview();
            cbUseCustomScreenshotsPath.Checked = Program.Settings.UseCustomScreenshotsPath;
            txtCustomScreenshotsPath.Text = Program.Settings.CustomScreenshotsPath;
            txtSaveImageSubFolderPattern.Text = Program.Settings.SaveImageSubFolderPattern;

            // Export / Import
            cbExportSettings.Checked = Program.Settings.ExportSettings;
            cbExportHistory.Checked = Program.Settings.ExportHistory;
            cbExportLogs.Checked = Program.Settings.ExportLogs;
            UpdateExportButton();

            // Proxy
            cbProxyMethod.SelectedIndex = (int)Program.Settings.ProxySettings.ProxyMethod;
            txtProxyUsername.Text = Program.Settings.ProxySettings.Username;
            txtProxyPassword.Text = Program.Settings.ProxySettings.Password;
            txtProxyHost.Text = Program.Settings.ProxySettings.Host ?? string.Empty;
            nudProxyPort.SetValue(Program.Settings.ProxySettings.Port);
            UpdateProxyControls();

            // Upload
            nudUploadLimit.SetValue(Program.Settings.UploadLimit);

            cbBufferSize.Items.Clear();
            for (int i = 0; i < MaxBufferSizePower; i++)
            {
                string size = ((long)(Math.Pow(2, i) * 1024)).ToSizeString(Program.Settings.BinaryUnits, 0);
                cbBufferSize.Items.Add(size);
            }

            cbBufferSize.SelectedIndex = Program.Settings.BufferSizePower.Between(0, MaxBufferSizePower);

            lvClipboardFormats.Items.Clear();
            foreach (ClipboardFormat cf in Program.Settings.ClipboardContentFormats)
            {
                AddClipboardFormat(cf);
            }

            nudRetryUpload.SetValue(Program.Settings.MaxUploadFailRetry);
            chkUseSecondaryUploaders.Checked = Program.Settings.UseSecondaryUploaders;
            tlpBackupDestinations.Enabled = Program.Settings.UseSecondaryUploaders;

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

            // Advanced
            pgSettings.SelectedObject = Program.Settings;

            tttvMain.MainTabControl = tcSettings;

            ready = true;
        }

        private Image GetLanguageIcon(SupportedLanguage language)
        {
            Image icon;

            switch (language)
            {
                default:
                case SupportedLanguage.Automatic:
                    icon = Resources.globe;
                    break;
                case SupportedLanguage.Dutch:
                    icon = Resources.nl;
                    break;
                case SupportedLanguage.English:
                    icon = Resources.us;
                    break;
                case SupportedLanguage.French:
                    icon = Resources.fr;
                    break;
                case SupportedLanguage.German:
                    icon = Resources.de;
                    break;
                case SupportedLanguage.Hungarian:
                    icon = Resources.hu;
                    break;
                case SupportedLanguage.Korean:
                    icon = Resources.kr;
                    break;
                case SupportedLanguage.PortugueseBrazil:
                    icon = Resources.br;
                    break;
                case SupportedLanguage.Russian:
                    icon = Resources.ru;
                    break;
                case SupportedLanguage.SimplifiedChinese:
                    icon = Resources.cn;
                    break;
                case SupportedLanguage.Spanish:
                    icon = Resources.es;
                    break;
                case SupportedLanguage.Turkish:
                    icon = Resources.tr;
                    break;
                case SupportedLanguage.Vietnamese:
                    icon = Resources.vn;
                    break;
            }

            return icon;
        }

        private void ChangeLanguage(SupportedLanguage language)
        {
            btnLanguages.Text = language.GetLocalizedDescription();
            btnLanguages.Image = GetLanguageIcon(language);

            if (ready)
            {
                Program.Settings.Language = language;

                if (LanguageHelper.ChangeLanguage(Program.Settings.Language) &&
                    MessageBox.Show(Resources.ApplicationSettingsForm_cbLanguage_SelectedIndexChanged_Language_Restart,
                    "ShareX", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Program.Restart();
                }
            }
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
            string personalPath = txtPersonalFolderPath.Text;

            if (string.IsNullOrEmpty(personalPath))
            {
                personalPath = Program.DefaultPersonalFolder;
            }
            else
            {
                personalPath = Helpers.ExpandFolderVariables(personalPath);
                personalPath = Helpers.GetAbsolutePath(personalPath);
            }

            lblPreviewPersonalFolderPath.Text = personalPath;
        }

        private void UpdateExportButton()
        {
            btnExport.Enabled = Program.Settings.ExportSettings || Program.Settings.ExportHistory || Program.Settings.ExportLogs;
        }

        #region General

        private void llTranslators_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            URLHelpers.OpenURL("https://github.com/ShareX/ShareX/wiki/Translation");
        }

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

        private void cbRememberMainFormPosition_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.RememberMainFormPosition = cbRememberMainFormPosition.Checked;
        }

        private void cbRememberMainFormSize_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.RememberMainFormSize = cbRememberMainFormSize.Checked;
        }

        private void btnEditQuickTaskMenu_Click(object sender, EventArgs e)
        {
            new QuickTaskMenuEditorForm().ShowDialog();
        }

        #endregion General

        #region Integration

        private void cbStartWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            if (ready)
            {
                IntegrationHelpers.CreateStartupShortcut(cbStartWithWindows.Checked);
            }
        }

        private void cbShellContextMenu_CheckedChanged(object sender, EventArgs e)
        {
            if (ready)
            {
                IntegrationHelpers.CreateShellContextMenuButton(cbShellContextMenu.Checked);
            }
        }

        private void cbSendToMenu_CheckedChanged(object sender, EventArgs e)
        {
            if (ready)
            {
                IntegrationHelpers.CreateSendToMenuButton(cbSendToMenu.Checked);
            }
        }

        private void btnChromeSupport_Click(object sender, EventArgs e)
        {
            new ChromeForm().Show();
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
            Helpers.BrowseFolder(Resources.ApplicationSettingsForm_btnBrowsePersonalFolderPath_Click_Choose_ShareX_personal_folder_path, txtPersonalFolderPath, Program.PersonalFolder, true);
        }

        private void btnOpenPersonalFolder_Click(object sender, EventArgs e)
        {
            Helpers.OpenFolder(lblPreviewPersonalFolderPath.Text);
        }

        private void cbUseCustomScreenshotsPath_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.UseCustomScreenshotsPath = cbUseCustomScreenshotsPath.Checked;
            lblSaveImageSubFolderPatternPreview.Text = Program.ScreenshotsFolder;
        }

        private void txtCustomScreenshotsPath_TextChanged(object sender, EventArgs e)
        {
            Program.Settings.CustomScreenshotsPath = txtCustomScreenshotsPath.Text;
            lblSaveImageSubFolderPatternPreview.Text = Program.ScreenshotsFolder;
        }

        private void btnBrowseCustomScreenshotsPath_Click(object sender, EventArgs e)
        {
            Helpers.BrowseFolder(Resources.ApplicationSettingsForm_btnBrowseCustomScreenshotsPath_Click_Choose_screenshots_folder_path, txtCustomScreenshotsPath, Program.PersonalFolder, true);
        }

        private void txtSaveImageSubFolderPattern_TextChanged(object sender, EventArgs e)
        {
            Program.Settings.SaveImageSubFolderPattern = txtSaveImageSubFolderPattern.Text;
            lblSaveImageSubFolderPatternPreview.Text = Program.ScreenshotsFolder;
        }

        private void btnOpenScreenshotsFolder_Click(object sender, EventArgs e)
        {
            Helpers.OpenFolder(lblSaveImageSubFolderPatternPreview.Text);
        }

        #endregion Paths

        #region Export / Import

        private void cbExportSettings_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.ExportSettings = cbExportSettings.Checked;
            UpdateExportButton();
        }

        private void cbExportHistory_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.ExportHistory = cbExportHistory.Checked;
            UpdateExportButton();
        }

        private void cbExportLogs_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.ExportLogs = cbExportLogs.Checked;
            UpdateExportButton();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.DefaultExt = "sxb";
                sfd.FileName = $"ShareX-{Application.ProductVersion}-backup.sxb";
                sfd.Filter = "ShareX backup (*.sxb)|*.sxb|All files (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    btnExport.Enabled = false;
                    btnImport.Enabled = false;
                    pbExportImport.Visible = true;

                    string exportPath = sfd.FileName;

                    DebugHelper.WriteLine("Export started: " + exportPath);

                    TaskEx.Run(() =>
                    {
                        Program.SaveAllSettings();

                        ExportImportManager.Export(exportPath);
                    },
                    () =>
                    {
                        if (!IsDisposed)
                        {
                            pbExportImport.Visible = false;
                            btnExport.Enabled = true;
                            btnImport.Enabled = true;
                        }

                        DebugHelper.WriteLine("Export completed: " + exportPath);
                    });
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "ShareX backup (*.sxb)|*.sxb|All files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    btnExport.Enabled = false;
                    btnImport.Enabled = false;
                    pbExportImport.Visible = true;

                    string importPath = ofd.FileName;

                    DebugHelper.WriteLine("Import started: " + importPath);

                    TaskEx.Run(() =>
                    {
                        ExportImportManager.Import(importPath);

                        Program.LoadAllSettings();
                    },
                    () =>
                    {
                        if (!IsDisposed)
                        {
                            UpdateControls();

                            pbExportImport.Visible = false;
                            btnExport.Enabled = true;
                            btnImport.Enabled = true;
                        }

                        LanguageHelper.ChangeLanguage(Program.Settings.Language);

                        Program.MainForm.UpdateControls();

                        DebugHelper.WriteLine("Import completed: " + importPath);
                    });
                }
            }
        }

        #endregion Export / Import

        #region Proxy

        private void cbProxyMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.ProxySettings.ProxyMethod = (ProxyMethod)cbProxyMethod.SelectedIndex;

            if (Program.Settings.ProxySettings.ProxyMethod == ProxyMethod.Automatic)
            {
                Program.Settings.ProxySettings.IsValidProxy();
                txtProxyHost.Text = Program.Settings.ProxySettings.Host ?? string.Empty;
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

        #region Upload

        private void nudUploadLimit_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.UploadLimit = (int)nudUploadLimit.Value;
        }

        private void cbBufferSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Settings.BufferSizePower = cbBufferSize.SelectedIndex;
        }

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

        private void chkUseSecondaryUploaders_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.UseSecondaryUploaders = chkUseSecondaryUploaders.Checked;
            tlpBackupDestinations.Enabled = Program.Settings.UseSecondaryUploaders;
        }

        private void nudRetryUpload_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.MaxUploadFailRetry = (int)nudRetryUpload.Value;
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
            using (Image testImage = Screenshot.CaptureActiveMonitor())
            using (PrintForm printForm = new PrintForm(testImage, Program.Settings.PrintSettings, true))
            {
                printForm.ShowDialog();
            }
        }

        private void cbPrintDontShowWindowsDialog_CheckedChanged(object sender, EventArgs e)
        {
            Program.Settings.PrintSettings.ShowPrintDialog = !cbPrintDontShowWindowsDialog.Checked;
        }

        #endregion Print
    }
}